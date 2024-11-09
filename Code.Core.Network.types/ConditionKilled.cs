namespace Code.Core.Network.types;

public class ConditionKilled : Condition
{
	public new class Factory : Condition.Factory
	{
		public override Condition Create(MessageReader reader)
		{
			return new ConditionKilled(reader);
		}
	}

	private readonly string _creatureVisualRep;

	private readonly string _creatureDisplayName;

	private readonly short _amountToKill;

	private readonly short _amountKilled;

	private const int TYPE_ID = 8;

	public override int GetTypeId()
	{
		return 8;
	}

	public ConditionKilled(MessageReader reader)
		: base(reader)
	{
		_creatureVisualRep = reader.ReadString();
		_creatureDisplayName = reader.ReadString();
		_amountToKill = reader.ReadInt16();
		_amountKilled = reader.ReadInt16();
	}

	public ConditionKilled(string creatureVisualRep, string creatureDisplayName, short amountToKill, short amountKilled, sbyte completed, sbyte active)
		: base(completed, active)
	{
		_creatureVisualRep = creatureVisualRep;
		_creatureDisplayName = creatureDisplayName;
		_amountToKill = amountToKill;
		_amountKilled = amountKilled;
	}

	public string GetCreatureVisualRep()
	{
		return _creatureVisualRep;
	}

	public string GetCreatureDisplayName()
	{
		return _creatureDisplayName;
	}

	public short GetAmountToKill()
	{
		return _amountToKill;
	}

	public short GetAmountKilled()
	{
		return _amountKilled;
	}

	public override int Size()
	{
		return 10 + MessageWriter.GetSize(_creatureVisualRep) + MessageWriter.GetSize(_creatureDisplayName);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_creatureVisualRep);
		writer.WriteString(_creatureDisplayName);
		writer.WriteInt16(_amountToKill);
		writer.WriteInt16(_amountKilled);
	}
}
