using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.World.Home;
using Code.World.Level;
using Code.World.Level.LevelInfo;
using Code.World.Player;
using Code.World.WorldMap;
using UnityEngine;

namespace Code.World.GUI.Navigator;

public sealed class MilMo_NavigationButton : MilMo_Button
{
	private Vector2 _mStartPos;

	private Vector2 _mEndPos;

	private bool _mEnteringFromHub;

	public MilMo_NavigationButton()
		: base(MilMo_GlobalUI.GetSystemUI)
	{
		_mStartPos = Vector2.zero;
		_mEndPos = Vector2.zero;
		base.FixedRes = true;
		SetAlignment(MilMo_GUI.Align.TopCenter);
		SetTexture("Batch01/Textures/HUD/IconWorlds");
		SetPressedTexture("Batch01/Textures/HUD/IconWorlds");
		SetHoverTexture("Batch01/Textures/HUD/IconWorldsMO");
		SetDefaultAngle(0f);
		SetHoverAngle(0f);
		SetAngle(0f);
		UseParentAlpha = false;
		AngleMover.MinVel.x = 0.01f;
		SetAnglePull(0.03f);
		SetAngleDrag(0.9f);
		SetScalePull(0.07f, 0.07f);
		SetScaleDrag(0.6f, 0.6f);
		SetPosPull(0f, 0.02f);
		SetPosDrag(0f, 0.9f);
		PosMover.MinVel = new Vector2(0.0001845f, 0.0001845f);
		Function = delegate
		{
			if (MilMo_Player.Instance == null)
			{
				Debug.Log("MilMo_Player is null");
			}
			else if (!MilMo_Player.Instance.OkToLeaveNavigator())
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
			}
			else
			{
				if (MilMo_Home.CurrentHome != null)
				{
					if (MilMo_World.Instance.PlayerController is MilMo_PlayerControllerHome milMo_PlayerControllerHome)
					{
						milMo_PlayerControllerHome.HideHands();
					}
					MilMo_Home.CurrentHome.PlayExitRoomSound();
				}
				else
				{
					MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Confirm);
				}
				MilMo_WorldMap.Toggle();
			}
		};
		SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		SetExtraScaleOnHover(5f, 5f);
		SetHoverSound(new MilMo_AudioClip("Content/Sounds/Batch01/CharBuilder/CharBuilderTick"));
		Close();
		UI.AddChild(this);
	}

	public void SetState(bool fromHub)
	{
		_mEnteringFromHub = fromHub;
		if (fromHub)
		{
			Function = delegate
			{
				if (MilMo_Player.Instance == null)
				{
					Debug.Log("MilMo_Player is null: jb_menubar.cs m_TownButton function");
				}
				else if (MilMo_Player.Instance.InNavigator)
				{
					MilMo_WorldMap.WasGoToHubClosed = true;
					MilMo_Player.Instance.RequestLeaveNavigator();
				}
				else if (MilMo_Player.Instance.OkToEnterHub())
				{
					MilMo_Player.Instance.RequestEnterHub();
				}
				else
				{
					SetColor(1f, 0f, 0f, 1f);
					ColorTo(DefaultColor);
					MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
				}
			};
			return;
		}
		Function = delegate
		{
			if (MilMo_Home.CurrentHome != null)
			{
				if (MilMo_World.Instance.PlayerController is MilMo_PlayerControllerHome milMo_PlayerControllerHome)
				{
					milMo_PlayerControllerHome.HideHands();
				}
				MilMo_Home.CurrentHome.PlayExitRoomSound();
			}
			else
			{
				if (!MilMo_Player.Instance.OkToLeaveNavigator())
				{
					MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
					return;
				}
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Confirm);
			}
			if (!MilMo_Player.Instance.InNavigator)
			{
				MilMo_WorldMap.UserOpen();
			}
			else if (MilMo_Level.CurrentLevel != null && MilMo_LevelInfo.IsPvp(MilMo_Level.CurrentLevel.VerboseName))
			{
				MilMo_WorldMap.UserCloseAndBackToGame();
			}
			else if (MilMo_Instance.CurrentInstance == null)
			{
				MilMo_WorldMap.WasTravelClosed = true;
				MilMo_WorldMap.TravelClosedFullLevelName = MilMo_Level.LastAdventureLevel;
				MilMo_Player.Instance.RequestLeaveNavigator();
			}
			else
			{
				MilMo_WorldMap.UserCloseAndBackToGame();
			}
		};
	}

	public void Open()
	{
		_mStartPos = new Vector2((float)(Screen.width - 90) - MilMo_WorldMap.UI.GlobalPosOffset.x, -70f + MilMo_WorldMap.UI.GlobalPosOffset.y);
		_mEndPos = new Vector2((float)(Screen.width - 90) - MilMo_WorldMap.UI.GlobalPosOffset.x, MilMo_WorldMap.UI.GlobalPosOffset.y);
		if (MilMo_Player.Instance.EnteringNavigator)
		{
			SetPosition(_mStartPos);
			GoTo(_mEndPos);
			SetDefaultAngle(0f);
			SetAngle(-180f);
			Angle(0f);
		}
		else
		{
			SetPosition(_mEndPos);
			SetDefaultAngle(0f);
			SetAngle(0f);
		}
		if (_mEnteringFromHub)
		{
			SetAllTextures("Batch01/Textures/HUD/IconMenuBack");
		}
		else if (MilMo_Home.CurrentHome != null)
		{
			SetAllTextures("Batch01/Textures/Homes/IconMyHome");
		}
		else
		{
			base.Tooltip = new MilMo_Tooltip(MilMo_Localization.GetLocString("LoginScreen_15"));
			SetAllTextures("Batch01/Textures/HUD/IconMenuBack");
		}
		UI.BringToFront(this);
		Enabled = true;
	}

	public void RefreshUI()
	{
		_mStartPos = new Vector2((float)(Screen.width - 90) - MilMo_WorldMap.UI.GlobalPosOffset.x, -70f + MilMo_WorldMap.UI.GlobalPosOffset.y);
		_mEndPos = new Vector2((float)(Screen.width - 90) - MilMo_WorldMap.UI.GlobalPosOffset.x, MilMo_WorldMap.UI.GlobalPosOffset.y);
		SetPosition(_mEndPos);
		SetScale(104f, 104f);
		if (Enabled)
		{
			Open();
		}
	}

	public void Close()
	{
		SetPosition(_mStartPos);
		Enabled = false;
		if (base.Tooltip != null && base.Tooltip.Enabled)
		{
			base.Tooltip.Close();
		}
	}
}
