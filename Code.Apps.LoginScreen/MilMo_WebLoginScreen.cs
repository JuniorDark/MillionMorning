using System.Collections;
using Code.Core.Config;
using Code.Core.Global;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.Button;
using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.Core.Utility;
using Code.World.GUI;
using Core.Settings;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.Apps.LoginScreen;

public sealed class MilMo_WebLoginScreen : MilMo_Login
{
	private enum LoginState
	{
		WaitForUser,
		DoLogin,
		RedoLogin
	}

	private const string WEB_LOGIN_USER_NOT_AUTHORIZED_YET = "-1";

	private const string WEB_LOGIN_USER_AUTH_INVALID = "-2";

	private const string WEB_LOGIN_USER_AUTH_ERROR = "-3";

	private const string WEB_LOGIN_INSTANCE_ERROR = "-4";

	private const string WEB_LOGIN_NO_INSTANCE = "-5";

	private const string WEB_LOGIN_MYSQL_CONNECT_ERROR = "-6";

	private const string WEB_LOGIN_USER_INFO_NO_USER = "-7";

	private const string WEB_LOGIN_USER_AUTH_NOT_FOUND = "-8";

	private const string WEB_LOGIN_SERVER_5_XX = "5";

	private const string WEB_LOGIN_SERVER_4_XX = "4";

	private string _lastAuthError = "-1";

	private string _lastAuthErrorText;

	private MilMo_Dialog _loginDialog;

	public MilMo_SimpleLabel emailLabel;

	public MilMo_SimpleLabel passwordLabel;

	public MilMo_SimpleTextField emailTextField;

	public MilMo_SimplePasswordField passwordTextField;

	public MilMo_Button goButton;

	public MilMo_Button backButton;

	public MilMo_Button lostPwButton;

	public MilMo_CheckBox rememberPwCheckbox;

	public MilMo_SimpleLabel lastErrorLabel;

	private MilMo_Dialog _resetDialog;

	public MilMo_SimpleLabel resetLabel;

	public MilMo_TextBlock resetTextBlock;

	public MilMo_SimpleTextField resetTextField;

	public MilMo_Button cancelButton;

	public MilMo_Button submitButton;

	private LoginState _loginState;

	private readonly MilMo_AudioClip _tickSound = new MilMo_AudioClip("Content/Sounds/Batch01/GUI/Generic/Tick");

