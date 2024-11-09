using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.World.Quest.Conditions;

public sealed class MilMo_TalkTo : MilMo_QuestCondition
{
	private string NPCTemplateIdentifier { get; }

	public string NPCVisualRep { get; }

	public string NPCDisplayName { get; }

	public IList<string> FullLevelNames { get; }

	public MilMo_TalkTo(ConditionTalkTo condition)
		: base(condition)
	{
		NPCTemplateIdentifier = condition.GetNpcTemplateIdentifier();
		NPCVisualRep = condition.GetNpcVisualRep();
		NPCDisplayName = condition.GetNpcDisplayName();
		FullLevelNames = condition.GetFullLevelNames();
	}

	public override string ToString()
	{
		return base.ToString() + " TalkTo{NPC=" + NPCTemplateIdentifier + " FullLevelNames.Count=" + FullLevelNames.Count + "}";
	}
}
