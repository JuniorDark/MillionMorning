using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Network;
using Code.Core.Portal.Steam;
using Code.Core.ResourceSystem;
using Core;
using Steamworks;
using UnityEngine;

namespace Code.World.GUI.Steam;

public sealed class SteamAvatarField : MilMo_Widget
{
	private readonly Vector2 _mFieldScale = new Vector2(300f, 32f);

	private readonly CSteamID _steamFriendId;

	public SteamAvatarField(MilMo_UserInterface ui, CSteamID steamFriendId)
		: base(ui)
	{
		_steamFriendId = steamFriendId;
		string friendPersonaName = SteamFriends.GetFriendPersonaName(steamFriendId);
		Texture2D smallAvatar = MilMo_SteamPortal.GetSmallAvatar(steamFriendId);
		Identifier = "SteamAvatarField " + MilMo_UserInterface.GetRandomID();
		GoToNow(0f, UI.Next.y);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetDefaultColor(0.6f, 0.6f, 0.6f, 1f);
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.GoToNow(10f, 0f);
		milMo_Widget.ScaleNow(32f, _mFieldScale.y);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.SetTexture(smallAvatar);
		AddChild(milMo_Widget);
		MilMo_Widget milMo_Widget2 = new MilMo_Widget(UI);
		milMo_Widget2.GoToNow(52f, 0f);
		milMo_Widget2.ScaleNow(170f, _mFieldScale.y);
		milMo_Widget2.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget2.SetText((friendPersonaName.Length >= 14) ? MilMo_Localization.GetNotLocalizedLocString(friendPersonaName.Substring(0, 13) + "...") : MilMo_Localization.GetNotLocalizedLocString(friendPersonaName));
		milMo_Widget2.SetFont(MilMo_GUI.Font.EborgMedium);
		milMo_Widget2.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		milMo_Widget2.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		AddChild(milMo_Widget2);
		MilMo_Button milMo_Button = new MilMo_Button(UI);
		milMo_Button.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		milMo_Button.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		milMo_Button.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		milMo_Button.GoToNow(227f, 0f);
		milMo_Button.ScaleNow(50f, _mFieldScale.y);
		milMo_Button.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Button.SetFont(MilMo_GUI.Font.EborgSmall);
		milMo_Button.SetFontScale(0.8f);
		milMo_Button.SetText(MilMo_Localization.GetLocString("World_370"));
		milMo_Button.UseParentAlpha = false;
		milMo_Button.FadeToDefaultColor = false;
		milMo_Button.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		milMo_Button.Function = InviteToGame;
		AddChild(milMo_Button);
		SetScale(_mFieldScale);
	}

	private void InviteToGame(object obj)
	{
		Debug.Log("Inviting user with id: " + _steamFriendId.ToString() + " to join game");
		SteamFriends.InviteUserToGame(_steamFriendId, "");
		Singleton<GameNetwork>.Instance.SendSteamInvite(_steamFriendId.ToString());
	}

	public override void Step()
	{
		if (Enabled)
		{
			base.Step();
		}
	}

	public override void Draw()
	{
		if (Enabled)
		{
			Color currentColor = CurrentColor;
			if (Parent != null && UseParentAlpha)
			{
				currentColor.a *= Parent.CurrentColor.a;
			}
			UnityEngine.GUI.color = currentColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
			UnityEngine.GUI.skin = Font;
			CheckPointerFocus();
			base.Draw();
		}
	}
}
