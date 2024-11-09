namespace Code.Core.Network.types;

public class LevelStateData
{
	private readonly string _fullLevelName;

	private readonly sbyte _guiUnlocked;

	public LevelStateData(MessageReader reader)
	{
		_fullLevelName = reader.ReadString();
		_guiUnlocked = reader.ReadInt8();
	}

	public LevelStateData(string fullLevelName, sbyte guiUnlocked)
	{
		_fullLevelName = fullLevelName;
		_guiUnlocked = guiUnlocked;
	}

	public string GetFullLevelName()
	{
		return _fullLevelName;
	}

	public sbyte GetGUIUnlocked()
	{
		return _guiUnlocked;
	}

	public int Size()
	{
		return 3 + MessageWriter.GetSize(_fullLevelName);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_fullLevelName);
		writer.WriteInt8(_guiUnlocked);
	}
}
