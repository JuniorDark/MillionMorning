namespace Code.Core.Network.types;

public class Damage
{
	private readonly string _type;

	private readonly float _damage;

	public Damage(MessageReader reader)
	{
		_type = reader.ReadString();
		_damage = reader.ReadFloat();
	}

	public Damage(string type, float damage)
	{
		_type = type;
		_damage = damage;
	}

	public string GetTemplateType()
	{
		return _type;
	}

	public float GetDamage()
	{
		return _damage;
	}

	public int Size()
	{
		return 6 + MessageWriter.GetSize(_type);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_type);
		writer.WriteFloat(_damage);
	}
}
