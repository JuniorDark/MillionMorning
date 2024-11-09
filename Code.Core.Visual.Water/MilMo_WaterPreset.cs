using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Visual.Water;

public class MilMo_WaterPreset
{
	public Color MainColor = Color.white;

	public float WaveScale = 0.063f;

	public float WaveSpeed = 0.2f;

	public float WaveStrength;

	public float ReflectionDistort = 0.14f;

	public string ReflectivePath;

	public float RefractionDistort = 1f;

	public Color RefractionColor = new Color(0.34f, 0.85f, 0.92f, 1f);

	public string BumpPath;

	public float MinAlpha = 0.5f;

	public bool DisablePixelLights;

	public bool EnableSunSparkles;

	public float ParticleSize;

	public float SunTrailWidthOffset;

	public bool EnableAmbientSparkles;

	public float AmbientLength;

	public int NumAmbientParticles;

	public static MilMo_WaterPreset Load(MilMo_SFFile file)
	{
		MilMo_WaterPreset milMo_WaterPreset = new MilMo_WaterPreset();
		while (file.NextRow())
		{
			if (file.IsNext("MainColor"))
			{
				milMo_WaterPreset.MainColor = file.GetColor();
			}
			else if (file.IsNext("Bumpmap"))
			{
				milMo_WaterPreset.BumpPath = file.GetString();
			}
			else if (file.IsNext("ReflectiveColor"))
			{
				milMo_WaterPreset.ReflectivePath = file.GetString();
			}
			else if (file.IsNext("WaveScale"))
			{
				milMo_WaterPreset.WaveScale = file.GetFloat();
			}
			else if (file.IsNext("WaveSpeed"))
			{
				milMo_WaterPreset.WaveSpeed = file.GetFloat();
			}
			else if (file.IsNext("WaveStrength"))
			{
				milMo_WaterPreset.WaveStrength = file.GetFloat();
			}
			else if (file.IsNext("DisablePixelLight"))
			{
				milMo_WaterPreset.DisablePixelLights = true;
			}
			else if (file.IsNext("ReflectionDistort"))
			{
				milMo_WaterPreset.ReflectionDistort = file.GetFloat();
			}
			else if (file.IsNext("RefractionDistort"))
			{
				milMo_WaterPreset.RefractionDistort = file.GetFloat();
			}
			else if (file.IsNext("SunSparkle"))
			{
				milMo_WaterPreset.EnableSunSparkles = true;
			}
			else if (file.IsNext("ParticleSize"))
			{
				milMo_WaterPreset.ParticleSize = file.GetFloat();
			}
			else if (file.IsNext("SunTrailWidthOffset"))
			{
				milMo_WaterPreset.SunTrailWidthOffset = file.GetFloat();
			}
			else if (file.IsNext("AmbientSparkle"))
			{
				milMo_WaterPreset.EnableAmbientSparkles = true;
			}
			else if (file.IsNext("AmbientSparkleLength"))
			{
				milMo_WaterPreset.AmbientLength = file.GetFloat();
			}
			else if (file.IsNext("AmbientSparkleNumberOfParticles"))
			{
				milMo_WaterPreset.NumAmbientParticles = file.GetInt();
			}
			else if (file.IsNext("RefractionColor"))
			{
				milMo_WaterPreset.RefractionColor = file.GetColor();
			}
			else if (file.IsNext("MinAlpha"))
			{
				milMo_WaterPreset.MinAlpha = file.GetFloat();
			}
		}
		return milMo_WaterPreset;
	}
}
