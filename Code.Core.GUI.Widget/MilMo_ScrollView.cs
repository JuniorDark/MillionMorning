using System.Collections.Generic;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget.Button;
using Code.Core.GUI.Widget.SimpleWindow;
using Code.Core.Input;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.GUI.Widget;

public class MilMo_ScrollView : MilMo_Widget
{
	private readonly MilMo_Mover _mSoftScroll = new MilMo_Mover();

	public Rect MViewSize = new Rect(0f, 0f, 1500f, 1500f);

	private bool _mUserControl;

	public Vector2 ScrollBarCompensate = Vector2.zero;

	protected bool MDrawBackground = true;

	public bool MShowHorizBar = true;

	public bool MShowVertBar = true;

	protected float MVertScrollBarAlign = 1f;

	private bool _mDraggableX;

	private bool _mDraggableY;

	private Vector2 _mDragVector = Vector2.zero;

	private readonly Color _mScrollbarColor = Color.white;

	private bool _mouseWheelScrollable = true;

	public MilMo_Mover SoftScroll => _mSoftScroll;

	public bool HorizontalDrag
	{
		get
		{
			return _mDraggableX;
		}
		set
		{
			_mDraggableX = value;
		}
	}

	public bool VerticalDrag
	{
		get
		{
			return _mDraggableY;
		}
		set
		{
			_mDraggableY = value;
		}
	}

	public bool MouseWheelScrollable
	{
		get
		{
			return _mouseWheelScrollable;
		}
		set
		{
			_mouseWheelScrollable = value;
		}
	}

	public MilMo_ScrollView(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "ScrollView " + MilMo_UserInterface.GetRandomID();
		SetScale(256f, 256f);
		SetScrollPull(0.05f, 0.05f);
		SetScrollDrag(0.55f, 0.55f);
		_mUserControl = true;
	}

	public override void Step()
	{
		if (!IsEnabled())
		{
			return;
		}
		if (_mUserControl && MilMo_Pointer.LeftButton)
		{
			SoftScroll.SetUpdateFunc(MilMo_Mover.UpdateFunc.Spring);
		}
		if (Hover() && (_mDraggableX || _mDraggableY) && MilMo_Pointer.LeftButton)
		{
			if (_mDragVector != Vector2.zero && _mDragVector != MilMo_Pointer.Position)
			{
				Vector2 vector = _mDragVector - MilMo_Pointer.Position;
				if (_mDraggableX)
				{
					SoftScroll.Target.x += vector.x;
					if (SoftScroll.Target.x < 0f)
					{
						SoftScroll.Target.x = 0f;
					}
					if (SoftScroll.Target.x > MViewSize.width - Scale.x)
					{
						SoftScroll.Target.x = MViewSize.width - Scale.x;
					}
				}
				if (_mDraggableY)
				{
					SoftScroll.Target.y += vector.y;
					if (SoftScroll.Target.y < 0f)
					{
						SoftScroll.Target.y = 0f;
					}
					if (SoftScroll.Target.y > MViewSize.height - Scale.x)
					{
						SoftScroll.Target.y = MViewSize.height - Scale.x;
					}
				}
			}
			_mDragVector = MilMo_Pointer.Position;
		}
		else
		{
			_mDragVector = Vector2.zero;
		}
		MouseScroll();
		SoftScroll.Update();
		base.Step();
	}

