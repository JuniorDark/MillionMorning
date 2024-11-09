namespace Code.Core.Network.types;

public class InventoryEntry
{
	public class Factory
	{
		public virtual InventoryEntry Create(MessageReader reader)
		{
			return new InventoryEntry(reader);
		}
	}

	private static readonly Factory[] ChildFactories;

	private readonly int _id;

	private readonly short _amount;

	private readonly sbyte _equipped;

	private readonly sbyte _favorite;

	private readonly Item _item;

	private const int TYPE_ID = 0;

	static InventoryEntry()
	{
		ChildFactories = new Factory[2];
		ChildFactories[0] = new Factory();
		ChildFactories[1] = new UpgradableInventoryEntry.Factory();
	}

	public static InventoryEntry Create(int id, MessageReader reader)
	{
		return ChildFactories[id].Create(reader);
	}

	public virtual int GetTypeId()
	{
		return 0;
	}

	public InventoryEntry(MessageReader reader)
	{
		_id = reader.ReadInt32();
		_amount = reader.ReadInt16();
		_equipped = reader.ReadInt8();
		_favorite = reader.ReadInt8();
		_item = Item.Create(reader.ReadTypeCode(), reader);
	}

	public InventoryEntry(int id, short amount, sbyte equipped, sbyte favorite, Item item)
	{
		_id = id;
		_amount = amount;
		_equipped = equipped;
		_favorite = favorite;
		_item = item;
	}

	public int GetId()
	{
		return _id;
	}

	public short GetAmount()
	{
		return _amount;
	}

	public sbyte GetEquipped()
	{
		return _equipped;
	}

	public sbyte GetFavorite()
	{
		return _favorite;
	}

	public Item GetItem()
	{
		return _item;
	}

	public virtual int Size()
	{
		return 10 + _item.Size();
	}

	public virtual void Write(MessageWriter writer)
	{
		writer.WriteInt32(_id);
		writer.WriteInt16(_amount);
		writer.WriteInt8(_equipped);
		writer.WriteInt8(_favorite);
		writer.WriteTypeCode(_item.GetTypeId());
		_item.Write(writer);
	}
}
