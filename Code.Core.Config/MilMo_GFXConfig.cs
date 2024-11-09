using Code.Core.Visual;
using Code.World;
using UnityEngine;
using UnityEngine.Rendering;

namespace Code.Core.Config;

public static class MilMo_GFXConfig
{
	public sealed class GFXSettings
	{
		public enum GFXTemplate
		{
			Low,
			Medium,
			High
		}

		public readonly GFXTemplate UsedTemplate;

		public int GetGFXTemplateInt()
		{
			if (IsLow())
			{
				return 1;
			}
			if (IsMedium())
			{
				return 2;
			}
			if (IsHigh())
			{
				return 3;
			}
			return 0;
		}

		public GFXSettings(GFXTemplate template)
		{
			UsedTemplate = template;
		}

		public bool IsLow()
		{
			return UsedTemplate == GFXTemplate.Low;
		}

		public bool IsMedium()
		{
			return UsedTemplate == GFXTemplate.Medium;
		}

		public bool IsHigh()
		{
			return UsedTemplate == GFXTemplate.High;
		}
	}

	private static void SetQualitySettingsDefinedInUnity(string name)
	{
		string[] names = QualitySettings.names;
		for (int i = 0; i < names.Length; i++)
		{
			if (names[i] == name)
			{
				QualitySettings.SetQualityLevel(i, applyExpensiveChanges: true);
			}
		}
	}

	private static void SetGraphicsTierDefinedInUnity(GraphicsTier tier)
	{
		Graphics.activeTier = tier;
	}

	public static void Apply(GFXSettings settings)
	{
		Debug.Log("Applying GFX settings");
		if (settings.IsLow())
		{
			SetGraphicsTierDefinedInUnity(GraphicsTier.Tier1);
			SetQualitySettingsDefinedInUnity("Low");
			QualitySettings.softVegetation = false;
			MilMo_BlobShadow.EnableBlobShadows();
			MilMo_Terrain.UseLowEnd();
			MilMo_Lod.GlobalLodFactor = 1.2f;
			Application.targetFrameRate = 30;
		}
		else if (settings.IsMedium())
		{
			SetGraphicsTierDefinedInUnity(GraphicsTier.Tier2);
			SetQualitySettingsDefinedInUnity("Medium");
			QualitySettings.softVegetation = false;
			MilMo_BlobShadow.EnableBlobShadows();
			MilMo_Terrain.UseHighEnd();
			MilMo_Lod.GlobalLodFactor = 1f;
			Application.targetFrameRate = 30;
		}
		else if (settings.IsHigh())
		{
			SetGraphicsTierDefinedInUnity(GraphicsTier.Tier3);
			SetQualitySettingsDefinedInUnity("High");
			QualitySettings.softVegetation = true;
			MilMo_BlobShadow.EnableBlobShadows();
			MilMo_Terrain.UseHighEnd();
			MilMo_Lod.GlobalLodFactor = 1f;
			Application.targetFrameRate = 60;
		}
	}

	public static GFXSettings GetDefaultSettings()
	{
		if (SystemInfo.graphicsMemorySize < 256 || SystemInfo.graphicsShaderLevel < 30 || SystemInfo.processorCount == 1)
		{
			return new GFXSettings(GFXSettings.GFXTemplate.Low);
		}
		return new GFXSettings(GFXSettings.GFXTemplate.High);
	}
}
