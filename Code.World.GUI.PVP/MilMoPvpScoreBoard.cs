using System.Collections.Generic;
using System.Linq;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Network.messages.server.PVP;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using Code.World.Level;
using Code.World.Level.LevelInfo;
using Code.World.Level.PVP;
using Code.World.Player;
using Core.GameEvent;
using UnityEngine;

namespace Code.World.GUI.PVP;

public class MilMoPvpScoreBoard : MilMo_SimpleBox
{
	private readonly MilMo_LocString m_TimeLeftString;

	private readonly MilMo_LocString m_RestartingInString;

	private MilMo_SimpleBox m_TeamBackBox;

	private MilMo_Widget m_TitleText;

	private MilMo_ScrollView m_TeamScroller;

	private MilMo_ScrollView m_Scroller;

	private jb_PVPScoreField m_CurrentPlayerField;

	private MilMo_Widget m_RankText;

	private MilMo_Widget m_PlayerText;

	private MilMo_Widget m_KillsText;

	private MilMo_Widget m_DeathsText;

	private MilMo_Widget m_TimerText;

	private MilMo_Widget m_Objectives;

	private MilMo_Button m_LeaveButton;

	private bool m_IsActive;

	private MilMo_TimerEvent m_DisableSchedule;

	private readonly MilMo_GenericReaction m_ToggleReaction;

	private IList<ScoreBoardEntry> m_Scores;

	private IList<TeamScoreEntry> m_TeamScores;

	public IList<ScoreBoardEntry> Scores
	{
		get
		{
			return m_Scores;
		}
		set
		{
			m_Scores = value;
		}
	}

	public IList<TeamScoreEntry> TeamScores
	{
		get
		{
			return m_TeamScores;
		}
		set
		{
			m_TeamScores = value;
		}
	}

	public MilMoPvpScoreBoard(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "PVPScoreBoard";
		m_ToggleReaction = MilMo_EventSystem.Listen("button_TogglePVPScoreBoard", delegate
		{
			if (MilMo_Level.CurrentLevel != null && MilMo_LevelInfo.IsPvp(MilMo_Level.CurrentLevel.VerboseName))
			{
				Toggle();
			}
		});
		m_ToggleReaction.Repeating = true;
		m_TimeLeftString = MilMo_Localization.GetLocString("PVP_9337").GetCopy();
		m_RestartingInString = MilMo_Localization.GetLocString("PVP_9338").GetCopy();
		UI.ResetLayout(10f, 10f);
		SetupBox();
		CreateTeamScoreWidgets();
		CreatePlayerScoreWidgets();
		CreateTimer();
		CreateLeaveButton();
		Close(null);
	}

	private void SetupBox()
	{
		SetFont(MilMo_GUI.Font.EborgLarge);
		SetTextOffset(0f, -35f);
		SetTextDropShadowPos(2f, 2f);
		TextOutline = new Vector2(1f, 1f);
		TextOutlineColor = new Color(0f, 0f, 0f, 0.3f);
		SetScale(650f, 500f);
		SetScalePull(0.08f, 0.08f);
		SetScaleDrag(0.5f, 0.5f);
		SetEnabled(e: false);
		SetPosPull(0.08f, 0.08f);
		SetPosDrag(0.7f, 0.7f);
		SetColor(Color.black);
		SetDefaultColor(Color.black);
		SetSkin(2);
		AllowPointerFocus = false;
	}

