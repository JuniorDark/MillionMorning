using System;
using System.Collections.Generic;
using Core.Settings;
using UI.Elements;
using UnityEngine;
using UnityEngine.Localization;

namespace UI.OptionsMenu.Video;

public class FullscreenModeDropdownElement : DropdownElement
{
	[Serializable]
	public class FullScreenModeOption
	{
		public FullScreenMode fullScreenMode;

		public LocalizedString text;
	}

	[SerializeField]
	private FullScreenModeOption[] fullScreenModes;

	protected override void CreateDropdown()
	{
		dropdown.ClearOptions();
		List<string> list = new List<string>();
		for (int i = 0; i < fullScreenModes.Length; i++)
		{
			string item = fullScreenModes[i].text.GetLocalizedString() ?? "";
			list.Add(item);
			if (Settings.FullScreenMode == fullScreenModes[i].fullScreenMode)
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
		if (fullScreenModes.Length <= value)
		{
			Debug.LogWarning("Value out of bounds");
			return;
		}
		SelectedDropdownIndex = value;
		Apply();
	}

	protected override void SetSettings()
	{
		FullScreenMode fullScreenMode = fullScreenModes[SelectedDropdownIndex].fullScreenMode;
		if (fullScreenMode != FullScreenMode.Windowed)
		{
			Settings.FullScreenMode = fullScreenMode;
		}
		Settings.Fullscreen = fullScreenMode != FullScreenMode.Windowed;
		Settings.Save();
		Settings.ApplyResolution();
	}
}
