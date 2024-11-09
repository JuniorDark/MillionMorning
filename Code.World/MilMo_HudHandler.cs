using System;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.World.GUI;
using Code.World.GUI.GameDialog;
using Code.World.GUI.Homes;
using Code.World.GUI.Ladder;
using Code.World.GUI.PVP;
using Code.World.GUI.QuestLog;
using Code.World.Home;
using Code.World.Level;
using Code.World.Level.LevelInfo;
using Code.World.Level.PVP;
using Code.World.Voting;
using Core.GameEvent;
using UI.HUD.States;
using UnityEngine;

namespace Code.World;

public class MilMo_HudHandler : MonoBehaviour
{
	private static readonly Vector2 RotateCameraButtonsScale = new Vector2(52f, 52f);

	private static readonly Vector2 RotateCameraButtonsOffset = new Vector2(-8f, -109f);

	private static readonly Vector2 LeaveChatroomButtonScale = new Vector2(128f, 128f);

	private static readonly Vector2 LeaveChatroomButtonOffset = new Vector2(-131f, 246f);

	private const int QUICK_INFO_HEIGHT = 75;

	public MilMo_MenuBar theMenuBar;

	public MilMo_HomeEnterText homeEnterText;

	public MilMo_QuestTracker theQuestTracker;

	public HudState.States hudState;

	private readonly Vector2 _loadingPanePos = new Vector2(-67f, 173f);

	public MilMo_Widget pvpWaitingText;

	public jb_PVPTeamScore pvpTeamScore;

	public jb_PVPZoneCountdown pvpZoneCountdown;

	private MilMo_Button _rotateSocialCamRightButton;

	private MilMo_Button _rotateSocialCamLeftButton;

	private MilMo_Button _leaveChatRoomButton;

	private MilMo_LoadingPane _loadingPane;

	private readonly MilMo_AudioClip _tickSound = new MilMo_AudioClip("Content/Sounds/Batch01/CharBuilder/CharBuilderTick");

	private readonly MilMo_AudioClip _wieldableEmptySound = new MilMo_AudioClip("Content/Sounds/Batch01/Inventory/CloseBag");

	private readonly MilMo_AudioClip _rewardTickSound = new MilMo_AudioClip("Content/Sounds/Batch01/CharacterShop/JuneCoinTick");

	private readonly MilMo_AudioClip _swooshClip = new MilMo_AudioClip("Content/Sounds/Batch01/GUI/Generic/Swap");

	private bool _hasTutorialCombatTimer;

	public bool hasCombatTimer;

	private readonly jb_RatingVisualization _ratingVisualization;

	private MilMo_VotingVisualization _votingVisualization;

	public MilMo_UserInterface UI { get; private set; }

	private void Start()
	{
		GameEvent.UpdateHudStateEvent.RegisterAction(SetHudState);
	}

	private void OnDestroy()
	{
		GameEvent.UpdateHudStateEvent.UnregisterAction(SetHudState);
	}

