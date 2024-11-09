namespace Code.Core.Network.types;

public class EntityStateEffectReference
{
	private readonly TemplateReference _template;

	private readonly float _modifier;

	private readonly int _id;

	private readonly sbyte _isPermanent;

	public EntityStateEffectReference(MessageReader reader)
	{
		_template = new TemplateReference(reader);
		_modifier = reader.ReadFloat();
		_id = reader.ReadInt32();
		_isPermanent = reader.ReadInt8();
	}

	public EntityStateEffectReference(TemplateReference template, float modifier, int id, sbyte isPermanent)
	{
		_template = template;
		_modifier = modifier;
		_id = id;
		_isPermanent = isPermanent;
	}

	public TemplateReference GetTemplate()
	{
		return _template;
	}

	public float GetModifier()
	{
		return _modifier;
	}

	public int GetId()
	{
		return _id;
	}

	public sbyte GetIsPermanent()
	{
		return _isPermanent;
	}

	public int Size()
	{
		return 9 + _template.Size();
	}

	public void Write(MessageWriter writer)
	{
		_template.Write(writer);
		writer.WriteFloat(_modifier);
		writer.WriteInt32(_id);
		writer.WriteInt8(_isPermanent);
	}
}
