namespace Code.Core.Network.types;

public class ScoreBoardEntry
{
	private readonly string _avatarName;

	private readonly string _playerID;

	private readonly int _kills;

	private readonly int _deaths;

	private readonly int _pvpPoints;

	private readonly int _objectives;

	public ScoreBoardEntry(MessageReader reader)
	{
		_avatarName = reader.ReadString();
		_playerID = reader.ReadString();
		_kills = reader.ReadInt32();
		_deaths = reader.ReadInt32();
		_pvpPoints = reader.ReadInt32();
		_objectives = reader.ReadInt32();
	}

	public ScoreBoardEntry(string avatarName, string playerID, int kills, int deaths, int pvpPoints, int objectives)
	{
		_avatarName = avatarName;
		_playerID = playerID;
		_kills = kills;
		_deaths = deaths;
		_pvpPoints = pvpPoints;
		_objectives = objectives;
	}

	public string GetAvatarName()
	{
		return _avatarName;
	}

	public string GetPlayerID()
	{
		return _playerID;
	}

	public int GetKills()
	{
		return _kills;
	}

	public int GetDeaths()
	{
		return _deaths;
	}

	public int GetPvpPoints()
	{
		return _pvpPoints;
	}

	public int GetObjectives()
	{
		return _objectives;
	}

	public int Size()
	{
		return 20 + MessageWriter.GetSize(_avatarName) + MessageWriter.GetSize(_playerID);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_avatarName);
		writer.WriteString(_playerID);
		writer.WriteInt32(_kills);
		writer.WriteInt32(_deaths);
		writer.WriteInt32(_pvpPoints);
		writer.WriteInt32(_objectives);
	}
}
