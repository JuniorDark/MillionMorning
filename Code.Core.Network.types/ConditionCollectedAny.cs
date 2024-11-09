using System.Collections.Generic;

namespace Code.Core.Network.types;

public class ConditionCollectedAny : Condition
{
	public new class Factory : Condition.Factory
	{
		public override Condition Create(MessageReader reader)
		{
			return new ConditionCollectedAny(reader);
		}
	}

	private readonly int _numberToCollect;

	private readonly IList<CollectedInfo> _collectedItems;

	private const int TYPE_ID = 3;

	public override int GetTypeId()
	{
		return 3;
	}

	public ConditionCollectedAny(MessageReader reader)
		: base(reader)
	{
		_numberToCollect = reader.ReadInt32();
		_collectedItems = new List<CollectedInfo>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_collectedItems.Add(new CollectedInfo(reader));
		}
	}

	public ConditionCollectedAny(int numberToCollect, IList<CollectedInfo> collectedItems, sbyte completed, sbyte active)
		: base(completed, active)
	{
		_numberToCollect = numberToCollect;
		_collectedItems = collectedItems;
	}

	public int GetNumberToCollect()
	{
		return _numberToCollect;
	}

	public IList<CollectedInfo> GetCollectedItems()
	{
		return _collectedItems;
	}

	public override int Size()
	{
		int num = 8;
		foreach (CollectedInfo collectedItem in _collectedItems)
		{
			num += collectedItem.Size();
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt32(_numberToCollect);
		writer.WriteInt16((short)_collectedItems.Count);
		foreach (CollectedInfo collectedItem in _collectedItems)
		{
			collectedItem.Write(writer);
		}
	}
}
