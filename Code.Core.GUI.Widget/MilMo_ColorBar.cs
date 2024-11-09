using System.Collections.Generic;
using Code.Core.GUI.Core;
using Code.Core.Sound;
using UnityEngine;

namespace Code.Core.GUI.Widget;

public sealed class MilMo_ColorBar : MilMo_Widget
{
	private readonly int _colorCount;

	private float _buttonSize = 50f;

	private float _padding = 5f;

	private const float CAPTION_SIZE = 27f;

	private float _buttonAlign;

	private readonly int _colorsPerRow;

	private MilMo_SimpleBox _frame;

	private readonly List<MilMo_Widget> _buttonList = new List<MilMo_Widget>();

	private readonly List<MilMo_Widget> _buttonShadeList = new List<MilMo_Widget>();

	public string ColorGroupName = "";

	public MilMo_Button SelectedButton { get; private set; }

	public string SelectedColorIndex => SelectedButton.Identifier;

	public MilMo_ColorBar(MilMo_UserInterface ui, int count, int colorsPerRow)
		: base(ui)
	{
		MilMo_ColorBar milMo_ColorBar = this;
		Identifier = "ColorBar " + MilMo_UserInterface.GetRandomID();
		_colorCount = count;
		_colorsPerRow = colorsPerRow;
		if (_colorsPerRow == 0)
		{
			_colorsPerRow = _colorCount;
		}
		CustomFunction = MilMo_Widget.Nothing;
		ScaleNow(200f, 200f / (float)_colorCount);
		GoToNow(0f, 0f);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		for (int j = 0; j < _colorCount; j++)
		{
			MilMo_Button milMo_Button = new MilMo_Button(UI);
			milMo_Button.SetAllTextures("Batch01/Textures/Core/ColorButton");
			milMo_Button.SetAlignment(MilMo_GUI.Align.BottomCenter);
			milMo_Button.SetFadeOutSpeed(0.05f * ((float)_colorCount * 0.35f / (float)j));
			milMo_Button.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
			milMo_Button.SetExtraScaleOnHover(0f, 2f);
			milMo_Button.SetScalePull(0.06f, 0.06f);
			milMo_Button.SetScaleDrag(0.5f, 0.5f);
			milMo_Button.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Impulse);
			milMo_Button.SetFadeInSpeed(0.03f);
			milMo_Button.Function = ClkSelect;
			milMo_Button.Args = j;
			milMo_Button.CustomFunction = null;
			milMo_Button.CustomArg = milMo_Button;
			AddChild(milMo_Button);
			_buttonList.Add(milMo_Button);
			SelectedButton = milMo_Button;
		}
		for (int i = 0; i < _colorCount; i++)
		{
			MilMo_Button shade = new MilMo_Button(UI);
			shade.SetAllTextures("Batch01/Textures/Core/ColorButtonShade");
			shade.SetAlignment(MilMo_GUI.Align.BottomCenter);
			shade.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
			shade.SetExtraScaleOnHover(0f, 2f);
			shade.SetScalePull(0.06f, 0.06f);
			shade.SetScaleDrag(0.5f, 0.5f);
			shade.SetPosPull(0f, 0.06f);
			shade.SetPosDrag(0f, 0.5f);
			shade.CustomArg = shade;
			shade.AllowPointerFocus = false;
			shade.SetDefaultColor(1f, 1f, 1f, 1f);
			AddChild(shade);
			_buttonShadeList.Add(shade);
			int ii = 0;
			base.Children.ForEach(delegate(MilMo_Widget widget)
			{
				if (ii < milMo_ColorBar._colorCount && ii == i)
				{
					shade.GetMotionFrom = widget;
				}
				ii++;
			});
		}
		SetTextAlignment(MilMo_GUI.Align.TopCenter);
		SetFontScale(1f);
		SetFont(MilMo_GUI.Font.GothamSmall);
	}

	public override void Step()
	{
		if (!Enabled)
		{
			return;
		}
		base.Step();
		foreach (MilMo_Widget child in base.Children)
		{
			child.SetAlpha(CurrentColor.a);
		}
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
			Rect screenPosition = GetScreenPosition();
			UnityEngine.GUI.skin = MilMo_GUISkins.GetSkin("Junebug");
			UnityEngine.GUI.Box(screenPosition, "");
			base.Draw();
		}
	}

	public void Select(int index)
	{
		for (int i = 0; i < base.Children.Count; i++)
		{
			if (i >= _colorCount)
			{
				if (base.Children[i].CustomArg is MilMo_Button milMo_Button)
				{
					if (i - _colorCount == index)
					{
						milMo_Button.ScaleImpulse(5f, 5f);
						milMo_Button.SetAllTextures("Batch01/Textures/Core/ColorButtonShadeSelect");
						milMo_Button.SetDefaultColor(1f, 1f, 1f, 1f);
					}
					else
					{
						milMo_Button.SetAllTextures("Batch01/Textures/Core/ColorButtonShade");
						milMo_Button.SetDefaultColor(1f, 1f, 1f, 1f);
					}
				}
			}
			else if (i == index)
			{
				(SelectedButton = base.Children[i].CustomArg as MilMo_Button)?.ScaleImpulse(5f, 5f);
			}
		}
	}

	public void ClkSelect(object obj, bool exec)
	{
		int b = (int)obj;
		int i = 0;
		base.Children.ForEach(delegate(MilMo_Widget widget)
		{
			if (i >= _colorCount)
			{
				MilMo_Button milMo_Button = (MilMo_Button)widget.CustomArg;
				if (i - _colorCount == b)
				{
					milMo_Button.ScaleImpulse(5f, 5f);
					milMo_Button.SetAllTextures("Batch01/Textures/Core/ColorButtonShadeSelect");
					milMo_Button.SetDefaultColor(1f, 1f, 1f, 1f);
				}
				else
				{
					milMo_Button.SetAllTextures("Batch01/Textures/Core/ColorButtonShade");
					milMo_Button.SetDefaultColor(1f, 1f, 1f, 1f);
				}
			}
			else if (i == b)
			{
				MilMo_Button milMo_Button2 = (MilMo_Button)widget.CustomArg;
				SelectedButton = milMo_Button2;
				milMo_Button2.ScaleImpulse(5f, 5f);
			}
			i++;
		});
		if (exec && CustomFunction != null && SelectedButton != null)
		{
			CustomFunction(SelectedButton.DefaultColor);
		}
	}

	public void ClkSelect(object o)
	{
		ClkSelect(o, exec: true);
	}

	private void SetButtonColor(int c, float r, float g, float b)
	{
		int v = 0;
		base.Children.ForEach(delegate(MilMo_Widget button)
		{
			if (v == c)
			{
				button.SetDefaultColor(new Color(r, g, b, 1f));
				button.SetColor(0f, 0f, 0f, 1f);
				MilMo_Button milMo_Button = (MilMo_Button)button.CustomArg;
				milMo_Button.SetHoverImpulseColor(r + 0.3f, g + 0.3f, b + 0.3f, 1f);
				SelectedButton = milMo_Button;
			}
			v++;
		});
	}

	public void SetButtonColor255(int c, Color color, string id)
	{
		if (c <= _buttonList.Count)
		{
			SetButtonColor(c, color.r, color.g, color.b);
			base.Children[c].Identifier = id;
		}
	}

	public string GetIdentifier()
	{
		return SelectedButton.Identifier;
	}

	public override void SetScale(float x, float y)
	{
		int rows = 1;
		if (_colorCount == _colorsPerRow)
		{
			y = x / (float)_colorCount + 27f + 5f;
		}
		else
		{
			rows = _colorCount / _colorsPerRow;
			y = (float)rows * (x / (float)_colorsPerRow) + 27f + 5f;
		}
		base.SetScale(x, y);
		if (rows > 1)
		{
			_buttonSize = ScaleMover.Target.x / base.Res.x / (float)(_colorsPerRow % _colorCount);
		}
		else
		{
			_buttonSize = ScaleMover.Target.x / base.Res.x / (float)_colorCount;
		}
		_padding = 0.1f;
		_buttonAlign = _buttonSize * 0.5f;
		int i = 0;
		int xx = 0;
		int yy = 1;
		base.Children.ForEach(delegate(MilMo_Widget button)
		{
			if (i < _colorCount)
			{
				button.GoToNow((float)xx * _buttonSize + _buttonAlign, (float)yy * _buttonSize + 27f - 5f);
				button.ScaleNow(_buttonSize * (1f - _padding), _buttonSize * (1f - _padding));
			}
			else
			{
				button.GoToNow((float)xx * _buttonSize + _buttonAlign, (float)(yy - rows) * _buttonSize + 27f - 5f);
				button.ScaleNow(_buttonSize * (1f - _padding), _buttonSize * (1f - _padding));
			}
			i++;
			xx++;
			if (xx == _colorsPerRow)
			{
				xx = 0;
				yy++;
			}
		});
	}

	public override void SetScale(Vector2 s)
	{
		SetScale(s.x, s.y);
	}

	private void SetScaleNew(float x)
	{
		float num = 1f;
		float y;
		if (_colorCount == _colorsPerRow)
		{
			y = x / (float)_colorCount + 27f + 5f;
		}
		else
		{
			num = (float)_colorCount / (float)_colorsPerRow;
			num = Mathf.CeilToInt(num);
			y = num * (x / (float)_colorsPerRow) + 27f + 5f;
		}
		base.SetScale(x, y);
		if (num > 1f)
		{
			_buttonSize = Scale.x / base.Res.x / (float)(_colorsPerRow % _colorCount);
		}
		else
		{
			_buttonSize = Scale.x / base.Res.x / (float)_colorCount;
		}
		_padding = 0.1f;
		_buttonAlign = _buttonSize * 0.5f;
		int num2 = 0;
		int num3 = 1;
		foreach (MilMo_Widget button in _buttonList)
		{
			button.GoToNow((float)num2 * _buttonSize + _buttonAlign, (float)num3 * _buttonSize + 27f - 5f);
			button.ScaleNow(_buttonSize * (1f - _padding), _buttonSize * (1f - _padding));
			num2++;
			if (num2 == _colorsPerRow)
			{
				num2 = 0;
				num3++;
			}
		}
		num2 = 0;
		num3 = 1;
		foreach (MilMo_Widget buttonShade in _buttonShadeList)
		{
			buttonShade.GoToNow((float)num2 * _buttonSize + _buttonAlign, (float)num3 * _buttonSize + 27f - 5f);
			buttonShade.ScaleNow(_buttonSize * (1f - _padding), _buttonSize * (1f - _padding));
			num2++;
			if (num2 == _colorsPerRow)
			{
				num2 = 0;
				num3++;
			}
		}
	}

	public override void SetYScale(float scale)
	{
		scale = (int)scale;
		SetScaleNew(0f);
		int num = 0;
		while ((float)(int)(Scale.y / base.Res.y) != scale)
		{
			num++;
			if ((float)(int)(Scale.y / base.Res.y) < scale)
			{
				SetScaleNew((int)(Scale.x / base.Res.x) + 1);
			}
			else
			{
				SetScaleNew((int)(Scale.x / base.Res.x) - 1);
			}
			if (num >= 1000)
			{
				Debug.LogWarning("MilMo_ColorBar:SetYScale failed. (infinite loop)");
				break;
			}
		}
	}

	public void SetMouseOverSound(MilMo_AudioClip clip)
	{
		int i = 0;
		base.Children.ForEach(delegate(MilMo_Widget button)
		{
			if (i < _colorCount)
			{
				((MilMo_Button)button.CustomArg).SetHoverSound(clip);
			}
			i++;
		});
	}

	public void SetClickSound(MilMo_AudioClip clip)
	{
		int i = 0;
		base.Children.ForEach(delegate(MilMo_Widget button)
		{
			if (i < _colorCount)
			{
				((MilMo_Button)button.CustomArg).SetClickSound(clip);
			}
			i++;
		});
	}
}