	public new void Awake()
	{
		string text = Settings.Server.ToLower();
		if (!(text == "en") && !(text == "br"))
		{
			Debug.LogWarning("Missing configuration for WebLogin on this server (" + text + "), using default 'en'");
			text = "en";
		}
		MilMo_Global.WebLoginURL = MilMo_Config.Instance.GetValue("WebLogin.LoginURL." + text);
		MilMo_Global.WebLoginPasswordResetURL = MilMo_Config.Instance.GetValue("WebLogin.PasswordResetURL." + text);
		Debug.Log("Email login screen\nLogin URL: " + MilMo_Global.WebLoginURL + "\nReset URL: " + MilMo_Global.WebLoginPasswordResetURL + "\n");
		base.Awake();
		ui.ResetLayout();
		Vector2 vector = new Vector2(-45f, 50f);
		_loginDialog = new MilMo_Dialog(ui)
		{
			Draggable = false
		};
		_loginDialog.SetText(MilMo_Localization.GetLocString("LoginScreen_12"));
		_loginDialog.SetFadeSpeed(0.025f);
		_loginDialog.SetScale(370f, 153f);
		_loginDialog.SetDefaultColor(1f, 1f, 1f, 1f);
		ui.AddChild(_loginDialog);
		ui.ResetLayout(_loginDialog);
		emailLabel = new MilMo_SimpleLabel(ui)
		{
			Identifier = "NameLabel"
		};
		emailLabel.SetText(MilMo_Localization.GetLocString("LoginScreen_13"));
		emailLabel.SetPosition(ui.Center.x + vector.x - 5f, ui.Align.Top + vector.y);
		emailLabel.SetScale(120f, 22f);
		emailLabel.SetAlignment(MilMo_GUI.Align.CenterRight);
		emailLabel.SetTextAlignment(MilMo_GUI.Align.CenterRight);
		emailLabel.SetFont(MilMo_GUI.Font.ArialRounded);
		_loginDialog.AddChild(emailLabel);
		emailTextField = new MilMo_SimpleTextField(ui)
		{
			Identifier = "Login Name textfield",
			InputText = Settings.WebLoginStoredEmail
		};
		emailTextField.SetPosition(ui.Center.x + vector.x + 5f, ui.Align.Top + vector.y);
		emailTextField.SetScale(152f, 22f);
		emailTextField.SetPosPull(0.06f, 0.06f);
		emailTextField.SetAlignment(MilMo_GUI.Align.CenterLeft);
		_loginDialog.AddChild(emailTextField);
		passwordLabel = new MilMo_SimpleLabel(ui);
		passwordLabel.SetText(MilMo_Localization.GetLocString("LoginScreen_14"));
		passwordLabel.SetPosition(ui.Center.x + vector.x - 5f, ui.Align.Top + vector.y + 25f);
		passwordLabel.SetScale(120f, 22f);
		passwordLabel.SetFont(MilMo_GUI.Font.ArialRounded);
		passwordLabel.SetAlignment(MilMo_GUI.Align.CenterRight);
		passwordLabel.SetTextAlignment(MilMo_GUI.Align.CenterRight);
		_loginDialog.AddChild(passwordLabel);
		passwordTextField = new MilMo_SimplePasswordField(ui)
		{
			Identifier = "Login Password textfield",
			InputText = Settings.WebLoginStoredPassword
		};
		passwordTextField.SetPosition(ui.Center.x + vector.x + 5f, ui.Align.Top + vector.y + 25f);
		passwordTextField.SetScale(152f, 22f);
		passwordTextField.SetAlignment(MilMo_GUI.Align.CenterLeft);
		_loginDialog.AddChild(passwordTextField);
		lastErrorLabel = new MilMo_SimpleLabel(ui)
		{
			Identifier = "ErrorLabel"
		};
		lastErrorLabel.SetText(MilMo_Localization.GetLocString("LoginScreen_443"));
		lastErrorLabel.SetAlignment(MilMo_GUI.Align.BottomLeft);
		lastErrorLabel.SetTextAlignment(MilMo_GUI.Align.BottomLeft);
		lastErrorLabel.SetPosition(ui.Align.Left + 18f, ui.Align.Top + 108f);
		lastErrorLabel.SetScale(220f, 22f);
		lastErrorLabel.SetDefaultTextColor(1f, 0f, 0f, 0f);
		lastErrorLabel.SetFont(MilMo_GUI.Font.ArialRounded);
		lastErrorLabel.SetCropMode(MilMo_GUI.CropMode.Stretch);
		_loginDialog.AddChild(lastErrorLabel);
		backButton = new MilMo_Button(ui);
		backButton.SetAlignment(MilMo_GUI.Align.BottomLeft);
		backButton.SetPosition(ui.Align.Left + 18f, ui.Align.Top + 165f);
		backButton.SetTexture("Batch01/Textures/Core/Invisible");
		backButton.SetText(MilMo_Localization.GetLocString("LoginScreen_15"));
		backButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		backButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		backButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		backButton.SetScale(86f, 40f);
		backButton.SetFont(MilMo_GUI.Font.EborgSmall);
		backButton.Function = null;
		backButton.SetEnabled(e: false);
		_loginDialog.AddChild(backButton);
		goButton = new MilMo_Button(ui);
		goButton.SetAlignment(MilMo_GUI.Align.BottomRight);
		goButton.SetPosition(ui.Align.Right - 18f, ui.Align.Top + 135f);
		goButton.SetTexture("Batch01/Textures/Core/Invisible");
		goButton.SetText(MilMo_Localization.GetLocString("LoginScreen_16"));
		goButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		goButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		goButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		goButton.SetScale(86f, 40f);
		goButton.SetFont(MilMo_GUI.Font.EborgSmall);
		goButton.SetHoverSound(_tickSound);
		goButton.Function = ClkGoButton;
		_loginDialog.AddChild(goButton);
		lostPwButton = new MilMo_Button(ui);
		lostPwButton.SetAlignment(MilMo_GUI.Align.BottomLeft);
		lostPwButton.SetTextAlignment(MilMo_GUI.Align.BottomLeft);
		lostPwButton.SetPosition(ui.Align.Left + 30f, ui.Align.Top + 138f);
		lostPwButton.SetAllTextures("Batch01/Textures/Core/Invisible");
		lostPwButton.SetText(MilMo_Localization.GetLocString("LoginScreen_17"));
		lostPwButton.SetScale(150f, 20f);
		lostPwButton.SetDefaultTextColor(1f, 1f, 1f, 0.7f);
		lostPwButton.SetHoverTextColor(1f, 1f, 1f, 1f);
		lostPwButton.SetFadeInSpeed(0.1f);
		lostPwButton.SetFont(MilMo_GUI.Font.ArialRounded);
		lostPwButton.SetFontScale(0.85f);
		lostPwButton.SetHoverSound(_tickSound);
		lostPwButton.Function = ClkLostPwButton;
		_loginDialog.AddChild(lostPwButton);
		rememberPwCheckbox = new MilMo_CheckBox(ui);
		rememberPwCheckbox.SetAlignment(MilMo_GUI.Align.BottomLeft);
		rememberPwCheckbox.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		rememberPwCheckbox.SetPosition(ui.Align.Left + 12f, ui.Align.Top + 127f);
		rememberPwCheckbox.SetText(MilMo_Localization.GetLocString("LoginScreen_18"));
		rememberPwCheckbox.SetTextOffset(20f, 0f);
		rememberPwCheckbox.SetScale(150f, 20f);
		rememberPwCheckbox.SetDefaultTextColor(1f, 1f, 1f, 0.7f);
		rememberPwCheckbox.SetFont(MilMo_GUI.Font.ArialRounded);
		rememberPwCheckbox.Checked = Settings.WebLoginStoredEmail != "";
		rememberPwCheckbox.SetHoverSound(_tickSound);
		_loginDialog.AddChild(rememberPwCheckbox);
		ui.ResetLayout();
		_resetDialog = new MilMo_Dialog(ui);
		_resetDialog.Draggable = false;
		_resetDialog.SetText(MilMo_Localization.GetLocString("LoginScreen_19"));
		_resetDialog.SetScale(270f, 153f);
		ui.AddChild(_resetDialog);
		ui.ResetLayout(_resetDialog);
		resetTextBlock = new MilMo_TextBlock(ui, MilMo_Localization.GetLocString("LoginScreen_20"), new Vector2(198f, 0f));
		resetTextBlock.GoToNow(ui.Align.Left + 36f, ui.Align.Top + (vector.y - 11f));
		resetTextBlock.SetFont(MilMo_GUI.Font.ArialRounded);
		resetTextBlock.SetAlignment(MilMo_GUI.Align.TopLeft);
		_resetDialog.AddChild(resetTextBlock);
		resetLabel = new MilMo_SimpleLabel(ui);
		resetLabel.Identifier = "NameLabel";
		resetLabel.SetText(MilMo_Localization.GetLocString("LoginScreen_13"));
		resetLabel.ScaleNow(120f, 22f);
		resetLabel.SetAlignment(MilMo_GUI.Align.TopRight);
		resetLabel.SetTextAlignment(MilMo_GUI.Align.TopRight);
		resetLabel.SetFont(MilMo_GUI.Font.ArialRounded);
		resetLabel.SetPosition(ui.Center.x + vector.x - 5f, ui.Next.y);
		_resetDialog.AddChild(resetLabel);
		resetTextField = new MilMo_SimpleTextField(ui);
		resetTextField.SetTextNoLocalization("");
		resetTextField.ScaleNow(152f, 22f);
		resetTextField.SetPosPull(0.06f, 0.06f);
		resetTextField.SetAlignment(MilMo_GUI.Align.TopLeft);
		resetTextField.SetPosition(ui.Center.x + vector.x + 5f, ui.Same.y);
		_resetDialog.AddChild(resetTextField);
		submitButton = new MilMo_Button(ui);
		submitButton.SetAlignment(MilMo_GUI.Align.TopRight);
		submitButton.SetPosition(ui.Align.Right - 18f, ui.Next.y + 18f);
		submitButton.SetTexture("Batch01/Textures/Core/Invisible");
		submitButton.SetText(MilMo_Localization.GetLocString("LoginScreen_21"));
		submitButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		submitButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		submitButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		submitButton.SetScale(86f, 40f);
		submitButton.SetFont(MilMo_GUI.Font.EborgSmall);
		submitButton.SetHoverSound(_tickSound);
		submitButton.Function = RetrievePassword;
		_resetDialog.AddChild(submitButton);
		cancelButton = new MilMo_Button(ui);
		cancelButton.SetAlignment(MilMo_GUI.Align.TopRight);
		cancelButton.SetPosition(ui.Align.Right - 114f, ui.Same.y);
		cancelButton.SetTexture("Batch01/Textures/Core/Invisible");
		cancelButton.SetText(MilMo_Localization.GetLocString("LoginScreen_22"));
		cancelButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		cancelButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		cancelButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		cancelButton.SetScale(86f, 40f);
		cancelButton.SetFont(MilMo_GUI.Font.EborgSmall);
		cancelButton.SetHoverSound(_tickSound);
		cancelButton.Function = ClkCancelButton;
		_resetDialog.AddChild(cancelButton);
		_resetDialog.SetYScale(ui.Next.y + 18f);
		ShowLoginDialog();
		_loginDialog.SetColor(1f, 1f, 1f, 0f);
		_loginDialog.ColorTo(1f, 1f, 1f, 1f);
		RefreshUI();
	}

