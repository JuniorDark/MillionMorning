namespace Code.Core.Network.types;

public class BoxLoot
{
	private readonly TemplateReference _template;

	private readonly int _amount;

	public BoxLoot(MessageReader reader)
	{
		_template = new TemplateReference(reader);
		_amount = reader.ReadInt32();
	}

	public BoxLoot(TemplateReference template, int amount)
	{
		_template = template;
		_amount = amount;
	}

	public TemplateReference GetTemplate()
	{
		return _template;
	}

	public int GetAmount()
	{
		return _amount;
	}

	public int Size()
	{
		return 4 + _template.Size();
	}

	public void Write(MessageWriter writer)
	{
		_template.Write(writer);
		writer.WriteInt32(_amount);
	}
}
