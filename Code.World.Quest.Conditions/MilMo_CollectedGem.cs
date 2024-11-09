using Code.Core.Network.types;

namespace Code.World.Quest.Conditions;

public sealed class MilMo_CollectedGem : MilMo_QuestCondition
{
	public int Amount { get; }

	public MilMo_CollectedGem(ConditionCollectedGem condition)
		: base(condition)
	{
		Amount = condition.GetAmount();
	}

	public override string ToString()
	{
		return base.ToString() + " CollectedGem{Amount=" + Amount + "}";
	}
}
