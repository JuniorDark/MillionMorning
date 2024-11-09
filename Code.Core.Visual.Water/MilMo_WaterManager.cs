using System.Collections.Generic;
using System.Linq;
using Code.Core.Collision;
using Code.Core.Global;
using Code.Core.ResourceSystem;
using Code.World.Environment;
using UnityEngine;

namespace Code.Core.Visual.Water;

public static class MilMo_WaterManager
{
	public enum WaterLevel
	{
		Land,
		Shallow,
		Deep
	}

	private static readonly List<GameObject> WaterPlanes = new List<GameObject>();

	public static readonly List<MilMo_Volume> WaterVolumes = new List<MilMo_Volume>();

	private const float DEEP_WATER_LIMIT = 0.53376f;

	private static readonly int AlphaMask = Shader.PropertyToID("_AlphaMask");

	private static readonly int MainColor = Shader.PropertyToID("_Color");

	private static readonly int BumpMap = Shader.PropertyToID("_BumpMap");

	private static readonly int ReflectiveColor = Shader.PropertyToID("_ReflectiveColor");

	private static readonly int WaveScale = Shader.PropertyToID("_WaveScale");

	private static readonly int WaveSpeed = Shader.PropertyToID("_WaveSpeed");

	private static readonly int WaveStrength = Shader.PropertyToID("_WaveStrength");

	private static readonly int RefractionDistort = Shader.PropertyToID("_RefrDistort");

	private static readonly int ReflectionDistort = Shader.PropertyToID("_ReflDistort");

	private static readonly int RefrColor = Shader.PropertyToID("_RefrColor");

	private static readonly int MinAlpha = Shader.PropertyToID("_MinAlpha");

	public static bool DisableUnderWaterEffect { get; set; }

