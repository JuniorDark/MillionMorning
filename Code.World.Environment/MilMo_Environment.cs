using System.Linq;
using Code.Core.Global;
using Code.Core.ResourceSystem;
using Code.Core.Visual.Water;
using Code.World.ImageEffects;
using Code.World.Level;
using UnityEngine;

namespace Code.World.Environment;

public class MilMo_Environment
{
	private static GameObject _sceneLight;

	private static Light _sceneLightComponent;

	public bool DisableColorCorrection;

	private bool _storedFog;

	private float _storedFogDensity;

	private Color _storedFogColor = Color.white;

	private Color _storedAmbientLightColor = Color.white;

	public static GameObject SceneLight
	{
		get
		{
			if (!_sceneLight)
			{
				_sceneLight = new GameObject("Light");
			}
			return _sceneLight;
		}
	}

	public static Light SceneLightComponent
	{
		get
		{
			if (!_sceneLightComponent)
			{
				_sceneLightComponent = SceneLight.GetComponent<Light>();
			}
			if (!_sceneLightComponent)
			{
				_sceneLightComponent = SceneLight.AddComponent<Light>();
			}
			return _sceneLightComponent;
		}
	}

	public Color BackgroundColor { get; private set; }

	public float FogDensity { get; private set; }

	public Color FogColor { get; private set; }

	public float SunFogDensity { get; private set; }

	public Color FogColorFacingSun { get; private set; }

	public float SunFogSlice { get; private set; }

	public float SunFogImpact { get; private set; }

	public string FlarePath { get; private set; }

	public MilMo_Environment()
	{
		BackgroundColor = new Color(26f / 85f, 0.6745098f, 77f / 85f);
		FogDensity = 0.01f;
		SunFogDensity = 0.01f;
		FogColor = Color.white;
		FogColorFacingSun = Color.white;
		SunFogSlice = 10f;
		SunFogImpact = 1f;
	}

	public void Load(MilMo_Level level, bool async, MilMo_SFFile file)
	{
		Debug.Log("Loading environment for level " + level.VerboseName);
		if (!MilMo_Terrain.GameObject)
		{
			Debug.LogWarning("Trying to load environment before terrain is loaded");
		}
		SceneLightComponent.type = LightType.Directional;
		SceneLightComponent.renderMode = LightRenderMode.ForcePixel;
		MilMo_EnvironmentSettings milMo_EnvironmentSettings = MilMo_EnvironmentSettings.Load(file);
		FogDensity = milMo_EnvironmentSettings.FogDensity;
		SunFogDensity = milMo_EnvironmentSettings.SunFogDensity;
		FogColor = milMo_EnvironmentSettings.FogColor;
		FogColorFacingSun = milMo_EnvironmentSettings.FogColorFacingSun;
		SunFogSlice = milMo_EnvironmentSettings.SunFogSlice;
		SunFogImpact = milMo_EnvironmentSettings.SunFogImpact;
		RenderSettings.fog = milMo_EnvironmentSettings.EnableFog;
		RenderSettings.fogMode = FogMode.Exponential;
		RenderSettings.fogDensity = FogDensity;
		RenderSettings.fogColor = milMo_EnvironmentSettings.FogColor;
		MilMo_Terrain.ApplySettingsAsync(milMo_EnvironmentSettings);
		foreach (MilMo_WaterSettings item in milMo_EnvironmentSettings.WaterSettings.Where((MilMo_WaterSettings waterSetting) => waterSetting != null))
		{
			item.WaterContentBasePath = "Content/Worlds/" + level.WorldContentName + "/Environment/Water/";
			if (async)
			{
				MilMo_WaterManager.LoadWaterMeshAsync(item);
			}
			else
			{
				MilMo_WaterManager.LoadWaterMesh(item);
			}
		}
		SceneLightComponent.transform.eulerAngles = milMo_EnvironmentSettings.LightDirection;
		SceneLightComponent.color = milMo_EnvironmentSettings.LightColor;
		SceneLightComponent.intensity = milMo_EnvironmentSettings.LightIntensity;
		SceneLightComponent.shadows = milMo_EnvironmentSettings.LightShadows;
		SceneLightComponent.shadowStrength = milMo_EnvironmentSettings.LightShadowStrength;
		SceneLightComponent.shadowBias = milMo_EnvironmentSettings.LightShadowBias;
		SceneLightComponent.shadowNormalBias = milMo_EnvironmentSettings.LightShadowNormalBias;
		SceneLightComponent.shadowNearPlane = milMo_EnvironmentSettings.LightShadowNearPlane;
		RenderSettings.ambientLight = milMo_EnvironmentSettings.AmbientLightColor;
		BackgroundColor = milMo_EnvironmentSettings.BackgroundColor;
		if ((bool)MilMo_Global.MainCamera)
		{
			MilMo_Global.MainCamera.backgroundColor = BackgroundColor;
		}
		RenderSettings.flareStrength = milMo_EnvironmentSettings.FlareStrength;
		FlarePath = milMo_EnvironmentSettings.FlarePath;
		if (async)
		{
			LoadFlareAsync(milMo_EnvironmentSettings.FlarePath, SceneLightComponent);
		}
		else
		{
			LoadFlareLocal(milMo_EnvironmentSettings.FlarePath, SceneLightComponent);
		}
		if ((bool)MilMo_ImageEffectsHandler.Bloom)
		{
			MilMo_ImageEffectsHandler.Bloom.threshold = milMo_EnvironmentSettings.BloomThreshold;
			MilMo_ImageEffectsHandler.Bloom.intensity = milMo_EnvironmentSettings.BloomGlowIntensity;
			MilMo_ImageEffectsHandler.Bloom.blurSize = milMo_EnvironmentSettings.BloomBlurSize;
			MilMo_ImageEffectsHandler.Bloom.blurIterations = milMo_EnvironmentSettings.BloomBlurIterations;
		}
		DisableColorCorrection = milMo_EnvironmentSettings.DisableColorCorrection;
		if ((bool)MilMo_ImageEffectsHandler.ColorCorrectionCurves)
		{
			MilMo_ImageEffectsHandler.ColorCorrectionCurves.enabled = !DisableColorCorrection;
		}
		MilMo_WaterManager.DisableUnderWaterEffect = milMo_EnvironmentSettings.DisableUnderWaterEffect;
		StoreRenderSettings();
	}

