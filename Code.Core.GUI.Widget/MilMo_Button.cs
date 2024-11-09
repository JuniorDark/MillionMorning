using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.Input;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using UnityEngine;

namespace Code.Core.GUI.Widget;

public class MilMo_Button : MilMo_Widget
{
	public delegate void ButtonFunc(object arg);

	public delegate void PointerHoverFunc();

	public delegate void PointerLeaveFunc();

	private MilMo_Texture _hoverTexture;

	private MilMo_Texture _pressedTexture;

	private Vector2 _extraScaleOnHover;

	private float _hoverAngle;

	public Vector2 DefaultScale;

	private float _defaultAngle;

	private Color _hoverColor;

	private Color _hoverImpulseColor;

	private Color _hoverTextColor;

	private MilMo_GUI.HoverBehaviour _hoverFadeMode;

	private MilMo_GUI.HoverBehaviour _hoverScaleMode;

	private float _fadeInSpeed;

	private float _fadeOutSpeed;

	private MilMo_GUI.DrawFunction _lastDrawFunction;

	private bool _pointerStillInside;

	private bool _hover;

	private bool _ignoreNextClick;

	public bool HasLinearRotation;

	public ButtonFunc Function;

	public ButtonFunc LeftMouseDownFunction;

	public ButtonFunc RightClickFunction;

	public ButtonFunc DoubleClickFunction;

	public object Args;

	public static ButtonFunc StaticFunction;

	public PointerHoverFunc PointerHoverFunction;

	public PointerLeaveFunc PointerLeaveFunction;

	private MilMo_AudioClip _hoverSound;

	private MilMo_AudioClip _clickSound;

	private MilMo_Tooltip _tooltip;

	internal MilMo_TimerEvent TooltipTimerEvent;

	public Color HoverColor => _hoverColor;

	public MilMo_Tooltip Tooltip
	{
		get
		{
			return _tooltip;
		}
		set
		{
			_tooltip = value;
			if (_tooltip != null)
			{
				_tooltip.SetParentButton(this);
			}
		}
	}

	public bool IgnoreNextClick
	{
		set
		{
			_ignoreNextClick = value;
		}
	}

