namespace Code.Core.Network.types;

public class CreatureTarget : AttackTarget
{
	public new class Factory : AttackTarget.Factory
	{
		public override AttackTarget Create(MessageReader reader)
		{
			return new CreatureTarget(reader);
		}
	}

	private readonly int _creatureId;

	private const int TYPE_ID = 2;

	public override int GetTypeId()
	{
		return 2;
	}

	public CreatureTarget(MessageReader reader)
		: base(reader)
	{
		_creatureId = reader.ReadInt32();
	}

	public CreatureTarget(int creatureId)
	{
		_creatureId = creatureId;
	}

	public int GetCreatureId()
	{
		return _creatureId;
	}

	public override int Size()
	{
		return 4;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt32(_creatureId);
	}
}
