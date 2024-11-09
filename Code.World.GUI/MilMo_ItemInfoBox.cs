using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using UnityEngine;

namespace Code.World.GUI;

public sealed class MilMo_ItemInfoBox : MilMo_Widget
{
	private readonly float _mPadding;

	private MilMo_TimerEvent _mHideSchedule;

	private MilMo_TimerEvent _mDisableSchedule;

	private readonly MilMo_EventSystem.MilMo_Callback _hide;

	private readonly MilMo_EventSystem.MilMo_Callback _disable;

	private readonly Color _mDescriptionTextColor = new Color(1f, 0.8f, 0f, 1f);

	public MilMo_ItemInfoBox(MilMo_UserInterface ui)
		: base(ui)
	{
		UI = ui;
		Identifier = "ItemInfoBox";
		_hide = OnHide;
		_disable = OnDisable;
		UI.ResetLayout();
		GoToNow(200f, 500f);
		ScaleNow(300f, 100f);
		SetFont(MilMo_GUI.Font.EborgSmall);
		SetTextAlignment(MilMo_GUI.Align.TopLeft);
		SetTextDropShadowPos(2f, 2f);
		SetTextOffset(105f, 0f);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		AllowPointerFocus = false;
		SetFadeSpeed(0.07f);
		FadeToDefaultColor = false;
		SetPosPull(0.1f, 0.1f);
		SetPosDrag(0.6f, 0.6f);
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetDefaultColor(1f, 1f, 1f, 1f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.CenterCenter);
		milMo_Widget.SetTextDropShadowPos(5f, 5f);
		milMo_Widget.SetFontScale(1f);
		milMo_Widget.SetTextOffset(0f, 0f);
		milMo_Widget.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		milMo_Widget.SetPosPull(0.05f, 0.05f);
		milMo_Widget.SetPosDrag(0.6f, 0.6f);
		milMo_Widget.SetScalePull(0.09f, 0.09f);
		milMo_Widget.SetScaleDrag(0.7f, 0.7f);
		milMo_Widget.ScaleMover.MinVel.x = 0.1f;
		milMo_Widget.ScaleMover.MinVel.y = 0.1f;
		milMo_Widget.GoToNow(32f, 32f);
		milMo_Widget.ScaleNow(128f, 128f);
		milMo_Widget.SetFont(MilMo_GUI.Font.EborgMedium);
		milMo_Widget.AllowPointerFocus = false;
		milMo_Widget.FadeToDefaultColor = false;
		AddChild(milMo_Widget);
		_mPadding = 0.1f;
		Step();
	}

	public override void Draw()
	{
		if (Enabled)
		{
			Color currentColor = CurrentColor;
			if (Parent != null && UseParentAlpha)
			{
				currentColor.a *= Parent.CurrentColor.a;
			}
			UnityEngine.GUI.color = currentColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
			Font = UI.Font0;
			UnityEngine.GUI.skin = Font;
			Rect screenPosition = GetScreenPosition();
			UnityEngine.GUI.Box(screenPosition, "");
			Vector2 vector = new Vector2(base.Res.x * (_mPadding * 50f), base.Res.y * (_mPadding * 50f));
			screenPosition.x += vector.x;
			screenPosition.y += vector.y * 0.5f;
			screenPosition.width -= vector.x * 2f;
			screenPosition.height -= vector.y;
			screenPosition.x += 100f * base.Res.x;
			screenPosition.width -= 100f * base.Res.x;
			screenPosition.y += 30f * base.Res.y;
			screenPosition.height -= 30f * base.Res.y;
			currentColor = new Color(0f, 0f, 0f, 1f);
			currentColor.a *= CurrentColor.a;
			if (Parent != null && UseParentAlpha)
			{
				currentColor.a *= Parent.CurrentColor.a;
			}
			UnityEngine.GUI.color = currentColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
			UnityEngine.GUI.Label(screenPosition, "");
			screenPosition.x -= 1f * base.Res.x;
			screenPosition.y -= 1f * base.Res.y;
			currentColor = _mDescriptionTextColor;
			currentColor.a *= CurrentColor.a;
			if (Parent != null && UseParentAlpha)
			{
				currentColor.a *= Parent.CurrentColor.a;
			}
			UnityEngine.GUI.color = currentColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
			UnityEngine.GUI.Label(screenPosition, "");
			Font = UI.Font5;
			UnityEngine.GUI.skin = Font;
			base.Children.ForEach(delegate(MilMo_Widget w)
			{
				w.Draw();
			});
			DrawText();
		}
	}

	public void HideSoon()
	{
		if (_mHideSchedule != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_mHideSchedule);
		}
		_mHideSchedule = MilMo_EventSystem.At(1f, _hide);
	}

	public void OnHide()
	{
		if (_mHideSchedule != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_mHideSchedule);
		}
		if (_mDisableSchedule != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_mDisableSchedule);
		}
		_mDisableSchedule = MilMo_EventSystem.At(1f, _disable);
	}

	private void OnDisable()
	{
		SetAlpha(0f);
		SetEnabled(e: false);
	}
}