	private void Update()
	{
		try
		{
			if (pvpWaitingText != null && pvpTeamScore != null)
			{
				if (MilMo_Level.CurrentLevel != null && MilMo_LevelInfo.IsPvp(MilMo_Level.CurrentLevel.VerboseName))
				{
					if (MilMo_Level.CurrentLevel.PvpHandler.MatchState == MilMo_PVPHandler.MilMo_MatchState.Waiting)
					{
						pvpWaitingText.SetText(MilMo_Localization.GetLocString("PVP_9339"));
						pvpWaitingText.SetEnabled(e: true);
					}
					else if (MilMo_Level.CurrentLevel.PvpHandler.MatchState == MilMo_PVPHandler.MilMo_MatchState.Scoreboard && !MilMo_Level.CurrentLevel.PvpHandler.MatchHasEnded)
					{
						pvpWaitingText.SetText(MilMo_Localization.GetLocString("PVP_9340"));
						pvpWaitingText.SetEnabled(e: true);
					}
					else
					{
						pvpWaitingText.SetEnabled(e: false);
					}
				}
				else
				{
					pvpWaitingText.SetEnabled(e: false);
					pvpTeamScore.SetEnabled(e: false);
				}
			}
			if (_loadingPane != null)
			{
				if (theQuestTracker != null && theQuestTracker.ActiveQuest != null && !theQuestTracker.IsHidden)
				{
					_loadingPane.SetPosition((float)Screen.width + _loadingPanePos.x, theQuestTracker.Pos.y + theQuestTracker.GetYScale() + 40f);
				}
				else
				{
					_loadingPane.SetPosition((float)Screen.width + _loadingPanePos.x, _loadingPanePos.y);
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}

	public void SetHudState(HudState.States newState)
	{
		hudState = newState;
		_ratingVisualization?.SetEnabled(e: false);
		_votingVisualization?.SetEnabled(e: false);
		if (newState == HudState.States.Home)
		{
			ShowRoomName(isEntering: true);
		}
		theMenuBar.SetEnabled(e: true);
		if (newState == HudState.States.Pvp || newState == HudState.States.PvpAbilities)
		{
			theMenuBar.SetPvpMode();
		}
		else
		{
			theMenuBar.SetNormalMode();
		}
		if (newState == HudState.States.Normal || newState == HudState.States.StarterLevel)
		{
			theQuestTracker.IsHidden = false;
		}
		else
		{
			theQuestTracker.IsHidden = true;
		}
	}

	public void ShowRoomName(bool isEntering)
	{
		string roomName = MilMo_Localization.GetLocString("Homes_13404").String;
		if (MilMo_Home.CurrentHome.CurrentRoom.Item.Modifiers.ContainsKey("RoomName"))
		{
			roomName = MilMo_Home.CurrentHome.CurrentRoom.Item.Modifiers["RoomName"];
		}
		homeEnterText.Show(MilMo_Home.CurrentHome.HomeName, roomName, isEntering);
	}

	private void OnGUI()
	{
		try
		{
			MilMo_UserInterface uI = UI;
			if (uI != null && uI.ScreenSizeDirty)
			{
				RefreshUI();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}

	private void RefreshUI()
	{
		pvpWaitingText?.SetPosition((float)Screen.width / 2f, Screen.height / 2 - 100);
		theQuestTracker.RefreshUI();
		pvpTeamScore.RefreshUI();
		pvpZoneCountdown.RefreshUI();
		theMenuBar?.RefreshUI();
		if (_rotateSocialCamLeftButton != null)
		{
			_rotateSocialCamLeftButton.SetPosition((float)Screen.width - RotateCameraButtonsScale.x + RotateCameraButtonsOffset.x, (float)Screen.height + RotateCameraButtonsOffset.y);
			_rotateSocialCamLeftButton.SetScale(RotateCameraButtonsScale);
		}
		if (_rotateSocialCamRightButton != null)
		{
			_rotateSocialCamRightButton.SetPosition((float)Screen.width - RotateCameraButtonsScale.x + RotateCameraButtonsOffset.x, (float)Screen.height + RotateCameraButtonsOffset.y);
			_rotateSocialCamRightButton.SetScale(RotateCameraButtonsScale);
		}
		if (_leaveChatRoomButton != null)
		{
			_leaveChatRoomButton.SetPosition((float)Screen.width + LeaveChatroomButtonOffset.x, LeaveChatroomButtonOffset.y);
			_leaveChatRoomButton.SetScale(LeaveChatroomButtonScale);
		}
		_loadingPane?.SetPosition((float)Screen.width + _loadingPanePos.x, _loadingPanePos.y);
		UI.ScreenSizeDirty = false;
	}

	public void InitUI(MilMo_UserInterface ui)
	{
		UI = ui;
		_loadingPane = new MilMo_LoadingPane(UI);
		UI.AddChild(_loadingPane);
		theMenuBar = new MilMo_MenuBar(UI);
		UI.AddChild(theMenuBar);
		theMenuBar.Open();
		homeEnterText = new MilMo_HomeEnterText();
		UI.PrintMouseFocus = false;
		theQuestTracker = new MilMo_QuestTracker(UI);
		UI.AddChild(theQuestTracker);
		pvpTeamScore = new jb_PVPTeamScore(UI);
		UI.AddChild(pvpTeamScore);
		pvpZoneCountdown = new jb_PVPZoneCountdown(UI);
		UI.AddChild(pvpZoneCountdown);
		RefreshUI();
		pvpWaitingText = new MilMo_Widget(UI);
		pvpWaitingText.SetFont(MilMo_GUI.Font.EborgLarge);
		pvpWaitingText.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		pvpWaitingText.SetAlignment(MilMo_GUI.Align.CenterCenter);
		pvpWaitingText.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		pvpWaitingText.SetText(MilMo_Localization.GetLocString("PVP_9339"));
		pvpWaitingText.SetScale(300f, 50f);
		pvpWaitingText.AllowPointerFocus = false;
		pvpWaitingText.SetEnabled(e: false);
		UI.AddChild(pvpWaitingText);
	}

	public void AddRotateCameraButtons(MilMo_Button.ButtonFunc rotateLeftFunction, MilMo_Button.ButtonFunc rotateRightFunction)
	{
		_rotateSocialCamRightButton = new MilMo_Button(UI);
		_rotateSocialCamRightButton.SetTexture("Batch01/Textures/Generic/RotateRightArrow");
		_rotateSocialCamRightButton.FixedRes = true;
		_rotateSocialCamRightButton.SetHoverTexture("Batch01/Textures/Generic/RotateRightArrow");
		_rotateSocialCamRightButton.SetPressedTexture("Batch01/Textures/Generic/RotateRightArrow");
		_rotateSocialCamRightButton.SetAlignment(MilMo_GUI.Align.BottomLeft);
		_rotateSocialCamRightButton.SetPosition((float)Screen.width - RotateCameraButtonsScale.x + RotateCameraButtonsOffset.x, (float)Screen.height + RotateCameraButtonsOffset.y - 75f);
		_rotateSocialCamRightButton.SetScale(RotateCameraButtonsScale);
		_rotateSocialCamRightButton.SetScalePull(0.05f, 0.05f);
		_rotateSocialCamRightButton.SetScaleDrag(0.6f, 0.7f);
		_rotateSocialCamRightButton.SetHoverSound(_tickSound);
		_rotateSocialCamRightButton.UseParentAlpha = false;
		_rotateSocialCamRightButton.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		_rotateSocialCamRightButton.Function = rotateRightFunction;
		_rotateSocialCamRightButton.Tooltip = new MilMo_Tooltip(MilMo_Localization.GetLocString("World_6817"));
		UI.AddChild(_rotateSocialCamRightButton);
		_rotateSocialCamLeftButton = new MilMo_Button(UI);
		_rotateSocialCamLeftButton.FixedRes = true;
		_rotateSocialCamLeftButton.SetTexture("Batch01/Textures/Generic/RotateLeftArrow");
		_rotateSocialCamLeftButton.SetHoverTexture("Batch01/Textures/Generic/RotateLeftArrow");
		_rotateSocialCamLeftButton.SetPressedTexture("Batch01/Textures/Generic/RotateLeftArrow");
		_rotateSocialCamLeftButton.SetAlignment(MilMo_GUI.Align.BottomRight);
		_rotateSocialCamLeftButton.SetPosition((float)Screen.width - RotateCameraButtonsScale.x + RotateCameraButtonsOffset.x, (float)Screen.height + RotateCameraButtonsOffset.y - 75f);
		_rotateSocialCamLeftButton.SetScale(RotateCameraButtonsScale);
		_rotateSocialCamLeftButton.SetScalePull(0.05f, 0.05f);
		_rotateSocialCamLeftButton.SetScaleDrag(0.6f, 0.7f);
		_rotateSocialCamLeftButton.SetHoverSound(_tickSound);
		_rotateSocialCamLeftButton.UseParentAlpha = false;
		_rotateSocialCamLeftButton.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		_rotateSocialCamLeftButton.Function = rotateLeftFunction;
		_rotateSocialCamLeftButton.Tooltip = new MilMo_Tooltip(MilMo_Localization.GetLocString("World_6817"));
		UI.AddChild(_rotateSocialCamLeftButton);
	}

	public void RemoveRotateCameraButtons()
	{
		if (_rotateSocialCamLeftButton != null)
		{
			UI.RemoveChild(_rotateSocialCamLeftButton);
			_rotateSocialCamLeftButton = null;
		}
		if (_rotateSocialCamRightButton != null)
		{
			UI.RemoveChild(_rotateSocialCamRightButton);
			_rotateSocialCamRightButton = null;
		}
	}

	public void AddLeaveChatroomButton(MilMo_Button.ButtonFunc leaveFunction)
	{
		_leaveChatRoomButton = new MilMo_Button(UI);
		_leaveChatRoomButton.FixedRes = true;
		_leaveChatRoomButton.SetAlignment(MilMo_GUI.Align.BottomLeft);
		_leaveChatRoomButton.SetTexture("Batch01/Textures/HUD/IconLeave");
		_leaveChatRoomButton.SetHoverTexture("Batch01/Textures/HUD/IconLeave");
		_leaveChatRoomButton.SetPressedTexture("Batch01/Textures/HUD/IconLeave");
		_leaveChatRoomButton.SetPosition((float)Screen.width + LeaveChatroomButtonOffset.x, LeaveChatroomButtonOffset.y);
		_leaveChatRoomButton.SetScale(LeaveChatroomButtonScale);
		_leaveChatRoomButton.SetScalePull(0.05f, 0.05f);
		_leaveChatRoomButton.SetScaleDrag(0.6f, 0.7f);
		_leaveChatRoomButton.SetHoverSound(_tickSound);
		_leaveChatRoomButton.UseParentAlpha = false;
		_leaveChatRoomButton.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		_leaveChatRoomButton.SetFont(MilMo_GUI.Font.EborgSmall);
		_leaveChatRoomButton.Function = leaveFunction;
		_leaveChatRoomButton.Tooltip = new MilMo_Tooltip(MilMo_Localization.GetLocString("World_6818"));
		UI.AddChild(_leaveChatRoomButton);
	}

	public void RemoveLeaveChatroomButton()
	{
		if (_leaveChatRoomButton != null)
		{
			UI.RemoveChild(_leaveChatRoomButton);
			_leaveChatRoomButton = null;
		}
	}

	public Vector2 GetHudElementPosition(MilMo_Widget element)
	{
		if (element != null)
		{
			return GetHudElementPosition(element.Identifier);
		}
		return MilMo_GUI.InvalidVector2;
	}

	public Vector2 GetHudElementPosition(MilMo_GameDialog.HudDestination destination)
	{
		return GetHudElementPosition(destination.ToString());
	}

	public static Vector2 GetHudElementPosition(string element)
	{
		return element switch
		{
			"HealthBar" => new Vector2(100f, 32f), 
			"Map" => new Vector2(Screen.width - 71, 55f), 
			"MedalBoard" => new Vector2(Screen.width - 391, 35f), 
			"QuestLog" => new Vector2(Screen.width - 332, 35f), 
			"Homes" => new Vector2(Screen.width - 272, 35f), 
			"HomesInNav" => new Vector2(90f, 35f), 
			"FriendList" => new Vector2(Screen.width - 217, 35f), 
			"CombatTimer" => new Vector2(Screen.width - 145, 95f), 
			"Shop" => new Vector2(Screen.width - 252, 35f), 
			"Town" => new Vector2(Screen.width - 152, 35f), 
			"FullScreen" => new Vector2(Screen.width - 18, 18f), 
			"ChatBar" => new Vector2(270f, Screen.height - 16), 
			"Ability1" => new Vector2(Screen.width - 330, Screen.height - 30), 
			"Ability2" => new Vector2(Screen.width - 270, Screen.height - 30), 
			"Potion" => new Vector2(Screen.width - 210, Screen.height - 30), 
			"Weapon" => new Vector2(Screen.width - 150, Screen.height - 30), 
			"Bag" => new Vector2(Screen.width - 80, Screen.height - 43), 
			"LeaveChair" => new Vector2((float)Screen.width + LeaveChatroomButtonOffset.x + 10f, LeaveChatroomButtonOffset.y - LeaveChatroomButtonScale.y / 2f), 
			"ChatRooms" => new Vector2(160f, 60f), 
			"Adventures" => new Vector2(48f, Screen.height - 48), 
			"Storage" => new Vector2(75f, 75f), 
			"FurnishingMode" => new Vector2(45f, 155f), 
			"Options" => new Vector2(Screen.width - 21, 100f), 
			_ => MilMo_GUI.InvalidVector2, 
		};
	}

	public void SendWidgetToHud(MilMo_Widget sentWidget, MilMo_GameDialog.HudDestination hudDestination, bool shakeTarget)
	{
		Vector2 targetPos = MilMo_World.HudHandler.GetHudElementPosition(hudDestination);
		Rect screenPosition = sentWidget.GetScreenPosition();
		screenPosition.x += sentWidget.ScaleMover.Target.x / 2f;
		screenPosition.y += sentWidget.ScaleMover.Target.y / 2f;
		UI.AddChild(sentWidget);
		sentWidget.SetPosition(screenPosition.x, screenPosition.y);
		sentWidget.GoTo(screenPosition.x - 30f, screenPosition.y - 30f);
		MilMo_EventSystem.At(0.3f, delegate
		{
			sentWidget.SetPosPull(0.04f, 0.04f);
			sentWidget.SetPosDrag(0.7f, 0.7f);
			sentWidget.GoTo((float)Screen.width / 1.5f, (float)Screen.height / 2f);
			if (targetPos.y < (float)Screen.height / 2f)
			{
				sentWidget.Impulse(0f, 50f);
			}
			else
			{
				sentWidget.Impulse(0f, -50f);
			}
			UI.SoundFx.Play(_swooshClip);
		});
		MilMo_EventSystem.At(0.5f, delegate
		{
			sentWidget.SetPosPull(0.04f, 0.04f);
			sentWidget.SetPosDrag(0.7f, 0.7f);
			sentWidget.GoTo(targetPos);
			sentWidget.SetAngle(-180f);
			sentWidget.SetScalePull(0.04f, 0.04f);
			sentWidget.SetScaleDrag(0.7f, 0.7f);
			sentWidget.ScaleTo(32f, 32f);
			sentWidget.AlphaTo(0f);
			sentWidget.SetFadeSpeed(0.04f);
			MilMo_EventSystem.At(2f, delegate
			{
				sentWidget.SetEnabled(e: false);
			});
		});
		MilMo_EventSystem.At(1f, delegate
		{
			if (shakeTarget && hudDestination == MilMo_GameDialog.HudDestination.Bag)
			{
				GameEvent.ImpulseBagButtonEvent.RaiseEvent();
			}
			if (hudDestination == MilMo_GameDialog.HudDestination.Bag)
			{
				UI.SoundFx.Play(_wieldableEmptySound);
			}
			else
			{
				UI.SoundFx.Play(_rewardTickSound);
			}
		});
		sentWidget.Angle(90f);
	}

	public void SendWidgetToHud(MilMo_Widget sentWidget, MilMo_GameDialog.HudDestination hudDestination)
	{
		SendWidgetToHud(sentWidget, hudDestination, shakeTarget: true);
	}

	public void ShowVoting(int points, bool canVote, float currentScore)
	{
		if (_ratingVisualization != null)
		{
			_ratingVisualization.SetEnabled(e: false);
		}
		if (_votingVisualization != null)
		{
			_votingVisualization.SetEnabled(e: true);
			MilMo_World.HudHandler.UI.RemoveChild(_votingVisualization);
			_votingVisualization.RemoveAllChildren();
			_votingVisualization = null;
		}
		string text = "Batch01/Textures/Voting/star";
		_votingVisualization = new MilMo_VotingVisualization(UI, MilMo_VoteManager.VoteTypes.HOMES, int.Parse(MilMo_Home.CurrentHome.OwnerID), text + "Outline", text + "Filled", text + "Empty", new Vector2(0f, 0f), points, 5, canVote, currentScore);
		MilMo_World.HudHandler.UI.AddChild(_votingVisualization);
	}
}
