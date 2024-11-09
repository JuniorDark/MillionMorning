using System;
using System.Collections.Generic;
using System.Linq;
using Code.Apps.Fade;
using Code.Core.BuddyBackend;
using Code.Core.Command;
using Code.Core.Config;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.Input;
using Code.Core.Music;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.Core.Spline;
using Code.Core.Utility;
using Code.World.Feeds;
using Code.World.GUI;
using Code.World.GUI.HudCounter.Counters;
using Code.World.GUI.LoadingScreen;
using Code.World.GUI.Navigator;
using Code.World.Home;
using Code.World.Level;
using Code.World.Level.LevelInfo;
using Code.World.Player;
using Core;
using Core.GameEvent;
using Core.Input;
using Localization;
using Player;
using UI.HUD.Dialogues;
using UI.HUD.Dialogues.Modal;
using UI.LockState;
using UI.Sprites;
using UnityEngine;

namespace Code.World.WorldMap;

public sealed class MilMo_WorldMap : MonoBehaviour
{
	private sealed class LevelButton
	{
		public MilMo_Button Button;

		public MilMo_PlayerCountTag PlayerCountTag;

		public Vector2 Position;

		public string World;

		public string Level;

		public bool ImageLoaded;

		public bool GotPlayerCount;

		public MilMo_LevelInfoPopup LevelInfoPopup;

		public bool IsMoving;

		public Vector2 PosMoverVel;

		public Vector2 PosMoverReset;

		public Vector2 SinAmp;

		public Vector2 SinRate;

		public MilMo_Mover.UpdateFunc PosMoverFunc;

		public bool IsLooping;
	}

	private class WorldMapImage
	{
		public MilMo_Widget Image;

		public Vector2 Position;

		public Vector2 Scale;

		public bool IsMoving;

		public bool IsStatic;

		public float ScrollerMultiplier = 1f;

		public Vector2 PosMoverVel;

		public Vector2 PosMoverReset;

		public Vector2 PosMoverValue;

		public MilMo_Mover.UpdateFunc PosMoverFunc;

		public bool IsLooping;
	}

	private struct SplineStruct
	{
		public MilMo_Spline Spline;

		public string Level1;

		public string Level2;
	}

	public delegate void WorldMapReady(bool success);

	public const string UI_IDENTIFIER = "WorldMap";

	public const byte WORLD_MAP_REQUEST_SUCCESS = 0;

	public const byte WORLD_MAP_REQUEST_FAIL = 1;

	private const int LEVELBUTTON = -1111;

	private const int GRIDLINE = -3333;

	private const int UNLOCKME = -4444;

	private MilMo_Widget _returnToAdventureWidget;

	private static MilMo_ScrollView _scroller;

	private MilMo_Widget _scrollerOverlay;

	private MilMo_Widget _background;

	private MilMo_Widget _leftFogFade;

	private MilMo_Widget _rightFogFade;

	private MilMo_Widget _extraLetterboxTop;

	private MilMo_Widget _extraLetterboxBottom;

	private MilMo_Widget _extraLetterboxLeft;

	private MilMo_Widget _extraLetterboxRight;

	private MilMo_Widget _locationArrow;

	private MilMo_Widget _locationText;

	private MilMo_Widget _exitCorner;

	private readonly List<LevelButton> _levelButtons = new List<LevelButton>();

	private readonly List<WorldMapImage> _worldMapImages = new List<WorldMapImage>();

	private readonly MilMo_Button _test;

	private MilMo_Dialog _travelDialog;

	private MilMo_TeleportStoneCounter _teleportStoneCounter;

	private MilMo_LoadingPane _loadingPane;

	private MilMo_WorldMapMouseIcon _mouseHand;

	private const float NON_UNLOCKED_ALPHA = 0.3f;

	private readonly Vector2 _travelDialogPos = new Vector2(100f, 100f);

	private Vector2 _exitCornerPos;

	private Vector2 _lastFrameScrollValue = Vector2.zero;

	private static MilMo_WorldMap _theWorldMap;

	private static string _theWorld;

	private static string _lastAdventureLevel;

	private MilMo_UserInterface _ui;

	private readonly List<string> _recentlyUnlockedMaps = new List<string>();

	private static WorldMapReady _readyCallback;

	private int _newCount;

	private bool _hasUnlockDialog;

	private bool _haveMouseOverALevelInfoPopup;

	public int screenWidth = 1024;

	public int screenHeight = 720;

	private Vector2 _res;

	private MilMo_TimerEvent _arrowTimer;

	private readonly MilMo_AudioClip _softTickSound = new MilMo_AudioClip("Content/Sounds/Batch01/CharacterShop/SoftTick");

	private static GroupManager _groupManager;

	private const int SPLINEDOT = -2222;

	private readonly Vector2 _splineDotScaleLocked = new Vector2(12f, 12f);

	private readonly Vector2 _splineDotScaleUnlocked = new Vector2(16f, 16f);

	private readonly Color _splineDotColorLocked = new Color(1f, 1f, 1f, 0.15f);

	private readonly Color _splineDotColorUnlocked = new Color(1f, 1f, 1f, 0.4f);

	private readonly List<SplineStruct> _splineStructs = new List<SplineStruct>();

	private readonly List<MilMo_Spline> _recentlyUnlockedSplines = new List<MilMo_Spline>();

	private static bool _splineEditMode;

	private int _devSplinePoints = 6;

	private bool _showDevSplineInfo;

	private static MilMo_Spline _devSpline;

	private static MilMo_Widget _devSplineInfo;

	private static MilMo_Widget _devSplineStart;

	private static MilMo_Widget _devSplineEnd;

	private static MilMo_Widget _devSplineMid;

	private string _devSplineLevel1;

	private string _devSplineLevel2;

	private static MilMo_Player PlayerInstance => MilMo_Player.Instance;

	public static bool WasAutoOpened { get; private set; }

	public static bool WasTravelClosed { get; set; }

	public static bool WasGoToHubClosed { get; set; }

	public static string TravelClosedFullLevelName { get; set; }

	public static MilMo_UserInterface UI
	{
		get
		{
			if (!(_theWorldMap != null))
			{
				return null;
			}
			return _theWorldMap._ui;
		}
	}

	private static void Create()
	{
		if (!(_theWorldMap != null))
		{
			_groupManager = GroupManager.Get();
			MilMo_Command.Instance.RegisterCommand("WorldMap.SplineEdit", ToggleSplineEdit);
			Component component = MilMo_Global.MainGameObject.AddComponent<MilMo_WorldMap>();
			if (component == null)
			{
				Debug.LogWarning("Failed to create world map. Failed to add component MilMo_WorldMap.");
				return;
			}
			_theWorldMap = (MilMo_WorldMap)component;
			_theWorldMap.enabled = false;
			_theWorldMap._ui = MilMo_UserInterfaceManager.CreateUserInterface("WorldMap");
			_theWorldMap._ui.PrintMouseFocus = false;
			_theWorldMap.InitUI();
			_theWorldMap._ui.Enabled = false;
			MilMo_EventSystem.Listen("level_player_counts_info", _theWorldMap.GotLevelPlayerCounts).Repeating = true;
			MilMo_EventSystem.Listen("level_exp_received", _theWorldMap.ShowXpOnIsland).Repeating = true;
		}
	}

	public static void Activate(WorldMapReady callback, string world)
	{
		_theWorld = world;
		_readyCallback = callback;
		if (_theWorldMap == null)
		{
			Create();
		}
		_lastAdventureLevel = MilMo_LevelInfo.GetLastLevelInfoForWorld(world)?.FullLevelName;
		_theWorldMap.ActivateInternal();
		if (MilMo_Home.CurrentHome != null)
		{
			MilMo_Home.CurrentHome.PlayExitRoomSound();
		}
	}

	public static void Deactivate()
	{
		_theWorldMap.DeactivateInternal();
	}

	private void ActivateInternal()
	{
		if (MilMo_LevelInfo.GetWorldInfoData(_theWorld).WorldMapMusic != "")
		{
			MilMo_Music.Instance.FadeIn(MilMo_LevelInfo.GetWorldInfoData(_theWorld).WorldMapMusic);
		}
		_teleportStoneCounter.Disable();
		base.enabled = true;
		UpdateRes();
		SetUpImages();
		SetupSplines();
		SetupLevels();
		RefreshLevelStatus();
		_ui.Enabled = true;
		_readyCallback?.Invoke(success: true);
		_teleportStoneCounter.SetNumber("0");
		UpdateTeleportStones(null);
		_hasUnlockDialog = false;
		_returnToAdventureWidget.SetPosition(-500f, -500f);
		if (!WasAutoOpened)
		{
			MilMo_EventSystem.At(1.5f, delegate
			{
				_teleportStoneCounter.Open();
			});
			if (MilMo_Level.CurrentLevel == null || MilMo_Level.CurrentLevel.VerboseName != _lastAdventureLevel)
			{
				MilMo_EventSystem.At(0.6f, delegate
				{
					if (!string.IsNullOrEmpty(_lastAdventureLevel))
					{
						MilMo_LevelInfoData levelInfoData = MilMo_LevelInfo.GetLevelInfoData(_lastAdventureLevel);
						_returnToAdventureWidget.Children[0].SetText(MilMo_Localization.GetLocString("WorldMap_4758"));
						_returnToAdventureWidget.SetPosition(levelInfoData.WorldMapPositionRes(_res).x, levelInfoData.WorldMapPositionRes(_res).y + 15f);
						_returnToAdventureWidget.SetAlpha(0f);
						_returnToAdventureWidget.Enabled = true;
						_returnToAdventureWidget.AlphaTo(0.4f);
						if (GetLevelButton(_lastAdventureLevel) is MilMo_Button milMo_Button)
						{
							milMo_Button.Function = delegate
							{
								if (PlayerInstance.OkToLeaveNavigator())
								{
									MilMo_Level.LastAdventureLevel = _lastAdventureLevel;
									MilMo_World.CurrentWorld = _theWorld;
									WasTravelClosed = true;
									TravelClosedFullLevelName = MilMo_Level.LastAdventureLevel;
									PlayerInstance.RequestLeaveNavigator();
								}
							};
						}
					}
				});
			}
			if (Singleton<MilMo_BuddyBackend>.Instance.GetBuddies() != null && Singleton<MilMo_BuddyBackend>.Instance.GetBuddies().Count > 0)
			{
				MilMo_EventSystem.Instance.PostEvent("tutorial_OpenNavigatorAndHasHomeKey", "");
			}
		}
		UI.ScreenSizeDirty = true;
	}

