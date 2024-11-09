using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.World.GUI.QuestLog.Condition_Widgets;
using Code.World.Level;
using Code.World.Level.LevelInfo;
using Code.World.Player;
using Code.World.Quest;
using Code.World.Quest.Conditions;
using Core;
using Core.GameEvent;
using UI.LockState;
using UnityEngine;

namespace Code.World.GUI.QuestLog;

public sealed class MilMo_QuestLog : MilMo_Window
{
	private MilMo_ComboBox _mAdventuresBox;

	private MilMo_ComboBox _mLevelBox;

	private MilMo_ScrollView _mQuestsScrollView;

	private MilMo_ScrollView _mConditionsScrollView;

	private MilMo_ScrollView _mRewardsScrollView;

	private MilMo_Widget _mQuestAmountText;

	private MilMo_Widget _mQuestDescriptionBox;

	private MilMo_Button _mActivateQuestButton;

	private string _mCurrentButtonTexture = "";

	private MilMo_Quest _mSelectedQuest;

	private string _mCurrentSelectedLevelFullName;

	private int _mQuestRewardCount;

	private int _mQuestCount;

	private readonly Vector2 _mWindowScale = new Vector2(550f, 408f);

	private readonly Vector2 _mQuestBoxScale = new Vector2(180f, 290f);

	private readonly Vector2 _mQuestBoxPos = new Vector2(20f, 65f);

	private readonly Vector2 _mDescriptionBoxScale = new Vector2(320f, 100f);

	private readonly Vector2 _mDescriptionBoxPos = new Vector2(210f, 65f);

	private readonly Vector2 _mConditionBoxScale = new Vector2(320f, 85f);

	private readonly Vector2 _mConditionBoxPos = new Vector2(210f, 175f);

	private readonly Vector2 _mRewardsBoxScale = new Vector2(320f, 85f);

	private readonly Vector2 _mRewardsBoxPos = new Vector2(210f, 270f);

	private const float M_LOWER_PARTS_Y = 365f;

	private const float M_BUTTON_X_SCALE = 97f;

	private readonly List<string> _mLevelStrings = new List<string>();

	private readonly List<string> _mWorldStrings = new List<string>();

	public sbyte Gender { get; set; }

	public MilMo_QuestLog(MilMo_UserInterface ui)
		: base(ui)
	{
		Gender = -1;
		GameEvent.ToggleQuestLOGEvent.RegisterAction(ToggleQuestLog);
		SpawnScale = _mWindowScale;
		TargetScale = _mWindowScale;
		HasCloseButton = true;
		base.CloseButton.SetScale(32f, 32f);
		SetTextNoLocalization("");
		InitUI();
		RegisterListeners();
		_mCurrentSelectedLevelFullName = ((MilMo_Level.CurrentLevel != null) ? MilMo_Level.CurrentLevel.VerboseName : MilMo_Level.LastAdventureLevel);
		ui.AddChild(this);
	}

	private void ToggleQuestLog()
	{
		if (MilMo_Player.Instance != null && !MilMo_Player.Instance.InGUIApp && !MilMo_Player.Instance.InPVP && Singleton<LockStateManager>.Instance.HasUnlockedQuestLog)
		{
			MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_ToggleQuestLog");
			Toggle();
		}
	}

	private void RegisterListeners()
	{
		MilMo_EventSystem.Listen("quest_updated", QuestUpdated).Repeating = true;
		MilMo_EventSystem.Listen("quest_received", QuestAdded).Repeating = true;
	}

