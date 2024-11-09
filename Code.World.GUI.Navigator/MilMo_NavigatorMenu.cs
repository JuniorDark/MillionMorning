using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.Input;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.World.GUI.Navigator.Menus;
using Code.World.Level;
using Code.World.WorldMap;
using Core.Analytics;
using Core.GameEvent;
using UnityEngine;

namespace Code.World.GUI.Navigator;

public sealed class MilMo_NavigatorMenu : MilMo_Widget
{
	private readonly List<MilMo_NavigatorMenuTabWindow> _mWindows;

	private readonly MilMo_NavigationButton _mNavigatorButton;

	private readonly MilMo_Widget _mButtonBackground;

	private int _mOpenWindowIndex = -1;

	private MilMo_Button _mFullScreenButton;

	private readonly MilMo_AudioClip _mTickSound = new MilMo_AudioClip("Content/Sounds/Batch01/CharBuilder/CharBuilderTick");

	private bool _mShowingMaximizeButton;

	private MilMo_TimerEvent _mOpenTimer;

	private readonly MilMo_Button _mWorldButton;

	private readonly MilMo_Button _mWorldButton2;

	private bool _shouldAutoShow;

	private readonly MilMo_NavigatorMenu_Bottom _mBottomMenu;

	private readonly MilMo_NavigatorMenu_ChangeWorld _mChangeWorldMenu;

	private readonly MilMo_NavigatorMenuTabWindow _mWorldNavWindow;

	public MilMo_NavigationButton NavigatorButton => _mNavigatorButton;

	public MilMo_NavigatorMenu()
		: base(MilMo_GlobalUI.GetSystemUI)
	{
		_mBottomMenu = new MilMo_NavigatorMenu_Bottom(UI);
		UI.AddChild(_mBottomMenu);
		_mNavigatorButton = new MilMo_NavigationButton();
		CreateFullScreenButton();
		if (Screen.fullScreen)
		{
			ShowMinimizeButton();
		}
		else
		{
			ShowMaximizeButton();
		}
		_mChangeWorldMenu = new MilMo_NavigatorMenu_ChangeWorld(UI);
		_mWindows = new List<MilMo_NavigatorMenuTabWindow>();
		base.FixedRes = true;
		_mButtonBackground = new MilMo_Widget(UI);
		_mButtonBackground.ScaleNow(47f, 56f);
		_mButtonBackground.SetTextureBlack();
		_mButtonBackground.FadeToDefaultColor = false;
		_mButtonBackground.SetAlpha(0f);
		_mButtonBackground.SetFadeSpeed(0.025f);
		_mButtonBackground.UseParentAlpha = false;
		_mButtonBackground.FixedRes = true;
		_mButtonBackground.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mButtonBackground.SetPosition(-500f, -500f);
		UI.AddChild(_mButtonBackground);
		_mWorldButton2 = new MilMo_Button(UI);
		_mWorldButton2.FixedRes = true;
		_mWorldButton2.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mWorldButton2.SetScale(60f, 60f);
		_mWorldButton2.AllowPointerFocus = false;
		UI.AddChild(_mWorldButton2);
		_mWorldButton = new MilMo_Button(UI);
		_mWorldButton.FixedRes = true;
		_mWorldButton.SetAllTextures("Batch01/Textures/HUD/IconMap");
		_mWorldButton.SetHoverTexture("Batch01/Textures/HUD/IconMapMO");
		_mWorldButton.SetAlignment(MilMo_GUI.Align.BottomLeft);
		_mWorldButton.SetScale(104f, 104f);
		_mWorldButton.SetScalePull(0.05f, 0.05f);
		_mWorldButton.SetScaleDrag(0.6f, 0.7f);
		_mWorldButton.SetHoverSound(new MilMo_AudioClip("Content/Sounds/Batch01/CharBuilder/CharBuilderTick"));
		_mWorldButton.SetHoverColor(1f, 1f, 0f, 1f);
		_mWorldButton.UseParentAlpha = false;
		_mWorldButton.SetPosPull(0.08f, 0.08f);
		_mWorldButton.SetPosDrag(0.6f, 0.6f);
		_mWorldButton.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		_mWorldButton.SetExtraScaleOnHover(5f, 5f);
		_mWorldButton.Tooltip = new MilMo_Tooltip(MilMo_Localization.GetLocString("World_6060"));
		_mWorldNavWindow = new MilMo_NavigatorMenuTabWindow(_mChangeWorldMenu.CreateMenu(), _mWorldButton2);
		_mWorldNavWindow.MAlign = MilMo_NavigatorMenuTabWindow.ButtonAlignment.BottomLeft;
		_mWindows.Add(_mWorldNavWindow);
		_mWorldButton.Function = ShowMenu;
		UI.AddChild(_mWorldButton);
		_mWorldButton.Enabled = false;
		SetTextureInvisible();
		SetPosPull(0.05f, 0.05f);
		SetPosDrag(0.65f, 0.65f);
		SetAlignment(MilMo_GUI.Align.CenterLeft);
		SetPosition(-500f, 0f);
		Enabled = false;
		UI.AddChild(this);
	}

