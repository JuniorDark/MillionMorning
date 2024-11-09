namespace Code.Core.Network.types;

public class PlayerStateEffectFunction
{
	private readonly string _op;

	private readonly string _value;

	public PlayerStateEffectFunction(MessageReader reader)
	{
		_op = reader.ReadString();
		_value = reader.ReadString();
	}

	public PlayerStateEffectFunction(string op, string value)
	{
		_op = op;
		_value = value;
	}

	public string GetOp()
	{
		return _op;
	}

	public string GetValue()
	{
		return _value;
	}

	public int Size()
	{
		return 4 + MessageWriter.GetSize(_op) + MessageWriter.GetSize(_value);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_op);
		writer.WriteString(_value);
	}
}
