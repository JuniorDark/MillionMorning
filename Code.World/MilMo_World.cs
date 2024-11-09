using System;
using System.Collections;
using Code.Apps.Fade;
using Code.Core.Avatar;
using Code.Core.BodyPack;
using Code.Core.BuddyBackend;
using Code.Core.Camera;
using Code.Core.Command;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.GUI.Core;
using Code.Core.Input;
using Code.Core.Items;
using Code.Core.Items.Home;
using Code.Core.Monetization;
using Code.Core.Music;
using Code.Core.Network;
using Code.Core.Network.messages.client;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.ObjectEffectSystem;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.Core.Utility;
using Code.Core.Visual;
using Code.World.CharacterShop;
using Code.World.CharBuilder.MakeOverStudio;
using Code.World.Climbing;
using Code.World.Environment;
using Code.World.Gameplay;
using Code.World.GUI;
using Code.World.GUI.GameDialog;
using Code.World.GUI.Hub;
using Code.World.GUI.Ladder;
using Code.World.GUI.LoadingScreen;
using Code.World.GUI.Navigator;
using Code.World.GUI.PVP;
using Code.World.Home;
using Code.World.Home.FurnitureActions;
using Code.World.Level;
using Code.World.Level.LevelInfo;
using Code.World.Player;
using Code.World.Quest;
using Code.World.WorldMap;
using Core;
using Core.Analytics;
using Core.GameEvent;
using Core.State;
using Localization;
using Player;
using UI.HUD.Dialogues;
using UI.HUD.States;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.World;

public sealed class MilMo_World : MonoBehaviour
{
	private class PendingHomeTravel
	{
		public readonly string PlayerId;

		public readonly string AvatarName;

		public readonly bool UseTeleportStone;

		public PendingHomeTravel(string playerId, string avatarName, bool useTeleportStone)
		{
			PlayerId = playerId;
			AvatarName = avatarName;
			UseTeleportStone = useTeleportStone;
		}
	}

	private MilMo_Level _level;

	private MilMo_Home _home;

	private float _sentTime;

	private bool _didSendStandStill;

	private Vector3 _lastPlayerPosition = Vector3.zero;

	private float _lastPlayerRotation;

	private float _deactivationTime;

	public static MilMo_HudHandler HudHandler;

	public static MilMoPvpScoreBoard PvpScoreBoard;

	public static MilMoPvpModeInfoWindow PvpModeInfoWindow;

	public static string CurrentWorld = "World01";

	private Color _backupBackgroundColor;

	private Rect _backupCameraRect = new Rect(0f, 0f, 1f, 1f);

	public Vector3 returnPosition;

	public Quaternion returnRotation;

	private Vector3 _positionOutsideCharBuilder;

	private Quaternion _rotationOutsideCharBuilder;

	private MilMo_Wieldable _wieldedItemOutsideShop;

	public MilMoInviteDialog theInviteDialog;

	private static bool _firstLevelLoad = true;

	private static bool _statsFirstLevelLoad = true;

	private static float _requestToEnterLevelStartTime;

	private static bool _requestToEnterLevelDialog;

	private static float _latestLevelLoadStartTime;

	private static MilMo_EventSystem.MilMo_Callback _inviteResponseReaction;

	private static MilMo_GenericReaction _levelInstanceInfoListener;

	private static MilMo_GenericReaction _homeInstanceInfoListener;

	private static MilMo_GenericReaction _serverLoginInfoLevelListener;

	private static MilMo_GenericReaction _serverLoginInfoHomeListener;

	private static MilMo_GenericReaction _startScreenInfoListener;

	private static MilMo_GenericReaction _enterHomeFailListener;

	private static MilMo_GenericReaction _homeAccessResponseListener;

	private PendingHomeTravel _pendingHomeTravel;

	private PendingHomeTravel _pendingHomeTravelCheckingHomeAccess;

	private float _previousLOD = 1f;

	public static MilMo_World Instance { get; private set; }

	public MilMo_PlayerControllerBase PlayerController { get; private set; }

	public MilMo_Camera Camera { get; private set; }

	public MilMo_UserInterface UI { get; private set; }

	private float DeactivatedDuration
	{
		get
		{
			if (!base.enabled)
			{
				return Time.time - _deactivationTime;
			}
			return 0f;
		}
	}

	public static void StartLevelChangeListeners()
	{
		if (_levelInstanceInfoListener == null)
		{
			_levelInstanceInfoListener = MilMo_EventSystem.Listen("server_level_instance_info", delegate(object o)
			{
				Create();
				Instance.GotLoadLevelInfo(o);
			});
			_levelInstanceInfoListener.Repeating = true;
		}
		if (_homeInstanceInfoListener == null)
		{
			_homeInstanceInfoListener = MilMo_EventSystem.Listen("server_home_instance_info", delegate(object o)
			{
				Create();
				Instance.GotLoadHomeInfo(o);
			});
			_homeInstanceInfoListener.Repeating = true;
		}
		if (_serverLoginInfoLevelListener == null)
		{
			_serverLoginInfoLevelListener = MilMo_EventSystem.Listen("server_login_info_level", delegate(object o)
			{
				Create();
				Instance.GotServerLoginInfoForLevel(o);
			});
			_serverLoginInfoLevelListener.Repeating = true;
		}
		if (_serverLoginInfoHomeListener == null)
		{
			_serverLoginInfoHomeListener = MilMo_EventSystem.Listen("server_login_info_home", delegate(object o)
			{
				Create();
				Instance.GotServerLoginInfoForHome(o);
			});
			_serverLoginInfoHomeListener.Repeating = true;
		}
		if (_startScreenInfoListener == null)
		{
			_startScreenInfoListener = MilMo_EventSystem.Listen("server_start_screen_info", delegate(object o)
			{
				Create();
				Instance.GotHubInfo(o);
			});
			_startScreenInfoListener.Repeating = true;
		}
		if (_enterHomeFailListener == null)
		{
			_enterHomeFailListener = MilMo_EventSystem.Listen("server_enter_home_fail", delegate
			{
				Create();
			});
			_enterHomeFailListener.Repeating = true;
		}
		if (_homeAccessResponseListener == null)
		{
			_homeAccessResponseListener = MilMo_EventSystem.Listen("server_home_access_response", delegate(object o)
			{
				Create();
				Instance.HandleHomeAccessResponse(o);
			});
			_homeAccessResponseListener.Repeating = true;
		}
	}

	private static bool Create()
	{
		if (Instance != null)
		{
			return true;
		}
		MilMo_LevelInfo.LoadLevels();
		Instance = MilMo_Global.MainGameObject.AddComponent<MilMo_World>();
		if (Instance == null)
		{
			Debug.LogWarning("Failed to create world: Failed to add component MilMo_World.");
			return false;
		}
		MilMo_Command.Instance.RegisterCommand("CameraDistance", Debug_CameraDistance);
		MilMo_Command.Instance.RegisterCommand("ChangeLevel", Debug_ChangeLevel);
		MilMo_Command.Instance.RegisterCommand("Debug_Strip", Debug_Strip);
		MilMo_Command.Instance.RegisterCommand("Teleport", Debug_TeleportPlayer);
		MilMo_Command.Instance.RegisterCommand("ReloadObjectEffects", MilMo_ObjectEffectSystem.Debug_ReloadObjectEffects);
		MilMo_Command.Instance.RegisterCommand("GameplayObject.DebugMode", MilMo_GameplayObject.Debug_ToggleDebugMode);
		MilMo_Command.Instance.RegisterCommand("Shop.ClearRemoteData", MilMo_CharacterShop.Debug_ClearRemoteData);
		MilMo_FurnitureState.RegisterActionCreateCallback(MilMo_ActionCreator.CreateAction);
		Instance.UI = MilMo_UserInterfaceManager.CreateUserInterface("World");
		if (Instance.UI == null)
		{
			Debug.LogWarning("Failed to create user interface for world");
			return false;
		}
		Instance.UI.PrintMouseFocus = false;
		Instance.UI.ResetLayout();
		Instance.UI.Enabled = false;
		Instance.Camera = Instance.gameObject.AddComponent<MilMo_Camera>();
		Instance._level = new MilMo_Level();
		Instance._level.StartGlobalListeners();
		Instance._level.PvpHandler.CreateAndStartShit();
		Instance._home = new MilMo_Home();
		Instance.PlayerController = new MilMo_PlayerControllerInGUIApp(MilMo_PlayerControllerBase.ControllerType.Game);
		MilMo_ProfileManager.Initialize();
		InitializeOpenNavigatorListener();
		InitializeOpenShopListener();
		InitializeOpenTownListener();
		MilMo_ObjectEffectSystem.Init();
		MilMo_BashAnimationSoundSystem.Init();
		Instance.enabled = false;
		MilMo_BlobShadow.AsyncLoadContent();
		MilMo_EventSystem.Listen("shop_response", Instance.HandleShopResponse).Repeating = true;
		MilMo_EventSystem.Listen("worldmap_response", Instance.HandleWorldMapResponse).Repeating = true;
		MilMo_EventSystem.Listen("hub_response", Instance.HandleHubResponse).Repeating = true;
		MilMo_EventSystem.Listen("charbuilder_response", Instance.HandleMakeoverStudioResponse).Repeating = true;
		MilMo_EventSystem.Listen("on_invite_request", HandleInviteResponse).Repeating = true;
		MilMo_Input.CurrentKeyboardFocusCallback = () => MilMo_UserInterface.KeyboardFocus;
		MilMo_Input.CurrentGUIMouseFocusCallback = () => MilMo_UserInterfaceManager.FinalMouseFocus != null;
		MilMo_Input.CurrentClickObjectCallback = () => MilMo_Instance.CurrentInstance != null && MilMo_Instance.CurrentInstance.TestClickOnObject();
		MilMo_ResourceManager.Instance.PreloadAssets(MilMo_WorldPreload.Assets, "World", null);
		MilMo_CharacterShop.AsyncLoadVisualReps();
		CreateGUIComponents();
		MilMo_EventSystem.Instance.PostEvent("world_created", null);
		Instance.InvokeRepeating("UpdateAfkTimer", 1f, 1f);
		Debug.Log("World created.");
		return true;
	}

	private static void CreateGUIComponents()
	{
		Instance.theInviteDialog = new MilMoInviteDialog(MilMo_GlobalUI.GetSystemUI);
		MilMo_GlobalUI.GetSystemUI.AddChild(Instance.theInviteDialog);
		Instance.theInviteDialog.Close(null);
		MilMo_GameDialogInvite.DisableFor(120f);
		HudHandler = Instance.gameObject.AddComponent<MilMo_HudHandler>();
		HudHandler.InitUI(Instance.UI);
		PvpScoreBoard = new MilMoPvpScoreBoard(Instance.UI);
		Instance.UI.AddChild(PvpScoreBoard);
		PvpModeInfoWindow = new MilMoPvpModeInfoWindow(Instance.UI);
		Instance.UI.AddChild(PvpModeInfoWindow);
	}

