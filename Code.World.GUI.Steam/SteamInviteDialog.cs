using System.Collections.Generic;
using System.Linq;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow;
using Code.Core.ResourceSystem;
using Steamworks;
using UnityEngine;

namespace Code.World.GUI.Steam;

public sealed class SteamInviteDialog : MilMo_Window
{
	private MilMo_Button _mCloseButton;

	private readonly Vector2 _mWindowSize = new Vector2(330f, 300f);

	private readonly List<CSteamID> _steamFriends;

	public SteamInviteDialog(MilMo_UserInterface ui, List<CSteamID> steamFriends)
		: base(ui)
	{
		_steamFriends = steamFriends;
		base.FixedRes = true;
		Identifier = "Steam Invite";
		HasCloseButton = false;
		base.CloseButton.Function = delegate
		{
			Close(null);
		};
		SetScale(_mWindowSize);
		SpawnScale = _mWindowSize;
		TargetScale = _mWindowSize;
		ExitScale = _mWindowSize;
		SetText(MilMo_Localization.GetLocString("Steam_0004"));
		MCaption.SetFont(MilMo_GUI.Font.EborgSmall);
		MCaption.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		CreateDragBlocker();
		CreateBackdrop();
		CreateFriendList();
		CreateCloseButton();
	}

	private void CreateDragBlocker()
	{
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.Identifier = "dragBlocker";
		milMo_Widget.IsInvisible = true;
		milMo_Widget.SetPosition(0f, 36f);
		milMo_Widget.SetScale(_mWindowSize);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		AddChild(milMo_Widget);
	}

	private void CreateBackdrop()
	{
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.AllowPointerFocus = false;
		milMo_Widget.Identifier = "blackBack";
		milMo_Widget.SetTexture("Batch01/Textures/Core/BlackTransparent");
		milMo_Widget.SetDefaultColor(0f, 0f, 0f, 0.4f);
		milMo_Widget.SetPosition(20f, 39f);
		milMo_Widget.SetScale(_mWindowSize.x - 20f, _mWindowSize.y - 90f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		AddChild(milMo_Widget);
	}

	private void CreateFriendList()
	{
		MilMo_ScrollView milMo_ScrollView = new MilMo_ScrollView(UI);
		milMo_ScrollView.HasBackground(b: false);
		milMo_ScrollView.AllowPointerFocus = false;
		milMo_ScrollView.MShowHorizBar = false;
		milMo_ScrollView.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		milMo_ScrollView.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_ScrollView.SetTextOffset(0f, -30f);
		milMo_ScrollView.SetTextDropShadowPos(2f, 2f);
		milMo_ScrollView.SetPosition(20f, 40f);
		milMo_ScrollView.SetScale(300f, 200f);
		milMo_ScrollView.SetScalePull(0.08f, 0.08f);
		milMo_ScrollView.SetScaleDrag(0.5f, 0.5f);
		milMo_ScrollView.SetPosPull(0.08f, 0.08f);
		milMo_ScrollView.SetPosDrag(0.7f, 0.7f);
		AddChild(milMo_ScrollView);
		UI.SetNext(20f, 20f);
		foreach (SteamAvatarField item in _steamFriends.Select((CSteamID steamFriendId) => new SteamAvatarField(UI, steamFriendId)))
		{
			milMo_ScrollView.AddChild(item);
		}
		milMo_ScrollView.RefreshViewSize();
	}

	private void CreateCloseButton()
	{
		_mCloseButton = new MilMo_Button(UI);
		_mCloseButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		_mCloseButton.Function = Close;
		_mCloseButton.FadeToDefaultColor = false;
		_mCloseButton.UseParentAlpha = false;
		_mCloseButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		_mCloseButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		_mCloseButton.SetFont(MilMo_GUI.Font.EborgSmall);
		_mCloseButton.SetFontScale(0.8f);
		_mCloseButton.SetText(MilMo_Localization.GetLocString("Generic_Close"));
		_mCloseButton.SetAlignment(MilMo_GUI.Align.BottomRight);
		_mCloseButton.SetScale(90f, 30f);
		_mCloseButton.SetPosition(_mWindowSize.x - 15f, _mWindowSize.y - 15f);
		_mCloseButton.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		AddChild(_mCloseButton);
	}
}
