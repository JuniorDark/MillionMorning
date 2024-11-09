using Code.Core.GUI.Core;
using Code.Core.Input;
using UnityEngine;

namespace Code.Core.GUI.Widget;

public sealed class MilMo_SimpleTextField : MilMo_Widget
{
	public string InputText = "";

	private int _maxLength = 1000;

	public bool IgnoreHover;

	public MilMo_SimpleTextField(MilMo_UserInterface ui)
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
		Font = UI.Font0;
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
		UnityEngine.GUI.skin = Font;
		Rect screenPosition = GetScreenPosition();
		if (Identifier == "ChatBarTextField")
		{
			UnityEngine.GUI.SetNextControlName(Identifier);
		}
		if (!IgnoreHover)
		{
			GUIStyle style = new GUIStyle(UnityEngine.GUI.skin.textField)
			{
				alignment = TextAnchor.MiddleLeft
			};
			InputText = UnityEngine.GUI.TextField(screenPosition, InputText, _maxLength, style);
		}
		else
		{
			UnityEngine.GUI.Box(screenPosition, InputText);
		}
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

	public void SetMaxLength(int len)
	{
		if (len > 0)
		{
			_maxLength = len;
		}
	}
}
