namespace Code.Core.Network.types;

public class WieldableFoodUseEmote
{
	private readonly sbyte _useNumber;

	private readonly string _emote;

	public WieldableFoodUseEmote(MessageReader reader)
	{
		_useNumber = reader.ReadInt8();
		_emote = reader.ReadString();
	}

	public WieldableFoodUseEmote(sbyte useNumber, string emote)
	{
		_useNumber = useNumber;
		_emote = emote;
	}

	public sbyte GetUseNumber()
	{
		return _useNumber;
	}

	public string GetEmote()
	{
		return _emote;
	}

	public int Size()
	{
		return 3 + MessageWriter.GetSize(_emote);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteInt8(_useNumber);
		writer.WriteString(_emote);
	}
}
