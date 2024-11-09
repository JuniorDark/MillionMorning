using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.Network;
using Code.Core.Network.messages.client;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using Code.World.GUI.Hub;
using Code.World.Player;
using Core;
using UnityEngine;

namespace Code.World.GUI.PVP;

public class MilMoPvpLadderWindow : MilMo_SimpleBox
{
	private delegate void jb_PVPRankArrived(int rank);

	private readonly MilMo_SimpleBox m_BackBox;

	private readonly MilMo_ScrollView m_Scroller;

	private jb_PVPLadderField m_CurrentPlayerField;

	private readonly MilMo_Widget m_RankText;

	private readonly MilMo_Widget m_PlayerText;

	private readonly MilMo_Widget m_PointsText;

	private readonly MilMo_Widget m_DarkBack;

	private readonly MilMo_Widget m_DarkBack2;

	private readonly MilMo_Widget m_StreamingArrow;

	private MilMo_ToggleBar m_PageToggle;

	private readonly MilMo_Button m_BackOneButton;

	private readonly MilMo_Button m_BackTenButton;

	private readonly MilMo_Button m_ForwardOneButton;

	private readonly MilMo_Button m_ForwardTenButton;

	private readonly MilMo_Button m_TopTenButton;

	private readonly MilMo_Button m_MeButton;

	protected MilMo_Button m_ExitButton;

	private readonly MilMo_UserInterface ui;

	private bool m_IsActive;

	private IList<PvPLadderEntry> m_Rankings;

	private int m_StartRank = 1;

	private int? m_PlayerRank;

	private MilMo_GenericReaction ladderReceivedReaction;

