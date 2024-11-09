using System.Collections.Generic;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.Quest.Conditions;

public sealed class MilMo_ArrivesAtAny : MilMo_QuestCondition
{
	public sealed class ArrivesAtAreaInfo
	{
		public string FullLevelName { get; }

		public MilMo_LocString AreaDisplayName { get; }

		private Vector3 Position { get; }

		private bool Visited { get; }

		public ArrivesAtAreaInfo(string fullLevelName, MilMo_LocString areaDisplayName, Vector3 position, bool visited)
		{
			FullLevelName = fullLevelName;
			AreaDisplayName = areaDisplayName;
			Position = position;
			Visited = visited;
		}
	}

	public IList<ArrivesAtAreaInfo> Areas { get; }

	private int NumberToVisit { get; }

	public MilMo_ArrivesAtAny(ConditionArrivesAtAny condition)
		: base(condition)
	{
		Areas = new List<ArrivesAtAreaInfo>();
		NumberToVisit = condition.GetNumberToVisit();
		foreach (LevelAreaInfo aera in condition.GetAeras())
		{
			Vector3 position = Vector3.zero;
			if (aera.GetPosition() != null)
			{
				position = new Vector3(aera.GetPosition().GetX(), aera.GetPosition().GetY(), aera.GetPosition().GetZ());
			}
			Areas.Add(new ArrivesAtAreaInfo(aera.GetFullLevelName(), MilMo_Localization.GetLocString(aera.GetAreaDisplayName()), position, aera.GetCompleted() == 1));
		}
	}

	public override string ToString()
	{
		return base.ToString() + " ArrivesAtAny{NumberToVisit=" + NumberToVisit + " Areas.Count=" + Areas.Count + "}";
	}
}
