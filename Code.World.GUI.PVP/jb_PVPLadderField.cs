using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.PVP;

public class jb_PVPLadderField : MilMo_Widget
{
	public MilMo_Widget Name;

	private readonly MilMo_Widget m_Rank;

	private readonly MilMo_Widget m_Points;

	private readonly Vector2 m_FieldScale = new Vector2(330f, 22f);

	public jb_PVPLadderField(MilMo_UserInterface ui, MilMoPvpLadderWindow ladderwindow)
		: base(ui)
	{
		Identifier = "PVPLadderField " + MilMo_UserInterface.GetRandomID();
		SetPosition(0f, UI.Next.y);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetDefaultColor(0.6f, 0.6f, 0.6f, 1f);
		float num = 10f;
		m_Rank = new MilMo_Widget(UI);
		m_Rank.GoToNow(0f + num, 0f);
		m_Rank.ScaleNow(50f, m_FieldScale.y);
		m_Rank.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Rank.Step();
		m_Rank.SetText(MilMo_Localization.GetNotLocalizedLocString("10"));
		m_Rank.SetFont(MilMo_GUI.Font.EborgSmall);
		m_Rank.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_Rank.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		m_Rank.SetExtraDrawTextSize(10f, 10f);
		AddChild(m_Rank);
		Name = new MilMo_Widget(UI);
		Name.GoToNow(60f + num, 0f);
		Name.SetAlignment(MilMo_GUI.Align.TopLeft);
		Name.SetScale(140f, m_FieldScale.y);
		Name.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		Name.SetFont(MilMo_GUI.Font.EborgSmall);
		Name.SetTextOffset(10f, 0f);
		Name.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		Name.SetExtraDrawTextSize(10f, 10f);
		AddChild(Name);
		m_Points = new MilMo_Widget(UI);
		m_Points.GoToNow(225f + num, 0f);
		m_Points.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Points.SetScale(140f, m_FieldScale.y);
		m_Points.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		m_Points.SetFont(MilMo_GUI.Font.EborgSmall);
		m_Points.SetTextOffset(10f, 0f);
		m_Points.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_Points.SetExtraDrawTextSize(10f, 10f);
		AddChild(m_Points);
		SetScale(m_FieldScale);
	}

	public void Fill(int rank, string player, int points)
	{
		m_Rank.SetText(MilMo_Localization.GetNotLocalizedLocString(rank.ToString()));
		Name.SetText(MilMo_Localization.GetNotLocalizedLocString(player));
		m_Points.SetText(MilMo_Localization.GetNotLocalizedLocString(points.ToString()));
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
		m_Rank.SetDefaultTextColor(Color.green);
		Name.SetDefaultTextColor(Color.green);
		m_Points.SetDefaultTextColor(Color.green);
		SetTextureBlack();
	}

	public void DeSelect()
	{
		m_Rank.SetDefaultTextColor(1f, 1f, 1f, 1f);
		Name.SetDefaultTextColor(1f, 1f, 1f, 1f);
		m_Points.SetDefaultTextColor(1f, 1f, 1f, 1f);
		SetTextureInvisible();
	}
}
