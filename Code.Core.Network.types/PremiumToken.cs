namespace Code.Core.Network.types;

public class PremiumToken : Token
{
	public new class Factory : Token.Factory
	{
		public override Token Create(MessageReader reader)
		{
			return new PremiumToken(reader);
		}
	}

	private readonly string _level;

	private readonly int _value;

	private readonly float _progress;

	private const int TYPE_ID = 3;

	public override int GetTypeId()
	{
		return 3;
	}

	public PremiumToken(MessageReader reader)
		: base(reader)
	{
		_level = reader.ReadString();
		_value = reader.ReadInt32();
		_progress = reader.ReadFloat();
	}

	public PremiumToken(string level, int value, float progress, vector3 position, sbyte isFound)
		: base(position, isFound)
	{
		_level = level;
		_value = value;
		_progress = progress;
	}

	public string GetLevel()
	{
		return _level;
	}

	public int GetValue()
	{
		return _value;
	}

	public float GetProgress()
	{
		return _progress;
	}

	public override int Size()
	{
		return 23 + MessageWriter.GetSize(_level);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_level);
		writer.WriteInt32(_value);
		writer.WriteFloat(_progress);
	}
}
