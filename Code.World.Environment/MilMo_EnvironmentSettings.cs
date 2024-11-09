using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Global;
using Code.Core.ResourceSystem;
using Code.Core.Visual.Water;
using UnityEngine;

namespace Code.World.Environment;

public class MilMo_EnvironmentSettings
{
	public class TerrainLayer
	{
		public int Index;

		public string Path;
	}

	public class DetailSprite
	{
		public int Index;

		public string Path;

		public Color DryColor;

		public Color HealthyColor;
	}

	public class TreeObject
	{
		public int Index;

		public string Path;
	}

	public readonly List<TerrainLayer> TerrainLayers = new List<TerrainLayer>();

	public readonly List<DetailSprite> DetailSprites = new List<DetailSprite>();

	public readonly List<TreeObject> TreeObjects = new List<TreeObject>();

	public bool EnableFog;

	public float FogDensity = 0.01f;

	public float SunFogDensity = 0.01f;

	public Color FogColor = Color.white;

	public Color FogColorFacingSun = Color.white;

	public float SunFogSlice = 10f;

	public float SunFogImpact = 1f;

	public float TreeBillboardDistance;

	public float TreeCrossFadeLength;

	public float TreeDistance;

	public int TreeMaximumFullLODCount;

	public int HeightmapMaximumLOD;

	public float HeightmapPixelError;

	public float BasemapDistance;

	public float DetailObjectDistance;

	public bool TerrainCastShadows;

	public Vector3 TerrainPosition = Vector3.zero;

	public readonly List<MilMo_WaterSettings> WaterSettings = new List<MilMo_WaterSettings>();

	public Vector3 LightDirection = new Vector3(0.5f, 0f, 0f);

	public Color LightColor = Color.white;

	public float LightIntensity = 0.2f;

	public LightShadows LightShadows;

	public float LightShadowStrength = 1f;

	public float LightShadowBias = 0.7f;

	public float LightShadowNormalBias = 0.4f;

	public float LightShadowNearPlane = 0.2f;

	public Color AmbientLightColor = Color.white;

	public Color BackgroundColor = Color.black;

	public float FlareStrength;

	public string FlarePath;

	public float BloomThreshold = 0.85f;

	public float BloomGlowIntensity = 1.5f;

	public float BloomBlurSize = 1f;

	public int BloomBlurIterations = 2;

	public bool DisableColorCorrection;

	public bool DisableUnderWaterEffect;

