using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.World.Quest.Conditions;

public sealed class MilMo_CollectedAny : MilMo_QuestCondition
{
	public sealed class MilMo_CollectedInfo
	{
		public string Item { get; }

		public int AmountToCollect { get; }

		private bool Completed { get; }

		public MilMo_CollectedInfo(string item, int amountToCollect, bool completed)
		{
			Item = item;
			AmountToCollect = amountToCollect;
			Completed = completed;
		}
	}

	public IList<MilMo_CollectedInfo> Items { get; }

	private int NumberToCollect { get; }

	public MilMo_CollectedAny(ConditionCollectedAny condition)
		: base(condition)
	{
		Items = new List<MilMo_CollectedInfo>();
		NumberToCollect = condition.GetNumberToCollect();
		foreach (CollectedInfo collectedItem in condition.GetCollectedItems())
		{
			Items.Add(new MilMo_CollectedInfo(collectedItem.GetItem(), collectedItem.GetAmountToCollect(), collectedItem.GetCompleted() == 1));
		}
	}

	public override string ToString()
	{
		return base.ToString() + " CollectedAny{NumberToCollect=" + NumberToCollect + " Items.Count=" + Items.Count + "}";
	}
}
