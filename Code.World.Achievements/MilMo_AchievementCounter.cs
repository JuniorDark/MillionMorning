using System.Collections.Generic;
using UnityEngine;

namespace Code.World.Achievements;

public class MilMo_AchievementCounter
{
	private readonly string _type;

	private int _count;

	private readonly List<MilMo_AchievementObjectiveListener> _listeners = new List<MilMo_AchievementObjectiveListener>();

	public int Count
	{
		get
		{
			return _count;
		}
		set
		{
			_count = value;
			foreach (MilMo_AchievementObjectiveListener listener in _listeners)
			{
				listener.Notify();
			}
		}
	}

	public string Object { get; private set; }

	public MilMo_AchievementCounter(string type, string obj, int count)
	{
		_type = type;
		Object = obj;
		_count = count;
	}

	public bool Meets(MilMo_AchievementObjective objective)
	{
		if (_count >= objective.Count && Object == objective.Object)
		{
			return _type == objective.Type;
		}
		return false;
	}

	public void AddListener(MilMo_AchievementObjectiveListener listener)
	{
		_listeners.Add(listener);
	}

	public void RemoveListener(MilMo_AchievementObjectiveListener listener)
	{
		if (!_listeners.Remove(listener))
		{
			Debug.LogWarning("Failed to remove listener from counter " + _type + " " + Object + " not found.");
		}
	}
}