	private void SelectQuest(MilMo_Quest quest)
	{
		MilMo_QuestEntryWidget milMo_QuestEntryWidget = null;
		foreach (MilMo_QuestEntryWidget item in _mQuestsScrollView.Children.OfType<MilMo_QuestEntryWidget>())
		{
			if (item.Quest.Id != quest.Id)
			{
				item.SetNoneSelected();
			}
			else
			{
				milMo_QuestEntryWidget = item;
			}
		}
		_mSelectedQuest = quest;
		if (milMo_QuestEntryWidget != null)
		{
			milMo_QuestEntryWidget.MTxt.SetDefaultTextColor(Color.green);
			milMo_QuestEntryWidget.MIsSelected = true;
		}
		SetUpConditions(quest.Conditions);
		SetUpRewards(quest);
		MilMo_LocString notLocalizedLocString = MilMo_Localization.GetNotLocalizedLocString("{0}\n{1}");
		notLocalizedLocString.SetFormatArgs(quest.Name, quest.Description);
		_mQuestDescriptionBox.SetText(notLocalizedLocString);
	}

	private void SetNoQuest()
	{
		_mQuestDescriptionBox.SetText(MilMo_Localization.GetLocString("World_416"));
		_mQuestRewardCount = 0;
		_mRewardsScrollView.RemoveAllChildren();
		foreach (MilMo_QuestConditionWidget item in _mConditionsScrollView.Children.OfType<MilMo_QuestConditionWidget>())
		{
			item.Destroy();
		}
		_mConditionsScrollView.RemoveAllChildren();
		_mSelectedQuest = null;
	}

