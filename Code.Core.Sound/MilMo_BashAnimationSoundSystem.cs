using System;
using System.Collections.Generic;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Sound;

public static class MilMo_BashAnimationSoundSystem
{
	private const string FILENAME = "BashAnimationSounds";

	private static readonly Dictionary<string, string> BashSounds = new Dictionary<string, string>();

	public static void Init()
	{
		MilMo_SFFile milMo_SFFile = MilMo_SimpleFormat.LoadLocal("BashAnimationSounds");
		if (milMo_SFFile == null)
		{
			Debug.LogWarning("Failed to load bash sound config file (BashAnimationSounds.txt)");
			return;
		}
		while (milMo_SFFile.NextRow())
		{
			string @string = milMo_SFFile.GetString();
			string string2 = milMo_SFFile.GetString();
			try
			{
				BashSounds.Add(@string, string2);
			}
			catch (ArgumentException)
			{
				Debug.LogWarning("Multiple occurrences of animation name " + @string + " in file BashAnimationSounds.txt. A bash animation can only have one associated sound.");
			}
		}
	}

	public static string GetSound(string animation)
	{
		if (!BashSounds.TryGetValue(animation, out var value))
		{
			return "";
		}
		return value;
	}
}
