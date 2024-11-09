using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Network.messages.server.PVP;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.PVP;

public class jb_QueueButton : MilMo_Widget
{
	public delegate void SelectMatchModeCallback(QueueInfo queueInfo);

	private readonly MilMo_Button m_Button;

	private readonly MilMo_Widget m_QueueCount;

	private readonly SelectMatchModeCallback m_Callback;

	public jb_QueueButton(MilMo_UserInterface ui, int index, QueueInfo queueInfo, SelectMatchModeCallback callback)
		: base(ui)
	{
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetPosition(5f, index * 60 + 10);
		SetScale(180f, 60f);
		m_Button = new MilMo_Button(UI);
		m_Button.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		m_Button.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		m_Button.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		m_Button.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Button.SetScale(170f, 50f);
		m_Button.SetPosition(0f, 0f);
		m_Button.SetText(MilMo_Localization.GetLocString(queueInfo.MatchMode.Title()));
		m_Button.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		m_Button.SetTextOffset(15f, 0f);
		m_Button.Function = delegate
		{
			callback(queueInfo);
		};
		m_Callback = callback;
		m_QueueCount = new MilMo_Widget(UI);
		m_QueueCount.SetFont(MilMo_GUI.Font.EborgSmall);
		m_QueueCount.SetFontScale(0.65f, 0.65f);
		m_QueueCount.SetScale(30f, 50f);
		m_QueueCount.SetAlignment(MilMo_GUI.Align.TopLeft);
		float x = m_Button.Pos.x + m_Button.Scale.x - m_QueueCount.Scale.x - 12f;
		m_QueueCount.SetPosition(x, -3f);
		m_QueueCount.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		SetQueueCount(queueInfo);
		AddChild(m_QueueCount);
		AddChild(m_Button);
	}

	public void UpdateButton(QueueInfo queueInfo)
	{
		m_Button.Function = delegate
		{
			m_Callback(queueInfo);
		};
		SetQueueCount(queueInfo);
	}

	public void SetButtonColor(Color col)
	{
		m_Button.SetDefaultColor(col);
	}

	private void SetQueueCount(QueueInfo queueInfo)
	{
		m_QueueCount.SetText(MilMo_Localization.GetNotLocalizedLocString("(" + queueInfo.QueueSize + "/" + queueInfo.MaxQueueSize + ")"));
	}
}