	private void ShowLoginDialog()
	{
		DisableAll();
		_loginDialog.SetEnabled(e: true);
		_loginDialog.SetColor(1f, 1f, 1f, 1f);
	}

	public new void Update()
	{
		if (_loginState == LoginState.DoLogin)
		{
			ShowLoginDialog();
			_loginState = LoginState.WaitForUser;
		}
		if (_loginState == LoginState.RedoLogin)
		{
			ShowLoginDialog();
			lastErrorLabel.TextColor.a = 1f;
			lastErrorLabel.DefaultTextColor.a = 1f;
			_loginState = LoginState.WaitForUser;
		}
		if (ui != null && ui.ScreenSizeDirty)
		{
			RefreshUI();
		}
	}

	private void DoLogin()
	{
		Debug.Log("Login: Sending form");
		loadingWidget.RefreshUI();
		loadingWidget.SetEnabled(e: true);
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("email", emailTextField.InputText);
		wWWForm.AddField("pass", passwordTextField.InputText);
		if (rememberPwCheckbox.Checked)
		{
			Settings.WebLoginStoredEmail = emailTextField.InputText;
			Settings.WebLoginStoredPassword = passwordTextField.InputText;
			Settings.Save();
		}
		else if (Settings.WebLoginStoredEmail != "" || Settings.WebLoginStoredPassword != "")
		{
			Settings.WebLoginStoredEmail = "";
			Settings.WebLoginStoredPassword = "";
			Settings.Save();
		}
		StartCoroutine(RequestLogin(MilMo_Global.WebLoginURL, wWWForm));
	}

