using System.Collections.Generic;

namespace Code.Core.Network.types;

public class ShopItems
{
	private readonly int _parentId;

	private readonly IList<ShopItem> _items;

	public ShopItems(MessageReader reader)
	{
		_parentId = reader.ReadInt32();
		_items = new List<ShopItem>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_items.Add(new ShopItem(reader));
		}
	}

	public ShopItems(int parentId, IList<ShopItem> items)
	{
		_parentId = parentId;
		_items = items;
	}

	public int GetParentId()
	{
		return _parentId;
	}

	public IList<ShopItem> GetItems()
	{
		return _items;
	}

	public int Size()
	{
		int num = 6;
		foreach (ShopItem item in _items)
		{
			num += item.Size();
		}
		return num;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteInt32(_parentId);
		writer.WriteInt16((short)_items.Count);
		foreach (ShopItem item in _items)
		{
			item.Write(writer);
		}
	}
}
