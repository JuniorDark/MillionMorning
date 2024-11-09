using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.Homes;

public class MilMo_HomeEnterText
{
	private readonly MilMo_Widget m_HomeName;

	private readonly MilMo_Widget m_RoomName;

	private MilMo_TimerEvent m_ShowTimer;

	private readonly MilMo_UserInterface ui;

	public MilMo_HomeEnterText()
	{
		ui = MilMo_GlobalUI.GetSystemUI;
		m_HomeName = new MilMo_Widget(ui);
		m_RoomName = new MilMo_Widget(ui);
		m_HomeName.SetFont(MilMo_GUI.Font.EborgXL);
		m_RoomName.SetFont(MilMo_GUI.Font.EborgLarge);
		m_HomeName.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_RoomName.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_HomeName.FadeToDefaultTextColor = false;
		m_RoomName.FadeToDefaultTextColor = false;
		m_HomeName.SetAlignment(MilMo_GUI.Align.CenterCenter);
		m_RoomName.SetAlignment(MilMo_GUI.Align.CenterCenter);
		m_HomeName.SetScale(600f, 70f);
		m_RoomName.SetScale(600f, 70f);
		m_HomeName.AllowPointerFocus = false;
		m_RoomName.AllowPointerFocus = false;
	}

	public void Show(string homeName, string roomName, bool firstTimeEntering)
	{
		if (m_ShowTimer != null)
		{
			Hide();
		}
		if (MilMo_BadWordFilter.GetStringIntegrity(homeName) != MilMo_BadWordFilter.StringIntegrity.OK)
		{
			homeName = "";
		}
		if (MilMo_BadWordFilter.GetStringIntegrity(roomName) != MilMo_BadWordFilter.StringIntegrity.OK)
		{
			roomName = "";
		}
		m_HomeName.SetTextColor(1f, 1f, 1f, 0f);
		m_RoomName.SetTextColor(1f, 1f, 1f, 0f);
		if (firstTimeEntering)
		{
			MilMo_GlobalUI.GetSystemUI.AddChild(m_HomeName);
		}
		MilMo_GlobalUI.GetSystemUI.AddChild(m_RoomName);
		m_HomeName.Enabled = true;
		m_RoomName.Enabled = true;
		m_HomeName.SetText(MilMo_Localization.GetNotLocalizedLocString(homeName));
		m_RoomName.SetText(MilMo_Localization.GetNotLocalizedLocString(roomName));
		RefreshUI();
		float delay = 2f;
		if (firstTimeEntering)
		{
			delay = 3.5f;
		}
		m_ShowTimer = MilMo_EventSystem.At(delay, delegate
		{
			m_HomeName.TextColorTo(1f, 1f, 1f, 1f);
			m_RoomName.TextColorTo(1f, 1f, 1f, 1f);
			m_ShowTimer = MilMo_EventSystem.At(3.5f, delegate
			{
				m_HomeName.TextColorTo(1f, 1f, 1f, 0f);
				m_RoomName.TextColorTo(1f, 1f, 1f, 0f);
				m_ShowTimer = MilMo_EventSystem.At(2f, delegate
				{
					Hide();
				});
			});
		});
	}

	public void RefreshUI()
	{
		m_HomeName.SetPosition((float)Screen.width * 0.5f, 150f);
		m_RoomName.SetPosition((float)Screen.width * 0.5f, 190f);
	}

	private void Hide()
	{
		m_HomeName.Enabled = false;
		m_RoomName.Enabled = false;
		MilMo_GlobalUI.GetSystemUI.RemoveChild(m_HomeName);
		MilMo_GlobalUI.GetSystemUI.RemoveChild(m_RoomName);
		MilMo_EventSystem.RemoveTimerEvent(m_ShowTimer);
		m_ShowTimer = null;
	}
}
