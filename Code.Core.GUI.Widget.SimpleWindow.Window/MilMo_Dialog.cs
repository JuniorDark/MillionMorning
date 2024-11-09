using System;
using System.Linq;
using Code.Core.GUI.Core;
using Code.Core.Input;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.World.GUI;
using UnityEngine;

namespace Code.Core.GUI.Widget.SimpleWindow.Window;

public sealed class MilMo_Dialog : MilMo_Window
{
	private const int CONTENT = 901;

	private const int BUTTON = 902;

	private const float BUTTON_HEIGHT_DIVIDE = 5f;

	public bool ForceKeyboardFocus;

	public bool OKOnEnter;

	private bool _mAllowOK = true;

	public MilMo_Widget Icon { get; private set; }

	public MilMo_TextBlock TextBlock { get; private set; }

	public MilMo_Dialog(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "Dialog " + MilMo_UserInterface.GetRandomID();
		base.FixedRes = true;
		SetText(MilMo_Localization.GetLocString("Generic_88"));
		SetTextOffset(0f, 100f);
		SetAlignment(MilMo_GUI.Align.TopCenter);
		TargetScale = new Vector2(300f, 200f);
		SpawnPos = new Vector2(UI.Center.x - TargetScale.x / 2f, UI.Center.y - TargetScale.y);
		TargetPos = SpawnPos;
		SetEnabled(e: false);
		Open();
	}

	public override void Draw()
	{
		if (!ReadyToDraw || Scale.x < 30f * base.Res.x)
		{
			return;
		}
		if (ForceKeyboardFocus)
		{
			MilMo_UserInterface.KeyboardFocus = true;
		}
		base.Draw();
		if (!OKOnEnter || (!MilMo_Input.GetKeyUp(KeyCode.KeypadEnter, useKeyboardFocus: false) && (!MilMo_Input.GetKeyUp(KeyCode.Return, useKeyboardFocus: false) || MilMo_Input.GetKey(KeyCode.LeftShift, useKeyboardFocus: false) || MilMo_Input.GetKey(KeyCode.RightShift, useKeyboardFocus: false) || MilMo_Input.GetKey(KeyCode.LeftControl, useKeyboardFocus: false) || MilMo_Input.GetKey(KeyCode.RightControl, useKeyboardFocus: false))))
		{
			return;
		}
		MilMo_Button milMo_Button = null;
		foreach (MilMo_Button item in base.Children.Where((MilMo_Widget b) => b.Info == 902))
		{
			milMo_Button = item;
		}
		if (milMo_Button != null && _mAllowOK && milMo_Button.Function != null)
		{
			_mAllowOK = false;
			milMo_Button.Function(this);
		}
	}

	public override void Refresh()
	{
		UI.BypassResolution();
		UI.ResetLayout(14f, 14f, this);
		float num = TargetScale.y / 5f;
		foreach (MilMo_Widget child in base.Children)
		{
			if (child.Info == 901)
			{
				Vector2 vector = Vector2.zero;
				if (child.CustomArg != null)
				{
					vector = (Vector2)child.CustomArg;
				}
				child.SetYPos(TargetScale.y / 2f - num / 2f + vector.y);
				if (Math.Abs(child.Align.x - 0f) < 0.01f)
				{
					child.SetXPos(UI.Padding.x * 2f + vector.x);
				}
				else if (Math.Abs(child.Align.x - 0.5f) < 0.01f)
				{
					child.SetXPos(TargetScale.x / 2f + vector.x);
				}
				else if (Math.Abs(child.Align.x - 1f) < 0.01f)
				{
					child.SetXPos(TargetScale.x - UI.Padding.x * 2f + vector.x);
				}
			}
		}
		UI.ResetLayout(14f, 14f, this);
		float num2 = 0f;
		foreach (MilMo_Widget item in base.Children.Where((MilMo_Widget child) => child.Info == 902))
		{
			_ = item;
			num2 += 1f;
		}
		if (num2 > 0f)
		{
			float x = (TargetScale.x - UI.Padding.x * 2f - (num2 - 1f) * (UI.Padding.x / 2f)) / num2;
			UI.Next.x += UI.Padding.x / 2f;
			foreach (MilMo_Widget child2 in base.Children)
			{
				if (child2.Info == 902)
				{
					float x2 = UI.Next.x - UI.Padding.x / 2f;
					child2.ScaleNow(x, num);
					child2.SetPosition(x2, TargetScale.y - UI.Padding.y);
				}
			}
		}
		UI.RestoreResolution();
	}

