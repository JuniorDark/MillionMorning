namespace Code.Core.Network.types;

public class InGameShopItem
{
	private readonly int _id;

	private readonly string _item;

	private readonly int _price;

	private readonly sbyte _forSale;

	private readonly string _currency;

	public InGameShopItem(MessageReader reader)
	{
		_id = reader.ReadInt32();
		_item = reader.ReadString();
		_price = reader.ReadInt32();
		_forSale = reader.ReadInt8();
		_currency = reader.ReadString();
	}

	public InGameShopItem(int id, string item, int price, sbyte forSale, string currency)
	{
		_id = id;
		_item = item;
		_price = price;
		_forSale = forSale;
		_currency = currency;
	}

	public int GetId()
	{
		return _id;
	}

	public string GetItem()
	{
		return _item;
	}

	public int GetPrice()
	{
		return _price;
	}

	public sbyte GetForSale()
	{
		return _forSale;
	}

	public string GetCurrency()
	{
		return _currency;
	}

	public int Size()
	{
		return 13 + MessageWriter.GetSize(_item) + MessageWriter.GetSize(_currency);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteInt32(_id);
		writer.WriteString(_item);
		writer.WriteInt32(_price);
		writer.WriteInt8(_forSale);
		writer.WriteString(_currency);
	}
}