	public void UnloadEnvironment(bool editorMode = false)
	{
		MilMo_WaterManager.Unload(editorMode);
	}

	public void RestoreRenderSettings()
	{
		RenderSettings.fog = _storedFog;
		RenderSettings.fogDensity = _storedFogDensity;
		RenderSettings.fogColor = _storedFogColor;
		RenderSettings.ambientLight = _storedAmbientLightColor;
	}

	public void StoreRenderSettings()
	{
		_storedFog = RenderSettings.fog;
		_storedFogDensity = RenderSettings.fogDensity;
		_storedFogColor = RenderSettings.fogColor;
		_storedAmbientLightColor = RenderSettings.ambientLight;
	}

	public void UpdateSunFog()
	{
		if (!((double)FogDensity < 0.001) && (!(FogColorFacingSun == FogColor) || SunFogDensity != FogDensity))
		{
			Vector3 forward = SceneLightComponent.transform.forward;
			forward.y = 0f;
			Vector3 forward2 = MilMo_Global.MainCamera.transform.forward;
			forward2.y = 0f;
			float num = (Vector3.Dot(forward, forward2) + 1f) / 2f;
			if (FogColorFacingSun != FogColor)
			{
				RenderSettings.fogColor = Color.Lerp(FogColorFacingSun, FogColor, num * (11f - SunFogSlice) + (1f - SunFogImpact));
			}
			if (SunFogDensity != FogDensity)
			{
				RenderSettings.fogDensity = Mathf.Lerp(SunFogDensity, FogDensity, num);
			}
		}
	}

	private static void LoadFlareLocal(string path, Light light)
	{
		if (!string.IsNullOrEmpty(path))
		{
			Flare flare = MilMo_ResourceManager.Instance.LoadFlareLocal(path);
			if (flare != null)
			{
				light.flare = flare;
			}
		}
	}

	private static async void LoadFlareAsync(string path, Light light)
	{
		if (!string.IsNullOrEmpty(path))
		{
			Flare flare = await MilMo_ResourceManager.Instance.LoadFlareAsync(path);
			if (flare != null)
			{
				light.flare = flare;
			}
		}
	}
}
