using Code.Core.GUI.Core;
using Code.Core.Input;
using Code.Core.ResourceSystem;
using Code.World.Home;
using UnityEngine;

namespace Code.World.GUI.Homes;

public class MilMoHomeDoorMarker : MilMoObjectMarker
{
	public delegate void OnClickCallback(long id);

	private readonly OnClickCallback _callback;

	private readonly MilMo_HomeFurniture _furniture;

	public MilMoHomeDoorMarker(MilMo_UserInterface ui, MilMo_HomeFurniture furniture, string texture, Vector2 scale, OnClickCallback callback)
		: base(ui, furniture.GameObject, Vector3.zero, 5, 0, MilMo_LocString.Empty)
	{
		_callback = callback;
		_furniture = furniture;
		FadeToDefaultColor = false;
		SetAlpha(0f);
		SetAllTextures(texture);
		ScaleTo(scale);
	}

	public override void Show()
	{
		SetAlpha(0.65f);
		UI.AddChild(this);
	}

	public override void Step()
	{
		if (Enabled)
		{
			base.Step();
			if (Hover())
			{
				InternalMouseOver();
			}
			else
			{
				AlphaTo(0.65f);
			}
		}
	}

	private void InternalMouseOver()
	{
		if (MilMo_Pointer.LeftButton)
		{
			InternalMouseDown();
		}
		AlphaTo(1f);
	}

	private void InternalMouseDown()
	{
		if (_callback != null)
		{
			_callback(_furniture.Item.Id);
		}
	}
}
