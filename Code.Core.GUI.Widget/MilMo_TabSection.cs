using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.World.GUI;
using UnityEngine;

namespace Code.Core.GUI.Widget;

public sealed class MilMo_TabSection : MilMo_Widget
{
	private struct IndexedWidget
	{
		public readonly MilMo_Widget Widget;

		public readonly int Index;

		public IndexedWidget(MilMo_Widget widget, int index)
		{
			Widget = widget;
			Index = index;
		}
	}

	private const float M_WIDTH = 200f;

	private float _mHeight = 20f;

	private readonly float _mCaptionHeight = 30f;

	private const float M_FOOTER_HEIGHT = 15f;

	private const float M_FADE_Y_OFFSET = -4f;

	private const float M_MINIMUM_HEIGHT_OF_SECTION_BOTTOM_FADE_TEXTURE = 120f;

	public readonly MilMo_Widget MTopFade;

	public readonly MilMo_Widget MBottomFade;

	private Vector2 _mNextPos = Vector2.zero;

	private Vector2 _mSamePos = Vector2.zero;

	private readonly List<IndexedWidget> _mIndexedWidgets = new List<IndexedWidget>();

	public MilMo_TabSection(MilMo_UserInterface ui, MilMo_LocString name, string identifier)
		: base(ui)
	{
		Identifier = identifier;
		GoToNow(0f, 0f);
		ScaleNow(200f, 20f);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		if (name == null || string.IsNullOrEmpty(name.String))
		{
			_mCaptionHeight = 5f;
		}
		SetText(name);
		SetFontScale(1f);
		SetDefaultTextColor(1f, 1f, 1f, 1f);
		SetFont(MilMo_GUI.Font.EborgSmall);
		SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		SetTextAlignment(MilMo_GUI.Align.TopCenter);
		MTopFade = new MilMo_Widget(UI);
		MTopFade.SetTexture("Batch01/Textures/Bag/TabSectionFadeTop");
		MTopFade.SetAlignment(MilMo_GUI.Align.TopLeft);
		MTopFade.SetPosition(0f, -4f);
		MTopFade.SetScale(254f, 67f);
		MTopFade.AllowPointerFocus = false;
		MTopFade.Identifier = "Fade";
		AddChild(MTopFade);
		MBottomFade = new MilMo_Widget(UI);
		MBottomFade.SetTexture("Batch01/Textures/Bag/TabSectionFadeBottom");
		MBottomFade.SetAlignment(MilMo_GUI.Align.BottomLeft);
		MBottomFade.SetPosition(0f, ScaleMover.Target.y / UI.Res.y);
		MBottomFade.SetScale(254f, 132f);
		MBottomFade.AllowPointerFocus = false;
		MBottomFade.Identifier = "Fade";
		AddChild(MBottomFade);
		SetFadeSpeed(0.075f);
		RefreshContents();
	}

	public override void Draw()
	{
		base.Draw();
		foreach (MilMo_Widget child in base.Children)
		{
			child.SetAlpha(CurrentColor.a);
			child.SetTextColor(1f, 1f, 1f, CurrentColor.a);
		}
	}

	public void AddChild(MilMo_Widget w, int index)
	{
		AddChild(w);
		if (index >= 0)
		{
			int num = -1;
			for (int i = 0; i < _mIndexedWidgets.Count; i++)
			{
				if (_mIndexedWidgets[i].Index > index)
				{
					num = i;
					break;
				}
				if (_mIndexedWidgets[i].Index == index)
				{
					RemoveChild(w);
					Debug.LogWarning("Trying to add several widgets at index " + index + " in tab section " + base.Text);
					return;
				}
			}
			if (num >= 0)
			{
				int index2 = base.Children.IndexOf(_mIndexedWidgets[num].Widget);
				base.Children.RemoveAt(base.Children.Count - 1);
				base.Children.Insert(index2, w);
				_mIndexedWidgets.Insert(num, new IndexedWidget(w, index));
			}
			else
			{
				_mIndexedWidgets.Add(new IndexedWidget(w, index));
			}
		}
		RefreshContents();
	}

	public int GetNumberOfChildrenByType(Type childType)
	{
		return base.Children.Count((MilMo_Widget widget) => widget != null && widget.GetType() == childType);
	}

	public override void RemoveAllChildrenByType(Type childType)
	{
		for (int num = _mIndexedWidgets.Count - 1; num >= 0; num--)
		{
			if (_mIndexedWidgets[num].Widget != null && _mIndexedWidgets[num].Widget.GetType() == childType)
			{
				_mIndexedWidgets.RemoveAt(num);
			}
		}
		base.RemoveAllChildrenByType(childType);
	}

	public void RefreshContents()
	{
		UI.ResetLayout(2f, 5f);
		_mNextPos = new Vector2(UI.Next.x, _mCaptionHeight);
		_mSamePos = _mNextPos;
		foreach (MilMo_Widget child in base.Children)
		{
			if (!(child.Identifier == "Fade"))
			{
				float num = _mNextPos.x;
				float y = _mSamePos.y;
				if (num * base.Res.x + child.Scale.x > ScaleMover.Target.x + 15f)
				{
					num = UI.Align.Left;
					y = _mNextPos.y - UI.Padding.y;
				}
				child.SetPosition(num, y);
				_mNextPos = UI.Next;
				_mSamePos = UI.Same;
			}
		}
		RefreshHeight();
	}

	private void RefreshHeight()
	{
		_mHeight = 0f;
		foreach (MilMo_Widget child in base.Children)
		{
			if (!(child.Identifier == "Fade"))
			{
				float num = child.PosMover.Target.y / UI.Res.y + child.ScaleMover.Target.y / UI.Res.y * (1f - child.Align.y);
				if (num > _mHeight)
				{
					_mHeight = num;
				}
			}
		}
		_mHeight += 15f;
		_mHeight = Mathf.Max(70f, _mHeight);
		SetYScale(_mHeight);
		if (GetNumberOfChildrenByType(typeof(MilMo_ItemButton)) > 0 && _mHeight < 120f)
		{
			MBottomFade.SetYScale(120f - (120f - _mHeight));
		}
		else
		{
			MBottomFade.SetYScale(132f);
		}
		if (MBottomFade != null)
		{
			MBottomFade.SetYPos(ScaleMover.Target.y / UI.Res.y + -4f);
		}
		if (MTopFade != null)
		{
			MTopFade.SetPosition(0f, -4f);
		}
	}
}
