using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.World.GUI;

public class MilMo_ChatBubble : MilMo_Widget
{
	public delegate void BubbleDone(MilMo_ChatBubble bubble);

	private readonly Vector2 _forcedScreenPos;

	public const int THINK = 1;

	private readonly MilMo_EventSystem.MilMo_Callback _fadeout;

	private readonly MilMo_EventSystem.MilMo_Callback _remove;

	private readonly MilMo_EventSystem.MilMo_Callback _showtext;

	private bool _fadingOut;

	private readonly int _mode = 1;

	private float _width;

	private float _height;

	private Rect _textRect;

	private Rect _frameRect;

	private Rect _shadowRect;

	private readonly MilMo_Widget _pointerShade;

	private readonly MilMo_Widget _plopShade1;

	private readonly MilMo_Widget _plopShade2;

	public readonly MilMo_Widget ChatPointer;

	public readonly MilMo_Widget ThinkPlop1;

	public readonly MilMo_Widget ThinkPlop2;

	private Vector2 _forcedScale = Vector2.zero;

	private Vector2 _forcedTextPadding = Vector2.zero;

	private readonly Vector2 _pointerSize = new Vector2(40f, 30f);

	private readonly Vector2 _thinkPlopSize = new Vector2(40f, 35f);

	private const float PADDING = 15f;

	private const float SHADE_PADDING = 6f;

	private const float LIFE_TIME_PER_LETTER = 0.15f;

	private readonly Color _bubbleColor;

	private readonly float _plopOffset1;

	private readonly float _plopOffset2;

	private BubbleDone _callback;

	private readonly MilMo_TimerEvent _hideTextEvent;

	private readonly MilMo_TimerEvent _hidePlop1Event;

	private readonly MilMo_TimerEvent _hidePlop2Event;

	private readonly MilMo_TimerEvent _fadeOutEvent;

	private readonly MilMo_TimerEvent _removeEvent;

	private readonly MilMo_TimerEvent _lastEvent;