	private void SetUpConditions(IList<MilMo_QuestCondition> conditions)
	{
		_mConditionsScrollView.ScrollToNow(0f, 0f);
		foreach (MilMo_QuestConditionWidget item in _mConditionsScrollView.Children.OfType<MilMo_QuestConditionWidget>())
		{
			item.Destroy();
		}
		_mConditionsScrollView.RemoveAllChildren();
		if (conditions == null)
		{
			return;
		}
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetFont(MilMo_GUI.Font.EborgSmall);
		milMo_Widget.SetFontScale(0.85f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		milMo_Widget.SetScale(_mConditionBoxScale.x - 10f, 25f);
		milMo_Widget.SetText(MilMo_Localization.GetLocString("QuestLog_9357"));
		milMo_Widget.SetPosition(0f, 0f);
		_mConditionsScrollView.AddChild(milMo_Widget);
		foreach (MilMo_QuestCondition condition in conditions)
		{
			AddQuestConditionToList(condition);
		}
		_mConditionsScrollView.RefreshViewSize();
	}

	private void SetUpRewards(MilMo_Quest quest)
	{
		_mRewardsScrollView.ScrollToNow(0f, 0f);
		_mQuestRewardCount = 0;
		_mRewardsScrollView.RemoveAllChildren();
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetFont(MilMo_GUI.Font.EborgSmall);
		milMo_Widget.SetFontScale(0.85f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		milMo_Widget.SetScale(_mRewardsBoxScale.x - 10f, 25f);
		milMo_Widget.SetText(MilMo_Localization.GetLocString("QuestLog_9358"));
		milMo_Widget.SetPosition(0f, 0f);
		_mRewardsScrollView.AddChild(milMo_Widget);
		if (quest.RewardCoins > 0)
		{
			AddQuestRewardToList("Content/GUI/Batch01/Textures/Shop/Coins", MilMo_Localization.GetLocString("World_8578"), quest.RewardCoins);
		}
		if (quest.RewardGems > 0)
		{
			AddQuestRewardToList("Content/GUI/Batch01/Textures/GameDialog/IconGem", MilMo_Localization.GetLocString("World_5541"), quest.RewardGems);
		}
		if (quest.RewardTelepods > 0)
		{
			MilMo_LocString locString = MilMo_Localization.GetLocString("World_6098");
			locString.SetFormatArgs("");
			AddQuestRewardToList("Content/Items/Batch01/SpecialItems/IconTeleportStone", locString, quest.RewardTelepods);
		}
		if (quest.RewardExp > 0)
		{
			AddQuestRewardToList(quest.RewardExp);
		}
		if (quest.RewardsItems.Count > 0)
		{
			foreach (MilMo_QuestRewardItem rewardsItem in quest.RewardsItems)
			{
				if (rewardsItem.Gender == -1 || rewardsItem.Gender == Gender)
				{
					AddQuestRewardToList(rewardsItem);
				}
			}
		}
		_mRewardsScrollView.RefreshViewSize();
	}

	private void AddQuestConditionToList(MilMo_QuestCondition condition)
	{
		MilMo_QuestConditionWidget milMo_QuestConditionWidget = null;
		if (condition is MilMo_ArrivesAt)
		{
			milMo_QuestConditionWidget = new MilMo_QuestConditionWidget_ArrivesAt(UI, (MilMo_ArrivesAt)condition);
		}
		else if (condition is MilMo_Wear)
		{
			milMo_QuestConditionWidget = new MilMo_QuestConditionWidget_Wear(UI, (MilMo_Wear)condition);
		}
		else if (condition is MilMo_ArrivesAtAny)
		{
			milMo_QuestConditionWidget = new MilMo_QuestConditionWidget_ArrivesAtAny(UI, (MilMo_ArrivesAtAny)condition);
		}
		else if (condition is MilMo_CollectedAny)
		{
			milMo_QuestConditionWidget = new MilMo_QuestConditionWidget_CollectedAny(UI, (MilMo_CollectedAny)condition);
		}
		else if (condition is MilMo_CollectedGem)
		{
			milMo_QuestConditionWidget = new MilMo_QuestConditionWidget_CollectedGem(UI, (MilMo_CollectedGem)condition);
		}
		else if (condition is MilMo_KilledAny)
		{
			MilMo_KilledAny milMo_KilledAny = (MilMo_KilledAny)condition;
			MilMo_Killed condition2 = new MilMo_Killed(new ConditionKilled(milMo_KilledAny.Kills[0].CreatureVisualRep, milMo_KilledAny.Kills[0].CreatureDisplayName, milMo_KilledAny.Kills[0].AmountToKill, milMo_KilledAny.Kills[0].AmountKilled, (sbyte)(milMo_KilledAny.Completed ? 1 : 0), (sbyte)(milMo_KilledAny.Active ? 1 : 0)));
			milMo_QuestConditionWidget = new MilMo_QuestConditionWidget_Killed(UI, condition2);
		}
		else if (condition is MilMo_Collected)
		{
			milMo_QuestConditionWidget = new MilMo_QuestConditionWidget_Collected(UI, (MilMo_Collected)condition);
		}
		else if (condition is MilMo_Killed)
		{
			milMo_QuestConditionWidget = new MilMo_QuestConditionWidget_Killed(UI, (MilMo_Killed)condition);
		}
		else if (condition is MilMo_TalkTo)
		{
			milMo_QuestConditionWidget = new MilMo_QuestConditionWidget_TalkTo(UI, (MilMo_TalkTo)condition);
		}
		else if (condition is MilMo_TalkToAny)
		{
			milMo_QuestConditionWidget = new MilMo_QuestConditionWidget_TalkToAny(UI, (MilMo_TalkToAny)condition);
		}
		if (milMo_QuestConditionWidget != null)
		{
			float y = 15f + _mConditionsScrollView.Children.OfType<MilMo_QuestConditionWidget>().Sum((MilMo_QuestConditionWidget wid) => wid.Scale.y);
			milMo_QuestConditionWidget.SetPosition(20f, y);
			_mConditionsScrollView.AddChild(milMo_QuestConditionWidget);
		}
	}

	private void AddQuestRewardToList(string texturePath, MilMo_LocString itemName, int count)
	{
		MilMo_QuestRewardWidget milMo_QuestRewardWidget = new MilMo_QuestRewardWidget(UI, texturePath, count, itemName);
		milMo_QuestRewardWidget.SetPosition(GetNextRewardPosition());
		_mRewardsScrollView.AddChild(milMo_QuestRewardWidget);
		_mQuestRewardCount++;
	}

	private Vector2 GetNextRewardPosition()
	{
		Vector2 result = default(Vector2);
		result.x = 25f + (float)(_mQuestRewardCount % 2) * _mRewardsBoxScale.x * 0.5f;
		result.y = 20 + _mQuestRewardCount / 2 * 30;
		return result;
	}

	private void AddQuestRewardToList(MilMo_QuestRewardItem item)
	{
		MilMo_QuestRewardWidget milMo_QuestRewardWidget = new MilMo_QuestRewardWidget(UI, item);
		milMo_QuestRewardWidget.SetPosition(GetNextRewardPosition());
		_mRewardsScrollView.AddChild(milMo_QuestRewardWidget);
		_mQuestRewardCount++;
	}

	private void AddQuestRewardToList(int expReward)
	{
		MilMo_QuestRewardWidget milMo_QuestRewardWidget = new MilMo_QuestRewardWidget(UI, expReward);
		milMo_QuestRewardWidget.SetPosition(GetNextRewardPosition());
		_mRewardsScrollView.AddChild(milMo_QuestRewardWidget);
		_mQuestRewardCount++;
	}

	private void AddQuestToList(MilMo_Quest quest)
	{
		MilMo_QuestEntryWidget milMo_QuestEntryWidget = new MilMo_QuestEntryWidget(UI, quest, delegate(object qasObj)
		{
			if (qasObj is MilMo_Quest quest2)
			{
				SelectQuest(quest2);
			}
		});
		milMo_QuestEntryWidget.SetPosition(20f, 5 + _mQuestCount * 25);
		_mQuestsScrollView.AddChild(milMo_QuestEntryWidget);
		_mQuestsScrollView.RefreshViewSize();
		_mQuestCount++;
	}

	private void RefreshLevelsDropdown()
	{
		_mLevelBox.RemoveAll();
		_mLevelStrings.Clear();
		_mLevelStrings.Add("All");
		_mLevelBox.AddItem(MilMo_Localization.GetLocString("World_6058"));
		string text = _mCurrentSelectedLevelFullName.Split(':')[0];
		for (int i = 0; i < MilMo_Player.Instance.Quests.ActiveQuests.Count; i++)
		{
			MilMo_Quest milMo_Quest = MilMo_Player.Instance.Quests.ActiveQuests[i];
			if ((!(text != "All") || !(milMo_Quest.World != text)) && !_mLevelStrings.Contains(milMo_Quest.Level))
			{
				_mLevelStrings.Add(milMo_Quest.Level);
				_mLevelBox.AddItem(MilMo_LevelInfo.GetLevelDisplayName(milMo_Quest.FullLevelName));
			}
		}
		_mLevelBox.SelectItem(0);
		_mLevelBox.RefreshUI();
	}

	private void RefreshWorldsDropdown()
	{
		_mWorldStrings.Clear();
		_mAdventuresBox.RemoveAll();
		_mAdventuresBox.AddItem(MilMo_Localization.GetLocString("World_5542"));
		_mWorldStrings.Add("All");
		foreach (MilMo_Quest item in MilMo_Player.Instance.Quests.ActiveQuests.Where((MilMo_Quest quest) => !_mWorldStrings.Contains(quest.World)))
		{
			MilMo_WorldInfoData worldInfoData;
			try
			{
				worldInfoData = MilMo_LevelInfo.GetWorldInfoData(item.World);
			}
			catch (InvalidOperationException)
			{
				Debug.LogWarning("Got quest " + item.TemplatePath + " for unknown world " + item.World);
				continue;
			}
			if (worldInfoData.VisibleInGUILists)
			{
				_mWorldStrings.Add(item.World);
				_mAdventuresBox.AddItem(worldInfoData.WorldDisplayName);
			}
		}
		_mAdventuresBox.RefreshUI();
	}

	private void ResetBoxesToDefault()
	{
		if (MilMo_Level.CurrentLevel == null && string.IsNullOrEmpty(MilMo_Level.LastAdventureLevel))
		{
			return;
		}
		_mCurrentSelectedLevelFullName = ((MilMo_Level.CurrentLevel != null) ? MilMo_Level.CurrentLevel.VerboseName : MilMo_Level.LastAdventureLevel);
		string text = _mCurrentSelectedLevelFullName.Split(':')[1];
		string text2 = _mCurrentSelectedLevelFullName.Split(':')[0];
		RefreshWorldsDropdown();
		RefreshLevelsDropdown();
		_mAdventuresBox.SelectItem(0);
		for (int i = 0; i < _mWorldStrings.Count; i++)
		{
			if (_mWorldStrings[i] == text2)
			{
				_mAdventuresBox.SelectItem(i);
			}
		}
		_mLevelBox.SelectItem(0);
		for (int j = 0; j < _mLevelStrings.Count; j++)
		{
			if (_mLevelStrings[j] == text)
			{
				_mLevelBox.SelectItem(j);
			}
		}
	}

	public override void Open()
	{
		if (MilMo_Level.CurrentLevel == null || !MilMo_LevelInfo.IsPvp(MilMo_Level.CurrentLevel.VerboseName))
		{
			ResetBoxesToDefault();
			base.Open();
		}
	}

	private void QuestUpdated(object o)
	{
		if (!(o is MilMo_Quest milMo_Quest))
		{
			return;
		}
		if (Enabled && _mSelectedQuest != null && _mSelectedQuest.Id == milMo_Quest.Id)
		{
			if (milMo_Quest.State == QuestState.Complete)
			{
				SetNoQuest();
			}
			else
			{
				SetUpConditions(milMo_Quest.Conditions);
			}
		}
		if (!(MilMo_World.HudHandler == null) && MilMo_World.HudHandler.theQuestTracker != null && MilMo_World.HudHandler.theQuestTracker.ActiveQuest != null && MilMo_World.HudHandler.theQuestTracker.ActiveQuest.Id == milMo_Quest.Id)
		{
			MilMo_World.HudHandler.theQuestTracker.Refresh(milMo_Quest);
		}
	}

	private void QuestAdded(object o)
	{
		if (o is MilMo_Quest questActive)
		{
			RefreshQuests();
			SetQuestActive(questActive);
		}
	}

	private void RefreshQuests()
	{
		_mQuestsScrollView.RemoveAllChildren();
		_mQuestCount = 0;
		int num = -1;
		if (_mSelectedQuest != null)
		{
			num = _mSelectedQuest.Id;
		}
		SetNoQuest();
		string text = _mCurrentSelectedLevelFullName.Split(':')[1];
		string text2 = _mCurrentSelectedLevelFullName.Split(':')[0];
		foreach (MilMo_Quest activeQuest in MilMo_Player.Instance.Quests.ActiveQuests)
		{
			if ((!(text2 != "All") || !(text2 != activeQuest.World) || activeQuest.IsGlobal) && (!(text != "All") || !(text != activeQuest.Level) || activeQuest.IsGlobal))
			{
				AddQuestToList(activeQuest);
				if (_mSelectedQuest == null || num == activeQuest.Id)
				{
					SelectQuest(activeQuest);
				}
			}
		}
		_mQuestsScrollView.RefreshViewSize();
		_mAdventuresBox.RefreshUI();
		_mLevelBox.RefreshUI();
	}

	private void SetQuestActive(object o)
	{
		if (_mSelectedQuest != null)
		{
			SetQuestActive(_mSelectedQuest);
		}
	}

	private static void SetQuestActive(MilMo_Quest quest)
	{
		if (quest != null)
		{
			if (MilMo_World.HudHandler.theQuestTracker.ActiveQuest != null && quest.Id == MilMo_World.HudHandler.theQuestTracker.ActiveQuest.Id)
			{
				MilMo_World.HudHandler.theQuestTracker.SetInactive();
			}
			else
			{
				MilMo_World.HudHandler.theQuestTracker.SetActiveQuest(quest);
			}
		}
	}

	public override void Draw()
	{
		if (!Enabled)
		{
			return;
		}
		if (MilMo_World.HudHandler != null && MilMo_World.HudHandler.theQuestTracker != null)
		{
			if (MilMo_World.HudHandler.theQuestTracker.ActiveQuest != null && _mSelectedQuest != null)
			{
				if (MilMo_World.HudHandler.theQuestTracker.ActiveQuest.Id == _mSelectedQuest.Id)
				{
					if (_mCurrentButtonTexture != "Batch01/Textures/Dialog/ButtonPressed")
					{
						_mActivateQuestButton.SetTexture("Batch01/Textures/Dialog/ButtonPressed");
						_mCurrentButtonTexture = "Batch01/Textures/Dialog/ButtonPressed";
					}
				}
				else if (_mCurrentButtonTexture != "Batch01/Textures/Dialog/ButtonNormal")
				{
					_mActivateQuestButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
					_mCurrentButtonTexture = "Batch01/Textures/Dialog/ButtonNormal";
				}
			}
			else if (_mCurrentButtonTexture != "Batch01/Textures/Dialog/ButtonNormal")
			{
				_mActivateQuestButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
				_mCurrentButtonTexture = "Batch01/Textures/Dialog/ButtonNormal";
			}
		}
		base.Draw();
	}

	private void InitUI()
	{
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.SetScale(_mDescriptionBoxScale);
		milMo_Widget.SetPosition(_mDescriptionBoxPos);
		milMo_Widget.SetTextureBlackTransparent();
		AddChild(milMo_Widget);
		_mQuestDescriptionBox = new MilMo_Widget(UI);
		_mQuestDescriptionBox.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mQuestDescriptionBox.SetScale(_mDescriptionBoxScale.x - 10f, _mDescriptionBoxScale.y - 10f);
		_mQuestDescriptionBox.SetPosition(_mDescriptionBoxPos.x + 5f, _mDescriptionBoxPos.y + 5f);
		_mQuestDescriptionBox.SetTextPadding(0f, 25f, 0f, 0f);
		_mQuestDescriptionBox.SetWordWrap(w: true);
		_mQuestDescriptionBox.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		_mQuestDescriptionBox.SetFont(MilMo_GUI.Font.ArialRoundedMedium);
		_mQuestDescriptionBox.SetFontScale(0.7f);
		_mQuestDescriptionBox.SetText(MilMo_Localization.GetLocString("World_416"));
		AddChild(_mQuestDescriptionBox);
		MilMo_Widget milMo_Widget2 = new MilMo_Widget(UI);
		milMo_Widget2.SetTextureBlackTransparent();
		milMo_Widget2.SetPosition(_mConditionBoxPos);
		milMo_Widget2.SetScale(_mConditionBoxScale);
		milMo_Widget2.SetAlignment(MilMo_GUI.Align.TopLeft);
		AddChild(milMo_Widget2);
		_mConditionsScrollView = new MilMo_ScrollView(UI);
		_mConditionsScrollView.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mConditionsScrollView.SetPosition(_mConditionBoxPos.x - 20f, _mConditionBoxPos.y);
		_mConditionsScrollView.SetScale(_mConditionBoxScale.x + 20f, _mConditionBoxScale.y);
		_mConditionsScrollView.ShowHorizontalBar(h: false);
		AddChild(_mConditionsScrollView);
		MilMo_Widget milMo_Widget3 = new MilMo_Widget(UI);
		milMo_Widget3.SetTextureBlackTransparent();
		milMo_Widget3.SetPosition(_mRewardsBoxPos);
		milMo_Widget3.SetScale(_mRewardsBoxScale);
		milMo_Widget3.SetAlignment(MilMo_GUI.Align.TopLeft);
		AddChild(milMo_Widget3);
		_mRewardsScrollView = new MilMo_ScrollView(UI);
		_mRewardsScrollView.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mRewardsScrollView.SetPosition(_mRewardsBoxPos.x - 20f, _mRewardsBoxPos.y);
		_mRewardsScrollView.SetScale(_mRewardsBoxScale.x + 20f, _mRewardsBoxScale.y);
		_mRewardsScrollView.ShowHorizontalBar(h: false);
		AddChild(_mRewardsScrollView);
		MilMo_Widget milMo_Widget4 = new MilMo_Widget(UI);
		milMo_Widget4.SetTextureBlackTransparent();
		milMo_Widget4.SetPosition(_mQuestBoxPos);
		milMo_Widget4.SetScale(_mQuestBoxScale);
		milMo_Widget4.SetAlignment(MilMo_GUI.Align.TopLeft);
		AddChild(milMo_Widget4);
		_mQuestsScrollView = new MilMo_ScrollView(UI);
		_mQuestsScrollView.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mQuestsScrollView.SetScale(_mQuestBoxScale.x + 20f, _mQuestBoxScale.y);
		_mQuestsScrollView.SetPosition(_mQuestBoxPos.x - 20f, _mQuestBoxPos.y);
		_mQuestsScrollView.ShowHorizontalBar(h: false);
		AddChild(_mQuestsScrollView);
		_mActivateQuestButton = new MilMo_Button(UI);
		_mActivateQuestButton.SetScale(97f, 25f);
		_mActivateQuestButton.SetText(MilMo_Localization.GetLocString("QuestLog_9347"));
		_mActivateQuestButton.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mActivateQuestButton.SetPosition(_mRewardsBoxPos.x + 10f + 97f, 365f);
		_mActivateQuestButton.Function = SetQuestActive;
		_mActivateQuestButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		_mActivateQuestButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		_mActivateQuestButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		_mActivateQuestButton.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_mActivateQuestButton.SetFont(MilMo_GUI.Font.EborgSmall);
		_mActivateQuestButton.SetFontScale(0.8f);
		AddChild(_mActivateQuestButton);
		MilMo_Button milMo_Button = new MilMo_Button(UI);
		milMo_Button.SetScale(97f, 25f);
		milMo_Button.SetText(MilMo_Localization.GetLocString("Generic_Close"));
		milMo_Button.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Button.SetPosition(_mRewardsBoxPos.x + 20f + 194f, 365f);
		milMo_Button.Function = Close;
		milMo_Button.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		milMo_Button.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		milMo_Button.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		milMo_Button.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		milMo_Button.SetFont(MilMo_GUI.Font.EborgSmall);
		milMo_Button.SetFontScale(0.8f);
		AddChild(milMo_Button);
		MilMo_Widget milMo_Widget5 = new MilMo_Widget(UI);
		milMo_Widget5.SetFont(MilMo_GUI.Font.EborgMedium);
		milMo_Widget5.SetText(MilMo_Localization.GetLocString("World_5544"));
		milMo_Widget5.SetPosition(20f, 5f);
		milMo_Widget5.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		milMo_Widget5.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget5.SetFontPreset(MilMo_GUI.FontPreset.DropShadow);
		milMo_Widget5.SetScale(200f, 30f);
		milMo_Widget5.AllowPointerFocus = false;
		AddChild(milMo_Widget5);
		_mAdventuresBox = new MilMo_ComboBox(UI, MilMo_ComboBox.ComboDropDirection.Down, MilMo_LocString.Empty, 180f, 220f, Color.black, Color.white);
		_mAdventuresBox.BoxTextureWidget.SetAllTextures("Batch01/Textures/Quest/QuestLogSelectionButton");
		_mAdventuresBox.BoxTextureWidget.SetHoverTexture("Batch01/Textures/Quest/QuestLogSelectionButtonMO");
		_mAdventuresBox.Background.SetTexture("Batch01/Textures/Quest/QuestLogBackground");
		_mAdventuresBox.Top.SetTexture("Batch01/Textures/Quest/QuestLogBackgroundTop");
		_mAdventuresBox.Top.SetYScale(5f);
		_mAdventuresBox.BackgroundColor = new Color(0f, 0f, 0.12f, 0.9f);
		_mAdventuresBox.ItemTextColor = Color.white;
		_mAdventuresBox.BoxTextureWidget.SetFont(MilMo_GUI.Font.EborgSmall);
		_mAdventuresBox.BoxTextureWidget.SetFontScale(0.7f);
		_mAdventuresBox.BoxTextureWidget.SetFontPreset(MilMo_GUI.FontPreset.DropShadow);
		_mAdventuresBox.SelectedItemTextColor = new Color(0.7f, 0.7f, 0.7f, 1f);
		_mAdventuresBox.SetPosition(20f, 35f);
		_mAdventuresBox.UseIcon = false;
		_mAdventuresBox.IndexChangedFunction = delegate(int index)
		{
			string text2 = _mCurrentSelectedLevelFullName.Split(':')[1];
			if (index != 0)
			{
				_mCurrentSelectedLevelFullName = _mWorldStrings[index] + ":" + text2;
			}
			else
			{
				_mCurrentSelectedLevelFullName = "All:" + text2;
			}
			RefreshLevelsDropdown();
			_mQuestsScrollView.ScrollToNow(0f, 0f);
		};
		AddChild(_mAdventuresBox);
		MilMo_Widget milMo_Widget6 = new MilMo_Widget(UI);
		milMo_Widget6.SetFont(MilMo_GUI.Font.EborgMedium);
		milMo_Widget6.SetText(MilMo_Localization.GetLocString("World_5543"));
		milMo_Widget6.SetPosition(210f, 5f);
		milMo_Widget6.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		milMo_Widget6.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget6.SetScale(80f, 30f);
		milMo_Widget6.SetFontPreset(MilMo_GUI.FontPreset.DropShadow);
		milMo_Widget6.AllowPointerFocus = false;
		AddChild(milMo_Widget6);
		_mLevelBox = new MilMo_ComboBox(UI, MilMo_ComboBox.ComboDropDirection.Down, MilMo_LocString.Empty, 160f, 220f, Color.black, Color.white);
		_mLevelBox.BoxTextureWidget.SetAllTextures("Batch01/Textures/Quest/QuestLogSelectionButton");
		_mLevelBox.BoxTextureWidget.SetHoverTexture("Batch01/Textures/Quest/QuestLogSelectionButtonMO");
		_mLevelBox.Background.SetTexture("Batch01/Textures/Quest/QuestLogBackground");
		_mLevelBox.Top.SetTexture("Batch01/Textures/Quest/QuestLogBackgroundTop");
		_mLevelBox.Top.SetYScale(5f);
		_mLevelBox.BackgroundColor = new Color(0f, 0f, 0.12f, 0.9f);
		_mLevelBox.BoxTextureWidget.SetFont(MilMo_GUI.Font.EborgSmall);
		_mLevelBox.BoxTextureWidget.SetFontScale(0.7f);
		_mLevelBox.BoxTextureWidget.SetFontPreset(MilMo_GUI.FontPreset.DropShadow);
		_mLevelBox.ItemTextColor = Color.white;
		_mLevelBox.SelectedItemTextColor = new Color(0.7f, 0.7f, 0.7f, 1f);
		_mLevelBox.UseIcon = false;
		_mLevelBox.SetPosition(210f, 35f);
		_mLevelBox.IndexChangedFunction = delegate(int index)
		{
			string text = _mCurrentSelectedLevelFullName.Split(':')[0];
			if (index != 0)
			{
				_mCurrentSelectedLevelFullName = text + ":" + _mLevelStrings[index];
			}
			else
			{
				_mCurrentSelectedLevelFullName = text + ":All";
			}
			RefreshQuests();
			_mQuestsScrollView.ScrollToNow(0f, 0f);
		};
		AddChild(_mLevelBox);
	}
}