	private void CreateTeamScoreWidgets()
	{
		m_TeamBackBox = new MilMo_SimpleBox(UI);
		m_TeamBackBox.SetPosition(40f, 10f);
		m_TeamBackBox.SetScale(570f, 150f);
		m_TeamBackBox.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_TeamBackBox.AllowPointerFocus = false;
		m_TeamBackBox.SetColor(Color.black);
		m_TeamBackBox.SetDefaultColor(Color.black);
		m_TeamBackBox.SetSkin(2);
		AddChild(m_TeamBackBox);
		m_TitleText = new MilMo_Widget(UI);
		m_TitleText.SetFont(MilMo_GUI.Font.EborgXL);
		m_TitleText.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_TitleText.SetScale(570f, 20f);
		m_TitleText.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_TitleText.SetPosition(40f, 20f);
		m_TitleText.SetText(MilMo_Localization.GetLocString("PVP_9383"));
		m_TitleText.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		m_TitleText.AllowPointerFocus = false;
		AddChild(m_TitleText);
		m_TeamScroller = new MilMo_ScrollView(UI);
		m_TeamScroller.MShowHorizBar = false;
		m_TeamScroller.HasBackground(b: false);
		m_TeamScroller.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		m_TeamScroller.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_TeamScroller.SetTextOffset(0f, -30f);
		m_TeamScroller.SetTextDropShadowPos(2f, 2f);
		m_TeamScroller.SetPosition(40f, 50f);
		m_TeamScroller.SetScale(570f, 100f);
		m_TeamScroller.SetScalePull(0.08f, 0.08f);
		m_TeamScroller.SetScaleDrag(0.5f, 0.5f);
		m_TeamScroller.SetPosPull(0.08f, 0.08f);
		m_TeamScroller.SetPosDrag(0.7f, 0.7f);
		m_TeamScroller.AllowPointerFocus = false;
		AddChild(m_TeamScroller);
	}

	private void CreatePlayerScoreWidgets()
	{
		float num = 30f;
		m_Scroller = new MilMo_ScrollView(UI);
		m_Scroller.MShowHorizBar = false;
		m_Scroller.HasBackground(b: false);
		m_Scroller.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		m_Scroller.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Scroller.SetTextOffset(0f, -30f);
		m_Scroller.SetTextDropShadowPos(2f, 2f);
		m_Scroller.SetPosition(10f + num, 200f);
		m_Scroller.SetScale(586f, 250f);
		m_Scroller.SetScalePull(0.08f, 0.08f);
		m_Scroller.SetScaleDrag(0.5f, 0.5f);
		m_Scroller.SetPosPull(0.08f, 0.08f);
		m_Scroller.SetPosDrag(0.7f, 0.7f);
		m_Scroller.AllowPointerFocus = false;
		AddChild(m_Scroller);
		m_RankText = new MilMo_Widget(UI);
		m_RankText.SetFont(MilMo_GUI.Font.EborgMedium);
		m_RankText.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_RankText.SetScale(40f, 50f);
		m_RankText.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_RankText.SetPosition(10f + num, 50f);
		m_RankText.SetText(MilMo_Localization.GetLocString("PVP_9385"));
		m_RankText.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		m_RankText.AllowPointerFocus = false;
		AddChild(m_RankText);
		m_PlayerText = new MilMo_Widget(UI);
		m_PlayerText.SetFont(MilMo_GUI.Font.EborgMedium);
		m_PlayerText.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_PlayerText.SetScale(140f, 50f);
		m_PlayerText.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_PlayerText.SetPosition(40f + num, 165f);
		m_PlayerText.SetText(MilMo_Localization.GetLocString("PVP_9327"));
		m_PlayerText.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		m_PlayerText.AllowPointerFocus = false;
		AddChild(m_PlayerText);
		m_Objectives = new MilMo_Widget(UI);
		m_Objectives.SetFont(MilMo_GUI.Font.EborgMedium);
		m_Objectives.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_Objectives.SetScale(50f, 40f);
		m_Objectives.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Objectives.SetPosition(300f + num, 165f);
		m_Objectives.SetText(MilMo_Localization.GetLocString("PVP_9392"));
		m_Objectives.SetExtraDrawTextSize(200f, 0f);
		m_Objectives.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		m_Objectives.AllowPointerFocus = false;
		AddChild(m_Objectives);
		m_Objectives.SetEnabled(e: false);
		m_KillsText = new MilMo_Widget(UI);
		m_KillsText.SetFont(MilMo_GUI.Font.EborgMedium);
		m_KillsText.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_KillsText.SetScale(50f, 40f);
		m_KillsText.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_KillsText.SetPosition(400f + num, 165f);
		m_KillsText.SetText(MilMo_Localization.GetLocString("PVP_9332"));
		m_KillsText.SetExtraDrawTextSize(200f, 0f);
		m_KillsText.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		m_KillsText.AllowPointerFocus = false;
		AddChild(m_KillsText);
		m_DeathsText = new MilMo_Widget(UI);
		m_DeathsText.SetFont(MilMo_GUI.Font.EborgMedium);
		m_DeathsText.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_DeathsText.SetScale(50f, 45f);
		m_DeathsText.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_DeathsText.SetPosition(500f + num, 165f);
		m_DeathsText.SetText(MilMo_Localization.GetLocString("PVP_9331"));
		m_DeathsText.SetExtraDrawTextSize(200f, 0f);
		m_DeathsText.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		m_DeathsText.AllowPointerFocus = false;
		AddChild(m_DeathsText);
	}

