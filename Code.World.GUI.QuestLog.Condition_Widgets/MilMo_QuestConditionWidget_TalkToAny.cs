using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.World.Level.LevelInfo;
using Code.World.Level.LevelObject;
using Code.World.Quest.Conditions;
using Core;
using UnityEngine;

namespace Code.World.GUI.QuestLog.Condition_Widgets;

public sealed class MilMo_QuestConditionWidget_TalkToAny : MilMo_QuestConditionWidget
{
	private int _mCurrentIndex;

	private readonly List<MilMo_Widget> _mTextWidgetRotationList;

	private readonly List<MilMo_Widget> _mIconWidgetRotationList;

	private readonly List<MilMo_Widget> _mLevelWidgetRotationList;

	public MilMo_QuestConditionWidget_TalkToAny(MilMo_UserInterface ui, MilMo_TalkToAny condition)
		: base(ui, condition)
	{
		MilMo_LocString copy = MilMo_Localization.GetLocString("QuestLog_9356").GetCopy();
		copy.SetFormatArgs("");
		MTextWidget.SetText(copy);
		_mTextWidgetRotationList = new List<MilMo_Widget>();
		_mIconWidgetRotationList = new List<MilMo_Widget>();
		_mLevelWidgetRotationList = new List<MilMo_Widget>();
		foreach (MilMo_TalkToAny.MilMo_TalkToInfo npC in condition.NpCs)
		{
			CreateWidgets(npC);
		}
		AddChild(_mIconWidgetRotationList[_mCurrentIndex]);
		AddChild(_mTextWidgetRotationList[_mCurrentIndex]);
		AddChild(_mLevelWidgetRotationList[_mCurrentIndex]);
		MTickEvent = MilMo_EventSystem.At(0.5f, Tick);
	}

	private void CreateWidgets(MilMo_TalkToAny.MilMo_TalkToInfo npc)
	{
		MilMo_Widget npcPortrait = new MilMo_Widget(UI);
		npcPortrait.SetScale(32f, 32f);
		npcPortrait.SetAlignment(MilMo_GUI.Align.CenterRight);
		npcPortrait.SetPosition(Scale.x - 37f, 16f);
		npcPortrait.FadeToDefaultColor = false;
		_mIconWidgetRotationList.Add(npcPortrait);
		MilMo_LevelInfoData levelInfoData = MilMo_LevelInfo.GetLevelInfoData(npc.FullLevelNames[0]);
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetScale(32f, 32f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.CenterRight);
		milMo_Widget.SetPosition(Scale.x - 3f, 16f);
		milMo_Widget.FadeToDefaultColor = false;
		milMo_Widget.SetTexture(levelInfoData.IconPath, prefixStandardGuiPath: false);
		_mLevelWidgetRotationList.Add(milMo_Widget);
		MilMo_Widget text = new MilMo_Widget(UI);
		text.SetTextAlignment(MTextWidget.TextAlign);
		text.SetAlignment(MilMo_GUI.Align.TopLeft);
		text.SetScale(MTextWidget.Scale);
		text.SetFont(MilMo_GUI.Font.EborgSmall);
		text.SetFontScale(0.75f);
		text.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		text.SetPosition(MTextWidget.Pos);
		text.FadeToDefaultTextColor = false;
		_mTextWidgetRotationList.Add(text);
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(npc.NPCTemplateIdentifier, delegate(MilMo_Template template, bool timeout)
		{
			if (!(template is MilMo_NpcTemplate milMo_NpcTemplate))
			{
				Debug.LogWarning("MilMo_QuestConditionWidget_TalkToAny: Npc template is null.");
			}
			else
			{
				string text2 = "Content/Creatures/" + milMo_NpcTemplate.VisualRep;
				string text3 = text2.Split('/')[^1];
				text2 = text2.Replace(text3, "");
				text2 = text2 + "Icon" + text3 + "0";
				npcPortrait.SetTexture(text2, prefixStandardGuiPath: false);
				MilMo_LocString copy = MilMo_Localization.GetLocString("QuestLog_9356").GetCopy();
				copy.SetFormatArgs(MilMo_Localization.GetLocString(milMo_NpcTemplate.Name));
				text.SetText(copy);
			}
		});
	}

	private void Tick()
	{
		_mIconWidgetRotationList[_mCurrentIndex].AlphaTo(0f);
		_mTextWidgetRotationList[_mCurrentIndex].TextColorTo(1f, 1f, 1f, 0f);
		_mLevelWidgetRotationList[_mCurrentIndex].AlphaTo(0f);
		_mCurrentIndex++;
		MTickEvent = MilMo_EventSystem.At(2f, delegate
		{
			RemoveChild(_mLevelWidgetRotationList[_mCurrentIndex - 1]);
			RemoveChild(_mTextWidgetRotationList[_mCurrentIndex - 1]);
			RemoveChild(_mIconWidgetRotationList[_mCurrentIndex - 1]);
			if (_mCurrentIndex == _mLevelWidgetRotationList.Count)
			{
				_mCurrentIndex = 0;
			}
			_mIconWidgetRotationList[_mCurrentIndex].SetAlpha(0f);
			_mTextWidgetRotationList[_mCurrentIndex].TextColorNow(1f, 1f, 1f, 0f);
			_mLevelWidgetRotationList[_mCurrentIndex].SetAlpha(0f);
			AddChild(_mIconWidgetRotationList[_mCurrentIndex]);
			AddChild(_mTextWidgetRotationList[_mCurrentIndex]);
			AddChild(_mLevelWidgetRotationList[_mCurrentIndex]);
			_mIconWidgetRotationList[_mCurrentIndex].AlphaTo(1f);
			_mTextWidgetRotationList[_mCurrentIndex].TextColorTo(1f, 1f, 1f, 1f);
			_mLevelWidgetRotationList[_mCurrentIndex].AlphaTo(1f);
			MTickEvent = MilMo_EventSystem.At(2f, Tick);
		});
	}
}
