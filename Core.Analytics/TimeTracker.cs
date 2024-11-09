using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Analytics;

public class TimeTracker : MonoBehaviour
{
	[SerializeField]
	private List<int> trackedMinutes = new List<int>();

	private DateTime _startTime;

	private int _trackedMinute;

	private const float UPDATE_INTERVAL_IN_SECONDS = 4f;

	private double _lastUpdate;

	private void Awake()
	{
		_startTime = DateTime.Now;
		trackedMinutes.AddRange(new List<int> { 2, 5, 10, 15, 20, 30, 60 });
	}

	private void Update()
	{
		if (!((double)Time.realtimeSinceStartup < _lastUpdate + 4.0))
		{
			Tick();
			_lastUpdate = Time.realtimeSinceStartup;
		}
	}

	private void Tick()
	{
		int minutes = (DateTime.Now - _startTime).Minutes;
		if (_trackedMinute != minutes)
		{
			MinuteReached(minutes);
		}
	}

	private void MinuteReached(int minutesSinceStart)
	{
		_trackedMinute = minutesSinceStart;
		if (_trackedMinute > trackedMinutes.Last())
		{
			base.enabled = false;
		}
		else if (trackedMinutes.Contains(_trackedMinute))
		{
			Analytics.PlayedMinutes(_trackedMinute);
		}
	}
}
