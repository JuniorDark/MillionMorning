using System;
using System.Collections.Generic;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Core;
using UnityEngine;

namespace Code.Core.Emote;

public static class MilMo_EmoteSystem
{
	public static readonly Dictionary<string, Vector2> UVPresets = new Dictionary<string, Vector2>(StringComparer.InvariantCultureIgnoreCase);

	private const string THE_FACES_PATH = "Faces";

	private const string THE_MOODS_PATH = "Moods";

	private const string THE_FACE_MOVERS_PATH = "FaceMovers";

	private static readonly Dictionary<string, Dictionary<string, string>> MoodAnimations = new Dictionary<string, Dictionary<string, string>>(StringComparer.InvariantCultureIgnoreCase);

	private static bool _created;

	public static bool Create()
	{
		if (_created)
		{
			return true;
		}
		if (!LoadConf())
		{
			Debug.LogWarning("Failed to load configuration file for emote system.");
			return false;
		}
		if (!LoadMoodConf())
		{
			Debug.LogWarning("Failed to load mood configuration file for emote system.");
			return false;
		}
		MilMo_FaceMover.LoadFaceMovers("FaceMovers");
		MilMo_Face.LoadFaces("Faces");
		MilMo_Face.LoadMoods("Moods");
		PreloadEmotes();
		_created = true;
		return true;
	}

	public static MilMo_Template EmoteCreator(string category, string path, string filePath)
	{
		return new MilMo_Emote(category, path, filePath);
	}

	private static bool LoadConf()
	{
		MilMo_SFFile milMo_SFFile = MilMo_SimpleFormat.LoadLocal("EmoteConf");
		if (milMo_SFFile == null)
		{
			return false;
		}
		UVPresets.Clear();
		while (milMo_SFFile.NextRow())
		{
			if (milMo_SFFile.GetString() == "UVPreset")
			{
				string @string = milMo_SFFile.GetString();
				float @float = milMo_SFFile.GetFloat();
				float float2 = milMo_SFFile.GetFloat();
				if (!UVPresets.ContainsKey(@string))
				{
					UVPresets.Add(@string, new Vector2(@float, float2));
				}
			}
		}
		return true;
	}

	private static bool LoadMoodConf()
	{
		MilMo_SFFile milMo_SFFile = MilMo_SimpleFormat.LoadLocal("MoodConf");
		if (milMo_SFFile == null)
		{
			return false;
		}
		MoodAnimations.Clear();
		while (milMo_SFFile.NextRow())
		{
			if (!milMo_SFFile.IsNext("<MOOD>"))
			{
				continue;
			}
			milMo_SFFile.NextRow();
			if (!milMo_SFFile.IsNext("</MOOD>"))
			{
				string @string = milMo_SFFile.GetString();
				Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
				MoodAnimations.Add(@string, dictionary);
				while (milMo_SFFile.NextRow() && !milMo_SFFile.IsNext("</MOOD>"))
				{
					string string2 = milMo_SFFile.GetString();
					string string3 = milMo_SFFile.GetString();
					dictionary.Add(string2, string3);
				}
			}
		}
		return true;
	}

	private static void PreloadEmotes()
	{
		foreach (MilMo_SFFile item in MilMo_SimpleFormat.LoadAllLocal("Emotes"))
		{
			if (Singleton<MilMo_TemplateContainer>.Instance.GetTemplate("Emote", "Emotes." + item.Name) == null)
			{
				Debug.LogWarning("Failed to preload emote " + item.Name);
			}
		}
	}

	public static string GetMoodAnimation(string anim, string mood)
	{
		if (mood == null)
		{
			return anim;
		}
		if (!MoodAnimations.TryGetValue(mood, out var value))
		{
			return anim;
		}
		if (!value.TryGetValue(anim, out var value2))
		{
			return anim;
		}
		return value2;
	}

	public static MilMo_Emote GetEmoteByName(string name)
	{
		if (Singleton<MilMo_TemplateContainer>.Instance.GetTemplate("Emote", name) is MilMo_Emote emote)
		{
			return new MilMo_Emote(emote);
		}
		Debug.LogWarning("Trying to fetch non existing emote template " + name);
		return null;
	}
}