	public MilMo_Button(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "Button " + MilMo_UserInterface.GetRandomID();
		SetDefaultScale(50f, 50f);
		SetExtraScaleOnHover(5f, 5f);
		_hoverScaleMode = MilMo_GUI.HoverBehaviour.None;
		SetDefaultAngle(0f);
		SetHoverAngle(0f);
		_hoverColor = DefaultColor;
		_hoverImpulseColor = DefaultColor;
		_hoverTextColor = _hoverColor;
		TargetColor = DefaultColor;
		CurrentColor = DefaultColor;
		_fadeInSpeed = 0.1f;
		_fadeOutSpeed = 0.01f;
		_hoverFadeMode = MilMo_GUI.HoverBehaviour.None;
		_lastDrawFunction = MilMo_GUI.DrawFunction.Normal;
		_pointerStillInside = false;
		_hover = false;
		_ignoreNextClick = false;
		Function = MilMo_Widget.Nothing;
		RightClickFunction = MilMo_Widget.Nothing;
		DoubleClickFunction = MilMo_Widget.Nothing;
		PointerHoverFunction = MilMo_Widget.Nothing;
		PointerLeaveFunction = MilMo_Widget.Nothing;
		SetArguments(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
		_hoverSound = null;
		_clickSound = null;
	}

	private void DrawNormal()
	{
		FadeSpeed = _fadeOutSpeed;
		base.Draw();
	}

	private void DrawOnHover()
	{
		if (_hoverScaleMode == MilMo_GUI.HoverBehaviour.Fade || _hoverScaleMode == MilMo_GUI.HoverBehaviour.Snap)
		{
			ScaleTo(DefaultScale.x / base.Res.x, DefaultScale.y / base.Res.y);
		}
		TextColor = _hoverTextColor;
		base.Draw();
	}

	public void HoverScaleImpulse()
	{
		if (!_pointerStillInside)
		{
			ScaleImpulse(_extraScaleOnHover.x / base.Res.x, _extraScaleOnHover.y / base.Res.y);
			_pointerStillInside = true;
		}
	}

	private void HoverPosImpulse()
	{
		if (!_pointerStillInside)
		{
			float num = _extraScaleOnHover.x / base.Res.x;
			float num2 = _extraScaleOnHover.y / base.Res.y;
			ImpulseRandom(num, num, num2, num2);
			_pointerStillInside = true;
		}
	}

	private void PlayHoverSound()
	{
		if (_hoverSound != null)
		{
			UI.SoundFx.Stop();
			UI.SoundFx.Play(_hoverSound);
		}
	}

	private void PlayClickSound()
	{
		if (_clickSound != null)
		{
			if (UI.SoundFx.IsPlaying())
			{
				UI.SoundFx.Stop();
			}
			UI.SoundFx.Play(_clickSound);
		}
	}

	public void SetHoverTexture(MilMo_Texture texture)
	{
		_hoverTexture = texture;
	}

	public void SetHoverTexture(string filename)
	{
		_hoverTexture = new MilMo_Texture("Content/GUI/" + filename);
		_hoverTexture.AsyncLoad();
	}

	public void SetAllTextures(MilMo_Texture icon)
	{
		_hoverTexture = icon;
		_pressedTexture = icon;
		base.Texture = icon;
	}

	public void SetAllTextures(Texture2D texture)
	{
		SetAllTextures(new MilMo_Texture(texture));
	}

	public void SetAllTextures(string filename, bool prefixStandardGuiPath = true)
	{
		if (string.IsNullOrEmpty(filename))
		{
			Debug.LogWarning("filename is empty, will just set textures to null");
			_hoverTexture = null;
			_pressedTexture = null;
		}
		else
		{
			base.Texture = (prefixStandardGuiPath ? new MilMo_Texture("Content/GUI/" + filename) : new MilMo_Texture(filename));
			base.Texture.AsyncLoad();
			_hoverTexture = base.Texture;
			_pressedTexture = base.Texture;
		}
	}

	public void SetPressedTexture(MilMo_Texture texture)
	{
		_pressedTexture = texture;
	}

	public void SetPressedTexture(string filename)
	{
		_pressedTexture = new MilMo_Texture("Content/GUI/" + filename);
		_pressedTexture.AsyncLoad();
	}

	public void SetDefaultScale(float x, float y)
	{
		x *= base.Res.x;
		y *= base.Res.y;
		DefaultScale.x = x;
		DefaultScale.y = y;
	}

	public void SetDefaultScale(Vector2 scale)
	{
		scale.x *= base.Res.x;
		scale.y *= base.Res.y;
		DefaultScale.x = scale.x;
		DefaultScale.y = scale.y;
	}

	public void SetDefaultAngle(float a)
	{
		_defaultAngle = a;
	}

	public void SetHoverAngle(float a)
	{
		_hoverAngle = a;
	}

	public void SetExtraScaleOnHover(float x, float y)
	{
		x *= base.Res.x;
		y *= base.Res.y;
		_extraScaleOnHover.x = x;
		_extraScaleOnHover.y = y;
	}

	public void SetHoverScaleMode(MilMo_GUI.HoverBehaviour mode)
	{
		_hoverScaleMode = mode;
	}

	public void SetHoverColor(float r, float g, float b, float a)
	{
		_hoverColor.r = r;
		_hoverColor.g = g;
		_hoverColor.b = b;
		_hoverColor.a = a;
	}

	public void SetHoverColor(Color col)
	{
		_hoverColor.r = col.r;
		_hoverColor.g = col.g;
		_hoverColor.b = col.b;
		_hoverColor.a = col.a;
	}

	public void SetHoverTextColor(float r, float g, float b, float a)
	{
		_hoverTextColor.r = r;
		_hoverTextColor.g = g;
		_hoverTextColor.b = b;
		_hoverTextColor.a = a;
	}

	public void SetHoverTextColor(Color col)
	{
		_hoverTextColor.r = col.r;
		_hoverTextColor.g = col.g;
		_hoverTextColor.b = col.b;
		_hoverTextColor.a = col.a;
	}

	public void SetHoverImpulseColor(float r, float g, float b, float a)
	{
		_hoverImpulseColor.r = r;
		_hoverImpulseColor.g = g;
		_hoverImpulseColor.b = b;
		_hoverImpulseColor.a = a;
	}

	public void SetFadeInSpeed(float f)
	{
		_fadeInSpeed = ((_fadeInSpeed > 0f) ? f : 0f);
	}

	public void SetFadeOutSpeed(float f)
	{
		_fadeOutSpeed = ((_fadeOutSpeed > 0f) ? f : 0f);
	}

	public void SetHoverFadeMode(MilMo_GUI.HoverBehaviour mode)
	{
		_hoverFadeMode = mode;
	}

	public void SetHoverSound(MilMo_AudioClip clip)
	{
		_hoverSound = clip;
	}

	public void SetClickSound(MilMo_AudioClip clip)
	{
		_clickSound = clip;
	}

	public void SetArguments(float a0, float a1, float a2, float a3, float a4, float a5, float a6, float a7)
	{
		Args = new float[8] { a0, a1, a2, a3, a4, a5, a6, a7 };
	}

	public static MilMo_Button CreateNewButton(MilMo_UserInterface ui, string texture, string mouseOverTexture, string mouseDownTexture, Vector2 scale, MilMo_LocString text)
	{
		MilMo_Button milMo_Button = new MilMo_Button(ui);
		milMo_Button.SetTexture(texture);
		milMo_Button.SetHoverTexture(mouseOverTexture);
		milMo_Button.SetPressedTexture(mouseDownTexture);
		milMo_Button.SetText(text);
		milMo_Button.SetFont(MilMo_GUI.Font.EborgMedium);
		milMo_Button.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		milMo_Button.SetScale(scale);
		milMo_Button.SetClickSound(new MilMo_AudioClip("Content/Sounds/Batch01/CharBuilder/Pick"));
		milMo_Button.SetHoverSound(new MilMo_AudioClip("Content/Sounds/Batch01/GUI/Generic/Tick"));
		return milMo_Button;
	}

	public override void Step()
	{
		if (!IsEnabled())
		{
			return;
		}
		bool flag = UI.ModalDialog != null && GetAncestor() != UI.ModalDialog;
		if (MilMo_GUI.GlobalFade != 0f || IgnoreGlobalFade)
		{
			if (Hover())
			{
				if (MilMo_Pointer.LeftButton)
				{
					if (!_ignoreNextClick && LeftMouseDownFunction != null)
					{
						LeftMouseDownFunction(Args);
						_ignoreNextClick = true;
					}
				}
				else if (MilMo_Pointer.LeftClick)
				{
					if (flag)
					{
						UI.FlashModalDialog();
						return;
					}
					if (_tooltip != null && _tooltip.Enabled && TooltipTimerEvent != null)
					{
						MilMo_EventSystem.RemoveTimerEvent(TooltipTimerEvent);
						TooltipTimerEvent = null;
						_tooltip.ClickClose();
					}
					if (!_ignoreNextClick && StaticFunction != null)
					{
						StaticFunction(Args);
					}
					if (!_ignoreNextClick && Function != null)
					{
						Function(Args);
						PlayClickSound();
					}
					else
					{
						_ignoreNextClick = false;
					}
				}
				if (MilMo_Pointer.RightClick)
				{
					if (flag)
					{
						UI.FlashModalDialog();
						return;
					}
					if (!_ignoreNextClick && RightClickFunction != null)
					{
						RightClickFunction(Args);
						PlayClickSound();
					}
					else
					{
						_ignoreNextClick = false;
					}
				}
				if (MilMo_Pointer.LeftDoubleClick)
				{
					if (flag)
					{
						UI.FlashModalDialog();
						return;
					}
					if (!_ignoreNextClick && DoubleClickFunction != null)
					{
						DoubleClickFunction(Args);
						PlayClickSound();
					}
					else
					{
						_ignoreNextClick = false;
					}
				}
				if (flag)
				{
					return;
				}
				if (!_hover)
				{
					if (PointerHoverFunction != null)
					{
						PointerHoverFunction();
					}
					PlayHoverSound();
					if (_tooltip != null && !_tooltip.Enabled)
					{
						TooltipTimerEvent = MilMo_EventSystem.At(0.5f, delegate
						{
							if (Hover())
							{
								_tooltip.Open();
							}
						});
					}
					MilMo_UserInterface.SelectedWidget = this;
				}
				_hover = true;
				if (!HasLinearRotation)
				{
					Angle(_hoverAngle);
				}
				CurrentTexture = ((!MilMo_Pointer.LeftButton) ? _hoverTexture : _pressedTexture);
				FadeSpeed = _fadeInSpeed;
				switch (_hoverFadeMode)
				{
				case MilMo_GUI.HoverBehaviour.Fade:
					ColorTo(_hoverColor);
					TextColor = _hoverTextColor;
					break;
				case MilMo_GUI.HoverBehaviour.Impulse:
					if (_lastDrawFunction != MilMo_GUI.DrawFunction.Hover)
					{
						ColorNow(_hoverImpulseColor);
						ColorTo(_hoverColor);
						TextColorNow(_hoverImpulseColor);
						TextColorTo(_hoverTextColor);
					}
					else
					{
						TextColorTo(_hoverTextColor);
					}
					break;
				}
				switch (_hoverScaleMode)
				{
				case MilMo_GUI.HoverBehaviour.Fade:
					ScaleTo(DefaultScale.x / base.Res.x + _extraScaleOnHover.x / base.Res.x, DefaultScale.y / base.Res.y + _extraScaleOnHover.y / base.Res.y);
					break;
				case MilMo_GUI.HoverBehaviour.Impulse:
					if (_lastDrawFunction != MilMo_GUI.DrawFunction.Hover)
					{
						HoverScaleImpulse();
					}
					break;
				case MilMo_GUI.HoverBehaviour.PosImpulse:
					if (_lastDrawFunction != MilMo_GUI.DrawFunction.Hover)
					{
						HoverPosImpulse();
					}
					break;
				case MilMo_GUI.HoverBehaviour.Snap:
					ScaleNow(DefaultScale.x / base.Res.x + _extraScaleOnHover.x / base.Res.x, DefaultScale.y / base.Res.y + _extraScaleOnHover.y / base.Res.y);
					break;
				}
			}
			else
			{
				if (flag)
				{
					return;
				}
				_ignoreNextClick = MilMo_Pointer.LeftButton;
				_pointerStillInside = false;
				if (_hover && PointerLeaveFunction != null)
				{
					PointerLeaveFunction();
					if (_tooltip != null)
					{
						if (TooltipTimerEvent != null)
						{
							MilMo_EventSystem.RemoveTimerEvent(TooltipTimerEvent);
							TooltipTimerEvent = null;
						}
						if (_tooltip.Enabled)
						{
							_tooltip.Close();
						}
					}
				}
				_hover = false;
				CurrentTexture = base.Texture;
				if (!HasLinearRotation)
				{
					Angle(_defaultAngle);
				}
			}
		}
		base.Step();
	}

	public override void Draw()
	{
		if (IsEnabled())
		{
			if (!_hover)
			{
				DrawNormal();
				_lastDrawFunction = MilMo_GUI.DrawFunction.Normal;
			}
			else
			{
				DrawOnHover();
				_lastDrawFunction = MilMo_GUI.DrawFunction.Hover;
			}
		}
	}
}
