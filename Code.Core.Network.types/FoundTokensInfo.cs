namespace Code.Core.Network.types;

public class FoundTokensInfo
{
	private readonly string _level;

	private readonly sbyte _tokensAmount;

	private readonly int _tokensFound;

	public FoundTokensInfo(MessageReader reader)
	{
		_level = reader.ReadString();
		_tokensAmount = reader.ReadInt8();
		_tokensFound = reader.ReadInt32();
	}

	public FoundTokensInfo(string level, sbyte tokensAmount, int tokensFound)
	{
		_level = level;
		_tokensAmount = tokensAmount;
		_tokensFound = tokensFound;
	}

	public string GetLevel()
	{
		return _level;
	}

	public sbyte GetTokensAmount()
	{
		return _tokensAmount;
	}

	public int GetTokensFound()
	{
		return _tokensFound;
	}

	public int Size()
	{
		return 7 + MessageWriter.GetSize(_level);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_level);
		writer.WriteInt8(_tokensAmount);
		writer.WriteInt32(_tokensFound);
	}
}
