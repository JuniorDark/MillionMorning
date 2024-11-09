using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.World.Level;
using Code.World.Level.PVP;
using UnityEngine;

namespace Code.World.GUI.PVP;

public class jb_PVPTeamScoreFieldLite : MilMo_Widget
{
	private readonly Vector2 m_FieldScale = new Vector2(340f, 30f);

	public jb_PVPTeamScoreFieldLite(MilMo_UserInterface ui, MilMo_PVPTeam team, bool isHeader)
		: base(ui)
	{
		Identifier = "PVPTeamScoreFieldLite " + MilMo_UserInterface.GetRandomID();
		GoToNow(0f, UI.Next.y);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetDefaultColor(0.6f, 0.6f, 0.6f, 1f);
		float num = 10f;
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.GoToNow(0f + num, 0f);
		milMo_Widget.ScaleNow(140f, m_FieldScale.y);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.Step();
		milMo_Widget.SetFont(MilMo_GUI.Font.EborgMedium);
		milMo_Widget.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		milMo_Widget.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		if (isHeader)
		{
			milMo_Widget.SetText(MilMo_Localization.GetNotLocalizedLocString("Team"));
		}
		else if (team != null)
		{
			milMo_Widget.SetDefaultTextColor(team.Color);
			milMo_Widget.SetText(MilMo_Localization.GetNotLocalizedLocString(team.Name));
		}
		AddChild(milMo_Widget);
		MilMo_Widget milMo_Widget2 = new MilMo_Widget(UI);
		milMo_Widget2.GoToNow(150f + num, 0f);
		milMo_Widget2.ScaleNow(80f, m_FieldScale.y);
		milMo_Widget2.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget2.Step();
		milMo_Widget2.SetFont(MilMo_GUI.Font.EborgMedium);
		milMo_Widget2.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		milMo_Widget2.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		if (isHeader)
		{
			milMo_Widget2.SetText(MilMo_Localization.GetNotLocalizedLocString("Rounds"));
		}
		else if (team != null)
		{
			int nrOfRounds = MilMo_Level.CurrentLevel.PvpHandler.NrOfRounds;
			string text = team.RoundsWon + "/" + nrOfRounds;
			milMo_Widget2.SetText(MilMo_Localization.GetNotLocalizedLocString(text));
		}
		AddChild(milMo_Widget2);
		MilMo_Widget milMo_Widget3 = new MilMo_Widget(UI);
		milMo_Widget3.GoToNow(240f + num, 0f);
		milMo_Widget3.ScaleNow(90f, m_FieldScale.y);
		milMo_Widget3.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget3.Step();
		milMo_Widget3.SetFont(MilMo_GUI.Font.EborgMedium);
		milMo_Widget3.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		milMo_Widget3.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		if (isHeader)
		{
			string identifier = MilMo_Level.CurrentLevel.PvpHandler.MatchMode.ObjectiveName();
			milMo_Widget3.SetText(MilMo_Localization.GetLocString(identifier));
		}
		else if (team != null)
		{
			int maxRoundScore = MilMo_Level.CurrentLevel.PvpHandler.MaxRoundScore;
			string text2 = team.RoundScore + "/" + maxRoundScore;
			milMo_Widget3.SetText(MilMo_Localization.GetNotLocalizedLocString(text2));
		}
		AddChild(milMo_Widget3);
		SetScale(m_FieldScale);
	}

	public override void Step()
	{
		if (Enabled)
		{
			base.Step();
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
