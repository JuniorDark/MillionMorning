using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Items;
using Code.Core.Items.Home;
using Code.Core.ResourceSystem;
using Code.World.Player;
using UnityEngine;

namespace Code.World.GUI.GameDialog;

public sealed class MilMo_GameDialogConverter : MilMo_GameDialogWithOpeningSequence
{
	public class Converter
	{
		internal readonly MilMo_Texture MTexture;

		internal readonly MilMo_Item MItem;

		internal readonly int MAmountOfItems;

		internal readonly string MOpenTexture;

		public Converter(MilMo_Texture texture, string openTexture, MilMo_Item item, int itemCount)
		{
			MOpenTexture = openTexture;
			MTexture = texture;
			MItem = item;
			MAmountOfItems = itemCount;
		}
	}

	private readonly Converter _mConverter;

	private readonly MilMo_Button.ButtonFunc _mOkayFunction;

	public MilMo_GameDialogConverter(MilMo_UserInterface ui, MilMo_Button.ButtonFunc okayFunction, Converter converter)
		: base(ui, okayFunction)
	{
		Vector2 res = ui.Res;
		ui.Res = new Vector2(1f, 1f);
		_mOkayFunction = okayFunction;
		_mConverter = converter;
		Identifier = "GameDialogConverter";
		SetHeadlineText(MilMo_Localization.GetLocString("World_10264"));
		CreateBoxIcon(_mConverter.MTexture);
		ui.Res = res;
	}

	protected override void ShowSecondaryWidgets()
	{
		base.ShowSecondaryWidgets();
		SetHeadlineText(MilMo_LocString.Empty);
	}

	protected override void GrowIn()
	{
		if (!MBoxOpened)
		{
			base.GrowIn();
		}
		else
		{
			ScaleTo(265f, Height);
		}
	}

	protected override void ScheduleShowCustomContent()
	{
		ScheduleOpenBox();
	}

	public void Show()
	{
		Debug.Log("Show (item list) game dialog " + DebugId + " of type " + ToString());
		ShouldShow = true;
		if (ShouldShow)
		{
			MilMo_LocString milMo_LocString;
			if (_mConverter.MAmountOfItems > 1)
			{
				milMo_LocString = MilMo_Localization.GetLocString("World_8003").GetCopy();
				milMo_LocString.SetFormatArgs(_mConverter.MAmountOfItems.ToString(), _mConverter.MItem.Template.DisplayName);
			}
			else
			{
				milMo_LocString = _mConverter.MItem.Template.DisplayName;
			}
			SetText(milMo_LocString, _mConverter.MItem.Template.FeedDescriptionIngame);
			Show(_mConverter.MItem);
		}
	}

	protected override void ShowIcon()
	{
		base.ShowIcon();
		if (_mConverter.MItem is MilMo_HomeEquipment)
		{
			if (MilMo_Player.Instance.InNavigator)
			{
				SetHudDestination(HudDestination.HomeInNav);
			}
			else if (MilMo_Player.InMyHome)
			{
				SetHudDestination(HudDestination.Storage);
			}
			else
			{
				SetHudDestination(HudDestination.Home);
			}
		}
		else
		{
			SetHudDestination(HudDestination.Bag);
		}
		Vector2 iconPos = IconPos;
		if (MBoxOpened)
		{
			iconPos.x -= 60f;
		}
		Icon.GoToNow(iconPos);
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
			if (!MBoxOpened)
			{
				MBoxIcon.SetPosition(-300f, MBoxIconPos.y + 150f);
				MBoxIcon.GoTo(MBoxIconPos);
				MBoxIcon.SetFadeSpeed(0.01f);
				MBoxIcon.SetPosDrag(0.5f, 0.5f);
				MBoxIcon.ScaleNow(300f, 300f);
				MBoxIcon.ScaleTo(128f, 128f);
			}
			PlayDialogSound(JingleClip);
			UI.Res = res;
		});
	}

	protected override void HideIcon()
	{
		if (!IsActive)
		{
			MBoxIcon.GoTo(-200f, 50f);
		}
		base.HideIcon();
	}

	private void ScheduleOpenBox()
	{
		MilMo_EventSystem.At(MBoxOpened ? 0.5f : 1f, delegate
		{
			OpenBox((_mConverter != null) ? _mConverter.MOpenTexture : "Batch01/Textures/Shop/IconGiftOpen", prefixStandardGUIPath: true);
		});
	}

	protected override void ShowButtons()
	{
		base.ShowButtons();
		ButtonRight.SetText(MilMo_Localization.GetLocString("Generic_Okay"));
		ButtonRight.Function = _mOkayFunction;
	}
}
