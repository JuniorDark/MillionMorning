namespace Code.Core.Network.types;

public class IntStringPair
{
	private readonly int _intValue;

	private readonly string _stringValue;

	public IntStringPair(MessageReader reader)
	{
		_intValue = reader.ReadInt32();
		_stringValue = reader.ReadString();
	}

	public IntStringPair(int intValue, string stringValue)
	{
		_intValue = intValue;
		_stringValue = stringValue;
	}

	public int GetIntValue()
	{
		return _intValue;
	}

	public string GetStringValue()
	{
		return _stringValue;
	}

	public int Size()
	{
		return 6 + MessageWriter.GetSize(_stringValue);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteInt32(_intValue);
		writer.WriteString(_stringValue);
	}
}
