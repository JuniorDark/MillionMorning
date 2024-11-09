using System;
using System.Collections.Generic;
using Code.Core.Config;
using Core.Settings;
using UI.Elements;
using UnityEngine;
using UnityEngine.Localization;

namespace UI.OptionsMenu.Video;

public class GraphicsQualityDropdownElement : DropdownElement
{
	[Serializable]
	public class GraphicQualityOptions
	{
		public Settings.QualityTierSetting tier;

		public LocalizedString text;
	}

	[SerializeField]
	private GraphicQualityOptions[] qualityTiers;

	protected override void CreateDropdown()
	{
		dropdown.ClearOptions();
		List<string> list = new List<string>();
		for (int i = 0; i < qualityTiers.Length; i++)
		{
			list.Add(qualityTiers[i].text.GetLocalizedString());
			if (Settings.QualityTier == qualityTiers[i].tier)
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
		if (qualityTiers.Length < value)
		{
			Debug.LogWarning("Value out of bounds");
			return;
		}
		SelectedDropdownIndex = value;
		Apply();
	}

	protected override void SetSettings()
	{
		Settings.QualityTier = qualityTiers[SelectedDropdownIndex].tier;
		Settings.Save();
		switch (Settings.QualityTier)
		{
		case Settings.QualityTierSetting.Low:
			MilMo_GFXConfig.Apply(new MilMo_GFXConfig.GFXSettings(MilMo_GFXConfig.GFXSettings.GFXTemplate.Low));
			break;
		case Settings.QualityTierSetting.Medium:
			MilMo_GFXConfig.Apply(new MilMo_GFXConfig.GFXSettings(MilMo_GFXConfig.GFXSettings.GFXTemplate.Medium));
			break;
		case Settings.QualityTierSetting.High:
			MilMo_GFXConfig.Apply(new MilMo_GFXConfig.GFXSettings(MilMo_GFXConfig.GFXSettings.GFXTemplate.High));
			break;
		default:
			MilMo_GFXConfig.Apply(MilMo_GFXConfig.GetDefaultSettings());
			break;
		}
	}
}
