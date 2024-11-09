using Code.Core.Network.types;

namespace Code.World.Quest;

public class MilMo_QuestCondition
{
	public bool Completed { get; }

	public bool Active { get; }

	public MilMo_QuestCondition(Condition condition)
	{
		Completed = condition.GetCompleted() == 1;
		Active = condition.GetActive() == 1;
	}

	public override string ToString()
	{
		return "Condition{Completed=" + Completed + " Active=" + Active + "}";
	}
}