	public MilMo_ChatBubble(MilMo_UserInterface ui, MilMo_LocString message, Vector2 forcedScreenPos)
		: base(ui)
	{
		int mode = 1;
		_bubbleColor = new Color(0f, 0.5f, 1f, 1f);
		_forcedScreenPos = forcedScreenPos;
		Identifier = "ChatBubble";
		_plopOffset1 = _thinkPlopSize.y + 3f;
		_plopOffset2 = _thinkPlopSize.y * 2.2f + 3f;
		_fadeout = FadeOut;
		_remove = Remove;
		_showtext = OnShowText;
		ScaleMover.Val.x = 0f;
		ScaleMover.Val.y = 0f;
		ScaleMover.Target.x = 0f;
		ScaleMover.Target.y = 0f;
		SetScalePull(0.12f, 0.12f);
		SetScaleDrag(0.7f, 0.7f);
		ScaleMover.MinVel.x = 0.01f;
		ScaleMover.MinVel.y = 0.01f;
		SetPosPull(0.12f, 0.12f);
		SetPosDrag(0.7f, 0.7f);
		PosMover.Target.x = 0f;
		PosMover.Target.y = 0f;
		PosMover.Val.x = 0f;
		PosMover.Val.y = 0f;
		SetSkin(1);
		SetAlignment(MilMo_GUI.Align.BottomCenter);
		FadeToDefaultColor = false;
		SetFadeSpeed(0.01f);
		SetDefaultTextColor(_bubbleColor);
		DefaultTextColor.a = 0f;
		TextColor.a = 0f;
		TargetTextColor.a = 0f;
		CurrentColor.a = 0f;
		TargetColor.a = 0f;
		SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		base.Text = message;
		AllowPointerFocus = false;
		_pointerShade = new MilMo_Widget(UI);
		_pointerShade.GoToNow(0f, 0f);
		_pointerShade.ScaleNow(0f, 0f);
		_pointerShade.SetScalePull(0.09f, 0.09f);
		_pointerShade.SetScaleDrag(0.7f, 0.7f);
		_pointerShade.ScaleMover.MinVel.x = 0.01f;
		_pointerShade.ScaleMover.MinVel.y = 0.01f;
		_pointerShade.SetPosPull(0.09f, 0.09f);
		_pointerShade.SetPosDrag(0.4f, 0.4f);
		_pointerShade.SetTexture("Batch01/Textures/World/ChatPointerShade");
		_pointerShade.SetAlignment(MilMo_GUI.Align.TopCenter);
		_pointerShade.FadeToDefaultColor = false;
		_pointerShade.SetFadeSpeed(0.008f);
		_pointerShade.SetDefaultColor(_bubbleColor);
		_pointerShade.SetAlpha(0.3f);
		_pointerShade.AllowPointerFocus = false;
		UI.AddChild(_pointerShade);
		ChatPointer = new MilMo_Widget(UI);
		ChatPointer.GoToNow(0f, 0f);
		ChatPointer.ScaleNow(0f, 0f);
		ChatPointer.SetScalePull(0.09f, 0.09f);
		ChatPointer.SetScaleDrag(0.7f, 0.7f);
		ChatPointer.ScaleMover.MinVel.x = 0.01f;
		ChatPointer.ScaleMover.MinVel.y = 0.01f;
		ChatPointer.SetPosPull(0.09f, 0.09f);
		ChatPointer.SetPosDrag(0.4f, 0.4f);
		ChatPointer.SetTexture("Batch01/Textures/World/ChatPointer");
		ChatPointer.SetAlignment(MilMo_GUI.Align.TopCenter);
		ChatPointer.FadeToDefaultColor = false;
		ChatPointer.SetFadeSpeed(0.008f);
		ChatPointer.AllowPointerFocus = false;
		UI.AddChild(ChatPointer);
		_plopShade1 = new MilMo_Widget(UI);
		_plopShade1.GoToNow(0f, 0f);
		_plopShade1.ScaleNow(0f, 0f);
		_plopShade1.SetScalePull(0.12f, 0.12f);
		_plopShade1.SetScaleDrag(0.6f, 0.6f);
		_plopShade1.ScaleMover.MinVel.x = 0.01f;
		_plopShade1.ScaleMover.MinVel.y = 0.01f;
		_plopShade1.SetPosPull(0.09f, 0.09f);
		_plopShade1.SetPosDrag(0.4f, 0.4f);
		_plopShade1.SetTexture("Batch01/Textures/World/ChatThinkPlop");
		_plopShade1.SetAlignment(MilMo_GUI.Align.CenterCenter);
		_plopShade1.FadeToDefaultColor = false;
		_plopShade1.SetFadeSpeed(0.008f);
		_plopShade1.AllowPointerFocus = false;
		_plopShade1.SetDefaultColor(_bubbleColor);
		_plopShade1.SetAlpha(0.3f);
		_plopShade1.SetRotationType(MilMo_Mover.UpdateFunc.Linear);
		_plopShade1.SetAngleVel(3f);
		UI.AddChild(_plopShade1);
		_plopShade2 = new MilMo_Widget(UI);
		_plopShade2.GoToNow(0f, 0f);
		_plopShade2.ScaleNow(0f, 0f);
		_plopShade2.SetScalePull(0.12f, 0.12f);
		_plopShade2.SetScaleDrag(0.6f, 0.6f);
		_plopShade2.ScaleMover.MinVel.x = 0.01f;
		_plopShade2.ScaleMover.MinVel.y = 0.01f;
		_plopShade2.SetPosPull(0.12f, 0.12f);
		_plopShade2.SetPosDrag(0.6f, 0.6f);
		_plopShade2.SetTexture("Batch01/Textures/World/ChatThinkPlop");
		_plopShade2.SetAlignment(MilMo_GUI.Align.CenterCenter);
		_plopShade2.FadeToDefaultColor = false;
		_plopShade2.SetFadeSpeed(0.008f);
		_plopShade2.SetDefaultColor(_bubbleColor);
		_plopShade2.SetAlpha(0.3f);
		_plopShade2.AllowPointerFocus = false;
		_plopShade2.SetRotationType(MilMo_Mover.UpdateFunc.Linear);
		_plopShade2.SetAngleVel(-3f);
		UI.AddChild(_plopShade2);
		ThinkPlop1 = new MilMo_Widget(UI);
		ThinkPlop1.GoToNow(0f, 0f);
		ThinkPlop1.ScaleNow(0f, 0f);
		ThinkPlop1.SetScalePull(0.12f, 0.12f);
		ThinkPlop1.SetScaleDrag(0.6f, 0.6f);
		ThinkPlop1.ScaleMover.MinVel.x = 0.01f;
		ThinkPlop1.ScaleMover.MinVel.y = 0.01f;
		ThinkPlop1.ScaleMover.MinVel.x = 0.05f;
		ThinkPlop1.ScaleMover.MinVel.y = 0.05f;
		ThinkPlop1.SetPosPull(0.09f, 0.09f);
		ThinkPlop1.SetPosDrag(0.4f, 0.4f);
		ThinkPlop1.AllowPointerFocus = false;
		ThinkPlop1.SetTexture("Batch01/Textures/World/ChatThinkPlop");
		ThinkPlop1.SetAlignment(MilMo_GUI.Align.CenterCenter);
		ThinkPlop1.FadeToDefaultColor = false;
		ThinkPlop1.SetFadeSpeed(0.008f);
		ThinkPlop1.SetRotationType(MilMo_Mover.UpdateFunc.Linear);
		ThinkPlop1.SetAngleVel(3f);
		UI.AddChild(ThinkPlop1);
		ThinkPlop2 = new MilMo_Widget(UI);
		ThinkPlop2.GoToNow(0f, 0f);
		ThinkPlop2.ScaleNow(0f, 0f);
		ThinkPlop2.SetScalePull(0.12f, 0.12f);
		ThinkPlop2.SetScaleDrag(0.6f, 0.6f);
		ThinkPlop2.ScaleMover.MinVel.x = 0.01f;
		ThinkPlop2.ScaleMover.MinVel.y = 0.01f;
		ThinkPlop2.ScaleMover.MinVel.x = 0.05f;
		ThinkPlop2.ScaleMover.MinVel.y = 0.05f;
		ThinkPlop2.SetPosPull(0.12f, 0.12f);
		ThinkPlop2.SetPosDrag(0.6f, 0.6f);
		ThinkPlop2.AllowPointerFocus = false;
		ThinkPlop2.SetTexture("Batch01/Textures/World/ChatThinkPlop");
		ThinkPlop2.SetAlignment(MilMo_GUI.Align.CenterCenter);
		ThinkPlop2.FadeToDefaultColor = false;
		ThinkPlop2.SetFadeSpeed(0.008f);
		ThinkPlop2.SetRotationType(MilMo_Mover.UpdateFunc.Linear);
		ThinkPlop2.SetAngleVel(-3f);
		UI.AddChild(ThinkPlop2);
		float num = Mathf.Max(5f, (float)base.Text.Length * 0.15f);
		_hideTextEvent = MilMo_EventSystem.At(num - 1f, OnHideText);
		_hidePlop1Event = MilMo_EventSystem.At(num - 0.66f, OnHidePlop1);
		_hidePlop2Event = MilMo_EventSystem.At(num - 0.33f, OnHidePlop2);
		_fadeOutEvent = MilMo_EventSystem.At(num, _fadeout);
		_removeEvent = MilMo_EventSystem.At(num + 4f, _remove);
		_lastEvent = MilMo_EventSystem.At(num, delegate
		{
			_callback?.Invoke(this);
		});
		SetScaleDrag(0.5f, 0.5f);
		ThinkPlop1.ScaleTo(_thinkPlopSize * 0.5f);
		_plopShade1.ScaleTo(_thinkPlopSize * 0.5f + new Vector2(15f, 15f));
		ThinkPlop1.ScaleMover.Arrive = ShowPlop2;
		_mode = mode;
		GoToNow(_forcedScreenPos);
		ChatPointer.GoToNow((Pos.x + Scale.x / 2f) / base.Res.x, (Pos.y + Scale.y) / base.Res.y + 15f - (_pointerSize.y + 3f));
		_pointerShade.GoToNow((Pos.x + Scale.x / 2f) / base.Res.x, (Pos.y + Scale.y) / base.Res.y + 15f - (_pointerSize.y + 3f));
		float x = (Pos.x + Scale.x / 2f) / base.Res.x;
		float num2 = (Pos.y + Scale.y) / base.Res.y + 15f;
		ThinkPlop1.GoToNow(x, num2 - _plopOffset1);
		_plopShade1.GoToNow(x, num2 - _plopOffset1);
		ThinkPlop2.GoToNow(x, num2 - _plopOffset2);
		_plopShade2.GoToNow(x, num2 - _plopOffset2);
	}

