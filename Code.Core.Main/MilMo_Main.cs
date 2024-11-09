using System;
using System.Threading.Tasks;
using Code.Apps.LoginScreen;
using Code.Core.Avatar;
using Code.Core.Avatar.Ragdoll;
using Code.Core.BodyPack;
using Code.Core.BuddyBackend;
using Code.Core.Command;
using Code.Core.Config;
using Code.Core.Emote;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.GUI.Core;
using Code.Core.Input;
using Code.Core.Network;
using Code.Core.Portal;
using Code.Core.ResourceSystem;
using Code.Core.Spline;
using Code.Core.Template;
using Code.Core.Utility;
using Code.Core.Visual;
using Code.Core.Visual.Effect;
using Code.World.Feeds;
using Code.World.GUI.Converters;
using Code.World.ImageEffects;
using Code.World.Tutorial;
using Code.World.Voting;
using Core;
using Core.Analytics;
using Core.Input;
using Core.Settings;
using UI.LoadingScreen;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Core.Main;

public class MilMo_Main : MonoBehaviour
{
	[SerializeField]
	private LoadingScreen loadingScreen;

	public async void Awake()
	{
		if (!MilMo_Config.Initialize())
		{
			return;
		}
		string text = "Release";
		Debug.Log(".\n _____ _ _ _____     \n|     |_| |     |___ \n| | | | | | | | | . |\n|_|_|_|_|_|_|_|_|___|\n MilMo Version " + Application.version + " (" + text + ")\n");
		Settings.ApplyResolution();
		Debug.Log("[System information]\nUnity Version:" + Application.unityVersion + "\nPlatform: " + MilMo_Utility.PlatformAsString() + "\nOperating System: " + SystemInfo.operatingSystem + "\nSystem Language: " + Application.systemLanguage.ToString() + "\nProcessor Type: " + SystemInfo.processorType + "\nProcessor Cores: " + SystemInfo.processorCount + "\nSystem Memory Size: " + SystemInfo.systemMemorySize + "\nGraphics Device Name: " + SystemInfo.graphicsDeviceName + "\nGraphics Device Vendor: " + SystemInfo.graphicsDeviceVendor + "\nGraphics Device Version: " + SystemInfo.graphicsDeviceVersion + "\nGraphics Device Memory: " + SystemInfo.graphicsMemorySize + "\nGraphics Shader Level: " + SystemInfo.graphicsShaderLevel + "\nGraphics Supports Shadows: " + SystemInfo.supportsShadows + "\nResolution: " + Screen.currentResolution.width + " x " + Screen.currentResolution.height + "\n");
		MilMo_Portal.Initialize();
		GameObject o = (MilMo_Global.MainGameObject = base.gameObject);
		o.AddComponent<TimeTracker>();
		MilMo_Global.Camera = o.AddComponent<UnityEngine.Camera>();
		MilMo_Global.Camera.tag = "MainCamera";
		MilMo_Global.Camera.backgroundColor = Color.white;
		MilMo_Global.Camera.allowMSAA = false;
		UpdateLoadingProgress(0.01f, "Initializing");
		MilMo_AvatarGlobalLODSettings.LoadGlobalLODSettings(Settings.QualityTier);
		Addressables.InstantiateAsync("PlayerPrefab").WaitForCompletion();
		o.AddComponent<FlareLayer>();
		MilMo_Global.ImageEffectsHandler = o.AddComponent<MilMo_ImageEffectsHandler>();
		GameObject obj = new GameObject("Audio");
		obj.transform.parent = o.transform;
		obj.transform.localPosition = Vector3.zero;
		MilMo_Global.AudioListener = obj;
		MilMo_Global.AudioListener.AddComponent<AudioListener>();
		if (MilMo_Global.ParticleLight == null)
		{
			MilMo_Global.ParticleLight = new GameObject("ParticleLight");
		}
		if (MilMo_Global.ParticleLight.GetComponent<Light>() == null)
		{
			MilMo_Global.ParticleLight.AddComponent<Light>();
		}
		MilMo_Global.ParticleLight.GetComponent<Light>().type = LightType.Point;
		MilMo_Global.ParticleLight.GetComponent<Light>().range = 20f;
		MilMo_ResourceManager instance = MilMo_ResourceManager.Instance;
		bool flag = !instance;
		if (!flag)
		{
			flag = !(await instance.ValidateContentServerAsync());
		}
		if (flag)
		{
			Debug.LogWarning("Need ResourceManager!");
			UpdateLoadingProgress(-1f, "Failed to initialize resource manager! Closing in 3 seconds");
			await Task.Delay(1000);
			UpdateLoadingProgress(-1f, "Failed to initialize resource manager! Closing in 2 seconds");
			await Task.Delay(1000);
			UpdateLoadingProgress(-1f, "Failed to initialize resource manager! Closing in 1 second");
			await Task.Delay(1000);
			Application.Quit();
			return;
		}
		TimeSpan wait = TimeSpan.FromSeconds(0.009999999776482582);
		UpdateLoadingProgress(0.1f, "Loading Interface");
		MilMo_UserInterfaceManager.Initialize(o);
		await Task.Delay(wait);
		UpdateLoadingProgress(0.1f, "Loading GUI");
		MilMo_GUISkins.AsyncLoadGUISkins();
		await Task.Delay(wait);
		UpdateLoadingProgress(0.2f, "Loading Translations");
		await MilMo_Localization.AsyncInitializeSystemLanguage();
		await Task.Delay(wait);
		UpdateLoadingProgress(0.4f, "Loading avatar data");
		MilMo_BodyPackSystem.CreateGeneric();
		await Task.Delay(wait);
		UpdateLoadingProgress(0.6f, "Loading avatar data");
		MilMo_BodyPackSystem.CreateForBoy();
		await Task.Delay(wait);
		UpdateLoadingProgress(0.8f, "Loading avatar data");
		MilMo_BodyPackSystem.CreateForGirl();
		await Task.Delay(wait);
		UpdateLoadingProgress(1f, "Ready");
		if (Singleton<MilMo_TemplateContainer>.Instance != null)
		{
			Singleton<MilMo_TemplateContainer>.Instance.Init();
		}
		MilMo_FeedExclamations.Load();
		MilMo_EmoteSystem.Create();
		RagdollManager.Initialize();
		MilMo_ConverterCategoryInfo.ReadData();
		if (Singleton<GameNetwork>.Instance != null)
		{
			Singleton<GameNetwork>.Instance.Create();
		}
		MilMo_VisualRepContainer.Initialize();
		CleanupComponents();
		base.gameObject.AddComponent<InputSwitch>();
		base.gameObject.AddComponent<MilMo_Pointer>();
		base.gameObject.AddComponent<MilMo_Input>();
		InputController.Get().SetOldKeyboardFocusController();
		if (Application.isEditor)
		{
			base.gameObject.AddComponent<MilMo_WebLoginScreen>();
		}
		else
		{
			base.gameObject.AddComponent<MilMo_SteamLogin>();
		}
		MilMo_Command.Instance.RegisterCommand("FPS", Debug_ToggleFPS, isCheat: false);
		MilMo_Command.Instance.RegisterCommand("GFXStats", Debug_GFXStats);
		MilMo_Command.Instance.RegisterCommand("MemStats", Debug_MemStats);
		MilMo_Command.Instance.RegisterCommand("ObjectStats", Debug_ObjectsStats);
		MilMo_Command.Instance.RegisterCommand("GUI", MilMo_UserInterface.Debug_GUI, isCheat: false);
		MilMo_Command.Instance.RegisterCommand("Leaks", Debug_Leaks);
		MilMo_Command.Instance.RegisterCommand("VisualReps", MilMo_VisualRepContainer.Debug_Count);
		MilMo_Command.Instance.RegisterCommand("GlobalLOD", MilMo_Lod.Debug_GlobalLod, isCheat: false);
		GameNetwork.RegisterCommands();
		MilMo_ParticleDamageEffect.RegisterCommands();
		MilMo_Spline.RegisterCommands();
		MilMo_ParticleDamageEffect.RegisterCommands();
		MilMo_GameDialogCreator.CreateListeners();
		Singleton<MilMo_TutorialManager>.Instance.Initialize();
		MilMo_BadWordFilter.AsyncInit();
		MilMo_VoteManager.Initialize();
	}

