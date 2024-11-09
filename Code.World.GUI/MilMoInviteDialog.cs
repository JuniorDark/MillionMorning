using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow;
using Code.Core.Network;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.World.GUI.LoadingScreen;
using Code.World.Player;
using Core;
using Core.Input;
using Localization;
using UI.HUD.Dialogues;
using UI.Sprites;
using UnityEngine;

namespace Code.World.GUI;

public class MilMoInviteDialog : MilMo_Window
{
	private readonly MilMo_GenericReaction m_ToggleReaction;

	private readonly MilMo_Widget m_Caption;

	private readonly MilMo_Widget m_Headline;

	private readonly MilMo_Widget m_InfoText;

	private readonly MilMo_Widget m_Pic;

	private readonly MilMo_SimpleTextField m_TextField;

	private readonly MilMo_Button m_InviteButton;

	private readonly MilMo_Button m_CancelButton;

	public MilMoInviteDialog(MilMo_UserInterface ui)
		: base(ui)
	{
		m_ToggleReaction = MilMo_EventSystem.Listen("button_ToggleInviteDialog", delegate
		{
			if (!MilMo_Player.Instance.AnyShopState)
			{
				Toggle();
			}
		});
		m_ToggleReaction.Repeating = true;
		Identifier = "InviteDialog";
		UI = ui;
		HasCloseButton = true;
		SetText(MilMo_LocString.Empty);
		SpawnScale = new Vector2(400f, 0f);
		TargetScale = new Vector2(400f, 240f);
		ExitScale = new Vector2(400f, 0f);
		SpawnPos = new Vector2(UI.Center.x - TargetScale.x / 2f, UI.Center.y - TargetScale.y / 2f - 150f);
		TargetPos = SpawnPos;
		UI.ResetLayout(20f, 20f, this);
		m_Pic = new MilMo_Widget(UI);
		m_Pic.SetPosition(20f, UI.Align.Top + 20f);
		m_Pic.SetTexture("Batch01/Textures/FriendList/Invite");
		m_Pic.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Pic.SetScale(140f, 181f);
		m_Pic.FadeToDefaultColor = false;
		AddChild(m_Pic);
		m_Caption = new MilMo_Widget(UI);
		m_Caption.SetPosition(UI.Center.x, UI.Align.Top - UI.Padding.x / 3f);
		m_Caption.SetText(MilMo_Localization.GetLocString("World_378"));
		m_Caption.SetTexture("Batch01/Textures/Core/Invisible");
		m_Caption.SetAlignment(MilMo_GUI.Align.TopCenter);
		m_Caption.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		m_Caption.SetScale(235f, 35f);
		m_Caption.SetFont(MilMo_GUI.Font.EborgMedium);
		m_Caption.SetTextOutline(1f, 1f);
		m_Caption.TextOutlineColor = new Color(0f, 0f, 0f, 0.25f);
		m_Caption.SetTextDropShadowPos(2f, 2f);
		m_Caption.FadeToDefaultColor = false;
		AddChild(m_Caption);
		m_Headline = new MilMo_Widget(UI);
		m_Headline.SetPosition(175f, UI.Next.y - 18f);
		m_Headline.SetText(MilMo_Localization.GetLocString("World_379"));
		m_Headline.SetTexture("Batch01/Textures/Core/Invisible");
		m_Headline.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_Headline.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		m_Headline.SetScale(220f, 50f);
		m_Headline.SetFont(MilMo_GUI.Font.EborgSmall);
		m_Headline.SetDefaultTextColor(1f, 1f, 0f, 1f);
		m_Headline.SetTextOutline(1f, 1f);
		m_Headline.TextOutlineColor = new Color(0f, 0f, 0f, 0.25f);
		m_Headline.SetTextDropShadowPos(2f, 2f);
		m_Headline.FadeToDefaultColor = false;
		AddChild(m_Headline);
		m_InfoText = new MilMo_Widget(UI);
		m_InfoText.SetPosition(175f, UI.Next.y - 18f);
		m_InfoText.SetText(MilMo_Localization.GetLocString("World_380"));
		m_InfoText.SetTexture("Batch01/Textures/Core/Invisible");
		m_InfoText.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_InfoText.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		m_InfoText.SetScale(220f, 40f);
		m_InfoText.SetFont(MilMo_GUI.Font.ArialRounded);
		m_InfoText.SetDefaultTextColor(1f, 1f, 1f, 1f);
		m_InfoText.SetTextDropShadowPos(1f, 1f);
		m_InfoText.FadeToDefaultColor = false;
		AddChild(m_InfoText);
		m_TextField = new MilMo_SimpleTextField(UI);
		m_TextField.SetPosition(175f, UI.Next.y - 18f);
		m_TextField.SetScale(200f, 22f);
		m_TextField.SetMaxLength(128);
		AddChild(m_TextField);
		m_CancelButton = new MilMo_Button(UI);
		m_CancelButton.SetPosition(175f, UI.Next.y - 8f);
		m_CancelButton.SetScale(90f, 40f);
		m_CancelButton.SetText(MilMo_Localization.GetLocString("World_371"));
		m_CancelButton.SetFont(MilMo_GUI.Font.EborgSmall);
		m_CancelButton.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_CancelButton.SetTexture("Batch01/Textures/Dialog/ButtonBigNormal");
		m_CancelButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonBigMO");
		m_CancelButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonBigPressed");
		m_CancelButton.Function = delegate
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Cancel);
			Close(null);
		};
		AddChild(m_CancelButton);
		m_InviteButton = new MilMo_Button(UI);
		m_InviteButton.SetPosition(UI.Next.x, UI.Same.y);
		m_InviteButton.SetScale(90f, 40f);
		m_InviteButton.SetText(MilMo_Localization.GetLocString("World_370"));
		m_InviteButton.SetFont(MilMo_GUI.Font.EborgSmall);
		m_InviteButton.SetAlignment(MilMo_GUI.Align.TopLeft);
		m_InviteButton.SetTexture("Batch01/Textures/Dialog/ButtonBigNormal");
		m_InviteButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonBigMO");
		m_InviteButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonBigPressed");
		m_InviteButton.Function = Invite;
		AddChild(m_InviteButton);
		BringToFront(base.CloseButton);
		Step();
		Open();
	}

	public override void Draw()
	{
		if (Enabled)
		{
			base.Draw();
			MilMo_UserInterface.KeyboardFocus = true;
		}
	}

	public override void Open()
	{
		if (MilMo_LoadingScreen.Instance.LoadingState == MilMo_LoadingScreen.State.Disabled)
		{
			Singleton<InputController>.Instance.SetOldKeyboardFocusController();
			base.Open();
		}
	}

	public override void Close(object o)
	{
		Singleton<InputController>.Instance.RestorePreviousController();
		m_TextField.InputText = "";
		base.Close(null);
	}

	private void Invite(object o)
	{
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Confirm);
		string inputText = m_TextField.InputText;
		if (!inputText.Contains("@") || !inputText.Contains("."))
		{
			MilMo_EventSystem.At(1f, ShowFailDialog);
		}
		else if (Singleton<GameNetwork>.Instance.SendInviteRequest(inputText))
		{
			Close(null);
			DialogueSpawner.SpawnOkCancelModal("World_382", "World_383", new AddressableSpriteLoader("IconInvite"), delegate
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Select);
				Open();
			}, null);
		}
		else
		{
			MilMo_EventSystem.At(1f, ShowFailDialog);
		}
	}

	private void ShowFailDialog()
	{
		Close(null);
		DialogueSpawner.SpawnOkModal(new LocalizedStringWithArgument("World_384"), new LocalizedStringWithArgument("World_385"), new AddressableSpriteLoader("WarningIcon"), delegate
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Select);
			Open();
		});
	}
}