	public static WaterLevel GetWaterLevel(Vector3 position, out float surfaceY)
	{
		using (IEnumerator<MilMo_Volume> enumerator = WaterVolumes.Where((MilMo_Volume volume) => volume.IsInside(position)).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				MilMo_Volume current = enumerator.Current;
				surfaceY = current.GetSurface();
				Vector3 pos = position;
				pos.y += 0.5f;
				float num = pos.y - MilMo_Physics.GetDistanceToGround(pos);
				return (!(surfaceY - num >= 0.53376f)) ? WaterLevel.Shallow : WaterLevel.Deep;
			}
		}
		surfaceY = 0f;
		return WaterLevel.Land;
	}

	public static async void LoadWaterMeshAsync(MilMo_WaterSettings settings)
	{
		string preset = settings.WaterContentBasePath + "Presets/" + settings.WaterPreset;
		string path = settings.WaterContentBasePath + settings.WaterMesh;
		GameObject gameObject = await MilMo_ResourceManager.Instance.LoadMeshAsync(path);
		if (!gameObject)
		{
			Debug.LogWarning("Failed to load game object for water");
			return;
		}
		Renderer waterRenderer = GetWaterRenderer(gameObject);
		if (!waterRenderer)
		{
			Debug.LogWarning("Water mesh '" + path + "' has no renderer");
			return;
		}
		MilMo_SFFile milMo_SFFile = await MilMo_SimpleFormat.RealAsyncLoad(preset);
		if (milMo_SFFile == null)
		{
			Debug.LogWarning("Failed to load preset '" + preset + "' for water");
		}
		else
		{
			InitializeWater(waterRenderer, settings, milMo_SFFile, async: true);
		}
	}

	public static void LoadWaterMesh(MilMo_WaterSettings settings)
	{
		string text = settings.WaterContentBasePath + "Presets/" + settings.WaterPreset;
		string text2 = settings.WaterContentBasePath + settings.WaterMesh;
		GameObject gameObject = MilMo_ResourceManager.Instance.LoadGameObjectLocal(text2);
		if (!gameObject)
		{
			Debug.LogWarning("Failed to load game object for water");
			return;
		}
		Renderer waterRenderer = GetWaterRenderer(gameObject);
		if (!waterRenderer)
		{
			Debug.LogWarning("Water mesh '" + text2 + "' has no renderer");
			return;
		}
		MilMo_SFFile milMo_SFFile = MilMo_SimpleFormat.LoadLocal(text);
		if (milMo_SFFile == null)
		{
			Debug.LogWarning("Failed to load preset '" + text + "' for water");
		}
		else
		{
			InitializeWater(waterRenderer, settings, milMo_SFFile, async: false);
		}
	}

	private static Renderer GetWaterRenderer(GameObject gameObject)
	{
		Renderer component = gameObject.GetComponent<Renderer>();
		if ((bool)component)
		{
			return component;
		}
		component = gameObject.GetComponentInChildren<Renderer>();
		if ((bool)component)
		{
			return component;
		}
		return null;
	}

	private static void InitializeWater(Renderer waterRenderer, MilMo_WaterSettings settings, MilMo_SFFile presetFile, bool async)
	{
		Shader shader = Resources.Load<Shader>("Shaders/Junebug/Water");
		if (!shader)
		{
			Debug.LogWarning("Failed to load water shader");
			return;
		}
		waterRenderer.material = new Material(shader);
		GameObject gameObject = Object.Instantiate(waterRenderer.gameObject);
		AddWaterSurface(gameObject, settings);
		AddWaterVolume(gameObject, settings);
		MilMo_Water milMo_Water = gameObject.AddComponent<MilMo_Water>();
		if (!milMo_Water)
		{
			Debug.LogWarning("Failed to add MilMo_Water script");
			return;
		}
		gameObject.layer = LayerMask.NameToLayer("Water");
		milMo_Water.preset = presetFile.Name;
		MilMo_WaterSunParticles milMo_WaterSunParticles = gameObject.AddComponent<MilMo_WaterSunParticles>();
		if ((bool)milMo_WaterSunParticles)
		{
			milMo_WaterSunParticles.SetWater(gameObject);
			milMo_WaterSunParticles.SetSunLight(MilMo_Environment.SceneLight);
		}
		LoadAlphaTexture(waterRenderer, settings, async);
		MilMo_WaterPreset milMo_WaterPreset = MilMo_WaterPreset.Load(presetFile);
		LoadBumpTexture(waterRenderer, settings.WaterContentBasePath + milMo_WaterPreset.BumpPath, async);
		LoadReflectiveTexture(waterRenderer, settings.WaterContentBasePath + milMo_WaterPreset.ReflectivePath, async);
		ApplyPreset(waterRenderer, milMo_Water, milMo_WaterSunParticles, milMo_WaterPreset);
	}

	private static void ApplyPreset(Renderer renderer, MilMo_Water waterScript, MilMo_WaterSunParticles waterSparks, MilMo_WaterPreset preset)
	{
		renderer.sharedMaterial.SetColor(MainColor, preset.MainColor);
		renderer.sharedMaterial.SetFloat(WaveScale, preset.WaveScale);
		renderer.sharedMaterial.SetFloat(WaveSpeed, preset.WaveSpeed);
		renderer.sharedMaterial.SetFloat(WaveStrength, preset.WaveStrength);
		renderer.sharedMaterial.SetFloat(ReflectionDistort, preset.ReflectionDistort);
		renderer.sharedMaterial.SetFloat(RefractionDistort, preset.RefractionDistort);
		renderer.sharedMaterial.SetColor(RefrColor, preset.RefractionColor);
		renderer.sharedMaterial.SetFloat(MinAlpha, preset.MinAlpha);
		waterScript.disablePixelLights = preset.DisablePixelLights;
		if ((bool)waterSparks)
		{
			waterSparks.enableSunSparkles = preset.EnableSunSparkles;
			waterSparks.particleSize = preset.ParticleSize;
			waterSparks.sunTrailWidthOffset = preset.SunTrailWidthOffset;
			waterSparks.enableAmbientSparkles = preset.EnableAmbientSparkles;
			waterSparks.ambientLength = preset.AmbientLength;
			waterSparks.numAmbientParticles = preset.NumAmbientParticles;
		}
	}

	private static void AddWaterSurface(GameObject gameObject, MilMo_WaterSettings settings)
	{
		gameObject.transform.position = settings.WaterPosition;
		gameObject.transform.eulerAngles = settings.WaterRotation;
		gameObject.transform.localScale = new Vector3(settings.WaterScale.x, settings.WaterScaleHeight, settings.WaterScale.y);
		gameObject.name = settings.WaterMesh;
		WaterPlanes.Add(gameObject);
	}

	private static void AddWaterVolume(GameObject gameObject, MilMo_WaterSettings settings)
	{
		Vector3 position = gameObject.transform.position;
		position.y -= 25f;
		Transform transform = new GameObject().transform;
		transform.position = position;
		MilMo_BoxTemplate milMo_BoxTemplate = new MilMo_BoxTemplate(new Vector3(settings.WaterScale.x * 2f, 50f, settings.WaterScale.y * 2f));
		WaterVolumes.Add(milMo_BoxTemplate.Instantiate(transform));
	}

	private static void LoadBumpTexture(Renderer renderer, string bumpPath, bool async)
	{
		if (async)
		{
			LoadAndSetBumpTextureAsync(renderer, bumpPath);
			return;
		}
		Texture2D texture2D = MilMo_ResourceManager.Instance.LoadTextureLocal(bumpPath);
		if (!texture2D)
		{
			Debug.LogWarning("Got invalid water bumpmap texture '" + bumpPath + "'");
		}
		else
		{
			SetBumpTexture(renderer, texture2D);
		}
	}

	private static async void LoadAndSetBumpTextureAsync(Renderer renderer, string bumpPath)
	{
		Texture2D texture2D = await MilMo_ResourceManager.Instance.LoadTextureAsync(bumpPath);
		if (texture2D == null)
		{
			Debug.LogWarning("Got invalid water bumpmap texture '" + bumpPath + "'");
		}
		else
		{
			SetBumpTexture(renderer, texture2D);
		}
	}

	private static void SetBumpTexture(Renderer renderer, Texture bumpTexture)
	{
		renderer.sharedMaterial.SetTexture(BumpMap, bumpTexture);
	}

	private static void LoadReflectiveTexture(Renderer renderer, string reflectivePath, bool async)
	{
		if (string.IsNullOrEmpty(reflectivePath))
		{
			return;
		}
		if (async)
		{
			LoadAndSetReflectiveTextureAsync(renderer, reflectivePath);
			return;
		}
		Texture2D texture2D = MilMo_ResourceManager.Instance.LoadTextureLocal(reflectivePath);
		if (!texture2D)
		{
			Debug.LogWarning("Got invalid water reflective color texture '" + reflectivePath + "'");
		}
		else
		{
			SetReflectiveTexture(renderer, texture2D);
		}
	}

	private static async void LoadAndSetReflectiveTextureAsync(Renderer renderer, string reflectivePath)
	{
		Texture2D texture2D = await MilMo_ResourceManager.Instance.LoadTextureAsync(reflectivePath);
		if (!texture2D)
		{
			Debug.LogWarning("Got invalid water reflective color texture '" + reflectivePath + "'");
		}
		else
		{
			SetReflectiveTexture(renderer, texture2D);
		}
	}

	private static void SetReflectiveTexture(Renderer renderer, Texture reflectiveTexture)
	{
		renderer.sharedMaterial.SetTexture(ReflectiveColor, reflectiveTexture);
	}

	private static void LoadAlphaTexture(Renderer renderer, MilMo_WaterSettings settings, bool async)
	{
		if (string.IsNullOrEmpty(settings.AlphaMaskPath))
		{
			return;
		}
		if (async)
		{
			LoadAndSetAlphaTextureAsync(renderer, settings);
			return;
		}
		Texture2D texture2D = MilMo_ResourceManager.Instance.LoadTextureLocal(settings.AlphaMaskPath);
		if (!texture2D)
		{
			Debug.LogWarning("Got invalid water alpha mask texture '" + settings.AlphaMaskPath + "'");
		}
		else
		{
			SetAlphaTexture(renderer, texture2D, settings.AlphaMaskScale, settings.AlphaMaskOffset);
		}
	}

	private static async void LoadAndSetAlphaTextureAsync(Renderer renderer, MilMo_WaterSettings settings)
	{
		Texture2D texture2D = await MilMo_ResourceManager.Instance.LoadTextureAsync(settings.AlphaMaskPath);
		if (!texture2D)
		{
			Debug.LogWarning("Got invalid water alpha mask texture '" + settings.AlphaMaskPath + "'");
		}
		else
		{
			SetAlphaTexture(renderer, texture2D, settings.AlphaMaskScale, settings.AlphaMaskOffset);
		}
	}

	private static void SetAlphaTexture(Renderer renderer, Texture mask, Vector2 scale, Vector2 offset)
	{
		renderer.sharedMaterial.SetTexture(AlphaMask, mask);
		renderer.sharedMaterial.SetTextureOffset(AlphaMask, offset);
		renderer.sharedMaterial.SetTextureScale(AlphaMask, scale);
	}

	public static void Unload(bool editorMode)
	{
		foreach (GameObject waterPlane in WaterPlanes)
		{
			if (!editorMode)
			{
				MilMo_WaterSunParticles componentInChildren = waterPlane.GetComponentInChildren<MilMo_WaterSunParticles>();
				if ((bool)componentInChildren)
				{
					componentInChildren.Destroy();
				}
			}
			MilMo_Global.Destroy(waterPlane, editorMode);
		}
		WaterPlanes.Clear();
		WaterVolumes.Clear();
	}
}
