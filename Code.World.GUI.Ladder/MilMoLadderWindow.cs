using System;
using System.Collections.Generic;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Network;
using Code.Core.Network.messages.client;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using Code.World.GUI.Hub;
using Code.World.Player;
using Code.World.Voting;
using Core;
using UnityEngine;

namespace Code.World.GUI.Ladder;

public class MilMoLadderWindow : MilMo_SimpleBox
{
	private readonly List<float> m_Widths;

	private readonly List<MilMo_Widget> m_Columns;

	private readonly MilMo_Widget m_RankText;

	private MilMo_Widget m_DarkBack;

	private readonly MilMo_Widget m_DarkBack2;

	private readonly MilMo_Widget m_StreamingArrow;

	private MilMo_ToggleBar m_PageToggle;

	private readonly MilMo_ScrollView m_Scroller;

	private readonly MilMo_Button m_BackOneButton;

	private readonly MilMo_Button m_BackTenButton;

	private readonly MilMo_Button m_ForwardOneButton;

	private readonly MilMo_Button m_ForwardTenButton;

	private readonly MilMo_Button m_TopTenButton;

	private readonly MilMo_Button m_MeButton;

	protected MilMo_Button m_ExitButton;

	private IList<LadderEntry> m_Entries;

	private int m_CurrentPage;

	private readonly int m_EntriesPerPage;

	private readonly MilMo_VoteManager.VoteTypes m_Type;

	private MilMo_LadderField.ButtonCallback m_ButtonCallback;

	private string m_ButtonTexture = "";

	private MilMo_LocString m_ButtonTooltip = MilMo_LocString.Empty;

	public void SetButton(MilMo_LadderField.ButtonCallback callback, string texture, MilMo_LocString tooltip)
	{
		m_ButtonCallback = callback;
		m_ButtonTexture = texture;
		m_ButtonTooltip = tooltip;
	}

