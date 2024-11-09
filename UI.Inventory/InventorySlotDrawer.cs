using System;
using System.Collections.Generic;
using System.Linq;
using Code.World.Inventory;
using Player;
using UI.Elements.Slot;
using UnityEngine;

namespace UI.Inventory;

public class InventorySlotDrawer : SlotDrawer
{
	[Header("Category")]
	[SerializeField]
	protected MilMo_InventoryEntry.InventoryCategory category;

	private Inventory _playerInventory;

	private AmmoManager _ammoManager;

	protected void Start()
	{
		_playerInventory = Inventory.GetPlayerInventory();
		if (_playerInventory == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing player inventory!");
			return;
		}
		if (category == MilMo_InventoryEntry.InventoryCategory.Weapons)
		{
			_ammoManager = AmmoManager.GetPlayerAmmoManager();
			if (_ammoManager == null)
			{
				Debug.LogError(base.gameObject.name + ": Missing ammo manager!");
				return;
			}
		}
		if (_playerInventory != null)
		{
			_playerInventory.OnEntryAdded += AddSlot;
			_playerInventory.OnEntryRemoved += RemoveSlot;
			_playerInventory.OnEntriesCleared += Redraw;
		}
		if (_ammoManager != null)
		{
			_ammoManager.OnEntryAdded += AddSlot;
			_ammoManager.OnEntryRemoved += RemoveSlot;
		}
		Redraw();
	}

	protected void OnDestroy()
	{
		if (_playerInventory != null)
		{
			_playerInventory.OnEntryAdded -= AddSlot;
			_playerInventory.OnEntryRemoved -= RemoveSlot;
			_playerInventory.OnEntriesCleared -= Redraw;
		}
		if (_ammoManager != null)
		{
			_ammoManager.OnEntryAdded += AddSlot;
			_ammoManager.OnEntryRemoved += RemoveSlot;
		}
	}

	protected override void AddSlot(ISlotItemEntry entry)
	{
		if ((MilMo_InventoryEntry.InventoryCategory)(object)entry.GetCategory() == category)
		{
			base.AddSlot(entry);
		}
	}

	protected override void RemoveSlot(ISlotItemEntry entry)
	{
		if ((MilMo_InventoryEntry.InventoryCategory)(object)entry.GetCategory() == category)
		{
			base.RemoveSlot(entry);
		}
	}

	protected override string GetSectionLocaleKey(Enum sectionIdentifier)
	{
		MilMo_InventoryEntry.InventorySection key = (MilMo_InventoryEntry.InventorySection)(object)sectionIdentifier;
		if (!MilMo_InventoryEntry.InventorySectionLocales.TryGetValue(key, out var value))
		{
			return "";
		}
		return value;
	}

	private void Redraw()
	{
		DestroyContent();
		if (_playerInventory != null)
		{
			GetInventoryEntries(out var entries);
			List<ISlotItemEntry> entries2 = entries.OfType<ISlotItemEntry>().ToList();
			AddSlots(entries2);
		}
		if (_ammoManager != null)
		{
			GetAmmoEntries(out var entries3);
			List<ISlotItemEntry> entries4 = entries3.OfType<ISlotItemEntry>().ToList();
			AddSlots(entries4);
		}
	}

	private void GetInventoryEntries(out List<MilMo_InventoryEntry> entries)
	{
		entries = ((_playerInventory != null) ? _playerInventory.GetAllEntriesOfCategory(category) : null);
	}

	private void GetAmmoEntries(out List<MilMo_InventoryEntry> entries)
	{
		entries = ((_ammoManager != null) ? _ammoManager.GetAllEntries() : null);
	}
}
