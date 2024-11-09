using System.Collections.Generic;
using System.Linq;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Network;
using Code.Core.Network.messages.server.PVP;
using Code.Core.ResourceSystem;
using Code.World.GUI.Hub;
using Core;
using UnityEngine;

namespace Code.World.GUI.PVP;

public class MilMoPvpJoinQueueWindow : MilMo_SimpleBox
{
	private MilMo_Button m_JoinButton;

	private MilMo_Button m_ExitButton;

	private jb_PVPModeInfoWidget m_InfoWidget;

	private MilMo_Widget m_ExtraInfo;

	private MilMo_MatchMode m_matchMode;

	private readonly Dictionary<MilMo_MatchMode, jb_QueueButton> m_QueueButtons;

	private int m_NextUpdate;

	public MilMoPvpJoinQueueWindow(MilMo_UserInterface ui)
		: base(ui)
	{
		m_QueueButtons = new Dictionary<MilMo_MatchMode, jb_QueueButton>();
		SetupBox();
		CreateExitButton();
		Vector2 infoWidgetScale = new Vector2(400f, 350f);
		CreateInfoWidget(infoWidgetScale);
		CreateDividers();
		CreateInfoText(infoWidgetScale);
		CreateJoinButton();
		CreateExtraInfo();
		BringToFront(m_ExitButton);
	}

	private void SetupBox()
	{
		SetFont(MilMo_GUI.Font.EborgLarge);
		SetTextOffset(0f, -35f);
		SetTextDropShadowPos(2f, 2f);
		TextOutline = new Vector2(1f, 1f);
		TextOutlineColor = new Color(0f, 0f, 0f, 0.3f);
		SetScale(640f, 400f);
		SetEnabled(e: false);
		SetFadeSpeed(0.2f);
		SetAlpha(1f);
		SetColor(Color.black);
		SetDefaultColor(Color.black);
		SetSkin(2);
	}

