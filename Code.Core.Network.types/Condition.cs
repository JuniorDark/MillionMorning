namespace Code.Core.Network.types;

public class Condition
{
	public class Factory
	{
		public virtual Condition Create(MessageReader reader)
		{
			return new Condition(reader);
		}
	}

	private static readonly Factory[] ChildFactories;

	private readonly sbyte _completed;

	private readonly sbyte _active;

	private const int TYPE_ID = 0;

	static Condition()
	{
		ChildFactories = new Factory[11];
		ChildFactories[0] = new Factory();
		ChildFactories[1] = new ConditionArrivesAt.Factory();
		ChildFactories[2] = new ConditionArrivesAtAny.Factory();
		ChildFactories[3] = new ConditionCollectedAny.Factory();
		ChildFactories[4] = new ConditionCollectedGem.Factory();
		ChildFactories[5] = new ConditionKilledAny.Factory();
		ChildFactories[6] = new ConditionCollected.Factory();
		ChildFactories[7] = new ConditionWear.Factory();
		ChildFactories[8] = new ConditionKilled.Factory();
		ChildFactories[9] = new ConditionTalkTo.Factory();
		ChildFactories[10] = new ConditionTalkToAny.Factory();
	}

	public static Condition Create(int id, MessageReader reader)
	{
		return ChildFactories[id].Create(reader);
	}

	public virtual int GetTypeId()
	{
		return 0;
	}

	public Condition(MessageReader reader)
	{
		_completed = reader.ReadInt8();
		_active = reader.ReadInt8();
	}

	public Condition(sbyte completed, sbyte active)
	{
		_completed = completed;
		_active = active;
	}

	public sbyte GetCompleted()
	{
		return _completed;
	}

	public sbyte GetActive()
	{
		return _active;
	}

	public virtual int Size()
	{
		return 2;
	}

	public virtual void Write(MessageWriter writer)
	{
		writer.WriteInt8(_completed);
		writer.WriteInt8(_active);
	}
}