	public static void AutoOpen()
	{
		if (!PlayerInstance.OkToEnterNavigator())
		{
			Debug.LogWarning("MilMo_WorldMap:AutoOpen failed: (OkToEnterWorldMap = false)");
			return;
		}
		MilMo_World.Deactivate();
		WasAutoOpened = true;
		PlayerInstance.RequestEnterNavigator();
	}

	public static void UserOpen()
	{
		if (MilMo_LoadingScreen.Instance.LoadingState == MilMo_LoadingScreen.State.Disabled)
		{
			if (!PlayerInstance.OkToEnterNavigator())
			{
				Debug.LogWarning("MilMo_WorldMap:UserOpen failed: (OkToEnterWorldMap = false)");
				return;
			}
			WasAutoOpened = false;
			PlayerInstance.RequestEnterNavigator();
		}
	}

	public static void UserCloseAndBackToGame()
	{
		if (!PlayerInstance.OkToLeaveNavigator())
		{
			return;
		}
		MilMo_EventSystem.At(0.0625f, delegate
		{
			if (_theWorldMap._travelDialog != null && _theWorldMap._travelDialog.IsActive)
			{
				_theWorldMap._travelDialog.CloseAndRemove(null);
			}
			MilMo_Fade.Instance.FadeInBackground();
			PlayerInstance.RequestLeaveNavigator();
		});
	}

	private void RefreshLevelStatus()
	{
		bool flag = false;
		RefreshPreviousSplines();
		foreach (MilMo_LevelInfoData item in MilMo_LevelInfo.GetLevelsInWorld(_theWorld))
		{
			if (item.IsUnlocked && !item.IsGUIUnlocked)
			{
				GUIUnlock(item);
				flag = true;
			}
		}
		ScrollerSetStartPosition();
		HideCompass();
		if (!WasAutoOpened)
		{
			MilMo_EventSystem.At(1f, ShowCompass);
		}
		if (flag)
		{
			ScrollerSetUnlockSpeed();
			LockInput();
			UnlockNextLevel();
		}
		else
		{
			UnlockInput();
			ScrollToLevelButton(_lastAdventureLevel);
		}
		RefreshLocationArrow();
	}

	private static void GUIUnlock(MilMo_LevelInfoData levelInfoData)
	{
		levelInfoData.UnlockGUI();
		MilMo_Widget levelButton = GetLevelButton(levelInfoData.FullLevelName);
		if (levelButton != null && levelButton.Info == -1111)
		{
			levelButton.Info2 = -4444;
		}
	}

	private void ShowXpOnIsland(object o)
	{
		ServerLevelExpRecieved message = o as ServerLevelExpRecieved;
		if (message == null || !PlayerInstance.InNavigator)
		{
			return;
		}
		using IEnumerator<Vector2> enumerator = (from button in _levelButtons
			where button.World + ":" + button.Level == message.getFullLevelName()
			select button.Position into position
			select position + _scroller.Pos).GetEnumerator();
		if (enumerator.MoveNext())
		{
			Vector2 current = enumerator.Current;
			current.x -= _scroller.SoftScroll.Val.x;
		}
	}

