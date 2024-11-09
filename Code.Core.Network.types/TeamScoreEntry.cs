using Code.Core.Network.messages.server.PVP;

namespace Code.Core.Network.types;

public class TeamScoreEntry
{
	private readonly NetworkTeam _team;

	private readonly int _roundsWon;

	private readonly int _roundScore;

	private readonly int _kills;

	private readonly int _deaths;

	public TeamScoreEntry(MessageReader reader)
	{
		_team = new NetworkTeam(reader);
		_roundsWon = reader.ReadInt32();
		_roundScore = reader.ReadInt32();
		_kills = reader.ReadInt32();
		_deaths = reader.ReadInt32();
	}

	public TeamScoreEntry(NetworkTeam team, int roundsWon, int roundScore, int kills, int deaths)
	{
		_team = team;
		_roundsWon = roundsWon;
		_roundScore = roundScore;
		_kills = kills;
		_deaths = deaths;
	}

	public NetworkTeam GetTeam()
	{
		return _team;
	}

	public int GetRoundsWon()
	{
		return _roundsWon;
	}

	public int GetRoundScore()
	{
		return _roundScore;
	}

	public int GetKills()
	{
		return _kills;
	}

	public int GetDeaths()
	{
		return _deaths;
	}

	public int Size()
	{
		return 16 + _team.size();
	}

	public void Write(MessageWriter writer)
	{
		_team.write(writer);
		writer.WriteInt32(_roundsWon);
		writer.WriteInt32(_roundScore);
		writer.WriteInt32(_kills);
		writer.WriteInt32(_deaths);
	}
}
