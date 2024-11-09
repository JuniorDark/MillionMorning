namespace Code.Core.Network.types;

public class StaticGem : Token
{
	public new class Factory : Token.Factory
	{
		public override Token Create(MessageReader reader)
		{
			return new StaticGem(reader);
		}
	}

	private readonly TemplateReference _template;

	private const int TYPE_ID = 4;

	public override int GetTypeId()
	{
		return 4;
	}

	public StaticGem(MessageReader reader)
		: base(reader)
	{
		_template = new TemplateReference(reader);
	}

	public StaticGem(TemplateReference template, vector3 position, sbyte isFound)
		: base(position, isFound)
	{
		_template = template;
	}

	public TemplateReference GetTemplate()
	{
		return _template;
	}

	public override int Size()
	{
		return 13 + _template.Size();
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		_template.Write(writer);
	}
}
