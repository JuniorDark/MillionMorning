using System;
using System.Collections.Generic;
using System.Linq;
using Code.World.Inventory;
using Core.GameEvent;
using UI.Elements.Slot;
using UnityEngine;

namespace UI.Inventory;

public class Inventory : MonoBehaviour
{
	[SerializeField]
	private List<MilMo_InventoryEntry> entries;

	public event Action<MilMo_InventoryEntry> OnEntryAdded;

	public event Action<MilMo_InventoryEntry> OnEntryRemoved;

	public event Action OnEntriesChanged;

	public event Action OnEntriesCleared;

	private void Awake()
	{
		entries = new List<MilMo_InventoryEntry>();
		GameEvent.InventoryItemAddedEvent.RegisterAction(AddEntry);
		GameEvent.InventoryItemRemovedEvent.RegisterAction(RemoveEntry);
		GameEvent.InventoryClearedEvent.RegisterAction(ClearEntries);
	}

	private void OnDestroy()
	{
		GameEvent.InventoryItemAddedEvent.UnregisterAction(AddEntry);
		GameEvent.InventoryItemRemovedEvent.UnregisterAction(RemoveEntry);
		GameEvent.InventoryClearedEvent.UnregisterAction(ClearEntries);
	}

	private void AddEntry(MilMo_InventoryEntry entry)
	{
		if (entry != null && !entries.Contains(entry))
		{
			entry.Init();
			entries.Add(entry);
			this.OnEntryAdded?.Invoke(entry);
			this.OnEntriesChanged?.Invoke();
		}
	}

	private void RemoveEntry(MilMo_InventoryEntry entry)
	{
		if (entry != null && entries.Contains(entry))
		{
			entries.Remove(entry);
			this.OnEntryRemoved?.Invoke(entry);
			this.OnEntriesChanged?.Invoke();
		}
	}

	private void ClearEntries()
	{
		entries.Clear();
		this.OnEntriesCleared?.Invoke();
		this.OnEntriesChanged?.Invoke();
	}

	public List<MilMo_InventoryEntry> GetAllEntriesOfCategory(MilMo_InventoryEntry.InventoryCategory cat)
	{
		return entries?.Where((MilMo_InventoryEntry entry) => entry.Category == cat).ToList();
	}

	public ISlotItemEntry GetEntryById(string id)
	{
		if (!int.TryParse(id, out var identifier))
		{
			return null;
		}
		return entries?.FirstOrDefault((MilMo_InventoryEntry entry) => entry.GetId() == identifier);
	}

	public ISlotItemEntry GetEntryByTemplateIdentifier(string templateIdentifier)
	{
		return entries?.FirstOrDefault((MilMo_InventoryEntry entry) => entry.Item?.Template?.Identifier == templateIdentifier);
	}

	public static Inventory GetPlayerInventory()
	{
		GameObject gameObject = GameObject.FindWithTag("Player");
		if (!(gameObject == null))
		{
			return gameObject.GetComponent<Inventory>();
		}
		return null;
	}
}
