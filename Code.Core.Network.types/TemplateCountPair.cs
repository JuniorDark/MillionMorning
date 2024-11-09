namespace Code.Core.Network.types;

public class TemplateCountPair
{
	private readonly TemplateReference _template;

	private readonly int _count;

	public TemplateCountPair(MessageReader reader)
	{
		_template = new TemplateReference(reader);
		_count = reader.ReadInt32();
	}

	public TemplateCountPair(TemplateReference template, int count)
	{
		_template = template;
		_count = count;
	}

	public TemplateReference GetTemplate()
	{
		return _template;
	}

	public int GetCount()
	{
		return _count;
	}

	public int Size()
	{
		return 4 + _template.Size();
	}

	public void Write(MessageWriter writer)
	{
		_template.Write(writer);
		writer.WriteInt32(_count);
	}
}
