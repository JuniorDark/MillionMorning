namespace Code.Core.Network.types;

public class RoomInfo
{
	private readonly HomeEquipment _item;

	private readonly short _itemCount;

	private readonly long _entrance;

	public RoomInfo(MessageReader reader)
	{
		_item = (HomeEquipment)Item.Create(reader.ReadTypeCode(), reader);
		_itemCount = reader.ReadInt16();
		_entrance = reader.ReadInt64();
	}

	public RoomInfo(HomeEquipment item, short itemCount, long entrance)
	{
		_item = item;
		_itemCount = itemCount;
		_entrance = entrance;
	}

	public HomeEquipment GetItem()
	{
		return _item;
	}

	public short GetItemCount()
	{
		return _itemCount;
	}

	public long GetEntrance()
	{
		return _entrance;
	}

	public int Size()
	{
		return 12 + _item.Size();
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteTypeCode(_item.GetTypeId());
		_item.Write(writer);
		writer.WriteInt16(_itemCount);
		writer.WriteInt64(_entrance);
	}
}
