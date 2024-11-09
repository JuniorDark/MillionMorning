using System.Collections.Generic;
using System.Linq;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.Network;
using Code.World.GUI.QuestLog.Tracker_Condition_Widgets;
using Code.World.Player;
using Code.World.Quest;
using Core;
using UnityEngine;

namespace Code.World.GUI.QuestLog;

public sealed class MilMo_QuestTracker : MilMo_Widget
{
	private readonly List<MilMo_QuestTrackerConditionWidget> _mConditionWidgets;

	private readonly MilMo_Widget _mQuestNameWidget;

	public MilMo_Quest ActiveQuest { get; private set; }

	public bool IsHidden { get; set; }

	public MilMo_QuestTracker(MilMo_UserInterface ui)
		: base(ui)
	{
		SetScale(240f, 0f);
		SetAlignment(MilMo_GUI.Align.TopRight);
		AllowPointerFocus = false;
		_mQuestNameWidget = new MilMo_Widget(ui);
		_mQuestNameWidget.SetScale(220f, 30f);
		_mQuestNameWidget.SetFont(MilMo_GUI.Font.EborgSmall);
		_mQuestNameWidget.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mQuestNameWidget.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		_mQuestNameWidget.SetDefaultTextColor(1f, 1f, 0f, 1f);
		_mQuestNameWidget.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		_mQuestNameWidget.TextOutlineColor = Color.black;
		_mQuestNameWidget.SetPosition(-2f, -7f);
		_mConditionWidgets = new List<MilMo_QuestTrackerConditionWidget>();
	}

	public override void Draw()
	{
		if (Enabled && !IsHidden)
		{
			if (ActiveQuest != null)
			{
				GUISkin skin = UnityEngine.GUI.skin;
				UnityEngine.GUI.skin = Skin;
				UnityEngine.GUI.color = new Color(1f, 1f, 1f, 0.4f) * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
				Rect screenPosition = GetScreenPosition();
				screenPosition.x -= 15f;
				screenPosition.y -= 15f;
				screenPosition.width = Scale.x + 20f;
				screenPosition.height = GetYScale() + 10f;
				UnityEngine.GUI.Box(screenPosition, "");
				UnityEngine.GUI.skin = skin;
			}
			base.Draw();
		}
	}

	public void SetActiveQuest(MilMo_Quest activeQuest)
	{
		if (MilMo_Player.Instance.Quests.IsCompleted(activeQuest.TemplatePath) && !MilMo_Player.Instance.Quests.IsInActiveQuests(activeQuest.TemplatePath))
		{
			SetInactive();
			return;
		}
		IList<MilMo_QuestCondition> conditions = activeQuest.Conditions;
		if (conditions == null)
		{
			return;
		}
		ActiveQuest = activeQuest;
		if (activeQuest.Name.String.Length > 25)
		{
			string text = activeQuest.Name.String.Remove(25);
			text += "...";
			_mQuestNameWidget.SetTextNoLocalization(text);
		}
		else
		{
			_mQuestNameWidget.SetText(activeQuest.Name);
		}
		_mConditionWidgets.Clear();
		RemoveAllChildren();
		AddChild(_mQuestNameWidget);
		foreach (MilMo_QuestCondition item in conditions)
		{
			AddCondition(item);
		}
		if (ActiveQuest != null)
		{
			Singleton<GameNetwork>.Instance.UpdateTrackedQuest(ActiveQuest.Id, track: true);
		}
	}

	public void Refresh(MilMo_Quest activeQuest)
	{
		SetInactive();
		SetActiveQuest(activeQuest);
	}

	private void AddCondition(MilMo_QuestCondition condition)
	{
		MilMo_QuestTrackerConditionWidget milMo_QuestTrackerConditionWidget = new MilMo_QuestTrackerConditionWidget(UI, condition);
		float y = 25f + _mConditionWidgets.Sum((MilMo_QuestTrackerConditionWidget con) => con.Scale.y);
		milMo_QuestTrackerConditionWidget.SetPosition(20f, y);
		AddChild(milMo_QuestTrackerConditionWidget);
		_mConditionWidgets.Add(milMo_QuestTrackerConditionWidget);
	}

	public void SetInactive()
	{
		if (ActiveQuest != null)
		{
			Singleton<GameNetwork>.Instance.UpdateTrackedQuest(ActiveQuest.Id, track: false);
		}
		ActiveQuest = null;
		_mQuestNameWidget.SetTextNoLocalization("");
		foreach (MilMo_QuestTrackerConditionWidget item in _mConditionWidgets.Where((MilMo_QuestTrackerConditionWidget widget) => widget != null))
		{
			item.Destroy();
		}
		_mConditionWidgets.Clear();
		RemoveAllChildren();
	}

	public void RefreshUI()
	{
		SetPosition(Screen.width - 15, 130f);
	}

	public float GetYScale()
	{
		return 43f + _mConditionWidgets.Sum((MilMo_QuestTrackerConditionWidget wid) => wid.Scale.y);
	}
}
