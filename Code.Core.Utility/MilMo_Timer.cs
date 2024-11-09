using System.Collections.Generic;
using UnityEngine;

namespace Code.Core.Utility;

public static class MilMo_Timer
{
	private static readonly Dictionary<string, float> MappedWatches = new Dictionary<string, float>();

	public static void StartUnique(string userIdentifier)
	{
		StartUnique(userIdentifier, resetIfExists: true);
	}

	private static void StartUnique(string userIdentifier, bool resetIfExists)
	{
		if (!MappedWatches.ContainsKey(userIdentifier))
		{
			MappedWatches.Add(userIdentifier, MilMo_Time.GetCurrentTime());
		}
		else if (resetIfExists)
		{
			Debug.LogWarning("Trying to create multiple instances of unique timer '" + userIdentifier + "'. Timer will be reset instead.");
			MappedWatches[userIdentifier] = MilMo_Time.GetCurrentTime();
		}
		else
		{
			Debug.LogWarning("Trying to create multiple instances of unique timer '" + userIdentifier + "'.");
		}
	}

	public static void StopUnique(string identifier)
	{
		StopUnique(identifier, identifier);
	}

	public static void StopUnique(string identifier, string message)
	{
		if (MappedWatches.TryGetValue(identifier, out var value))
		{
			Debug.Log(message + "@" + (MilMo_Time.GetCurrentTime() - value).ToString("F4") + "s");
			MappedWatches.Remove(identifier);
		}
	}
}
