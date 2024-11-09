using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.World.Level.LevelInfo;
using Code.World.Quest.Conditions;

namespace Code.World.GUI.QuestLog.Condition_Widgets;

public sealed class MilMo_QuestConditionWidget_ArrivesAt : MilMo_QuestConditionWidget
{
	public MilMo_QuestConditionWidget_ArrivesAt(MilMo_UserInterface ui, MilMo_ArrivesAt condition)
		: base(ui, condition)
	{
		MilMo_LevelInfoData levelInfoData = MilMo_LevelInfo.GetLevelInfoData(condition.FullLevelName);
		MilMo_LocString copy = MilMo_Localization.GetLocString("QuestLog_9349").GetCopy();
		copy.SetFormatArgs(condition.AreaDisplayName);
		MTextWidget.SetText(copy);
		AddLeftSideIcon(levelInfoData.IconPath, prefixStandardGuiPath: false);
	}
}
