using System.Collections.Generic;
using Code.Core.GUI.Core;
using Code.Core.Input;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.GUI.Widget;

public sealed class MilMo_ComboBox : MilMo_Widget
{
	private sealed class ComboBoxButton : MilMo_Button
	{
		public ComboBoxButton(MilMo_UserInterface ui)
			: base(ui)
		{
		}

		public override bool Hover()
		{
			Rect ancestorPointerOffset = GetAncestorPointerOffset();
			ancestorPointerOffset.x += Pos.x;
			ancestorPointerOffset.y += Pos.y;
			ancestorPointerOffset.width = Scale.x;
			ancestorPointerOffset.height = Scale.y;
			if (!ancestorPointerOffset.Contains(MilMo_Pointer.Position))
			{
				return false;
			}
			MilMo_UserInterface.PointerFocus = this;
			return true;
		}
	}

	public delegate void IndexChanged(int newIndex);

	public enum ComboDropDirection
	{
		Up,
		Down
	}

	public IndexChanged IndexChangedFunction;

	private readonly ComboDropDirection _mDropDirection;

	private int _mSelectedItemIndex = -1;

	private readonly MilMo_Widget _mTop;

	private Color _mBackgroundColor;

	private Color _mItemTextColor;

	private Color _mSelectedItemTextColor;

	private readonly List<ComboBoxButton> _mItems;

	private readonly Vector2 _mMaxSize = Vector2.zero;

	private bool _mUseTop = true;

	public bool UseIcon { private get; set; }

	private MilMo_ScrollView Menu { get; set; }

	public MilMo_Widget Background { get; private set; }

	private MilMo_Widget Icon { get; set; }

	public MilMo_Button BoxTextureWidget { get; private set; }

	public Color BackgroundColor
	{
		set
		{
			_mBackgroundColor = value;
			RefreshUI();
		}
	}

	public Color ItemTextColor
	{
		set
		{
			_mItemTextColor = value;
			RefreshUI();
		}
	}

	public Color SelectedItemTextColor
	{
		private get
		{
			return _mSelectedItemTextColor;
		}
		set
		{
			_mSelectedItemTextColor = value;
		}
	}

	public int SelectedIndex => _mSelectedItemIndex;

	public MilMo_Widget Top => _mTop;

	public MilMo_ComboBox(MilMo_UserInterface ui, ComboDropDirection dropDirection, MilMo_LocString defaultText, float width, float maxHeight, Color backgroundColor, Color textColor)
		: base(ui)
	{
		UseIcon = true;
		_mItemTextColor = textColor;
		_mBackgroundColor = backgroundColor;
		_mSelectedItemTextColor = _mItemTextColor;
		_mSelectedItemTextColor.a = 0.5f;
		_mDropDirection = dropDirection;
		BoxTextureWidget = new MilMo_Button(UI);
		BoxTextureWidget.SetText(defaultText);
		_mItems = new List<ComboBoxButton>();
		_mTop = new MilMo_Widget(UI);
		Background = new MilMo_Widget(UI);
		Menu = new MilMo_ScrollView(UI);
		Icon = new MilMo_Widget(UI);
		_mMaxSize.x = width;
		_mMaxSize.y = maxHeight;
		InitWidgets();
	}

