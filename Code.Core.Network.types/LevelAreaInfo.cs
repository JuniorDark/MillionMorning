namespace Code.Core.Network.types;

public class LevelAreaInfo
{
	private readonly string _fullLevelName;

	private readonly string _areaDisplayName;

	private readonly vector3 _position;

	private readonly sbyte _completed;

	public LevelAreaInfo(MessageReader reader)
	{
		_fullLevelName = reader.ReadString();
		_areaDisplayName = reader.ReadString();
		if (reader.ReadInt8() == 1)
		{
			_position = new vector3(reader);
		}
		_completed = reader.ReadInt8();
	}

	public LevelAreaInfo(string fullLevelName, string areaDisplayName, vector3 position, sbyte completed)
	{
		_fullLevelName = fullLevelName;
		_areaDisplayName = areaDisplayName;
		_position = position;
		_completed = completed;
	}

	public string GetFullLevelName()
	{
		return _fullLevelName;
	}

	public string GetAreaDisplayName()
	{
		return _areaDisplayName;
	}

	public vector3 GetPosition()
	{
		return _position;
	}

	public sbyte GetCompleted()
	{
		return _completed;
	}

	public int Size()
	{
		int num = 6;
		num += MessageWriter.GetSize(_fullLevelName);
		num += MessageWriter.GetSize(_areaDisplayName);
		if (_position != null)
		{
			num += 12;
		}
		return num;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_fullLevelName);
		writer.WriteString(_areaDisplayName);
		if (_position == null)
		{
			writer.WriteInt8(0);
		}
		else
		{
			writer.WriteInt8(1);
			_position.Write(writer);
		}
		writer.WriteInt8(_completed);
	}
}
