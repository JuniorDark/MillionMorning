using System;
using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.Utility;
using Core;

namespace Code.World.Player;

public class MilMo_LastActionTimeManager
{
	private delegate void ActionTimeUpdatedCallback(DateTime newTime);

	private static MilMo_LastActionTimeManager _instance;

	private readonly Dictionary<string, DateTime> _lastActionTimes = new Dictionary<string, DateTime>();

	private readonly Dictionary<string, List<ActionTimeUpdatedCallback>> _actionTimeUpdatedCallbacks = new Dictionary<string, List<ActionTimeUpdatedCallback>>();

	public static MilMo_LastActionTimeManager Instance => _instance ?? (_instance = new MilMo_LastActionTimeManager());

	private MilMo_LastActionTimeManager()
	{
	}

	public void Read(IEnumerable<ActionTime> actionTimes)
	{
		foreach (ActionTime actionTime in actionTimes)
		{
			_lastActionTimes.Add(actionTime.GetAction(), MilMo_Utility.GetDateTimeFromMilliseconds(actionTime.GetTime()));
		}
		MilMo_EventSystem.Listen("update_action_time", ActionTimeUpdated);
	}

	public void AsyncUpdateActionTime(string action)
	{
		AsyncUpdateActionTime(action, null);
	}

	private void AsyncUpdateActionTime(string action, ActionTimeUpdatedCallback callback)
	{
		if (callback != null)
		{
			if (!_actionTimeUpdatedCallbacks.TryGetValue(action, out var value))
			{
				value = new List<ActionTimeUpdatedCallback>();
				_actionTimeUpdatedCallbacks.Add(action, value);
			}
			value.Add(callback);
		}
		Singleton<GameNetwork>.Instance.SendActionTimeUpdate(action);
	}

	private void ActionTimeUpdated(object msgAsObj)
	{
		if (!(msgAsObj is ServerLastActionTimeUpdated serverLastActionTimeUpdated))
		{
			return;
		}
		DateTime dateTimeFromMilliseconds = MilMo_Utility.GetDateTimeFromMilliseconds(serverLastActionTimeUpdated.getActionTime().GetTime());
		_lastActionTimes[serverLastActionTimeUpdated.getActionTime().GetAction()] = dateTimeFromMilliseconds;
		if (!_actionTimeUpdatedCallbacks.TryGetValue(serverLastActionTimeUpdated.getActionTime().GetAction(), out var value))
		{
			return;
		}
		foreach (ActionTimeUpdatedCallback item in value)
		{
			item(dateTimeFromMilliseconds);
		}
		value.Clear();
	}
}