	private void InitWidgets()
	{
		SetAlignment((_mDropDirection == ComboDropDirection.Up) ? MilMo_GUI.Align.BottomLeft : MilMo_GUI.Align.TopLeft);
		base.FixedRes = true;
		AllowPointerFocus = false;
		Background.SetTexture("Batch01/Textures/Generic/ComboBoxMenu");
		Background.SetDefaultColor(_mBackgroundColor);
		Background.SetAlignment(MilMo_GUI.Align.TopLeft);
		Background.Enabled = false;
		AddChild(Background);
		BoxTextureWidget.SetAllTextures("Batch01/Textures/Generic/ComboBox");
		BoxTextureWidget.SetDefaultTextColor(_mItemTextColor);
		BoxTextureWidget.SetFont(MilMo_GUI.Font.ArialRounded);
		BoxTextureWidget.SetAlignment(MilMo_GUI.Align.TopLeft);
		BoxTextureWidget.SetHoverTextColor(_mSelectedItemTextColor);
		BoxTextureWidget.AllowPointerFocus = true;
		BoxTextureWidget.FixedRes = true;
		BoxTextureWidget.Function = Toggle;
		BoxTextureWidget.Enabled = true;
		AddChild(BoxTextureWidget);
		Menu.SetAlignment(MilMo_GUI.Align.TopLeft);
		Menu.FixedRes = true;
		Menu.AllowPointerFocus = true;
		Menu.Enabled = false;
		Menu.MouseWheelScrollable = false;
		AddChild(Menu);
		_mTop.SetTexture("Batch01/Textures/Generic/ComboBoxMenuTop");
		_mTop.SetAlignment(MilMo_GUI.Align.BottomLeft);
		if (_mDropDirection == ComboDropDirection.Down)
		{
			_mTop.SetTexture("Batch01/Textures/Generic/ComboBoxMenuBottom");
			_mTop.SetAlignment(MilMo_GUI.Align.TopLeft);
		}
		_mTop.SetDefaultColor(_mBackgroundColor);
		Background.AddChild(_mTop);
		_mTop.Enabled = false;
		_mTop.UseParentAlpha = false;
		Icon.SetTexture((_mDropDirection == ComboDropDirection.Up) ? "Batch01/Textures/Generic/ComboBoxIconUp" : "Batch01/Textures/Generic/ComboBoxIconDown");
		Icon.SetAlignment(MilMo_GUI.Align.CenterRight);
		BoxTextureWidget.AddChild(Icon);
	}

	private void Toggle(object o)
	{
		if (Menu.Enabled)
		{
			CloseMenu();
		}
		else if (_mItems.Count > 1)
		{
			OpenMenu();
		}
		RefreshUI();
	}

	private void CloseMenu()
	{
		Menu.Enabled = false;
		Background.Enabled = false;
		if (_mItems.Count > 1 && UseIcon)
		{
			Icon.Enabled = true;
		}
	}

	private void OpenMenu()
	{
		Menu.Enabled = true;
		Background.Enabled = true;
		Icon.Enabled = false;
		UI.BringToFront(this);
	}

	public void SelectItem(int selectionIndex)
	{
		_mSelectedItemIndex = selectionIndex;
		if (Menu.Enabled)
		{
			CloseMenu();
		}
		if (IndexChangedFunction != null)
		{
			IndexChangedFunction(_mSelectedItemIndex);
		}
		RefreshUI();
		foreach (ComboBoxButton mItem in _mItems)
		{
			mItem.SetDefaultTextColor(_mItemTextColor);
		}
		_mItems[_mSelectedItemIndex].SetDefaultTextColor(_mSelectedItemTextColor);
	}

	private void SetColorAndScale()
	{
		Menu.SetDefaultTextColor(_mItemTextColor);
		BoxTextureWidget.SetDefaultTextColor(_mItemTextColor);
		_mTop.SetDefaultColor(_mBackgroundColor);
		Background.SetDefaultColor(_mBackgroundColor);
		Icon.SetScale(13f, 14f);
		BoxTextureWidget.SetScale(_mMaxSize.x, 24f);
		Menu.SetScale(_mMaxSize.x, 0f);
		_mTop.SetXScale(_mMaxSize.x);
	}

