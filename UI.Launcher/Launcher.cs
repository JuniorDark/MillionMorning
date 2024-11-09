using System.Collections.Generic;
using Core.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Launcher;

public class Launcher : MonoBehaviour
{
	private List<string> _servers;

	private List<string> _serverLabels;

	[SerializeField]
	private TMP_Dropdown languageDropdown;

	[SerializeField]
	private TMP_Dropdown serverDropdown;

	[SerializeField]
	private TMP_Dropdown resolutionDropdown;

	[SerializeField]
	private ResolutionsSO resolutions;

	private int _selectedDropdownIndex;

	[SerializeField]
	private Toggle fullscreenToggle;

	[SerializeField]
	private TMP_Text version;

	private void Start()
	{
		Application.targetFrameRate = 30;
		QualitySettings.vSyncCount = 1;
		LocalizationSettings.SelectedLocaleAsync.WaitForCompletion();
		InitializeVersion();
		InitializeServers();
		InitializeResolutions();
		InitializeDropdowns();
		InitializeFullscreenToggle();
		Screen.SetResolution(800, 600, fullscreen: false);
	}

	private void InitializeServers()
	{
		_servers = new List<string> { "EN", "BR" };
		_serverLabels = new List<string> { "Europe", "Brasil" };
	}

	private void InitializeResolutions()
	{
		if (!resolutionDropdown)
		{
			return;
		}
		resolutionDropdown.ClearOptions();
		List<string> list = new List<string>();
		for (int i = 0; i < resolutions.supportedResolutions.Count; i++)
		{
			string item = $"{resolutions.supportedResolutions[i].width} x {resolutions.supportedResolutions[i].height}";
			list.Add(item);
			if (Mathf.Approximately(resolutions.supportedResolutions[i].width, Settings.ResolutionWidth) && Mathf.Approximately(resolutions.supportedResolutions[i].height, Settings.ResolutionHeight))
			{
				_selectedDropdownIndex = i;
			}
		}
		resolutionDropdown.AddOptions(list);
		resolutionDropdown.SetValueWithoutNotify(_selectedDropdownIndex);
		resolutionDropdown.RefreshShownValue();
		resolutionDropdown.onValueChanged.AddListener(SetResolution);
	}

	private void InitializeDropdowns()
	{
		if (!languageDropdown)
		{
			return;
		}
		languageDropdown.onValueChanged.AddListener(SetLanguage);
		if (!serverDropdown)
		{
			return;
		}
		serverDropdown.options = new List<TMP_Dropdown.OptionData>();
		for (int i = 0; i < _servers.Count; i++)
		{
			serverDropdown.options.Add(new TMP_Dropdown.OptionData(_serverLabels[i]));
			if (Settings.Server.ToUpperInvariant().Equals(_servers[i]))
			{
				serverDropdown.value = i;
			}
		}
		serverDropdown.onValueChanged.AddListener(SetServer);
		SetServer(serverDropdown.value);
		serverDropdown.RefreshShownValue();
	}

	private void InitializeVersion()
	{
		string text = "";
		version.text = "Version: " + Application.version + " " + text;
	}

	private void InitializeFullscreenToggle()
	{
		if ((bool)fullscreenToggle)
		{
			fullscreenToggle.isOn = Settings.Fullscreen;
			fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
		}
	}

	public void StartGame()
	{
		SceneManager.LoadScene("Main");
	}

	private void SetLanguage(int value)
	{
		Settings.Language = LocalizationSettings.AvailableLocales.Locales[value];
		SaveSettings();
	}

	private void SetServer(int value)
	{
		if (_servers.Count >= value)
		{
			Settings.Server = _servers[value];
			SaveSettings();
		}
	}

	private void SetFullscreen(bool value)
	{
		Settings.Fullscreen = value;
		SaveSettings();
	}

	private void SetResolution(int value)
	{
		if (resolutions.supportedResolutions.Count < value)
		{
			Debug.LogWarning("Value out of bounds");
			return;
		}
		_selectedDropdownIndex = value;
		ResolutionsSO.Resolution resolution = resolutions.supportedResolutions[_selectedDropdownIndex];
		ResolutionsSO.Resolution windowResolution = resolutions.GetWindowResolution(_selectedDropdownIndex);
		Settings.ResolutionWidth = resolution.width;
		Settings.ResolutionHeight = resolution.height;
		Settings.WindowWidth = windowResolution.width;
		Settings.WindowHeight = windowResolution.height;
		SaveSettings();
	}

	private void SaveSettings()
	{
		Settings.Save();
	}
}
