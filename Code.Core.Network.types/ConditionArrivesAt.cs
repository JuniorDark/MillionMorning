namespace Code.Core.Network.types;

public class ConditionArrivesAt : Condition
{
	public new class Factory : Condition.Factory
	{
		public override Condition Create(MessageReader reader)
		{
			return new ConditionArrivesAt(reader);
		}
	}

	private readonly string _fullLevelName;

	private readonly string _areaDisplayName;

	private readonly vector3 _position;

	private const int TYPE_ID = 1;

	public override int GetTypeId()
	{
		return 1;
	}

	public ConditionArrivesAt(MessageReader reader)
		: base(reader)
	{
		_fullLevelName = reader.ReadString();
		_areaDisplayName = reader.ReadString();
		if (reader.ReadInt8() == 1)
		{
			_position = new vector3(reader);
		}
	}

	public ConditionArrivesAt(string fullLevelName, string areaDisplayName, vector3 position, sbyte completed, sbyte active)
		: base(completed, active)
	{
		_fullLevelName = fullLevelName;
		_areaDisplayName = areaDisplayName;
		_position = position;
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

	public override int Size()
	{
		int num = 7;
		num += MessageWriter.GetSize(_fullLevelName);
		num += MessageWriter.GetSize(_areaDisplayName);
		if (_position != null)
		{
			num += 12;
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_fullLevelName);
		writer.WriteString(_areaDisplayName);
		if (_position == null)
		{
			writer.WriteInt8(0);
			return;
		}
		writer.WriteInt8(1);
		_position.Write(writer);
	}
}
