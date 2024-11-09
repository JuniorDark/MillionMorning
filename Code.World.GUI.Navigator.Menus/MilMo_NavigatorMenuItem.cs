using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using UnityEngine;

namespace Code.World.GUI.Navigator.Menus;

public abstract class MilMo_NavigatorMenuItem
{
	protected MilMo_Widget Menu;

	protected MilMo_ScrollView MScrollView;

	protected readonly MilMo_UserInterface UI;

	protected MilMo_NavigatorMenuTabWindow MParentWindow;

	public virtual MilMo_NavigatorMenuTabWindow ParentWindow
	{
		get
		{
			return MParentWindow;
		}
		set
		{
			MParentWindow = value;
			MParentWindow.ChildMenuItem = this;
		}
	}

	private Rect MenuPosition => Menu.GetScreenPosition();

	protected MilMo_NavigatorMenuItem(MilMo_UserInterface ui)
	{
		UI = ui;
	}

	public virtual MilMo_Widget CreateMenu()
	{
		return null;
	}

	public virtual void UpdateMenu()
	{
	}

	public virtual void Open()
	{
	}

	public bool RectIsIntersecting(Rect rectToCheckIfOverlap)
	{
		if (!Menu.Enabled)
		{
			return false;
		}
		Rect menuPosition = MenuPosition;
		float x = rectToCheckIfOverlap.x;
		float num = rectToCheckIfOverlap.x + rectToCheckIfOverlap.width;
		float y = rectToCheckIfOverlap.y;
		float num2 = rectToCheckIfOverlap.y + rectToCheckIfOverlap.height;
		float x2 = menuPosition.x;
		float num3 = menuPosition.x + menuPosition.width;
		float y2 = menuPosition.y;
		float num4 = menuPosition.y + menuPosition.height;
		bool flag = false;
		bool flag2 = false;
		if (x < num3 && x2 < x)
		{
			flag = true;
		}
		if (x2 < num && x < x2)
		{
			flag = true;
		}
		if (y > y2 && y < num4)
		{
			flag2 = true;
		}
		if (y2 > y && y2 < num2)
		{
			flag2 = true;
		}
		return flag && flag2;
	}
}