	public override void Draw()
	{
		if (!IsEnabled())
		{
			return;
		}
		Rect screenPosition = GetScreenPosition();
		Rect alignedScreenPosition = GetAlignedScreenPosition();
		Vector2 pivotPoint = default(Vector2);
		pivotPoint.x = alignedScreenPosition.x;
		pivotPoint.y = alignedScreenPosition.y;
		GUIUtility.RotateAroundPivot(0f, pivotPoint);
		Rect position = screenPosition;
		Rect position2 = screenPosition;
		Rect position3 = screenPosition;
		if (MVertScrollBarAlign == 0f)
		{
			position3.x += 16f;
		}
		if (Parent != null && UseParentAlpha)
		{
			CurrentColor.a = Parent.CurrentColor.a;
		}
		UnityEngine.GUI.color = _mScrollbarColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
		if (_mUserControl)
		{
			if (HasVertScrollBar())
			{
				if (MVertScrollBarAlign == 1f)
				{
					position.x += position.width - 16f;
				}
				position.width = 16f;
				if (HasHorizScrollBar())
				{
					position.height -= 16f;
				}
				SoftScroll.Target.y = UnityEngine.GUI.VerticalScrollbar(position, SoftScroll.Target.y, Scale.y, 0f, MViewSize.height);
				position3.width -= 16f;
			}
			if (HasHorizScrollBar())
			{
				position2.y += position2.height - 16f;
				if (HasVertScrollBar())
				{
					position2.width -= 16f;
				}
				SoftScroll.Target.x = UnityEngine.GUI.HorizontalScrollbar(position2, SoftScroll.Target.x, Scale.x, 0f, MViewSize.width);
				position3.height -= 16f;
			}
		}
		UnityEngine.GUI.color = CurrentColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
		if (MDrawBackground && base.Texture != null && base.Texture.Texture != null)
		{
			UnityEngine.GUI.DrawTexture(position3, base.Texture.Texture, ScaleMode.StretchToFill, alphaBlend: true, 0f);
		}
		Color color = UnityEngine.GUI.color;
		color.a = 0f;
		UnityEngine.GUI.color = color;
		CheckPointerFocus();
		if (!MShowVertBar)
		{
			screenPosition.width += ScrollBarCompensate.x;
		}
		if (!MShowHorizBar)
		{
			screenPosition.height += ScrollBarCompensate.y;
		}
		if (MVertScrollBarAlign == 0f)
		{
			screenPosition.x += 16f;
		}
		UnityEngine.GUI.BeginScrollView(screenPosition, SoftScroll.Val, MViewSize);
		Vector2 globalPosOffset = UI.GlobalPosOffset;
		UI.GlobalPosOffset = Vector2.zero;
		IList<MilMo_Widget> children = base.Children;
		int count = children.Count;
		for (int i = 0; i < count; i++)
		{
			MilMo_Widget milMo_Widget = children[i];
			DisableOutsideView(milMo_Widget);
			milMo_Widget.Draw();
		}
		UI.GlobalPosOffset = globalPosOffset;
		UnityEngine.GUI.EndScrollView();
		MilMo_Widget.RestoreMatrix();
	}

	public bool ContainsMouse()
	{
		return ContainsPoint(MilMo_Pointer.Position);
	}

	private bool ContainsPoint(Vector2 screenCoord)
	{
		Rect screenPosition = GetScreenPosition();
		Rect ancestorPointerOffset = GetAncestorPointerOffset();
		if (GetWindowAncestor() != null)
		{
			screenPosition.x += ancestorPointerOffset.x;
			screenPosition.y += ancestorPointerOffset.y;
			screenPosition.width += ancestorPointerOffset.width;
			screenPosition.height += ancestorPointerOffset.height;
		}
		screenPosition.x += UI.GlobalInputOffset.x;
		screenPosition.y += UI.GlobalInputOffset.y;
		if (!screenPosition.Contains(screenCoord))
		{
			return false;
		}
		return true;
	}

	public bool InView(MilMo_Widget w)
	{
		Vector2 pos = w.Pos;
		for (MilMo_Widget parent = w.Parent; parent != this; parent = parent.Parent)
		{
			if (parent.Parent == null)
			{
				Debug.Log("Checking MilMo_ScrollView.InView on a widget that is not a child of a scroll view.");
				return false;
			}
			pos += parent.Pos;
		}
		if (pos.x - w.ScaleMover.Target.x * (1f - w.Align.x) > GetScrollPosition().x + ScaleMover.Target.x || pos.x + w.ScaleMover.Target.x * (1f - w.Align.x) < GetScrollPosition().x)
		{
			return false;
		}
		if (pos.y - w.ScaleMover.Target.y * (1f - w.Align.y) > GetScrollPosition().y + ScaleMover.Target.y || pos.y + w.ScaleMover.Target.y * (1f - w.Align.y) < GetScrollPosition().y)
		{
			return false;
		}
		return true;
	}

	protected virtual void DisableOutsideView(MilMo_Widget w)
	{
		if (w is MilMo_Tab || w.Identifier == "LevelInfoPopup")
		{
			return;
		}
		MilMo_Window windowAncestor = GetWindowAncestor();
		if (windowAncestor == null || !(windowAncestor.Identifier == "Messenger"))
		{
			w.SetEnabled(e: true);
			float num = 0f;
			if (w.PosMover.Target.x - w.ScaleMover.Target.x * (1f - w.Align.x) + num > GetScrollPosition().x + ScaleMover.Target.x || w.PosMover.Target.x + w.ScaleMover.Target.x * (1f - w.Align.x) - num < GetScrollPosition().x)
			{
				w.SetEnabled(e: false);
			}
			if (w.PosMover.Target.y - w.ScaleMover.Target.y * (1f - w.Align.y) + num > GetScrollPosition().y + ScaleMover.Target.y || w.PosMover.Target.y + w.ScaleMover.Target.y * (1f - w.Align.y) - num < GetScrollPosition().y)
			{
				w.SetEnabled(e: false);
			}
		}
	}

