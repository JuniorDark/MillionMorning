using System.Collections.Generic;
using System.Linq;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.World.Quest;

namespace Code.World.GUI.QuestLog.Condition_Widgets;

public class MilMo_QuestConditionWidget : MilMo_Widget
{
	private readonly MilMo_Widget _mCheckMarkWidget;

	protected readonly MilMo_Widget MTextWidget;

	protected readonly List<MilMo_Widget> MLeftSideWidgets;

	protected MilMo_TimerEvent MTickEvent;

	protected MilMo_QuestConditionWidget(MilMo_UserInterface ui, MilMo_QuestCondition condition)
		: base(ui)
	{
		ScaleNow(305f, 35f);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		MTextWidget = new MilMo_Widget(UI);
		MTextWidget.SetAlignment(MilMo_GUI.Align.TopLeft);
		MTextWidget.SetPosition(30f, 7f);
		MTextWidget.SetScale(200f, 21f);
		MTextWidget.SetFont(MilMo_GUI.Font.EborgSmall);
		MTextWidget.SetFontScale(0.75f);
		MTextWidget.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		MTextWidget.SetTextNoLocalization("");
		AddChild(MTextWidget);
		_mCheckMarkWidget = new MilMo_Widget(UI);
		_mCheckMarkWidget.SetScale(25f, 25f);
		_mCheckMarkWidget.SetTexture("Batch01/Textures/Homes/IconCheck");
		_mCheckMarkWidget.SetAlignment(MilMo_GUI.Align.CenterCenter);
		_mCheckMarkWidget.SetPosition(18f, 20f);
		MilMo_Widget milMo_Widget = new MilMo_Widget(ui);
		milMo_Widget.SetScale(32f, 32f);
		milMo_Widget.SetTexture("Batch01/Textures/Quest/IconQuestCheckBox");
		milMo_Widget.SetAlignment(MilMo_GUI.Align.CenterCenter);
		milMo_Widget.SetPosition(18f, 20f);
		AddChild(milMo_Widget);
		MLeftSideWidgets = new List<MilMo_Widget>();
		if (condition.Completed)
		{
			SetState(done: true);
		}
	}

	protected MilMo_Widget CreateTextWidget()
	{
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetFont(MilMo_GUI.Font.EborgSmall);
		milMo_Widget.SetFontScale(0.7f);
		milMo_Widget.SetScale(48f, 24f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.CenterRight);
		milMo_Widget.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		return milMo_Widget;
	}

	private void SetState(bool done)
	{
		if (done)
		{
			AddChild(_mCheckMarkWidget);
		}
	}

	private void AddLeftSideWidget(MilMo_Widget widget)
	{
		float num = 3f + MLeftSideWidgets.Sum((MilMo_Widget w) => 2f + w.Scale.x);
		widget.SetPosition(Scale.x - num, 16f);
		MLeftSideWidgets.Add(widget);
		AddChild(widget);
	}

	protected void AddLeftSideCounter(int current, int target)
	{
		if (current > target)
		{
			current = target;
		}
		MilMo_Widget milMo_Widget = CreateTextWidget();
		milMo_Widget.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		milMo_Widget.SetScale(38f, 24f);
		milMo_Widget.SetTextNoLocalization(current + "/" + target);
		AddLeftSideWidget(milMo_Widget);
	}

	protected void AddLeftSideIcon(string texture, bool prefixStandardGuiPath)
	{
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetScale(32f, 32f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.CenterRight);
		milMo_Widget.SetTexture(texture, prefixStandardGuiPath);
		AddLeftSideWidget(milMo_Widget);
	}

	protected void AddLeftSideIcon(MilMo_Widget w)
	{
		w.SetScale(32f, 32f);
		w.SetAlignment(MilMo_GUI.Align.CenterRight);
		AddLeftSideWidget(w);
	}

	public virtual void Destroy()
	{
		if (MTickEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(MTickEvent);
			MTickEvent = null;
		}
	}
}
