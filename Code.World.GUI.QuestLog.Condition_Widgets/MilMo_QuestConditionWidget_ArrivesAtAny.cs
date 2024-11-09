using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.World.Level.LevelInfo;
using Code.World.Quest.Conditions;

namespace Code.World.GUI.QuestLog.Condition_Widgets;

public sealed class MilMo_QuestConditionWidget_ArrivesAtAny : MilMo_QuestConditionWidget
{
	private readonly List<MilMo_Widget> _mLevelWidgetRotationList;

	private readonly List<MilMo_Widget> _mTextWidgetRotationList;

	private int _mCurrentIndex;

	public MilMo_QuestConditionWidget_ArrivesAtAny(MilMo_UserInterface ui, MilMo_ArrivesAtAny condition)
		: base(ui, condition)
	{
		_mLevelWidgetRotationList = new List<MilMo_Widget>();
		_mTextWidgetRotationList = new List<MilMo_Widget>();
		MilMo_LocString copy = MilMo_Localization.GetLocString("QuestLog_9350").GetCopy();
		copy.SetFormatArgs("");
		MTextWidget.SetText(copy);
		for (int i = 0; i < condition.Areas.Count; i++)
		{
			AddWidgets(condition.Areas[i]);
		}
		AddChild(_mTextWidgetRotationList[0]);
		AddChild(_mLevelWidgetRotationList[0]);
		MTickEvent = MilMo_EventSystem.At(0.5f, Tick);
	}

	private void AddWidgets(MilMo_ArrivesAtAny.ArrivesAtAreaInfo info)
	{
		MilMo_LevelInfoData levelInfoData = MilMo_LevelInfo.GetLevelInfoData(info.FullLevelName);
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetTextAlignment(MTextWidget.TextAlign);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.SetScale(MTextWidget.Scale);
		milMo_Widget.SetFont(MilMo_GUI.Font.EborgSmall);
		milMo_Widget.SetFontScale(0.75f);
		milMo_Widget.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		milMo_Widget.SetPosition(MTextWidget.Pos);
		milMo_Widget.FadeToDefaultTextColor = false;
		MilMo_LocString copy = MilMo_Localization.GetLocString("QuestLog_9350").GetCopy();
		copy.SetFormatArgs(info.AreaDisplayName);
		milMo_Widget.SetText(copy);
		_mTextWidgetRotationList.Add(milMo_Widget);
		MilMo_Widget milMo_Widget2 = new MilMo_Widget(UI);
		milMo_Widget2.SetScale(32f, 32f);
		milMo_Widget2.SetAlignment(MilMo_GUI.Align.CenterRight);
		milMo_Widget2.SetPosition(Scale.x - 3f, 16f);
		milMo_Widget2.FadeToDefaultColor = false;
		milMo_Widget2.SetTexture(levelInfoData.IconPath, prefixStandardGuiPath: false);
		_mLevelWidgetRotationList.Add(milMo_Widget2);
	}

	private void Tick()
	{
		_mTextWidgetRotationList[_mCurrentIndex].AlphaTo(0f);
		_mLevelWidgetRotationList[_mCurrentIndex].TextColorTo(1f, 1f, 1f, 0f);
		_mCurrentIndex++;
		MTickEvent = MilMo_EventSystem.At(2f, delegate
		{
			RemoveChild(_mTextWidgetRotationList[_mCurrentIndex - 1]);
			RemoveChild(_mLevelWidgetRotationList[_mCurrentIndex - 1]);
			if (_mCurrentIndex == _mTextWidgetRotationList.Count)
			{
				_mCurrentIndex = 0;
			}
			_mLevelWidgetRotationList[_mCurrentIndex].SetAlpha(0f);
			_mTextWidgetRotationList[_mCurrentIndex].TextColorNow(1f, 1f, 1f, 0f);
			AddChild(_mLevelWidgetRotationList[_mCurrentIndex]);
			AddChild(_mTextWidgetRotationList[_mCurrentIndex]);
			_mLevelWidgetRotationList[_mCurrentIndex].AlphaTo(1f);
			_mTextWidgetRotationList[_mCurrentIndex].TextColorTo(1f, 1f, 1f, 1f);
			MTickEvent = MilMo_EventSystem.At(2f, Tick);
		});
	}
}
