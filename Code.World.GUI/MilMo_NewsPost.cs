using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI;

public sealed class MilMo_NewsPost : MilMo_Widget
{
	private MilMo_Widget m_Divider;

	private readonly GUISkin m_Skin;

	public MilMo_NewsPost(MilMo_UserInterface ui, string date, string headline, string post, Vector2 scale)
		: base(ui)
	{
		Identifier = "NewsPost " + MilMo_UserInterface.GetRandomID();
		UI = ui;
		m_Skin = UI.Skins[0];
		m_Skin.label.wordWrap = true;
		scale.x -= 35f;
		ScaleNow(scale.x, scale.y);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetTexture("Batch01/Textures/Core/Invisible");
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetScale(scale.x + 100f, 50f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.SetPosition(-50f, 0f);
		milMo_Widget.SetDefaultColor(1f, 1f, 1f, 0.3f);
		milMo_Widget.SetTexture("Batch01/Textures/Core/Black");
		AddChild(milMo_Widget);
		MilMo_SimpleLabel milMo_SimpleLabel = new MilMo_SimpleLabel(UI);
		milMo_SimpleLabel.SetText(MilMo_Localization.GetLocString(headline));
		milMo_SimpleLabel.SetScale(scale.x, 40f);
		milMo_SimpleLabel.SetPosition(0f, 0f);
		milMo_SimpleLabel.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		milMo_SimpleLabel.SetWordWrap(w: false);
		milMo_SimpleLabel.SetFont(MilMo_GUI.Font.EborgSmall);
		milMo_SimpleLabel.SetDefaultTextColor(1f, 1f, 0f, 1f);
		milMo_SimpleLabel.SetTextDropShadowPos(2f, 2f);
		AddChild(milMo_SimpleLabel);
		MilMo_SimpleLabel milMo_SimpleLabel2 = new MilMo_SimpleLabel(UI);
		milMo_SimpleLabel2.SetText(MilMo_Localization.GetNotLocalizedLocString(date));
		milMo_SimpleLabel2.SetScale(scale.x, 20f);
		milMo_SimpleLabel2.SetPosition(0f, 25f);
		milMo_SimpleLabel2.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		milMo_SimpleLabel2.SetWordWrap(w: false);
		milMo_SimpleLabel2.SetDefaultTextColor(1f, 1f, 1f, 0.5f);
		AddChild(milMo_SimpleLabel2);
		MilMo_SimpleLabel milMo_SimpleLabel3 = new MilMo_SimpleLabel(UI);
		milMo_SimpleLabel3.SetText(MilMo_Localization.GetLocString(post));
		float textHeight = GetTextHeight(milMo_SimpleLabel3.Text.String, scale.x);
		milMo_SimpleLabel3.SetScale(scale.x, textHeight);
		milMo_SimpleLabel3.SetPosition(0f, 50f);
		milMo_SimpleLabel3.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		milMo_SimpleLabel3.SetWordWrap(w: true);
		AddChild(milMo_SimpleLabel3);
		ScaleNow(scale.x, milMo_SimpleLabel3.ScaleMover.Target.y / base.Res.y + 50f);
	}

	public override void Draw()
	{
		if (Enabled)
		{
			base.Draw();
		}
	}

	private float GetTextHeight(string msg, float width)
	{
		return m_Skin.label.CalcHeight(new GUIContent(msg), width) + 10f;
	}
}
