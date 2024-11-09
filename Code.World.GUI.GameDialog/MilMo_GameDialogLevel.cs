using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.GameDialog;

public class MilMo_GameDialogLevel : MilMo_GameDialogGenericTextBox
{
	private readonly Color _mLevelBoxColor = new Color(0.8f, 0.8f, 1f, 1f);

	public MilMo_GameDialogLevel(MilMo_UserInterface ui, MilMo_Button.ButtonFunc okayFunction, MilMo_LocString headline, MilMo_LocString eventDescription, MilMo_LocString levelName, MilMo_LocString levelDescription)
		: this(ui, ButtonMode.Okay, okayFunction, null, headline, eventDescription, levelName, levelDescription)
	{
	}

	public MilMo_GameDialogLevel(MilMo_UserInterface ui, MilMo_Button.ButtonFunc shareFunction, MilMo_Button.ButtonFunc skipFunction, MilMo_LocString headline, MilMo_LocString eventDescription, MilMo_LocString levelName, MilMo_LocString levelDescription)
		: this(ui, ButtonMode.SkipShare, shareFunction, skipFunction, headline, eventDescription, levelName, levelDescription)
	{
	}

	private MilMo_GameDialogLevel(MilMo_UserInterface ui, ButtonMode buttonMode, MilMo_Button.ButtonFunc rightButtonFunction, MilMo_Button.ButtonFunc leftButtonFunction, MilMo_LocString headline, MilMo_LocString eventDescription, MilMo_LocString levelName, MilMo_LocString levelDescription)
		: base(ui, buttonMode, rightButtonFunction, leftButtonFunction)
	{
		Vector2 res = ui.Res;
		ui.Res = new Vector2(1f, 1f);
		base.CustomJinglePath = "Content/Sounds/Batch01/GUI/GamePlayDialog/NewLevelJingle";
		ItemFrameColor = new Color(0f, 0f, 1f, 0.8f);
		SetHeadlineText(headline);
		SetEventDescription(eventDescription);
		SetText(levelName, levelDescription);
		Identifier = "GameDialogLevel_" + levelName;
		IconScale = new Vector2(128f, 128f);
		IconPos = new Vector2(55f, -10f);
		Headline.SetPosition(120f, 6f);
		EventDescription.SetPosition(122f, 27f);
		TextBox.SetDefaultColor(_mLevelBoxColor);
		ui.Res = res;
	}

	protected override void ShowIcon()
	{
		base.ShowIcon();
		Vector2 res = UI.Res;
		UI.Res = new Vector2(1f, 1f);
		Icon.SetPosition(-200f, IconPos.y - 100f);
		Icon.GoTo(IconPos);
		Icon.SetFadeSpeed(0.01f);
		Icon.SetPosDrag(0.4f, 0.4f);
		Icon.ScaleNow(0f, 0f);
		if (ShowIconEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(ShowIconEvent);
		}
		ShowIconEvent = MilMo_EventSystem.At(0f, delegate
		{
			Icon.SetAlpha(0f);
			Icon.AlphaTo(1f);
			Icon.SetScalePull(0.1f, 0.1f);
			Icon.SetScaleDrag(0.2f, 0.2f);
			Icon.ScaleNow(0f, 0f);
			Icon.ScaleTo(IconScale);
			PlayDialogSound(JingleClip);
		});
		UI.Res = res;
	}
}
