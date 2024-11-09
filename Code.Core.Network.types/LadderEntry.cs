namespace Code.Core.Network.types;

public class LadderEntry
{
	public class Factory
	{
		public virtual LadderEntry Create(MessageReader reader)
		{
			return new LadderEntry(reader);
		}
	}

	private static readonly Factory[] ChildFactories;

	private readonly float _score;

	private readonly int _identifier;

	private const int TYPE_ID = 0;

	static LadderEntry()
	{
		ChildFactories = new Factory[2];
		ChildFactories[0] = new Factory();
		ChildFactories[1] = new HomeLadderEntry.Factory();
	}

	public static LadderEntry Create(int id, MessageReader reader)
	{
		return ChildFactories[id].Create(reader);
	}

	public virtual int GetTypeId()
	{
		return 0;
	}

	public LadderEntry(MessageReader reader)
	{
		_score = reader.ReadFloat();
		_identifier = reader.ReadInt32();
	}

	public LadderEntry(float score, int identifier)
	{
		_score = score;
		_identifier = identifier;
	}

	public float GetScore()
	{
		return _score;
	}

	public int GetIdentifier()
	{
		return _identifier;
	}

	public virtual int Size()
	{
		return 8;
	}

	public virtual void Write(MessageWriter writer)
	{
		writer.WriteFloat(_score);
		writer.WriteInt32(_identifier);
	}
}
