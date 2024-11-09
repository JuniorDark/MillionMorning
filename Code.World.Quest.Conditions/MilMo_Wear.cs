using Code.Core.Network.types;

namespace Code.World.Quest.Conditions;

public sealed class MilMo_Wear : MilMo_QuestCondition
{
	public string Item { get; }

	public MilMo_Wear(ConditionWear condition)
		: base(condition)
	{
		Item = condition.GetItem();
	}

	public override string ToString()
	{
		return base.ToString() + " Wear{Item=" + Item + "}";
	}
}
