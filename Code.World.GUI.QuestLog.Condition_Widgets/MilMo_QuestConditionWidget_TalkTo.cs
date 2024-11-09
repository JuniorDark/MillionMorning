using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.World.Level.LevelInfo;
using Code.World.Quest.Conditions;

namespace Code.World.GUI.QuestLog.Condition_Widgets;

public sealed class MilMo_QuestConditionWidget_TalkTo : MilMo_QuestConditionWidget
{
	public MilMo_QuestConditionWidget_TalkTo(MilMo_UserInterface ui, MilMo_TalkTo condition)
		: base(ui, condition)
	{
		MilMo_LocString copy = MilMo_Localization.GetLocString("QuestLog_9355").GetCopy();
		copy.SetFormatArgs(MilMo_Localization.GetLocString(condition.NPCDisplayName));
		MTextWidget.SetText(copy);
		foreach (string fullLevelName in condition.FullLevelNames)
		{
			MilMo_LevelInfoData levelInfoData = MilMo_LevelInfo.GetLevelInfoData(fullLevelName);
			AddLeftSideIcon(levelInfoData.IconPath, prefixStandardGuiPath: false);
		}
		string text = "Content/Creatures/" + condition.NPCVisualRep;
		string text2 = text.Split('/')[^1];
		text = text.Replace(text2, "");
		text = text + "Icon" + text2 + "0";
		AddLeftSideIcon(text, prefixStandardGuiPath: false);
	}
}