	public MilMoPvpLadderWindow(MilMo_UserInterface ui)
		: base(ui)
	{
		this.ui = ui;
		Identifier = "PVPLadderWindow";
		UI.ResetLayout(10f, 10f);
		SetText(MilMo_Localization.GetLocString("PVP_9326"));
		SetFont(MilMo_GUI.Font.EborgLarge);
		SetTextOffset(0f, -35f);
		SetTextDropShadowPos(2f, 2f);
		TextOutline = new Vector2(1f, 1f);
		TextOutlineColor = new Color(0f, 0f, 0f, 0.3f);
		SetScale(370f, 390f);
		SetScalePull(0.08f, 0.08f);
		SetScaleDrag(0.5f, 0.5f);
		SetEnabled(e: false);
		SetPosPull(0.08f, 0.08f);
		SetPosDrag(0.7f, 0.7f);
		SetFadeSpeed(0.2f);
		m_BackBox = new MilMo_SimpleBox(UI);
		m_BackBox.SetPosition(10f, 10f);
		m_BackBox.SetScale(350f, 370f);
		m_BackBox.SetAlignment(MilMo_GUI.Align.TopLeft);
		AddChild(m_BackBox);
		float num = 10f;
		m_Scroller = new MilMo_ScrollView(UI);
		m_Scroller.MShowHorizBar = false;
		m_Scroller.MShowVertBar = false;
		m_Scroller.HasBackground(b: false);
		m_Scroller.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		m_Scroller.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Scroller.SetTextOffset(0f, -30f);
		m_Scroller.SetTextDropShadowPos(2f, 2f);
		m_Scroller.SetPosition(10f + num, 50f);
		m_Scroller.SetScale(370f, 338f);
		m_Scroller.SetScalePull(0.08f, 0.08f);
		m_Scroller.SetScaleDrag(0.5f, 0.5f);
		m_Scroller.SetPosPull(0.08f, 0.08f);
		m_Scroller.SetPosDrag(0.7f, 0.7f);
		AllowPointerFocus = false;
		AddChild(m_Scroller);
		m_DarkBack = new MilMo_Widget(UI);
		m_DarkBack.SetScale(350f, 30f);
		m_DarkBack.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_DarkBack.SetPosition(10f, 20f);
		m_DarkBack.SetTextureBlackTransparent();
		AddChild(m_DarkBack);
		m_RankText = new MilMo_Widget(UI);
		m_RankText.SetFont(MilMo_GUI.Font.EborgMedium);
		m_RankText.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_RankText.SetScale(200f, 50f);
		m_RankText.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_RankText.SetPosition(20f + num, 20f);
		m_RankText.SetText(MilMo_Localization.GetNotLocalizedLocString("#"));
		m_RankText.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		AddChild(m_RankText);
		m_PlayerText = new MilMo_Widget(UI);
		m_PlayerText.SetFont(MilMo_GUI.Font.EborgMedium);
		m_PlayerText.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_PlayerText.SetScale(200f, 50f);
		m_PlayerText.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_PlayerText.SetPosition(80f + num, 20f);
		m_PlayerText.SetText(MilMo_Localization.GetLocString("PVP_9327"));
		m_PlayerText.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		AddChild(m_PlayerText);
		m_PointsText = new MilMo_Widget(UI);
		m_PointsText.SetFont(MilMo_GUI.Font.EborgMedium);
		m_PointsText.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_PointsText.SetScale(40f, 40f);
		m_PointsText.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_PointsText.SetPosition(205f + num, 15f);
		m_PointsText.SetText(MilMo_Localization.GetLocString("PVP_9328"));
		m_PointsText.SetTextOffset(140f, 5f);
		m_PointsText.SetExtraDrawTextSize(200f, 0f);
		m_PointsText.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		AddChild(m_PointsText);
		m_ExitButton = new MilMo_Button(UI);
		m_ExitButton.SetAllTextures("Batch01/Textures/World/CloseButton");
		m_ExitButton.SetHoverTexture("Batch01/Textures/World/CloseButtonMO");
		m_ExitButton.SetPosition(363f, 16f);
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
		m_DarkBack2 = new MilMo_Widget(UI);
		m_DarkBack2.SetPosition(50f, 325f);
		m_DarkBack2.SetScale(270f, 31f);
		m_DarkBack2.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_DarkBack2.SetTextureBlackTransparent();
		AddChild(m_DarkBack2);
		m_StreamingArrow = new MilMo_Widget(UI);
		m_StreamingArrow.SetTexture("Batch01/Textures/World/StreamingArrow");
		m_StreamingArrow.SetScale(64f, 64f);
		m_StreamingArrow.SetPosition(185f, 140f);
		m_StreamingArrow.SetAlignment(MilMo_GUI.Align.CenterCenter);
		m_StreamingArrow.SetDefaultColor(1f, 1f, 1f, 1f);
		m_StreamingArrow.FixedRes = true;
		m_StreamingArrow.AllowPointerFocus = false;
		m_StreamingArrow.SetFadeSpeed(0.05f);
		m_StreamingArrow.FadeToDefaultColor = false;
		m_StreamingArrow.AngleMover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Linear);
		m_StreamingArrow.AngleMover.Vel.x = 6f;
		m_StreamingArrow.SetAlpha(0f);
		BringToFront(m_StreamingArrow);
		AddChild(m_StreamingArrow);
		m_BackTenButton = new MilMo_Button(UI);
		m_BackTenButton.SetPosition(50f, 325f);
		m_BackTenButton.SetScale(30f, 30f);
		m_BackTenButton.SetTextNoLocalization("<<");
		m_BackTenButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		m_BackTenButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		m_BackTenButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		m_BackTenButton.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_BackTenButton.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		m_BackTenButton.SetFont(MilMo_GUI.Font.EborgSmall);
		m_BackTenButton.Function = delegate
		{
			m_StartRank = Mathf.Max(m_StartRank - 100, 1);
			Refresh();
		};
		AddChild(m_BackTenButton);
		m_BackOneButton = new MilMo_Button(UI);
		m_BackOneButton.SetPosition(80f, 325f);
		m_BackOneButton.SetScale(30f, 30f);
		m_BackOneButton.SetTextNoLocalization("<");
		m_BackOneButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		m_BackOneButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		m_BackOneButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		m_BackOneButton.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_BackOneButton.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		m_BackOneButton.SetFont(MilMo_GUI.Font.EborgSmall);
		m_BackOneButton.Function = delegate
		{
			m_StartRank = Mathf.Max(m_StartRank - 10, 1);
			Refresh();
		};
		AddChild(m_BackOneButton);
		m_TopTenButton = new MilMo_Button(UI);
		m_TopTenButton.SetPosition(108f, 325f);
		m_TopTenButton.SetScale(80f, 30f);
		m_TopTenButton.SetText(MilMo_Localization.GetLocString("PVP_9329"));
		m_TopTenButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		m_TopTenButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		m_TopTenButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		m_TopTenButton.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_TopTenButton.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		m_TopTenButton.SetFont(MilMo_GUI.Font.EborgSmall);
		m_TopTenButton.Function = delegate
		{
			m_StartRank = 1;
			Refresh();
		};
		AddChild(m_TopTenButton);
		m_MeButton = new MilMo_Button(UI);
		m_MeButton.SetPosition(182f, 325f);
		m_MeButton.SetScale(80f, 30f);
		m_MeButton.SetText(MilMo_Localization.GetLocString("PVP_9333"));
		m_MeButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		m_MeButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		m_MeButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		m_MeButton.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_MeButton.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		m_MeButton.SetFont(MilMo_GUI.Font.EborgSmall);
		m_MeButton.Function = delegate
		{
			ShowPlayer();
		};
		AddChild(m_MeButton);
		m_ForwardOneButton = new MilMo_Button(UI);
		m_ForwardOneButton.SetPosition(260f, 325f);
		m_ForwardOneButton.SetScale(30f, 30f);
		m_ForwardOneButton.SetTextNoLocalization(">");
		m_ForwardOneButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		m_ForwardOneButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		m_ForwardOneButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		m_ForwardOneButton.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_ForwardOneButton.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		m_ForwardOneButton.SetFont(MilMo_GUI.Font.EborgSmall);
		m_ForwardOneButton.Function = delegate
		{
			m_StartRank += 10;
			Refresh();
		};
		AddChild(m_ForwardOneButton);
		m_ForwardTenButton = new MilMo_Button(UI);
		m_ForwardTenButton.SetPosition(290f, 325f);
		m_ForwardTenButton.SetScale(30f, 30f);
		m_ForwardTenButton.SetTextNoLocalization(">>");
		m_ForwardTenButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		m_ForwardTenButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		m_ForwardTenButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		m_ForwardTenButton.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_ForwardTenButton.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		m_ForwardTenButton.SetFont(MilMo_GUI.Font.EborgSmall);
		m_ForwardTenButton.Function = delegate
		{
			m_StartRank += 100;
			Refresh();
		};
		AddChild(m_ForwardTenButton);
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

