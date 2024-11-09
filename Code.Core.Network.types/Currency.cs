namespace Code.Core.Network.types;

public class Currency
{
	private readonly string _id;

	private readonly float _exchangeRate;

	public Currency(MessageReader reader)
	{
		_id = reader.ReadString();
		_exchangeRate = reader.ReadFloat();
	}

	public Currency(string id, float exchangeRate)
	{
		_id = id;
		_exchangeRate = exchangeRate;
	}

	public string GetId()
	{
		return _id;
	}

	public float GetExchangeRate()
	{
		return _exchangeRate;
	}

	public int Size()
	{
		return 6 + MessageWriter.GetSize(_id);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_id);
		writer.WriteFloat(_exchangeRate);
	}
}