	public void AddContent(MilMo_Widget w, float horizAlign)
	{
		w.Info = 901;
		if (Math.Abs(horizAlign - 0f) < 0.01f)
		{
			w.SetAlignment(MilMo_GUI.Align.CenterLeft);
		}
		else if (Math.Abs(horizAlign - 0.5f) < 0.01f)
		{
			w.SetAlignment(MilMo_GUI.Align.CenterCenter);
		}
		else if (Math.Abs(horizAlign - 1f) < 0.01f)
		{
			w.SetAlignment(MilMo_GUI.Align.CenterRight);
		}
		AddChild(w);
		Refresh();
	}

	public void AddContent(MilMo_Widget w, float horizAlign, float x, float y)
	{
		w.CustomArg = new Vector2(x, y);
		AddContent(w, horizAlign);
	}

	public void AddButton(MilMo_LocString text = null, MilMo_Button.ButtonFunc func = null)
	{
		MilMo_Button milMo_Button = new MilMo_Button(UI);
		milMo_Button.SetAlignment(MilMo_GUI.Align.BottomLeft);
		milMo_Button.Info = 902;
		milMo_Button.SetTexture("Batch01/Textures/Core/Invisible");
		milMo_Button.FixedRes = true;
		if (text != null && text.Length > 0)
		{
			milMo_Button.SetText(text);
			milMo_Button.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
			milMo_Button.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
			milMo_Button.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
			milMo_Button.SetFont(MilMo_GUI.Font.EborgSmall);
			if (text.Length > 7)
			{
				string[] array = text.ToString().Split('\n');
				int num = array.Select((string row) => row.Length).Concat(new int[1]).Max();
				if (num > 8)
				{
					milMo_Button.SetFontScale(Mathf.Max(0.5f, 1f - ((float)num / 8f - 1f)));
				}
				else if (array.Length > 1)
				{
					milMo_Button.SetFontScale(0.75f);
				}
			}
			milMo_Button.Function = func;
		}
		else
		{
			milMo_Button.SetText(MilMo_LocString.Empty);
			if (func != null)
			{
				milMo_Button.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
				milMo_Button.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
				milMo_Button.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
				milMo_Button.Function = func;
			}
			else
			{
				milMo_Button.IsInvisible = true;
				milMo_Button.AllowPointerFocus = false;
				milMo_Button.Function = null;
			}
		}
		AddChild(milMo_Button);
		Refresh();
	}

	public void StandardContent(string icon, MilMo_LocString caption, MilMo_LocString text)
	{
		MilMo_Widget milMo_Widget = null;
		if (!string.IsNullOrEmpty(icon))
		{
			milMo_Widget = new MilMo_Widget(UI);
			milMo_Widget.SetTexture(icon);
			milMo_Widget.FixedRes = true;
			milMo_Widget.SetScale(80f, 80f);
			milMo_Widget.SetScalePull(0.06f, 0.06f);
			milMo_Widget.SetScaleDrag(0.5f, 0.5f);
			milMo_Widget.SetFadeSpeed(0.05f);
		}
		StandardContent(milMo_Widget, caption, text);
	}

	private void StandardContent(MilMo_Widget icon, MilMo_LocString caption, MilMo_LocString text)
	{
		float horizAlign = 0.5f;
		float x = 200f;
		SetText(caption);
		if (icon != null)
		{
			horizAlign = 1f;
			x = 150f;
			Icon = icon;
			AddContent(Icon, 0f);
		}
		TextBlock = new MilMo_TextBlock(UI, text, new Vector2(x, 93f), fixedHeight: true);
		TextBlock.TextWidget.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		TextBlock.FixedRes = true;
		AddContent(TextBlock, horizAlign);
	}

