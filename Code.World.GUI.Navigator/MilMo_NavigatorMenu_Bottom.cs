using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.World.WorldMap;
using UnityEngine;

namespace Code.World.GUI.Navigator;

public sealed class MilMo_NavigatorMenu_Bottom : MilMo_Widget
{
	public MilMo_NavigatorMenu_Bottom(MilMo_UserInterface ui)
		: base(ui)
	{
		SetAlignment(MilMo_GUI.Align.TopLeft);
		base.FixedRes = true;
		AllowPointerFocus = false;
		Close();
	}

	public void RefreshUI()
	{
		if (MilMo_WorldMap.UI != null)
		{
			SetScale((float)Screen.width - MilMo_WorldMap.UI.GlobalPosOffset.x * 2f, MilMo_WorldMap.UI.GlobalPosOffset.y + 84f * MilMo_WorldMap.UI.Res.y);
			SetPosition(MilMo_WorldMap.UI.GlobalPosOffset.x, (float)Screen.height - (MilMo_WorldMap.UI.GlobalPosOffset.y + 84f * MilMo_WorldMap.UI.Res.y));
		}
		else
		{
			SetScale(0f, 0f);
			SetPosition(0f, 0f);
		}
	}

	public void Open()
	{
		Enabled = true;
	}

	public void Close()
	{
		Enabled = false;
	}
}
