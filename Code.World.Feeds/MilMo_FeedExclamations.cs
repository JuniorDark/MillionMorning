using System.Collections.Generic;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.World.Feeds;

public static class MilMo_FeedExclamations
{
	private class Exclamation : MilMo_Randomizer.IRandomElement
	{
		public MilMo_LocString ExclamationString;

		public float Probability { get; }

		public float NormalizedProbability { get; set; }

		public Exclamation()
		{
			Probability = 1f;
		}
	}

	private static readonly List<MilMo_Randomizer.IRandomElement> Exclamations = new List<MilMo_Randomizer.IRandomElement>();

	private static MilMo_Randomizer _theExclamationRandomizer;

	public static void Load()
	{
		MilMo_SFFile milMo_SFFile = MilMo_SimpleFormat.LoadLocal("FeedScript/Exclamations");
		if (milMo_SFFile == null)
		{
			Debug.LogWarning("Failed to load feed exclamations file (file \"FeedScript/Exclamations.txt\" not found in a resources folder).");
			return;
		}
		while (milMo_SFFile.NextRow())
		{
			Exclamation item = new Exclamation
			{
				ExclamationString = MilMo_Localization.GetLocString(milMo_SFFile.GetString())
			};
			Exclamations.Add(item);
		}
		_theExclamationRandomizer = new MilMo_Randomizer(Exclamations);
	}

	public static MilMo_LocString GetExclamation()
	{
		if (_theExclamationRandomizer?.Next() is Exclamation exclamation)
		{
			return exclamation.ExclamationString;
		}
		return MilMo_LocString.Empty;
	}
}
