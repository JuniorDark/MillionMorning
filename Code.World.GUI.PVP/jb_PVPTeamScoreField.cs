using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.World.Level;
using UnityEngine;

namespace Code.World.GUI.PVP;

public class jb_PVPTeamScoreField : MilMo_Widget
{
	private readonly Vector2 m_FieldScale = new Vector2(554f, 30f);

	private readonly MilMo_Widget m_Name;

	private readonly MilMo_Widget m_Rank;

	private readonly MilMo_Widget m_RoundScore;

	private readonly MilMo_Widget m_Kills;

	private readonly MilMo_Widget m_Deaths;

	private readonly MilMo_Widget m_Surrender;

	private readonly int maxRoundScore;

	private readonly string objectiveName;

	public jb_PVPTeamScoreField(MilMo_UserInterface ui, int nrOfTeams)
		: base(ui)
	{
		Identifier = "PVPTeamScoreField " + MilMo_UserInterface.GetRandomID();
		GoToNow(0f, UI.Next.y);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetDefaultColor(0.6f, 0.6f, 0.6f, 1f);
		maxRoundScore = MilMo_Level.CurrentLevel.PvpHandler.MaxRoundScore;
		objectiveName = MilMo_Level.CurrentLevel.PvpHandler.MatchMode.ObjectiveName();
		if (nrOfTeams <= 3)
		{
			m_FieldScale = new Vector2(470f, 30f);
		}
		float num = 10f;
		m_Rank = new MilMo_Widget(UI);
		m_Rank.GoToNow(15f, 0f);
		m_Rank.ScaleNow(10f, m_FieldScale.y);
		m_Rank.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Rank.Step();
		m_Rank.SetFont(MilMo_GUI.Font.EborgSmall);
		m_Rank.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_Rank.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		AddChild(m_Rank);
		m_Name = new MilMo_Widget(UI);
		m_Name.GoToNow(25f + num, 0f);
		m_Name.ScaleNow(120f, m_FieldScale.y);
		m_Name.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Name.Step();
		m_Name.SetFont(MilMo_GUI.Font.EborgSmall);
		m_Name.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_Name.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		AddChild(m_Name);
		m_RoundScore = new MilMo_Widget(UI);
		m_RoundScore.GoToNow(205f + num, 0f);
		m_RoundScore.ScaleNow(115f, m_FieldScale.y);
		m_RoundScore.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_RoundScore.Step();
		m_RoundScore.SetFont(MilMo_GUI.Font.EborgSmall);
		m_RoundScore.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_RoundScore.SetTextAlignment(MilMo_GUI.Align.TopRight);
		AddChild(m_RoundScore);
		m_Kills = new MilMo_Widget(UI);
		m_Kills.GoToNow(380f + num, 0f);
		m_Kills.ScaleNow(65f, m_FieldScale.y);
		m_Kills.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Kills.Step();
		m_Kills.SetFont(MilMo_GUI.Font.EborgSmall);
		m_Kills.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_Kills.SetTextAlignment(MilMo_GUI.Align.TopRight);
		AddChild(m_Kills);
		m_Deaths = new MilMo_Widget(UI);
		m_Deaths.GoToNow(455f + num, 0f);
		if (nrOfTeams <= 3)
		{
			m_Deaths.ScaleNow(90f, m_FieldScale.y);
		}
		else
		{
			m_Deaths.ScaleNow(74f, m_FieldScale.y);
		}
		m_Deaths.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Deaths.Step();
		m_Deaths.SetFont(MilMo_GUI.Font.EborgSmall);
		m_Deaths.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_Deaths.SetTextAlignment(MilMo_GUI.Align.TopRight);
		AddChild(m_Deaths);
		m_Surrender = new MilMo_Widget(UI);
		m_Surrender.GoToNow(205f + num, 0f);
		if (nrOfTeams <= 3)
		{
			m_Surrender.ScaleNow(340f, m_FieldScale.y);
		}
		else
		{
			m_Surrender.ScaleNow(324f, m_FieldScale.y);
		}
		m_Surrender.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Surrender.Step();
		m_Surrender.SetFont(MilMo_GUI.Font.EborgLarge);
		m_Surrender.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_Surrender.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		m_Surrender.SetText(MilMo_Localization.GetLocString("PVP_9397"));
		AddChild(m_Surrender);
		m_Surrender.SetEnabled(e: false);
		SetScale(m_FieldScale);
	}

	public void Fill(int rank, TeamScoreEntry teamScore)
	{
		Color asColor = teamScore.GetTeam().GetColor().getAsColor();
		m_Rank.SetDefaultTextColor(asColor);
		m_Rank.SetText(MilMo_Localization.GetNotLocalizedLocString(rank.ToString()));
		m_Name.SetDefaultTextColor(asColor);
		m_Name.SetText(MilMo_Localization.GetNotLocalizedLocString(teamScore.GetTeam().GetName()));
		string text = teamScore.GetRoundScore() + "/" + maxRoundScore + " " + MilMo_Localization.GetLocString(objectiveName).String;
		m_RoundScore.SetDefaultTextColor(asColor);
		m_RoundScore.SetText(MilMo_Localization.GetNotLocalizedLocString(text));
		m_Kills.SetEnabled(e: true);
		if (MilMo_Level.CurrentLevel.PvpHandler.MatchMode == MilMo_MatchMode.DEATH_MATCH)
		{
			m_Kills.SetEnabled(e: false);
		}
		m_Kills.SetDefaultTextColor(asColor);
		m_Kills.SetText(MilMo_Localization.GetNotLocalizedLocString(teamScore.GetKills() + " kills"));
		m_Deaths.SetDefaultTextColor(asColor);
		m_Deaths.SetText(MilMo_Localization.GetNotLocalizedLocString(teamScore.GetDeaths() + " deaths"));
		if (MilMo_Level.CurrentLevel.PvpHandler.MatchState != 0 && teamScore.GetTeam().GetPlayers().Count == 0)
		{
			m_Surrender.SetEnabled(e: true);
			m_RoundScore.SetEnabled(e: false);
			m_Kills.SetEnabled(e: false);
			m_Deaths.SetEnabled(e: false);
		}
	}

	public override void Draw()
	{
		if (Enabled)
		{
			Color currentColor = CurrentColor;
			if (Parent != null && UseParentAlpha)
			{
				currentColor.a *= Parent.CurrentColor.a;
			}
			UnityEngine.GUI.color = currentColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
			UnityEngine.GUI.skin = Font;
			CheckPointerFocus();
			base.Draw();
		}
	}
}
