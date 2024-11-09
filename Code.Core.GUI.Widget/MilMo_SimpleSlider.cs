using Code.Core.GUI.Core;
using UnityEngine;

namespace Code.Core.GUI.Widget;

public sealed class MilMo_SimpleSlider : MilMo_Widget
{
	private float _mVal = 0.5f;

	public float Min;

	public float Max = 1f;

	public float Val
	{
		get
		{
			return _mVal;
		}
		set
		{
			if (value < Min || value > Max)
			{
				Debug.LogWarning("Attempted to set MilMo_SimpleSlider out of range");
			}
			else
			{
				_mVal = value;
			}
		}
	}

	public MilMo_SimpleSlider(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "SimpleSlider " + MilMo_UserInterface.GetRandomID();
		ScaleMover.Target.x = 100f;
		ScaleMover.Target.y = 30f;
		ScaleMover.Val.x = ScaleMover.Target.x;
		ScaleMover.Val.y = ScaleMover.Target.y;
		PosMover.Target.x = PosMover.Val.x;
		PosMover.Target.y = PosMover.Val.y;
		PosMover.Val.x = PosMover.Target.x;
		PosMover.Val.y = PosMover.Target.y;
		SetFont(MilMo_GUI.Font.ArialRounded);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetTextAlignment(MilMo_GUI.Align.TopCenter);
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
		_mVal = UnityEngine.GUI.HorizontalSlider(screenPosition, _mVal, Min, Max);
		DrawText();
		CheckPointerFocus();
		foreach (MilMo_Widget child in base.Children)
		{
			child.Draw();
		}
	}
}
