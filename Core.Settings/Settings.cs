using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Input;
using Core.GameEvent;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Core.Settings;

[Serializable]
public static class Settings
{
	public enum QualityTierSetting
	{
		Low = 1,
		Medium,
		High
	}

	private static bool _initialized;

	private static Locale _language;

	private static string _server = "EN";

	private static bool _fullscreen;

	private static FullScreenMode _fullScreenMode = FullScreenMode.FullScreenWindow;

	private static int _resolutionWidth = 800;

	private static int _resolutionHeight = 600;

	private static int _windowWidth = 800;

	private static int _windowHeight = 600;

	private static QualityTierSetting _qualityTier = QualityTierSetting.High;

	private static ControlModeSetting _controlMode = ControlModeSetting.MMORPG;

	private static float _cameraSensitivity = 1f;

	private static float _masterVolume = 0.9f;

	private static float _sfxVolume = 0.9f;

	private static float _musicVolume = 0.9f;

	private static float _ambienceVolume = 0.9f;

	public static string WebLoginStoredEmail = "";

	public static string WebLoginStoredPassword = "";

	private static bool _playEmotesOnChat = true;

	private static bool _showTutorials = true;

	public static Locale Language
	{
		get
		{
			return _language;
		}
		set
		{
			if (ValidateLocale(value))
			{
				_language = value;
			}
		}
	}

	public static string Server
	{
		get
		{
			return _server;
		}
		set
		{
			_server = value;
		}
	}

	public static bool Fullscreen
	{
		get
		{
			return _fullscreen;
		}
		set
		{
			_fullscreen = value;
		}
	}

	public static FullScreenMode FullScreenMode
	{
		get
		{
			return _fullScreenMode;
		}
		set
		{
			_fullScreenMode = value;
		}
	}

	public static int ResolutionWidth
	{
		get
		{
			return _resolutionWidth;
		}
		set
		{
			_resolutionWidth = Mathf.Clamp(value, 800, int.MaxValue);
		}
	}

	public static int ResolutionHeight
	{
		get
		{
			return _resolutionHeight;
		}
		set
		{
			_resolutionHeight = Mathf.Clamp(value, 600, int.MaxValue);
		}
	}

	public static int WindowWidth
	{
		get
		{
			return _windowWidth;
		}
		set
		{
			_windowWidth = Mathf.Clamp(value, 800, int.MaxValue);
		}
	}

	public static int WindowHeight
	{
		get
		{
			return _windowHeight;
		}
		set
		{
			_windowHeight = Mathf.Clamp(value, 600, int.MaxValue);
		}
	}

	public static QualityTierSetting QualityTier
	{
		get
		{
			return _qualityTier;
		}
		set
		{
			_qualityTier = value;
		}
	}

	public static ControlModeSetting ControlMode
	{
		get
		{
			return _controlMode;
		}
		set
		{
			_controlMode = value;
		}
	}

	public static float CameraSensitivity
	{
		get
		{
			return _cameraSensitivity;
		}
		set
		{
			_cameraSensitivity = Mathf.Clamp(value, 0.5f, 2f);
		}
	}

	public static float MasterVolume => _masterVolume;

	public static float SFXVolume => _sfxVolume;

	public static float MusicVolume => _musicVolume;

	public static float AmbienceVolume => _ambienceVolume;

	public static bool PlayEmotesOnChat
	{
		get
		{
			return _playEmotesOnChat;
		}
		set
		{
			_playEmotesOnChat = value;
		}
	}

	public static bool ShowTutorials
	{
		get
		{
			return _showTutorials;
		}
		set
		{
			_showTutorials = value;
		}
	}

	private static bool ValidateLocale(Locale locale)
	{
		if (locale != null)
		{
			return LocalizationSettings.AvailableLocales.Locales.FirstOrDefault((Locale l) => l == locale);
		}
		return false;
	}

