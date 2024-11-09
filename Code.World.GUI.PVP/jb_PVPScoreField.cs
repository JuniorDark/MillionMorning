using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.World.Level;
using UnityEngine;

namespace Code.World.GUI.PVP;

public class jb_PVPScoreField : MilMo_Widget
{
	public MilMo_Widget Name;

	private readonly MilMo_Widget m_Rank;

	private readonly MilMo_Widget m_Kills;

	private readonly MilMo_Widget m_Deaths;

	private readonly MilMo_Widget m_Dead;

	private readonly MilMo_Widget m_Objectives;

	private readonly Vector2 m_FieldScale = new Vector2(570f, 30f);

	public jb_PVPScoreField(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "PVPScoreField " + MilMo_UserInterface.GetRandomID();
		GoToNow(0f, UI.Next.y);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetDefaultColor(0.6f, 0.6f, 0.6f, 1f);
		m_Rank = new MilMo_Widget(UI);
		m_Rank.GoToNow(0f, 0f);
		m_Rank.ScaleNow(40f, m_FieldScale.y);
		m_Rank.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Rank.Step();
		m_Rank.SetText(MilMo_Localization.GetNotLocalizedLocString("10"));
		m_Rank.SetFont(MilMo_GUI.Font.EborgMedium);
		m_Rank.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_Rank.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		AddChild(m_Rank);
		m_Dead = new MilMo_Widget(UI);
		m_Dead.SetTexture("Content/Items/Batch01/SpecialAbilities/IconDamage", prefixStandardGuiPath: false);
		m_Dead.GoToNow(75f, 0f);
		m_Dead.SetAlignment(MilMo_GUI.Align.TopCenter);
		m_Dead.SetScale(30f, m_FieldScale.y);
		m_Dead.AllowPointerFocus = false;
		AddChild(m_Dead);
		m_Dead.SetEnabled(e: false);
		Name = new MilMo_Widget(UI);
		Name.GoToNow(90f, 0f);
		Name.SetAlignment(MilMo_GUI.Align.TopLeft);
		Name.SetScale(140f, m_FieldScale.y);
		Name.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		Name.SetFont(MilMo_GUI.Font.EborgMedium);
		Name.SetTextDropShadowPos(1f, 1f);
		Name.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		AddChild(Name);
		m_Objectives = new MilMo_Widget(UI);
		m_Objectives.GoToNow(290f, 0f);
		m_Objectives.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Objectives.SetScale(50f, m_FieldScale.y);
		m_Objectives.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		m_Objectives.SetFont(MilMo_GUI.Font.EborgMedium);
		m_Objectives.SetTextDropShadowPos(1f, 1f);
		m_Objectives.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		AddChild(m_Objectives);
		m_Objectives.SetEnabled(e: false);
		m_Kills = new MilMo_Widget(UI);
		m_Kills.GoToNow(390f, 0f);
		m_Kills.ScaleNow(50f, m_FieldScale.y);
		m_Kills.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Kills.Step();
		m_Kills.SetText(MilMo_Localization.GetNotLocalizedLocString("10"));
		m_Kills.SetFont(MilMo_GUI.Font.EborgMedium);
		m_Kills.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_Kills.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		AddChild(m_Kills);
		m_Deaths = new MilMo_Widget(UI);
		m_Deaths.GoToNow(490f, 0f);
		m_Deaths.ScaleNow(50f, m_FieldScale.y);
		m_Deaths.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Deaths.Step();
		m_Deaths.SetText(MilMo_Localization.GetNotLocalizedLocString("10"));
		m_Deaths.SetFont(MilMo_GUI.Font.EborgMedium);
		m_Deaths.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_Deaths.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		AddChild(m_Deaths);
		SetScale(m_FieldScale);
	}

	public void Fill(int rank, string player, int kills, int deaths)
	{
		m_Rank.SetEnabled(e: true);
		Name.GoToNow(90f, 0f);
		m_Dead.GoToNow(75f, 0f);
		if (MilMo_Level.CurrentLevel.PvpHandler.IsTeamMode)
		{
			m_Rank.SetEnabled(e: false);
			Name.GoToNow(30f, 0f);
			m_Dead.GoToNow(15f, 0f);
		}
		m_Rank.SetText(MilMo_Localization.GetNotLocalizedLocString(rank.ToString()));
		Name.SetText(MilMo_Localization.GetNotLocalizedLocString(player));
		m_Kills.SetText(MilMo_Localization.GetNotLocalizedLocString(kills.ToString()));
		m_Deaths.SetText(MilMo_Localization.GetNotLocalizedLocString(deaths.ToString()));
		m_Deaths.SetEnabled(e: true);
		if (MilMo_Level.CurrentLevel.PvpHandler.MatchMode == MilMo_MatchMode.BATTLE_ROYALE)
		{
			m_Deaths.SetEnabled(e: false);
			m_Dead.SetEnabled(e: false);
			if (deaths > 0)
			{
				m_Dead.SetEnabled(e: true);
			}
		}
	}

	public void FillObjective(int objectives)
	{
		m_Objectives.SetEnabled(e: true);
		m_Objectives.SetText(MilMo_Localization.GetNotLocalizedLocString(objectives.ToString()));
	}

	public new void SetColor(Color teamColor)
	{
		m_Rank.SetDefaultTextColor(teamColor);
		Name.SetDefaultTextColor(teamColor);
		m_Kills.SetDefaultTextColor(teamColor);
		m_Deaths.SetDefaultTextColor(teamColor);
		m_Objectives.SetDefaultTextColor(teamColor);
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

	public void Select()
	{
		SetTextureBlack();
	}

	public void DeSelect()
	{
		SetTextureInvisible();
	}
}
