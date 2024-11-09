using System.Threading.Tasks;
using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Items;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.GameDialog;

public sealed class MilMo_GameDialogGift : MilMo_GameDialogWithOpeningSequence
{
	private readonly MilMo_Button _mGiftButton;

	private MilMo_TimerEvent _mShowGiftButtonEvent;

	public MilMo_GameDialogGift(MilMo_UserInterface ui, MilMo_Button.ButtonFunc okayFunction, MilMo_LocString giver, MilMo_LocString itemName, MilMo_LocString itemDescription, HudDestination hudDestination)
		: base(ui, okayFunction)
	{
		Vector2 res = ui.Res;
		ui.Res = new Vector2(1f, 1f);
		SetEventDescription(MilMo_Localization.GetLocString("World_6097"));
		SetHeadlineText(giver);
		SetHudDestination(hudDestination);
		SetText(itemName, itemDescription);
		Identifier = "GameDialogGift_" + itemName;
		CreateBoxIcon("Batch01/Textures/Shop/IconGift", prefixStandardGUIPath: true);
		_mGiftButton = new MilMo_Button(ui);
		_mGiftButton.SetText(MilMo_Localization.GetLocString("World_6100"));
		_mGiftButton.SetPosition(130f, 115f);
		_mGiftButton.ScaleNow(150f, 50f);
		_mGiftButton.SetTexture("Batch01/Textures/Dialog/ButtonMO");
		_mGiftButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonBright");
		_mGiftButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonBright");
		_mGiftButton.FadeToDefaultColor = false;
		_mGiftButton.SetFadeOutSpeed(0.08f);
		_mGiftButton.SetDefaultTextColor(new Color(1f, 1f, 1f, 1f));
		_mGiftButton.SetDefaultColor(ButtonColor);
		_mGiftButton.SetColor(1f, 1f, 1f, 0f);
		_mGiftButton.SetFont(MilMo_GUI.Font.EborgSmall);
		_mGiftButton.SetHoverSound(null);
		_mGiftButton.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		_mGiftButton.Function = GiftButtonCallback;
		AddChild(_mGiftButton);
		ui.Res = res;
	}

	private async void GiftButtonCallback(object arg)
	{
		_mGiftButton.SetText(null);
		_mGiftButton.Function = null;
		_mGiftButton.AlphaTo(0f);
		await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/Batch01/Textures/Shop/IconGiftOpen");
		ShakeBox();
		await Task.Delay(1500);
		MBoxIcon.SetColor(1f, 1f, 1f, 1f);
		OpenBox("Batch01/Textures/Shop/IconGiftOpen", prefixStandardGUIPath: true);
	}

	protected override void ScheduleShowCustomContent()
	{
		ScheduleShowOpenGiftButton();
	}

	protected override void ShowIcon()
	{
		base.ShowIcon();
		Icon.GoToNow(IconPos);
		if (ShowIconEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(ShowIconEvent);
		}
		ShowIconEvent = MilMo_EventSystem.At(0f, delegate
		{
			Vector2 res = UI.Res;
			UI.Res = new Vector2(1f, 1f);
			Icon.SetScalePull(0.1f, 0.1f);
			Icon.SetScaleDrag(0.4f, 0.4f);
			Icon.ScaleNow(0f, 0f);
			MBoxIcon.SetPosition(-300f, MBoxIconPos.y + 150f);
			MBoxIcon.GoTo(MBoxIconPos);
			MBoxIcon.SetFadeSpeed(0.01f);
			MBoxIcon.SetPosDrag(0.5f, 0.5f);
			MBoxIcon.ScaleNow(300f, 300f);
			MBoxIcon.ScaleTo(128f, 128f);
			PlayDialogSound(JingleClip);
			UI.Res = res;
		});
	}

	protected override void HideIcon()
	{
		MBoxIcon.GoTo(-200f, 50f);
		base.HideIcon();
	}

	public override async void Show(MilMo_Item item)
	{
		ShouldShow = true;
		await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/Batch01/Textures/Shop/IconGift");
		if (ShouldShow)
		{
			base.Show(item);
		}
	}

	private void ScheduleShowOpenGiftButton()
	{
		_mGiftButton.AllowPointerFocus = false;
		if (_mShowGiftButtonEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_mShowGiftButtonEvent);
		}
		_mShowGiftButtonEvent = MilMo_EventSystem.At(2f, delegate
		{
			_mGiftButton.AlphaTo(1f);
			_mGiftButton.SetHoverSound(TickSound);
			_mGiftButton.AllowPointerFocus = true;
		});
	}
}
