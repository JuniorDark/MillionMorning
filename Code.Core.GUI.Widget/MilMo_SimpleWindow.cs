using System.Linq;
using Code.Core.GUI.Core;
using UnityEngine;

namespace Code.Core.GUI.Widget;

public class MilMo_SimpleWindow : MilMo_Widget
{
	public readonly MilMo_Button BackDrop;

	protected readonly MilMo_SimpleLabel MCaption;

	protected bool MAllowOffscreen;

	public int WindowId { get; private set; }

	public bool Draggable
	{
		set
		{
			BackDrop.DragTarget = (value ? DragType.Ancestor : DragType.None);
		}
	}

	public override bool IgnoreGlobalFade
	{
		protected get
		{
			return base.IgnoreGlobalFade;
		}
		set
		{
			base.IgnoreGlobalFade = value;
			if (BackDrop != null)
			{
				BackDrop.IgnoreGlobalFade = value;
			}
			if (MCaption != null)
			{
				MCaption.IgnoreGlobalFade = value;
			}
		}
	}

	public MilMo_SimpleWindow(MilMo_UserInterface ui)
		: base(ui)
	{
		base.FixedRes = true;
		WindowId = MilMo_UserInterface.NextWindowId();
		Identifier = "SimpleWindow " + WindowId;
		SetSkin(0);
		IsWindow = true;
		ScaleMover.Val.x = 160f * base.Res.x;
		ScaleMover.Val.y = 96f * base.Res.y;
		ScaleMover.Target.x = 200f * base.Res.x;
		ScaleMover.Target.y = 120f * base.Res.y;
		PosMover.Target.x = PosMover.Val.x;
		PosMover.Target.y = PosMover.Val.y;
		PosMover.Val.x = PosMover.Target.x;
		PosMover.Val.y = PosMover.Target.y;
		ScaleMover.SetUpdateFunc(ScaleType);
		SetScalePull(0.06f, 0.06f);
		SetScaleDrag(0.5f, 0.5f);
		SetFont(MilMo_GUI.Font.ArialRounded);
		SetAlignment(MilMo_GUI.Align.CenterCenter);
		BackDrop = new MilMo_Button(UI);
		BackDrop.SetPosition(0f, 0f);
		BackDrop.IsInvisible = true;
		BackDrop.SetAllTextures("Batch01/Textures/Core/Invisible");
		BackDrop.SetAlignment(MilMo_GUI.Align.TopLeft);
		BackDrop.Identifier = "Backdrop";
		BackDrop.DragTarget = DragType.Ancestor;
		AddChild(BackDrop);
		MCaption = new MilMo_SimpleLabel(UI);
		MCaption.SetAlignment(MilMo_GUI.Align.TopCenter);
		MCaption.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		MCaption.AllowPointerFocus = false;
		MCaption.SetFont(MilMo_GUI.Font.EborgMedium);
		AddChild(MCaption);
		AllowPointerFocus = false;
	}

	public override void Draw()
	{
		if (!Enabled)
		{
			return;
		}
		MCaption.SetText(base.Text);
		MCaption.ScaleNow(Scale.x / base.Res.x, 40f / base.Res.y);
		MCaption.GoToNow(Scale.x / 2f / base.Res.x, 0f / base.Res.y);
		Color currentColor = CurrentColor;
		if (Parent != null && UseParentAlpha)
		{
			currentColor.a *= Parent.CurrentColor.a;
		}
		UnityEngine.GUI.color = currentColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
		UnityEngine.GUI.skin = UI.Font0;
		if (!MAllowOffscreen)
		{
			if (PosMover.Target.x + UI.GlobalPosOffset.x < 0f)
			{
				PosMover.Target.x = 0f - UI.GlobalPosOffset.x;
			}
			if (PosMover.Target.y + UI.GlobalPosOffset.y < 0f)
			{
				PosMover.Target.y = 0f - UI.GlobalPosOffset.y;
			}
			if (PosMover.Target.x + UI.GlobalPosOffset.x > (float)Screen.width - ScaleMover.Target.x)
			{
				PosMover.Target.x = (float)Screen.width - ScaleMover.Target.x - UI.GlobalPosOffset.x;
			}
			if (PosMover.Target.y + UI.GlobalPosOffset.y > (float)Screen.height - ScaleMover.Target.y)
			{
				PosMover.Target.y = (float)Screen.height - ScaleMover.Target.y - UI.GlobalPosOffset.y;
			}
		}
		Rect clientRect = new Rect(PosMover.Target.x + UI.GlobalPosOffset.x, PosMover.Target.y + UI.GlobalPosOffset.y, ScaleMover.Target.x, ScaleMover.Target.y);
		BackDrop.Scale = ScaleMover.Target;
		UnityEngine.GUI.skin = Skin;
		if (!IsInvisible)
		{
			clientRect = UnityEngine.GUI.Window(WindowId, clientRect, WindowFunc, "");
		}
		CheckPointerFocus();
		GoToNow(clientRect.x - UI.GlobalPosOffset.x, clientRect.y - UI.GlobalPosOffset.y);
		ScaleTo(clientRect.width, clientRect.height);
	}

	protected void WindowFunc(int i)
	{
		Vector2 globalPosOffset = UI.GlobalPosOffset;
		UI.GlobalPosOffset = Vector2.zero;
		foreach (MilMo_Widget item in base.Children.Where((MilMo_Widget w) => w != null))
		{
			item.Draw();
		}
		UI.GlobalPosOffset = globalPosOffset;
	}

	protected override Rect GetChildOffset()
	{
		return new Rect(0f, 0f, 0f, 0f);
	}
}
