using System;
using Code.World.Level;
using Core.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI.OptionsMenu.Misc;

public class MiscTabContent : MonoBehaviour
{
	[Serializable]
	public struct ControlToggle
	{
		public ControlModeSetting controlModeSetting;

		public Toggle toggle;
	}

	[SerializeField]
	private ControlToggle[] controlToggles;

	[SerializeField]
	private Slider cameraSensitivitySlider;

	[SerializeField]
	private Toggle playEmotesOnChatToggle;

	[SerializeField]
	private Toggle disableTutorialToggle;

	private void OnEnable()
	{
		UpdateSelectedControl();
		for (int i = 0; i < controlToggles.Length; i++)
		{
			int index = i;
			controlToggles[i].toggle.onValueChanged.AddListener(delegate
			{
				OnControlModeUpdate(controlToggles[index].controlModeSetting);
			});
		}
		cameraSensitivitySlider.minValue = 0.5f;
		cameraSensitivitySlider.maxValue = 2f;
		cameraSensitivitySlider.value = Settings.CameraSensitivity;
		cameraSensitivitySlider.onValueChanged.AddListener(OnCameraSensitivitySliderUpdate);
		playEmotesOnChatToggle.isOn = Settings.PlayEmotesOnChat;
		playEmotesOnChatToggle.onValueChanged.AddListener(OnPlayEmotesOnChatToggleUpdate);
		MilMo_Level currentLevel = MilMo_Level.CurrentLevel;
		if (currentLevel != null && currentLevel.IsStarterLevel())
		{
			disableTutorialToggle.interactable = false;
			disableTutorialToggle.isOn = false;
		}
		else
		{
			disableTutorialToggle.isOn = !Settings.ShowTutorials;
			disableTutorialToggle.onValueChanged.AddListener(OnDisableTutorialUpdate);
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < controlToggles.Length; i++)
		{
			controlToggles[i].toggle.onValueChanged.RemoveAllListeners();
		}
		cameraSensitivitySlider.onValueChanged.RemoveListener(OnCameraSensitivitySliderUpdate);
		playEmotesOnChatToggle.onValueChanged.RemoveListener(OnPlayEmotesOnChatToggleUpdate);
		disableTutorialToggle.onValueChanged.RemoveListener(OnDisableTutorialUpdate);
	}

	private void UpdateSelectedControl()
	{
		ControlModeSetting controlMode = Settings.ControlMode;
		for (int i = 0; i < controlToggles.Length; i++)
		{
			controlToggles[i].toggle.SetIsOnWithoutNotify(controlToggles[i].controlModeSetting == controlMode);
		}
	}

	private void OnControlModeUpdate(ControlModeSetting controlModeSetting)
	{
		Settings.SetControlMode(controlModeSetting);
		UpdateSelectedControl();
	}

	private void OnPlayEmotesOnChatToggleUpdate(bool b)
	{
		Settings.PlayEmotesOnChat = b;
	}

	private void OnDisableTutorialUpdate(bool b)
	{
		Settings.ShowTutorials = !b;
	}

	private void OnCameraSensitivitySliderUpdate(float f)
	{
		Settings.CameraSensitivity = f;
	}
}
