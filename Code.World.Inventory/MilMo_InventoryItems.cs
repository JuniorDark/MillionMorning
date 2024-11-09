using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Items;
using Core.GameEvent;

namespace Code.World.Inventory;

public class MilMo_InventoryItems
{
	public delegate void ForEachItem(MilMo_InventoryEntry e);

	private readonly List<MilMo_InventoryEntry> _items;

	public MilMo_InventoryItems()
	{
		_items = new List<MilMo_InventoryEntry>();
	}

	public void Clear()
	{
		_items.Clear();
		GameEvent.InventoryClearedEvent?.RaiseEvent();
	}

	public void DoForEach(ForEachItem callback)
	{
		foreach (MilMo_InventoryEntry item in _items)
		{
			callback(item);
		}
	}

	public void Add(MilMo_InventoryEntry item)
	{
		_items.Add(item);
	}

	public void Remove(MilMo_InventoryEntry item)
	{
		_items.Remove(item);
	}

	public bool Contains(MilMo_Item item)
	{
		return _items.Any((MilMo_InventoryEntry e) => e.Item == item);
	}

	public bool ContainsTemplate(MilMo_Item item)
	{
		return _items.Any((MilMo_InventoryEntry e) => e.Item.Template == item.Template);
	}

	public bool Contains(string identifier)
	{
		return _items.Any((MilMo_InventoryEntry e) => e.Item?.Template.Identifier == identifier);
	}

	public MilMo_InventoryEntry Get(int entryId)
	{
		return _items.FirstOrDefault((MilMo_InventoryEntry i) => i.Id == entryId);
	}

	public MilMo_InventoryEntry Get(string identifier)
	{
		return _items.FirstOrDefault((MilMo_InventoryEntry i) => i.Item?.Identifier == identifier);
	}

	public MilMo_InventoryEntry GetByTemplateIdentifier(string identifier)
	{
		return _items.FirstOrDefault((MilMo_InventoryEntry e) => e.Item.Template.Identifier == identifier);
	}

	public List<MilMo_InventoryEntry> GetAllByType(Type t)
	{
		return _items.Where((MilMo_InventoryEntry i) => i.Item.GetType() == t).ToList();
	}
}
