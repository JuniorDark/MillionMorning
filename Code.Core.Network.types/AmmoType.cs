namespace Code.Core.Network.types;

public class AmmoType
{
	private readonly string _type;

	private readonly int _amount;

	public AmmoType(MessageReader reader)
	{
		_type = reader.ReadString();
		_amount = reader.ReadInt32();
	}

	public AmmoType(string type, int amount)
	{
		_type = type;
		_amount = amount;
	}

	public string GetCategoryType()
	{
		return _type;
	}

	public int GetAmount()
	{
		return _amount;
	}

	public int Size()
	{
		return 6 + MessageWriter.GetSize(_type);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_type);
		writer.WriteInt32(_amount);
	}
}
