using System;
using System.Linq;
using System.Threading.Tasks;
using Code.Core.Collision;
using Code.Core.Command;
using Code.Core.EventSystem;
using Code.Core.Input;
using Code.Core.ResourceSystem;
using Code.Core.Visual.Water;
using Code.World;
using Code.World.Chat.ChatRoom;
using Code.World.Gameplay;
using Code.World.Level;
using Code.World.Player;
using UnityEngine;

namespace Code.Core.Camera;

public class MilMo_Camera : MonoBehaviour
{
	private enum PlayCameraType
	{
		Game,
		Social,
		Chat,
		Home
	}

	public static MilMo_Camera Instance;

	public MilMo_PlatformCameraController platformCameraController;

	public MilMo_RpgCameraController rpgCameraController;

	public MilMo_NewRpgCameraController newRpgCameraController;

	public MilMo_ActionCameraController actionCameraController;

	public MilMo_MovieCameraController movieCameraController;

	public MilMo_ChatCameraController chatCameraController;

	public MilMo_SocialCameraController socialCameraController;

	public MilMo_SplineRideCameraController splineRideController;

	public MilMo_HomeCameraController homeCameraController;

	public MilMo_ShopCameraController shopCameraController;

	public MilMo_HubCameraController hubCameraController;

	[SerializeField]
	private PlayCameraType _currentPlayCameraType;

	private MilMo_GenericReaction _startSplineRideReaction;

	private MilMo_GenericReaction _endSplineRideReaction;

	public bool IsStarted { get; private set; }

	public void Startup()
	{
		if (!IsStarted)
		{
			IsStarted = true;
			platformCameraController = new MilMo_PlatformCameraController();
			rpgCameraController = new MilMo_RpgCameraController();
			newRpgCameraController = new MilMo_NewRpgCameraController();
			actionCameraController = new MilMo_ActionCameraController();
			movieCameraController = new MilMo_MovieCameraController();
			chatCameraController = new MilMo_ChatCameraController();
			socialCameraController = new MilMo_SocialCameraController();
			splineRideController = new MilMo_SplineRideCameraController();
			homeCameraController = new MilMo_HomeCameraController();
			shopCameraController = new MilMo_ShopCameraController();
			hubCameraController = new MilMo_HubCameraController();
			HookupTransforms();
			DefaultCameraSettings();
			HookupGameCam();
			_startSplineRideReaction = MilMo_EventSystem.Listen("spline_ride_start", HookupSplineRideCam);
			_startSplineRideReaction.Repeating = true;
			_endSplineRideReaction = MilMo_EventSystem.Listen("spline_ride_end", SplineRideEnd);
			_endSplineRideReaction.Repeating = true;
			Instance = this;
			MilMo_Command.Instance.RegisterCommand("Camera", Debug_CameraEvent);
			MilMo_Command.Instance.RegisterCommand("ActionCam", Debug_ActionCam);
			MilMo_Command.Instance.RegisterCommand("Camera.ExecuteScript", Debug_ExecuteScript);
		}
	}

	public void HookupTransforms()
	{
		MilMo_CameraController.CameraTransform = base.transform;
		MilMo_CameraController.CameraComponent = GetComponent<UnityEngine.Camera>();
		MilMo_CameraController.Player = MilMo_Player.Instance.Avatar.GameObject.transform;
		MilMo_CameraController.PlayerHead = MilMo_Player.Instance.Avatar.Head;
	}

	public void SetupPosition()
	{
		MilMo_PlatformCameraController milMo_PlatformCameraController = platformCameraController;
		if (milMo_PlatformCameraController != null && milMo_PlatformCameraController.HookedUp)
		{
			platformCameraController.SetupPosition();
			return;
		}
		MilMo_RpgCameraController milMo_RpgCameraController = rpgCameraController;
		if (milMo_RpgCameraController != null && milMo_RpgCameraController.HookedUp)
		{
			rpgCameraController.SetupPosition();
			return;
		}
		MilMo_SocialCameraController milMo_SocialCameraController = socialCameraController;
		if (milMo_SocialCameraController != null && milMo_SocialCameraController.HookedUp)
		{
			socialCameraController.SetupPosition();
			return;
		}
		MilMo_HomeCameraController milMo_HomeCameraController = homeCameraController;
		if (milMo_HomeCameraController != null && milMo_HomeCameraController.HookedUp)
		{
			homeCameraController.SetupPosition();
			return;
		}
		MilMo_NewRpgCameraController milMo_NewRpgCameraController = newRpgCameraController;
		if (milMo_NewRpgCameraController != null && milMo_NewRpgCameraController.HookedUp)
		{
			newRpgCameraController.SetupPosition();
		}
	}