	private IEnumerator RequestLogin(string url, WWWForm form)
	{
		UnityWebRequest www = UnityWebRequest.Post(url, form);
		yield return www.SendWebRequest();
		if (www.result != UnityWebRequest.Result.Success)
		{
			HandleAuthFail(www.error);
			yield break;
		}
		loadingWidget.SetEnabled(e: false);
		if (www.downloadHandler.text.StartsWith("-"))
		{
			HandleAuthFail(www.downloadHandler.text);
			yield break;
		}
		DisableAll();
		MilMo_Login.SetUserToken(www.downloadHandler.text);
	}

	private void HandleAuthFail(string errCode)
	{
		loadingWidget.SetEnabled(e: false);
		if (errCode.StartsWith("-1"))
		{
			_lastAuthError = "-1";
			_lastAuthErrorText = "WEB_LOGIN_USER_NOT_AUTHORIZED_YET";
			lastErrorLabel.SetText(MilMo_Localization.GetNotLocalizedLocString(_lastAuthErrorText));
			_loginState = LoginState.DoLogin;
		}
		else if (errCode.StartsWith("-2"))
		{
			_lastAuthError = "-2";
			_lastAuthErrorText = "WEB_LOGIN_USER_AUTH_INVALID";
			lastErrorLabel.SetText(MilMo_Localization.GetLocString("LoginScreen_443"));
			_loginState = LoginState.RedoLogin;
		}
		else if (errCode.StartsWith("-3"))
		{
			_lastAuthError = "-3";
			_lastAuthErrorText = "WEB_LOGIN_USER_AUTH_ERROR";
			lastErrorLabel.SetText(MilMo_Localization.GetNotLocalizedLocString(_lastAuthErrorText));
			_loginState = LoginState.RedoLogin;
		}
		else if (errCode.StartsWith("-4"))
		{
			_lastAuthError = "-4";
			_lastAuthErrorText = "WEB_LOGIN_INSTANCE_ERROR";
			lastErrorLabel.SetText(MilMo_Localization.GetNotLocalizedLocString(_lastAuthErrorText));
			_loginState = LoginState.RedoLogin;
		}
		else if (errCode.StartsWith("-5"))
		{
			_lastAuthError = "-5";
			_lastAuthErrorText = "WEB_LOGIN_NO_INSTANCE";
			lastErrorLabel.SetText(MilMo_Localization.GetNotLocalizedLocString(_lastAuthErrorText));
			_loginState = LoginState.RedoLogin;
		}
		else if (errCode.StartsWith("-6"))
		{
			_lastAuthError = "-6";
			_lastAuthErrorText = "WEB_LOGIN_MYSQL_CONNECT_ERROR";
			lastErrorLabel.SetText(MilMo_Localization.GetNotLocalizedLocString(_lastAuthErrorText));
			_loginState = LoginState.RedoLogin;
		}
		else if (errCode.StartsWith("-7"))
		{
			_lastAuthError = "-7";
			_lastAuthErrorText = "WEB_LOGIN_USER_INFO_NO_USER";
			lastErrorLabel.SetText(MilMo_Localization.GetNotLocalizedLocString(_lastAuthErrorText));
			_loginState = LoginState.RedoLogin;
		}
		else if (errCode.StartsWith("-8"))
		{
			_lastAuthError = "-8";
			_lastAuthErrorText = "WEB_LOGIN_USER_AUTH_NOT_FOUND";
			lastErrorLabel.SetText(MilMo_Localization.GetNotLocalizedLocString(_lastAuthErrorText));
			_loginState = LoginState.RedoLogin;
		}
		else if (errCode.StartsWith("4"))
		{
			_lastAuthError = "4";
			_lastAuthErrorText = "WEB_LOGIN_SERVER_4XX";
			lastErrorLabel.SetText(MilMo_Localization.GetLocString("LoginScreen_448"));
			_loginState = LoginState.RedoLogin;
		}
		else if (errCode.StartsWith("5"))
		{
			_lastAuthError = "5";
			_lastAuthErrorText = "WEB_LOGIN_SERVER_5XX";
			lastErrorLabel.SetText(MilMo_Localization.GetLocString("LoginScreen_448"));
			_loginState = LoginState.RedoLogin;
		}
		else
		{
			_lastAuthError = "-1";
			_lastAuthErrorText = "WEB_LOGIN_USER_NOT_AUTHORIZED_YET";
			lastErrorLabel.SetText(MilMo_Localization.GetLocString("LoginScreen_449"));
			_loginState = LoginState.DoLogin;
		}
		Debug.LogWarning("Authorization failed: " + _lastAuthErrorText + " (" + _lastAuthError + ")");
		MilMo_Dialog streamErrorDialog = new MilMo_Dialog(ui);
		if (ui != null)
		{
			ui.AddChild(streamErrorDialog);
		}
		streamErrorDialog.DoWarning(MilMo_Localization.GetLocString("LoginScreen_23"), MilMo_Localization.GetLocString("LoginScreen_432"), delegate
		{
			streamErrorDialog.CloseAndRemove(null);
			ShowLoginDialog();
		});
		streamErrorDialog.TextBlock.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
	}

