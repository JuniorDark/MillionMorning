using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.GUI.Widget.SimpleWindow.Window;

public class MilMo_Popup : MilMo_Window
{
	protected readonly MilMo_SimpleLabel MTextWidget;

	protected readonly MilMo_SimpleLabel CaptionWidget;

	protected readonly MilMo_Widget Background;

	protected readonly MilMo_Widget BlackOutline;

	protected readonly MilMo_Widget BackShade;

	public MilMo_Widget RequireWidgetEnabled;

	private readonly float _popupArrowHeight = 14f;

	private float _popupArrowOffset;

	protected readonly float ShadePadding = 4f;

	private float _captionWidgetOffset;

	public const float PADDING = 10f;

	private readonly Color _mDefaultColor = new Color(1f, 0.98f, 0.58f, 1f);

	private readonly Vector2 _captionSize = new Vector2(300f, 0f);

	protected Vector2 TextAreaSize;

	private bool _mWidgetsRefreshed;

	public float PopupArrowOffset
	{
		set
		{
			_popupArrowOffset = value;
			RefreshWidgets();
		}
	}

	public MilMo_SimpleLabel TextWidget => MTextWidget;

	public MilMo_Widget PopupArrow { get; }

	public float CaptionOffset { get; }

	protected float CaptionWidgetOffset
	{
		set
		{
			_captionWidgetOffset = value;
		}
	}

	public MilMo_Popup(MilMo_UserInterface ui, MilMo_LocString caption, MilMo_LocString message, Vector2 textAreaSize)
		: base(ui)
	{
		Identifier = "Popup";
		TextAreaSize = textAreaSize;
		base.FixedRes = true;
		SetSkin(1);
		AllowPointerFocus = false;
		ScaleMover.Val.x = 0f;
		ScaleMover.Val.y = 0f;
		ScaleMover.Target.x = 0f;
		ScaleMover.Target.y = 0f;
		SetScalePull(0.12f, 0.12f);
		SetScaleDrag(0.7f, 0.7f);
		SetPosPull(0.12f, 0.12f);
		SetPosDrag(0.7f, 0.7f);
		FadeToDefaultColor = false;
		SetFadeSpeed(0.04f);
		SetDefaultTextColor(Color.black);
		DefaultTextColor.a = 0f;
		TextColor.a = 0f;
		TargetTextColor.a = 0f;
		CurrentColor.a = 0f;
		TargetColor.a = 0f;
		SetAlignment(MilMo_GUI.Align.TopCenter);
		SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		base.Text = MilMo_LocString.Empty;
		BackShade = new MilMo_Widget(UI)
		{
			AllowPointerFocus = false
		};
		BackShade.GoToNow(ShadePadding + 1f, ShadePadding + 1f);
		BackShade.SetTexture("Batch01/Textures/Core/Black");
		BackShade.SetAlignment(MilMo_GUI.Align.TopLeft);
		BackShade.FadeToDefaultColor = false;
		BackShade.SetFadeSpeed(0.08f);
		BackShade.UseParentAlpha = false;
		AddChild(BackShade);
		BlackOutline = new MilMo_Widget(UI)
		{
			AllowPointerFocus = false
		};
		BlackOutline.GoToNow(0f, 0f);
		BlackOutline.SetTexture("Batch01/Textures/Core/Black");
		BlackOutline.SetAlignment(MilMo_GUI.Align.TopLeft);
		BlackOutline.FadeToDefaultColor = false;
		BlackOutline.SetFadeSpeed(0.08f);
		BlackOutline.SetColor(Color.black);
		BlackOutline.UseParentAlpha = false;
		AddChild(BlackOutline);
		Background = new MilMo_Widget(UI)
		{
			AllowPointerFocus = false
		};
		Background.SetPosition(1f, 1f);
		Background.SetScalePull(0.09f, 0.09f);
		Background.SetScaleDrag(0.7f, 0.7f);
		Background.SetPosPull(0.09f, 0.09f);
		Background.SetPosDrag(0.4f, 0.4f);
		Background.SetTextureWhite();
		Background.SetAlignment(MilMo_GUI.Align.TopLeft);
		Background.FadeToDefaultColor = false;
		Background.SetFadeSpeed(0.08f);
		Background.SetColor(_mDefaultColor);
		Background.UseParentAlpha = false;
		AddChild(Background);
		PopupArrow = new MilMo_Widget(UI)
		{
			AllowPointerFocus = false
		};
		PopupArrow.ScaleNow(24f, 0f);
		PopupArrow.ScaleTo(24f, 14f);
		PopupArrow.SetScalePull(0.09f, 0.09f);
		PopupArrow.SetScaleDrag(0.7f, 0.7f);
		PopupArrow.SetPosPull(0.09f, 0.09f);
		PopupArrow.SetPosDrag(0.4f, 0.4f);
		PopupArrow.SetTexture("Batch01/Textures/World/PopupArrow");
		PopupArrow.SetAlignment(MilMo_GUI.Align.TopCenter);
		PopupArrow.SetColor(_mDefaultColor);
		PopupArrow.FadeToDefaultColor = false;
		PopupArrow.SetFadeSpeed(0.08f);
		PopupArrow.UseParentAlpha = false;
		AddChild(PopupArrow);
		if (caption.String != "")
		{
			_captionSize = new Vector2(_captionSize.x, 30f);
		}
		CaptionOffset = _captionSize.y * 0.6f;
		CaptionWidget = new MilMo_SimpleLabel(UI);
		CaptionWidget.SetText(caption);
		CaptionWidget.AllowPointerFocus = false;
		CaptionWidget.SetFont(MilMo_GUI.Font.EborgSmall);
		CaptionWidget.SetScale(_captionSize);
		CaptionWidget.SetPosition(10f, 3f);
		CaptionWidget.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		CaptionWidget.SetDefaultTextColor(Color.black);
		CaptionWidget.SetFadeSpeed(0.04f);
		CaptionWidget.UseParentAlpha = false;
		AddChild(CaptionWidget);
		MTextWidget = new MilMo_SimpleLabel(UI);
		MTextWidget.SetText(message);
		MTextWidget.SetWordWrap(w: true);
		MTextWidget.SetScale(TextAreaSize);
		MTextWidget.SetPosition(10f, 10f + CaptionOffset);
		MTextWidget.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		MTextWidget.SetDefaultTextColor(Color.black);
		MTextWidget.SetFadeSpeed(0.04f);
		MTextWidget.AllowPointerFocus = false;
		MTextWidget.UseParentAlpha = false;
		MTextWidget.SetTextureWhite();
		AddChild(MTextWidget);
		RefreshUI();
		Initialize();
	}

