namespace Code.Core.Network.types;

public class ConditionCollectedGem : Condition
{
	public new class Factory : Condition.Factory
	{
		public override Condition Create(MessageReader reader)
		{
			return new ConditionCollectedGem(reader);
		}
	}

	private readonly int _amount;

	private const int TYPE_ID = 4;

	public override int GetTypeId()
	{
		return 4;
	}

	public ConditionCollectedGem(MessageReader reader)
		: base(reader)
	{
		_amount = reader.ReadInt32();
	}

	public ConditionCollectedGem(int amount, sbyte completed, sbyte active)
		: base(completed, active)
	{
		_amount = amount;
	}

	public int GetAmount()
	{
		return _amount;
	}

	public override int Size()
	{
		return 6;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt32(_amount);
	}
}
