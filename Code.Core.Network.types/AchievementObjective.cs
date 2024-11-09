namespace Code.Core.Network.types;

public class AchievementObjective
{
	private readonly string _type;

	private readonly string _obj;

	private readonly int _count;

	public AchievementObjective(MessageReader reader)
	{
		_type = reader.ReadString();
		_obj = reader.ReadString();
		_count = reader.ReadInt32();
	}

	public AchievementObjective(string type, string obj, int count)
	{
		_type = type;
		_obj = obj;
		_count = count;
	}

	public string GetTemplateType()
	{
		return _type;
	}

	public string GetObj()
	{
		return _obj;
	}

	public int GetCount()
	{
		return _count;
	}

	public int Size()
	{
		return 8 + MessageWriter.GetSize(_type) + MessageWriter.GetSize(_obj);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_type);
		writer.WriteString(_obj);
		writer.WriteInt32(_count);
	}
}
