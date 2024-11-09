using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.Input;
using Code.Core.ResourceSystem;
using Code.World.GUI;
using Code.World.Player;
using UnityEngine;

namespace Code.World.WorldMap;

internal sealed class MilMo_WorldMapMouseIcon : MilMo_Widget
{
	private readonly MilMo_Texture _openHandTexture;

	private readonly MilMo_Texture _closedHandTexture;

	private bool _isDeactivating;

	private readonly MilMo_UserInterface _worldMapUI;

	public MilMo_WorldMapMouseIcon(MilMo_UserInterface worldMapUI)
		: base(MilMo_GlobalUI.GetSystemUI)
	{
		_worldMapUI = worldMapUI;
		_openHandTexture = new MilMo_Texture("Content/GUI/Batch01/Textures/Homes/IconGrab");
		_openHandTexture.AsyncLoad();
		_closedHandTexture = new MilMo_Texture("Content/GUI/Batch01/Textures/WorldMap/IconGrabMap");
		_closedHandTexture.AsyncLoad();
		SetScale(32f, 32f);
		SetAlignment(MilMo_GUI.Align.CenterCenter);
		PosMover.Drag = new Vector2(0.5f, 0.5f);
		PosMover.Pull = new Vector2(0.25f, 0.25f);
		AllowPointerFocus = false;
		FadeToDefaultColor = false;
		Enabled = false;
		UI.AddChild(this);
	}

	public override void SetEnabled(bool e)
	{
		if (e)
		{
			Activate();
		}
		else
		{
			Deactivate();
		}
	}

	private void Activate()
	{
		if (!Enabled && MilMo_Player.Instance != null && MilMo_Player.Instance.InNavigator && !MilMo_Player.Instance.LeavingNavigator)
		{
			SetAlpha(0f);
			Enabled = true;
			AlphaTo(1f);
		}
	}

	private void Deactivate()
	{
		if (!_isDeactivating && Enabled)
		{
			AlphaTo(0f);
			MilMo_EventSystem.At(0.2f, delegate
			{
				Enabled = false;
				_isDeactivating = false;
			});
			_isDeactivating = true;
		}
	}

	public override void Step()
	{
		if (!Enabled)
		{
			return;
		}
		if (MilMo_Player.Instance != null && !MilMo_Player.Instance.InNavigator && Enabled)
		{
			Enabled = false;
			SetAlpha(0f);
			_isDeactivating = false;
			return;
		}
		if (MilMo_UserInterfaceManager.MouseFocus != null && MilMo_UserInterfaceManager.MouseFocus.UI != _worldMapUI)
		{
			Deactivate();
		}
		SetTexture(MilMo_Pointer.LeftButton ? _closedHandTexture : _openHandTexture);
		GoTo(MilMo_Pointer.Position);
		base.Step();
	}
}
