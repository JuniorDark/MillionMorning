namespace Code.Core.Network.types;

public class NullableString
{
	private readonly string _value;

	public NullableString(MessageReader reader)
	{
		_value = reader.ReadString();
	}

	public NullableString(string value)
	{
		_value = value;
	}

	public string GetValue()
	{
		return _value;
	}

	public int Size()
	{
		return 2 + MessageWriter.GetSize(_value);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_value);
	}
}