	private void CreateTimer()
	{
		m_TimerText = new MilMo_Widget(UI);
		m_TimerText.SetFont(MilMo_GUI.Font.EborgMedium);
		m_TimerText.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_TimerText.SetScale(650f, 45f);
		m_TimerText.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_TimerText.SetPosition(0f, 460f);
		m_TimerText.SetText(m_TimeLeftString);
		m_TimerText.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		m_TimerText.AllowPointerFocus = false;
		AddChild(m_TimerText);
	}

	private void CreateLeaveButton()
	{
		m_LeaveButton = new MilMo_Button(UI);
		m_LeaveButton.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_LeaveButton.SetPosition(Scale.x - 140f, Scale.y - 130f);
		m_LeaveButton.SetTexture("Batch01/Textures/Core/Invisible");
		m_LeaveButton.SetText(MilMo_Localization.GetLocString("PVP_9370"));
		m_LeaveButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		m_LeaveButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		m_LeaveButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		m_LeaveButton.SetScale(130f, 40f);
		m_LeaveButton.SetFont(MilMo_GUI.Font.EborgSmall);
		m_LeaveButton.SetEnabled(e: false);
		m_LeaveButton.Function = delegate
		{
			GameEvent.OpenTownEvent.RaiseEvent();
		};
		AddChild(m_LeaveButton);
	}

	private void RefreshUI()
	{
		SetPosition((float)(Screen.width / 2) - UI.GlobalInputOffset.x + 50f, (float)(Screen.height / 2) - UI.GlobalInputOffset.y);
	}

	public override void Draw()
	{
		if (UI.ScreenSizeDirty)
		{
			RefreshUI();
		}
		SetXPos((float)(Screen.width / 2) - UI.GlobalInputOffset.x);
		base.Draw();
	}

	public void Toggle()
	{
		if (!m_IsActive)
		{
			Open();
		}
		else
		{
			Close(0);
		}
	}

	public void Refresh()
	{
		RefreshWidgets();
		SetCurrentPlayer(MilMo_Player.Instance.Avatar.Name);
	}

	public void RefreshWidgets()
	{
		UI.ResetLayout(10f, 4f);
		UI.SetNext(10f, 0f);
		m_Scroller.RemoveAllChildren();
		m_TeamScroller.RemoveAllChildren();
		int num = 0;
		if (m_Scores == null)
		{
			return;
		}
		UpdateCommonState();
		if (MilMo_Level.CurrentLevel.PvpHandler.IsTeamMode)
		{
			SetTeamScoreState();
			int num2 = 0;
			int num3 = -1;
			foreach (TeamScoreEntry teamScore in m_TeamScores)
			{
				jb_PVPTeamScoreField jb_PVPTeamScoreField2 = new jb_PVPTeamScoreField(UI, m_TeamScores.Count);
				if (num3 != teamScore.GetRoundsWon())
				{
					num3 = teamScore.GetRoundsWon();
					num2++;
				}
				jb_PVPTeamScoreField2.Fill(num2, teamScore);
				m_TeamScroller.AddChild(jb_PVPTeamScoreField2);
			}
			m_TeamScroller.RefreshViewSize();
			UI.SetNext(10f, 0f);
			foreach (ScoreBoardEntry score in m_Scores)
			{
				num++;
				jb_PVPScoreField jb_PVPScoreField2 = new jb_PVPScoreField(UI);
				jb_PVPScoreField2.Fill(num, score.GetAvatarName(), score.GetKills(), score.GetDeaths());
				Color teamColor = MilMo_Level.CurrentLevel.PvpHandler.GetTeamColor(score.GetPlayerID());
				jb_PVPScoreField2.SetColor(teamColor);
				if (MilMo_Level.CurrentLevel.PvpHandler.MatchMode.Equals(MilMo_MatchMode.CAPTURE_THE_FLAG) || MilMo_Level.CurrentLevel.PvpHandler.MatchMode.Equals(MilMo_MatchMode.KING_OF_THE_HILL))
				{
					jb_PVPScoreField2.FillObjective(score.GetObjectives());
				}
				m_Scroller.AddChild(jb_PVPScoreField2);
			}
		}
		else
		{
			SetPlayerScoreState();
			foreach (ScoreBoardEntry score2 in m_Scores)
			{
				num++;
				jb_PVPScoreField jb_PVPScoreField3 = new jb_PVPScoreField(UI);
				jb_PVPScoreField3.Fill(num, score2.GetAvatarName(), score2.GetKills(), score2.GetDeaths());
				m_Scroller.AddChild(jb_PVPScoreField3);
			}
		}
		m_Scroller.RefreshViewSize();
	}

