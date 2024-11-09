using System;
using System.Collections.Generic;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.BodyPack.ColorSystem;

public static class MilMo_ColorSystem
{
	private static Dictionary<string, MilMo_Color> _skinColors = new Dictionary<string, MilMo_Color>(StringComparer.InvariantCultureIgnoreCase);

	private static Dictionary<string, MilMo_Color> _hairColors = new Dictionary<string, MilMo_Color>(StringComparer.InvariantCultureIgnoreCase);

	private static Dictionary<string, MilMo_Color> _eyeColors = new Dictionary<string, MilMo_Color>(StringComparer.InvariantCultureIgnoreCase);

	public static bool Create()
	{
		return LoadAllColors();
	}

	private static bool LoadAllColors()
	{
		if (!LoadColors("Avatar/Colors/Skin", ref _skinColors))
		{
			Debug.LogWarning("Failed to load skin colors");
			return false;
		}
		if (!LoadColors("Avatar/Colors/Hair", ref _hairColors))
		{
			Debug.LogWarning("Failed to load hair colors");
			return false;
		}
		if (!LoadColors("Avatar/Colors/Eye", ref _eyeColors))
		{
			Debug.LogWarning("Failed to load eye colors");
			return false;
		}
		return true;
	}

	public static MilMo_Color GetSkinColor(string colorName)
	{
		return GetColor(colorName, ref _skinColors);
	}

	public static MilMo_Color GetEyeColor(string colorName)
	{
		return GetColor(colorName, ref _eyeColors);
	}

	private static MilMo_Color GetColor(string colorName, ref Dictionary<string, MilMo_Color> colors)
	{
		colors.TryGetValue(colorName, out var value);
		return value;
	}

	private static bool LoadColors(string colorPath, ref Dictionary<string, MilMo_Color> colors)
	{
		colors.Clear();
		MilMo_ResourceManager.Instance.LoadAllLocal(colorPath);
		IEnumerable<MilMo_SFFile> enumerable = MilMo_SimpleFormat.LoadAllLocal(colorPath);
		int num = 0;
		foreach (MilMo_SFFile item in enumerable)
		{
			if (item == null)
			{
				Debug.LogWarning("Failed to load a color file in path " + colorPath + " at index " + num);
				continue;
			}
			num++;
			if (item.Name.Length > 16)
			{
				Debug.LogWarning("Color name is greater than 16 characters. Please shorten the name for color " + item.Path);
				continue;
			}
			MilMo_Color milMo_Color = new MilMo_Color();
			if (!milMo_Color.Read(item))
			{
				Debug.LogWarning("Failed to load color " + item.Path);
			}
			else if (colors.ContainsKey(item.Name))
			{
				Debug.LogWarning("Trying to add color with same name twice (" + item.Name + ")");
			}
			else
			{
				colors.Add(item.Name, milMo_Color);
			}
		}
		return true;
	}
}
