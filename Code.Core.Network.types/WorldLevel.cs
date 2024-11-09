namespace Code.Core.Network.types;

public class WorldLevel
{
	private readonly string _world;

	private readonly string _level;

	private readonly sbyte _locked;

	public WorldLevel(MessageReader reader)
	{
		_world = reader.ReadString();
		_level = reader.ReadString();
		_locked = reader.ReadInt8();
	}

	public WorldLevel(string world, string level, sbyte locked)
	{
		_world = world;
		_level = level;
		_locked = locked;
	}

	public string GetWorld()
	{
		return _world;
	}

	public string GetLevel()
	{
		return _level;
	}

	public sbyte GetLocked()
	{
		return _locked;
	}

	public int Size()
	{
		return 5 + MessageWriter.GetSize(_world) + MessageWriter.GetSize(_level);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_world);
		writer.WriteString(_level);
		writer.WriteInt8(_locked);
	}
}
