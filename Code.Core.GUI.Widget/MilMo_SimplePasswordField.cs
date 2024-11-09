using Code.Core.GUI.Core;
using Code.Core.Input;
using UnityEngine;

namespace Code.Core.GUI.Widget;

public sealed class MilMo_SimplePasswordField : MilMo_Widget
{
	public string InputText = "";

	private const int M_MAX_LENGTH = 1000;

	public MilMo_SimplePasswordField(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "SimpleTextField " + MilMo_UserInterface.GetRandomID();
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
		if (Identifier == "ChatBarTextField")
		{
			UnityEngine.GUI.SetNextControlName(Identifier);
		}
		InputText = UnityEngine.GUI.PasswordField(screenPosition, InputText, '*', 1000);
		CheckPointerFocus();
		foreach (MilMo_Widget child in base.Children)
		{
			child.Draw();
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

	public void SetInputText(string text)
	{
		InputText = text;
	}
}
