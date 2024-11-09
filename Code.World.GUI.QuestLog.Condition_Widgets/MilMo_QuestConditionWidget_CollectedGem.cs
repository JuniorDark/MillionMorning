using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.World.Quest.Conditions;
using Core.State;

namespace Code.World.GUI.QuestLog.Condition_Widgets;

public sealed class MilMo_QuestConditionWidget_CollectedGem : MilMo_QuestConditionWidget
{
	public MilMo_QuestConditionWidget_CollectedGem(MilMo_UserInterface ui, MilMo_CollectedGem condition)
		: base(ui, condition)
	{
		MilMo_LocString copy = MilMo_Localization.GetLocString("QuestLog_9353").GetCopy();
		copy.SetFormatArgs(condition.Amount.ToString());
		MTextWidget.SetText(copy);
		CreateTextWidget().SetTextNoLocalization(GlobalStates.Instance.playerState.gems.Get() + "/" + condition.Amount);
		new MilMo_Widget(ui).SetTexture("Batch01/Textures/GameDialog/IconGem");
		AddLeftSideCounter(GlobalStates.Instance.playerState.gems.Get(), condition.Amount);
		AddLeftSideIcon("Batch01/Textures/GameDialog/IconGem", prefixStandardGuiPath: true);
	}
}