	private void UpdateCommonState()
	{
		if (MilMo_Level.CurrentLevel.PvpHandler.MatchHasEnded)
		{
			if (MilMo_Level.CurrentLevel.PvpHandler.IsWinner())
			{
				m_TitleText.SetText(MilMo_Localization.GetLocString("PVP_9396"));
			}
			else
			{
				m_TitleText.SetText(MilMo_Localization.GetLocString("PVP_9384"));
			}
			m_LeaveButton.SetEnabled(e: true);
		}
		else
		{
			m_TitleText.SetText(MilMo_Localization.GetLocString("PVP_9383"));
			m_LeaveButton.SetEnabled(e: false);
		}
		m_DeathsText.SetEnabled(e: true);
		if (MilMo_Level.CurrentLevel.PvpHandler.MatchMode.Equals(MilMo_MatchMode.BATTLE_ROYALE))
		{
			m_DeathsText.SetEnabled(e: false);
		}
		m_Objectives.SetEnabled(e: false);
	}

	private void SetTeamScoreState()
	{
		m_TeamBackBox.SetEnabled(e: true);
		m_TeamScroller.SetEnabled(e: true);
		m_RankText.SetEnabled(e: false);
		SetScale(650f, 500f);
		m_TimerText.SetPosition(0f, 460f);
		m_Scroller.SetScale(586f, 250f);
		m_TeamBackBox.SetScale(570f, 150f);
		SetPositions(120f, 0f);
		if (m_TeamScores.Count == 2)
		{
			m_TeamBackBox.SetScale(570f, 120f);
			SetPositions(90f, 0f);
		}
		if (MilMo_Level.CurrentLevel.PvpHandler.MatchMode.Equals(MilMo_MatchMode.CAPTURE_THE_FLAG) || MilMo_Level.CurrentLevel.PvpHandler.MatchMode.Equals(MilMo_MatchMode.KING_OF_THE_HILL))
		{
			m_Objectives.SetEnabled(e: true);
		}
		UI.ResetLayout(0f, 4f);
		UI.SetNext(0f, 0f);
	}

	private void SetPlayerScoreState()
	{
		m_TeamBackBox.SetEnabled(e: false);
		m_TeamScroller.SetEnabled(e: false);
		m_RankText.SetEnabled(e: true);
		SetScale(650f, 420f);
		m_TimerText.SetPosition(0f, 380f);
		m_Scroller.SetScale(586f, 290f);
		SetPositions(0f, 60f);
		UI.ResetLayout(10f, 4f);
		UI.SetNext(10f, 0f);
	}

	private void SetPositions(float yOffset, float playerOffset)
	{
		m_Scroller.SetPosition(40f, 80f + yOffset);
		m_PlayerText.SetPosition(70f + playerOffset, 45f + yOffset);
		m_Objectives.SetPosition(330f, 45f + yOffset);
		m_KillsText.SetPosition(430f, 45f + yOffset);
		m_DeathsText.SetPosition(530f, 45f + yOffset);
	}

	public void DummyOpen()
	{
		m_TeamScores = new List<TeamScoreEntry>();
		m_Scores = new List<ScoreBoardEntry>();
		for (int i = 0; i < 12; i++)
		{
			ScoreBoardEntry item = new ScoreBoardEntry(MilMo_Utility.RandomID().ToString(), MilMo_Utility.RandomID().ToString(), MilMo_Utility.RandomInt(0, 100), i, 0, 0);
			m_Scores.Add(item);
		}
		for (int j = 0; j < 4; j++)
		{
			List<string> range = m_Scores.Select((ScoreBoardEntry s) => s.GetPlayerID()).ToList().GetRange(j * 3, 3);
			color teamColor = new color(MilMo_Utility.RandomFloat(0f, 1f), MilMo_Utility.RandomFloat(0f, 1f), MilMo_Utility.RandomFloat(0f, 1f), 1f);
			TeamScoreEntry item2 = new TeamScoreEntry(new NetworkTeam(range, teamColor, "The Marauders"), MilMo_Utility.RandomInt(0, 100), MilMo_Utility.RandomInt(0, 100), MilMo_Utility.RandomInt(0, 100), MilMo_Utility.RandomInt(0, 100));
			m_TeamScores.Add(item2);
		}
		Open();
	}

