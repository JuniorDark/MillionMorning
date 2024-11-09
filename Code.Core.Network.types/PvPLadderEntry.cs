namespace Code.Core.Network.types;

public class PvPLadderEntry
{
	private readonly string _avatarName;

	private readonly int _playerID;

	private readonly int _rank;

	private readonly int _score;

	public PvPLadderEntry(MessageReader reader)
	{
		_avatarName = reader.ReadString();
		_playerID = reader.ReadInt32();
		_rank = reader.ReadInt32();
		_score = reader.ReadInt32();
	}

	public PvPLadderEntry(string avatarName, int playerID, int rank, int score)
	{
		_avatarName = avatarName;
		_playerID = playerID;
		_rank = rank;
		_score = score;
	}

	public string GetAvatarName()
	{
		return _avatarName;
	}

	public int GetPlayerID()
	{
		return _playerID;
	}

	public int GetRank()
	{
		return _rank;
	}

	public int GetScore()
	{
		return _score;
	}

	public int Size()
	{
		return 14 + MessageWriter.GetSize(_avatarName);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_avatarName);
		writer.WriteInt32(_playerID);
		writer.WriteInt32(_rank);
		writer.WriteInt32(_score);
	}
}