	private static Locale GetLocaleByIdentifier(string language)
	{
		Locale selectedLocale = LocalizationSettings.SelectedLocaleAsync.WaitForCompletion();
		List<Locale> locales = LocalizationSettings.AvailableLocales.Locales;
		if (language == "" && (bool)selectedLocale && (bool)locales.FirstOrDefault((Locale l) => l == selectedLocale))
		{
			language = selectedLocale.Identifier.Code;
		}
		if (!locales.FirstOrDefault((Locale l) => l.Identifier.Code == language))
		{
			language = "en";
		}
		return LocalizationSettings.SelectedLocale = locales.FirstOrDefault((Locale l) => l.Identifier.Code == language);
	}

	public static void ToggleFullscreen()
	{
		Fullscreen = !Screen.fullScreen;
		Save();
		ApplyResolution();
	}

	public static void ApplyResolution()
	{
		if (Fullscreen)
		{
			Screen.SetResolution(ResolutionWidth, ResolutionHeight, FullScreenMode);
		}
		else
		{
			Screen.SetResolution(WindowWidth, WindowHeight, FullScreenMode.Windowed);
		}
	}

	public static void SetControlMode(ControlModeSetting control)
	{
		Debug.LogWarning("Setting controller: " + Enum.GetName(typeof(ControlModeSetting), control));
		ControlMode = control;
		MilMo_Input.SetDefaultControlMode(control);
	}

	public static void SetMasterVolume(float value)
	{
		_masterVolume = Mathf.Clamp(value, 0f, 1f);
		Core.GameEvent.GameEvent.MasterVolumeEvent.RaiseEvent(_masterVolume);
	}

	public static void SetSFXVolume(float value)
	{
		_sfxVolume = Mathf.Clamp(value, 0f, 1f);
		Core.GameEvent.GameEvent.SFXVolumeEvent.RaiseEvent(_sfxVolume);
	}

	public static void SetMusicVolume(float value)
	{
		_musicVolume = Mathf.Clamp(value, 0f, 1f);
		Core.GameEvent.GameEvent.MusicVolumeEvent.RaiseEvent(_musicVolume);
	}

	public static void SetAmbienceVolume(float value)
	{
		_ambienceVolume = Mathf.Clamp(value, 0f, 1f);
		Core.GameEvent.GameEvent.AmbienceVolumeEvent.RaiseEvent(_ambienceVolume);
	}

	public static T ReadSetting<T>(string key, T defaultValue)
	{
		if (!PlayerPrefs.HasKey(key))
		{
			return defaultValue;
		}
		if (typeof(T) == typeof(string))
		{
			return (T)(object)PlayerPrefs.GetString(key);
		}
		if (typeof(T) == typeof(float))
		{
			return (T)(object)PlayerPrefs.GetFloat(key);
		}
		if (typeof(T) == typeof(int))
		{
			return (T)(object)PlayerPrefs.GetInt(key);
		}
		if (typeof(T) == typeof(bool))
		{
			return (T)(object)(PlayerPrefs.GetInt(key) != 0);
		}
		Debug.LogWarning($"Unable to read {key} from PlayerPref: {typeof(T)} is not supported.");
		return default(T);
	}

	public static void SaveSetting<T>(string key, T value)
	{
		if (typeof(T) == typeof(string))
		{
			PlayerPrefs.SetString(key, (string)(object)value);
		}
		else if (typeof(T) == typeof(int))
		{
			PlayerPrefs.SetInt(key, (int)(object)value);
		}
		else if (typeof(T) == typeof(float))
		{
			PlayerPrefs.SetFloat(key, (float)(object)value);
		}
		else if (typeof(T) == typeof(bool))
		{
			PlayerPrefs.SetInt(key, ((bool)(object)value) ? 1 : 0);
		}
		else
		{
			Debug.LogWarning($"Unable to save {key}:{value} to PlayerPref: {typeof(T)} is not supported.");
		}
	}