	private void UpdateLoadingProgress(float value, string text)
	{
		loadingScreen.UpdateProgress(value);
		loadingScreen.UpdateText(text);
	}

	private async Task DisconnectAndWait()
	{
		MilMo_BuddyBackend buddyInstance = Singleton<MilMo_BuddyBackend>.Instance;
		GameNetwork networkInstance = Singleton<GameNetwork>.Instance;
		if (networkInstance != null)
		{
			networkInstance.DisconnectFromGameServer();
		}
		if (buddyInstance != null)
		{
			buddyInstance.Disconnect();
		}
		bool connectedToNexus = true;
		bool connectedToGameserver = true;
		for (int i = 1; i < 5; i++)
		{
			connectedToNexus = buddyInstance != null && buddyInstance.IsConnected;
			connectedToGameserver = networkInstance != null && networkInstance.IsConnectedToGameServer;
			if (connectedToNexus)
			{
				buddyInstance.Update();
			}
			if (!connectedToGameserver && !connectedToNexus)
			{
				return;
			}
			await Task.Delay(1000);
		}
		Debug.LogWarning("Disconnect timed out. " + $"Connected to game server: {connectedToGameserver}, " + $"connected to Nexus: {connectedToNexus}");
	}

	private async void OnApplicationQuit()
	{
		Screen.SetResolution(800, 600, fullscreen: false);
		await Task.Delay(500);
		await DisconnectAndWait();
		MilMo_EventSystem.Enabled = false;
		Debug.Log("Thank you for playing");
	}

