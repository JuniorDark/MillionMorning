using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Network;
using Code.Core.ResourceSystem;
using Code.World.GUI.Hub;
using Core;
using UnityEngine;

namespace Code.World.GUI.PVP;

public class MilMoPvpLeaveQueueDialog : MilMo_SimpleBox
{
	public delegate void LeaveCallback(bool left);

	private readonly MilMo_Button m_LeaveButton;

	private readonly MilMo_Button m_CancelButton;

	private readonly MilMo_Widget m_Title;

	private readonly MilMo_Widget m_TextBox;

	public MilMoPvpLeaveQueueDialog(MilMo_UserInterface ui, LeaveCallback callback)
		: base(ui)
	{
		MilMoPvpLeaveQueueDialog milMoPvpLeaveQueueDialog = this;
		SetFont(MilMo_GUI.Font.EborgLarge);
		SetTextDropShadowPos(2f, 2f);
		TextOutline = new Vector2(1f, 1f);
		TextOutlineColor = new Color(0f, 0f, 0f, 0.3f);
		SetScale(400f, 200f);
		SetEnabled(e: false);
		SetFadeSpeed(0.2f);
		SetAlpha(1f);
		SetColor(Color.black);
		SetSkin(2);
		m_Title = new MilMo_Widget(UI);
		m_Title.SetWordWrap(w: true);
		m_Title.SetScale(Scale.x - 20f, 30f);
		m_Title.SetFont(MilMo_GUI.Font.GothamLarge);
		m_Title.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Title.SetPosition(15f, 5f);
		m_Title.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		AddChild(m_Title);
		m_TextBox = new MilMo_Widget(UI);
		m_TextBox.SetWordWrap(w: true);
		m_TextBox.SetScale(Scale.x - 20f, Scale.y - 100f);
		m_TextBox.SetFont(MilMo_GUI.Font.GothamMedium);
		m_TextBox.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_TextBox.SetPosition(15f, 15f);
		m_TextBox.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		AddChild(m_TextBox);
		m_LeaveButton = new MilMo_Button(UI);
		m_LeaveButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		m_LeaveButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		m_LeaveButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		m_LeaveButton.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_LeaveButton.SetScale(70f, 30f);
		m_LeaveButton.SetPosition(Scale.x - 90f, Scale.y - 50f);
		m_LeaveButton.SetText(MilMo_Localization.GetLocString("PVP_9390"));
		m_LeaveButton.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		m_LeaveButton.Function = delegate
		{
			Singleton<GameNetwork>.Instance.SendLeavePvPQueue();
			callback(left: true);
			milMoPvpLeaveQueueDialog.Close(null);
		};
		AddChild(m_LeaveButton);
		m_CancelButton = new MilMo_Button(UI);
		m_CancelButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		m_CancelButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		m_CancelButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		m_CancelButton.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_CancelButton.SetScale(70f, 30f);
		m_CancelButton.SetPosition(Scale.x - 170f, Scale.y - 50f);
		m_CancelButton.SetText(MilMo_Localization.GetLocString("Generic_Cancel"));
		m_CancelButton.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		m_CancelButton.Function = delegate
		{
			milMoPvpLeaveQueueDialog.Close(null);
		};
		AddChild(m_CancelButton);
	}

	public void Open(MilMo_LocString matchModeName, int queueLeft)
	{
		MilMo_LocString locString = MilMo_Localization.GetLocString("PVP_9386");
		locString.SetFormatArgs(matchModeName);
		m_Title.SetText(locString);
		MilMo_LocString locString2 = MilMo_Localization.GetLocString("PVP_9387");
		locString2.SetFormatArgs(matchModeName, queueLeft);
		m_TextBox.SetText(locString2);
		Refresh();
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