	public void RefreshUI()
	{
		Menu.ShowHorizontalBar(h: false);
		SetColorAndScale();
		float num = 5f;
		_mTop.Enabled = _mUseTop;
		if (_mItems.Count > 0)
		{
			num += _mItems[0].Scale.y * (float)(_mItems.Count + 1);
		}
		if (num > _mMaxSize.y)
		{
			Menu.SetScale(_mMaxSize.x - 5f, _mMaxSize.y);
			Menu.ShowVerticalBar(v: true);
		}
		else
		{
			Menu.ShowVerticalBar(v: false);
			Menu.SetScale(_mMaxSize.x, num);
		}
		SetScale(_mMaxSize);
		Menu.RemoveAllChildren();
		if (_mItems.Count > 0)
		{
			Vector2 position = new Vector2(25f, 10f);
			for (int i = 0; i < _mItems.Count; i++)
			{
				Menu.AddChild(_mItems[i]);
				_mItems[i].SetPosition(position);
				position.y += _mItems[i].Scale.y;
				if (i != _mSelectedItemIndex)
				{
					_mItems[i].SetDefaultTextColor(_mItemTextColor);
				}
			}
		}
		if (Menu.MShowVertBar)
		{
			Menu.RefreshViewSize(0f, 15f);
			Menu.Scale.y -= 4f;
		}
		else
		{
			Menu.RefreshViewSize();
		}
		if (_mSelectedItemIndex != -1)
		{
			BoxTextureWidget.SetText(_mItems[_mSelectedItemIndex].Text);
		}
		if (Menu.MShowVertBar)
		{
			Background.SetScale(Menu.Scale.x + 5f, Menu.Scale.y);
		}
		else
		{
			Background.SetScale(Menu.Scale);
		}
		Background.SetPosition(Menu.Pos);
		if (_mDropDirection == ComboDropDirection.Up)
		{
			Menu.SetPosition(0f, BoxTextureWidget.Pos.y - Menu.Scale.y);
			BoxTextureWidget.SetPosition(0f, Scale.y - BoxTextureWidget.Scale.y);
			_mTop.SetPosition(0f, Background.Scale.y);
		}
		else
		{
			Menu.SetPosition(0f, BoxTextureWidget.Pos.y + BoxTextureWidget.Scale.y);
			BoxTextureWidget.SetPosition(0f, 0f);
			_mTop.SetPosition(0f, Background.Scale.y);
		}
		Icon.Enabled = false;
		if (UseIcon)
		{
			if (_mItems.Count > 1 && !Menu.Enabled)
			{
				Icon.Enabled = true;
			}
			Icon.SetPosition(BoxTextureWidget.Scale.x - 7f, BoxTextureWidget.Scale.y * 0.5f);
		}
	}

	public void AddItem(MilMo_LocString text)
	{
		ComboBoxButton comboBoxButton = new ComboBoxButton(UI);
		comboBoxButton.SetScale(_mMaxSize.x - 10f, 20f);
		comboBoxButton.SetText(text);
		comboBoxButton.SetDefaultTextColor(_mItemTextColor);
		comboBoxButton.SetFont(MilMo_GUI.Font.ArialRounded);
		comboBoxButton.FixedRes = true;
		comboBoxButton.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		comboBoxButton.AllowPointerFocus = true;
		comboBoxButton.SetAlignment(MilMo_GUI.Align.TopLeft);
		int selectionIndex = _mItems.Count;
		comboBoxButton.SetHoverTextColor(SelectedItemTextColor);
		comboBoxButton.Function = delegate
		{
			SelectItem(selectionIndex);
		};
		AddItem(comboBoxButton);
	}

	private void AddItem(ComboBoxButton button)
	{
		_mItems.Add(button);
		RefreshUI();
	}

	public override void Step()
	{
		if (!Enabled)
		{
			return;
		}
		if (Menu.Enabled && MilMo_Pointer.LeftClick)
		{
			bool flag = false;
			foreach (ComboBoxButton mItem in _mItems)
			{
				if (mItem.Hover())
				{
					flag = true;
					break;
				}
			}
			if (!flag && !Menu.ContainsMouse() && !BoxTextureWidget.Hover())
			{
				CloseMenu();
			}
		}
		base.Step();
	}

	public void RemoveAll()
	{
		CloseMenu();
		Menu.RemoveAllChildren();
		_mItems.Clear();
		_mSelectedItemIndex = -1;
	}
}
