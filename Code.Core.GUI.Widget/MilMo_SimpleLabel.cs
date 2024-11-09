using Code.Core.GUI.Core;
using UnityEngine;

namespace Code.Core.GUI.Widget;

public sealed class MilMo_SimpleLabel : MilMo_Widget
{
	public MilMo_SimpleLabel(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "SimpleLabel " + MilMo_UserInterface.GetRandomID();
		ScaleMover.Target.x = 150f;
		ScaleMover.Target.y = 20f;
		ScaleMover.Val.x = ScaleMover.Target.x;
		ScaleMover.Val.y = ScaleMover.Target.y;
		PosMover.Target.x = PosMover.Val.x;
		PosMover.Target.y = PosMover.Val.y;
		PosMover.Val.x = PosMover.Target.x;
		PosMover.Val.y = PosMover.Target.y;
		SetScalePull(0.06f, 0.06f);
		SetScaleDrag(0.5f, 0.5f);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetFont(MilMo_GUI.Font.ArialRounded);
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
			UnityEngine.GUI.skin = Font;
			UnityEngine.GUI.Label(GetScreenPosition(), "");
			DrawText();
		}
	}
}