	private void CleanupComponents()
	{
		MilMo_Global.Destroy(GetComponent<MilMo_Pointer>());
		MilMo_Global.Destroy(GetComponent<AudioListener>());
	}

	private static string Debug_Leaks(string[] args)
	{
		if (args.Length < 2)
		{
			return "usage: Leaks {0|1}";
		}
		bool flag;
		try
		{
			flag = Convert.ToInt32(args[1]) != 0;
		}
		catch (FormatException ex)
		{
			return ex?.ToString() + " usage: Leaks {0|1}";
		}
		MilMo_DetectLeaks component = MilMo_Global.MainGameObject.GetComponent<MilMo_DetectLeaks>();
		if (flag)
		{
			if (component != null)
			{
				return "Leaks already enabled. Use 'Leaks 0' to disable.";
			}
			MilMo_Global.MainGameObject.AddComponent<MilMo_DetectLeaks>();
			return "Leaks was enabled";
		}
		if (component == null)
		{
			return "Leaks not enabled. Use 'Leaks 1' to enable.";
		}
		UnityEngine.Object.Destroy(component);
		return "Leaks was disabled";
	}

	private static string Debug_ToggleFPS(string[] args)
	{
		if (Singleton<Analytics>.Instance == null)
		{
			return "Service unavailable";
		}
		if (!Singleton<Analytics>.Instance.ToggleVisualization())
		{
			return "fps disabled";
		}
		return "fps enabled";
	}

	private static string Debug_GFXStats(string[] args)
	{
		return SystemInfo.graphicsDeviceName + " mem=" + SystemInfo.graphicsMemorySize + " shader=" + SystemInfo.graphicsShaderLevel;
	}

	private static string Debug_MemStats(string[] args)
	{
		long num = GC.GetTotalMemory(forceFullCollection: true) / 1048576;
		return SystemInfo.systemMemorySize + "MB total memory " + num + "MB allocated by c#";
	}

	private static string Debug_ObjectsStats(string[] args)
	{
		Debug.Log("-----------------------------------------");
		Debug.Log("textures=" + Resources.FindObjectsOfTypeAll(typeof(Texture)).Length);
		Debug.Log("meshes=" + Resources.FindObjectsOfTypeAll(typeof(Mesh)).Length);
		Debug.Log("gameobjects=" + Resources.FindObjectsOfTypeAll(typeof(GameObject)).Length);
		Debug.Log("materials=" + Resources.FindObjectsOfTypeAll(typeof(Material)).Length);
		Debug.Log("audioClips=" + Resources.FindObjectsOfTypeAll(typeof(AudioClip)).Length);
		Debug.Log("components=" + Resources.FindObjectsOfTypeAll(typeof(Component)).Length);
		Debug.Log("all=" + Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object)).Length);
		return "Printed stats to log";
	}
}
