namespace Code.Core.Network.types;

public class ExitArrow
{
	private readonly string _identifier;

	private readonly float _heightOffset;

	public ExitArrow(MessageReader reader)
	{
		_identifier = reader.ReadString();
		_heightOffset = reader.ReadFloat();
	}

	public ExitArrow(string identifier, float heightOffset)
	{
		_identifier = identifier;
		_heightOffset = heightOffset;
	}

	public string GetIdentifier()
	{
		return _identifier;
	}

	public float GetHeightOffset()
	{
		return _heightOffset;
	}

	public int Size()
	{
		return 6 + MessageWriter.GetSize(_identifier);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_identifier);
		writer.WriteFloat(_heightOffset);
	}
}