	public static void Init()
	{
		if (!_initialized)
		{
			Language = GetLocaleByIdentifier(ReadSetting("ClientLanguage", "en"));
			Server = ReadSetting("ClientServer", _server);
			Fullscreen = ReadSetting("ClientFullscreen", _fullscreen);
			FullScreenMode = (FullScreenMode)ReadSetting("ClientFullScreenMode", (int)_fullScreenMode);
			ResolutionWidth = ReadSetting("ClientResolutionWidth", _resolutionWidth);
			ResolutionHeight = ReadSetting("ClientResolutionHeight", _resolutionHeight);
			WindowWidth = ReadSetting("ClientWindowWidth", _windowWidth);
			WindowHeight = ReadSetting("ClientWindowHeight", _windowHeight);
			QualityTier = (QualityTierSetting)ReadSetting("ClientQualityTier", (int)_qualityTier);
			ControlMode = (ControlModeSetting)ReadSetting("ClientControlMode", (int)_controlMode);
			CameraSensitivity = ReadSetting("ClientCameraSensitivity", _cameraSensitivity);
			SetMasterVolume(ReadSetting("ClientMasterVolume", _masterVolume));
			SetSFXVolume(ReadSetting("ClientSFXVolume", _sfxVolume));
			SetMusicVolume(ReadSetting("ClientMusicVolume", _musicVolume));
			SetAmbienceVolume(ReadSetting("ClientAmbienceVolume", _sfxVolume));
			string text = ReadSetting("ClientWebLoginStoredEmail", "");
			string text2 = ReadSetting("ClientWebLoginStoredPassword", "");
			WebLoginStoredEmail = ((!string.IsNullOrEmpty(text)) ? MD5.Decrypt(text) : text);
			WebLoginStoredPassword = ((!string.IsNullOrEmpty(text2)) ? MD5.Decrypt(text2) : text2);
			PlayEmotesOnChat = ReadSetting("ClientPlayEmotesOnChat", _playEmotesOnChat);
			ShowTutorials = ReadSetting("ClientShowTutorials", _showTutorials);
			Core.GameEvent.GameEvent.ToggleFullscreenEvent.RegisterAction(ToggleFullscreen);
			_initialized = true;
		}
	}

	public static void Save()
	{
		if ((bool)_language)
		{
			SaveSetting("ClientLanguage", _language.Identifier.Code);
		}
		SaveSetting("ClientServer", _server);
		SaveSetting("ClientFullscreen", _fullscreen);
		SaveSetting("ClientFullScreenMode", (int)_fullScreenMode);
		SaveSetting("ClientResolutionWidth", _resolutionWidth);
		SaveSetting("ClientResolutionHeight", _resolutionHeight);
		SaveSetting("ClientWindowWidth", _windowWidth);
		SaveSetting("ClientWindowHeight", _windowHeight);
		SaveSetting("ClientQualityTier", (int)_qualityTier);
		SaveSetting("ClientControlMode", (int)_controlMode);
		SaveSetting("ClientCameraSensitivity", _cameraSensitivity);
		SaveSetting("ClientMasterVolume", _masterVolume);
		SaveSetting("ClientSFXVolume", _sfxVolume);
		SaveSetting("ClientMusicVolume", _musicVolume);
		SaveSetting("ClientAmbienceVolume", _ambienceVolume);
		SaveSetting("ClientWebLoginStoredEmail", (!string.IsNullOrEmpty(WebLoginStoredEmail)) ? MD5.Encrypt(WebLoginStoredEmail) : "");
		SaveSetting("ClientWebLoginStoredPassword", (!string.IsNullOrEmpty(WebLoginStoredPassword)) ? MD5.Encrypt(WebLoginStoredPassword) : "");
		SaveSetting("ClientPlayEmotesOnChat", _playEmotesOnChat);
		SaveSetting("ClientShowTutorials", _showTutorials);
	}
}
