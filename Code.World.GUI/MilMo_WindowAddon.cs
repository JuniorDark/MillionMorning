using System.Collections.Generic;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget.SimpleWindow;
using UnityEngine;

namespace Code.World.GUI;

public sealed class MilMo_WindowAddon : MilMo_Widget
{
	private readonly int _windowId;

	public Vector2 WindowOffset;

	private readonly MilMo_Widget _picture;

	public MilMo_Window ParentWindow;

	private readonly List<MilMo_Widget> _backdropList = new List<MilMo_Widget>();

	private bool _bringToFront;

	public MilMo_WindowAddon(MilMo_UserInterface ui)
		: base(ui)
	{
		base.FixedRes = true;
		_windowId = MilMo_UserInterface.NextWindowId();
		Identifier = "WindowAddon " + _windowId;
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
		SetAlignment(MilMo_GUI.Align.TopLeft);
		AllowPointerFocus = true;
		DragTarget = DragType.Self;
		_picture = new MilMo_Widget(UI);
		_picture.SetAlignment(MilMo_GUI.Align.TopLeft);
		_picture.SetPosition(0f, 0f);
	}

	private void WindowFunc(int i)
	{
		_picture.Scale = Scale;
		_picture.Draw();
	}

	public override void Draw()
	{
		if (!Enabled)
		{
			return;
		}
		UnityEngine.GUI.color = new Color(0f, 0f, 0f, 0f);
		if (PosMover.Target.x < 0f)
		{
			PosMover.Target.x = 0f;
		}
		if (PosMover.Target.x + ScaleMover.Target.x > (float)Screen.width)
		{
			PosMover.Target.x = (float)Screen.width - ScaleMover.Target.x;
		}
		if (PosMover.Target.y < 0f)
		{
			PosMover.Target.y = 0f;
		}
		if (PosMover.Target.y + ScaleMover.Target.y > (float)Screen.height)
		{
			PosMover.Target.y = (float)Screen.height - ScaleMover.Target.y;
		}
		Rect clientRect = new Rect(PosMover.Target.x + UI.GlobalPosOffset.x, PosMover.Target.y + UI.GlobalPosOffset.y, ScaleMover.Target.x, ScaleMover.Target.y);
		clientRect.x -= UI.GlobalPosOffset.x;
		clientRect.y -= UI.GlobalPosOffset.y;
		clientRect.width += UI.GlobalPosOffset.x;
		clientRect.height += UI.GlobalPosOffset.y;
		if (!IsInvisible)
		{
			clientRect = UnityEngine.GUI.Window(_windowId, clientRect, WindowFunc, "");
		}
		clientRect.x += UI.GlobalPosOffset.x;
		clientRect.y += UI.GlobalPosOffset.y;
		clientRect.width -= UI.GlobalPosOffset.x;
		clientRect.height -= UI.GlobalPosOffset.y;
		if (_bringToFront)
		{
			UnityEngine.GUI.BringWindowToFront(_windowId);
			_bringToFront = false;
		}
		CheckPointerFocus();
		GoToNow(clientRect.x - UI.GlobalPosOffset.x, clientRect.y - UI.GlobalPosOffset.y);
		ScaleTo(clientRect.width, clientRect.height);
		foreach (MilMo_Widget backdrop in _backdropList)
		{
			backdrop.SetPosition(Pos.x, Pos.y);
			if (backdrop.Pos.x < 0f)
			{
				backdrop.SetXPos(0f);
			}
			if (backdrop.Pos.x > (float)Screen.width - backdrop.Scale.x)
			{
				backdrop.SetXPos((float)Screen.width - backdrop.Scale.x);
			}
			if (backdrop.Pos.y < 0f)
			{
				backdrop.SetYPos(0f);
			}
			if (backdrop.Pos.y > (float)(Screen.height - 428))
			{
				backdrop.SetYPos(Screen.height - 428);
			}
		}
	}

	public override void SetTexture(string filename)
	{
		_picture.SetTexture(filename);
	}

	protected override Rect GetChildOffset()
	{
		return new Rect(0f, 0f, 0f, 0f);
	}

	public void AddBackdrop(MilMo_Widget w)
	{
		w.AllowPointerFocus = false;
		_backdropList.Add(w);
		UI.AddChild(w);
	}

	public override void SetEnabled(bool e)
	{
		foreach (MilMo_Widget backdrop in _backdropList)
		{
			backdrop.SetEnabled(e);
		}
		base.SetEnabled(e);
	}

	public void Destroy()
	{
		foreach (MilMo_Widget backdrop in _backdropList)
		{
			UI.RemoveChild(backdrop);
		}
		_backdropList.Clear();
		UI.RemoveChild(this);
	}

	public void BringToFront()
	{
		_bringToFront = true;
	}
}