	private void UnlockNextLevel()
	{
		bool found = false;
		foreach (MilMo_Widget level in _scroller.Children.Where((MilMo_Widget level) => !found && level.Info == -1111 && level.Info2 == -4444))
		{
			found = true;
			if (MilMo_LevelInfo.GetWorldInfoData(_lastAdventureLevel.Split(':')[0]).FixedPosition)
			{
				FadeInLevelButton((MilMo_Button)level);
				UnlockNextSpline();
			}
			else
			{
				_scroller.ScrollTo(level.PosMover.Target.x / _ui.Res.x - (float)screenWidth / 2f, 0f);
				_scroller.SoftScroll.Arrive = delegate
				{
					FadeInLevelButton((MilMo_Button)level);
					UnlockNextSpline();
				};
			}
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Swap);
			_recentlyUnlockedMaps.Add(level.Identifier);
			level.Info2 = 0;
		}
		if (!found)
		{
			ScrollToLevelButton(_lastAdventureLevel);
		}
	}

	private static void HideCompass()
	{
		MilMo_GlobalUI.Instance.CloseItem("NavigatorMenu");
	}

	private static void ShowCompass()
	{
		MilMo_GlobalUI.Instance.OpenItem("NavigatorMenu");
	}

	private void FadeInLevelButton(MilMo_Widget button)
	{
		button.FadeAlpha(0.3f, 1f);
		_scroller.SoftScroll.Arrive = null;
		MilMo_EventSystem.At(1f, UnlockNextLevel);
	}

	public static void HideArrow()
	{
		_theWorldMap._locationArrow.SetAlpha(0f);
	}

	public static void ShowArrow()
	{
		_theWorldMap._locationArrow.SetAlpha(1f);
	}

	private void MoveArrowToLevelButton(MilMo_Widget levelButton)
	{
		if (levelButton != null)
		{
			_locationArrow.GoTo(levelButton.PosMover.Target.x / _ui.Res.x, levelButton.PosMover.Target.y / _ui.Res.y - 30f);
		}
		_locationText.SetEnabled(e: false);
		if (!WasAutoOpened)
		{
			return;
		}
		MilMo_EventSystem.At(3f, delegate
		{
			if (!_hasUnlockDialog)
			{
				if (MilMo_Level.CurrentLevel != null)
				{
					MilMo_GameDialogCreator.LevelUnlocked(MilMo_Level.CurrentLevel, _ui, null);
					MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_UnlockLevel", MilMo_Level.CurrentLevel.VerboseName);
				}
				_hasUnlockDialog = true;
				MilMo_GameDialogCreator.AddLastDialogClosedCallback(_ui, callImmediatelyIfNoDialog: false, UserCloseAndBackToGame);
			}
		});
	}

	private void ScrollToLevelButton(string fullLevelName)
	{
		if (string.IsNullOrEmpty(fullLevelName) || !MilMo_LevelInfo.GetWorldInfoData(fullLevelName.Split(':')[0]).FixedPosition)
		{
			ScrollerSetDefaultSpeed();
			MilMo_Widget milMo_Widget = _scroller.Children.FirstOrDefault((MilMo_Widget level) => level.Identifier == _lastAdventureLevel);
			if (milMo_Widget != null)
			{
				_scroller.ScrollToNow(milMo_Widget.PosMover.Target.x / _ui.Res.x - (float)screenWidth / 2f, 0f);
			}
			MilMo_Widget milMo_Widget2 = _scroller.Children.FirstOrDefault((MilMo_Widget level) => level.Identifier == fullLevelName);
			if (milMo_Widget2 != null)
			{
				_scroller.ScrollTo(milMo_Widget2.PosMover.Target.x / _ui.Res.x - (float)screenWidth / 2f, 0f);
			}
			_scroller.SoftScroll.Arrive = null;
			if (!WasAutoOpened)
			{
				UnlockInput();
			}
		}
	}

	private static void LockInput()
	{
		_scroller.IsUserControlled(b: false);
	}

	private static void UnlockInput()
	{
		_scroller.IsUserControlled(b: true);
	}

	private void SetUpImages()
	{
		RemoveGrid();
		SetupGrid();
		foreach (WorldMapImage worldMapImage2 in _worldMapImages)
		{
			_scroller.Children.Remove(worldMapImage2.Image);
		}
		_worldMapImages.Clear();
		_scrollerOverlay.RemoveAllChildren();
		foreach (MilMo_WorldImageInfoData item in MilMo_LevelInfo.GetImagesInWorld(_theWorld))
		{
			WorldMapImage worldMapImage = new WorldMapImage
			{
				Image = new MilMo_Widget(UI)
				{
					AllowPointerFocus = false
				},
				Scale = item.Scale
			};
			worldMapImage.Scale.x *= _res.x;
			worldMapImage.Scale.y *= _res.y;
			worldMapImage.IsStatic = item.IsStatic;
			LoadAndSetLevelImageAsync("Content/Worlds/" + item.World.Replace("orld", "") + "/WorldMapImages/" + item.Texture, item, worldMapImage);
			worldMapImage.Image.SetAlignment(MilMo_GUI.Align.CenterCenter);
			worldMapImage.Position = item.Position;
			worldMapImage.Position.x *= _res.x;
			worldMapImage.Position.y *= _res.y;
			if (item.PosMover != 0)
			{
				worldMapImage.IsMoving = true;
				worldMapImage.PosMoverFunc = item.PosMover;
				worldMapImage.PosMoverReset = item.PosMoverLoopReset;
				worldMapImage.PosMoverReset.x *= _res.x;
				worldMapImage.PosMoverReset.y *= _res.y;
				worldMapImage.PosMoverVel = item.PosMoverVel;
				worldMapImage.IsLooping = item.PosMoverLooping;
				worldMapImage.PosMoverValue = item.PosMoverLoopValue;
				worldMapImage.PosMoverValue.x *= _res.x;
				worldMapImage.PosMoverValue.y *= _res.y;
				worldMapImage.ScrollerMultiplier = item.ScrollerMultiplier;
			}
			_worldMapImages.Add(worldMapImage);
			if (worldMapImage.IsStatic)
			{
				_scrollerOverlay.AddChild(worldMapImage.Image);
			}
			else
			{
				_scroller.AddChild(worldMapImage.Image);
			}
			worldMapImage.Image.Enabled = true;
		}
		_background.SetDefaultColor(MilMo_LevelInfo.GetWorldInfoData(_theWorld).BackgroundColor);
		if (_theWorld == "World01")
		{
			_leftFogFade.Enabled = true;
			_rightFogFade.Enabled = true;
		}
		else
		{
			_leftFogFade.Enabled = false;
			_rightFogFade.Enabled = false;
		}
	}

	private static async void LoadAndSetLevelImageAsync(string iconPath, MilMo_WorldImageInfoData image, WorldMapImage wmi)
	{
		Texture2D texture = await MilMo_ResourceManager.Instance.LoadTextureAsync(iconPath, "LevelIcon", image.Priority);
		wmi.Image.SetTexture(new MilMo_Texture(texture));
		if (wmi.Scale != Vector2.zero)
		{
			wmi.Image.SetScale(wmi.Scale);
		}
		else
		{
			wmi.Image.SetScaleToTexture();
		}
		wmi.Image.SetDefaultColor(1f, 1f, 1f, 1f);
		wmi.Image.SetAlpha(0f);
	}

	private void SetupLevels()
	{
		IEnumerable<MilMo_LevelInfoData> levelsInWorld = MilMo_LevelInfo.GetLevelsInWorld(_theWorld);
		RemoveLevelButtons();
		foreach (MilMo_LevelInfoData item in levelsInWorld)
		{
			MilMo_LevelInfoData data = item;
			if (data.IsChatRoom)
			{
				continue;
			}
			MilMo_Button level = new MilMo_Button(_ui);
			MilMo_PlayerCountTag milMo_PlayerCountTag = new MilMo_PlayerCountTag(_ui);
			milMo_PlayerCountTag.SetInvisible();
			milMo_PlayerCountTag.Enabled = false;
			milMo_PlayerCountTag.SetPosition(data.WorldMapPositionRes(_res));
			if (data.IsGUIUnlocked || !data.InvisibleIfUnlocked)
			{
				_scroller.AddChild(milMo_PlayerCountTag);
			}
			LevelButton levelButton = new LevelButton
			{
				Button = level,
				PlayerCountTag = milMo_PlayerCountTag,
				World = data.World,
				Level = data.Level,
				Position = data.WorldMapPositionRes(_res)
			};
			levelButton.Position.x -= 20f * _res.x;
			levelButton.Position.y += 10f * _res.y;
			if (data.PosMoveFunc != 0)
			{
				levelButton.IsMoving = true;
				levelButton.PosMoverFunc = data.PosMoveFunc;
				levelButton.PosMoverReset = data.PosMoveLoopReset;
				levelButton.PosMoverVel = data.PosMoveVel;
				levelButton.IsLooping = data.PosMoveLooping;
				levelButton.SinAmp = data.SinAmp;
				levelButton.SinRate = data.SinRate;
				if (data.PosMoveFunc == MilMo_Mover.UpdateFunc.Sinus)
				{
					level.SetPosPull(0f, 0f);
					level.SetPosDrag(0f, 0f);
				}
			}
			string text = data.World.Replace("orld", "");
			string text2 = data.Level.Replace("evel", "");
			string iconTexture = "Content/Worlds/" + text + "/LevelIcons/LevelIcon" + text + text2;
			levelButton.LevelInfoPopup = new MilMo_LevelInfoPopup(_ui, data.DisplayName, data.WorldMapDescription, 265f, iconTexture, data.IsUnlocked, data.ExplorationTokens, data.CoinTokens, data.PremiumToken, data.IsMembersOnlyArea, data.RequiredAvatarLevel);
			levelButton.LevelInfoPopup.TelepodToButton.Function = delegate
			{
				if (!data.InvisibleIfUnlocked || data.IsGUIUnlocked)
				{
					if (!_splineEditMode)
					{
						ShowTravelDialog(data);
					}
					else if (InputSwitch.GetKey(KeyCode.LeftControl))
					{
						_devSplineLevel2 = _devSplineLevel1;
						_devSplineLevel1 = level.Identifier;
					}
					levelButton.LevelInfoPopup.HideLevelInfoPopup();
					_haveMouseOverALevelInfoPopup = false;
				}
			};
			UI.AddChild(levelButton.LevelInfoPopup);
			levelButton.Button.PointerHoverFunction = delegate
			{
				if ((!data.InvisibleIfUnlocked || data.IsGUIUnlocked) && !_haveMouseOverALevelInfoPopup && !WasAutoOpened && (_travelDialog == null || !_travelDialog.IsActive))
				{
					levelButton.LevelInfoPopup.ShowLevelInfoPopup(data.FullLevelName == _lastAdventureLevel);
				}
			};
			float num = (data.IsGUIUnlocked ? 1f : ((!data.InvisibleIfUnlocked) ? 0.3f : 0f));
			float alpha = num;
			LoadAndSetTexture(GetImagePath(data.FullLevelName), levelButton, alpha);
			level.SetPosition(data.WorldMapPositionRes(_res));
			level.Identifier = data.FullLevelName;
			level.Function = delegate
			{
				if (!data.InvisibleIfUnlocked || data.IsGUIUnlocked)
				{
					levelButton.LevelInfoPopup.HideLevelInfoPopup();
					_haveMouseOverALevelInfoPopup = false;
					if (data.FullLevelName == _lastAdventureLevel)
					{
						UserCloseAndBackToGame();
					}
					else if (!_splineEditMode)
					{
						ShowTravelDialog(data);
					}
					else if (InputSwitch.GetKey(KeyCode.LeftControl))
					{
						_devSplineLevel2 = _devSplineLevel1;
						_devSplineLevel1 = level.Identifier;
					}
				}
			};
			level.SetFont(MilMo_GUI.Font.EborgSmall);
			level.Info = -1111;
			_scroller.AddChild(level);
			if (_worldMapImages.Count > 0)
			{
				level.SetAlpha(0f);
				MilMo_EventSystem.At(1f, delegate
				{
					level.AlphaTo(1f);
				});
			}
			_levelButtons.Add(levelButton);
		}
		UpdatePlayerCountTags();
		BringGridLinesToFront();
	}

	private void RemoveLevelButtons()
	{
		foreach (LevelButton levelButton in _levelButtons)
		{
			_scroller.Children.Remove(levelButton.Button);
			_scroller.Children.Remove(levelButton.PlayerCountTag);
			UI.Children.Remove(levelButton.LevelInfoPopup);
		}
		_levelButtons.Clear();
	}

	private void RemoveImages()
	{
		foreach (WorldMapImage worldMapImage in _worldMapImages)
		{
			_scroller.Children.Remove(worldMapImage.Image);
		}
		_worldMapImages.Clear();
	}

	private void ShowTravelDialog(MilMo_LevelInfoData data)
	{
		if (WasAutoOpened)
		{
			return;
		}
		MilMo_Dialog travelDialog = _travelDialog;
		if (travelDialog != null && travelDialog.IsActive)
		{
			_travelDialog.CloseAndRemove(null);
		}
		if (PlayerInstance.AvatarLevel < data.RequiredAvatarLevel)
		{
			MilMo_Dialog tooLowLevelDialog = new MilMo_Dialog(MilMo_GlobalUI.GetSystemUI);
			MilMo_GlobalUI.GetSystemUI.AddChild(tooLowLevelDialog);
			MilMo_RequiredLevelTag milMo_RequiredLevelTag = new MilMo_RequiredLevelTag(MilMo_GlobalUI.GetSystemUI, data.RequiredAvatarLevel);
			milMo_RequiredLevelTag.ScaleByFactor(2.5f);
			MilMo_LocString copy = MilMo_Localization.GetLocString("WorldMap_13179").GetCopy();
			copy.SetFormatArgs(data.RequiredAvatarLevel);
			tooLowLevelDialog.DoOK(milMo_RequiredLevelTag, MilMo_Localization.GetLocString("WorldMap_13178"), copy, delegate
			{
				tooLowLevelDialog.Close(null);
			});
		}
		else if (data.IsMembersOnlyArea && !PlayerInstance.IsMember)
		{
			DialogueSpawner.SpawnGoToShopDialogue(new EnterShopModalMessageData(new LocalizedStringWithArgument("World_7628"), new LocalizedStringWithArgument("World_7629"), "Shop:Batch01.Subscriptions.SubscriptionSixMonths", new AddressableSpriteLoader("IconPremium")));
		}
		else if (data.IsUnlocked)
		{
			if (data.FullLevelName != _lastAdventureLevel)
			{
				if (PlayerInstance.Avatar.TeleportStones > 0 || (PlayerInstance.Avatar.Role != 0 && !_groupManager.InGroup(PlayerInstance.Avatar.Id)))
				{
					MoveArrowToLevelButton((MilMo_Button)GetLevelButton(data.FullLevelName));
					ScrollToLevelButton(data.FullLevelName);
					_travelDialog = new MilMo_Dialog(_ui);
					MilMo_LocString locString = MilMo_Localization.GetLocString("WorldMap_28");
					MilMo_LocString locString2 = MilMo_Localization.GetLocString("WorldMap_29");
					locString2.SetFormatArgs(data.DisplayNameUpperCase);
					_travelDialog.DoYesNo(GetLevelIconPath(data.FullLevelName), locString, locString2, delegate
					{
						UpdateTeleportStones(null);
						if (PlayerInstance.Avatar.Role == 0 || _groupManager.InGroup(PlayerInstance.Avatar.Id))
						{
							_teleportStoneCounter.SetNumber((_newCount - 1).ToString());
						}
						MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Confirm);
						_travelDialog.CloseAndRemove(null);
						MilMo_EventSystem.At(1f, delegate
						{
							PlayerInstance.RequestLeaveNavigator();
							WasTravelClosed = true;
							TravelClosedFullLevelName = data.FullLevelName;
						});
					}, delegate
					{
						MoveArrowToLevelButton((MilMo_Button)GetLevelButton(_lastAdventureLevel));
						_travelDialog.CloseAndRemove(null);
					});
					_travelDialog.SetPosition(_travelDialogPos);
					_ui.AddChild(_travelDialog);
					string levelIconPath = GetLevelIconPath(data.FullLevelName);
					LoadAndSetTravelDialogueIconAsync(levelIconPath);
					MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Select);
				}
				else
				{
					_teleportStoneCounter.ShakeInRed();
					_travelDialog = new MilMo_Dialog(_ui);
					MilMo_LocString locString3 = MilMo_Localization.GetLocString("WorldMap_360");
					MilMo_LocString locString4 = MilMo_Localization.GetLocString("WorldMap_361");
					locString4.SetFormatArgs(data.DisplayNameUpperCase);
					_travelDialog.DoOK("Content/Items/Batch01/SpecialItems/IconTeleportStone", locString3, locString4, delegate
					{
						_travelDialog.CloseAndRemove(null);
					});
					_travelDialog.SetPosition(_travelDialogPos);
					_ui.AddChild(_travelDialog);
					string path = "Content/Items/Batch01/SpecialItems/IconTeleportStone";
					LoadAndSetTravelDialogueIconAsync(path);
					MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Info);
				}
			}
			else
			{
				MoveArrowToLevelButton((MilMo_Button)GetLevelButton(_lastAdventureLevel));
			}
		}
		else
		{
			_travelDialog = new MilMo_Dialog(_ui);
			MilMo_LocString locString5 = MilMo_Localization.GetLocString("WorldMap_362");
			MilMo_LocString locString6 = MilMo_Localization.GetLocString("WorldMap_363");
			locString6.SetFormatArgs(data.DisplayNameUpperCase);
			_travelDialog.DoOK(GetLevelIconPath(data.FullLevelName), locString5, locString6, delegate
			{
				_travelDialog.CloseAndRemove(null);
			});
			_travelDialog.SetPosition(_travelDialogPos);
			_ui.AddChild(_travelDialog);
			string levelIconPath2 = GetLevelIconPath(data.FullLevelName);
			LoadAndSetTravelDialogueIconAsync(levelIconPath2);
			MoveArrowToLevelButton((MilMo_Button)GetLevelButton(_lastAdventureLevel));
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Info);
		}
	}

	private async void LoadAndSetTravelDialogueIconAsync(string path)
	{
		Texture2D texture = await MilMo_ResourceManager.Instance.LoadTextureAsync(path, "LevelIcon");
		_travelDialog.Icon.SetTexture(new MilMo_Texture(texture));
		_travelDialog.Icon.SetDefaultColor(1f, 1f, 1f, 1f);
		_travelDialog.Icon.SetPosition(65f, _travelDialog.Icon.PosMover.Target.y / _ui.Res.y + 10f);
		_travelDialog.Icon.SetScale(0f, 0f);
		_travelDialog.Icon.SetAlignment(MilMo_GUI.Align.CenterCenter);
		_travelDialog.Icon.SetScale(0f, 0f);
		_travelDialog.Icon.SetAngle(-90f);
		_travelDialog.Icon.Angle(0f);
		_travelDialog.Icon.ScaleTo(100f, 100f);
		_travelDialog.Icon.AngleMover.Pull.x = 0.09f;
		_travelDialog.Icon.AngleMover.Drag.x = 0.45f;
	}

	private void RefreshLocationArrow()
	{
		_scroller.BringToFront(_locationText);
		_scroller.BringToFront(_locationArrow);
		if (!MilMo_Config.Instance.IsTrue("Launcher.OfflineMap", defaultValue: false))
		{
			foreach (MilMo_Widget level in _scroller.Children.Where((MilMo_Widget level) => level.Identifier == _lastAdventureLevel))
			{
				if (_arrowTimer != null)
				{
					MilMo_EventSystem.RemoveTimerEvent(_arrowTimer);
				}
				_arrowTimer = MilMo_EventSystem.At(0f, delegate
				{
					_locationArrow.SetPosition(level.PosMover.Target.x / _ui.Res.x, level.PosMover.Target.y / _ui.Res.y - 1000f);
					_locationText.SetPosition(level.PosMover.Target.x / _ui.Res.x, level.PosMover.Target.y / _ui.Res.y + 75f);
					MoveArrowToLevelButton((MilMo_Button)level);
				});
			}
			return;
		}
		MilMo_EventSystem.At(5f, delegate
		{
			if (_test != null)
			{
				_locationArrow.SetPosition(_test.PosMover.Target.x / _ui.Res.x, _test.PosMover.Target.y / _ui.Res.y - 1000f);
				_locationText.SetPosition(_test.PosMover.Target.x / _ui.Res.x, _test.PosMover.Target.y / _ui.Res.y - 1000f);
				MoveArrowToLevelButton(_test);
			}
		});
	}

	private void DeactivateInternal()
	{
		RemoveImages();
		RemoveLevelButtons();
		RemoveSplines();
		_ui.Enabled = false;
		base.enabled = false;
		_recentlyUnlockedMaps.Clear();
		_recentlyUnlockedSplines.Clear();
		HideCompass();
	}

	private void Update()
	{
		try
		{
			bool flag = false;
			foreach (LevelButton levelButton in _levelButtons)
			{
				levelButton.PlayerCountTag.SetPosition(levelButton.Position);
				if (levelButton.LevelInfoPopup.Enabled)
				{
					flag = true;
					Vector2 zero = Vector2.zero;
					zero += _scroller.Pos;
					zero.x -= _scroller.SoftScroll.Val.x;
					zero += levelButton.PlayerCountTag.Pos;
					zero.x -= levelButton.LevelInfoPopup.InsideWidget.Scale.x * 0.5f;
					zero.y -= levelButton.LevelInfoPopup.InsideWidget.Scale.y;
					zero.x += 10f;
					levelButton.LevelInfoPopup.SetPosition(zero);
					if (zero.x < 7f)
					{
						levelButton.LevelInfoPopup.MoveWhenOutside(7f, levelButton.LevelInfoPopup.Pos.y);
					}
					if (zero.x + levelButton.LevelInfoPopup.InsideWidget.Scale.x > (float)Screen.width)
					{
						levelButton.LevelInfoPopup.MoveWhenOutside((float)Screen.width - (levelButton.LevelInfoPopup.InsideWidget.Scale.x + 14f), levelButton.LevelInfoPopup.Pos.y);
					}
					if (zero.y < 84f * UI.Res.y)
					{
						levelButton.LevelInfoPopup.MoveWhenOutside(levelButton.LevelInfoPopup.Pos.x, 84f * UI.Res.y);
					}
					_haveMouseOverALevelInfoPopup = true;
					if (levelButton.Button.Hover() || levelButton.LevelInfoPopup.Hover())
					{
						continue;
					}
					bool flag2 = false;
					foreach (MilMo_Widget item in levelButton.LevelInfoPopup.Children.Where((MilMo_Widget t) => t.Hover()))
					{
						_ = item;
						flag2 = true;
					}
					if (!flag2)
					{
						_haveMouseOverALevelInfoPopup = false;
						levelButton.LevelInfoPopup.HideLevelInfoPopup();
					}
				}
				else if (levelButton.Button.Hover() && !levelButton.LevelInfoPopup.ShouldOpen)
				{
					levelButton.Button.PointerHoverFunction();
				}
				else if (!levelButton.Button.Hover() && levelButton.LevelInfoPopup.ShouldOpen)
				{
					levelButton.LevelInfoPopup.ShouldOpen = false;
				}
			}
			if (_locationText.PosMover.Val.y < 150f)
			{
				_locationText.PosMover.Val.y = 150f;
			}
			if (_scroller.ContainsMouse() && !flag && !WasAutoOpened && (_scroller.VerticalDrag || _scroller.VerticalDrag))
			{
				_mouseHand.SetEnabled(e: true);
			}
			else
			{
				_mouseHand.SetEnabled(e: false);
			}
			Vector2 vector = new Vector2(_scroller.SoftScroll.Val.x / _ui.Res.x, _scroller.SoftScroll.Val.y / _ui.Res.y);
			foreach (WorldMapImage worldMapImage in _worldMapImages)
			{
				if (worldMapImage.IsStatic && worldMapImage.IsMoving)
				{
					Vector2 vector2 = (vector - _lastFrameScrollValue) * worldMapImage.ScrollerMultiplier;
					if (worldMapImage.Image.PosMover.Val.x - vector2.x > worldMapImage.Image.PosMover.LoopReset.x)
					{
						float num = worldMapImage.Image.PosMover.LoopReset.x - (worldMapImage.Image.PosMover.Val.x - vector2.x);
						worldMapImage.Image.PosMover.Val.x = worldMapImage.Image.PosMover.LoopVal.x - num;
					}
					else if (worldMapImage.Image.PosMover.Val.x - vector2.x < worldMapImage.Image.PosMover.LoopVal.x)
					{
						float num2 = worldMapImage.Image.PosMover.LoopVal.x - (worldMapImage.Image.PosMover.Val.x - vector2.x);
						worldMapImage.Image.PosMover.Val.x = worldMapImage.Image.PosMover.LoopReset.x - num2;
					}
					else
					{
						worldMapImage.Image.PosMover.Val.x -= vector2.x;
					}
				}
			}
			if (_ui != null && _ui.ScreenSizeDirty)
			{
				RefreshUI();
			}
			if (vector.x + _scrollerOverlay.Scale.x >= _scroller.MViewSize.width / _ui.Res.x)
			{
				float x = _scroller.MViewSize.width / _ui.Res.x - (float)screenWidth - 10f;
				_scroller.ScrollToNow(x, 0f);
			}
			else if (vector.x <= 0f)
			{
				_scrollerOverlay.SetPosition(0f, 0f);
			}
			else
			{
				_scrollerOverlay.SetPosition(vector);
			}
			if (_splineEditMode)
			{
				DoDevSplineInput();
			}
			DoExitCornerVisibility();
			_lastFrameScrollValue = new Vector2(_scroller.SoftScroll.Val.x / _ui.Res.x, _scroller.SoftScroll.Val.y / _ui.Res.y);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}

	private void RefreshUI()
	{
		UpdateRes();
		((MilMo_NavigatorMenu)MilMo_GlobalUI.Instance.GetItem("NavigatorMenu")).RefreshUI();
		MilMo_LevelInfoData levelInfoData = MilMo_LevelInfo.GetLevelInfoData(_lastAdventureLevel);
		MilMo_WorldInfoData worldInfoData = MilMo_LevelInfo.GetWorldInfoData(_theWorld);
		_exitCornerPos = new Vector2((float)screenWidth * _ui.Res.x - 4f, 6f + 72f * _ui.Res.y);
		_ui.BringToFront(_exitCorner);
		_teleportStoneCounter.SpawnPosition = new Vector2(-180f, 152f);
		_teleportStoneCounter.TargetPosition = new Vector2(30f, 152f);
		_teleportStoneCounter.SetPosition(_teleportStoneCounter.TargetPosition);
		_background.SetPosition(0f, 0f + 72f * _res.y);
		_background.SetScale(screenWidth, screenHeight);
		float num = (((float)screenWidth < worldInfoData.ViewSize.x * _res.x) ? 0f : ((float)screenWidth * 0.5f - worldInfoData.ViewSize.x * _res.x * 0.5f));
		_extraLetterboxTop.SetPosition(0f, 0f);
		_extraLetterboxTop.SetScale(screenWidth, 72f * _res.y);
		_extraLetterboxBottom.SetPosition(0f, (float)screenHeight - 72f * _res.y);
		_extraLetterboxBottom.SetScale(screenWidth, 72f * _res.y);
		_extraLetterboxLeft.SetPosition(0f, 0f);
		_extraLetterboxLeft.SetScale(num, screenHeight);
		_extraLetterboxRight.SetPosition((float)screenWidth - num, 0f);
		_extraLetterboxRight.SetScale(num, screenHeight);
		_scroller.SetPosition(num, 0f + 72f * _res.y);
		_scrollerOverlay.SetPosition(num, 0f);
		_scroller.SetScale((float)screenWidth - num * 2f, (float)screenHeight - 144f * _res.y);
		_scrollerOverlay.SetScale((float)screenWidth - num * 2f, (float)screenHeight - 144f * _res.y);
		_scroller.SetViewSize(new Vector2(worldInfoData.ViewSize.x * _res.x, worldInfoData.ViewSize.y * _res.y));
		_scroller.HorizontalDrag = false;
		_scroller.VerticalDrag = false;
		_scroller.ScrollBarCompensate.x = 0f;
		_scroller.ScrollBarCompensate.y = 0f;
		switch (worldInfoData.ScrollMode.ToUpper())
		{
		case "BOTH":
			_scroller.VerticalDrag = true;
			_scroller.HorizontalDrag = true;
			_scroller.ScrollBarCompensate.x = 20f;
			_scroller.ScrollBarCompensate.y = 19f;
			break;
		case "HORIZ":
			_scroller.VerticalDrag = false;
			_scroller.HorizontalDrag = true;
			_scroller.ScrollBarCompensate.x = 20f;
			_scroller.ScrollBarCompensate.y = 19f;
			break;
		case "VERT":
			_scroller.VerticalDrag = true;
			_scroller.HorizontalDrag = false;
			_scroller.ScrollBarCompensate.x = 20f;
			_scroller.ScrollBarCompensate.y = 19f;
			break;
		}
		foreach (MilMo_LevelInfoData item in MilMo_LevelInfo.GetLevelsInWorld(_theWorld))
		{
			if (!item.IsChatRoom)
			{
				MilMo_Widget levelButton = GetLevelButton(item.FullLevelName);
				levelButton.SetPosition(item.WorldMapPositionRes(_res));
				levelButton.Pos.x -= 20f * _res.x;
				levelButton.Pos.y += 10f * _res.y;
				Texture texture = levelButton.Texture.Texture;
				if ((bool)texture)
				{
					levelButton.SetScale((float)texture.width * _res.x, (float)texture.height * _res.y);
				}
			}
		}
		MoveArrowToLevelButton((MilMo_Button)GetLevelButton(_lastAdventureLevel));
		RemoveSplines();
		SetupSplines();
		RefreshPreviousSplines();
		foreach (WorldMapImage worldMapImage in _worldMapImages)
		{
			if (worldMapImage.Scale != Vector2.zero)
			{
				worldMapImage.Image.SetScale(worldMapImage.Scale);
			}
			else
			{
				worldMapImage.Image.SetScaleToTexture();
			}
			worldMapImage.Image.SetPosition(worldMapImage.Position);
			if (worldMapImage.IsMoving)
			{
				worldMapImage.Image.PosMover.LoopVal = new Vector2(worldMapImage.PosMoverValue.x * _ui.Res.x, worldMapImage.PosMoverValue.y * _ui.Res.y);
				worldMapImage.Image.PosMover.LoopReset = new Vector2(worldMapImage.PosMoverReset.x * _ui.Res.x, worldMapImage.PosMoverReset.y * _ui.Res.y);
				worldMapImage.Image.Pos = new Vector2(worldMapImage.Position.x * _ui.Res.x, worldMapImage.Position.y * _ui.Res.y);
				worldMapImage.Image.PosMover.SetUpdateFunc(worldMapImage.PosMoverFunc);
				worldMapImage.Image.PosMover.Vel = worldMapImage.PosMoverVel;
				worldMapImage.Image.PosMover.Looping = worldMapImage.IsLooping;
			}
		}
		foreach (LevelButton levelButton2 in _levelButtons)
		{
			levelButton2.PlayerCountTag.SetPosition(levelButton2.Position);
			levelButton2.PlayerCountTag.RefreshPositionAndScale();
			if (levelButton2.IsMoving)
			{
				levelButton2.Button.PosMover.SetUpdateFunc(levelButton2.PosMoverFunc);
				levelButton2.Button.PosMover.Vel = levelButton2.PosMoverVel;
				levelButton2.Button.PosMover.LoopReset = levelButton2.PosMoverReset;
				levelButton2.Button.PosMover.LoopVal = levelButton2.Position;
				levelButton2.Button.PosMover.Looping = levelButton2.IsLooping;
				levelButton2.Button.PosMover.SinAmp = levelButton2.SinAmp;
				levelButton2.Button.PosMover.SinRate = levelButton2.SinRate;
			}
		}
		_locationArrow.SetScale(50f, 50f);
		_locationArrow.SetScaleType(MilMo_Mover.UpdateFunc.Sinus);
		RefreshLocationArrow();
		if (!WasAutoOpened && (MilMo_Level.CurrentLevel == null || MilMo_Level.CurrentLevel.VerboseName != _lastAdventureLevel) && levelInfoData != null)
		{
			_returnToAdventureWidget.SetPosition(levelInfoData.WorldMapPositionRes(_res).x, levelInfoData.WorldMapPositionRes(_res).y + 15f);
		}
		_leftFogFade.SetPosition(0f, 0f + 72f * _res.y);
		_leftFogFade.SetScale(256f, (float)screenHeight - 144f * _res.y);
		_rightFogFade.SetPosition(screenWidth, 0f + 72f * _res.y);
		_rightFogFade.SetScale(-256f, (float)screenHeight - 144f * _res.y);
		_exitCorner.SetPosition(_exitCornerPos);
		_exitCorner.SetScale(75f, 50f);
		RemoveGrid();
		SetupGrid();
		Vector2 loadingPanePosition = MilMo_LevelInfo.GetWorldInfoData(_theWorld).LoadingPanePosition;
		_loadingPane?.SetPosition((float)screenWidth * _ui.Res.x + loadingPanePosition.x, (float)screenHeight * _ui.Res.y - 72f * _ui.Res.y + loadingPanePosition.y);
		_ui.ScreenSizeDirty = false;
	}

	private void UpdateRes()
	{
		screenWidth = ((Screen.width <= 1024 || Screen.height <= 720) ? 1024 : Screen.width);
		screenHeight = ((Screen.width <= 1024 || Screen.height <= 720) ? 720 : Screen.height);
		Vector2 vector = new Vector2(Mathf.Max((float)Screen.width / 1024f, 1f), Mathf.Max((float)Screen.height / 720f, 1f));
		if (screenWidth <= 1024 || screenHeight <= 720)
		{
			screenWidth = 1024;
			screenHeight = 720;
			_ui.OffsetMode = true;
			_ui.UpdateGlobalOffset();
			_ui.ResetLayout(10f, 10f);
		}
		else
		{
			_ui.OffsetMode = false;
			_ui.SetGlobalOffset(new Vector2(0f, 0f));
			_ui.ResetLayout(10f, 10f);
		}
		_res = new Vector2(Mathf.Min(vector.x, vector.y), Mathf.Min(vector.x, vector.y));
	}

	private void InitUI()
	{
		UpdateRes();
		MilMo_NavigatorMenu milMo_NavigatorMenu = new MilMo_NavigatorMenu();
		MilMo_GlobalUI.Instance.AddItem(milMo_NavigatorMenu, "NavigatorMenu", milMo_NavigatorMenu.Open, milMo_NavigatorMenu.Close);
		_background = new MilMo_Widget(_ui);
		_background.SetTexture("Batch01/Textures/Core/White");
		_background.SetDefaultColor(0f, 0f, 0f, 1f);
		_background.SetAlignment(MilMo_GUI.Align.TopLeft);
		_background.IgnoreGlobalFade = true;
		_ui.AddChild(_background);
		_extraLetterboxTop = new MilMo_Widget(_ui);
		_extraLetterboxTop.SetTexture("Batch01/Textures/Core/White");
		_extraLetterboxTop.SetDefaultColor(0f, 0f, 0f, 1f);
		_extraLetterboxTop.SetAlignment(MilMo_GUI.Align.TopLeft);
		_extraLetterboxTop.IgnoreGlobalFade = true;
		_ui.AddChild(_extraLetterboxTop);
		_extraLetterboxBottom = new MilMo_Widget(_ui);
		_extraLetterboxBottom.SetTexture("Batch01/Textures/Core/White");
		_extraLetterboxBottom.SetDefaultColor(0f, 0f, 0f, 1f);
		_extraLetterboxBottom.SetAlignment(MilMo_GUI.Align.TopLeft);
		_extraLetterboxBottom.IgnoreGlobalFade = true;
		_ui.AddChild(_extraLetterboxBottom);
		_extraLetterboxLeft = new MilMo_Widget(_ui);
		_extraLetterboxLeft.SetTexture("Batch01/Textures/Core/White");
		_extraLetterboxLeft.SetDefaultColor(0f, 0f, 0f, 1f);
		_extraLetterboxLeft.SetAlignment(MilMo_GUI.Align.TopLeft);
		_extraLetterboxLeft.IgnoreGlobalFade = true;
		_ui.AddChild(_extraLetterboxLeft);
		_extraLetterboxRight = new MilMo_Widget(_ui);
		_extraLetterboxRight.SetTexture("Batch01/Textures/Core/White");
		_extraLetterboxRight.SetDefaultColor(0f, 0f, 0f, 1f);
		_extraLetterboxRight.SetAlignment(MilMo_GUI.Align.TopLeft);
		_extraLetterboxRight.IgnoreGlobalFade = true;
		_ui.AddChild(_extraLetterboxRight);
		_scroller = new MilMo_ScrollView(_ui);
		_scroller.HasBackground(b: false);
		_scroller.SetAlignment(MilMo_GUI.Align.TopLeft);
		_scroller.MShowVertBar = false;
		_scroller.MShowHorizBar = false;
		ScrollerSetDefaultSpeed();
		_scroller.SoftScroll.MinVel = new Vector2(0.05f, 0.05f);
		_ui.AddChild(_scroller);
		_scrollerOverlay = new MilMo_Widget(_ui);
		_scrollerOverlay.SetAlignment(MilMo_GUI.Align.TopLeft);
		_scrollerOverlay.AllowPointerFocus = false;
		_scroller.AddChild(_scrollerOverlay);
		_locationArrow = new MilMo_Widget(_ui)
		{
			Identifier = "LocationArrow"
		};
		_locationArrow.SetTexture("Batch01/Textures/Generic/DownArrow");
		_locationArrow.SetAlignment(MilMo_GUI.Align.BottomCenter);
		_locationArrow.AllowPointerFocus = false;
		_locationArrow.SetPosPull(0.09f, 0.09f);
		_locationArrow.SetPosDrag(0.45f, 0.45f);
		_locationArrow.SetFadeSpeed(0.05f);
		_locationArrow.SetMoveType(MilMo_Mover.UpdateFunc.Sinus);
		_locationArrow.PosMover.SinRate = new Vector2(0f, 4f);
		_locationArrow.PosMover.SinAmp = new Vector2(0f, 2f);
		_locationArrow.SetScaleType(MilMo_Mover.UpdateFunc.Sinus);
		_locationArrow.ScaleMover.SinRate = new Vector2(4f, 4f);
		_locationArrow.ScaleMover.SinAmp = new Vector2(-1f, 2f);
		_scroller.AddChild(_locationArrow);
		_locationText = new MilMo_Widget(_ui);
		_locationText.SetScale(400f, 100f);
		_locationText.SetPosPull(0.09f, 0.09f);
		_locationText.SetPosDrag(0.45f, 0.45f);
		_locationText.SetAlignment(MilMo_GUI.Align.BottomCenter);
		_locationText.AllowPointerFocus = false;
		_locationText.SetFont(MilMo_GUI.Font.EborgMedium);
		_locationText.SetTextOffset(0f, -75f);
		_locationText.SetTextDropShadowPos(2f, 2f);
		_locationText.TextOutline = new Vector2(1f, 1f);
		_locationText.TextOutlineColor = new Color(0f, 0f, 0f, 0.3f);
		_locationText.SetFadeSpeed(0.05f);
		_locationText.SetDefaultTextColor(0.46f, 1f, 0.52f, 1f);
		_scroller.AddChild(_locationText);
		_leftFogFade = new MilMo_Widget(_ui);
		_leftFogFade.SetTexture("Batch01/Textures/Generic/HorizontalGradient");
		_leftFogFade.SetDefaultColor(1f, 1f, 1f, 0.35f);
		_leftFogFade.SetAlignment(MilMo_GUI.Align.TopLeft);
		_leftFogFade.AllowPointerFocus = false;
		_leftFogFade.Enabled = false;
		_ui.AddChild(_leftFogFade);
		_rightFogFade = new MilMo_Widget(_ui);
		_rightFogFade.SetTexture("Batch01/Textures/Generic/HorizontalGradient");
		_rightFogFade.SetDefaultColor(1f, 1f, 1f, 0.35f);
		_rightFogFade.SetAlignment(MilMo_GUI.Align.TopLeft);
		_rightFogFade.AllowPointerFocus = false;
		_rightFogFade.Enabled = false;
		_ui.AddChild(_rightFogFade);
		_loadingPane = new MilMo_LoadingPane(_ui);
		_ui.AddChild(_loadingPane);
		_exitCorner = new MilMo_Widget(_ui)
		{
			FixedRes = true
		};
		_exitCorner.SetAlignment(MilMo_GUI.Align.TopCenter);
		_exitCorner.SetTextureInvisible();
		_exitCorner.SetDefaultColor(1f, 1f, 1f, 1f);
		_exitCorner.SetTextDropShadowPos(2f, 2f);
		_exitCorner.TextOutline = new Vector2(1f, 1f);
		_exitCorner.TextOutlineColor = new Color(0f, 0f, 0f, 0.3f);
		_exitCorner.SetText(MilMo_Localization.GetLocString("WorldMap_364"));
		_exitCorner.SetDefaultTextColor(1f, 1f, 1f, 1f);
		_exitCorner.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_exitCorner.SetTextOffset(0f, -4f);
		_exitCorner.SetScalePull(0.09f, 0.09f);
		_exitCorner.SetScaleDrag(0.75f, 0.75f);
		_exitCorner.SetAngle(45f);
		_exitCorner.SetExtraDrawTextSize(10f, 0f);
		_exitCorner.SetFont(MilMo_GUI.Font.EborgSmall);
		_exitCorner.SetEnabled(e: false);
		_exitCorner.AllowPointerFocus = false;
		_ui.AddChild(_exitCorner);
		_returnToAdventureWidget = new MilMo_Widget(_ui);
		_returnToAdventureWidget.SetTexture("Batch01/Textures/HUD/ReturnHereBackground");
		_returnToAdventureWidget.SetAlignment(MilMo_GUI.Align.TopCenter);
		_returnToAdventureWidget.SetDefaultColor(0f, 0f, 0f, 0.4f);
		_returnToAdventureWidget.SetScale(166f, 28f);
		_returnToAdventureWidget.SetAlpha(0f);
		_returnToAdventureWidget.Enabled = false;
		_returnToAdventureWidget.AllowPointerFocus = false;
		_mouseHand = new MilMo_WorldMapMouseIcon(_ui);
		MilMo_Widget milMo_Widget = new MilMo_Widget(_ui);
		milMo_Widget.SetDefaultColor(1f, 1f, 1f, 1f);
		milMo_Widget.SetScale(166f, 28f);
		milMo_Widget.SetPosition(0f, -2f);
		milMo_Widget.UseParentAlpha = false;
		milMo_Widget.AllowPointerFocus = false;
		milMo_Widget.SetFont(MilMo_GUI.Font.EborgSmall);
		milMo_Widget.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.SetText(MilMo_Localization.GetLocString("WorldMap_4758"));
		milMo_Widget.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		milMo_Widget.SetDefaultTextColor(1f, 1f, 1f, 1f);
		milMo_Widget.SetFontScale(0.8f);
		_returnToAdventureWidget.AddChild(milMo_Widget);
		_scroller.AddChild(_returnToAdventureWidget);
		_teleportStoneCounter = new MilMo_TeleportStoneCounter(_ui);
		_teleportStoneCounter.Close(null);
		_ui.AddChild(_teleportStoneCounter);
		MilMo_EventSystem.Listen("teleportstones_updated", UpdateTeleportStones).Repeating = true;
		UpdateTeleportStones(null);
	}

	private void SetupGrid()
	{
		if (!(_theWorld != "World01"))
		{
			for (int i = 1; i < 20; i++)
			{
				MilMo_Widget milMo_Widget = new MilMo_Widget(_ui)
				{
					Info = -3333
				};
				milMo_Widget.SetTexture("Batch01/Textures/Core/White");
				milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
				milMo_Widget.SetDefaultColor(1f, 1f, 1f, 0.075f);
				milMo_Widget.SetPosition((float)(i * 300) + -100f, 0f);
				milMo_Widget.SetScale(2f, 600f);
				milMo_Widget.AllowPointerFocus = false;
				milMo_Widget.IgnoredByScrollViewRefresh = true;
				_scroller.AddChild(milMo_Widget);
			}
			for (int j = 1; j < 3; j++)
			{
				MilMo_Widget milMo_Widget2 = new MilMo_Widget(_ui)
				{
					Info = -3333
				};
				milMo_Widget2.SetTexture("Batch01/Textures/Core/White");
				milMo_Widget2.SetAlignment(MilMo_GUI.Align.TopLeft);
				milMo_Widget2.SetDefaultColor(1f, 1f, 1f, 0.075f);
				milMo_Widget2.SetPosition(0f, (float)(j * 300) + -100f);
				milMo_Widget2.SetScale(10000f, 2f);
				milMo_Widget2.AllowPointerFocus = false;
				milMo_Widget2.IgnoredByScrollViewRefresh = true;
				_scroller.AddChild(milMo_Widget2);
			}
		}
	}

	private static void RemoveGrid()
	{
		for (int num = _scroller.Children.Count - 1; num >= 0; num--)
		{
			MilMo_Widget milMo_Widget = _scroller.Children[num];
			if (milMo_Widget.Info == -3333)
			{
				_scroller.RemoveChild(milMo_Widget);
			}
		}
	}

	private void BringGridLinesToFront()
	{
		foreach (LevelButton levelButton in _levelButtons)
		{
			_scroller.BringToFront(levelButton.PlayerCountTag);
		}
		for (int num = _scroller.Children.Count - 1; num >= 0; num--)
		{
			MilMo_Widget milMo_Widget = _scroller.Children[num];
			if (milMo_Widget.Info == -3333)
			{
				_scroller.BringToFront(milMo_Widget);
			}
		}
		_scroller.BringToFront(_locationArrow);
		_scroller.BringToFront(_returnToAdventureWidget);
	}

	private void UpdateTeleportStones(object msgAsObject)
	{
		ServerUpdateTeleportStones serverUpdateTeleportStones = msgAsObject as ServerUpdateTeleportStones;
		int num = 0;
		if (serverUpdateTeleportStones != null)
		{
			num = serverUpdateTeleportStones.getTeleportStoneCount();
		}
		else
		{
			MilMo_Player playerInstance = PlayerInstance;
			if (playerInstance != null && playerInstance.Avatar != null)
			{
				num = PlayerInstance.Avatar.TeleportStones;
			}
		}
		GameEvent.OnTeleportStoneUpdate?.Invoke(num);
		_teleportStoneCounter.SetNumber(num.ToString());
		_newCount = num;
	}

	private static void ScrollerSetDefaultSpeed()
	{
		_scroller.SetScrollPull(0.08f, 0.08f);
		_scroller.SetScrollDrag(0.25f, 0.25f);
	}

	private static void ScrollerSetUnlockSpeed()
	{
		_scroller.SetScrollPull(0.09f, 0.09f);
		_scroller.SetScrollDrag(0.45f, 0.45f);
	}

	private static void ScrollerSetStartPosition()
	{
		if (!string.IsNullOrEmpty(_lastAdventureLevel))
		{
			if (MilMo_LevelInfo.GetWorldInfoData(_lastAdventureLevel.Split(':')[0]).FixedPosition)
			{
				_scroller.ScrollToNow(0f, 0f);
				return;
			}
			float x = GetLevelButton(_lastAdventureLevel).PosMover.Target.x;
			float num = x;
			float num2 = _scroller.MViewSize.width - x;
			x = ((!(num < num2)) ? 0f : _scroller.MViewSize.width);
			_scroller.ScrollToNow(x, 0f);
		}
		else
		{
			_scroller.ScrollToNow(0f, 0f);
		}
	}

	public static void Toggle()
	{
		if (Singleton<LockStateManager>.Instance.HasUnlockedNavigator)
		{
			if (_theWorldMap == null)
			{
				Create();
			}
			if (_theWorldMap != null && _theWorldMap.enabled)
			{
				UserCloseAndBackToGame();
			}
			else
			{
				UserOpen();
			}
		}
	}

	private async void LoadAndSetTexture(string imagePath, LevelButton levelButton, float alpha)
	{
		Texture2D texture2D = await MilMo_ResourceManager.Instance.LoadTextureAsync(imagePath, "WorldMapImage");
		if (!(texture2D == null))
		{
			levelButton.Button.SetScale((float)texture2D.width * _res.x, (float)texture2D.height * _res.y);
			levelButton.Button.SetAllTextures(new MilMo_Texture(texture2D));
			levelButton.ImageLoaded = true;
			levelButton.Button.SetDefaultColor(1f, 1f, 1f, alpha);
			levelButton.Button.SetAlpha(0f);
			if (imagePath.Contains("W01L08"))
			{
				levelButton.Button.SetFixedPointerZoneSize(273f, 95f);
				levelButton.Button.SetFixedPointerZoneOffset(63f, -38f);
			}
			if (imagePath.Contains("W01L09"))
			{
				levelButton.Button.SetFixedPointerZoneSize(242f, 103f);
				levelButton.Button.SetFixedPointerZoneOffset(81f, -16f);
			}
			if (imagePath.Contains("W01L10"))
			{
				levelButton.Button.SetFixedPointerZoneSize(333f, 160f);
				levelButton.Button.SetFixedPointerZoneOffset(0f, 24f);
			}
			if (imagePath.Contains("W01L11"))
			{
				levelButton.Button.SetFixedPointerZoneSize(109f, 156f);
				levelButton.Button.SetFixedPointerZoneOffset(0f, -34f);
			}
			if (levelButton.GotPlayerCount)
			{
				levelButton.PlayerCountTag.SetEnabled(e: true);
				levelButton.PlayerCountTag.SetInvisible();
				levelButton.PlayerCountTag.FadeIn();
			}
		}
	}

	private void UpdatePlayerCountTags()
	{
		List<string> list = new List<string>();
		MilMo_Level currentLevel = MilMo_Level.CurrentLevel;
		foreach (LevelButton levelButton in _levelButtons)
		{
			if (currentLevel != null && levelButton.Level == currentLevel.Name && levelButton.World == currentLevel.World)
			{
				levelButton.PlayerCountTag.PlayerCount = currentLevel.NumberOfPlayers;
				levelButton.GotPlayerCount = true;
				if (levelButton.ImageLoaded)
				{
					levelButton.PlayerCountTag.SetEnabled(e: true);
					levelButton.PlayerCountTag.SetInvisible();
					levelButton.PlayerCountTag.FadeIn();
				}
			}
			else
			{
				list.Add(levelButton.World + ":" + levelButton.Level);
			}
			if (!levelButton.GotPlayerCount || !levelButton.ImageLoaded)
			{
				levelButton.PlayerCountTag.SetEnabled(e: false);
				levelButton.PlayerCountTag.SetInvisible();
			}
		}
		Singleton<GameNetwork>.Instance.RequestLevelPlayerCounts(list);
	}

	private void GotLevelPlayerCounts(object msgAsObj)
	{
		if (!base.enabled || !(msgAsObj is ServerLevelPlayerCounts serverLevelPlayerCounts))
		{
			return;
		}
		foreach (LevelButton levelButton in _levelButtons)
		{
			if (levelButton.GotPlayerCount)
			{
				continue;
			}
			foreach (LevelPlayerCount playerCount in serverLevelPlayerCounts.getPlayerCounts())
			{
				if (!(playerCount.GetLevel() != levelButton.Level) && !(playerCount.GetWorld() != levelButton.World))
				{
					levelButton.PlayerCountTag.PlayerCount = playerCount.GetPlayers();
					levelButton.GotPlayerCount = true;
					if (levelButton.ImageLoaded)
					{
						levelButton.PlayerCountTag.SetEnabled(e: true);
						levelButton.PlayerCountTag.SetInvisible();
						levelButton.PlayerCountTag.FadeIn();
					}
				}
			}
		}
		BringGridLinesToFront();
	}

	private static string GetImagePath(string fullName)
	{
		string text = fullName.Replace("orld", "");
		text = text.Replace("evel", "");
		text = text.Replace(":", "");
		string text2 = text.Remove(3, 3);
		return "Content/Worlds/" + text2 + "/WorldMapImages/WorldMapImage" + text;
	}

	public static string GetLevelIconPath(string fullName)
	{
		string text = fullName.Replace("orld", "");
		text = text.Replace("evel", "");
		text = text.Replace(":", "");
		string text2 = text.Remove(3, 3);
		text.Remove(0, 3);
		return "Content/Worlds/" + text2 + "/LevelIcons/LevelIcon" + text;
	}

	public static string GetLevelIcoAddressable(string fullName)
	{
		string text = fullName.Replace("orld", "");
		text = text.Replace("evel", "");
		text = text.Replace(":", "");
		return "LevelIcon" + text;
	}

	private static MilMo_Widget GetLevelButton(string fullLevelName)
	{
		MilMo_Widget result = null;
		foreach (MilMo_Widget item in from level in _scroller.Children
			where level.Info == -1111
			where level.Identifier == fullLevelName
			select level)
		{
			result = item;
		}
		return result;
	}

	private void DoExitCornerVisibility()
	{
		_exitCorner.SetEnabled(e: false);
	}

	private void SetupSplines()
	{
		Vector3 tangent = new Vector3(1f, 0f);
		Vector3 tangent2 = new Vector3(0f, -1f);
		foreach (MilMo_WorldMapSplineData item2 in MilMo_LevelInfo.GetSplinesInWorld(_theWorld))
		{
			MilMo_Spline milMo_Spline = new MilMo_Spline();
			SplineStruct splineStruct = default(SplineStruct);
			splineStruct.Spline = milMo_Spline;
			splineStruct.Level1 = item2.Level1;
			splineStruct.Level2 = item2.Level2;
			SplineStruct item = splineStruct;
			milMo_Spline.AddWaypoint(item2.StartPosRes(_res), tangent, 0f);
			milMo_Spline.AddWaypoint(item2.MidPosRes(_res), (float)item2.Points / 2f);
			milMo_Spline.AddWaypoint(item2.EndPosRes(_res), tangent2, item2.Points);
			_splineStructs.Add(item);
			for (int i = 1; i < item2.Points; i++)
			{
				MilMo_Widget milMo_Widget = new MilMo_Widget(_ui)
				{
					Info = -2222,
					AllowPointerFocus = false,
					CustomArg = milMo_Spline
				};
				milMo_Widget.SetPosition(milMo_Spline.GetPointAtTime(i).Position);
				milMo_Widget.SetPosPull(0.05f, 0.05f);
				milMo_Widget.SetPosDrag(0.95f, 0.95f);
				milMo_Widget.SetScalePull(0.05f, 0.05f);
				milMo_Widget.SetScaleDrag(0.6f, 0.6f);
				milMo_Widget.SetTexture("Batch01/Textures/WorldMap/SplineDot");
				milMo_Widget.SetScale(_splineDotScaleLocked);
				milMo_Widget.SetDefaultColor(_splineDotColorLocked);
				milMo_Widget.IgnoredByScrollViewRefresh = true;
				_scroller.AddChild(milMo_Widget);
			}
		}
	}

	private void RefreshPreviousSplines()
	{
		foreach (SplineStruct splineStruct in _splineStructs)
		{
			if (!MilMo_LevelInfo.IsUnlocked(splineStruct.Level1) || !MilMo_LevelInfo.IsUnlocked(splineStruct.Level2) || !MilMo_LevelInfo.IsGUIUnlocked(splineStruct.Level1) || !MilMo_LevelInfo.IsGUIUnlocked(splineStruct.Level2))
			{
				continue;
			}
			foreach (MilMo_Widget item in from point in _scroller.Children
				where point.Info == -2222
				where point.CustomArg == splineStruct.Spline
				select point)
			{
				InstantUnlockSplineDot(item);
			}
		}
	}

	private void UnlockNextSpline()
	{
		bool found = false;
		float num = 0f;
		foreach (SplineStruct item in _splineStructs.Where((SplineStruct s) => !found))
		{
			num = UnlockSpline(item);
			if ((double)Math.Abs(num) > 0.0)
			{
				found = true;
			}
		}
		if (found)
		{
			MilMo_EventSystem.At(num + 0.5f, UnlockNextSpline);
		}
	}

	private float UnlockSpline(SplineStruct s)
	{
		if (SplineIsUnlocked(s) || !MilMo_LevelInfo.IsUnlocked(s.Level1) || !MilMo_LevelInfo.IsUnlocked(s.Level2))
		{
			return 0f;
		}
		int num = 0;
		foreach (MilMo_Widget item in _scroller.Children.Where((MilMo_Widget point) => point.Info == -2222 && point.CustomArg == s.Spline))
		{
			if (_recentlyUnlockedMaps.Contains(s.Level1) || _recentlyUnlockedMaps.Contains(s.Level2))
			{
				AnimUnlockSplineDot(num, item);
			}
			else
			{
				InstantUnlockSplineDot(item);
			}
			num++;
		}
		return (float)num * 0.2f;
	}

	private void InstantUnlockSplineDot(MilMo_Widget dot)
	{
		dot.SetDefaultColor(_splineDotColorUnlocked);
		dot.SetScale(_splineDotScaleUnlocked);
	}

	private void AnimUnlockSplineDot(int time, MilMo_Widget dot)
	{
		MilMo_EventSystem.At((float)time * 0.2f, delegate
		{
			dot.SetDefaultColor(_splineDotColorUnlocked);
			dot.SetScale(_splineDotScaleLocked);
			dot.ScaleTo(_splineDotScaleUnlocked);
			dot.ImpulseRandom(-2f, 2f, -2f, 2f);
			dot.ScaleImpulse(MilMo_Utility.Random() * 0.2f, MilMo_Utility.Random() * 0.2f);
			_ui.SoundFx.Play(_softTickSound);
		});
	}

	private bool SplineIsUnlocked(SplineStruct splineStruct)
	{
		return _scroller.Children.Where((MilMo_Widget p) => p.Info == -2222 && p.CustomArg == splineStruct.Spline).Any((MilMo_Widget p) => p.TargetColor == _splineDotColorUnlocked);
	}

	private void RemoveSplines()
	{
		_devSpline?.Clear();
		for (int num = _scroller.Children.Count - 1; num >= 0; num--)
		{
			MilMo_Widget milMo_Widget = _scroller.Children[num];
			if (milMo_Widget.Info == -2222)
			{
				_scroller.RemoveChild(milMo_Widget);
			}
		}
		_splineStructs.Clear();
	}

	private void RemoveDevSpline()
	{
		_devSpline.Clear();
		for (int num = _scroller.Children.Count - 1; num >= 0; num--)
		{
			MilMo_Widget milMo_Widget = _scroller.Children[num];
			if (milMo_Widget.Info == -2222 && milMo_Widget.CustomArg == _devSpline)
			{
				_scroller.RemoveChild(milMo_Widget);
			}
		}
		_splineStructs.Clear();
		for (int num2 = _splineStructs.Count - 1; num2 >= 0; num2--)
		{
			SplineStruct item = _splineStructs[num2];
			if (item.Spline == _devSpline)
			{
				_splineStructs.Remove(item);
			}
		}
	}

	private void DoDevSplineInput()
	{
		if (_devSplineStart != null)
		{
			_devSplineStart.SetDefaultColor(1f, 1f, 1f, _showDevSplineInfo ? 1 : 0);
			if (MilMo_Pointer.LeftButton && !InputSwitch.GetKey(KeyCode.LeftControl) && MilMo_Pointer.Position.y - _ui.GlobalPosOffset.y < 580f)
			{
				_devSplineStart.SetPosition(MilMo_Pointer.Position.x - _ui.GlobalPosOffset.x + _scroller.SoftScroll.Val.x, MilMo_Pointer.Position.y - _ui.GlobalPosOffset.y);
				_devSplineStart.SetTextNoLocalization("Start");
			}
		}
		if (_devSplineMid != null)
		{
			_devSplineMid.SetDefaultColor(1f, 1f, 1f, _showDevSplineInfo ? 1 : 0);
			if (MilMo_Pointer.MiddleButton && !InputSwitch.GetKey(KeyCode.LeftControl) && MilMo_Pointer.Position.y - _ui.GlobalPosOffset.y < 580f)
			{
				_devSplineMid.SetPosition(MilMo_Pointer.Position.x - _ui.GlobalPosOffset.x + _scroller.SoftScroll.Val.x, MilMo_Pointer.Position.y - _ui.GlobalPosOffset.y);
				_devSplineMid.SetTextNoLocalization("Mid");
			}
		}
		if (_devSplineEnd != null)
		{
			_devSplineEnd.SetDefaultColor(1f, 1f, 1f, _showDevSplineInfo ? 1 : 0);
			if (MilMo_Pointer.RightButton && !InputSwitch.GetKey(KeyCode.LeftControl) && MilMo_Pointer.Position.y - _ui.GlobalPosOffset.y < 580f)
			{
				_devSplineEnd.SetPosition(MilMo_Pointer.Position.x - _ui.GlobalPosOffset.x + _scroller.SoftScroll.Val.x, MilMo_Pointer.Position.y - _ui.GlobalPosOffset.y);
				_devSplineEnd.SetTextNoLocalization("End");
			}
		}
		if (_devSplineInfo != null && _devSplineStart != null && _devSplineMid != null && _devSplineEnd != null)
		{
			_devSplineInfo.SetTextNoLocalization("World: World01\nLevel1: " + _devSplineLevel1 + "\nLevel2: " + _devSplineLevel2 + "\nStartPos: " + (int)(_devSplineStart.PosMover.Target.x / _ui.Res.x) + ", " + (int)(_devSplineStart.PosMover.Target.y / _ui.Res.y) + "\nMidPos: " + (int)(_devSplineMid.PosMover.Target.x / _ui.Res.x) + ", " + (int)(_devSplineMid.PosMover.Target.y / _ui.Res.y) + "\nEndPos: " + (int)(_devSplineEnd.PosMover.Target.x / _ui.Res.x) + ", " + (int)(_devSplineEnd.PosMover.Target.y / _ui.Res.y) + "\nPoints: " + (_devSplinePoints - 1));
		}
		if (InputSwitch.GetKeyUp(KeyCode.KeypadMinus))
		{
			_devSplinePoints--;
		}
		if (InputSwitch.GetKeyUp(KeyCode.KeypadPlus))
		{
			_devSplinePoints++;
		}
		if (_devSplinePoints < 4)
		{
			_devSplinePoints = 4;
		}
		if (InputSwitch.GetKeyUp(KeyCode.Keypad0))
		{
			_showDevSplineInfo = !_showDevSplineInfo;
		}
		if (InputSwitch.GetKeyUp(KeyCode.KeypadEnter))
		{
			DumpSpline();
		}
		RefreshDevSpline();
	}

	private void RefreshDevSpline()
	{
		if (_devSpline == null)
		{
			_devSpline = new MilMo_Spline();
			SplineStruct splineStruct = default(SplineStruct);
			splineStruct.Spline = _devSpline;
			splineStruct.Level1 = "World01:Level00";
			splineStruct.Level2 = "World01:Level00";
			SplineStruct item = splineStruct;
			_splineStructs.Add(item);
		}
		if (_devSplineInfo == null)
		{
			_devSplineInfo = new MilMo_Widget(_ui);
			_devSplineInfo.SetPosition(50f, 70f);
			_devSplineInfo.SetFont(MilMo_GUI.Font.ArialRounded);
			_devSplineInfo.SetScale(200f, 200f);
			_devSplineInfo.SetTexture("Batch01/Textures/Core/Invisible");
			_devSplineInfo.SetTextAlignment(MilMo_GUI.Align.TopLeft);
			_devSplineInfo.SetAlignment(MilMo_GUI.Align.TopLeft);
			_devSplineInfo.SetDefaultColor(1f, 1f, 1f, 1f);
			_devSplineInfo.TextDropShadowPos = new Vector2(1f, 1f);
			_ui.AddChild(_devSplineInfo);
		}
		else
		{
			_devSplineInfo.SetEnabled(e: true);
		}
		if (_devSplineStart == null)
		{
			_devSplineStart = new MilMo_Widget(_ui)
			{
				AllowPointerFocus = false
			};
			_devSplineStart.SetTexture("Batch01/Textures/WorldMap/DevSplineCursor");
			_devSplineStart.SetScale(15f, 15f);
			_devSplineStart.SetExtraDrawTextSize(100f, 30f);
			_devSplineStart.SetTextOffset(0f, -25f);
			_devSplineStart.SetFont(MilMo_GUI.Font.ArialRounded);
			_devSplineStart.SetPosition(300f, 300f);
			_devSplineStart.SetAlignment(MilMo_GUI.Align.CenterCenter);
			_devSplineStart.SetDefaultColor(1f, 1f, 1f, 1f);
			_devSplineStart.TextDropShadowPos = new Vector2(1f, 1f);
			_devSplineStart.IgnoredByScrollViewRefresh = true;
			_scroller.AddChild(_devSplineStart);
		}
		else
		{
			_devSplineStart.SetEnabled(e: true);
		}
		if (_devSplineMid == null)
		{
			_devSplineMid = new MilMo_Widget(_ui)
			{
				AllowPointerFocus = false
			};
			_devSplineMid.SetTexture("Batch01/Textures/WorldMap/DevSplineCursor");
			_devSplineMid.SetScale(15f, 15f);
			_devSplineMid.SetExtraDrawTextSize(100f, 30f);
			_devSplineMid.SetTextOffset(0f, -25f);
			_devSplineMid.SetFont(MilMo_GUI.Font.ArialRounded);
			_devSplineMid.SetPosition(300f, 300f);
			_devSplineMid.SetAlignment(MilMo_GUI.Align.CenterCenter);
			_devSplineMid.SetDefaultColor(1f, 1f, 1f, 1f);
			_devSplineMid.TextDropShadowPos = new Vector2(1f, 1f);
			_devSplineMid.IgnoredByScrollViewRefresh = true;
			_scroller.AddChild(_devSplineMid);
		}
		else
		{
			_devSplineMid.SetEnabled(e: true);
		}
		if (_devSplineEnd == null)
		{
			_devSplineEnd = new MilMo_Widget(_ui)
			{
				AllowPointerFocus = false
			};
			_devSplineEnd.SetTexture("Batch01/Textures/WorldMap/DevSplineCursor");
			_devSplineEnd.SetScale(15f, 15f);
			_devSplineEnd.SetExtraDrawTextSize(100f, 30f);
			_devSplineEnd.SetTextOffset(0f, -25f);
			_devSplineEnd.SetFont(MilMo_GUI.Font.ArialRounded);
			_devSplineEnd.SetPosition(300f, 300f);
			_devSplineEnd.SetAlignment(MilMo_GUI.Align.CenterCenter);
			_devSplineEnd.SetDefaultColor(1f, 1f, 1f, 1f);
			_devSplineEnd.TextDropShadowPos = new Vector2(1f, 1f);
			_devSplineEnd.IgnoredByScrollViewRefresh = true;
			_scroller.AddChild(_devSplineEnd);
		}
		else
		{
			_devSplineEnd.SetEnabled(e: true);
		}
		RemoveDevSpline();
		Vector2 target = _devSplineStart.PosMover.Target;
		Vector2 target2 = _devSplineMid.PosMover.Target;
		Vector2 target3 = _devSplineEnd.PosMover.Target;
		Vector3 tangent = new Vector3(1f, 0f);
		Vector3 tangent2 = new Vector3(0f, -1f);
		_devSpline.AddWaypoint(target, tangent, 0f);
		_devSpline.AddWaypoint(target2, (float)_devSplinePoints / 2f);
		_devSpline.AddWaypoint(target3, tangent2, _devSplinePoints);
		for (int i = 1; i < _devSplinePoints; i++)
		{
			MilMo_Widget milMo_Widget = new MilMo_Widget(_ui)
			{
				Info = -2222,
				AllowPointerFocus = false,
				CustomArg = _devSpline
			};
			milMo_Widget.SetPosition(_devSpline.GetPointAtTime(i).Position);
			milMo_Widget.SetTexture("Batch01/Textures/WorldMap/SplineDot");
			milMo_Widget.SetScale(_splineDotScaleUnlocked);
			milMo_Widget.SetDefaultColor(_splineDotColorUnlocked);
			milMo_Widget.IgnoredByScrollViewRefresh = true;
			_scroller.AddChild(milMo_Widget);
		}
	}

	private void DumpSpline()
	{
		Debug.Log("<SPLINE>");
		Debug.Log("World World01");
		if (_devSplineLevel1 != null)
		{
			Debug.Log("Level1 " + _devSplineLevel1);
		}
		if (_devSplineLevel2 != null)
		{
			Debug.Log("Level2 " + _devSplineLevel2);
		}
		Debug.Log("StartPos " + (int)_devSplineStart.PosMover.Target.x + " " + (int)_devSplineStart.PosMover.Target.y);
		Debug.Log("MidPos " + (int)_devSplineMid.PosMover.Target.x + " " + (int)_devSplineMid.PosMover.Target.y);
		Debug.Log("EndPos " + (int)_devSplineEnd.PosMover.Target.x + " " + (int)_devSplineEnd.PosMover.Target.y);
		Debug.Log("Points " + (_devSplinePoints - 1));
		Debug.Log("</SPLINE>");
	}

	private static string ToggleSplineEdit(string[] args)
	{
		if (!_splineEditMode)
		{
			_splineEditMode = true;
			return "spline edit mode enabled";
		}
		_splineEditMode = false;
		_devSplineInfo?.SetEnabled(e: false);
		_devSplineStart?.SetEnabled(e: false);
		_devSplineMid?.SetEnabled(e: false);
		_devSplineEnd?.SetEnabled(e: false);
		return "spline edit mode disabled";
	}
}
