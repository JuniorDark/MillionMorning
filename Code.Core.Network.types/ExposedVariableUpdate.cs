namespace Code.Core.Network.types;

public class ExposedVariableUpdate
{
	private readonly string _name;

	private readonly float _value;

	public ExposedVariableUpdate(MessageReader reader)
	{
		_name = reader.ReadString();
		_value = reader.ReadFloat();
	}

	public ExposedVariableUpdate(string name, float value)
	{
		_name = name;
		_value = value;
	}

	public string GetName()
	{
		return _name;
	}

	public float GetValue()
	{
		return _value;
	}

	public int Size()
	{
		return 6 + MessageWriter.GetSize(_name);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_name);
		writer.WriteFloat(_value);
	}
}
