using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.World.Quest.Conditions;

namespace Code.World.GUI.QuestLog.Condition_Widgets;

public sealed class MilMo_QuestConditionWidget_Killed : MilMo_QuestConditionWidget
{
	public MilMo_QuestConditionWidget_Killed(MilMo_UserInterface ui, MilMo_Killed condition)
		: base(ui, condition)
	{
		MilMo_LocString copy = MilMo_Localization.GetLocString("QuestLog_9354").GetCopy();
		copy.SetFormatArgs(condition.AmountToKill, MilMo_Localization.GetLocString(condition.CreatureDisplayName));
		MTextWidget.SetText(copy);
		string creatureVisualRep = condition.CreatureVisualRep;
		string text = creatureVisualRep.Split('/')[^1];
		creatureVisualRep = "Batch01/Textures/Creatures/Icon" + text;
		AddLeftSideCounter(condition.AmountKilled, condition.AmountToKill);
		AddLeftSideIcon(creatureVisualRep, prefixStandardGuiPath: true);
	}
}