	private static void Destroy()
	{
		if (!(Instance == null) && Instance.UI != null)
		{
			Instance.UI.Cleanup();
			Instance._level.StopGlobalListeners();
			Instance._level.Unload();
			Instance._home.Unload();
		}
	}

	public static void Activate()
	{
		Debug.Log("MilMo_World::Activate");
		if (Instance == null && !Create())
		{
			Debug.LogWarning("Failed to create world. This is fatal.");
			return;
		}
		if (Instance != null)
		{
			float deactivatedDuration = Instance.DeactivatedDuration;
			Instance.enabled = true;
			Instance.Camera.enabled = true;
			if (MilMo_Instance.CurrentInstance != null && (bool)MilMo_Environment.SceneLight)
			{
				MilMo_Environment.SceneLight.SetActive(value: true);
			}
			if ((bool)MilMo_Global.ImageEffectsHandler)
			{
				MilMo_Global.ImageEffectsHandler.EnableEffects("bloom", enable: true);
				if (MilMo_Level.CurrentLevel != null && !MilMo_Level.CurrentLevel.Environment.DisableColorCorrection)
				{
					MilMo_Global.ImageEffectsHandler.EnableEffects("enhance", enable: true);
				}
			}
			Instance.UI.Enabled = true;
			HudHandler.enabled = true;
			GameEvent.ShowHUDEvent.RaiseEvent(args: true);
			MilMo_EventSystem.Instance.PostEvent("world_activated", deactivatedDuration);
		}
		if (MilMo_Player.Instance.Avatar != null)
		{
			MilMo_Player.Instance.Avatar.InitDamageNumberEffect();
		}
	}

	private void UpdateAfkTimer()
	{
		MilMo_Player.Instance.UpdateAfkTimer();
	}

	public static void Deactivate()
	{
		Debug.Log("MilMo_World::Deactivate");
		if (Instance.enabled)
		{
			Instance._deactivationTime = Time.time;
		}
		if (HudHandler != null)
		{
			HudHandler.enabled = false;
		}
		GameEvent.ShowHUDEvent.RaiseEvent(args: false);
		if (Instance.Camera != null)
		{
			Instance.Camera.enabled = false;
		}
		MilMo_GlobalUI.Instance.ClosePanels();
		MilMo_Utility.SetUnlockedMode();
		if (MilMo_Instance.CurrentInstance != null && (bool)MilMo_Environment.SceneLight)
		{
			MilMo_Environment.SceneLight.SetActive(value: false);
		}
		if ((bool)MilMo_Global.ImageEffectsHandler)
		{
			MilMo_Global.ImageEffectsHandler.EnableEffects("water", enable: false);
			MilMo_Global.ImageEffectsHandler.EnableEffects("bloom", enable: false);
			MilMo_Global.ImageEffectsHandler.EnableEffects("enhance", enable: false);
		}
		if (Instance.UI != null)
		{
			Instance.UI.Enabled = false;
		}
		Instance.enabled = false;
	}