	public async Task CameraEvent(string evt)
	{
		if (evt.Equals("HappyPickup", StringComparison.InvariantCultureIgnoreCase))
		{
			HappyPickupInit();
			return;
		}
		if (evt.Equals("Social", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_World.Instance.ChangePlayerController(MilMo_PlayerControllerBase.ControllerType.Social);
			HookupSocialCam();
			return;
		}
		if (evt.Equals("Game", StringComparison.InvariantCultureIgnoreCase))
		{
			MilMo_World.Instance.ChangePlayerController(MilMo_PlayerControllerBase.ControllerType.Game);
			HookupGameCam();
			return;
		}
		if (evt.Contains("Shake"))
		{
			MilMo_GameCameraController milMo_GameCameraController = MilMo_Input.Mode switch
			{
				MilMo_Input.ControlMode.Mmorpg => rpgCameraController, 
				MilMo_Input.ControlMode.Platform => platformCameraController, 
				_ => null, 
			};
			if (milMo_GameCameraController != null)
			{
				if (evt == "Shake3")
				{
					milMo_GameCameraController.BossCameraShake();
				}
				else
				{
					milMo_GameCameraController.ShakeCameraPosition(1.5f, 0.25f);
				}
			}
			return;
		}
		MilMo_SFFile milMo_SFFile = await MilMo_SimpleFormat.RealAsyncLoad("Content/CameraScripts/" + evt);
		if (milMo_SFFile == null)
		{
			Debug.LogWarning("Could not find camera script " + evt);
			return;
		}
		milMo_SFFile.NextRow();
		if (milMo_SFFile.PeekIsNext("Movie") || milMo_SFFile.PeekIsNext("CameraSettings"))
		{
			Instance.ExecuteScript(milMo_SFFile);
		}
	}

	public void ValidatePosition()
	{
		MilMo_PlatformCameraController milMo_PlatformCameraController = platformCameraController;
		if (milMo_PlatformCameraController != null && milMo_PlatformCameraController.HookedUp)
		{
			MilMo_GameCameraController.ValidatePosition();
			return;
		}
		MilMo_RpgCameraController milMo_RpgCameraController = rpgCameraController;
		if (milMo_RpgCameraController != null && milMo_RpgCameraController.HookedUp)
		{
			MilMo_GameCameraController.ValidatePosition();
			return;
		}
		MilMo_NewRpgCameraController milMo_NewRpgCameraController = newRpgCameraController;
		if (milMo_NewRpgCameraController != null && milMo_NewRpgCameraController.HookedUp)
		{
			MilMo_GameCameraController.ValidatePosition();
		}
	}

	public bool InsideWater()
	{
		if (MilMo_Level.CurrentLevel != null)
		{
			return MilMo_WaterManager.WaterVolumes.Any((MilMo_Volume volume) => volume.IsInside(base.transform.position));
		}
		return false;
	}

	private void HookupSplineRideCam(object splineAsObj)
	{
		if (!movieCameraController.HookedUp && splineAsObj is MilMo_PlayerSpline spline)
		{
			UnhookAll();
			if (splineRideController != null)
			{
				splineRideController.Spline = spline;
				splineRideController.HookUp();
			}
		}
	}

	private void HookupActionCam()
	{
		UnhookAll();
		if (actionCameraController != null)
		{
			actionCameraController.HookUp();
		}
	}

	private void HookupMovieCam()
	{
		UnhookAll();
		if (movieCameraController != null)
		{
			movieCameraController.HookUp();
		}
	}

	public void HookupGameCam()
	{
		switch (MilMo_Input.Mode)
		{
		case MilMo_Input.ControlMode.Platform:
			if (platformCameraController != null && !platformCameraController.HookedUp)
			{
				UnhookAll();
				platformCameraController.HookUp();
				_currentPlayCameraType = PlayCameraType.Game;
			}
			break;
		case MilMo_Input.ControlMode.Mmorpg:
			if (rpgCameraController != null && !rpgCameraController.HookedUp)
			{
				UnhookAll();
				rpgCameraController.HookUp();
				_currentPlayCameraType = PlayCameraType.Game;
			}
			break;
		case MilMo_Input.ControlMode.NewMmorpg:
			if (newRpgCameraController != null && !newRpgCameraController.HookedUp)
			{
				UnhookAll();
				newRpgCameraController.HookUp();
				_currentPlayCameraType = PlayCameraType.Game;
			}
			break;
		case MilMo_Input.ControlMode.PlatformFurnishing:
		case MilMo_Input.ControlMode.MmorpgFurnishing:
			break;
		}
	}

	public void HookupChatCam(MilMo_ChatRoom chatRoom)
	{
		MilMo_World.Instance.ChangePlayerController(MilMo_PlayerControllerBase.ControllerType.Chat);
		if (chatCameraController == null)
		{
			return;
		}
		MilMo_SocialCameraController milMo_SocialCameraController = socialCameraController;
		if (milMo_SocialCameraController == null || !milMo_SocialCameraController.HookedUp)
		{
			MilMo_HomeCameraController milMo_HomeCameraController = homeCameraController;
			if (milMo_HomeCameraController == null || !milMo_HomeCameraController.HookedUp)
			{
				UnhookAll();
				chatCameraController.HookUp(chatRoom);
				_currentPlayCameraType = PlayCameraType.Chat;
			}
		}
	}

	private void HookupChatCam()
	{
		if (chatCameraController != null && !chatCameraController.HookedUp)
		{
			UnhookAll();
			chatCameraController.HookUp();
			_currentPlayCameraType = PlayCameraType.Chat;
		}
	}

	private void HookupSocialCam()
	{
		if (socialCameraController != null && !socialCameraController.HookedUp)
		{
			UnhookAll();
			socialCameraController.HookUp();
			_currentPlayCameraType = PlayCameraType.Social;
		}
	}

	public void HookupHomeCam()
	{
		if (homeCameraController != null && !homeCameraController.HookedUp)
		{
			UnhookAll();
			homeCameraController.HookUp();
			_currentPlayCameraType = PlayCameraType.Home;
		}
	}

	public void HookupShopCam()
	{
		if (shopCameraController != null && !shopCameraController.HookedUp)
		{
			UnhookAll();
			shopCameraController.HookUp();
		}
	}

	public void HookupHubCam()
	{
		if (hubCameraController != null && !hubCameraController.HookedUp)
		{
			UnhookAll();
			hubCameraController.HookUp();
		}
	}

	public void HappyPickupInit()
	{
		if (actionCameraController != null)
		{
			if (!actionCameraController.HookedUp)
			{
				HookupActionCam();
			}
			actionCameraController.HappyPickup();
		}
	}

	public void Shake(string evt)
	{
		if (actionCameraController != null)
		{
			if (!actionCameraController.HookedUp)
			{
				HookupActionCam();
			}
			int.TryParse(evt.Substring(5), out var result);
			actionCameraController.Shake(result);
			MilMo_EventSystem.At(0.5f * (float)result, delegate
			{
				actionCameraController.Shake(shouldShake: false);
				HookupCurrentPlayCamera();
			});
		}
	}

	public void HappyPickupEnd()
	{
		HookupCurrentPlayCamera();
	}

	public void LeaveChatRoom()
	{
		if (chatCameraController != null && !chatCameraController.HookedUp)
		{
			if (socialCameraController != null && socialCameraController.HookedUp)
			{
				MilMo_World.Instance.ChangePlayerController(MilMo_PlayerControllerBase.ControllerType.Social);
			}
			else if (homeCameraController != null && homeCameraController.HookedUp)
			{
				MilMo_World.Instance.ChangePlayerController(MilMo_PlayerControllerBase.ControllerType.Home);
			}
		}
		else
		{
			MilMo_World.Instance.ChangePlayerController(MilMo_PlayerControllerBase.ControllerType.Game);
			HookupGameCam();
		}
	}

	public void HookupCurrentPlayCamera()
	{
		switch (_currentPlayCameraType)
		{
		case PlayCameraType.Game:
			HookupGameCam();
			break;
		case PlayCameraType.Social:
			HookupSocialCam();
			break;
		case PlayCameraType.Home:
			HookupHomeCam();
			break;
		case PlayCameraType.Chat:
			HookupChatCam();
			break;
		}
	}

	public void DefaultCameraSettings()
	{
		MilMo_CameraController.CameraComponent.farClipPlane = 2800f;
		MilMo_CameraController.CameraComponent.nearClipPlane = 0.1f;
		MilMo_CameraController.CameraComponent.fieldOfView = 30f;
		MilMo_CameraController.CameraComponent.depth = 0f;
	}

	public void ExecuteScript(MilMo_SFFile file)
	{
		if (movieCameraController != null)
		{
			if (!movieCameraController.HookedUp)
			{
				HookupMovieCam();
			}
			movieCameraController.ExecuteScript(file);
		}
	}

	public static bool Orbit()
	{
		return MilMo_CameraController.Orbit;
	}

	public void Reset()
	{
		MilMo_PlatformCameraController milMo_PlatformCameraController = platformCameraController;
		if (milMo_PlatformCameraController != null && milMo_PlatformCameraController.HookedUp)
		{
			platformCameraController.Reset();
			return;
		}
		MilMo_RpgCameraController milMo_RpgCameraController = rpgCameraController;
		if (milMo_RpgCameraController != null && milMo_RpgCameraController.HookedUp)
		{
			rpgCameraController.Reset();
			return;
		}
		MilMo_NewRpgCameraController milMo_NewRpgCameraController = newRpgCameraController;
		if (milMo_NewRpgCameraController != null && milMo_NewRpgCameraController.HookedUp)
		{
			newRpgCameraController.Reset();
		}
	}

	private void LateUpdate()
	{
		try
		{
			if (MilMo_Input.Mode == MilMo_Input.ControlMode.Mmorpg && platformCameraController != null && (platformCameraController.HookedUp || newRpgCameraController.HookedUp))
			{
				HookupGameCam();
			}
			if (MilMo_Input.Mode == MilMo_Input.ControlMode.NewMmorpg && newRpgCameraController != null && (rpgCameraController.HookedUp || platformCameraController.HookedUp))
			{
				HookupGameCam();
			}
			if (MilMo_Input.Mode == MilMo_Input.ControlMode.Platform && rpgCameraController != null && (rpgCameraController.HookedUp || newRpgCameraController.HookedUp))
			{
				HookupGameCam();
			}
			if (platformCameraController != null)
			{
				platformCameraController.Update();
			}
			if (rpgCameraController != null)
			{
				rpgCameraController.Update();
			}
			if (newRpgCameraController != null)
			{
				newRpgCameraController.Update();
			}
			if (movieCameraController != null)
			{
				movieCameraController.Update();
			}
			if (chatCameraController != null)
			{
				chatCameraController.Update();
			}
			if (socialCameraController != null)
			{
				socialCameraController.Update();
			}
			if (homeCameraController != null)
			{
				homeCameraController.Update();
			}
			if (splineRideController != null)
			{
				splineRideController.Update();
			}
			if (shopCameraController != null)
			{
				shopCameraController.Update();
			}
			if (hubCameraController != null)
			{
				hubCameraController.Update();
			}
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
			if (actionCameraController != null)
			{
				actionCameraController.FixedUpdate();
			}
			if (movieCameraController != null)
			{
				movieCameraController.FixedUpdate();
			}
			if (chatCameraController != null)
			{
				chatCameraController.FixedUpdate();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}

	public void UnhookAll()
	{
		if (actionCameraController != null && actionCameraController.HookedUp)
		{
			actionCameraController.Unhook();
		}
		if (movieCameraController != null && movieCameraController.HookedUp)
		{
			movieCameraController.Unhook();
		}
		if (platformCameraController != null && platformCameraController.HookedUp)
		{
			platformCameraController.Unhook();
		}
		if (newRpgCameraController != null && newRpgCameraController.HookedUp)
		{
			newRpgCameraController.Unhook();
		}
		if (rpgCameraController != null && rpgCameraController.HookedUp)
		{
			rpgCameraController.Unhook();
		}
		if (chatCameraController != null && chatCameraController.HookedUp)
		{
			chatCameraController.Unhook();
		}
		if (socialCameraController != null && socialCameraController.HookedUp)
		{
			socialCameraController.Unhook();
		}
		if (splineRideController != null && splineRideController.HookedUp)
		{
			splineRideController.Unhook();
		}
		if (homeCameraController != null && homeCameraController.HookedUp)
		{
			homeCameraController.Unhook();
		}
		if (shopCameraController != null && shopCameraController.HookedUp)
		{
			shopCameraController.Unhook();
		}
		if (hubCameraController != null && hubCameraController.HookedUp)
		{
			hubCameraController.Unhook();
		}
	}

	private void SplineRideEnd(object o)
	{
		if (splineRideController.HookedUp)
		{
			HookupCurrentPlayCamera();
		}
	}

	private static string Debug_ExecuteScript(string[] args)
	{
		if (args.Length < 2)
		{
			return "usage: Camera.ExecuteScript <scriptFilePath>";
		}
		string path = "Content/CameraScripts/" + args[1];
		MilMo_SimpleFormat.AsyncLoad(path, delegate(MilMo_SFFile file)
		{
			if (file == null)
			{
				Debug.LogWarning("File " + path + " not found.");
			}
			else
			{
				file.NextRow();
				if (file.PeekIsNext("Movie") || file.PeekIsNext("CameraSettings"))
				{
					Debug.LogWarning("Executing camera script " + args[1]);
					Instance.ExecuteScript(file);
				}
				else
				{
					Debug.LogWarning("Unknown camera script type '" + file.GetString());
				}
			}
		});
		return "Async loading " + path + "...";
	}

	private static string Debug_ActionCam(string[] args)
	{
		if (Instance.movieCameraController.HookedUp)
		{
			Instance.HookupCurrentPlayCamera();
			return "Action cam deactivated.";
		}
		Instance.HookupMovieCam();
		return "Action cam activated. 'ActionCam' again to deactivate.";
	}

	private static string Debug_CameraEvent(string[] args)
	{
		if (args.Length < 2)
		{
			return "usage: Camera {HappyPickup|Social|Game}";
		}
		MilMo_World.Instance.Camera.CameraEvent(args[1]);
		return "";
	}
}
