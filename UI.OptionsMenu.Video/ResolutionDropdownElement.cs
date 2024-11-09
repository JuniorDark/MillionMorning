using System.Collections.Generic;
using Core.Settings;
using UI.Elements;
using UI.HUD.Dialogues;
using UnityEngine;

namespace UI.OptionsMenu.Video;

public class ResolutionDropdownElement : DropdownElement
{
	[SerializeField]
	private ResolutionsSO resolutions;

	protected override void CreateDropdown()
	{
		dropdown.ClearOptions();
		List<string> list = new List<string>();
		for (int i = 0; i < resolutions.supportedResolutions.Count; i++)
		{
			string item = $"{resolutions.supportedResolutions[i].width} x {resolutions.supportedResolutions[i].height}";
			list.Add(item);
			if (Mathf.Approximately(resolutions.supportedResolutions[i].width, Settings.ResolutionWidth) && Mathf.Approximately(resolutions.supportedResolutions[i].height, Settings.ResolutionHeight))
			{
				SelectedDropdownIndex = i;
			}
		}
		dropdown.AddOptions(list);
		dropdown.SetValueWithoutNotify(SelectedDropdownIndex);
		dropdown.RefreshShownValue();
	}

	public void OnValueChanged(int value)
	{
		if (resolutions.supportedResolutions.Count < value)
		{
			Debug.LogWarning("Value out of bounds");
			return;
		}
		SelectedDropdownIndex = value;
		if (LastDropdownIndex != SelectedDropdownIndex)
		{
			SetSettings();
			DialogueSpawner.SpawnKeepVideoOptionsModal();
		}
	}

	protected override void SetSettings()
	{
		ResolutionsSO.Resolution resolution = resolutions.supportedResolutions[SelectedDropdownIndex];
		ResolutionsSO.Resolution windowResolution = resolutions.GetWindowResolution(SelectedDropdownIndex);
		Settings.ResolutionWidth = resolution.width;
		Settings.ResolutionHeight = resolution.height;
		Settings.WindowWidth = windowResolution.width;
		Settings.WindowHeight = windowResolution.height;
		Settings.Save();
		Settings.ApplyResolution();
	}
}
