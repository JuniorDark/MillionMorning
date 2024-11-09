namespace Code.Core.Network.types;

public class AttackTarget
{
	public class Factory
	{
		public virtual AttackTarget Create(MessageReader reader)
		{
			return new AttackTarget(reader);
		}
	}

	private static readonly Factory[] ChildFactories;

	private static readonly int TypeId;

	static AttackTarget()
	{
		ChildFactories = new Factory[3];
		TypeId = 0;
		ChildFactories[0] = new Factory();
		ChildFactories[1] = new PlayerTarget.Factory();
		ChildFactories[2] = new CreatureTarget.Factory();
	}

	public static AttackTarget Create(int id, MessageReader reader)
	{
		return ChildFactories[id].Create(reader);
	}

	public virtual int GetTypeId()
	{
		return TypeId;
	}

	public AttackTarget(MessageReader reader)
	{
	}

	public AttackTarget()
	{
	}

	public virtual int Size()
	{
		return 0;
	}

	public virtual void Write(MessageWriter writer)
	{
	}
}
