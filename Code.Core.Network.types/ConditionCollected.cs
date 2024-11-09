namespace Code.Core.Network.types;

public class ConditionCollected : Condition
{
	public new class Factory : Condition.Factory
	{
		public override Condition Create(MessageReader reader)
		{
			return new ConditionCollected(reader);
		}
	}

	private readonly string _item;

	private readonly short _amount;

	private const int TYPE_ID = 6;

	public override int GetTypeId()
	{
		return 6;
	}

	public ConditionCollected(MessageReader reader)
		: base(reader)
	{
		_item = reader.ReadString();
		_amount = reader.ReadInt16();
	}

	public ConditionCollected(string item, short amount, sbyte completed, sbyte active)
		: base(completed, active)
	{
		_item = item;
		_amount = amount;
	}

	public string GetItem()
	{
		return _item;
	}

	public short GetAmount()
	{
		return _amount;
	}

	public override int Size()
	{
		return 6 + MessageWriter.GetSize(_item);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_item);
		writer.WriteInt16(_amount);
	}
}
