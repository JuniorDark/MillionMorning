using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.World.Quest.Conditions;

public sealed class MilMo_TalkToAny : MilMo_QuestCondition
{
	public sealed class MilMo_TalkToInfo
	{
		private string _mNPCVisualRep;

		private string _mNPCDisplayName;

		public string NPCTemplateIdentifier { get; }

		public IList<string> FullLevelNames { get; }

		private bool TalkedTo { get; }

		public MilMo_TalkToInfo(string npcTemplateIdentifier, IList<string> fullLevelNames, bool talkedTo)
		{
			NPCTemplateIdentifier = npcTemplateIdentifier;
			FullLevelNames = fullLevelNames;
			TalkedTo = talkedTo;
		}
	}

	public IList<MilMo_TalkToInfo> NpCs { get; }

	private short NumberToTalkTo { get; }

	public MilMo_TalkToAny(ConditionTalkToAny condition)
		: base(condition)
	{
		NpCs = new List<MilMo_TalkToInfo>();
		NumberToTalkTo = condition.GetNumberToTalkTo();
		foreach (TalkToInfo nPC in condition.GetNPCS())
		{
			NpCs.Add(new MilMo_TalkToInfo(nPC.GetNpcTemplateIdentifier(), nPC.GetFullLevelNames(), nPC.GetTalkedTo() == 1));
		}
	}

	public override string ToString()
	{
		return base.ToString() + " TalkToAny{NumberToTalkTo=" + NumberToTalkTo + " NPCs.Count=" + NpCs.Count + "}";
	}
}