	public void ForceClose()
	{
		_fadeout();
		MilMo_EventSystem.At(0.2f, delegate
		{
			MilMo_EventSystem.RemoveTimerEvent(_hideTextEvent);
			MilMo_EventSystem.RemoveTimerEvent(_hidePlop1Event);
			MilMo_EventSystem.RemoveTimerEvent(_hidePlop2Event);
			MilMo_EventSystem.RemoveTimerEvent(_fadeOutEvent);
			MilMo_EventSystem.RemoveTimerEvent(_removeEvent);
			MilMo_EventSystem.RemoveTimerEvent(_lastEvent);
			_remove();
			if (_callback != null)
			{
				_callback(this);
			}
		});
	}

	public void SetCallback(BubbleDone callback)
	{
		_callback = callback;
	}

	private void ShowPlop2()
	{
		ThinkPlop2.ScaleTo(_thinkPlopSize);
		_plopShade2.ScaleTo(_thinkPlopSize + new Vector2(15f, 15f));
		ThinkPlop2.ScaleMover.Arrive = ShowBubble;
		ThinkPlop1.ScaleMover.Arrive = MilMo_Widget.Nothing;
	}

	private void ShowBubble()
	{
		SetAlpha(1f);
		MilMo_EventSystem.At(0.15f, _showtext);
		Vector2 vector = UI.Skins[1].label.CalcSize(new GUIContent(base.Text.String));
		float num = vector.x * vector.y;
		_width = (int)Mathf.Max(50f, Mathf.Sqrt(5f * num / 2f));
		_height = (int)(GetLineCount(base.Text.String) * vector.y + 5f);
		if (_forcedScale != Vector2.zero)
		{
			_width = _forcedScale.x;
			_height = _forcedScale.y;
		}
		ScaleMover.Target.x = _width;
		ScaleMover.Target.y = _height;
		ScaleMover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Spring);
		ThinkPlop2.ScaleMover.Arrive = MilMo_Widget.Nothing;
	}

	private void OnShowText()
	{
		DefaultTextColor = _bubbleColor;
	}

	private void OnHideText()
	{
		SetFadeSpeed(0.07f);
		DefaultTextColor = new Color(1f, 1f, 1f, 0f);
	}

	private void OnHidePlop1()
	{
		_fadingOut = true;
		SetFadeSpeed(0.1f);
		AlphaTo(0f);
		ScaleTo(0f, 0f);
	}

	private void OnHidePlop2()
	{
		ThinkPlop2.SetFadeSpeed(0.1f);
		ThinkPlop2.AlphaTo(0f);
		ThinkPlop2.ScaleTo(0f, 0f);
		_plopShade2.SetFadeSpeed(0.1f);
		_plopShade2.AlphaTo(0f);
		_plopShade2.ScaleTo(0f, 0f);
	}

	public override void Draw()
	{
		if (!Enabled)
		{
			return;
		}
		if (_forcedScreenPos == Vector2.zero)
		{
			Remove();
			return;
		}
		Vector2 vector = new Vector2(base.Res.x * 15f, base.Res.y * 15f);
		float num = 6f;
		float num2;
		if (_mode == 1)
		{
			num *= 2f;
			num2 = _thinkPlopSize.y * 3f;
		}
		else
		{
			num2 = _pointerSize.y;
		}
		_textRect = GetScreenPosition();
		_textRect.y -= num2 * base.Res.y;
		_frameRect = _textRect;
		_frameRect.x -= vector.x;
		_frameRect.y -= vector.y;
		_frameRect.width += vector.x * 2f;
		_frameRect.height += vector.y * 2f;
		_textRect.x += _forcedTextPadding.x;
		_textRect.y += _forcedTextPadding.y;
		_shadowRect = _frameRect;
		_shadowRect.x -= num;
		_shadowRect.y -= num;
		_shadowRect.width += num * 2f;
		_shadowRect.height += num * 2f;
		if (_fadingOut && _mode != 1)
		{
			SetInvisible();
		}
		float x = (Pos.x + Scale.x / 2f) / base.Res.x;
		float num3 = (Pos.y + Scale.y) / base.Res.y + 15f;
		ThinkPlop1.GoTo(x, num3 - _plopOffset1);
		_plopShade1.GoTo(x, num3 - _plopOffset1);
		ThinkPlop2.GoTo(x, num3 - _plopOffset2);
		_plopShade2.GoTo(x, num3 - _plopOffset2);
		GUISkin skin = UnityEngine.GUI.skin;
		UnityEngine.GUI.skin = Skin;
		Color bubbleColor = _bubbleColor;
		bubbleColor.a = CurrentColor.a * 0.2f;
		UnityEngine.GUI.color = bubbleColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
		UnityEngine.GUI.Box(_shadowRect, "");
		bubbleColor = CurrentColor;
		UnityEngine.GUI.color = bubbleColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
		UnityEngine.GUI.Box(_frameRect, "");
		GUIStyle label = UI.Skins[1].label;
		Color textColor = label.normal.textColor;
		label.normal.textColor = TextColor;
		UnityEngine.GUI.Label(_textRect, base.Text.String, label);
		label.normal.textColor = textColor;
		UnityEngine.GUI.skin = skin;
		foreach (MilMo_Widget child in base.Children)
		{
			child.Draw();
		}
	}

	private void SetInvisible()
	{
		SetFadeSpeed(0.07f);
		AlphaTo(0f);
		ThinkPlop1.SetFadeSpeed(0.09f);
		ThinkPlop1.AlphaTo(0f);
		ThinkPlop2.SetFadeSpeed(0.09f);
		ThinkPlop2.AlphaTo(0f);
		_plopShade1.SetFadeSpeed(0.09f);
		_plopShade1.AlphaTo(0f);
		_plopShade2.SetFadeSpeed(0.09f);
		_plopShade2.AlphaTo(0f);
	}

	private float GetLineCount(string msg)
	{
		int num = 1;
		int num2 = 0;
		float num3 = 0f;
		int num4 = 0;
		string[] array = msg.Split();
		foreach (string text in array)
		{
			num4++;
			num3 = UI.Skins[1].label.CalcSize(new GUIContent(text)).x + 3f;
			num2 += (int)num3;
			if (num3 > (float)(int)_width)
			{
				num += (int)num3 / (int)_width + 1;
			}
			else if (num2 > (int)_width)
			{
				num++;
				num2 = (int)num3;
			}
		}
		if (num4 != 1)
		{
			return Mathf.Max(num, 2f);
		}
		if (num3 > (float)(int)_width)
		{
			_width = Mathf.Min(100f, (int)num3);
		}
		return Mathf.Max(num, 2f);
	}

	private void FadeOut()
	{
		ThinkPlop1.SetFadeSpeed(0.1f);
		ThinkPlop1.AlphaTo(0f);
		ThinkPlop1.ScaleTo(0f, 0f);
		_plopShade1.SetFadeSpeed(0.1f);
		_plopShade1.AlphaTo(0f);
		_plopShade1.ScaleTo(0f, 0f);
		_fadingOut = true;
	}

	public void Remove()
	{
		UI.RemoveChild(this);
		UI.RemoveChild(ChatPointer);
		UI.RemoveChild(_pointerShade);
		UI.RemoveChild(ThinkPlop1);
		UI.RemoveChild(ThinkPlop2);
		UI.RemoveChild(_plopShade1);
		UI.RemoveChild(_plopShade2);
	}
}
