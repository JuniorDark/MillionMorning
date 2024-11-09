using Code.Core.Network.types;

namespace Code.World.Quest.Conditions;

public sealed class MilMo_Killed : MilMo_QuestCondition
{
	public string CreatureVisualRep { get; }

	public string CreatureDisplayName { get; }

	public short AmountToKill { get; }

	public short AmountKilled { get; }

	public MilMo_Killed(ConditionKilled condition)
		: base(condition)
	{
		CreatureVisualRep = condition.GetCreatureVisualRep();
		CreatureDisplayName = condition.GetCreatureDisplayName();
		AmountToKill = condition.GetAmountToKill();
		AmountKilled = condition.GetAmountKilled();
	}

	public override string ToString()
	{
		return base.ToString() + " Killed{Creature=" + CreatureDisplayName + " AmountToKill=" + AmountToKill + " AmountKilled=" + AmountKilled + "}";
	}
}
