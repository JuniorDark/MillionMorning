using System;
using System.Collections.Generic;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.Ladder;

public sealed class MilMo_LadderField : MilMo_Widget
{
	public delegate void ButtonCallback(int entryId);

	private readonly Vector2 _mFieldScale = new Vector2(515f, 25f);

	private readonly MilMo_Button _mButton;

	private readonly MilMo_Widget _mBackground;

	public MilMo_LadderField(MilMo_UserInterface ui, int rank, int entryIdentifier, ButtonCallback buttonCallback, string buttonTexture, MilMo_LocString buttonTooltip)
		: base(ui)
	{
		MilMo_LadderField milMo_LadderField = this;
		_mBackground = new MilMo_Widget(UI);
		_mBackground.SetTextureBlack();
		_mBackground.SetAlignment(MilMo_GUI.Align.CenterLeft);
		AddChild(_mBackground);
		_mBackground.Enabled = false;
		ButtonCallback mCallback = buttonCallback;
		SetDefaultColor(0.6f, 0.6f, 0.6f, 1f);
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.GoToNow(5f, 0f);
		milMo_Widget.ScaleNow(60f, _mFieldScale.y);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.CenterLeft);
		milMo_Widget.Step();
		milMo_Widget.SetText(MilMo_Localization.GetNotLocalizedLocString(rank.ToString()));
		milMo_Widget.SetFont(MilMo_GUI.Font.EborgSmall);
		milMo_Widget.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		milMo_Widget.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		AddChild(milMo_Widget);
		_mButton = new MilMo_Button(UI);
		_mButton.SetAllTextures(buttonTexture);
		_mButton.SetScale(22f, 22f);
		_mButton.SetAlignment(MilMo_GUI.Align.CenterCenter);
		_mButton.Tooltip = new MilMo_Tooltip(buttonTooltip);
		_mButton.PointerHoverFunction = delegate
		{
			milMo_LadderField._mButton.ScaleImpulse(1f, 1f);
		};
		_mButton.SetScalePull(0.1f, 0.1f);
		_mButton.SetScaleDrag(0.6f, 0.6f);
		_mButton.Function = delegate
		{
			if (mCallback != null)
			{
				mCallback(entryIdentifier);
			}
		};
		AddChild(_mButton);
	}

	public void SetIsLocalPlayer()
	{
		foreach (MilMo_Widget child in base.Children)
		{
			child.SetDefaultTextColor(Color.green);
			if (child.Info == 999)
			{
				child.SetAlpha(0f);
			}
		}
		_mBackground.Enabled = true;
		SendToBack(_mBackground);
	}

	public void Init(List<float> widths, List<string> texts, float score)
	{
		float num = 60f;
		for (int i = 0; i < texts.Count; i++)
		{
			MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
			milMo_Widget.GoToNow(num, 0f);
			milMo_Widget.SetText(MilMo_Localization.GetNotLocalizedLocString(texts[i]));
			milMo_Widget.SetAlignment(MilMo_GUI.Align.CenterLeft);
			milMo_Widget.SetScale(widths[i] * 12f, _mFieldScale.y);
			milMo_Widget.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
			milMo_Widget.SetFont(MilMo_GUI.Font.EborgSmall);
			milMo_Widget.SetFontScale(0.75f);
			milMo_Widget.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
			milMo_Widget.SetExtraDrawTextSize(2f, 8f);
			AddChild(milMo_Widget);
			num += widths[i] * 12f;
		}
		num = VisualizeScore(score, 5, num);
		_mButton.SetPosition(num + 22f, 0f);
		SetScale(_mFieldScale);
		_mBackground.SetScale(_mFieldScale.x, _mFieldScale.y);
		_mBackground.SetPosition(15f, 0f);
	}

	private float VisualizeScore(float score, int max, float startPos)
	{
		float num = startPos;
		for (int i = 0; i < max; i++)
		{
			MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
			milMo_Widget.SetTexture("Batch01/Textures/Voting/starEmpty");
			milMo_Widget.FadeToDefaultColor = false;
			milMo_Widget.SetPosition(startPos, 0f);
			milMo_Widget.SetEnabled(e: true);
			milMo_Widget.AllowPointerFocus = false;
			milMo_Widget.SetAlignment(MilMo_GUI.Align.CenterLeft);
			milMo_Widget.SetScale(22f, 22f);
			milMo_Widget.Info = 999;
			startPos += 22f;
			AddChild(milMo_Widget);
		}
		MilMo_Button milMo_Button = new MilMo_Button(UI);
		milMo_Button.SetAlignment(MilMo_GUI.Align.CenterLeft);
		milMo_Button.SetPosition(num, 0f);
		milMo_Button.SetScale(startPos - num, 22f);
		MilMo_LocString copy = MilMo_Localization.GetLocString("Homes_13292").GetCopy();
		copy.SetFormatArgs(Math.Round(score, 2));
		milMo_Button.Tooltip = new MilMo_Tooltip(copy);
		AddChild(milMo_Button);
		float result = startPos;
		startPos = num;
		while (score > 0f)
		{
			score -= 1f;
			MilMo_Widget milMo_Widget2 = new MilMo_Widget(UI);
			milMo_Widget2.SetTexture("Batch01/Textures/Voting/starFilled");
			if (score < 0f)
			{
				milMo_Widget2.SetCropMode(MilMo_GUI.CropMode.Cropadelic);
				milMo_Widget2.MxFillAmount = 1f + score;
			}
			milMo_Widget2.SetPosition(startPos, 0f);
			milMo_Widget2.SetEnabled(e: true);
			milMo_Widget2.AllowPointerFocus = false;
			milMo_Widget2.SetAlignment(MilMo_GUI.Align.CenterLeft);
			milMo_Widget2.SetScale(22f, 22f);
			startPos += 22f;
			AddChild(milMo_Widget2);
		}
		return result;
	}
}
