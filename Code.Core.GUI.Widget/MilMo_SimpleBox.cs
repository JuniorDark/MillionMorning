using Code.Core.GUI.Core;
using UnityEngine;

namespace Code.Core.GUI.Widget;

public class MilMo_SimpleBox : MilMo_Widget
{
	private readonly int _mWindowId;

	private bool _bringToFront;

	public MilMo_SimpleBox(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "SimpleBox " + MilMo_UserInterface.GetRandomID();
		_mWindowId = MilMo_UserInterface.NextWindowId();
		ScaleMover.Target.x = 200f;
		ScaleMover.Target.y = 150f;
		ScaleMover.Val.x = ScaleMover.Target.x;
		ScaleMover.Val.y = ScaleMover.Target.y;
		PosMover.Target.x = PosMover.Val.x;
		PosMover.Target.y = PosMover.Val.y;
		PosMover.Val.x = PosMover.Target.x;
		PosMover.Val.y = PosMover.Target.y;
		SetFont(MilMo_GUI.Font.ArialRounded);
		SetAlignment(MilMo_GUI.Align.CenterCenter);
		SetTextAlignment(MilMo_GUI.Align.TopCenter);
	}

	public void BringToFront()
	{
		_bringToFront = true;
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
		UnityEngine.GUI.skin = Skin;
		UnityEngine.GUI.Box(GetScreenPosition(), "");
		DrawText();
		CheckPointerFocus();
		foreach (MilMo_Widget child in base.Children)
		{
			child.Draw();
		}
		if (_bringToFront)
		{
			UnityEngine.GUI.BringWindowToFront(_mWindowId);
			_bringToFront = false;
		}
	}
}
