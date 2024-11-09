using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.Core.Utility;
using UnityEngine;

namespace Code.World.GUI.GameDialog;

public abstract class MilMo_GameDialogWithOpeningSequence : MilMo_GameDialogGenericTextBox
{
	protected Vector2 MBoxIconPos = new Vector2(60f, 37f);

	protected MilMo_Widget MBoxIcon;

	protected bool MBoxOpened;

	private readonly MilMo_AudioClip _mGiftSqueezeSound = new MilMo_AudioClip("Content/Sounds/Batch01/GUI/GamePlayDialog/GiftSqueeze");

	private readonly MilMo_AudioClip _mGiftPopOpenSound = new MilMo_AudioClip("Content/Sounds/Batch01/GUI/GamePlayDialog/GiftPopOpen");

	protected MilMo_GameDialogWithOpeningSequence(MilMo_UserInterface ui, MilMo_Button.ButtonFunc okayFunction)
		: base(ui, ButtonMode.Okay, okayFunction, null)
	{
		Vector2 res = ui.Res;
		ui.Res = new Vector2(1f, 1f);
		base.CustomJinglePath = "Content/Sounds/Batch01/GUI/GamePlayDialog/ReceivedItemJingle";
		IconScale = new Vector2(128f, 128f);
		IconPos = new Vector2(95f, -75f);
		Headline.SetPosition(152f, 32f);
		Headline.SetAlignment(MilMo_GUI.Align.TopCenter);
		Headline.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		EventDescription.SetPosition(122f, 19f);
		EventDescription.SetAlignment(MilMo_GUI.Align.CenterLeft);
		EventDescription.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		ui.Res = res;
	}

	protected void CreateBoxIcon(string texturePath, bool prefixStandardGUIPath)
	{
		MBoxIcon = new MilMo_Widget(UI);
		MBoxIcon.SetPosition(MBoxIconPos);
		MBoxIcon.SetPosPull(0.05f, 0.05f);
		MBoxIcon.SetPosDrag(0.6f, 0.6f);
		MBoxIcon.SetScalePull(0.1f, 0.1f);
		MBoxIcon.SetScaleDrag(0.4f, 0.4f);
		MBoxIcon.SetAnglePull(0.09f);
		MBoxIcon.SetAngleDrag(0.4f);
		MBoxIcon.ScaleNow(0f, 0f);
		MBoxIcon.SetAlignment(MilMo_GUI.Align.BottomCenter);
		MBoxIcon.SetTexture(texturePath, prefixStandardGUIPath);
		MBoxIcon.FadeToDefaultColor = false;
		MBoxIcon.SetFadeSpeed(0.08f);
		MBoxIcon.SetMinScaleVel(0.01f, 0.01f);
		MBoxIcon.SetMinScale(0f, 0f);
		MBoxIcon.AllowPointerFocus = false;
		AddChild(MBoxIcon);
	}

	protected void CreateBoxIcon(MilMo_Texture texture)
	{
		MBoxIcon = new MilMo_Widget(UI);
		MBoxIcon.SetPosition(MBoxIconPos);
		MBoxIcon.SetPosPull(0.05f, 0.05f);
		MBoxIcon.SetPosDrag(0.6f, 0.6f);
		MBoxIcon.SetScalePull(0.1f, 0.1f);
		MBoxIcon.SetScaleDrag(0.4f, 0.4f);
		MBoxIcon.SetAnglePull(0.09f);
		MBoxIcon.SetAngleDrag(0.4f);
		MBoxIcon.ScaleNow(0f, 0f);
		MBoxIcon.SetAlignment(MilMo_GUI.Align.BottomCenter);
		MBoxIcon.SetTexture(texture);
		MBoxIcon.FadeToDefaultColor = false;
		MBoxIcon.SetFadeSpeed(0.08f);
		MBoxIcon.SetMinScaleVel(0.01f, 0.01f);
		MBoxIcon.SetMinScale(0f, 0f);
		MBoxIcon.AllowPointerFocus = false;
		AddChild(MBoxIcon);
	}

	protected void ShakeBox()
	{
		float flip = 1f;
		UI.SoundFx.Play(_mGiftSqueezeSound);
		MBoxIcon.SetPosition(MBoxIconPos);
		for (int i = 0; i < 30; i++)
		{
			MilMo_EventSystem.At(0.05f * (float)i, delegate
			{
				MBoxIcon.ScaleImpulse(3f, -12f);
			});
		}
		MilMo_EventSystem.At(0.5f, delegate
		{
			MBoxIcon.ColorTo(1f, 0f, 0f, 1f);
			for (int j = 0; j < 10; j++)
			{
				float flip1 = flip;
				MilMo_EventSystem.At(0.1f * (float)j, delegate
				{
					MBoxIcon.SetPosition(MBoxIconPos);
					MBoxIcon.Impulse(3f * flip1, 0f);
				});
				flip = 0f - flip;
			}
		});
	}

	protected void OpenBox(string openBoxTexturePath, bool prefixStandardGUIPath)
	{
		if (!MBoxOpened)
		{
			UI.SoundFx.Play(_mGiftPopOpenSound);
			MBoxIcon.SetTexture(openBoxTexturePath, prefixStandardGUIPath);
			MBoxIcon.SetPosPull(0.05f, 0.05f);
			MBoxIcon.SetPosDrag(0.4f, 0.4f);
			MBoxIcon.GoTo(7f, 10f);
			MBoxOpened = true;
		}
		MBoxIcon.ScaleTo(100f, 100f);
		MBoxIcon.ScaleImpulse(-20f, 90f);
		MBoxIcon.SetScalePull(0.1f, 0.1f);
		MBoxIcon.SetScaleDrag(0.6f, 0.6f);
		MBoxIcon.AngleImpulse(30f, 30f);
		Icon.GoTo(60f, -100f);
		Icon.SetAngle(0f - 3000f * MilMo_Utility.Random() - 12.5f);
		Icon.Angle(0f);
		Icon.SetFadeSpeed(0.08f);
		Icon.SetAlpha(0f);
		Icon.AlphaTo(1f);
		MilMo_EventSystem.At(0.1f, delegate
		{
			Icon.Impulse(10f, -100f);
		});
		MilMo_EventSystem.At(0.2f, delegate
		{
			Icon.GoTo(130f, -80f);
		});
		Icon.ScaleToTexture();
		MilMo_EventSystem.At(1f, delegate
		{
			ScheduleShowSecondaryWidgets();
			ScheduleShowButtons();
		});
	}
}
