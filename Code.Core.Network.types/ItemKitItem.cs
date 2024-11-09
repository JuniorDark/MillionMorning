namespace Code.Core.Network.types;

public class ItemKitItem
{
	private readonly TemplateReference _itemTemplate;

	private readonly short _amount;

	public ItemKitItem(MessageReader reader)
	{
		_itemTemplate = new TemplateReference(reader);
		_amount = reader.ReadInt16();
	}

	public ItemKitItem(TemplateReference itemTemplate, short amount)
	{
		_itemTemplate = itemTemplate;
		_amount = amount;
	}

	public TemplateReference GetItemTemplate()
	{
		return _itemTemplate;
	}

	public short GetAmount()
	{
		return _amount;
	}

	public int Size()
	{
		return 2 + _itemTemplate.Size();
	}

	public void Write(MessageWriter writer)
	{
		_itemTemplate.Write(writer);
		writer.WriteInt16(_amount);
	}
}