	public MilMoLadderWindow(MilMo_UserInterface ui, MilMo_LocString title, int entriesPerPage, List<string> columns, List<float> columnSizes, MilMo_VoteManager.VoteTypes type, Vector2 size)
		: base(ui)
	{
		SetText(title);
		SetFont(MilMo_GUI.Font.EborgLarge);
		SetTextOffset(0f, -35f);
		SetTextDropShadowPos(2f, 2f);
		SetAlpha(1f);
		TextOutline = new Vector2(1f, 1f);
		TextOutlineColor = new Color(0f, 0f, 0f, 0.3f);
		SetScale(size);
		SetScalePull(0.08f, 0.08f);
		SetScaleDrag(0.5f, 0.5f);
		SetEnabled(e: false);
		SetPosPull(0.08f, 0.08f);
		SetPosDrag(0.7f, 0.7f);
		SetFadeSpeed(0.2f);
		AllowPointerFocus = true;
		SetText(MilMo_Localization.GetLocString("Homes_13300"));
		m_Columns = new List<MilMo_Widget>(columns.Count);
		m_Widths = columnSizes;
		m_Entries = new List<LadderEntry>(entriesPerPage);
		m_Type = type;
		m_EntriesPerPage = entriesPerPage;
		m_Scroller = new MilMo_ScrollView(UI);
		m_Scroller.MShowHorizBar = false;
		m_Scroller.MShowVertBar = false;
		m_Scroller.HasBackground(b: false);
		m_Scroller.SetTextureBlack();
		m_Scroller.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Scroller.SetPosition(0f, 50f);
		m_Scroller.SetScale(size.x - 15f, size.y - 50f);
		m_Scroller.SetViewSize(size.x - 15f, size.y - 50f);
		AllowPointerFocus = false;
		AddChild(m_Scroller);
		float num = 10f;
		m_RankText = new MilMo_Widget(UI);
		m_RankText.SetFont(MilMo_GUI.Font.EborgMedium);
		m_RankText.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		m_RankText.SetScale(50f, 35f);
		m_RankText.SetAlignment(MilMo_GUI.Align.CenterLeft);
		m_RankText.SetPosition(num, 20f);
		m_RankText.SetText(MilMo_Localization.GetNotLocalizedLocString("#"));
		m_RankText.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		AddChild(m_RankText);
		num += 50f;
		for (int i = 0; i < columns.Count; i++)
		{
			MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
			milMo_Widget.SetFont(MilMo_GUI.Font.EborgMedium);
			milMo_Widget.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
			milMo_Widget.SetScale(m_Widths[i] * 12f, 35f);
			milMo_Widget.SetAlignment(MilMo_GUI.Align.CenterLeft);
			milMo_Widget.SetPosition(num, 20f);
			milMo_Widget.SetText(MilMo_Localization.GetLocString(columns[i]));
			milMo_Widget.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
			milMo_Widget.AllowPointerFocus = false;
			m_Columns.Add(milMo_Widget);
			AddChild(milMo_Widget);
			num += m_Widths[i] * 12f;
		}
		MilMo_Widget milMo_Widget2 = new MilMo_Widget(UI);
		milMo_Widget2.SetFont(MilMo_GUI.Font.EborgMedium);
		milMo_Widget2.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		milMo_Widget2.SetScale(110f, 35f);
		milMo_Widget2.SetAlignment(MilMo_GUI.Align.CenterLeft);
		milMo_Widget2.SetPosition(num, 20f);
		milMo_Widget2.SetText(MilMo_Localization.GetLocString("Homes_13299"));
		milMo_Widget2.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		m_Columns.Add(milMo_Widget2);
		AddChild(milMo_Widget2);
		m_ExitButton = new MilMo_Button(UI);
		m_ExitButton.SetAllTextures("Batch01/Textures/World/CloseButton");
		m_ExitButton.SetHoverTexture("Batch01/Textures/World/CloseButtonMO");
		m_ExitButton.SetPosition(size.x - 16f, 4f);
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
		Vector2 position = new Vector2((Scale.x - 240f) / 2f, Scale.y - 50f);
		m_DarkBack2 = new MilMo_Widget(UI);
		m_DarkBack2.SetPosition(position);
		m_DarkBack2.SetScale(270f, 31f);
		m_DarkBack2.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_DarkBack2.SetTextureBlackTransparent();
		AddChild(m_DarkBack2);
		m_BackTenButton = new MilMo_Button(UI);
		m_BackTenButton.SetPosition(position);
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
			RequestPage(-10);
		};
		AddChild(m_BackTenButton);
		position.x += 30f;
		m_BackOneButton = new MilMo_Button(UI);
		m_BackOneButton.SetPosition(position);
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
			RequestPage(-1);
		};
		AddChild(m_BackOneButton);
		position.x += 30f;
		m_TopTenButton = new MilMo_Button(UI);
		m_TopTenButton.SetPosition(position);
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
			RequestPage(-m_CurrentPage);
		};
		AddChild(m_TopTenButton);
		position.x += 74f;
		m_MeButton = new MilMo_Button(UI);
		m_MeButton.SetPosition(position);
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
			RequestPageContainingSelf();
		};
		AddChild(m_MeButton);
		position.x += 78f;
		m_ForwardOneButton = new MilMo_Button(UI);
		m_ForwardOneButton.SetPosition(position);
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
			RequestPage(1);
		};
		AddChild(m_ForwardOneButton);
		position.x += 30f;
		m_ForwardTenButton = new MilMo_Button(UI);
		m_ForwardTenButton.SetPosition(position);
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
			RequestPage(10);
		};
		AddChild(m_ForwardTenButton);
		Close(null);
	}

	public void Refresh()
	{
		UI.ResetLayout(10f);
		UI.SetNext(10f, 0f);
		UpdateLadder();
	}

	private void RequestPage(int jump)
	{
		ShowStreamingArrow();
		Singleton<GameNetwork>.Instance.SendToGameServer(new ClientRequestLadderPage(m_EntriesPerPage, (sbyte)m_Type, Math.Max(m_CurrentPage + jump, 0)));
	}

	private void RequestPageContainingSelf()
	{
		ShowStreamingArrow();
		Singleton<GameNetwork>.Instance.SendToGameServer(new ClientRequestLadderPageContainingSelf((sbyte)m_Type, (sbyte)m_EntriesPerPage));
	}

	public void PageReceived(ServerLadderPage message)
	{
		HideStreamingArrow();
		if (message != null)
		{
			if (message.getEntries().Count > 0)
			{
				m_Entries = message.getEntries();
			}
			m_CurrentPage = message.getPageNumber();
			UpdateLadder();
		}
	}

	private void UpdateLadder()
	{
		m_Scroller.RemoveAllChildren();
		int num = m_CurrentPage * m_EntriesPerPage + 1;
		for (int i = 0; i < m_Entries.Count; i++)
		{
			if (m_Entries[i] is HomeLadderEntry)
			{
				MilMo_LadderField milMo_LadderField = new MilMo_LadderField(UI, num, m_Entries[i].GetIdentifier(), m_ButtonCallback, m_ButtonTexture, m_ButtonTooltip);
				milMo_LadderField.SetAlignment(MilMo_GUI.Align.TopLeft);
				HomeLadderEntry homeLadderEntry = (HomeLadderEntry)m_Entries[i];
				List<string> list = new List<string>(2);
				list.Add(homeLadderEntry.GetHomeName());
				list.Add(homeLadderEntry.GetPlayerName());
				milMo_LadderField.Init(m_Widths, list, homeLadderEntry.GetScore());
				milMo_LadderField.SetPosition(0f, (i + 1) * 25);
				if (m_Entries[i].GetIdentifier().ToString().Equals(MilMo_Player.Instance.Avatar.Id))
				{
					milMo_LadderField.SetIsLocalPlayer();
				}
				m_Scroller.AddChild(milMo_LadderField);
				num++;
			}
		}
		m_Entries.Clear();
		m_Scroller.RefreshViewSize();
	}

	private void RefreshUI()
	{
		SetPosition((float)(Screen.width / 2) - UI.GlobalInputOffset.x + 50f, (float)(Screen.height / 2) - UI.GlobalInputOffset.y);
	}

	public override void Draw()
	{
		if (Enabled)
		{
			if (UI.ScreenSizeDirty)
			{
				RefreshUI();
			}
			SetXPos((float)(Screen.width / 2) - UI.GlobalInputOffset.x);
			GUISkin skin = UnityEngine.GUI.skin;
			UnityEngine.GUI.skin = Skin;
			new Color(1f, 1f, 1f, 1f);
			UnityEngine.GUI.color = CurrentColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
			Rect screenPosition = m_Scroller.GetScreenPosition();
			screenPosition.x += 15f;
			screenPosition.width -= 15f;
			screenPosition.height -= 65f;
			UnityEngine.GUI.Box(screenPosition, "");
			UnityEngine.GUI.skin = skin;
			base.Draw();
		}
	}

	public void Open()
	{
		RequestPage(0);
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

	public void ShowStreamingArrow()
	{
		m_StreamingArrow.AlphaTo(1f);
	}

	public void HideStreamingArrow()
	{
		m_StreamingArrow.AlphaTo(0f);
	}
}