	public static MilMo_EnvironmentSettings Load(MilMo_SFFile file)
	{
		MilMo_EnvironmentSettings milMo_EnvironmentSettings = new MilMo_EnvironmentSettings();
		if (file == null)
		{
			Debug.LogError("MilMo_EnvironmentSettings: Unable to read file");
			return milMo_EnvironmentSettings;
		}
		Dictionary<int, string> dictionary = new Dictionary<int, string>();
		while (file.NextRow())
		{
			if (file.IsNext("ENVIRONMENT"))
			{
				dictionary.Add(file.GetLineNumber(), file.GetString());
			}
		}
		file.Reset();
		int num = 0;
		if (dictionary.Count > 0)
		{
			foreach (KeyValuePair<int, string> item in dictionary)
			{
				if (MilMo_Global.EventTags.Contains(item.Value) || item.Value == "Default")
				{
					num = item.Key + 1;
				}
			}
		}
		while (file.NextRow())
		{
			if (file.GetLineNumber() < num)
			{
				continue;
			}
			if (file.IsNext("ENVIRONMENT"))
			{
				break;
			}
			if (file.IsNext("TerrainLayer"))
			{
				int @int = file.GetInt();
				string @string = file.GetString();
				milMo_EnvironmentSettings.TerrainLayers.Add(new TerrainLayer
				{
					Index = @int,
					Path = @string
				});
			}
			else if (file.IsNext("DetailSprite"))
			{
				int int2 = file.GetInt();
				string string2 = file.GetString();
				Color dryColor = (file.HasMoreTokens() ? file.GetColor() : Color.green);
				Color healthyColor = (file.HasMoreTokens() ? file.GetColor() : Color.green);
				milMo_EnvironmentSettings.DetailSprites.Add(new DetailSprite
				{
					Index = int2,
					Path = string2,
					DryColor = dryColor,
					HealthyColor = healthyColor
				});
			}
			else if (file.IsNext("TreeObject"))
			{
				int int3 = file.GetInt();
				string string3 = file.GetString();
				milMo_EnvironmentSettings.TreeObjects.Add(new TreeObject
				{
					Index = int3,
					Path = string3
				});
			}
			else if (file.IsNext("TreeBillboardDistance"))
			{
				milMo_EnvironmentSettings.TreeBillboardDistance = file.GetFloat();
			}
			else if (file.IsNext("TreeCrossFadeLength"))
			{
				milMo_EnvironmentSettings.TreeCrossFadeLength = file.GetFloat();
			}
			else if (file.IsNext("TreeDistance"))
			{
				milMo_EnvironmentSettings.TreeDistance = file.GetFloat();
			}
			else if (file.IsNext("TreeMaximumFullLODCount"))
			{
				milMo_EnvironmentSettings.TreeMaximumFullLODCount = (int)file.GetFloat();
			}
			else if (file.IsNext("HeightmapMaximumLOD"))
			{
				milMo_EnvironmentSettings.HeightmapMaximumLOD = file.GetInt();
			}
			else if (file.IsNext("HeightmapPixelError"))
			{
				milMo_EnvironmentSettings.HeightmapPixelError = file.GetFloat();
			}
			else if (file.IsNext("BasemapDistance"))
			{
				milMo_EnvironmentSettings.BasemapDistance = file.GetFloat();
			}
			else if (file.IsNext("DetailObjectDistance"))
			{
				milMo_EnvironmentSettings.DetailObjectDistance = file.GetFloat();
			}
			else if (file.IsNext("Water"))
			{
				milMo_EnvironmentSettings.WaterSettings.Add(MilMo_WaterSettings.Load(file));
			}
			else if (file.IsNext("Fog"))
			{
				milMo_EnvironmentSettings.EnableFog = file.GetBool();
			}
			else if (file.IsNext("FogDensity"))
			{
				milMo_EnvironmentSettings.FogDensity = file.GetFloat();
				milMo_EnvironmentSettings.SunFogDensity = milMo_EnvironmentSettings.FogDensity;
			}
			else if (file.IsNext("SunFogDensity"))
			{
				milMo_EnvironmentSettings.SunFogDensity = file.GetFloat();
			}
			else if (file.IsNext("FogColor"))
			{
				milMo_EnvironmentSettings.FogColor = file.GetColor();
				milMo_EnvironmentSettings.FogColorFacingSun = milMo_EnvironmentSettings.FogColor;
			}
			else if (file.IsNext("FogColorFacingSun"))
			{
				milMo_EnvironmentSettings.FogColorFacingSun = file.GetColor();
			}
			else if (file.IsNext("SunFogSlice"))
			{
				milMo_EnvironmentSettings.SunFogSlice = file.GetFloat();
			}
			else if (file.IsNext("SunFogImpact"))
			{
				milMo_EnvironmentSettings.SunFogImpact = file.GetFloat();
			}
			else if (file.IsNext("BackgroundColor"))
			{
				milMo_EnvironmentSettings.BackgroundColor = file.GetColor();
			}
			else if (file.IsNext("AmbientLightColor"))
			{
				milMo_EnvironmentSettings.AmbientLightColor = file.GetColor();
			}
			else if (file.IsNext("Flare"))
			{
				milMo_EnvironmentSettings.FlarePath = file.GetString();
			}
			else if (file.IsNext("FlareStrength"))
			{
				milMo_EnvironmentSettings.FlareStrength = file.GetFloat();
			}
			else if (file.IsNext("LightDirection"))
			{
				milMo_EnvironmentSettings.LightDirection = file.GetVector3();
			}
			else if (file.IsNext("LightColor"))
			{
				milMo_EnvironmentSettings.LightColor = file.GetColor();
			}
			else if (file.IsNext("LightIntensity"))
			{
				milMo_EnvironmentSettings.LightIntensity = file.GetFloat();
			}
			else if (file.IsNext("LightShadows"))
			{
				string string4 = file.GetString();
				if (string4.Equals("Hard", StringComparison.InvariantCultureIgnoreCase))
				{
					milMo_EnvironmentSettings.LightShadows = LightShadows.Hard;
				}
				else if (string4.Equals("Soft", StringComparison.InvariantCultureIgnoreCase))
				{
					milMo_EnvironmentSettings.LightShadows = LightShadows.Soft;
				}
				else if (string4.Equals("None", StringComparison.InvariantCultureIgnoreCase))
				{
					milMo_EnvironmentSettings.LightShadows = LightShadows.None;
				}
			}
			else if (file.IsNext("LightShadowStrength"))
			{
				milMo_EnvironmentSettings.LightShadowStrength = file.GetFloat();
			}
			else if (file.IsNext("LightShadowBias"))
			{
				milMo_EnvironmentSettings.LightShadowBias = file.GetFloat();
			}
			else if (file.IsNext("LightShadowNormalBias"))
			{
				milMo_EnvironmentSettings.LightShadowNormalBias = file.GetFloat();
			}
			else if (file.IsNext("LightShadowNearPlane"))
			{
				milMo_EnvironmentSettings.LightShadowNearPlane = file.GetFloat();
			}
			else if (file.IsNext("TerrainCastShadows"))
			{
				milMo_EnvironmentSettings.TerrainCastShadows = file.GetBool();
			}
			else if (file.IsNext("BloomThreshold"))
			{
				milMo_EnvironmentSettings.BloomThreshold = file.GetFloat();
			}
			else if (file.IsNext("BloomGlowIntensity"))
			{
				milMo_EnvironmentSettings.BloomGlowIntensity = file.GetFloat();
			}
			else if (file.IsNext("BloomBlurSize"))
			{
				milMo_EnvironmentSettings.BloomBlurSize = file.GetFloat();
			}
			else if (file.IsNext("BloomBlurIterations"))
			{
				milMo_EnvironmentSettings.BloomBlurIterations = file.GetInt();
			}
			else if (file.IsNext("DisableColorCorrection"))
			{
				milMo_EnvironmentSettings.DisableColorCorrection = true;
			}
			else if (file.IsNext("TerrainPosition"))
			{
				milMo_EnvironmentSettings.TerrainPosition = file.GetVector3();
			}
			else if (file.IsNext("DisableUnderWaterEffect"))
			{
				milMo_EnvironmentSettings.DisableUnderWaterEffect = true;
			}
		}
		file.Reset();
		return milMo_EnvironmentSettings;
	}
}
