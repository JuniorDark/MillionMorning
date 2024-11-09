using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.EventSystem;
using Code.Core.Items;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.World.Inventory;
using Core.GameEvent;
using Core.State;
using UnityEngine;

namespace Player;

public class AmmoManager : MonoBehaviour
{
	private readonly Dictionary<string, MilMo_InventoryEntry> _ammoEntries = new Dictionary<string, MilMo_InventoryEntry>();

	private const string LOCALIZATION_PREFIX = "AmmoType_";

	private const string LOCALIZATION_PREFIX_DESCRIPTION = "AmmoType_Description_";

	private const string BAG_ICON_PATH = "Content/GUI/Batch01/Textures/AmmoTypes/Bag";

	private readonly Dictionary<string, MilMo_InventoryEntry.InventorySection> _ammoSections = new Dictionary<string, MilMo_InventoryEntry.InventorySection>
	{
		{
			"Arrows",
			MilMo_InventoryEntry.InventorySection.Bow
		},
		{
			"Cells",
			MilMo_InventoryEntry.InventorySection.Gun
		},
		{
			"Mana",
			MilMo_InventoryEntry.InventorySection.Wand
		}
	};

	private MilMo_GenericReaction _ammoTypes;

	private MilMo_GenericReaction _ammoUpdate;

	public event Action<MilMo_InventoryEntry> OnEntryAdded;

	public event Action<MilMo_InventoryEntry> OnEntryRemoved;

	private void OnEnable()
	{
		_ammoTypes = MilMo_EventSystem.Listen("ammo_types", GotAmmoTypes);
		_ammoTypes.Repeating = true;
		_ammoUpdate = MilMo_EventSystem.Listen("ammo_update", GotAmmoUpdate);
		_ammoUpdate.Repeating = true;
	}

	private void OnDisable()
	{
		MilMo_EventSystem.RemoveReaction(_ammoTypes);
		_ammoTypes = null;
		MilMo_EventSystem.RemoveReaction(_ammoUpdate);
		_ammoUpdate = null;
	}

	private MilMo_InventoryEntry.InventorySection GetSectionForAmmoType(string ammoType)
	{
		_ammoSections.TryGetValue(ammoType, out var value);
		return value;
	}

	public int GetAmount(string ammoType)
	{
		return GetEntry(ammoType)?.Amount ?? 0;
	}

	public List<MilMo_InventoryEntry> GetAllEntries()
	{
		return _ammoEntries.Values.ToList();
	}

	public MilMo_InventoryEntry GetEntry(string ammoType)
	{
		_ammoEntries.TryGetValue(ammoType, out var value);
		return value;
	}

	private MilMo_InventoryEntry AddEntry(string ammoType, int ammoAmount)
	{
		MilMo_InventoryEntry milMo_InventoryEntry = new MilMo_InventoryEntry();
		milMo_InventoryEntry.Amount = ammoAmount;
		milMo_InventoryEntry.Category = MilMo_InventoryEntry.InventoryCategory.Weapons;
		milMo_InventoryEntry.Section = GetSectionForAmmoType(ammoType);
		MilMo_AmmoTemplate milMo_AmmoTemplate = MilMo_AmmoTemplate.Create(milMo_InventoryEntry.Category.ToString(), ammoType, ammoType);
		milMo_AmmoTemplate.CustomIdiotIconPath = "Content/GUI/Batch01/Textures/AmmoTypes/Bag" + ammoType;
		milMo_AmmoTemplate.DisplayName = MilMo_Localization.GetLocString("AmmoType_" + ammoType);
		milMo_AmmoTemplate.Description = MilMo_Localization.GetLocString("AmmoType_Description_" + ammoType);
		milMo_InventoryEntry.Item = new MilMo_Ammo(milMo_AmmoTemplate, new Dictionary<string, string>());
		_ammoEntries.Add(ammoType, milMo_InventoryEntry);
		this.OnEntryAdded?.Invoke(milMo_InventoryEntry);
		return milMo_InventoryEntry;
	}

	private void RemoveEntry(string ammoType)
	{
		_ammoEntries.TryGetValue(ammoType, out var value);
		if (value != null)
		{
			this.OnEntryRemoved?.Invoke(value);
			_ammoEntries.Remove(ammoType);
		}
	}

	private void DestroyEntries()
	{
		foreach (string item in _ammoEntries.Keys.ToList())
		{
			RemoveEntry(item);
		}
	}

	private void GotAmmoTypes(object messageAsObject)
	{
		if (messageAsObject is ServerAmmoTypes serverAmmoTypes)
		{
			RecreateAmmo(serverAmmoTypes.getAmmoTypes());
		}
	}

	private void GotAmmoUpdate(object messageAsObject)
	{
		if (messageAsObject is ServerAmmoUpdate serverAmmoUpdate)
		{
			UpdateAmmoAmountForType(serverAmmoUpdate.getAmmoType(), serverAmmoUpdate.getAmmoAmount());
		}
	}

	public void RecreateAmmo(IEnumerable<AmmoType> types)
	{
		DestroyEntries();
		foreach (AmmoType type in types)
		{
			if (type.GetAmount() > 0)
			{
				AddEntry(type.GetCategoryType(), type.GetAmount());
			}
		}
	}

	private void UpdateAmmoAmountForType(string ammoType, int newAmount)
	{
		bool num = GlobalStates.Instance.playerState.ammoType.Get() == ammoType;
		MilMo_InventoryEntry milMo_InventoryEntry = GetEntry(ammoType) ?? AddEntry(ammoType, 0);
		int num2 = newAmount - milMo_InventoryEntry.Amount;
		milMo_InventoryEntry.Amount = newAmount;
		if (milMo_InventoryEntry.Amount == 0)
		{
			RemoveEntry(ammoType);
		}
		if (num)
		{
			GlobalStates.Instance.playerState.ammoAmount.Set(newAmount);
			if (num2 < 0)
			{
				GameEvent.AmmoSpentEvent.RaiseEvent(num2);
			}
			else
			{
				GameEvent.AmmoIncreasedEvent.RaiseEvent(num2);
			}
			string eventName = ((num2 < 0) ? "ammomanager_lost_ammo" : "ammomanager_got_ammo");
			MilMo_EventSystem.Instance.PostEvent(eventName, ammoType);
		}
	}

	public static AmmoManager GetPlayerAmmoManager()
	{
		GameObject gameObject = GameObject.FindWithTag("Player");
		if (!(gameObject == null))
		{
			return gameObject.GetComponent<AmmoManager>();
		}
		return null;
	}
}
