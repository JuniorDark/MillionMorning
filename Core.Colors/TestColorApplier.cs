using System.Collections.Generic;
using UnityEngine;

namespace Core.Colors;

public class TestColorApplier : MonoBehaviour
{
	[SerializeField]
	private ScriptableColor color;

	[SerializeField]
	private List<Material> targetMaterials;

	private void OnValidate()
	{
		Apply();
	}

	public void Apply()
	{
		foreach (Material targetMaterial in targetMaterials)
		{
			ResetColor(targetMaterial);
			UseColor(targetMaterial, color);
		}
	}

	private void ResetColor(Material material)
	{
		material.SetColor("_OverlayColor1", Color.clear);
		material.SetColor("_OverlayColor2", Color.clear);
		material.SetColor("_OverlayColor3", Color.clear);
		material.SetColor("_SoftlightColor1", Color.clear);
		material.SetColor("_SoftlightColor2", Color.clear);
		material.SetColor("_SoftlightColor3", Color.clear);
		material.SetFloat("_Saturation", 1f);
	}

	public void UseColor(Material material, ScriptableColor scriptableColor)
	{
		int num = 1;
		foreach (Color overlay in scriptableColor.GetOverlays())
		{
			material.SetColor("_OverlayColor" + num, overlay);
			num++;
		}
		foreach (Color softLight in scriptableColor.GetSoftLights())
		{
			material.SetColor("_SoftlightColor" + num, softLight);
			num++;
		}
		int saturation = scriptableColor.GetSaturation();
		if (saturation != 0)
		{
			material.SetFloat("_Saturation", (float)saturation / 100f);
		}
	}
}
