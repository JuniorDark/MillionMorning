namespace Code.Core.Network.types;

public class NullableInt
{
	private readonly int _value;

	public NullableInt(MessageReader reader)
	{
		_value = reader.ReadInt32();
	}

	public NullableInt(int value)
	{
		_value = value;
	}

	public int GetValue()
	{
		return _value;
	}

	public int Size()
	{
		return 4;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteInt32(_value);
	}
}