	protected override Rect GetChildOffset()
	{
		return new Rect(0f, 0f, 0f, 0f);
	}

	public void SetViewSize(Vector2 size)
	{
		SetViewSize(size.x, size.y);
	}

	public void SetViewSize(float x, float y)
	{
		x *= UI.Res.x;
		y *= UI.Res.y;
		MViewSize.width = x;
		MViewSize.height = y;
	}

	public void SetScrollPull(float x, float y)
	{
		SoftScroll.Pull.x = x;
		SoftScroll.Pull.y = y;
	}

	public void SetScrollDrag(float x, float y)
	{
		SoftScroll.Drag.x = x;
		SoftScroll.Drag.y = y;
	}

	public void HasBackground(bool b)
	{
		MDrawBackground = b;
	}

	public void IsUserControlled(bool b)
	{
		_mUserControl = b;
	}

	public void ScrollTo(float x, float y)
	{
		x *= base.Res.x;
		y *= base.Res.y;
		if (x < 0f)
		{
			x = 0f;
		}
		if (y < 0f)
		{
			y = 0f;
		}
		SoftScroll.Target.x = x;
		SoftScroll.Target.y = y;
		SoftScroll.SetUpdateFunc(MilMo_Mover.UpdateFunc.Spring);
	}

	public void ScrollToNow(float x, float y)
	{
		x *= base.Res.x;
		y *= base.Res.y;
		if (x < 0f)
		{
			x = 0f;
		}
		if (y < 0f)
		{
			y = 0f;
		}
		SoftScroll.Target.x = x;
		SoftScroll.Target.y = y;
		SoftScroll.Val.x = x;
		SoftScroll.Val.y = y;
		SoftScroll.Vel.x = 0f;
		SoftScroll.Vel.y = 0f;
		SoftScroll.SetUpdateFunc(MilMo_Mover.UpdateFunc.Nothing);
	}

	public Vector2 GetScrollPosition()
	{
		return new Vector2(SoftScroll.Val.x, SoftScroll.Val.y);
	}

	public Vector2 GetScrollTarget()
	{
		return new Vector2(SoftScroll.Target.x, SoftScroll.Target.y);
	}

	public void RefreshViewSize()
	{
		MViewSize.width = 0f;
		MViewSize.height = 0f;
		foreach (MilMo_Widget child in base.Children)
		{
			Vector2 zero = Vector2.zero;
			float num = 19f;
			zero.x = child.PosMover.Target.x + child.ScaleMover.Target.x * (1f - child.Align.x);
			if (zero.x > MViewSize.width && !child.IgnoredByScrollViewRefresh)
			{
				MViewSize.width = zero.x - num;
			}
			zero.y = child.PosMover.Target.y + child.ScaleMover.Target.y * (1f - child.Align.y);
			if (zero.y > MViewSize.height && !child.IgnoredByScrollViewRefresh)
			{
				MViewSize.height = zero.y;
			}
		}
	}

	public void RefreshViewSize(float x, float y)
	{
		RefreshViewSize();
		MViewSize.width += x;
		MViewSize.height += y;
	}

	public void ShowHorizontalBar(bool h)
	{
		MShowHorizBar = h;
	}

	public void ShowVerticalBar(bool v)
	{
		MShowVertBar = v;
	}

	private void MouseScroll()
	{
		if (MouseWheelScrollable && MouseInsideScroller(this) && ((double)MilMo_Pointer.ScrollDelta >= 0.1 || (double)MilMo_Pointer.ScrollDelta <= -0.1))
		{
			if (HasVertScrollBar())
			{
				SoftScroll.SetUpdateFunc(MilMo_Mover.UpdateFunc.Spring);
				SoftScroll.Target.y -= MilMo_Pointer.ScrollDelta * 100f;
			}
			else if (HasHorizScrollBar())
			{
				SoftScroll.SetUpdateFunc(MilMo_Mover.UpdateFunc.Spring);
				SoftScroll.Target.x -= MilMo_Pointer.ScrollDelta * 100f;
			}
		}
	}

	private bool HasHorizScrollBar()
	{
		if (MShowHorizBar)
		{
			return MViewSize.width > Scale.x;
		}
		return false;
	}

	private bool HasVertScrollBar()
	{
		if (MShowVertBar)
		{
			return MViewSize.height > Scale.y;
		}
		return false;
	}
}
