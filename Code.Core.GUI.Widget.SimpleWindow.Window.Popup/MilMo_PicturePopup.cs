using System;
using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.GUI.Widget.SimpleWindow.Window.Popup;

public class MilMo_PicturePopup : MilMo_Popup
{
	public MilMo_ProgressBar ProgressBar;

	public MilMo_Widget ProgressCounter;

	private readonly MilMo_Widget _miniIcon;

	protected readonly float MIconPadding;

	protected float BottomPad = 10f;

	public float IconAlignment;

	protected float IconScale { get; private set; }

	public MilMo_Widget Icon { get; private set; }

	public MilMo_PicturePopup(MilMo_UserInterface ui, MilMo_LocString caption, MilMo_LocString message, Vector2 textAreaSize)
		: base(ui, caption, message, textAreaSize)
	{
		IconScale = 100f;
		Identifier = "PicturePopup";
		base.FixedRes = true;
		MIconPadding = 5f;
		base.CaptionWidgetOffset = -5f;
		AllowPointerFocus = false;
		BackDrop.AllowPointerFocus = false;
		Icon = new MilMo_Widget(UI);
		Icon.AllowPointerFocus = false;
		Icon.SetScalePull(0.09f, 0.09f);
		Icon.SetScaleDrag(0.7f, 0.7f);
		Icon.SetPosPull(0.09f, 0.09f);
		Icon.SetPosDrag(0.4f, 0.4f);
		Icon.SetTexture("Batch01/Textures/CharBuilder/IconShirt");
		Icon.SetAlignment(MilMo_GUI.Align.CenterCenter);
		Icon.FadeToDefaultColor = false;
		Icon.SetFadeSpeed(0.08f);
		Icon.SetColor(1f, 1f, 1f, 1f);
		Icon.UseParentAlpha = false;
		AddChild(Icon);
		InitWidgets();
	}

	protected override void RefreshWidgets()
	{
		float num = IconScale + base.CaptionOffset + MIconPadding * 2f + BottomPad + 2f;
		if (ScaleMover.Target.y < num)
		{
			SetYScale(num);
		}
		base.RefreshWidgets();
		RefreshIconAlignment();
		Icon.SetScale(60f, 60f);
		Icon.ScaleTo(IconScale, IconScale);
	}

	public virtual void RefreshIconAlignment()
	{
		float num = 0f;
		float num2 = 0f;
		if (Math.Abs(IconAlignment - 0f) < 0.001f)
		{
			num = IconScale;
		}
		else if (Math.Abs(IconAlignment - 1f) < 0.001f)
		{
			num2 = base.TextWidget.Scale.x;
		}
		base.TextWidget.SetPosition(10f + num, 10f + base.CaptionOffset);
		ProgressBar?.SetXPos(11f + num);
		ProgressCounter?.SetXPos(53f + num);
		_miniIcon?.SetXPos(100f + num);
		Icon?.SetPosition(MIconPadding + IconScale / 2f + num2, MIconPadding + base.CaptionOffset + IconScale / 2f);
	}

	protected override void FadeIn()
	{
		base.FadeIn();
		Icon.SetAlpha(0f);
		Icon.AlphaTo(Icon.DefaultColor.a);
	}

	public virtual void FadeOut()
	{
		base.TextWidget.SetDefaultTextColor(1f, 1f, 1f, 0f);
		CaptionWidget.SetDefaultTextColor(1f, 1f, 1f, 0f);
		MilMo_EventSystem.At(1f, base.Remove);
		foreach (MilMo_Widget child in base.Children)
		{
			child.SetAlpha(0f);
		}
		Background.SetAlpha(0f);
		BlackOutline.SetAlpha(0f);
		BackShade.SetAlpha(0f);
		base.PopupArrow.SetAlpha(0f);
		base.TextWidget.SetAlpha(0f);
		CaptionWidget.SetAlpha(0f);
		Icon.SetAlpha(0f);
	}

	protected override void InitWidgets()
	{
		RefreshIconAlignment();
		SetScale(base.TextWidget.Scale.x + 20f + IconScale, base.TextWidget.Scale.y + 20f + base.CaptionOffset + BottomPad);
		float num = IconScale + base.CaptionOffset + MIconPadding * 2f + BottomPad + 2f;
		if (ScaleMover.Target.y < num)
		{
			SetYScale(num);
		}
	}
}
