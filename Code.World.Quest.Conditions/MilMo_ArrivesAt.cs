using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.Quest.Conditions;

public sealed class MilMo_ArrivesAt : MilMo_QuestCondition
{
	public string FullLevelName { get; }

	public MilMo_LocString AreaDisplayName { get; }

	private Vector3 Position { get; }

	public MilMo_ArrivesAt(ConditionArrivesAt condition)
		: base(condition)
	{
		FullLevelName = condition.GetFullLevelName();
		AreaDisplayName = MilMo_Localization.GetLocString(condition.GetAreaDisplayName());
		Position = ((condition.GetPosition() == null) ? Vector3.zero : new Vector3(condition.GetPosition().GetX(), condition.GetPosition().GetY(), condition.GetPosition().GetZ()));
	}

	public override string ToString()
	{
		return base.ToString() + " ArrivesAt{LevelName=" + FullLevelName + " Position=" + Position.ToString() + "}";
	}
}
