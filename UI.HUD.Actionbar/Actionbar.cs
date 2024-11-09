using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Items;
using Code.World.Player.Skills;
using Core.GameEvent;
using Player;
using UI.Elements.Slot;
using UI.HUD.Actionbar.Hotkeys;
using UI.Inventory;
using UnityEngine;

namespace UI.HUD.Actionbar;

public class Actionbar : HudElement
{
	[SerializeField]
	private List<ActionSlot> slots;

	public const int ACTIONBAR_SLOT_1 = 0;

	public const int ACTIONBAR_SLOT_2 = 1;

	public const int ACTIONBAR_SLOT_3 = 2;

	public const int ACTIONBAR_SLOT_4 = 3;

	public const int ACTIONBAR_SLOT_5 = 4;

	private IList<string> _receivedHotkeys;

	private UI.Inventory.Inventory _inventory;

	private SkillManager _skillManager;

	private void Start()
	{
		_inventory = UI.Inventory.Inventory.GetPlayerInventory();
		_skillManager = SkillManager.GetPlayerSkillManager();
		if (_receivedHotkeys != null)
		{
			SetupHotkeys();
		}
	}

	private void OnEnable()
	{
		GameEvent.OnAbilitySlotEvent = (Action<int>)Delegate.Combine(GameEvent.OnAbilitySlotEvent, new Action<int>(PerformUseAction));
		if (_inventory != null)
		{
			_inventory.OnEntryRemoved += ActionbarRemoveSlot;
		}
		if (_skillManager != null)
		{
			_skillManager.OnEntryRemoved += ActionbarRemoveSlot;
		}
	}

	private void OnDisable()
	{
		GameEvent.OnAbilitySlotEvent = (Action<int>)Delegate.Remove(GameEvent.OnAbilitySlotEvent, new Action<int>(PerformUseAction));
		if (_inventory != null)
		{
			_inventory.OnEntryRemoved -= ActionbarRemoveSlot;
		}
		if (_skillManager != null)
		{
			_skillManager.OnEntryRemoved -= ActionbarRemoveSlot;
		}
	}

	private void OnDestroy()
	{
		if (_inventory != null)
		{
			_inventory.OnEntryRemoved -= ActionbarRemoveSlot;
		}
		if (_skillManager != null)
		{
			_skillManager.OnEntryRemoved -= ActionbarRemoveSlot;
		}
	}

	private void ActionbarRemoveSlot(ISlotItemEntry entry)
	{
		ClearActionSlotByEntry(entry.GetItem());
	}

	private void ClearActionSlotByEntry(IEntryItem entry)
	{
		ActionSlot actionSlot = slots.FirstOrDefault((ActionSlot s) => s.IsContainingEntry(entry));
		if (actionSlot != null)
		{
			actionSlot.ClearSlotItem();
		}
	}

	private void PerformUseAction(int index)
	{
		slots[index]?.onClick?.Invoke();
	}

	private void SetupHotkeys()
	{
		for (int i = 0; i < _receivedHotkeys.Count; i++)
		{
			string text = _receivedHotkeys[i];
			if (!string.IsNullOrEmpty(text))
			{
				ISlotItemEntry entry = null;
				if (GetISlotItemEntry(text, ref entry))
				{
					break;
				}
				if (entry != null)
				{
					slots[i].SetSlotItemEntry(entry);
				}
			}
		}
	}

	private bool GetISlotItemEntry(string rawHotkey, ref ISlotItemEntry entry)
	{
		string[] array = rawHotkey.Split(':');
		int num = array.Length;
		string text = array[0];
		if (num < 2)
		{
			return false;
		}
		if (text == "Item")
		{
			entry = _inventory.GetEntryById(array[1]);
		}
		else if (text == "Skill")
		{
			if (num != 4)
			{
				return true;
			}
			string text2 = array[1];
			string text3 = array[2];
			string text4 = array[3];
			string id = text + ":" + text2 + ":" + text3 + ":" + text4;
			entry = _skillManager.GetEntryBySId(id);
		}
		return false;
	}

	public void ReceiveHotkeys(IList<string> arrayOfHotkeys)
	{
		_receivedHotkeys = arrayOfHotkeys;
		if (_inventory != null)
		{
			SetupHotkeys();
		}
	}

	public void VerifySlot()
	{
	}

	public void ClearDuplicateSlots(ISlotItemEntry newEntry, HotkeyType hotkey)
	{
		foreach (ActionSlot slot in slots)
		{
			ISlotItemEntry slotItemEntry = slot.GetSlotItemEntry();
			if (slotItemEntry == null)
			{
				continue;
			}
			slotItemEntry.GetId();
			if (false || newEntry == null)
			{
				continue;
			}
			newEntry.GetId();
			if (false || slotItemEntry.GetId() != newEntry.GetId() || slot.GetHotkey() == hotkey)
			{
				continue;
			}
			if (newEntry.GetItem() is MilMo_Skill || newEntry.GetItem() is MilMo_Ability)
			{
				if (newEntry.GetItem().GetDisplayName().Identifier == slotItemEntry.GetItem().GetDisplayName().Identifier)
				{
					slot.ClearSlotItem();
				}
			}
			else
			{
				slot.ClearSlotItem();
			}
		}
	}

	public int GetActionSlotIndex(ActionSlot actionSlot)
	{
		for (int i = 0; i < slots.Count; i++)
		{
			ActionSlot actionSlot2 = slots[i];
			if ((object)actionSlot == actionSlot2)
			{
				return (sbyte)i;
			}
		}
		return -1;
	}

	public override void SetHudVisibility(bool shouldShow)
	{
		base.gameObject.SetActive(shouldShow);
	}
}
