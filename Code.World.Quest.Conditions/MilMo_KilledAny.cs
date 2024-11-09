using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.World.Quest.Conditions;

public sealed class MilMo_KilledAny : MilMo_QuestCondition
{
	public sealed class MilMo_KillInfo
	{
		public string CreatureVisualRep { get; }

		public string CreatureDisplayName { get; }

		public short AmountToKill { get; }

		public short AmountKilled { get; }

		public MilMo_KillInfo(string creatureVisualRep, string creatureDisplayName, short amountToKill, short amountKilled)
		{
			CreatureVisualRep = creatureVisualRep;
			CreatureDisplayName = creatureDisplayName;
			AmountToKill = amountToKill;
			AmountKilled = amountKilled;
		}
	}

	public IList<MilMo_KillInfo> Kills { get; }

	private int NumberToKill { get; }

	public MilMo_KilledAny(ConditionKilledAny condition)
		: base(condition)
	{
		Kills = new List<MilMo_KillInfo>();
		NumberToKill = condition.GetNumberToKill();
		foreach (KilledInfo kill in condition.GetKills())
		{
			Kills.Add(new MilMo_KillInfo(kill.GetCreatureVisualRep(), kill.GetCreatureDisplayName(), kill.GetAmountToKill(), kill.GetAmountKilled()));
		}
	}

	public override string ToString()
	{
		return base.ToString() + " KilledAny{NumberToKill=" + NumberToKill + " Kills.Count=" + Kills.Count + "}";
	}
}
