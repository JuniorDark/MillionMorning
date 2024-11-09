using System;
using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Items;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.World.GUI.GameDialog;

public sealed class MilMo_GameDialogBox : MilMo_GameDialogWithOpeningSequence
{
	private List<Code.World.Feeds.MilMo_BoxLoot>.Enumerator _mBoxItemIterator;

	private readonly IMilMo_OpenableBox _mBox;

	private readonly MilMo_Button.ButtonFunc _mOkayFunction;

	public MilMo_GameDialogBox(MilMo_UserInterface ui, MilMo_Button.ButtonFunc okayFunction, IMilMo_OpenableBox box)
		: base(ui, okayFunction)
	{
		Vector2 res = ui.Res;
		ui.Res = new Vector2(1f, 1f);
		_mOkayFunction = okayFunction;
		_mBox = box;
		Identifier = "GameDialogBox_" + MilMo_Utility.ExtractNameFromPath(_mBox.IconPathClosed);
		SetHeadlineText(MilMo_Localization.GetLocString("World_8002"));
		PreloadIcon();
		CreateBoxIcon(_mBox.IconPathClosed, prefixStandardGUIPath: false);
		ui.Res = res;
	}

	private async void PreloadIcon()
	{
		await MilMo_ResourceManager.Instance.LoadTextureAsync(_mBox.IconPathOpen);
	}

	protected override void ShowSecondaryWidgets()
	{
		base.ShowSecondaryWidgets();
		SetHeadlineText(MilMo_LocString.Empty);
	}

	private void ShowNextItemInBox(object o)
	{
		if (PreviousIcon != null && PreviousIcon.Parent == this)
		{
			PreviousIcon.SetEnabled(e: false);
		}
		PreviousIcon = Icon;
		HideIcon();
		Vector2 res = UI.Res;
		UI.Res = new Vector2(1f, 1f);
		Icon = new MilMo_Widget(UI);
		Icon.SetPosPull(0.05f, 0.05f);
		Icon.SetPosDrag(0.6f, 0.6f);
		Icon.SetAnglePull(0.09f);
		Icon.SetAngleDrag(0.4f);
		Icon.ScaleNow(IconScale);
		Icon.SetAlignment(MilMo_GUI.Align.CenterCenter);
		Icon.FadeToDefaultColor = false;
		Icon.SetFadeSpeed(0.08f);
		Icon.SetMinScaleVel(0.01f, 0.01f);
		Icon.SetMinScale(0f, 0f);
		Icon.AllowPointerFocus = false;
		AddChild(Icon);
		UI.Res = res;
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

	public void Show(List<Code.World.Feeds.MilMo_BoxLoot>.Enumerator itemIterator)
	{
		Debug.Log("Show (item list) game dialog " + DebugId + " of type " + ToString());
		_mBoxItemIterator = itemIterator;
		if (_mBoxItemIterator.MoveNext())
		{
			ShouldShow = true;
			return;
		}
		throw new ArgumentException("Item iterator passed to game dialog in Box mode has no values");
	}

	protected override void ShowIcon()
	{
		base.ShowIcon();
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
		MilMo_EventSystem.At(MBoxOpened ? 1f : 2f, delegate
		{
			if (_mBox != null)
			{
				OpenBox(_mBox.IconPathOpen, prefixStandardGUIPath: false);
			}
			else
			{
				OpenBox("Batch01/Textures/Shop/IconGiftOpen", prefixStandardGUIPath: true);
			}
		});
	}

	protected override void ShowButtons()
	{
		base.ShowButtons();
		if (_mBoxItemIterator.MoveNext())
		{
			ButtonRight.SetText(MilMo_Localization.GetLocString("World_8004"));
			ButtonRight.Function = ShowNextItemInBox;
		}
		else
		{
			ButtonRight.SetText(MilMo_Localization.GetLocString("Generic_Okay"));
			ButtonRight.Function = _mOkayFunction;
		}
	}
}