	public void DoOK(string icon, MilMo_LocString caption, MilMo_LocString text, MilMo_Button.ButtonFunc okFunc, bool impulse = false)
	{
		StandardContent(icon, caption, text);
		AddButton();
		AddButton(MilMo_Localization.GetLocString("Generic_OK"), okFunc);
		AddButton();
		if (impulse)
		{
			Icon.ScaleImpulse(20f, 20f);
		}
	}

	public void DoOK(MilMo_Widget icon, MilMo_LocString caption, MilMo_LocString text, MilMo_Button.ButtonFunc okFunc)
	{
		StandardContent(icon, caption, text);
		AddButton();
		AddButton(MilMo_Localization.GetLocString("Generic_OK"), okFunc);
		AddButton();
	}

	public static void DoOkCancel(MilMo_UserInterface ui, string icon, MilMo_LocString caption, MilMo_LocString text, MilMo_Button.ButtonFunc okFunction, MilMo_Button.ButtonFunc cancelFunction)
	{
		MilMo_Dialog d = new MilMo_Dialog(ui);
		ui.AddChild(d);
		d.BringToFront();
		d.DoOKCancel(icon, caption, text, delegate(object o)
		{
			d.CloseAndRemove(null);
			okFunction?.Invoke(o);
		}, delegate(object o)
		{
			d.CloseAndRemove(null);
			cancelFunction?.Invoke(o);
		});
	}

	public void DoOKCancel(string icon, MilMo_LocString caption, MilMo_LocString text, MilMo_Button.ButtonFunc okFunc, MilMo_Button.ButtonFunc cancelFunc)
	{
		StandardContent(icon, caption, text);
		AddButton();
		AddButton(MilMo_Localization.GetLocString("Generic_OK"), okFunc);
		AddButton(MilMo_Localization.GetLocString("Generic_Cancel"), cancelFunc);
	}

	public void DoYesNo(string icon, MilMo_LocString caption, MilMo_LocString text, MilMo_Button.ButtonFunc yesFunc, MilMo_Button.ButtonFunc noFunc)
	{
		StandardContent(icon, caption, text);
		AddButton(MilMo_LocString.Empty);
		AddButton(MilMo_Localization.GetLocString("Generic_Yes"), yesFunc);
		AddButton(MilMo_Localization.GetLocString("Generic_No"), noFunc);
	}

	public void DoCustomCustomCancel(string icon, MilMo_LocString caption, MilMo_LocString text, MilMo_LocString button1Text, MilMo_LocString button2Text, MilMo_Button.ButtonFunc button1Func, MilMo_Button.ButtonFunc button2Func, MilMo_Button.ButtonFunc cancelFunc)
	{
		StandardContent(icon, caption, text);
		AddButton(button1Text, button1Func);
		AddButton(button2Text, button2Func);
		AddButton(MilMo_Localization.GetLocString("Generic_Cancel"), cancelFunc);
	}

	public void DoWarning(MilMo_LocString caption, MilMo_LocString text, MilMo_Button.ButtonFunc okFunc)
	{
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
		StandardContent("Batch01/Textures/Dialog/Warning", caption, text);
		Icon.ScaleImpulse(20f, 20f);
		Icon.SetColor(1f, 0f, 0f, 1f);
		AddButton();
		AddButton(MilMo_Localization.GetLocString("Generic_OK"), okFunc);
		AddButton();
	}

	public void CloseAndRemove(object o)
	{
		IsInvisible = true;
		UI.ModalDialog = null;
		Close(null);
		ScaleMover.Arrive = Remove;
		ForceKeyboardFocus = false;
		MilMo_UserInterface.KeyboardFocus = false;
		_mAllowOK = false;
	}

	private void Remove()
	{
		DestroyFlag = true;
		ForceKeyboardFocus = false;
	}
}
