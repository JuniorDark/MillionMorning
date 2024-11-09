using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Player.Moods;

[CreateAssetMenu(menuName = "Player/Create Mood Configuration", fileName = "Mood", order = 0)]
public class MoodConfiguration : ScriptableObject
{
	[SerializeField]
	private List<Mood> moods;

	private int _index;

	public List<Mood> GetMoods()
	{
		return moods;
	}

	public Mood GetMoodByKey(string key)
	{
		return moods.FirstOrDefault((Mood m) => m.GetKey().Equals(key, StringComparison.CurrentCultureIgnoreCase));
	}

	private int GetMoodIndex(Mood mood)
	{
		if (mood == null)
		{
			return 0;
		}
		for (int i = 0; i < moods.Count; i++)
		{
			if (moods[i].GetKey() == mood.GetKey())
			{
				return i;
			}
		}
		return 0;
	}

	public Mood GetNextMood(Mood mood)
	{
		int num = GetMoodIndex(mood) + 1;
		if (num >= moods.Count)
		{
			num = 0;
		}
		return moods[num];
	}

	public Mood GetPreviousMood(Mood mood)
	{
		int num = GetMoodIndex(mood) - 1;
		if (num < 0)
		{
			num = moods.Count - 1;
		}
		return moods[num];
	}
}
