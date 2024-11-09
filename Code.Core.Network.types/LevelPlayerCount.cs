namespace Code.Core.Network.types;

public class LevelPlayerCount
{
	private readonly string _world;

	private readonly string _level;

	private readonly int _players;

	public LevelPlayerCount(MessageReader reader)
	{
		_world = reader.ReadString();
		_level = reader.ReadString();
		_players = reader.ReadInt32();
	}

	public LevelPlayerCount(string world, string level, int players)
	{
		_world = world;
		_level = level;
		_players = players;
	}

	public string GetWorld()
	{
		return _world;
	}

	public string GetLevel()
	{
		return _level;
	}

	public int GetPlayers()
	{
		return _players;
	}

	public int Size()
	{
		return 8 + MessageWriter.GetSize(_world) + MessageWriter.GetSize(_level);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_world);
		writer.WriteString(_level);
		writer.WriteInt32(_players);
	}
}
