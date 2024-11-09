namespace Code.Core.Network.types;

public class SkillMode
{
	private readonly string _name;

	private readonly string _desc;

	private readonly string _icon;

	private readonly float _cooldown;

	public SkillMode(MessageReader reader)
	{
		_name = reader.ReadString();
		_desc = reader.ReadString();
		_icon = reader.ReadString();
		_cooldown = reader.ReadFloat();
	}

	public SkillMode(string name, string desc, string icon, float cooldown)
	{
		_name = name;
		_desc = desc;
		_icon = icon;
		_cooldown = cooldown;
	}

	public string GetName()
	{
		return _name;
	}

	public string GetDesc()
	{
		return _desc;
	}

	public string GetIcon()
	{
		return _icon;
	}

	public float GetCooldown()
	{
		return _cooldown;
	}

	public int Size()
	{
		return 10 + MessageWriter.GetSize(_name) + MessageWriter.GetSize(_desc) + MessageWriter.GetSize(_icon);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_name);
		writer.WriteString(_desc);
		writer.WriteString(_icon);
		writer.WriteFloat(_cooldown);
	}
}
