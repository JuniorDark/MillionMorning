namespace Code.Core.Network.types;

public class ConditionWear : Condition
{
	public new class Factory : Condition.Factory
	{
		public override Condition Create(MessageReader reader)
		{
			return new ConditionWear(reader);
		}
	}

	private readonly string _item;

	private const int TYPE_ID = 7;

	public override int GetTypeId()
	{
		return 7;
	}

	public ConditionWear(MessageReader reader)
		: base(reader)
	{
		_item = reader.ReadString();
	}

	public ConditionWear(string item, sbyte completed, sbyte active)
		: base(completed, active)
	{
		_item = item;
	}

	public string GetItem()
	{
		return _item;
	}

	public override int Size()
	{
		return 4 + MessageWriter.GetSize(_item);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_item);
	}
}
