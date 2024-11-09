using System.Threading.Tasks;
using Code.Apps.Fade;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.Network;
using Code.Core.Network.messages;
using Code.Core.Network.messages.server;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.World;
using Code.World.GUI;
using Code.World.Player;
using Code.World.Slides;
using Core;
using Core.Analytics;
using UnityEngine;

namespace Code.Apps.LoginScreen;

public class MilMo_Login : MonoBehaviour
{
	public MilMo_UserInterface ui;

	public MilMo_Widget errorLabel;

	public MilMo_LoadingStartup loadingWidget;

	public void Awake()
	{
		ui = MilMo_UserInterfaceManager.CreateUserInterface("LoginScreen");
		ui.OffsetMode = false;
		ui.UpdateGlobalOffset();
		ui.ResetLayout();
		errorLabel = new MilMo_Widget(ui);
		errorLabel.SetFont(MilMo_GUI.Font.ArialRounded);
		errorLabel.SetFontScale(1f);
		errorLabel.SetFadeSpeed(0.02f);
		errorLabel.SetPosition((float)Screen.width / 2f, (float)Screen.height / 2f);
		errorLabel.SetScale(520f, 100f);
		errorLabel.SetScalePull(0.06f, 0.06f);
		errorLabel.SetScaleDrag(0.5f, 0.5f);
		errorLabel.SetPosPull(0.06f, 0.06f);
		errorLabel.SetPosDrag(0.5f, 0.5f);
		errorLabel.SetAlignment(MilMo_GUI.Align.CenterCenter);
		errorLabel.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		errorLabel.AllowPointerFocus = false;
		errorLabel.SetDefaultTextColor(Color.black);
		errorLabel.SetTextColor(Color.black);
		errorLabel.SetEnabled(e: false);
		ui.AddChild(errorLabel);
		loadingWidget = new MilMo_LoadingStartup(ui);
		loadingWidget.SetEnabled(e: false);
		ui.AddChild(loadingWidget);
		ui.ResetLayout();
		MilMo_EventSystem.Listen("login_token_received", DoTokenLogin);
		MilMo_EventSystem.Listen("login_failed", HandleLoginFail);
		MilMo_EventSystem.Listen("connect_info_game", HandleConnectAttemptDone);
		MilMo_EventSystem.Listen("initial_settings_received", HandleInitialSettings);
	}