	private void Update()
	{
		try
		{
			if (!MilMo_Player.Instance.InInstance)
			{
				return;
			}
			if (MilMo_Instance.CurrentInstance != null)
			{
				MilMo_Instance.CurrentInstance.Update();
			}
			if (PlayerController != null)
			{
				MilMo_PlayerControllerBase.StartMovingPlatformFrame();
				PlayerController.UpdatePlayer();
			}
			bool flag = false;
			bool flag2 = false;
			if (MilMo_Player.Instance.Avatar != null && (bool)MilMo_Player.Instance.Avatar.GameObject)
			{
				Vector3 position = MilMo_Player.Instance.Avatar.GameObject.transform.position;
				float y = MilMo_Player.Instance.Avatar.GameObject.transform.rotation.eulerAngles.y;
				flag = (position - _lastPlayerPosition).magnitude > 0.1f * Time.deltaTime;
				flag2 = Mathf.Abs(y - _lastPlayerRotation) > 2f * Time.deltaTime;
			}
			if (!flag && !flag2)
			{
				if (!_didSendStandStill || Time.time - _sentTime > 10f)
				{
					MilMo_Player.Instance.SendPlayerStopFrame();
					_didSendStandStill = true;
					_sentTime = Time.time;
				}
			}
			else
			{
				if (_didSendStandStill || Time.time - _sentTime > MilMo_Player.UpdateToServerFrequency)
				{
					MilMo_Player.Instance.SendPlayerFrame();
					_sentTime = Time.time;
				}
				_didSendStandStill = false;
			}
			if (MilMo_Player.Instance.Avatar != null && MilMo_Player.Instance.Avatar.GameObject != null)
			{
				_lastPlayerPosition = MilMo_Player.Instance.Avatar.GameObject.transform.position;
				_lastPlayerRotation = MilMo_Player.Instance.Avatar.GameObject.transform.rotation.eulerAngles.y;
			}
			MilMo_QuestAreaManager.Update();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}

	private void FixedUpdate()
	{
		try
		{
			if (MilMo_Player.Instance.InInstance)
			{
				if (MilMo_Instance.CurrentInstance != null)
				{
					MilMo_Instance.CurrentInstance.FixedUpdate();
				}
				if (PlayerController != null)
				{
					MilMo_PlayerControllerBase.FixedUpdate();
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}

	private void LateUpdate()
	{
		try
		{
			if (MilMo_Player.Instance.InInstance)
			{
				if (MilMo_Instance.CurrentInstance != null)
				{
					MilMo_Instance.CurrentInstance.LateUpdate();
				}
				if (PlayerController != null)
				{
					PlayerController.LateUpdatePlayer();
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}

	private void UnloadCurrentInstance()
	{
		if (!_firstLevelLoad)
		{
			_level.Unload();
		}
		_home.Unload();
		MilMo_Player.Instance.InInstance = false;
	}

	public void LoginToNewServer(string host, string token, MilMo_EventSystem.MilMo_Callback playerInfoDoneCallback)
	{
		MilMo_EventSystem.RemoveReaction(MilMo_Player.DisconnectListener);
		MilMo_Player.LogoutGame(delegate
		{
			MilMo_Player.Instance.InInstance = false;
			ChangePlayerController(MilMo_PlayerControllerBase.ControllerType.InGUIApp, null);
			Camera.UnhookAll();
			MilMo_Level.ClearAllExplorationTokensFoundListeners();
			MilMo_Player.Instance.UnloadPlayerInfo();
			MilMo_CharacterShop.ClearShopData();
			MilMo_Player.LoginGame(host, token, delegate(MilMo_LoginInfo loginInfo)
			{
				if (!loginInfo.Success)
				{
					MilMo_Player.WarningDialog(MilMo_Localization.GetLocString("World_418"), MilMo_Localization.GetLocString("World_419"));
				}
				else
				{
					MilMo_EventSystem.AddReaction(MilMo_Player.DisconnectListener);
					MilMo_Player.Instance.RequestLocalPlayerInfo(delegate(bool infoSuccess, ServerLocalPlayerInfo playerInfo)
					{
						if (!infoSuccess)
						{
							MilMo_Player.WarningDialog(MilMo_Localization.GetLocString("World_420"), MilMo_Localization.GetLocString("World_421"));
						}
						else if (playerInfoDoneCallback != null)
						{
							playerInfoDoneCallback();
						}
					});
				}
			});
		});
	}

	private static void HandleInviteResponse(object messageAsObject)
	{
		if (messageAsObject is ServerInviteResponse serverInviteResponse)
		{
			string uRL = serverInviteResponse.getURL();
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("email", serverInviteResponse.getEmail());
			wWWForm.AddField("rid", serverInviteResponse.getId());
			wWWForm.AddField("ik", serverInviteResponse.getInviteKey());
			if (uRL.Length != 0)
			{
				Debug.Log("Sending invite response with url " + uRL);
				Instance.StartCoroutine(Post(uRL, wWWForm));
			}
		}
	}

	private static IEnumerator Post(string url, WWWForm form)
	{
		UnityWebRequest www = UnityWebRequest.Post(url, form);
		yield return www.SendWebRequest();
		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.LogWarning("Failed to send invite response with url " + url + ". " + www.error);
		}
		else
		{
			Debug.Log("Invite response: " + www.downloadHandler.text + " from " + url);
		}
	}

	public void ChangePlayerController(MilMo_PlayerControllerBase.ControllerType controllerType, params object[] args)
	{
		if (PlayerController == null)
		{
			return;
		}
		MilMo_PlayerControllerBase.ControllerType type = PlayerController.Type;
		switch (controllerType)
		{
		case MilMo_PlayerControllerBase.ControllerType.Chat:
			if (PlayerController.Type != MilMo_PlayerControllerBase.ControllerType.Chat)
			{
				PlayerController.Exit();
				PlayerController = new MilMo_PlayerControllerChat();
			}
			break;
		case MilMo_PlayerControllerBase.ControllerType.Game:
			if (PlayerController.Type != 0)
			{
				PlayerController.Exit();
				PlayerController = new MilMo_PlayerControllerGame();
			}
			break;
		case MilMo_PlayerControllerBase.ControllerType.Social:
			if (PlayerController.Type != MilMo_PlayerControllerBase.ControllerType.Social)
			{
				PlayerController.Exit();
				PlayerController = new MilMo_PlayerControllerSocial();
			}
			break;
		case MilMo_PlayerControllerBase.ControllerType.Home:
			if (PlayerController.Type != MilMo_PlayerControllerBase.ControllerType.Home)
			{
				PlayerController.Exit();
				PlayerController = new MilMo_PlayerControllerHome();
			}
			break;
		case MilMo_PlayerControllerBase.ControllerType.SplineRide:
			if (PlayerController.Type != MilMo_PlayerControllerBase.ControllerType.SplineRide)
			{
				PlayerController.Exit();
				PlayerController = new MilMo_PlayerControllerSplineRide();
			}
			break;
		case MilMo_PlayerControllerBase.ControllerType.Climb:
			if (!MilMo_Player.Instance.IsExhausted && PlayerController.Type != MilMo_PlayerControllerBase.ControllerType.Climb)
			{
				if (args == null || args.Length == 0)
				{
					Debug.LogWarning("Trying to use climb controller without valid attach info");
					break;
				}
				if (!(args[0] is MilMo_ClimbingSurface.MilMo_AttachInfo))
				{
					Debug.LogWarning("Trying to use climb controller without valid attach info");
					break;
				}
				PlayerController.Exit();
				PlayerController = new MilMo_PlayerControllerClimb(args[0] as MilMo_ClimbingSurface.MilMo_AttachInfo, type);
			}
			break;
		case MilMo_PlayerControllerBase.ControllerType.InGUIApp:
			if (PlayerController.Type != MilMo_PlayerControllerBase.ControllerType.InGUIApp)
			{
				PlayerController.Exit();
				PlayerController = new MilMo_PlayerControllerInGUIApp(type);
			}
			break;
		}
	}

	private void OnApplicationQuit()
	{
		Destroy();
	}

	private static string Debug_CameraDistance(string[] args)
	{
		if (Instance == null)
		{
			return "Can't alter camera distance if not in world.";
		}
		if (args.Length < 2)
		{
			return "CameraDistance " + MilMo_CameraController.Distance;
		}
		MilMo_CameraController.Distance = MilMo_Utility.StringToFloat(args[1]);
		return "Camera distance set to " + MilMo_CameraController.Distance;
	}

	private static string Debug_TeleportPlayer(string[] args)
	{
		if (args.Length < 4)
		{
			return "usage: Teleport <x> <y> <z>";
		}
		Vector3 vector = new Vector3(MilMo_Utility.StringToFloat(args[1]), MilMo_Utility.StringToFloat(args[2]), MilMo_Utility.StringToFloat(args[3]));
		MilMo_Player.Instance.Avatar.GameObject.transform.position = vector;
		Vector3 vector2 = vector;
		return "Teleporting player to " + vector2.ToString();
	}

	private static string Debug_ChangeLevel(string[] args)
	{
		if (args.Length < 2)
		{
			return "usage: ChangeLevel <levelName>";
		}
		string level = args[1];
		string world = "World01";
		if (MilMo_Level.CurrentLevel != null)
		{
			world = MilMo_Level.CurrentLevel.World;
		}
		string[] array = args[1].Split(":".ToCharArray());
		if (array.Length > 1)
		{
			world = array[0];
			level = array[1];
		}
		MilMo_LoadingScreen.Instance.LevelLoadFade(12f);
		MilMo_EventSystem.At(2f, delegate
		{
			Singleton<GameNetwork>.Instance.Debug_RequestChangeLevel(world, level, "EN");
		});
		Debug.Log("ChangeLevel: " + args[1]);
		return "Trying to enter level " + level + " in " + world;
	}

	private static string Debug_Strip(string[] args)
	{
		if (MilMo_Player.Instance == null || MilMo_Player.Instance.Avatar == null)
		{
			return "Player has no avatar.";
		}
		MilMo_Player.Instance.Avatar.StripLocal();
		return "Removed all equipped wearables from avatar";
	}

	public bool GoToHome(string playerId, string avatarName)
	{
		if (!int.TryParse(playerId, out var result))
		{
			return false;
		}
		if (playerId == MilMo_Player.Instance.Id)
		{
			GoToHome(MilMo_Player.Instance.Id, MilMo_Player.Instance.Avatar.Name, useTeleportStone: false);
			return true;
		}
		_pendingHomeTravelCheckingHomeAccess = new PendingHomeTravel(playerId, avatarName, useTeleportStone: false);
		Singleton<GameNetwork>.Instance.SendToGameServer(new ClientCheckHomeAccess(result));
		return true;
	}

	private void HandleHomeAccessResponse(object message)
	{
		if (_pendingHomeTravelCheckingHomeAccess != null)
		{
			ServerHomeAccessResponse serverHomeAccessResponse = (ServerHomeAccessResponse)message;
			if (serverHomeAccessResponse != null && serverHomeAccessResponse.getResult() == 1)
			{
				GoToHome(_pendingHomeTravelCheckingHomeAccess.PlayerId, _pendingHomeTravelCheckingHomeAccess.AvatarName, _pendingHomeTravelCheckingHomeAccess.UseTeleportStone);
				return;
			}
		}
		DialogueSpawner.SpawnQuickInfoDialogue(new LocalizedStringWithArgument("Homes_13288"), new LocalizedStringWithArgument("Homes_13305"), "IconMyHome", 7);
	}

	private void SetPendingHomeTravel(string playerId, string avatarName, bool useTeleportStone)
	{
		if (MilMo_Home.CurrentHome == null || MilMo_Home.CurrentHome.OwnerID != playerId)
		{
			_pendingHomeTravel = new PendingHomeTravel(playerId, avatarName, useTeleportStone);
		}
	}

	private void GoToHome(string playerId, string avatarName, bool useTeleportStone)
	{
		_pendingHomeTravel = null;
		if (MilMo_Player.Instance.InShop)
		{
			SetPendingHomeTravel(playerId, avatarName, useTeleportStone);
			MilMo_Player.Instance.RequestLeaveShop();
		}
		else if (MilMo_Player.Instance.InCharBuilderWorld)
		{
			SetPendingHomeTravel(playerId, avatarName, useTeleportStone);
			MilMo_Player.Instance.RequestLeaveCharBuilder();
		}
		else if (MilMo_Player.Instance.InNavigator)
		{
			SetPendingHomeTravel(playerId, avatarName, useTeleportStone);
			MilMo_Player.Instance.RequestLeaveNavigator();
		}
		else if (MilMo_Player.Instance.InHub)
		{
			SetPendingHomeTravel(playerId, avatarName, useTeleportStone);
			MilMo_Player.Instance.RequestLeaveHub();
		}
		else if (MilMo_Player.Instance.Avatar != null && !MilMo_Player.Instance.Avatar.InCombat && (MilMo_Home.CurrentHome == null || !(MilMo_Home.CurrentHome.OwnerID == playerId)))
		{
			MilMo_Player.Instance.Teleporting = true;
			MilMo_LoadingScreen.Instance.LoadHomeFade(40f, avatarName);
			Singleton<GameNetwork>.Instance.RequestHomeServerInfo(playerId, useTeleportStone);
		}
	}

	private void GotServerLoginInfoForHome(object messageAsObject)
	{
		ServerLoginInfoHome message = messageAsObject as ServerLoginInfoHome;
		if (message != null)
		{
			Debug.Log("Got new server " + message.getHost() + " for home home:" + message.getHomeOwnerId());
			UnloadCurrentInstance();
			MilMo_Player.Instance.Teleporting = true;
			LoginToNewServer(message.getHost(), message.getToken(), delegate
			{
				MilMo_Home.RequestLoadNewItemsAfterFinishLoad = message.getHomeOwnerId() == MilMo_Player.Instance.Id && message.getHomeHasNewItemsInDB() != 0;
			});
		}
	}

	private void GotLoadHomeInfo(object messageAsObject)
	{
		ServerHomeInstanceInfo serverHomeInstanceInfo = (ServerHomeInstanceInfo)messageAsObject;
		if (serverHomeInstanceInfo == null)
		{
			return;
		}
		Debug.Log("Got home instance info message with start room " + serverHomeInstanceInfo.getStartRoomInfo().GetItem().GetId() + " with " + serverHomeInstanceInfo.getStartRoomInfo().GetItemCount() + " pieces of equipment to load.");
		MilMo_LoadingScreen.Instance.CancelTimeout();
		UnloadCurrentInstance();
		MilMo_Player.Instance.Teleporting = false;
		if (MilMo_Player.Instance.InShop)
		{
			MilMo_Player.Instance.RequestLeaveShop();
		}
		else if (MilMo_Player.Instance.InCharBuilderWorld)
		{
			MilMo_Player.Instance.RequestLeaveCharBuilder();
		}
		if (MilMo_Player.Instance.InNavigator)
		{
			MilMo_Player.Instance.InNavigator = false;
			MilMo_WorldMap.Deactivate();
		}
		if (MilMo_Player.Instance.InHub)
		{
			MilMo_Player.Instance.InHub = false;
			MilMo_Hub.Instance.Deactivate();
		}
		MilMo_Player.Instance.Teleporting = true;
		string homeOwnerId = serverHomeInstanceInfo.getHomeOwnerId();
		string homeName = serverHomeInstanceInfo.getHomeName();
		RoomInfo startRoomInfo = serverHomeInstanceInfo.getStartRoomInfo();
		bool hasDefaultEquipmentOnly = serverHomeInstanceInfo.getHasDefaultEquipmentOnly() != 0;
		MilMo_ResourceManager.Instance.SmoothLoading = false;
		MilMo_ResourceManager.Instance.Paused = true;
		_home.AsyncLoad(homeOwnerId, homeName, startRoomInfo, hasDefaultEquipmentOnly, delegate(bool success)
		{
			if (success)
			{
				Debug.Log("Successfully loaded home " + homeName);
				RequestJoinHome();
			}
			else
			{
				MilMo_Player.Instance.Teleporting = false;
				Debug.LogWarning("Failed to load home " + homeOwnerId);
				DialogueSpawner.SpawnWarningModalDialogue(new LocalizedStringWithArgument("Homes_9342"), new LocalizedStringWithArgument("Homes_9341"));
			}
		});
	}

	private void RequestJoinHome()
	{
		if (!MilMo_BodyPackSystem.AllDone || !MilMo_Player.Instance.IsDone)
		{
			if (!MilMo_BodyPackSystem.AllDone)
			{
				Debug.Log("Requesting to join home but body pack system isn't loaded. Retrying in 0.2 seconds...");
			}
			if (!MilMo_Player.Instance.IsDone)
			{
				Debug.Log("Requesting to join home but player isn't fully loaded. Retrying in 0.2 seconds...");
			}
			MilMo_EventSystem.At(0.2f, RequestJoinHome);
			return;
		}
		Camera.HookupTransforms();
		if (!Camera.IsStarted)
		{
			Camera.Startup();
		}
		MilMo_ResourceManager.Instance.SmoothLoading = true;
		MilMo_ResourceManager.Instance.Paused = false;
		Debug.Log("Requesting to enter home");
		MilMo_EventSystem.Listen("join_home", JoinHome);
		Singleton<GameNetwork>.Instance.RequestJoinInstance();
	}

	private void JoinHome(object o)
	{
		Debug.Log("MilMo_World::JoinHome");
		MilMo_Player.Instance.Teleporting = false;
		if (_home == null)
		{
			Debug.LogWarning("Trying to join home before it has been loaded");
			return;
		}
		GameEvent.UpdateHudStateEvent.RaiseEvent(HudState.States.Home);
		MilMo_Player.Instance.EquipSlots.CurrentMode = IWeaponSlots.Mode.All;
		MilMo_Instance.CurrentInstance.IgnoreClickOnObjects = false;
		PvpScoreBoard.Close(null);
		MilMo_AvatarGlobalLODSettings.IsPvpMode = false;
		MilMo_Player.Instance.InPVP = false;
		((MilMoPvpLadderWindow)MilMo_GlobalUI.Instance.GetItem("PvpLadderWindow")).Close(null);
		((MilMoLadderWindow)MilMo_GlobalUI.Instance.GetItem("HomeLadderWindow")).Close(null);
		ChangePlayerController(MilMo_PlayerControllerBase.ControllerType.Home, null);
		Camera.HookupHomeCam();
		PlayerController.Lock(0f, playMoveAnimationOnUnlock: true);
		MilMo_ActiveRoom.EntryPointData currentEntryPoint = _home.CurrentEntryPoint;
		MilMo_Player.Instance.Avatar.GameObject.transform.position = currentEntryPoint.Pos;
		MilMo_PlayerControllerBase.SetRotation(currentEntryPoint.Rot);
		Camera.homeCameraController.SetPan(currentEntryPoint.Rot.eulerAngles.y + 180f);
		Camera.SetupPosition();
		MilMo_Player.Instance.Avatar.EnableBlobShadows();
		GC.Collect();
		Resources.UnloadUnusedAssets();
		MilMo_Player.Instance.InInstance = true;
		if (((ServerLocalPlayerJoinHome)o).getNeedsHomeBoxPosition() != 0)
		{
			_home.GenerateHomeDeliveryBoxPosition(null);
		}
		MilMo_Player.Instance.UpdateInDialogue(inDialogue: false);
		if (MilMo_Level.FirstTimeEnterLevel)
		{
			MilMo_Level.FirstTimeEnterLevel = false;
		}
		_home.StartPlayMusic();
		MilMo_UserInterface.ResetCameraRect();
		MilMo_Player.Instance.ExitAllNpcRange();
		if (MilMo_Home.RequestLoadNewItemsAfterFinishLoad)
		{
			Singleton<GameNetwork>.Instance.RequestCheckForNewHomeItems();
		}
		Singleton<Analytics>.Instance.LevelStart("Home", 0f);
		MilMo_EventSystem.At(1f, delegate
		{
			PlayerController.Unlock();
			MilMo_LoadingScreen.Instance.Hide();
			if (MilMo_Home.CurrentHome != null)
			{
				MilMo_Home.CurrentHome.PlayEnterRoomSound();
			}
			if (MilMo_Player.InMyHome)
			{
				MilMo_EventSystem.Instance.PostEvent("tutorial_EnterOwnHome", "");
			}
		});
	}

	private static void InitializeOpenTownListener()
	{
		GameEvent.OpenTownEvent.RegisterAction(OpenTown);
	}

	private static void OpenTown()
	{
		if (MilMo_Player.Instance == null)
		{
			Debug.Log("MilMo_Player is null");
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
			GameEvent.OpenTownFailEvent.RaiseEvent();
		}
	}

	private void HandleHubResponse(object msgAsObject)
	{
		MilMo_Player.Instance.PendingHubRequest = null;
		if (!(msgAsObject is ServerHubResponse serverHubResponse))
		{
			return;
		}
		bool flag = !MilMo_Player.Instance.InHub;
		string text = (flag ? "Enter" : "leave");
		if (serverHubResponse.getHubResult() != 0)
		{
			Debug.LogWarning("Failed to " + text + " Town");
			return;
		}
		if (flag)
		{
			EnterHub();
		}
		else
		{
			LeaveHub();
		}
		MilMo_Player.Instance.Avatar.DisableRagdoll();
		MilMo_Player.Instance.InPVP = false;
	}

	private void GotHubInfo(object msgAsObj)
	{
		if (!(msgAsObj is ServerStartScreenInfo serverStartScreenInfo))
		{
			return;
		}
		if (MilMo_Player.Instance.InHub || MilMo_Player.Instance.EnteringHub || (MilMo_Instance.CurrentInstance != null && serverStartScreenInfo.getUnloadCurrentInstance() == 0))
		{
			MilMo_Hub.Instance.ReadInfoMessage(serverStartScreenInfo);
			return;
		}
		MilMo_LoadingScreen.Instance.LoadTownFade();
		if (!MilMo_BodyPackSystem.AllDone || !MilMo_Player.Instance.IsDone)
		{
			MilMo_EventSystem.At(0.2f, delegate
			{
				GotHubInfo(msgAsObj);
			});
			return;
		}
		if (serverStartScreenInfo.getUnloadCurrentInstance() != 0 && MilMo_Instance.CurrentInstance != null)
		{
			UnloadCurrentInstance();
		}
		if (serverStartScreenInfo.getLastAdventureLevel().Contains(":"))
		{
			MilMo_Level.LastAdventureLevel = serverStartScreenInfo.getLastAdventureLevel();
			CurrentWorld = serverStartScreenInfo.getLastAdventureLevel().Split(':')[0];
		}
		MilMo_Hub.Instance.ReadInfoMessage(serverStartScreenInfo);
		if (!Camera.IsStarted)
		{
			Camera.Startup();
		}
		MilMo_Player.Instance.RequestEnterHub();
	}

	private void EnterHub()
	{
		MilMo_Player.Instance.EnteringHub = true;
		Analytics.ScreenVisit("EnterTown");
		MilMo_LoadingScreen.Instance.LoadTownFade();
		MilMo_EventSystem.At(1f, delegate
		{
			if (MilMo_Level.CurrentLevel != null && MilMo_LevelInfo.GetLevelInfoData(MilMo_Level.CurrentLevel.VerboseName).IsPvp && MilMo_Player.Instance != null && MilMo_Player.Instance.Avatar != null)
			{
				MilMo_Player.Instance.Avatar.StopParticleEffect("PvpNoDamage");
			}
			MilMo_GlobalUI.Instance.ClosePanels();
			ChangePlayerController(MilMo_PlayerControllerBase.ControllerType.InGUIApp);
			MilMo_Hub.Instance.Activate(delegate(bool success)
			{
				MilMo_LoadingScreen.Instance.Hide(enableWorldUI: false);
				if (MilMo_Level.CurrentLevel != null && MilMo_Level.CurrentLevel.AmbientSoundManager != null)
				{
					MilMo_Level.CurrentLevel.AmbientSoundManager.Mute();
				}
				if (MilMo_Home.CurrentHome != null)
				{
					MilMo_Home.CurrentHome.MuteFurnitureSounds();
				}
				MilMo_EventSystem.At(0.5f, delegate
				{
					MilMo_Player.Instance.InHub = true;
					MilMo_Player.Instance.EnteringHub = false;
				});
				if (!success)
				{
					Debug.LogWarning("Error. Could not create the hub.");
				}
				MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_EnterTown");
			});
		});
	}

	private void LeaveHub()
	{
		MilMo_Player.Instance.LeavingHub = true;
		Analytics.ScreenVisit("LeaveTown");
		MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_LeaveTown");
		if (MilMo_Player.Instance.PendingTeleportToFriendMessage != null)
		{
			string text = MilMo_Player.Instance.PendingTeleportToFriendMessage.getHomeOwnerId().ToString();
			if (text.Length == 0)
			{
				MilMo_LoadingScreen.Instance.LevelLoadFade(12f);
			}
			else
			{
				MilMo_LoadingScreen.Instance.LoadHomeFade(40f, MilMo_ProfileManager.GetStoredName(text));
			}
			MilMo_EventSystem.At(0.8f, delegate
			{
				MilMo_Hub.Instance.Deactivate();
				MilMo_Hub.WasTravelClosed = false;
				MilMo_Player.Instance.LeavingHub = false;
				MilMo_Player.Instance.InHub = false;
				MilMo_EventSystem.Instance.AsyncPostEvent("teleport_to_friend_ok", MilMo_Player.Instance.PendingTeleportToFriendMessage);
				GC.Collect();
				Resources.UnloadUnusedAssets();
				Activate();
			});
			return;
		}
		if (MilMo_Player.Instance.PendingHomeKick)
		{
			MilMo_LevelInfoData levelInfo = MilMo_LevelInfo.GetLevelInfoData(MilMo_Level.LastAdventureLevel);
			MilMo_LoadingScreen.Instance.LevelLoadFade(12f, "", forceNoResetAlpha: true);
			MilMo_EventSystem.At(0.25f, delegate
			{
				MilMo_LocString locString = MilMo_Localization.GetLocString("Generic_56");
				locString.SetFormatArgs(levelInfo.DisplayName);
				MilMo_LoadingScreen.Instance.SetLoadingText(locString);
				MilMo_LevelData.LoadAndSetLevelIcon(levelInfo.World, levelInfo.Level);
			});
			MilMo_EventSystem.At(1.5f, delegate
			{
				MilMo_Hub.Instance.Deactivate();
				MilMo_Hub.WasTravelClosed = false;
				MilMo_Player.Instance.InHub = false;
				MilMo_Player.Instance.LeavingHub = false;
				Activate();
			});
			MilMo_Player.ShowKickedFromHomeQuickInfo();
			MilMo_Player.Instance.PendingHomeKick = false;
			return;
		}
		if (MilMo_Hub.WasTravelClosed)
		{
			MilMo_LevelInfo.Travel(MilMo_Hub.TravelClosedFullLevelName, MilMo_LevelInfo.GetLevelInfoData(MilMo_Hub.TravelClosedFullLevelName).IsChatRoom);
			MilMo_EventSystem.At(0.8f, delegate
			{
				MilMo_Hub.Instance.Deactivate();
				MilMo_Hub.WasTravelClosed = false;
				MilMo_Player.Instance.InNavigator = false;
				MilMo_Player.Instance.InHub = false;
				MilMo_Player.Instance.LeavingHub = false;
				Activate();
			});
			ChangePlayerController(MilMo_PlayerControllerInGUIApp.PreviousControllerType);
			return;
		}
		if (_pendingHomeTravel != null && (MilMo_Home.CurrentHome == null || MilMo_Home.CurrentHome.OwnerID != _pendingHomeTravel.PlayerId))
		{
			MilMo_LoadingScreen.Instance.LoadHomeFade(40f, _pendingHomeTravel.AvatarName);
			Singleton<GameNetwork>.Instance.RequestHomeServerInfo(_pendingHomeTravel.PlayerId, useTeleportStone: false);
			MilMo_EventSystem.At(1.5f, delegate
			{
				MilMo_Hub.Instance.Deactivate();
				MilMo_Hub.WasTravelClosed = false;
				MilMo_Player.Instance.InHub = false;
				MilMo_Player.Instance.LeavingHub = false;
				GC.Collect();
				Resources.UnloadUnusedAssets();
				Activate();
			});
			_pendingHomeTravel = null;
			ChangePlayerController(MilMo_PlayerControllerInGUIApp.PreviousControllerType);
			return;
		}
		_pendingHomeTravel = null;
		if (MilMo_Instance.CurrentInstance == null)
		{
			MilMo_LevelInfo.Travel(MilMo_Level.LastAdventureLevel, isChatroom: false, checkUnlocked: false);
			MilMo_EventSystem.At(0.8f, delegate
			{
				MilMo_Hub.Instance.Deactivate();
				MilMo_Hub.WasTravelClosed = false;
				MilMo_Player.Instance.InHub = false;
				MilMo_Player.Instance.LeavingHub = false;
				GC.Collect();
				Resources.UnloadUnusedAssets();
				Activate();
			});
			ChangePlayerController(MilMo_PlayerControllerInGUIApp.PreviousControllerType);
			return;
		}
		MilMo_Fade.Instance.FadeOutAll();
		MilMo_Hub.Instance.Deactivate();
		GC.Collect();
		Resources.UnloadUnusedAssets();
		Activate();
		ChangePlayerController(MilMo_PlayerControllerInGUIApp.PreviousControllerType);
		Instance.Camera.HookupCurrentPlayCamera();
		MilMo_Instance.CurrentInstance.StartPlayMusic();
		if (MilMo_Level.CurrentLevel != null && MilMo_Level.CurrentLevel.AmbientSoundManager != null)
		{
			MilMo_Level.CurrentLevel.AmbientSoundManager.Unmute();
		}
		if (MilMo_Home.CurrentHome != null)
		{
			MilMo_Home.CurrentHome.UnmuteFurnitureSounds();
		}
		MilMo_EventSystem.At(2f, delegate
		{
			MilMo_Player.Instance.InHub = false;
			MilMo_Player.Instance.LeavingHub = false;
		});
	}

	private void GotServerLoginInfoForLevel(object messageAsObject)
	{
		if (messageAsObject is ServerLoginInfoLevel serverLoginInfoLevel)
		{
			Debug.Log("Got new server " + serverLoginInfoLevel.getHost() + " for level " + serverLoginInfoLevel.getWorldName() + ":" + serverLoginInfoLevel.getLevelName() + ":" + serverLoginInfoLevel.getLanguage() + ":" + serverLoginInfoLevel.getInstanceId());
			UnloadCurrentInstance();
			MilMo_Player.Instance.Teleporting = true;
			LoginToNewServer(serverLoginInfoLevel.getHost(), serverLoginInfoLevel.getToken(), null);
		}
	}

	private void GotLoadLevelInfo(object messageAsObject)
	{
		if (!(messageAsObject is ServerLevelInstanceInfo serverLevelInstanceInfo))
		{
			return;
		}
		Debug.Log("Got load level message " + serverLevelInstanceInfo.getWorldName() + ":" + serverLevelInstanceInfo.getLevelName() + ":" + serverLevelInstanceInfo.getLanguage() + ":" + serverLevelInstanceInfo.getInstanceId());
		MilMo_LoadingScreen.Instance.CancelTimeout();
		UnloadCurrentInstance();
		MilMo_Player.Instance.Teleporting = false;
		if (MilMo_Player.Instance.InShop)
		{
			MilMo_Player.Instance.RequestLeaveShop();
		}
		else if (MilMo_Player.Instance.InCharBuilderWorld)
		{
			MilMo_Player.Instance.RequestLeaveCharBuilder();
		}
		if (MilMo_Player.Instance.InNavigator)
		{
			MilMo_Player.Instance.InNavigator = false;
			MilMo_WorldMap.Deactivate();
		}
		if (MilMo_Player.Instance.InHub)
		{
			MilMo_Player.Instance.InHub = false;
			MilMo_Hub.Instance.Deactivate();
		}
		MilMo_Player.Instance.Teleporting = true;
		_latestLevelLoadStartTime = Time.realtimeSinceStartup;
		string worldName = serverLevelInstanceInfo.getWorldName();
		string levelName = serverLevelInstanceInfo.getLevelName();
		string language = serverLevelInstanceInfo.getLanguage();
		string instanceId = serverLevelInstanceInfo.getInstanceId();
		Vector3 entryPoint = new Vector3(serverLevelInstanceInfo.getEntryPoint().GetX(), serverLevelInstanceInfo.getEntryPoint().GetY(), serverLevelInstanceInfo.getEntryPoint().GetZ());
		if (MilMo_LoadingScreen.Instance.LoadingState != MilMo_LoadingScreen.State.LoadLevel)
		{
			MilMo_LoadingScreen.Instance.LevelLoadFade();
		}
		string fullLevelName = worldName + ":" + levelName;
		MilMo_LocString locString = MilMo_Localization.GetLocString("Generic_56");
		locString.SetFormatArgs(MilMo_LevelInfo.GetLevelDisplayName(fullLevelName));
		MilMo_LoadingScreen.Instance.SetLoadingText(locString);
		MilMo_LevelData.LoadAndSetLevelIcon(worldName, levelName);
		MilMo_ResourceManager.Instance.SmoothLoading = false;
		MilMo_ResourceManager.Instance.Paused = true;
		_level.AsyncLoad(worldName, levelName, language, instanceId, entryPoint, delegate(bool success)
		{
			if (success)
			{
				Debug.Log("Successfully loaded level " + fullLevelName);
				_requestToEnterLevelStartTime = Time.realtimeSinceStartup;
				_requestToEnterLevelDialog = false;
				RequestJoinLevel();
			}
			else
			{
				MilMo_Player.Instance.Teleporting = false;
				Debug.LogWarning("Failed to load level " + fullLevelName);
				LocalizedStringWithArgument message = new LocalizedStringWithArgument("World_425", fullLevelName);
				DialogueSpawner.SpawnWarningModalDialogue(new LocalizedStringWithArgument("World_424"), message);
			}
			_firstLevelLoad = false;
		});
	}

	private void RequestJoinLevel()
	{
		if (Time.realtimeSinceStartup - _requestToEnterLevelStartTime > 40f && !_requestToEnterLevelDialog)
		{
			Debug.Log("Request enter level timed out (" + (Time.realtimeSinceStartup - _requestToEnterLevelStartTime) + ")");
			DialogueSpawner.SpawnWarningModalDialogue(new LocalizedStringWithArgument("World_426"), new LocalizedStringWithArgument("World_427"));
			_requestToEnterLevelDialog = true;
		}
		if (!MilMo_BodyPackSystem.AllDone || !MilMo_Player.Instance.IsDone)
		{
			if (!MilMo_BodyPackSystem.AllDone)
			{
				Debug.Log("Requesting to enter level but body pack system isn't loaded. Retrying in 0.2 seconds...");
			}
			if (!MilMo_Player.Instance.IsDone)
			{
				Debug.Log("Requesting to enter level but player isn't fully loaded. Retrying in 0.2 seconds...");
			}
			MilMo_EventSystem.At(0.2f, RequestJoinLevel);
			return;
		}
		Camera.HookupTransforms();
		if (!Camera.IsStarted)
		{
			Camera.Startup();
		}
		MilMo_ResourceManager.Instance.SmoothLoading = true;
		MilMo_ResourceManager.Instance.Paused = false;
		Debug.Log("Requesting to join level");
		MilMo_EventSystem.Listen("join_level", JoinLevel);
		Singleton<GameNetwork>.Instance.RequestJoinInstance();
	}

	private void JoinLevel(object o)
	{
		Debug.Log("MilMo_World::JoinLevel");
		MilMo_Player.Instance.Teleporting = false;
		ServerLocalPlayerJoinLevel msg = o as ServerLocalPlayerJoinLevel;
		if (msg == null)
		{
			Debug.LogWarning("Level data message is null");
			return;
		}
		if (_level == null)
		{
			Debug.LogWarning("Trying to enter level before it has been loaded");
			return;
		}
		MilMo_LevelInfoData levelInfoData = MilMo_LevelInfo.GetLevelInfoData(MilMo_Level.CurrentLevel.VerboseName);
		GameEvent.UpdateHudStateEvent.RaiseEvent(levelInfoData.HudState);
		if (levelInfoData.IsPvp)
		{
			MilMo_EventSystem.Instance.PostEvent("tutorial_JoinPVP", "");
			if (MilMo_Player.Instance.EquipSlots != null)
			{
				MilMo_Player.Instance.EquipSlots.CurrentMode = IWeaponSlots.Mode.AllExceptEmpty;
			}
			MilMo_Instance.CurrentInstance.IgnoreClickOnObjects = true;
			MilMo_AvatarGlobalLODSettings.IsPvpMode = true;
			MilMo_Player.Instance.InPVP = true;
			MilMo_Player.Instance.Avatar.SetInvulnerable(value: true);
		}
		else
		{
			if (MilMo_Player.Instance.EquipSlots != null)
			{
				MilMo_Player.Instance.EquipSlots.CurrentMode = IWeaponSlots.Mode.All;
			}
			MilMo_Instance.CurrentInstance.IgnoreClickOnObjects = false;
			PvpScoreBoard.Close(null);
			MilMo_AvatarGlobalLODSettings.IsPvpMode = false;
			MilMo_Player.Instance.InPVP = false;
		}
		((MilMoPvpLadderWindow)MilMo_GlobalUI.Instance.GetItem("PvpLadderWindow")).Close(null);
		((MilMoLadderWindow)MilMo_GlobalUI.Instance.GetItem("HomeLadderWindow")).Close(null);
		ChangePlayerController(MilMo_PlayerControllerBase.ControllerType.Game, null);
		Camera.HookupGameCam();
		PlayerController.Lock(0f, playMoveAnimationOnUnlock: true);
		Vector3 playerPos = new Vector3(msg.getPlayerPosition().GetX(), msg.getPlayerPosition().GetY(), msg.getPlayerPosition().GetZ());
		MilMo_Player.Instance.Avatar.GameObject.transform.position = playerPos;
		MilMo_PlayerControllerBase.SetRotation(Quaternion.AngleAxis(msg.getPlayerRotation(), Vector3.up));
		Camera.SetupPosition();
		MilMo_Player.Instance.Avatar.EnableBlobShadows();
		GC.Collect();
		Resources.UnloadUnusedAssets();
		MilMo_Player.Instance.InInstance = true;
		if (msg.getMembershipTimeLeftInDays() >= 0)
		{
			MilMo_Player.Instance.SetMembershipDaysLeft(msg.getMembershipTimeLeftInDays() + 1);
		}
		if (msg.getGemBonusTimeLeftInSeconds() > 0)
		{
			GlobalStates.Instance.playerState.gemBonusTimeLeftInSeconds.Set(msg.getGemBonusTimeLeftInSeconds());
		}
		MilMo_Player.Instance.UpdateInDialogue(inDialogue: false);
		if (MilMo_Level.FirstTimeEnterLevel)
		{
			MilMo_Level.FirstTimeEnterLevel = false;
		}
		_level.StartPlayMusic();
		MilMo_UserInterface.ResetCameraRect();
		MilMo_Player.Instance.ExitAllNpcRange();
		MilMo_Player.Instance.RefreshWieldable();
		MilMo_EventSystem.At(1f, async delegate
		{
			Camera.Reset();
			Activate();
			PlayerController.Unlock();
			MilMo_Level.CurrentLevel.StartAmbienceSounds();
			bool flag = false;
			if (!string.IsNullOrEmpty(msg.getRoom()))
			{
				MilMo_LoadingScreen.Instance.LoadRoomFade(12f, "Content/Sounds/Batch01/Player/PlayerSpawn");
				MilMo_Level.CurrentLevel.PlayerChangeRoom(MilMo_Player.Instance.Avatar.Id, msg.getRoom(), playerPos, msg.getPlayerRotation());
				flag = true;
			}
			else if (!string.IsNullOrEmpty(MilMo_Player.Instance.Avatar.Room))
			{
				MilMo_Level.CurrentLevel.SetOutdoorState();
			}
			MilMo_LevelInfo.HasBeenInLevel(MilMo_Level.CurrentLevel.VerboseName);
			bool flag2 = false;
			if (!MilMo_LevelInfo.IsGUIUnlocked(MilMo_Level.CurrentLevel.VerboseName) && !levelInfoData.IsChatRoom && levelInfoData.HudState != HudState.States.StarterLevel)
			{
				if (!levelInfoData.SkipNavigatorFeed)
				{
					MilMo_WorldMap.AutoOpen();
				}
				else
				{
					flag2 = true;
				}
				MilMo_LevelInfo.UnlockGUI(MilMo_Level.CurrentLevel.VerboseName);
			}
			else if (!flag)
			{
				flag2 = true;
			}
			if (flag2)
			{
				MilMo_SFFile milMo_SFFile = null;
				if (levelInfoData.HudState == HudState.States.StarterLevel)
				{
					string worldContentName = MilMo_Level.CurrentLevel.WorldContentName;
					string levelContentName = MilMo_Level.CurrentLevel.LevelContentName;
					milMo_SFFile = await MilMo_SimpleFormat.RealAsyncLoad("Content/CameraScripts/" + worldContentName + levelContentName + "Intro");
					milMo_SFFile?.NextRow();
				}
				MilMo_LoadingScreen.Instance.Hide();
				MilMo_Player.Instance.Avatar.PlaySoundEffect("Content/Sounds/Batch01/Player/PlayerSpawn");
				if (milMo_SFFile != null)
				{
					MilMo_Camera.Instance.ExecuteScript(milMo_SFFile);
				}
			}
			float num = Time.realtimeSinceStartup - _latestLevelLoadStartTime;
			if (_statsFirstLevelLoad)
			{
				Debug.Log($"Startup LevelLoad for {MilMo_Level.CurrentLevel.VerboseName} took {num} seconds");
				_statsFirstLevelLoad = false;
			}
			else
			{
				Debug.Log($"LevelLoad for {MilMo_Level.CurrentLevel.VerboseName} took {num} seconds");
			}
			Singleton<Analytics>.Instance.LevelStart(MilMo_Level.CurrentLevel.VerboseName, num);
			MilMo_EventSystem.At(4f, CreateEnterLevelQuickInfoDialog);
		});
	}

	private static void CreateEnterLevelQuickInfoDialog()
	{
		if (MilMo_Level.CurrentLevel == null)
		{
			return;
		}
		bool flag = false;
		string text = "";
		foreach (var (_, milMo_RemotePlayer2) in MilMo_Level.CurrentLevel.Players)
		{
			if (Singleton<MilMo_BuddyBackend>.Instance.IsBuddy(milMo_RemotePlayer2.Avatar.Id) && !Singleton<GroupManager>.Instance.InGroup(milMo_RemotePlayer2.Avatar.Id))
			{
				flag = true;
				text = text + milMo_RemotePlayer2.Avatar.Name + ", ";
			}
		}
		if (flag)
		{
			text = text.Remove(text.Length - 2);
			string iconKey = "LevelIcon" + MilMo_Level.CurrentLevel.WorldContentName + MilMo_Level.CurrentLevel.LevelContentName;
			DialogueSpawner.SpawnQuickInfoDialogue(new LocalizedStringWithArgument("World_7330"), new LocalizedStringWithArgument(text), iconKey, 6);
		}
	}

	private void HandleMakeoverStudioResponse(object msgAsObject)
	{
		MilMo_Player.Instance.PendingCharBuilderRequest = null;
		if (!(msgAsObject is ServerEnterCharBuilderResponse serverEnterCharBuilderResponse))
		{
			return;
		}
		if (!MilMo_Player.Instance.InCharBuilderWorld)
		{
			if (serverEnterCharBuilderResponse.getEnterResult() == 0)
			{
				EnterMakeoverStudio();
			}
			else
			{
				Debug.LogWarning("Failed to enter char builder");
			}
		}
		else if (serverEnterCharBuilderResponse.getEnterResult() == 0)
		{
			LeaveMakeoverStudio();
		}
		else
		{
			Debug.LogWarning("Failed to leave char builder");
		}
	}

	private void EnterMakeoverStudio()
	{
		MilMo_Player.Instance.EnteringCharBuilderWorld = true;
		Analytics.ScreenVisit("EnterMakeoverStudio");
		returnPosition = MilMo_Player.Instance.Avatar.GameObject.transform.position;
		returnRotation = MilMo_Player.Instance.Avatar.GameObject.transform.rotation;
		_backupBackgroundColor = MilMo_Global.Camera.backgroundColor;
		_backupCameraRect = MilMo_Global.Camera.rect;
		ChangePlayerController(MilMo_PlayerControllerBase.ControllerType.InGUIApp, null);
		MilMo_LoadingScreen.Instance.MakeOverStudioFade();
		MilMo_EventSystem.At(1f, delegate
		{
			Camera.UnhookAll();
			CreateMakeOverStudio();
			if (MilMo_Level.CurrentLevel != null && MilMo_Level.CurrentLevel.AmbientSoundManager != null)
			{
				MilMo_Level.CurrentLevel.AmbientSoundManager.Mute();
			}
			if (MilMo_Player.Instance.InNavigator)
			{
				MilMo_WorldMap.Deactivate();
			}
			if (MilMo_Player.Instance.InHub)
			{
				MilMo_Hub.Instance.Deactivate();
			}
			Deactivate();
			if (MilMo_Home.CurrentHome != null)
			{
				MilMo_Home.CurrentHome.MuteFurnitureSounds();
			}
			RenderSettings.fog = false;
			MilMo_Player.Instance.Silhouette.Enable(enable: false);
			MilMo_Player.Instance.Avatar.DisablePuffs();
			MilMo_Player.Instance.Avatar.DisableBlobShadows();
			MilMo_Player.Instance.Avatar.PlayAnimation("Idle");
			GC.Collect();
			Resources.UnloadUnusedAssets();
			MilMo_EventSystem.At(1.5f, delegate
			{
				MilMo_Player.Instance.EnteringCharBuilderWorld = false;
				MilMo_Player.Instance.InCharBuilderWorld = true;
			});
		});
	}

	private void LeaveMakeoverStudio()
	{
		MilMo_Player.Instance.LeavingCharBuilderWorld = true;
		Analytics.ScreenVisit("LeaveMakeoverStudio");
		bool wasInHub = MilMo_Player.Instance.InHub;
		MilMo_Fade.Instance.FadeInBackground();
		MilMo_EventSystem.At(0.5f, delegate
		{
			DestroyMakeOverStudio();
			Activate();
			if (MilMo_Level.CurrentLevel != null && MilMo_Level.CurrentLevel.AmbientSoundManager != null)
			{
				MilMo_Level.CurrentLevel.AmbientSoundManager.Unmute();
			}
			if (MilMo_Level.CurrentLevel != null)
			{
				MilMo_Instance.CurrentInstance.StartPlayMusic();
			}
			Camera.DefaultCameraSettings();
			MilMo_Global.Camera.backgroundColor = _backupBackgroundColor;
			MilMo_Global.Camera.rect = _backupCameraRect;
			if (MilMo_Instance.CurrentInstance != null)
			{
				MilMo_Instance.CurrentInstance.Environment.RestoreRenderSettings();
			}
			ChangePlayerController(MilMo_PlayerControllerInGUIApp.PreviousControllerType);
			MilMo_PlayerControllerBase.SetRotation(returnRotation);
			MilMo_PlayerControllerBase.SetPosition(returnPosition);
			Camera.HookupCurrentPlayCamera();
			MilMo_Player.Instance.Avatar.StopSitting();
			MilMo_Player.Instance.Avatar.IdleAnimation = "LandIdle";
			MilMo_Player.Instance.Avatar.PlayAnimation(MilMo_Player.Instance.Avatar.IdleAnimation);
			MilMo_Player.Instance.Avatar.EnableBlobShadows();
			MilMo_Player.Instance.Avatar.EnablePuffs();
			MilMo_Player.Instance.Silhouette.Enable(enable: true);
			MilMo_Player.Instance.LeavingCharBuilderWorld = false;
			MilMo_Player.Instance.InCharBuilderWorld = false;
			if (MilMo_Player.Instance.PendingHomeKick)
			{
				if (!wasInHub)
				{
					MilMo_LoadingScreen.Instance.LevelLoadFade(12f);
					MilMo_LevelInfoData levelInfo = MilMo_LevelInfo.GetLevelInfoData(MilMo_Level.LastAdventureLevel);
					MilMo_EventSystem.At(0.25f, delegate
					{
						MilMo_LocString locString = MilMo_Localization.GetLocString("Generic_56");
						locString.SetFormatArgs(levelInfo.DisplayName);
						MilMo_LoadingScreen.Instance.SetLoadingText(locString);
						MilMo_LevelData.LoadAndSetLevelIcon(levelInfo.World, levelInfo.Level);
						MilMo_Player.ShowKickedFromHomeQuickInfo();
						MilMo_Player.Instance.PendingHomeKick = false;
					});
				}
				else
				{
					MilMo_Player.Instance.RequestLeaveHub();
				}
			}
			else if (_pendingHomeTravel != null)
			{
				GoToHome(_pendingHomeTravel.PlayerId, _pendingHomeTravel.AvatarName);
			}
			else if (MilMo_Player.Instance.InHub)
			{
				MilMo_Hub.Instance.Activate(delegate
				{
					MilMo_Fade.Instance.FadeOutAll();
				});
			}
			else
			{
				MilMo_EventSystem.At(0.5f, delegate
				{
					MilMo_Fade.Instance.FadeOutAll();
				});
			}
		});
	}

	private MilMo_MakeOverStudio CreateMakeOverStudio()
	{
		MilMo_MakeOverStudio milMo_MakeOverStudio = base.gameObject.AddComponent<MilMo_MakeOverStudio>();
		if (milMo_MakeOverStudio == null)
		{
			Debug.Log("Failed to add component MilMo_MakeOverStudio");
		}
		return milMo_MakeOverStudio;
	}

	public void DestroyMakeOverStudio()
	{
		MilMo_MakeOverStudio component = base.gameObject.GetComponent<MilMo_MakeOverStudio>();
		if (!(component == null))
		{
			component.Leave();
		}
	}

	private static void InitializeOpenNavigatorListener()
	{
		GameEvent.OpenNavigatorEvent.RegisterAction(OpenNavigator);
	}

	private static void OpenNavigator()
	{
		if (MilMo_Player.Instance == null)
		{
			Debug.Log("MilMo_Player is null");
			return;
		}
		if (!MilMo_Player.Instance.OkToEnterNavigator())
		{
			GameEvent.OpenNavigatorFailEvent.RaiseEvent();
			return;
		}
		if (MilMo_Home.CurrentHome == null)
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Confirm);
		}
		MilMo_WorldMap.Toggle();
	}

	private void HandleWorldMapResponse(object msgAsObject)
	{
		MilMo_Player.Instance.PendingNavigatorRequest = null;
		if (!(msgAsObject is ServerWorldMapResponse serverWorldMapResponse))
		{
			return;
		}
		if (serverWorldMapResponse.getWorldMapResult() == 0)
		{
			MilMo_Player.Instance.Avatar.DisableRagdoll();
		}
		if (!MilMo_Player.Instance.InNavigator)
		{
			if (serverWorldMapResponse.getWorldMapResult() == 0)
			{
				EnterNavigator();
			}
			else
			{
				Debug.Log("Failed to enter world map");
			}
		}
		else if (serverWorldMapResponse.getWorldMapResult() == 0)
		{
			LeaveNavigator();
		}
		else
		{
			Debug.LogWarning("Failed to leave world map");
		}
	}

	private void EnterNavigator()
	{
		MilMo_Player.Instance.EnteringNavigator = true;
		Analytics.ScreenVisit("EnterWorldMap");
		returnPosition = MilMo_Player.Instance.Avatar.GameObject.transform.position;
		returnRotation = MilMo_Player.Instance.Avatar.GameObject.transform.rotation;
		MilMo_LoadingScreen.Instance.LoadNavigatorFade();
		MilMo_EventSystem.At(1f, delegate
		{
			bool wasInHub = MilMo_Player.Instance.InHub;
			if (wasInHub)
			{
				MilMo_Hub.Instance.Deactivate();
				Camera.HookupCurrentPlayCamera();
			}
			Deactivate();
			Camera.enabled = true;
			ChangePlayerController(MilMo_PlayerControllerBase.ControllerType.InGUIApp);
			GC.Collect();
			Resources.UnloadUnusedAssets();
			MilMo_WorldMap.Activate(delegate
			{
				((MilMo_NavigatorMenu)MilMo_GlobalUI.Instance.GetItem("NavigatorMenu")).NavigatorButton.SetState(wasInHub);
				MilMo_LoadingScreen.Instance.Hide(enableWorldUI: false);
				if (MilMo_Home.CurrentHome != null)
				{
					MilMo_Home.CurrentHome.MuteFurnitureSounds();
				}
				MilMo_EventSystem.At(2f, delegate
				{
					MilMo_Player.Instance.InNavigator = true;
					MilMo_Player.Instance.EnteringNavigator = false;
					if (!MilMo_WorldMap.WasAutoOpened)
					{
						MilMo_EventSystem.Instance.PostEvent("tutorial_WorldMap", "");
					}
				});
			}, CurrentWorld);
		});
	}

	private void LeaveNavigator()
	{
		MilMo_Player.Instance.LeavingNavigator = true;
		Analytics.ScreenVisit("LeaveWorldMap");
		if (MilMo_Player.Instance.PendingTeleportToFriendMessage != null)
		{
			string text = MilMo_Player.Instance.PendingTeleportToFriendMessage.getHomeOwnerId().ToString();
			if (text.Length == 0)
			{
				MilMo_LoadingScreen.Instance.LevelLoadFade(12f);
			}
			else
			{
				MilMo_LoadingScreen.Instance.LoadHomeFade(40f, MilMo_ProfileManager.GetStoredName(text));
			}
			MilMo_EventSystem.At(0.8f, delegate
			{
				MilMo_WorldMap.WasTravelClosed = false;
				MilMo_Player.Instance.InNavigator = false;
				MilMo_Player.Instance.LeavingNavigator = false;
				MilMo_Player.Instance.InHub = false;
				MilMo_EventSystem.Instance.AsyncPostEvent("teleport_to_friend_ok", MilMo_Player.Instance.PendingTeleportToFriendMessage);
				GC.Collect();
				Resources.UnloadUnusedAssets();
				Activate();
			});
			return;
		}
		if (MilMo_Player.Instance.PendingHomeKick)
		{
			MilMo_LevelInfoData levelInfo = MilMo_LevelInfo.GetLevelInfoData(MilMo_Level.LastAdventureLevel);
			MilMo_LoadingScreen.Instance.LevelLoadFade(12f, "", forceNoResetAlpha: true);
			MilMo_EventSystem.At(0.25f, delegate
			{
				MilMo_LocString locString = MilMo_Localization.GetLocString("Generic_56");
				locString.SetFormatArgs(levelInfo.DisplayName);
				MilMo_LoadingScreen.Instance.SetLoadingText(locString);
				MilMo_LevelData.LoadAndSetLevelIcon(levelInfo.World, levelInfo.Level);
			});
			MilMo_EventSystem.At(0.8f, delegate
			{
				MilMo_WorldMap.Deactivate();
				MilMo_WorldMap.WasTravelClosed = false;
				MilMo_Player.Instance.InNavigator = false;
				MilMo_Player.Instance.LeavingNavigator = false;
				MilMo_Player.Instance.InHub = false;
				Activate();
			});
			MilMo_Player.ShowKickedFromHomeQuickInfo();
			MilMo_Player.Instance.PendingHomeKick = false;
			return;
		}
		if (MilMo_WorldMap.WasTravelClosed)
		{
			MilMo_LevelInfo.Travel(MilMo_WorldMap.TravelClosedFullLevelName, MilMo_LevelInfo.GetLevelInfoData(MilMo_WorldMap.TravelClosedFullLevelName).IsChatRoom);
			MilMo_EventSystem.At(0.8f, delegate
			{
				MilMo_WorldMap.Deactivate();
				MilMo_WorldMap.WasTravelClosed = false;
				MilMo_Player.Instance.InNavigator = false;
				MilMo_Player.Instance.LeavingNavigator = false;
				MilMo_Player.Instance.InHub = false;
				Activate();
			});
			ChangePlayerController(MilMo_PlayerControllerInGUIApp.PreviousControllerType);
			return;
		}
		if (_pendingHomeTravel != null)
		{
			if (MilMo_Player.Instance.InHub)
			{
				MilMo_WorldMap.Deactivate();
				MilMo_Player.Instance.InNavigator = false;
				MilMo_Player.Instance.LeavingNavigator = false;
				MilMo_Player.Instance.RequestLeaveHub();
			}
			else
			{
				MilMo_LoadingScreen.Instance.LoadHomeFade(40f, _pendingHomeTravel.AvatarName);
				Singleton<GameNetwork>.Instance.RequestHomeServerInfo(_pendingHomeTravel.PlayerId, _pendingHomeTravel.UseTeleportStone);
				MilMo_EventSystem.At(1.5f, delegate
				{
					MilMo_WorldMap.Deactivate();
					MilMo_WorldMap.WasTravelClosed = false;
					MilMo_Player.Instance.InNavigator = false;
					MilMo_Player.Instance.LeavingNavigator = false;
					MilMo_Player.Instance.InHub = false;
					GC.Collect();
					Resources.UnloadUnusedAssets();
					Activate();
				});
			}
			_pendingHomeTravel = null;
			ChangePlayerController(MilMo_PlayerControllerInGUIApp.PreviousControllerType);
			return;
		}
		MilMo_WorldMap.Deactivate();
		GC.Collect();
		Resources.UnloadUnusedAssets();
		Activate();
		if (MilMo_Player.Instance.InHub)
		{
			MilMo_Hub.Instance.Activate(delegate
			{
				MilMo_Fade.Instance.FadeOutAll();
			});
		}
		else
		{
			MilMo_Fade.Instance.FadeOutAll();
			if (MilMo_Instance.CurrentInstance != null)
			{
				MilMo_Instance.CurrentInstance.StartPlayMusic();
			}
			if (MilMo_Home.CurrentHome != null)
			{
				MilMo_Home.CurrentHome.UnmuteFurnitureSounds();
			}
			ChangePlayerController(MilMo_PlayerControllerInGUIApp.PreviousControllerType);
			MilMo_PlayerControllerBase.SetRotation(returnRotation);
			MilMo_PlayerControllerBase.SetPosition(returnPosition);
			Camera.HookupCurrentPlayCamera();
		}
		MilMo_EventSystem.At(2f, delegate
		{
			MilMo_Player.Instance.InNavigator = false;
			MilMo_Player.Instance.LeavingNavigator = false;
		});
	}

	private static void InitializeOpenShopListener()
	{
		GameEvent.OpenShopEvent.RegisterAction(OpenShop);
	}

	private static void OpenShop()
	{
		if (MilMo_Player.Instance == null)
		{
			Debug.Log("MilMo_Player is null");
			return;
		}
		if (!MilMo_Player.Instance.OkToEnterShop())
		{
			GameEvent.OpenShopFailEvent.RaiseEvent();
			return;
		}
		if (MilMo_Home.CurrentHome == null)
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Confirm);
		}
		MilMo_Player.Instance.RequestEnterShop();
		MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_OpenShop");
	}

	private void HandleShopResponse(object msgAsObject)
	{
		MilMo_Player.Instance.PendingShopRequest = null;
		if (!(msgAsObject is ServerShopResponse serverShopResponse))
		{
			return;
		}
		MilMo_ServerTime.Instance.SetServerTimeInGMT(serverShopResponse.getCurrentTimestampGMT());
		if (!MilMo_Player.Instance.InShop)
		{
			if (serverShopResponse.getShopResult() == 0)
			{
				if (serverShopResponse.getCurrencyInfo() != null)
				{
					MilMo_Monetization.Instance.SetExchangeRate(serverShopResponse.getCurrencyInfo().GetId(), serverShopResponse.getCurrencyInfo().GetExchangeRate());
				}
				EnterShop();
			}
			else
			{
				Debug.LogWarning("Failed to enter shop");
			}
		}
		else if (serverShopResponse.getShopResult() == 0)
		{
			LeaveShop();
		}
		else
		{
			Debug.LogWarning("Failed to leave shop");
		}
	}

	private void EnterShop()
	{
		MilMo_Player.Instance.EnteringShop = true;
		Analytics.PremiumStoreOpened();
		returnPosition = MilMo_Player.Instance.Avatar.GameObject.transform.position;
		returnRotation = MilMo_Player.Instance.Avatar.GameObject.transform.rotation;
		_backupBackgroundColor = MilMo_Global.Camera.backgroundColor;
		_backupCameraRect = MilMo_Global.Camera.rect;
		ChangePlayerController(MilMo_PlayerControllerBase.ControllerType.InGUIApp);
		MilMo_LoadingScreen.Instance.LoadShopFade();
		MilMo_Music.Instance.FadeIn("Batch01/Music/ItemShop");
		MilMo_EventSystem.At(1.5f, delegate
		{
			Camera.HookupShopCam();
			ChangePlayerController(MilMo_PlayerControllerBase.ControllerType.InGUIApp, null);
			_previousLOD = MilMo_Lod.GlobalLodFactor;
			MilMo_Lod.GlobalLodFactor = 0.01f;
			if (MilMo_Level.CurrentLevel != null && MilMo_Level.CurrentLevel.AmbientSoundManager != null)
			{
				MilMo_Level.CurrentLevel.AmbientSoundManager.Mute();
			}
			if (MilMo_Player.Instance.InNavigator)
			{
				MilMo_WorldMap.Deactivate();
			}
			if (MilMo_Player.Instance.InHub)
			{
				MilMo_Hub.Instance.Deactivate();
			}
			Deactivate();
			if (MilMo_Home.CurrentHome != null)
			{
				MilMo_Home.CurrentHome.MuteFurnitureSounds();
			}
			Camera.enabled = true;
			RenderSettings.fog = false;
			MilMo_Player.Instance.Silhouette.Enable(enable: false);
			MilMo_Player.Instance.Avatar.DisablePuffs();
			MilMo_Player.Instance.Avatar.DisableBlobShadows();
			MilMo_Player.Instance.Avatar.PlayAnimation("Idle");
			_wieldedItemOutsideShop = MilMo_Player.Instance.Avatar.WieldedItem;
			MilMo_Player.Instance.Avatar.Unwield();
			GC.Collect();
			Resources.UnloadUnusedAssets();
			MilMo_LoadingScreen.Instance.ShopActivationWait();
			MilMo_CharacterShop.Activate(delegate
			{
				MilMo_LoadingScreen.Instance.ShopFinalWait();
				MilMo_EventSystem.At(1f, FadeInShop);
				Texture2D returnIcon = null;
				MilMo_LocString returnName = MilMo_LocString.Empty;
				if (MilMo_Instance.CurrentInstance != null)
				{
					returnIcon = MilMo_Instance.CurrentInstance.Icon;
					returnName = MilMo_Instance.CurrentInstance.ShopDisplayName;
				}
				else if (MilMo_Player.Instance.InHub)
				{
					returnName = MilMo_Localization.GetLocString("World_8824");
				}
				if (MilMo_Player.Instance.InHub)
				{
					LoadAndSetReturnIconAsync("Content/GUI/Batch01/Textures/Homes/IconMyHome");
				}
				else
				{
					MilMo_CharacterShop.SetReturnIcon(returnIcon);
				}
				MilMo_CharacterShop.SetReturnName(returnName);
			});
		});
	}

	private static async void LoadAndSetReturnIconAsync(string path)
	{
		MilMo_CharacterShop.SetReturnIcon(await MilMo_ResourceManager.Instance.LoadTextureAsync(path));
	}

	private void LeaveShop()
	{
		MilMo_Lod.GlobalLodFactor = _previousLOD;
		MilMo_Player.Instance.LeavingShop = true;
		Analytics.ScreenVisit("LeaveShop");
		MilMo_Player.Instance.Avatar.BodyPackManager.Restore();
		MilMo_Player.Instance.Avatar.AsyncApply(delegate
		{
			if (!MilMo_Player.Instance.PendingHomeKick && _pendingHomeTravel == null)
			{
				MilMo_Fade.Instance.FadeOutAll();
			}
			else if (!MilMo_Player.Instance.InHub)
			{
				if (MilMo_Player.Instance.PendingHomeKick)
				{
					MilMo_LevelInfoData levelInfo = MilMo_LevelInfo.GetLevelInfoData(MilMo_Level.LastAdventureLevel);
					MilMo_LoadingScreen.Instance.LevelLoadFade(12f);
					MilMo_EventSystem.At(0.25f, delegate
					{
						MilMo_LocString locString = MilMo_Localization.GetLocString("Generic_56");
						locString.SetFormatArgs(levelInfo.DisplayName);
						MilMo_LoadingScreen.Instance.SetLoadingText(locString);
						MilMo_LevelData.LoadAndSetLevelIcon(levelInfo.World, levelInfo.Level);
					});
					MilMo_Player.Instance.PendingHomeKick = false;
					MilMo_Player.ShowKickedFromHomeQuickInfo();
				}
				else if (_pendingHomeTravel != null)
				{
					MilMo_LoadingScreen.Instance.LoadHomeFade(40f, _pendingHomeTravel.AvatarName);
					Singleton<GameNetwork>.Instance.RequestHomeServerInfo(_pendingHomeTravel.PlayerId, _pendingHomeTravel.UseTeleportStone);
					_pendingHomeTravel = null;
				}
			}
			MilMo_CharacterShop.Deactivate();
			if (MilMo_Player.Instance.PendingTeleportToFriendMessage != null)
			{
				string text = MilMo_Player.Instance.PendingTeleportToFriendMessage.getHomeOwnerId().ToString();
				if (text.Length == 0)
				{
					MilMo_LoadingScreen.Instance.LevelLoadFade(12f);
				}
				else
				{
					MilMo_LoadingScreen.Instance.LoadHomeFade(40f, MilMo_ProfileManager.GetStoredName(text));
				}
				MilMo_EventSystem.At(0.8f, delegate
				{
					MilMo_Player.Instance.InShop = false;
					MilMo_Player.Instance.LeavingShop = false;
					MilMo_Player.Instance.InHub = false;
					MilMo_EventSystem.Instance.AsyncPostEvent("teleport_to_friend_ok", MilMo_Player.Instance.PendingTeleportToFriendMessage);
					GC.Collect();
					Resources.UnloadUnusedAssets();
					Activate();
				});
			}
			else
			{
				if (MilMo_Level.CurrentLevel != null && MilMo_Level.CurrentLevel.AmbientSoundManager != null)
				{
					MilMo_Level.CurrentLevel.AmbientSoundManager.Unmute();
				}
				Camera.DefaultCameraSettings();
				MilMo_Global.Camera.backgroundColor = _backupBackgroundColor;
				MilMo_Global.Camera.rect = _backupCameraRect;
				if (MilMo_Instance.CurrentInstance != null)
				{
					MilMo_Instance.CurrentInstance.Environment.RestoreRenderSettings();
				}
				ChangePlayerController(MilMo_PlayerControllerInGUIApp.PreviousControllerType);
				MilMo_PlayerControllerBase.SetRotation(returnRotation);
				MilMo_PlayerControllerBase.SetPosition(returnPosition);
				Camera.HookupCurrentPlayCamera();
				MilMo_Player.Instance.Avatar.PlayAnimation(MilMo_Player.Instance.Avatar.IdleAnimation);
				MilMo_Player.Instance.Avatar.EnableBlobShadows();
				MilMo_Player.Instance.Avatar.EnablePuffs();
				MilMo_Player.Instance.Silhouette.Enable(enable: true);
				if (_wieldedItemOutsideShop != null && MilMo_Player.Instance.Avatar.WieldedItem == null)
				{
					MilMo_Player.Instance.Avatar.Wield(_wieldedItemOutsideShop, applyBodyPack: true);
				}
				GC.Collect();
				Resources.UnloadUnusedAssets();
				if (MilMo_Player.Instance.InNavigator)
				{
					MilMo_WorldMap.Activate(delegate
					{
						((MilMo_NavigatorMenu)MilMo_GlobalUI.Instance.GetItem("NavigatorMenu")).NavigatorButton.SetState(MilMo_Player.Instance.InHub);
						MilMo_Player.Instance.InNavigator = true;
					}, CurrentWorld);
					MilMo_Player.Instance.InShop = false;
					MilMo_Player.Instance.LeavingShop = false;
				}
				else
				{
					Activate();
					if (MilMo_Player.Instance.InHub)
					{
						MilMo_Hub.Instance.Activate(null);
						if (MilMo_Player.Instance.PendingHomeKick || _pendingHomeTravel != null)
						{
							MilMo_Player.Instance.RequestLeaveHub();
						}
					}
					if (!MilMo_Player.Instance.InHub || MilMo_Player.Instance.InNavigator)
					{
						if (MilMo_Instance.CurrentInstance != null)
						{
							MilMo_Instance.CurrentInstance.StartPlayMusic();
						}
						if (MilMo_Home.CurrentHome != null)
						{
							MilMo_Home.CurrentHome.UnmuteFurnitureSounds();
						}
					}
					MilMo_EventSystem.At(2f, delegate
					{
						MilMo_Player.Instance.InShop = false;
						MilMo_Player.Instance.LeavingShop = false;
					});
				}
			}
		}, "LeaveShop");
	}

	private static void FadeInShop()
	{
		if (!MilMo_CharacterShop.PreloadedAssetsDone)
		{
			MilMo_EventSystem.At(0.5f, FadeInShop);
			return;
		}
		MilMo_LoadingScreen.Instance.Hide(enableWorldUI: false);
		MilMo_EventSystem.At(1.5f, delegate
		{
			MilMo_Player.Instance.InShop = true;
			MilMo_Player.Instance.EnteringShop = false;
		});
	}
}