	private void SetAutoShow()
	{
		bool shouldAutoShow = MilMo_Level.CurrentLevel?.VerboseName == "World00:Level08";
		_shouldAutoShow = shouldAutoShow;
	}

	private void ShowMenu(object arg)
	{
		if (_mWorldNavWindow.Enabled)
		{
			CloseAllWindows();
		}
		else
		{
			_mWorldNavWindow.Open();
			string text = MilMo_Level.CurrentLevel?.VerboseName;
			if (text != null)
			{
				Analytics.OpenCompass(text);
			}
			_mWorldButton.ScaleTo(60f, 60f);
			_mButtonBackground.SetScale(46f, 60f);
			_mButtonBackground.SetPosition(new Vector2(_mWorldButton2.Pos.x + 6f, _mWorldButton2.Pos.y - 2f));
			_mWorldButton.IgnoreNextStepDueToBringToFront = true;
		}
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Select);
	}

	private void CreateFullScreenButton()
	{
		_mFullScreenButton = new MilMo_Button(UI);
		_mFullScreenButton.SetAlignment(MilMo_GUI.Align.TopRight);
		_mFullScreenButton.SetHoverSound(_mTickSound);
		_mFullScreenButton.SetDefaultColor(1f, 1f, 1f, 0.6f);
		_mFullScreenButton.SetHoverColor(1f, 1f, 1f, 1f);
		_mFullScreenButton.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
		_mFullScreenButton.FixedRes = true;
		_mFullScreenButton.Tooltip = new MilMo_Tooltip(MilMo_Localization.GetLocString("WorldMap_5590"));
		_mFullScreenButton.Enabled = false;
		_mFullScreenButton.LeftMouseDownFunction = delegate
		{
			GameEvent.ToggleFullscreenEvent.RaiseEvent();
		};
		UI.AddChild(_mFullScreenButton);
	}

	public void RefreshUI()
	{
		_mWorldNavWindow.Close();
		_mWorldButton.ScaleTo(104f, 104f);
		_mNavigatorButton.RefreshUI();
		_mFullScreenButton.SetPosition(Screen.width, 0f);
		_mFullScreenButton.SetScale(35f, 35f);
		SetPosition(MilMo_WorldMap.UI.GlobalPosOffset + new Vector2(0f, 84f * MilMo_WorldMap.UI.Res.y - 15f));
		_mWorldNavWindow.RefreshUI();
		if (_mOpenWindowIndex != -1)
		{
			_mWindows[_mOpenWindowIndex].RefreshUI();
			_mButtonBackground.SetPosition(Pos + new Vector2(_mWindows[_mOpenWindowIndex].MButtonToExpandFrom.Pos.x + 6f, _mWindows[_mOpenWindowIndex].MButtonToExpandFrom.Pos.y + 3f));
		}
		if (_mWorldNavWindow.Enabled)
		{
			_mButtonBackground.SetScale(46f, 60f);
			_mButtonBackground.SetPosition(new Vector2(_mWorldButton2.Pos.x + 6f, _mWorldButton2.Pos.y - 2f));
		}
		else
		{
			_mButtonBackground.SetScale(47f, 59f);
		}
		_mBottomMenu.RefreshUI();
		_mWorldButton.SetPosition(_mBottomMenu.Pos.x + 15f, _mBottomMenu.Pos.y - 15f + 60f);
		_mWorldButton2.SetPosition(_mBottomMenu.Pos.x + 15f, _mBottomMenu.Pos.y - 15f);
	}

	private void CloseAllWindows()
	{
		for (int i = 0; i < _mWindows.Count; i++)
		{
			_mWindows[i].Close();
		}
		_mWorldNavWindow.Close();
		_mWorldButton.ScaleTo(104f, 104f);
		_mOpenWindowIndex = -1;
		_mButtonBackground.SetPosition(-500f, -500f);
	}

	private void UpdateItems()
	{
		SetAutoShow();
		_mChangeWorldMenu.UpdateMenu();
	}

	public void Open()
	{
		_mWorldButton.SetXPos(-500f);
		_mOpenTimer = MilMo_EventSystem.At(0.5f, delegate
		{
			Enabled = true;
			if (MilMo_WorldMap.UI != null)
			{
				GoTo(MilMo_WorldMap.UI.GlobalPosOffset + new Vector2(0f, 84f * MilMo_WorldMap.UI.Res.y - 15f));
				UpdateItems();
				_mNavigatorButton.Open();
				_mFullScreenButton.Enabled = true;
				BringToFront(_mFullScreenButton);
				MilMo_EventSystem.RemoveTimerEvent(_mOpenTimer);
				_mOpenTimer = null;
				_mWorldButton.Enabled = true;
				_mWorldButton2.Enabled = true;
				_mBottomMenu.Open();
				_mWorldButton.GoTo(_mBottomMenu.Pos.x + 15f, _mBottomMenu.Pos.y - 15f + 60f);
				if (_shouldAutoShow)
				{
					ShowMenu(null);
				}
			}
		});
	}

	public override void Step()
	{
		if (!Enabled)
		{
			return;
		}
		base.Step();
		if (!MilMo_Pointer.LeftButton || _mWorldButton.Hover())
		{
			return;
		}
		for (int i = 0; i < _mWindows.Count; i++)
		{
			if (_mWindows[i].Enabled && !_mWindows[i].MouseOverAll())
			{
				CloseAllWindows();
				break;
			}
		}
	}

	public override void Draw()
	{
		if (!Enabled)
		{
			return;
		}
		if (Screen.fullScreen)
		{
			if (_mShowingMaximizeButton)
			{
				ShowMinimizeButton();
			}
		}
		else if (!_mShowingMaximizeButton)
		{
			ShowMaximizeButton();
		}
		if (_mWorldNavWindow.Enabled)
		{
			MilMo_WorldMap.HideArrow();
		}
		else
		{
			MilMo_WorldMap.ShowArrow();
		}
		base.Draw();
	}

	public void Close()
	{
		if (_mOpenTimer != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_mOpenTimer);
			_mOpenTimer = null;
		}
		Enabled = false;
		CloseAllWindows();
		GoTo(-500f, MilMo_WorldMap.UI.GlobalPosOffset.y + 84f * MilMo_WorldMap.UI.Res.y - 15f);
		_mNavigatorButton.Close();
		_mFullScreenButton.Enabled = false;
		_mBottomMenu.Close();
		_mWorldButton.Enabled = false;
		_mWorldButton2.Enabled = false;
	}

	private void ShowMaximizeButton()
	{
		_mFullScreenButton.SetTexture("Batch01/Textures/HUD/MaximizeButtonWhite");
		_mFullScreenButton.SetHoverTexture("Batch01/Textures/HUD/MaximizeButtonWhite");
		_mFullScreenButton.SetPressedTexture("Batch01/Textures/HUD/MaximizeButtonWhitePressed");
		_mShowingMaximizeButton = true;
	}

	private void ShowMinimizeButton()
	{
		_mFullScreenButton.SetTexture("Batch01/Textures/HUD/MinimizeButtonWhite");
		_mFullScreenButton.SetHoverTexture("Batch01/Textures/HUD/MinimizeButtonWhite");
		_mFullScreenButton.SetPressedTexture("Batch01/Textures/HUD/MinimizeButtonWhitePressed");
		_mShowingMaximizeButton = false;
	}
}