	public override void Step()
	{
		base.Step();
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
		m_Rankings = new List<PvPLadderEntry>();
		for (int i = 0; i < 1000; i++)
		{
			PvPLadderEntry item = new PvPLadderEntry(MilMo_NameGenerator.GetName(), 0, i + 1, MilMo_Utility.RandomInt(1, 1000000));
			m_Rankings.Add(item);
		}
		Open();
	}

	public void Open()
	{
		Refresh();
		m_IsActive = true;
		SetEnabled(e: true);
		SetPosition((float)(Screen.width / 2) - UI.GlobalInputOffset.x + 50f, (float)(Screen.height / 2) - UI.GlobalInputOffset.y);
		SetAlpha(0f);
		AlphaTo(1f);
		ShowPlayer();
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

	public void SetCurrentPlayer(string playerName)
	{
		if (m_CurrentPlayerField != null)
		{
			m_CurrentPlayerField.DeSelect();
		}
		foreach (jb_PVPLadderField child in m_Scroller.Children)
		{
			if (child.Name.Text == MilMo_Localization.GetNotLocalizedLocString(playerName))
			{
				child.Select();
				m_CurrentPlayerField = child;
			}
		}
	}

	public void ShowPlayer()
	{
		GetPlayerRank(delegate(int rank)
		{
			if (rank == -1)
			{
				MilMo_Dialog dialog = new MilMo_Dialog(ui);
				ui.AddChild(dialog);
				dialog.BringToFront();
				dialog.DoOK("Batch01/Textures/HUD/IconLadder", MilMo_Localization.GetLocString("PVP_9335"), MilMo_Localization.GetLocString("PVP_9336"), delegate
				{
					dialog.CloseAndRemove(null);
				});
			}
			else
			{
				m_StartRank = rank - rank % 10;
				Refresh();
			}
		});
	}

	public void Refresh()
	{
		UI.ResetLayout(10f);
		UI.SetNext(10f, 0f);
		if (m_Rankings == null || m_Rankings.Count == 0 || m_Rankings[0].GetRank() != m_StartRank)
		{
			ShowStreamingArrow();
			if (ladderReceivedReaction == null)
			{
				ladderReceivedReaction = MilMo_EventSystem.Listen("pvp_ladder_received", LadderReceived);
			}
			Singleton<GameNetwork>.Instance.SendToGameServer(new ClientGetLadderFromPosition(m_StartRank, 10));
		}
		else
		{
			UpdateLadder();
		}
	}

	private void LadderReceived(object msgAsObj)
	{
		HideStreamingArrow();
		ladderReceivedReaction = null;
		if (msgAsObj is ServerPvPLadder serverPvPLadder)
		{
			m_Rankings = serverPvPLadder.getEntries();
			if (m_Rankings.Count > 0)
			{
				m_StartRank = m_Rankings[0].GetRank();
			}
			else
			{
				m_StartRank = 0;
			}
			UpdateLadder();
		}
	}

	private void UpdateLadder()
	{
		m_Scroller.RemoveAllChildren();
		foreach (PvPLadderEntry ranking in m_Rankings)
		{
			Vector2 padding = UI.Padding;
			UI.Padding = Vector2.zero;
			jb_PVPLadderField jb_PVPLadderField2 = new jb_PVPLadderField(UI, this);
			jb_PVPLadderField2.Fill(ranking.GetRank(), ranking.GetAvatarName(), ranking.GetScore());
			m_Scroller.AddChild(jb_PVPLadderField2);
			UI.Padding = padding;
		}
		m_Scroller.RefreshViewSize();
		SetCurrentPlayer(MilMo_Player.Instance.Avatar.Name);
	}

	private void GetPlayerRank(jb_PVPRankArrived callback)
	{
		if (m_PlayerRank.HasValue)
		{
			callback(m_PlayerRank.Value);
			return;
		}
		MilMo_EventSystem.Listen("pvp_rank_received", delegate(object msgAsObj)
		{
			if (msgAsObj is ServerLadderPositionFor serverLadderPositionFor && !(serverLadderPositionFor.getPlayerId() != MilMo_Player.Instance.Id))
			{
				if (!m_PlayerRank.HasValue)
				{
					m_PlayerRank = serverLadderPositionFor.getRank();
				}
				callback(m_PlayerRank.Value);
			}
		});
		Singleton<GameNetwork>.Instance.SendToGameServer(new ClientGetLadderPositionFor(MilMo_Player.Instance.Id));
	}

	public void ShowStreamingArrow()
	{
		m_StreamingArrow.AlphaTo(1f);
	}

	public void HideStreamingArrow()
	{
		m_StreamingArrow.AlphaTo(0f);
	}
}
