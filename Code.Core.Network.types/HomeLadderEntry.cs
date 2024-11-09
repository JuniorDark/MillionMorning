namespace Code.Core.Network.types;

public class HomeLadderEntry : LadderEntry
{
	public new class Factory : LadderEntry.Factory
	{
		public override LadderEntry Create(MessageReader reader)
		{
			return new HomeLadderEntry(reader);
		}
	}

	private readonly string _homeName;

	private readonly string _playerName;

	private const int TYPE_ID = 1;

	public override int GetTypeId()
	{
		return 1;
	}

	public HomeLadderEntry(MessageReader reader)
		: base(reader)
	{
		_homeName = reader.ReadString();
		_playerName = reader.ReadString();
	}

	public HomeLadderEntry(string homeName, string playerName, float score, int identifier)
		: base(score, identifier)
	{
		_homeName = homeName;
		_playerName = playerName;
	}

	public string GetHomeName()
	{
		return _homeName;
	}

	public string GetPlayerName()
	{
		return _playerName;
	}

	public override int Size()
	{
		return 12 + MessageWriter.GetSize(_homeName) + MessageWriter.GetSize(_playerName);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_homeName);
		writer.WriteString(_playerName);
	}
}
