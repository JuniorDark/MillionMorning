using System.Collections.Generic;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.World.GUI.Navigator;
using Code.World.Level;
using Code.World.Level.LevelInfo;
using Code.World.Player;
using Core.GameEvent;
using UnityEngine;

namespace Code.World.GUI.Hub;

public sealed class MilMo_HubMenu : MilMo_Widget
{
	private readonly MilMo_Button _backButton;

	private readonly MilMo_Button _fullScreenButton;

	private readonly List<MilMo_Button> _buttonList;

	private readonly MilMo_Widget _background;

	private readonly MilMo_PremiumInfoWidget _premiumInfoWidget;

	private readonly Vector2 _backButtonStartPos = new Vector2((float)Screen.width / 2f, -150f);

	private bool _showingMaximizeButton;

	private static readonly MilMo_AudioClip TickSound = new MilMo_AudioClip("Content/Sounds/Batch01/CharBuilder/CharBuilderTick");

	public MilMo_HubMenu(MilMo_UserInterface ui)
		: base(ui)
	{
		_buttonList = new List<MilMo_Button>();
		_background = new MilMo_Widget(UI);
		_background.AllowPointerFocus = false;
		_background.SetPosPull(0.05f, 0.05f);
		_background.SetPosDrag(0.65f, 0.65f);
		_background.SetAlignment(MilMo_GUI.Align.TopCenter);
		_background.SetTexture("Batch01/Textures/HUD/PaneMenuBarHub");
		_background.SetScale(246f, 60f);
		_background.SetDefaultColor(1f, 1f, 1f, 0.65f);
		UI.AddChild(_background);
		_backButton = new MilMo_Button(UI);
		_backButton.SetAlignment(MilMo_GUI.Align.TopCenter);
		_backButton.FixedRes = true;
		_backButton.SetDefaultAngle(0f);
		_backButton.SetHoverAngle(0f);
		_backButton.SetAngle(0f);
		_backButton.UseParentAlpha = false;
		_backButton.AllowPointerFocus = true;
		_backButton.AngleMover.MinVel.x = 0.01f;
		_backButton.SetAnglePull(0.03f);
		_backButton.SetAngleDrag(0.9f);
		_backButton.SetScalePull(0.07f, 0.07f);
		_backButton.SetScaleDrag(0.6f, 0.6f);
		_backButton.SetPosPull(0f, 0.02f);
		_backButton.SetPosDrag(0f, 0.9f);
		_backButton.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		_backButton.SetExtraScaleOnHover(5f, 5f);
		_backButton.SetHoverSound(TickSound);
		_backButton.PosMover.MinVel = new Vector2(0.0001845f, 0.0001845f);
		_backButton.Tooltip = new MilMo_Tooltip(MilMo_Localization.GetLocString("WorldMap_4757"));
		_backButton.LeftMouseDownFunction = delegate
		{
			if (MilMo_Player.Instance.OkToLeaveHub())
			{
				if (MilMo_Level.CurrentLevel != null && MilMo_LevelInfo.IsChatroom(MilMo_Level.CurrentLevel.VerboseName))
				{
					MilMo_Hub.WasTravelClosed = true;
					MilMo_Hub.TravelClosedFullLevelName = MilMo_Level.LastAdventureLevel;
					MilMo_Player.Instance.RequestLeaveHub();
					MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Confirm);
				}
				else
				{
					if (MilMo_Player.InHome)
					{
						MilMo_Hub.WasTravelClosed = true;
						MilMo_Hub.TravelClosedFullLevelName = MilMo_Level.LastAdventureLevel;
					}
					if (MilMo_LevelInfo.GetLevelInfoData(MilMo_Level.LastAdventureLevel) != null)
					{
						MilMo_Player.Instance.RequestLeaveHub();
						MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Confirm);
					}
				}
			}
			else
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
			}
		};
		UI.AddChild(_backButton);
		_fullScreenButton = new MilMo_Button(UI);
		_fullScreenButton.SetAlignment(MilMo_GUI.Align.TopRight);
		_fullScreenButton.Identifier = "FullScreen";
		_fullScreenButton.Tooltip = new MilMo_Tooltip(MilMo_Localization.GetLocString("WorldMap_5590"));
		_fullScreenButton.UseParentAlpha = false;
		_fullScreenButton.SetHoverSound(TickSound);
		_fullScreenButton.SetDefaultColor(1f, 1f, 1f, 0.6f);
		_fullScreenButton.SetHoverColor(1f, 1f, 1f, 1f);
		_fullScreenButton.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
		_fullScreenButton.LeftMouseDownFunction = delegate
		{
			GameEvent.ToggleFullscreenEvent.RaiseEvent();
			UpdateFullscreenButton();
		};
		UI.AddChild(_fullScreenButton);
		MilMo_Button milMo_Button = CreateMenuButton("Batch01/Textures/HUD/IconOptions", "Batch01/Textures/HUD/IconOptionsMO", delegate
		{
			GameEvent.ToggleOptionsEvent.RaiseEvent();
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Select);
		});
		milMo_Button.Tooltip = new MilMo_Tooltip(MilMo_Localization.GetLocString("WorldMap_4751"));
		_buttonList.Add(milMo_Button);
		UI.AddChild(milMo_Button);
		MilMoDeliveryBoxTimer milMoDeliveryBoxTimer = new MilMoDeliveryBoxTimer(UI);
		milMoDeliveryBoxTimer.Open();
		UI.AddChild(milMoDeliveryBoxTimer);
		_premiumInfoWidget = new MilMo_PremiumInfoWidget(UI);
		UI.AddChild(_premiumInfoWidget);
	}

	private MilMo_Button CreateMenuButton(string texture, string mouseOverTexture, MilMo_Button.ButtonFunc function)
	{
		MilMo_Button milMo_Button = new MilMo_Button(UI);
		milMo_Button.SetAllTextures(texture);
		milMo_Button.SetHoverTexture(mouseOverTexture);
		milMo_Button.SetAlignment(MilMo_GUI.Align.TopCenter);
		milMo_Button.SetScale(60f, 60f);
		milMo_Button.SetScalePull(0.05f, 0.05f);
		milMo_Button.SetScaleDrag(0.6f, 0.7f);
		milMo_Button.SetHoverColor(1f, 1f, 0f, 1f);
		milMo_Button.Function = function;
		milMo_Button.SetHoverSound(TickSound);
		milMo_Button.UseParentAlpha = false;
		milMo_Button.SetPosPull(0.08f, 0.08f);
		milMo_Button.SetPosDrag(0.6f, 0.6f);
		milMo_Button.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		milMo_Button.SetExtraScaleOnHover(5f, 5f);
		return milMo_Button;
	}

	public void RefreshUI()
	{
		_premiumInfoWidget.Refresh();
		_premiumInfoWidget.SetPosition((float)Screen.width * 0.5f - 105f, Screen.height - 52);
		_background.SetScale(246f, 60f);
		_background.SetPosition(Screen.width / 2 + 35, 0f);
		_backButton.SetScale(104f, 104f);
		_backButton.GoToNow(Screen.width / 2 + 35, 8f);
		_backButton.Angle(0f);
		UI.BringToFront(_background);
		for (int i = 0; i < _buttonList.Count; i++)
		{
			_buttonList[i].SetScale(60f, 60f);
			_buttonList[i].SetPosition(_background.Pos.x + (float)i * (_buttonList[i].ScaleMover.Target.x / UI.Res.x) + 40f, _background.Pos.y + 10f);
			UI.BringToFront(_buttonList[i]);
		}
		UI.BringToFront(_backButton);
		_fullScreenButton.SetPosition(Screen.width, 0f);
		_fullScreenButton.SetScale(35f, 35f);
		UI.BringToFront(_fullScreenButton);
	}

	public void Open()
	{
		_backButton.Enabled = true;
		_backButton.SetPosition(_backButtonStartPos);
		_backButton.GoTo(Screen.width / 2 + 35, 8f);
		_backButton.SetDefaultAngle(0f);
		_backButton.SetAngle(-180f);
		MilMo_LevelInfoData levelInfoData = MilMo_LevelInfo.GetLevelInfoData(MilMo_Level.LastAdventureLevel);
		if (levelInfoData != null)
		{
			string text = levelInfoData.World.Replace("orld", "");
			string text2 = levelInfoData.Level.Replace("evel", "");
			string filename = "Content/Worlds/" + text + "/LevelIcons/LevelIcon" + text + text2;
			_backButton.SetAllTextures(filename, prefixStandardGuiPath: false);
			foreach (MilMo_Button button in _buttonList)
			{
				button.GoToNow(-500f, _background.Pos.y + 10f);
			}
		}
		RefreshUI();
	}

	public void Close()
	{
		_backButton.Enabled = false;
		foreach (MilMo_Button button in _buttonList)
		{
			button.GoToNow(-500f, _background.Pos.y + 10f);
		}
	}

	public override void Step()
	{
		if (Screen.fullScreen)
		{
			if (_showingMaximizeButton)
			{
				ShowMinimizeButton();
			}
		}
		else if (!_showingMaximizeButton)
		{
			ShowMaximizeButton();
		}
		base.Step();
	}

	private void ShowMaximizeButton()
	{
		_fullScreenButton.SetTexture("Batch01/Textures/HUD/MaximizeButtonWhite");
		_fullScreenButton.SetHoverTexture("Batch01/Textures/HUD/MaximizeButtonWhite");
		_fullScreenButton.SetPressedTexture("Batch01/Textures/HUD/MaximizeButtonWhitePressed");
		_showingMaximizeButton = true;
	}

	private void ShowMinimizeButton()
	{
		_fullScreenButton.SetTexture("Batch01/Textures/HUD/MinimizeButtonWhite");
		_fullScreenButton.SetHoverTexture("Batch01/Textures/HUD/MinimizeButtonWhite");
		_fullScreenButton.SetPressedTexture("Batch01/Textures/HUD/MinimizeButtonWhitePressed");
		_showingMaximizeButton = false;
	}

	private void UpdateFullscreenButton()
	{
		if (Screen.fullScreen)
		{
			ShowMinimizeButton();
		}
		else
		{
			ShowMinimizeButton();
		}
	}
}
