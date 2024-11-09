using Code.Core.GUI.Core;
using Code.Core.GUI.Widget.SimpleWindow;
using Code.Core.Input;
using UnityEngine;

namespace Code.Core.GUI.Widget;

public sealed class MilMo_SimpleTextArea : MilMo_Widget
{
	private int _mMaxLength = 1000;

	public string InputText = "";

	public bool FocusOnNextDraw;

	public MilMo_SimpleTextArea(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "SimpleTextArea" + MilMo_UserInterface.GetRandomID();
		PosMover.Val.x = 0f;
		PosMover.Val.y = 0f;
		ScaleMover.Val.x = 100f * base.Res.x;
		ScaleMover.Val.y = 20f * base.Res.y;
		PosMover.Target.x = PosMover.Val.x;
		PosMover.Target.y = PosMover.Val.y;
		ScaleMover.Target.x = ScaleMover.Val.x;
		ScaleMover.Target.y = ScaleMover.Val.y;
		SetAlignment(MilMo_GUI.Align.TopLeft);
	}

	public override void Draw()
	{
		if (!Enabled)
		{
			return;
		}
		Color currentColor = CurrentColor;
		if (Parent != null && UseParentAlpha)
		{
			currentColor.a *= Parent.CurrentColor.a;
		}
		UnityEngine.GUI.color = currentColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
		UnityEngine.GUI.skin = UI.Font0;
		Rect screenPosition = GetScreenPosition();
		UnityEngine.GUI.SetNextControlName(Identifier);
		UnityEngine.GUI.skin.textArea.fontSize = 12;
		InputText = UnityEngine.GUI.TextArea(screenPosition, InputText, _mMaxLength);
		if (FocusOnNextDraw)
		{
			MilMo_Window windowAncestor = GetWindowAncestor();
			if (windowAncestor != null)
			{
				UnityEngine.GUI.FocusWindow(windowAncestor.WindowId);
			}
			UnityEngine.GUI.FocusControl(Identifier);
			MilMo_UserInterface.KeyboardFocus = true;
			FocusOnNextDraw = false;
		}
		CheckPointerFocus();
		for (int i = 0; i < base.Children.Count; i++)
		{
			base.Children[i].Draw();
		}
	}

	public override void Step()
	{
		if (IsEnabled())
		{
			if ((MilMo_Pointer.LeftButton || MilMo_Pointer.LeftClick) && Hover())
			{
				MilMo_UserInterface.KeyboardFocus = true;
			}
			base.Step();
		}
	}

	public void SetMaxLength(int len)
	{
		if (len > 0)
		{
			_mMaxLength = len;
		}
	}
}
