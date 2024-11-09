using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.World.GUI.GameDialog;

public sealed class MilMo_GameDialogItem : MilMo_GameDialogGenericTextBox
{
	public MilMo_GameDialogItem(MilMo_UserInterface ui, MilMo_Button.ButtonFunc okayFunction, MilMo_LocString headline, MilMo_LocString eventDescription, MilMo_LocString itemName, MilMo_LocString itemDescription, HudDestination hudDestination)
		: this(ui, ButtonMode.Okay, okayFunction, null, headline, eventDescription, itemName, itemDescription, hudDestination)
	{
	}

	private MilMo_GameDialogItem(MilMo_UserInterface ui, ButtonMode buttonMode, MilMo_Button.ButtonFunc rightButtonFunction, MilMo_Button.ButtonFunc leftButtonFunction, MilMo_LocString headline, MilMo_LocString eventDescription, MilMo_LocString itemName, MilMo_LocString itemDescription, HudDestination hudDestination)
		: base(ui, buttonMode, rightButtonFunction, leftButtonFunction)
	{
		Vector2 res = ui.Res;
		ui.Res = new Vector2(1f, 1f);
		base.CustomJinglePath = "Content/Sounds/Batch01/GUI/GamePlayDialog/ReceivedItemJingle";
		SetEventDescription(eventDescription);
		SetHeadlineText(headline);
		SetHudDestination(hudDestination);
		SetText(itemName, itemDescription);
		Identifier = "GameDialogItem_" + itemName;
		ui.Res = res;
	}

	protected override void ShowIcon()
	{
		base.ShowIcon();
		Icon.GoToNow(IconPos);
		Icon.ScaleNow(0f, 0f);
		if (ShowIconEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(ShowIconEvent);
		}
		ShowIconEvent = MilMo_EventSystem.At(0f, delegate
		{
			Icon.SetAngle(0f - 3000f * MilMo_Utility.Random() - 12.5f);
			Icon.Angle(0f);
			Icon.SetAlpha(0f);
			Icon.AlphaTo(1f);
			Icon.SetScalePull(0.1f, 0.1f);
			Icon.SetScaleDrag(0.4f, 0.4f);
			Icon.ScaleNow(0f, 0f);
			Icon.ScaleTo(IconScale);
			PlayDialogSound(JingleClip);
		});
	}
}