	private void CreateInfoText(Vector2 infoWidgetScale)
	{
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetFont(MilMo_GUI.Font.GothamSmall);
		milMo_Widget.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		milMo_Widget.SetScale(infoWidgetScale.x - 40f, 60f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.SetPosition(Scale.x - 410f, Scale.y - 40f);
		milMo_Widget.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.SetWordWrap(w: true);
		milMo_Widget.SetText(MilMo_Localization.GetLocString("PVP_9388"));
		AddChild(milMo_Widget);
	}

	private void CreateExtraInfo()
	{
		m_ExtraInfo = new MilMo_Widget(UI);
		m_ExtraInfo.SetFont(MilMo_GUI.Font.GothamSmall);
		m_ExtraInfo.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_ExtraInfo.SetScale(100f, 60f);
		m_ExtraInfo.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_ExtraInfo.SetPosition(Scale.x / 2f + 20f, m_JoinButton.Pos.y - 40f);
		m_ExtraInfo.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		m_ExtraInfo.SetWordWrap(w: true);
		m_ExtraInfo.SetEnabled(e: false);
		AddChild(m_ExtraInfo);
	}

	private void CreateInfoWidget(Vector2 infoWidgetScale)
	{
		m_InfoWidget = new jb_PVPModeInfoWidget(UI, infoWidgetScale);
		m_InfoWidget.Enabled = true;
		m_InfoWidget.SetScale(infoWidgetScale);
		m_InfoWidget.SetPosition(Scale.x - 450f, 1f);
		AddChild(m_InfoWidget);
	}

	private void CreateDividers()
	{
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetTexture("Batch01/Textures/Homes/StorageDivider");
		milMo_Widget.Enabled = true;
		milMo_Widget.SetAngle(90f);
		milMo_Widget.SetScale(Scale.y, 2f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.SetPosition(Scale.x - 455f, 0f);
		AddChild(milMo_Widget);
		MilMo_Widget milMo_Widget2 = new MilMo_Widget(UI);
		milMo_Widget2.SetTexture("Batch01/Textures/Homes/StorageDivider");
		milMo_Widget2.Enabled = true;
		milMo_Widget2.SetScale(350f, 2f);
		milMo_Widget2.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget2.SetPosition(Scale.x - 425f, Scale.y - 50f);
		milMo_Widget2.SetAlpha(0.5f);
		AddChild(milMo_Widget2);
	}

	private void CreateJoinButton()
	{
		m_JoinButton = new MilMo_Button(UI);
		m_JoinButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		m_JoinButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		m_JoinButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		m_JoinButton.SetAlignment(MilMo_GUI.Align.TopCenter);
		m_JoinButton.SetScale(150f, 40f);
		m_JoinButton.SetPosition(Scale.x - 250f, Scale.y - 100f);
		m_JoinButton.SetText(MilMo_Localization.GetLocString("PVP_9395"));
		m_JoinButton.SetFont(MilMo_GUI.Font.EborgSmall);
		m_JoinButton.SetFontPreset(MilMo_GUI.FontPreset.Outline);
		m_JoinButton.Function = delegate
		{
			Singleton<GameNetwork>.Instance.RequestJoinPvPQueue((int)m_matchMode);
			Close(null);
		};
		AddChild(m_JoinButton);
	}

	private void CreateExitButton()
	{
		m_ExitButton = new MilMo_Button(UI);
		m_ExitButton.SetAllTextures("Batch01/Textures/World/CloseButton");
		m_ExitButton.SetHoverTexture("Batch01/Textures/World/CloseButtonMO");
		m_ExitButton.SetPosition(Scale.x - 10f, 5f);
		m_ExitButton.SetScale(32f, 32f);
		m_ExitButton.SetAlignment(MilMo_GUI.Align.TopRight);
		m_ExitButton.Function = delegate
		{
			Close(null);
		};
		AddChild(m_ExitButton);
	}

	public void Open(ICollection<QueueInfo> queueInfoList)
	{
		if (queueInfoList.Count != 0)
		{
			if (m_QueueButtons.Count == 0)
			{
				CreateQueueList(queueInfoList);
			}
			else
			{
				UpdateQueueList(queueInfoList);
			}
			if (!IsEnabled())
			{
				SetEnabled(e: true);
				SetPosition((float)(Screen.width / 2) - UI.GlobalInputOffset.x + 50f, (float)(Screen.height / 2) - UI.GlobalInputOffset.y);
				SetAlpha(0f);
				AlphaTo(1f);
				SetUpdateTime();
			}
		}
	}

	private void UpdateQueueList(IEnumerable<QueueInfo> queueInfoList)
	{
		foreach (QueueInfo queueInfo in queueInfoList)
		{
			m_QueueButtons[queueInfo.MatchMode].UpdateButton(queueInfo);
			if (queueInfo.MatchMode.Equals(m_matchMode))
			{
				SelectMatchMode(queueInfo);
			}
		}
	}

	private void CreateQueueList(ICollection<QueueInfo> queueInfoList)
	{
		Refresh();
		RemoveQueueButtons();
		CreateQueueButtons(queueInfoList);
		QueueInfo queueInfo = queueInfoList.FirstOrDefault();
		SelectMatchMode(queueInfo);
	}

	private void SetUpdateTime()
	{
		m_NextUpdate = Mathf.FloorToInt(Time.time) + 5;
	}

	public override void Step()
	{
		base.Step();
		if (IsEnabled() && Time.time >= (float)m_NextUpdate)
		{
			SetUpdateTime();
			Singleton<GameNetwork>.Instance.RequestPvPQueues();
		}
	}

	private void RemoveQueueButtons()
	{
		foreach (jb_QueueButton value in m_QueueButtons.Values)
		{
			RemoveChild(value);
		}
		m_QueueButtons.Clear();
	}

	private void Close(object obj)
	{
		if (MilMo_Hub.Instance.enabled)
		{
			MilMo_Hub.ClickMade();
		}
		UI.ResetLayout(10f, 10f);
		GoTo((float)(Screen.width / 2) - UI.GlobalInputOffset.x + 50f, -500f);
		SetEnabled(e: false);
	}

	private void Refresh()
	{
		UI.ResetLayout(10f);
		UI.SetNext(10f, 0f);
	}

	private void CreateQueueButtons(IEnumerable<QueueInfo> queueInfoList)
	{
		int num = 0;
		foreach (QueueInfo queueInfo in queueInfoList)
		{
			CreateQueueButton(num++, queueInfo);
		}
	}

	private void CreateQueueButton(int index, QueueInfo queueInfo)
	{
		jb_QueueButton jb_QueueButton2 = new jb_QueueButton(UI, index, queueInfo, SelectMatchMode);
		AddChild(jb_QueueButton2);
		m_QueueButtons.Add(queueInfo.MatchMode, jb_QueueButton2);
	}

	private void SelectMatchMode(QueueInfo queueInfo)
	{
		m_matchMode = queueInfo.MatchMode;
		SetQueueButtonMarked(m_matchMode);
		m_InfoWidget.SetDescription(m_matchMode, queueInfo.ScoreGoal);
		if (queueInfo.CanJoinNow)
		{
			m_JoinButton.SetText(MilMo_Localization.GetLocString("PVP_9393"));
			m_ExtraInfo.SetText(MilMo_Localization.GetLocString("PVP_9394"));
			m_ExtraInfo.SetEnabled(e: true);
		}
		else
		{
			m_JoinButton.SetText(MilMo_Localization.GetLocString("PVP_9395"));
			m_ExtraInfo.SetEnabled(e: false);
		}
	}

	private void SetQueueButtonMarked(MilMo_MatchMode matchMode)
	{
		foreach (jb_QueueButton value in m_QueueButtons.Values)
		{
			value.SetButtonColor(Color.white);
		}
		m_QueueButtons[m_matchMode].SetButtonColor(Color.black);
	}
}
