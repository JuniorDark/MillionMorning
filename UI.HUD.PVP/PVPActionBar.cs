using System;
using Code.Core.EventSystem;
using Code.Core.ResourceSystem;
using Code.World.Inventory;
using Code.World.Player;
using Core.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.PVP;

public class PVPActionBar : HudElement
{
	[Header("Ability")]
	[SerializeField]
	private PVPActionSlot pvpActionSlot;

	[Header("Wieldable")]
	[SerializeField]
	private Image weaponImage;

	[SerializeField]
	private Image weaponTrack;

	[SerializeField]
	private TMP_Text weaponLevel;

	[Header("FX")]
	[SerializeField]
	private PVPUpgrade pvpUpgrade;

	[SerializeField]
	private PVPCircle pvpCircle;

	private MilMo_InventoryEntry _lastWieldedEntry;

	private MilMo_GenericReaction _abilityAdded;

	private MilMo_GenericReaction _abilityRemoved;

	private MilMo_GenericReaction _wieldableChanged;

	private MilMo_Player PlayerInstance => MilMo_Player.Instance;

	private void Awake()
	{
		if (pvpActionSlot == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing pvpActionSlot");
		}
		if (weaponImage == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing weaponImage");
		}
		if (weaponTrack == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing weaponTrack");
		}
		if (weaponLevel == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing weaponLevel");
		}
		if (pvpUpgrade == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing pvpUpgrade");
		}
		if (pvpCircle == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing pvpCircle");
		}
	}

	private void OnEnable()
	{
		_abilityAdded = MilMo_EventSystem.Listen("consumable_or_ability_added", PVPAbilityAdded);
		_abilityAdded.Repeating = true;
		_abilityRemoved = MilMo_EventSystem.Listen("consumable_or_ability_removed", PVPAbilityRemoved);
		_abilityRemoved.Repeating = true;
		_wieldableChanged = MilMo_EventSystem.Listen("wieldable_changed", WieldableChanged);
		_wieldableChanged.Repeating = true;
		if (pvpActionSlot != null)
		{
			pvpActionSlot.ClearSlotItem();
		}
		RefreshPVPWeaponButton(null, null, 0);
		WieldableChanged(PlayerInstance?.EquipSlots?.CurrentItemInventoryEntry?.Item);
	}

	private void OnDisable()
	{
		MilMo_EventSystem.RemoveReaction(_abilityAdded);
		_abilityAdded = null;
		MilMo_EventSystem.RemoveReaction(_abilityRemoved);
		_abilityRemoved = null;
		MilMo_EventSystem.RemoveReaction(_wieldableChanged);
		_wieldableChanged = null;
		if (pvpActionSlot != null)
		{
			pvpActionSlot.ClearSlotItem();
		}
		RefreshPVPWeaponButton(null, null, 0);
	}

	public void OnUse()
	{
		if (pvpActionSlot != null)
		{
			pvpActionSlot.onClick?.Invoke();
		}
	}

	private void PVPAbilityAdded(object entryAsObj)
	{
		MilMo_InventoryEntry slotItemEntry = (MilMo_InventoryEntry)entryAsObj;
		if (pvpActionSlot != null)
		{
			pvpActionSlot.SetSlotItemEntry(slotItemEntry);
		}
	}

	private void PVPAbilityRemoved(object o)
	{
		if (pvpActionSlot != null)
		{
			pvpActionSlot.ClearSlotItem();
		}
	}

	private async void WieldableChanged(object inventoryEntryAsObject)
	{
		if (!(inventoryEntryAsObject is MilMo_UpgradableInventoryEntry { Item: var item, CurrentUpgradeLevel: var level, ItemTrackIcon: var trackIcon } milMo_UpgradableInventoryEntry))
		{
			_lastWieldedEntry = null;
			RefreshPVPWeaponButton(null, null, 0);
		}
		else
		{
			if (_lastWieldedEntry == milMo_UpgradableInventoryEntry && _lastWieldedEntry.Item == item)
			{
				return;
			}
			_lastWieldedEntry = milMo_UpgradableInventoryEntry;
			Texture2D weaponTexture = await item.AsyncGetIcon();
			if (weaponTexture == null)
			{
				RefreshPVPWeaponButton(null, null, level);
				return;
			}
			Texture2D texture2D = await MilMo_ResourceManager.Instance.LoadTextureAsync(trackIcon);
			if (texture2D == null)
			{
				RefreshPVPWeaponButton(weaponTexture, null, level);
			}
			else
			{
				RefreshPVPWeaponButton(weaponTexture, texture2D, level);
			}
		}
	}

	private void RefreshPVPWeaponButton(Texture2D weaponTexture, Texture2D weaponTrackTexture, int level)
	{
		if (!(weaponImage == null) && !(weaponLevel == null) && !(weaponTrack == null) && !(pvpCircle == null))
		{
			if (weaponTexture == null)
			{
				weaponImage.gameObject.SetActive(value: false);
				weaponLevel.gameObject.SetActive(value: false);
				weaponTrack.gameObject.SetActive(value: false);
				pvpCircle.SetSpeed(0);
				return;
			}
			Core.Utilities.UI.SetIcon(weaponImage, weaponTexture);
			LeanTween.scale(weaponImage.gameObject, Vector3.zero, 0f);
			LeanTween.scale(weaponImage.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBounce);
			weaponLevel.text = level.ToString();
			LeanTween.scale(weaponLevel.gameObject, Vector3.zero, 0f);
			LeanTween.scale(weaponLevel.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBounce);
			Core.Utilities.UI.SetIcon(weaponTrack, weaponTrackTexture);
			LeanTween.scale(weaponTrack.gameObject, Vector3.zero, 0f);
			LeanTween.scale(weaponTrack.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBounce);
			pvpCircle.SetSpeed(level);
			weaponImage.gameObject.SetActive(value: true);
			weaponLevel.gameObject.SetActive(value: true);
			weaponTrack.gameObject.SetActive(value: true);
		}
	}

	public async void OnInventoryItemAdded(MilMo_InventoryEntry entry)
	{
		if (entry is MilMo_UpgradableInventoryEntry upgradableEntry && !(pvpUpgrade == null))
		{
			Texture2D texture2D = await upgradableEntry.GetItem().AsyncGetIcon();
			pvpUpgrade.SetWeapon(texture2D, upgradableEntry.CurrentUpgradeLevel);
			pvpUpgrade.NewWeapon();
			upgradableEntry.OnUpgraded = (Action<MilMo_UpgradableInventoryEntry>)Delegate.Combine(upgradableEntry.OnUpgraded, new Action<MilMo_UpgradableInventoryEntry>(WeaponUpgraded));
		}
	}

	private async void WeaponUpgraded(MilMo_UpgradableInventoryEntry entry)
	{
		if (entry != null && !(pvpUpgrade == null))
		{
			Texture2D texture2D = await entry.GetItem().AsyncGetIcon();
			pvpUpgrade.SetWeapon(texture2D, entry.CurrentUpgradeLevel);
			pvpUpgrade.LevelUpWeapon();
		}
	}

	public override void SetHudVisibility(bool shouldShow)
	{
		base.gameObject.SetActive(shouldShow);
	}
}