	protected static void SetUserToken(string arg)
	{
		string[] array = arg.Split(new char[1] { '#' }, 4);
		if (array.Length < 4)
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
			MilMo_Global.RemoteGameHost = "";
			MilMo_Global.AuthorizationToken = "";
			Debug.LogWarning("Token is in wrong format");
			return;
		}
		Debug.Log("User info received! \nhost:  " + array[0] + "\nworld: " + array[1] + "\nlevel: " + array[2] + "\ntoken: " + array[3]);
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Confirm);
		MilMo_Global.RemoteGameHost = array[0];
		MilMo_Global.AuthorizationToken = array[3];
		MilMo_EventSystem.Instance.PostEvent("login_token_received", null);
	}

	protected void DoTokenLogin(object o)
	{
		Debug.Log("Login: Sending token to: " + MilMo_Global.RemoteGameHost);
		loadingWidget.SetEnabled(e: true);
		MilMo_World.StartLevelChangeListeners();
		MilMo_Player.LoginGame(MilMo_Global.RemoteGameHost, MilMo_Global.AuthorizationToken, LoginDone);
	}

	private void LoginDone(MilMo_LoginInfo loginInfo)
	{
		loadingWidget.SetEnabled(e: false);
		if (loginInfo.Success)
		{
			Debug.Log("Login OK\n");
			RequestInitialSettings();
		}
		else
		{
			HandleLoginFail(loginInfo);
		}
	}

	private static void RequestInitialSettings()
	{
		Singleton<GameNetwork>.Instance.RequestInitialSettings();
	}

	private void HandleInitialSettings(object msgAsObject)
	{
		if (!(msgAsObject is ServerInitialSettings serverInitialSettings))
		{
			Debug.LogWarning("Could not read the initial settings from the server.");
			return;
		}
		MilMo_Global.EventTags = serverInitialSettings.Events.ToArray();
		string[] eventTags = MilMo_Global.EventTags;
		foreach (string text in eventTags)
		{
			Debug.Log("EventTag: " + text);
		}
		RequestPlayerInfo();
	}

	private void RequestPlayerInfo()
	{
		Debug.Log("Requesting player info");
		MilMo_Player.Instance.RequestLocalPlayerInfo(delegate(bool infoSuccess, ServerLocalPlayerInfo playerInfo)
		{
			if (!infoSuccess)
			{
				SetFailState(MilMo_Localization.GetLocString("LoginScreen_458"));
			}
			else
			{
				MilMo_Player.DisconnectListener = MilMo_EventSystem.Listen("disconnected_game", delegate(object wasConnectedAsObject)
				{
					if ((bool)wasConnectedAsObject)
					{
						MilMo_Dialog disconnectDialogue = new MilMo_Dialog(MilMo_Fade.Instance.UserInterface);
						MilMo_Fade.Instance.UserInterface.AddChild(disconnectDialogue);
						disconnectDialogue.DoOK("Batch01/Textures/Dialog/Warning", MilMo_Localization.GetLocString("Generic_Disconnected"), MilMo_Localization.GetLocString("Generic_453"), delegate
						{
							disconnectDialogue.CloseAndRemove(null);
							Application.Quit();
						});
						disconnectDialogue.IgnoreGlobalFade = true;
						disconnectDialogue.BringToFront();
						MilMo_Fade.Instance.FadeInAll();
					}
				});
				MilMo_Player.DisconnectListener.Repeating = true;
				if (!playerInfo.HasAvatar())
				{
					MilMoAnalyticsHandler.FirstTimePlayer();
					Debug.Log("Successfully got player information with no avatar");
					Cleanup();
					MilMo_SlideManager.Instance.StartSlideShow("Intro", dontShowIfAlreadyShown: true, LaunchAvatarEditor);
				}
				else
				{
					MilMo_Global.Camera.backgroundColor = Color.black;
					Debug.Log("Successfully got player information with avatar");
					Cleanup();
				}
			}
		});
	}

	private async void LaunchAvatarEditor()
	{
		await Task.Delay(1);
		SceneManager sceneManager = Object.FindObjectOfType<SceneManager>();
		if (sceneManager == null)
		{
			Debug.LogError("Failed to find any SceneManager.");
		}
		else
		{
			sceneManager.LaunchAvatarEditor();
		}
	}

	private void HandleLoginFail(object o)
	{
		MilMo_LoginInfo milMo_LoginInfo = (MilMo_LoginInfo)o;
		Debug.Log("Login failed");
		Debug.LogWarning("Login failed: " + milMo_LoginInfo.LoginResponse);
		loadingWidget.SetEnabled(e: false);
		switch (milMo_LoginInfo.LoginResponse)
		{
		case LoginResponse.Disconnected:
			SetFailState(MilMo_Localization.GetLocString("LoginScreen_459"));
			break;
		case LoginResponse.BadUserOrPassword:
			SetFailState(MilMo_Localization.GetLocString("LoginScreen_450"));
			break;
		case LoginResponse.UserAlreadyLoggedOn:
			SetFailState(MilMo_Localization.GetLocString("LoginScreen_444"));
			break;
		case LoginResponse.ServerCantBeTrusted:
			SetFailState(MilMo_Localization.GetLocString("LoginScreen_445"));
			break;
		case LoginResponse.AlreadyLoggedIn:
			SetFailState(MilMo_Localization.GetLocString("LoginScreen_446"));
			break;
		case LoginResponse.AlreadyLoggingIn:
			SetFailState(MilMo_Localization.GetLocString("LoginScreen_451"));
			break;
		case LoginResponse.TimeOut:
			SetFailState(MilMo_Localization.GetLocString("LoginScreen_447"));
			break;
		case LoginResponse.NotConnected:
			SetFailState(MilMo_Localization.GetLocString("LoginScreen_448"));
			break;
		case LoginResponse.UnknownError:
			SetFailState(MilMo_Localization.GetLocString("LoginScreen_449"));
			break;
		case LoginResponse.Banned:
			SetBannedState(milMo_LoginInfo.BanReason, milMo_LoginInfo.BanExpiration);
			break;
		default:
			SetFailState(MilMo_Localization.GetLocString("LoginScreen_450"));
			break;
		case LoginResponse.Success:
		case LoginResponse.LoggingOut:
			break;
		}
	}

	private void HandleConnectAttemptDone(object o)
	{
		if ((ConnectResponse)o != 0)
		{
			SetFailState(MilMo_Localization.GetLocString("LoginScreen_448"));
		}
	}

	private void SetBannedState(string reason, string expires)
	{
		if (errorLabel != null)
		{
			MilMo_LocString locString = MilMo_Localization.GetLocString("LoginScreen_461");
			locString.SetFormatArgs(reason, expires);
			errorLabel.SetText(locString);
			errorLabel.SetFontScale(2f);
			errorLabel.SetEnabled(e: true);
		}
	}

	private void SetFailState(MilMo_LocString reason)
	{
		if (errorLabel != null)
		{
			MilMo_LocString locString = MilMo_Localization.GetLocString("LoginScreen_460");
			locString.SetFormatArgs(reason);
			errorLabel.SetText(locString);
			errorLabel.SetEnabled(e: true);
		}
	}

	public void Update()
	{
		if (ui != null && ui.ScreenSizeDirty)
		{
			RefreshUI();
		}
	}

	protected void RefreshUI()
	{
		ui.OffsetMode = false;
		ui.SetGlobalOffset(new Vector2(0f, 0f));
		ui.ResetLayout(10f, 10f);
		loadingWidget.SetPosition((float)Screen.width / 2f, (float)Screen.height / 2f);
		errorLabel.SetPosition((float)Screen.width / 2f, (float)Screen.height / 2f);
		ui.ScreenSizeDirty = false;
	}

	private void Cleanup()
	{
		MilMo_UserInterfaceManager.DestroyUserInterface(ui);
		Object.Destroy(this);
	}
}