	public void Open()
	{
		if (MilMo_Level.CurrentLevel != null && MilMo_Level.CurrentLevel.IsPvp())
		{
			if (MilMo_Level.CurrentLevel.PvpHandler.IsTeamMode)
			{
				MilMo_World.HudHandler.pvpTeamScore.SetEnabled(e: false);
			}
			Refresh();
			m_IsActive = true;
			if (m_DisableSchedule != null)
			{
				MilMo_EventSystem.RemoveTimerEvent(m_DisableSchedule);
			}
			SetEnabled(e: true);
			SetPosition((float)Screen.width / 2f - UI.GlobalInputOffset.x + 50f, -500f);
			GoTo((float)Screen.width / 2f - UI.GlobalInputOffset.x + 50f, (float)Screen.height / 2f - UI.GlobalInputOffset.y);
			AlphaTo(1f);
			UpdatePVPScoreBoardTime();
			MilMo_World.HudHandler.theMenuBar.ToggledScoreBoard(open: true);
		}
	}

	public void Close(object obj)
	{
		m_IsActive = false;
		UI.ResetLayout(10f, 10f);
		GoTo((float)(Screen.width / 2) - UI.GlobalInputOffset.x + 50f, -500f);
		m_DisableSchedule = MilMo_EventSystem.At(1f, delegate
		{
			SetEnabled(e: false);
		});
		AlphaTo(0f);
		if (MilMo_Level.CurrentLevel != null && MilMo_Level.CurrentLevel.IsPvp() && MilMo_Level.CurrentLevel.PvpHandler.IsTeamMode)
		{
			MilMo_World.HudHandler.pvpTeamScore.SetEnabled(e: true);
		}
		MilMo_World.HudHandler.theMenuBar.ToggledScoreBoard(open: false);
	}

	public void SetCurrentPlayer(string playerName)
	{
		if (m_CurrentPlayerField != null)
		{
			m_CurrentPlayerField.DeSelect();
		}
		foreach (jb_PVPScoreField child in m_Scroller.Children)
		{
			if (child.Name.Text == MilMo_Localization.GetLocString(playerName))
			{
				child.Select();
				m_CurrentPlayerField = child;
			}
		}
	}

	private void UpdatePVPScoreBoardTime()
	{
		if (!m_IsActive)
		{
			return;
		}
		if (MilMo_Level.CurrentLevel != null && MilMo_LevelInfo.IsPvp(MilMo_Level.CurrentLevel.VerboseName))
		{
			if (MilMo_Level.CurrentLevel.PvpHandler.MatchState == MilMo_PVPHandler.MilMo_MatchState.Waiting)
			{
				m_TimerText.SetTextNoLocalization("");
			}
			else
			{
				int num = Mathf.Max(MilMo_Level.CurrentLevel.PvpHandler.SecondsToNextMatchState / 60, 0);
				int num2 = Mathf.Max(MilMo_Level.CurrentLevel.PvpHandler.SecondsToNextMatchState - num * 60, 0);
				string text = num.ToString();
				string text2 = num2.ToString();
				if (num < 10)
				{
					text = "0" + text;
				}
				if (num2 < 10)
				{
					text2 = "0" + text2;
				}
				MilMo_LocString milMo_LocString = ((MilMo_Level.CurrentLevel.PvpHandler.MatchState == MilMo_PVPHandler.MilMo_MatchState.Scoreboard) ? m_RestartingInString : m_TimeLeftString);
				milMo_LocString.SetFormatArgs(text + ":" + text2);
				m_TimerText.SetText(milMo_LocString);
			}
		}
		MilMo_EventSystem.At(1f, UpdatePVPScoreBoardTime);
	}
}
