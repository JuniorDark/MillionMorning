using System;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Visual.Water;

public class MilMo_WaterSettings
{
	public string WaterContentBasePath = "";

	public Vector3 WaterPosition = new Vector3(0f, 0f, 0f);

	public Vector2 WaterScale = new Vector2(1000f, 1000f);

	public float WaterScaleHeight;

	public Vector3 WaterRotation = new Vector3(0f, 0f, 0f);

	public string WaterMesh = "Water";

	public string AlphaMaskPath = "";

	public Vector2 AlphaMaskScale = new Vector2(1f, 1f);

	public Vector2 AlphaMaskOffset = new Vector2(0f, 0f);

	public string WaterPreset;

	public static MilMo_WaterSettings Load(MilMo_SFFile file)
	{
		if (!file.HasMoreTokens())
		{
			Debug.LogWarning("Water without valid preset at line " + file.GetLineNumber());
			return null;
		}
		MilMo_WaterSettings milMo_WaterSettings = new MilMo_WaterSettings();
		milMo_WaterSettings.WaterPreset = file.GetString();
		while (file.NextRow())
		{
			string @string = file.GetString();
			if (!@string.StartsWith("Water", StringComparison.InvariantCulture) || @string.Equals("Water"))
			{
				break;
			}
			switch (@string)
			{
			case "WaterPosition":
				milMo_WaterSettings.WaterPosition = file.GetVector3();
				break;
			case "WaterScale":
				milMo_WaterSettings.WaterScale = file.GetVector2();
				break;
			case "WaterScaleHeight":
				milMo_WaterSettings.WaterScaleHeight = file.GetFloat();
				break;
			case "WaterRotation":
				milMo_WaterSettings.WaterRotation = file.GetVector3();
				break;
			case "WaterMesh":
				milMo_WaterSettings.WaterMesh = file.GetString();
				break;
			case "WaterAlphaMask":
				milMo_WaterSettings.AlphaMaskPath = file.GetString();
				if (file.HasMoreTokens())
				{
					milMo_WaterSettings.AlphaMaskScale = file.GetVector2();
				}
				if (file.HasMoreTokens())
				{
					milMo_WaterSettings.AlphaMaskOffset = file.GetVector2();
				}
				break;
			}
		}
		file.PrevRow();
		return milMo_WaterSettings;
	}
}
