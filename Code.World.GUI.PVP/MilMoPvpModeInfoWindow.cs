using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.World.GUI.Hub;
using UnityEngine;

namespace Code.World.GUI.PVP;

public class MilMoPvpModeInfoWindow : MilMo_SimpleBox
{
	private readonly jb_PVPModeInfoWidget m_modeInfoWidget;

	private readonly MilMo_Button m_ExitButton;

	private bool m_IsActive;

	public MilMoPvpModeInfoWindow(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "PVPModeInfoWindow";
		UI.ResetLayout(10f, 10f);
		SetFont(MilMo_GUI.Font.EborgLarge);
		SetTextOffset(0f, -35f);
		SetTextDropShadowPos(2f, 2f);
		TextOutline = new Vector2(1f, 1f);
		TextOutlineColor = new Color(0f, 0f, 0f, 0.3f);
		SetScale(500f, 350f);
		SetScalePull(0.08f, 0.08f);
		SetScaleDrag(0.5f, 0.5f);
		SetEnabled(e: false);
		SetPosPull(0.08f, 0.08f);
		SetPosDrag(0.7f, 0.7f);
		SetFadeSpeed(0.2f);
		SetSkin(2);
		m_modeInfoWidget = new jb_PVPModeInfoWidget(UI, Scale);
		m_modeInfoWidget.Enabled = true;
		m_modeInfoWidget.SetScale(400f, 200f);
		m_modeInfoWidget.SetPosition(1f, 1f);
		AddChild(m_modeInfoWidget);
		m_ExitButton = new MilMo_Button(UI);
		m_ExitButton.SetAllTextures("Batch01/Textures/World/CloseButton");
		m_ExitButton.SetHoverTexture("Batch01/Textures/World/CloseButtonMO");
		m_ExitButton.SetPosition(Scale.x - 10f, 10f);
		m_ExitButton.SetScale(32f, 32f);
		m_ExitButton.SetAlignment(MilMo_GUI.Align.TopRight);
		m_ExitButton.SetDefaultColor(1f, 1f, 1f, 1f);
		m_ExitButton.SetDefaultColor(1f, 1f, 1f, 1f);
		m_ExitButton.SetFont(MilMo_GUI.Font.EborgSmall);
		m_ExitButton.SetTextOutline(4f, 4f);
		m_ExitButton.TextOutlineColor = new Color(0f, 0f, 0f, 1f);
		m_ExitButton.SetTextOffset(-5f, 24f);
		m_ExitButton.SetDefaultTextColor(1f, 1f, 1f, 0.5f);
		m_ExitButton.SetHoverTextColor(1f, 1f, 1f, 0.8f);
		m_ExitButton.SetFontScale(0.8f);
		m_ExitButton.FadeToDefaultColor = false;
		m_ExitButton.SetFadeOutSpeed(0.08f);
		m_ExitButton.Function = delegate
		{
			Close(null);
		};
		AddChild(m_ExitButton);
		Close(null);
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

	public void DummyOpen()
	{
		m_modeInfoWidget.SetDescription(MilMo_MatchMode.CAPTURE_THE_FLAG, 3);
		Open();
	}

	public void SetDescription(MilMo_MatchMode matchMode, int scoreGoal)
	{
		m_modeInfoWidget.SetDescription(matchMode, scoreGoal);
	}

	public void Open()
	{
		Refresh();
		m_IsActive = true;
		SetEnabled(e: true);
		SetPosition((float)(Screen.width / 2) - UI.GlobalInputOffset.x + 50f, (float)(Screen.height / 2) - UI.GlobalInputOffset.y);
		SetAlpha(0f);
		AlphaTo(1f);
	}

	public void Close(object obj)
	{
		if (MilMo_Hub.Instance.enabled)
		{
			MilMo_Hub.ClickMade();
		}
		m_IsActive = false;
		UI.ResetLayout(10f, 10f);
		GoTo((float)(Screen.width / 2) - UI.GlobalInputOffset.x + 50f, -500f);
		SetEnabled(e: false);
	}

	public void Refresh()
	{
		UI.ResetLayout(10f);
		UI.SetNext(10f, 0f);
	}
}