	private void Initialize()
	{
		Open();
	}

	protected virtual void RefreshWidgets()
	{
		_mWidgetsRefreshed = true;
		RefreshUI();
		FadeIn();
	}

	private void RefreshUI()
	{
		InitWidgets();
		Vector2 scale = Scale;
		scale.x /= base.Res.x;
		scale.y /= base.Res.y;
		BackShade.GoToNow(ShadePadding + 1f, ShadePadding + 1f);
		BackShade.SetScale(scale.x, scale.y - (_popupArrowHeight - 1f));
		BlackOutline.GoToNow(0f, 0f);
		BlackOutline.SetScale(scale.x - ShadePadding, scale.y - ShadePadding - 8f);
		Background.SetPosition(1f, 1f);
		Background.SetScale(scale.x - ShadePadding - 2f, scale.y - ShadePadding - 2f - 8f);
		PopupArrow.ScaleNow(24f, 0f);
		PopupArrow.ScaleTo(24f, 14f);
		PopupArrow.SetPosition(scale.x / 2f + _popupArrowOffset, scale.y - _popupArrowHeight + 1f);
		CaptionWidget.SetPosition(10f + _captionWidgetOffset, 3f + _captionWidgetOffset);
		CaptionWidget.SetScale(_captionSize);
		CaptionWidget.SetFontScale(1f);
		MTextWidget.SetPosition(10f, 10f + CaptionOffset);
		MTextWidget.SetScale(TextAreaSize);
		MTextWidget.SetFontScale(1f);
		SpawnScale = Scale;
		TargetScale = Scale;
	}

	public override void Draw()
	{
		if (!Enabled)
		{
			return;
		}
		MilMo_Widget requireWidgetEnabled = RequireWidgetEnabled;
		if (requireWidgetEnabled != null && !requireWidgetEnabled.Enabled)
		{
			Remove();
			return;
		}
		if (!_mWidgetsRefreshed)
		{
			RefreshWidgets();
		}
		SetAlpha(0f);
		base.Draw();
	}

	protected virtual void FadeIn()
	{
		Background.SetAlpha(0f);
		Background.AlphaTo(1f);
		BlackOutline.SetAlpha(0f);
		BlackOutline.AlphaTo(1f);
		BackShade.SetAlpha(0f);
		BackShade.AlphaTo(0.3f);
		PopupArrow.SetAlpha(0f);
		PopupArrow.AlphaTo(1f);
		MTextWidget.SetAlpha(0f);
		MTextWidget.AlphaTo(1f);
		CaptionWidget.SetAlpha(0f);
		CaptionWidget.AlphaTo(1f);
	}

	protected virtual void InitWidgets()
	{
		SetScale(MTextWidget.Scale.x + 20f, MTextWidget.Scale.y + 20f + CaptionOffset);
	}

	public override void SetColor(Color col)
	{
		Background.TargetColor.r = col.r;
		Background.TargetColor.g = col.g;
		Background.TargetColor.b = col.b;
		PopupArrow.TargetColor.r = col.r;
		PopupArrow.TargetColor.g = col.g;
		PopupArrow.TargetColor.b = col.b;
	}

	public override void SetTextColor(Color col)
	{
		MTextWidget.DefaultTextColor.r = col.r;
		MTextWidget.DefaultTextColor.g = col.g;
		MTextWidget.DefaultTextColor.b = col.b;
		MTextWidget.DefaultTextColor.a = col.a;
	}

	public override void SetTextColor(float r, float g, float b, float a)
	{
		MTextWidget.DefaultTextColor.r = r;
		MTextWidget.DefaultTextColor.g = g;
		MTextWidget.DefaultTextColor.b = b;
		MTextWidget.DefaultTextColor.a = a;
	}

	private void SetCaptionTextColor(Color col)
	{
		CaptionWidget.DefaultTextColor.r = col.r;
		CaptionWidget.DefaultTextColor.g = col.g;
		CaptionWidget.DefaultTextColor.b = col.b;
		CaptionWidget.DefaultTextColor.a = col.a;
	}

	public void SetCaptionTextColor(float r, float g, float b, float a)
	{
		SetCaptionTextColor(new Color(r, g, b, a));
	}

	public void Remove()
	{
		DestroyFlag = true;
	}
}
