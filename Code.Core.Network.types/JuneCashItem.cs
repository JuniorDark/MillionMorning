namespace Code.Core.Network.types;

public class JuneCashItem
{
	private readonly string _product;

	private readonly string _title;

	private readonly int _quantity;

	private readonly double _price;

	public JuneCashItem(MessageReader reader)
	{
		_product = reader.ReadString();
		_title = reader.ReadString();
		_quantity = reader.ReadInt32();
		_price = reader.ReadDouble();
	}

	public JuneCashItem(string product, string title, int quantity, double price)
	{
		_product = product;
		_title = title;
		_quantity = quantity;
		_price = price;
	}

	public string GetProduct()
	{
		return _product;
	}

	public string GetTitle()
	{
		return _title;
	}

	public int GetQuantity()
	{
		return _quantity;
	}

	public double GetPrice()
	{
		return _price;
	}

	public string GetTexture()
	{
		return "Batch01/Textures/Shop/" + _product;
	}

	public int Size()
	{
		return 16 + MessageWriter.GetSize(_product) + MessageWriter.GetSize(_title);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_product);
		writer.WriteString(_title);
		writer.WriteInt32(_quantity);
		writer.WriteDouble(_price);
	}
}
