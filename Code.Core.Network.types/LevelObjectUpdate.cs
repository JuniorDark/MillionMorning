namespace Code.Core.Network.types;

public class LevelObjectUpdate
{
	public class Factory
	{
		public virtual LevelObjectUpdate Create(MessageReader reader)
		{
			return new LevelObjectUpdate(reader);
		}
	}

	private static readonly Factory[] ChildFactories;

	private readonly int _id;

	private const int TYPE_ID = 0;

	static LevelObjectUpdate()
	{
		ChildFactories = new Factory[2];
		ChildFactories[0] = new Factory();
		ChildFactories[1] = new CreatureUpdate.Factory();
	}

	public static LevelObjectUpdate Create(int id, MessageReader reader)
	{
		return ChildFactories[id].Create(reader);
	}

	public virtual int GetTypeId()
	{
		return 0;
	}

	public LevelObjectUpdate(MessageReader reader)
	{
		_id = reader.ReadInt32();
	}

	public LevelObjectUpdate(int id)
	{
		_id = id;
	}

	public int GetId()
	{
		return _id;
	}

	public virtual int Size()
	{
		return 4;
	}

	public virtual void Write(MessageWriter writer)
	{
		writer.WriteInt32(_id);
	}
}
