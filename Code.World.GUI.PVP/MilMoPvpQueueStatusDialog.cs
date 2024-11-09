using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.ResourceSystem;
using Code.World.GUI.Hub;
using UnityEngine;

namespace Code.World.GUI.PVP;

public class MilMoPvpQueueStatusDialog : MilMo_SimpleBox
{
	private readonly MilMo_Button m_LeaveButton;

	private readonly MilMo_Widget m_TextBox;

	private readonly MilMoPvpLeaveQueueDialog m_leaveQueueDialog;

	private MilMo_LocString m_MatchModeName;

	private int queueDiff;

	public MilMoPvpQueueStatusDialog(MilMo_UserInterface ui)
		: base(ui)
	{
		SetFont(MilMo_GUI.Font.EborgLarge);
		SetTextOffset(0f, -35f);
		SetTextDropShadowPos(2f, 2f);
		TextOutline = new Vector2(1f, 1f);
		TextOutlineColor = new Color(0f, 0f, 0f, 0.3f);
		SetScale(270f, 30f);
		SetEnabled(e: false);
		SetFadeSpeed(0.2f);
		SetAlpha(1f);
		SetColor(Color.black);
		m_TextBox = new MilMo_Widget(UI);
		m_TextBox.SetWordWrap(w: true);
		m_TextBox.SetScale(Scale.x - 20f, 20f);
		m_TextBox.SetFont(MilMo_GUI.Font.ArialRoundedMedium);
		m_TextBox.SetFontScale(0.6f, 0.6f);
		m_TextBox.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_TextBox.SetPosition(0f, 5f);
		AddChild(m_TextBox);
		m_leaveQueueDialog = new MilMoPvpLeaveQueueDialog(UI, delegate(bool left)
		{
			if (left)
			{
				Close(null);
			}
		});
		UI.AddChild(m_leaveQueueDialog);
		m_LeaveButton = new MilMo_Button(UI);
		m_LeaveButton.SetAllTextures("Batch01/Textures/World/CloseButton");
		m_LeaveButton.SetHoverTexture("Batch01/Textures/World/CloseButtonMO");
		m_LeaveButton.SetPosition(Scale.x - 5f, 3f);
		m_LeaveButton.SetScale(20f, 20f);
		m_LeaveButton.SetAlignment(MilMo_GUI.Align.TopRight);
		m_LeaveButton.SetFont(MilMo_GUI.Font.EborgSmall);
		m_LeaveButton.Function = delegate
		{
			m_leaveQueueDialog.Open(m_MatchModeName, queueDiff);
		};
		AddChild(m_LeaveButton);
	}

	public void Open(MilMo_LocString matchModeName, int queueSize, int maxSize)
	{
		Refresh();
		m_MatchModeName = matchModeName;
		queueDiff = maxSize - queueSize;
		MilMo_LocString locString = MilMo_Localization.GetLocString("PVP_9389");
		locString.SetFormatArgs(matchModeName, queueSize, maxSize);
		m_TextBox.SetText(locString);
		SetEnabled(e: true);
		SetPosition((float)(Screen.width / 2) - Scale.x, 0f + Scale.y / 2f);
		SetAlpha(0f);
		AlphaTo(1f);
	}

	public void Close(object obj)
	{
		if (MilMo_Hub.Instance.enabled)
		{
			MilMo_Hub.ClickMade();
		}
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
