namespace Code.Core.Network.types;

public class NpcTemplate : Template
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new NpcTemplate(reader);
		}
	}

	private readonly string _visualRep;

	private readonly string _name;

	private const int TYPE_ID = 13;

	public override int GetTypeId()
	{
		return 13;
	}

	public NpcTemplate(MessageReader reader)
		: base(reader)
	{
		_visualRep = reader.ReadString();
		_name = reader.ReadString();
	}

	public NpcTemplate(string visualRep, string name, string type, TemplateReference reference)
		: base(type, reference)
	{
		_visualRep = visualRep;
		_name = name;
	}

	public string GetVisualRep()
	{
		return _visualRep;
	}

	public string GetName()
	{
		return _name;
	}

	public override int Size()
	{
		return 4 + base.Size() + MessageWriter.GetSize(_visualRep) + MessageWriter.GetSize(_name);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_visualRep);
		writer.WriteString(_name);
	}
}
