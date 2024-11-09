using System.Collections.Generic;
using Code.Core.Config;
using UnityEngine;

namespace Code.Core.GUI.Core;

public static class MilMo_GUISkins
{
	private static readonly bool DevMode = MilMo_Config.Instance.IsTrue("Debug.GUI", defaultValue: false);

	private static readonly string[] Skins = new string[14]
	{
		"GuiSkins/Chat", "GuiSkins/Default", "GuiSkins/Homes", "GuiSkins/Junebug", "GuiSkins/JunebugDarker", "GuiSkins/FontArialRounded", "GuiSkins/FontArialRoundedMedium", "GuiSkins/FontEborgSmall", "GuiSkins/FontEborgMedium", "GuiSkins/FontEborgLarge",
		"GuiSkins/FontEborgXL", "GuiSkins/FontGothamSmall", "GuiSkins/FontGothamMedium", "GuiSkins/FontGothamLarge"
	};

	private static readonly Dictionary<string, GUISkin> GuiSkins = new Dictionary<string, GUISkin>();

	public static void AsyncLoadGUISkins()
	{
		if (DevMode)
		{
			Debug.Log("MilMo_GUISkins: GUI skins loading");
		}
		string[] skins = Skins;
		foreach (string text in skins)
		{
			GUISkin gUISkin = Resources.Load<GUISkin>(text);
			if (gUISkin == null)
			{
				Debug.LogWarning("Failed to load gui skin " + text);
			}
			else
			{
				GuiSkins.Add(gUISkin.name, gUISkin);
			}
		}
		if (DevMode)
		{
			Debug.Log("MilMo_GUISkins: GUI skins ready (" + GuiSkins.Count + "/" + Skins.Length + ")");
		}
	}

	public static GUISkin GetSkin(string name)
	{
		if (!GuiSkins.TryGetValue(name, out var value))
		{
			return null;
		}
		return value;
	}
}
