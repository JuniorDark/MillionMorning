using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.CharacterShop;

public class MilMo_ComingSoonCard : MilMo_Widget
{
	private readonly MilMo_Widget m_Picture;

	private readonly MilMo_Widget m_Caption;

	private readonly MilMo_Widget m_Txt;

	public MilMo_ComingSoonCard(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "ComingSoonCard" + MilMo_UserInterface.GetRandomID();
		m_Picture = new MilMo_Widget(UI);
		m_Picture.SetTexture("Batch01/Textures/Shop/ComingSoon");
		m_Picture.SetAlignment(MilMo_GUI.Align.TopCenter);
		m_Picture.SetDefaultColor(1f, 1f, 1f, 0.5f);
		m_Picture.FadeSpeed = 0.01f;
		m_Picture.SetPosPull(0.05f, 0.05f);
		m_Picture.SetPosDrag(0.6f, 0.6f);
		AddChild(m_Picture);
		m_Caption = new MilMo_Widget(UI);
		m_Caption.SetTexture("Batch01/Textures/Core/Invisible");
		m_Caption.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Caption.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		m_Caption.SetFont(MilMo_GUI.Font.EborgLarge);
		m_Caption.SetText(MilMo_Localization.GetLocString("CharacterShop_265"));
		m_Caption.SetDefaultTextColor(1f, 1f, 1f, 0.75f);
		m_Caption.TextOutlineColor = new Color(0f, 0f, 0f, 1f);
		m_Caption.SetPosPull(0.05f, 0.05f);
		m_Caption.SetPosDrag(0.6f, 0.6f);
		AddChild(m_Caption);
		m_Txt = new MilMo_Widget(UI);
		m_Txt.SetTexture("Batch01/Textures/Core/Invisible");
		m_Txt.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Txt.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		m_Txt.SetFont(MilMo_GUI.Font.ArialRounded);
		m_Txt.SetText(MilMo_Localization.GetLocString("CharacterShop_266"));
		m_Txt.SetTextDropShadowPos(2f, 2f);
		m_Txt.SetDefaultTextColor(1f, 0.75f, 0.2f, 0.75f);
		m_Txt.SetPosPull(0.05f, 0.05f);
		m_Txt.SetPosDrag(0.58f, 0.58f);
		AddChild(m_Txt);
		RefreshUI();
	}

	public void RefreshUI()
	{
		m_Picture.GoTo(217f, 137f);
		m_Picture.SetXScale(237f);
		float num = m_Picture.Scale.x / base.Res.x;
		num *= 1.556f;
		m_Picture.SetYScale(num / base.Res.y);
		m_Caption.GoTo(-40f, 10f);
		m_Caption.SetScale(450f, 100f);
		m_Caption.SetTextOutline(1f, 1f);
		m_Caption.SetTextDropShadowPos(3f, 3f);
		m_Txt.GoTo(-40f, 65f);
		m_Txt.SetScale(450f, 50f);
	}

	public void Show()
	{
		m_Txt.SetPosition(-450f, 65f);
		m_Caption.SetPosition(-450f, 10f);
		m_Picture.SetPosition(100f, 237f);
		m_Picture.SetAlpha(0f);
		m_Picture.AlphaTo(1f);
		RefreshUI();
	}
}
