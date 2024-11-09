using Code.Core.Network.types;

namespace Code.World.Quest.Conditions;

public sealed class MilMo_Collected : MilMo_QuestCondition
{
	public string Item { get; }

	public int Amount { get; }

	public MilMo_Collected(ConditionCollected condition)
		: base(condition)
	{
		Item = condition.GetItem();
		Amount = condition.GetAmount();
	}

	public override string ToString()
	{
		return base.ToString() + " Collected{Item=" + Item + " Amount=" + Amount + "}";
	}
}