	private void RetrievePassword(object o)
	{
		DisableAll();
		if (MilMo_Utility.IsEmail(resetTextField.InputText))
		{
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("action", "lostpassword");
			wWWForm.AddField("email", resetTextField.InputText);
			StartCoroutine(RequestResetPassword(MilMo_Global.WebLoginPasswordResetURL, wWWForm));
			return;
		}
		MilMo_Dialog brokenEmailDialog = new MilMo_Dialog(ui);
		ui.AddChild(brokenEmailDialog);
		brokenEmailDialog.DoWarning(MilMo_Localization.GetLocString("LoginScreen_456"), MilMo_Localization.GetLocString("LoginScreen_457"), delegate
		{
			brokenEmailDialog.CloseAndRemove(null);
			ShowLoginDialog();
		});
	}

	private IEnumerator RequestResetPassword(string url, WWWForm form)
	{
		UnityWebRequest www = UnityWebRequest.Post(url, form);
		yield return www.SendWebRequest();
		if (www.result != UnityWebRequest.Result.Success)
		{
			MilMo_Dialog errorDialog = new MilMo_Dialog(ui);
			if (ui != null)
			{
				ui.AddChild(errorDialog);
			}
			errorDialog.DoWarning(MilMo_Localization.GetLocString("Generic_ERROR"), MilMo_Localization.GetLocString("LoginScreen_436"), delegate
			{
				errorDialog.CloseAndRemove(null);
				ShowLoginDialog();
			});
			errorDialog.TextBlock.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		}
		else if (www.downloadHandler.text == "OK")
		{
			MilMo_Dialog retrieveDialog = new MilMo_Dialog(ui);
			if (ui != null)
			{
				ui.AddChild(retrieveDialog);
				retrieveDialog.DoOK("Batch01/Textures/Dialog/Message", MilMo_Localization.GetLocString("LoginScreen_433"), MilMo_Localization.GetLocString("LoginScreen_434"), delegate
				{
					retrieveDialog.CloseAndRemove(null);
					ShowLoginDialog();
				}, impulse: true);
				retrieveDialog.SetPosition(_loginDialog.PosMover.Target.x / ui.Res.x, _loginDialog.PosMover.Target.y / ui.Res.y);
			}
			retrieveDialog.Draggable = false;
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Confirm);
		}
		loadingWidget.SetEnabled(e: false);
	}

	private void ShowPasswordResetDialog()
	{
		DisableAll();
		_resetDialog.SetEnabled(e: true);
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Info);
	}

	private void DisableAll()
	{
		_resetDialog.SetEnabled(e: false);
		_loginDialog.SetEnabled(e: false);
	}

	private void ClkLostPwButton(object obj)
	{
		ShowPasswordResetDialog();
	}

	private void ClkGoButton(object obj)
	{
		DoLogin();
		_loginDialog.SetFadeSpeed(0.1f);
		_loginDialog.SetColor(1f, 1f, 1f, 1f);
		_loginDialog.ColorTo(1f, 1f, 1f, 0f);
	}

	private void ClkCancelButton(object obj)
	{
		ShowLoginDialog();
	}

	private new void RefreshUI()
	{
		base.RefreshUI();
		_loginDialog.SetPosition((float)Screen.width / 2f - _loginDialog.ScaleMover.Target.x / 2f, (float)Screen.height / 2f - _loginDialog.ScaleMover.Target.y);
		_resetDialog.SetPosition((float)Screen.width / 2f - _resetDialog.ScaleMover.Target.x / 2f, (float)Screen.height / 2f - _resetDialog.ScaleMover.Target.y);
	}
}
