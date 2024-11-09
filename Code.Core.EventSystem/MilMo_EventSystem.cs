using System.Collections.Generic;

namespace Code.Core.EventSystem;

public class MilMo_EventSystem
{
	public delegate void MilMo_Callback();

	public delegate void MilMo_EventCallback(object data);

	private static MilMo_EventSystem _theEventSystem;

	private readonly Dictionary<string, List<MilMo_GenericReaction>> _reactions = new Dictionary<string, List<MilMo_GenericReaction>>();

	private static bool _enabled = true;

	private readonly List<MilMo_GenericReaction> _activeNextFrameCallbacks = new List<MilMo_GenericReaction>();

	private readonly List<MilMo_GenericReaction> _nextFrameCallbacks = new List<MilMo_GenericReaction>();

	private static readonly object NextFrameLock = new object();

	private MilMo_TimerEvent _timers;

	private static readonly object TimerQueueLock = new object();

	private readonly List<MilMo_GenericEvent> _asyncEventQueue = new List<MilMo_GenericEvent>();

	private static readonly object EventQueueLock = new object();

	private readonly List<MilMo_GenericReaction> _updateEvents = new List<MilMo_GenericReaction>();

	private readonly List<MilMo_GenericReaction> _fixedUpdateEvents = new List<MilMo_GenericReaction>();

	private readonly List<MilMo_GenericReaction> _fixedUpdateEventsToRemove = new List<MilMo_GenericReaction>();

	private readonly List<MilMo_GenericReaction> _lateUpdateEvents = new List<MilMo_GenericReaction>();

	private readonly List<MilMo_GenericReaction> _preRenderEvents = new List<MilMo_GenericReaction>();

	private readonly Queue<MilMo_GenericReaction> _updateOne = new Queue<MilMo_GenericReaction>();

	private MilMo_EventSystemRunner _runner;

	public static MilMo_EventSystem Instance
	{
		get
		{
			if (_theEventSystem == null)
			{
				Create();
			}
			return _theEventSystem;
		}
	}

	public static bool Enabled
	{
		set
		{
			_enabled = value;
		}
	}

	private MilMo_EventSystem()
	{
	}

	private static void Create()
	{
		_theEventSystem = new MilMo_EventSystem();
	}

	public void FixedUpdate()
	{
		if (!_enabled)
		{
			return;
		}
		lock (TimerQueueLock)
		{
			MilMo_TimerEvent milMo_TimerEvent = _timers;
			for (int i = 0; i < 1000; i++)
			{
				if (milMo_TimerEvent == null)
				{
					break;
				}
				if (!milMo_TimerEvent.Test())
				{
					break;
				}
				milMo_TimerEvent = (_timers = milMo_TimerEvent.Next);
			}
		}
		lock (EventQueueLock)
		{
			for (int j = 0; j < _asyncEventQueue.Count; j++)
			{
				PostEvent(_asyncEventQueue[j].Event, _asyncEventQueue[j].Data);
			}
			_asyncEventQueue.Clear();
		}
		for (int num = _updateEvents.Count - 1; num >= 0; num--)
		{
			if (_updateEvents[num].HighPriority)
			{
				_updateEvents[num].Execute(null);
			}
		}
		for (int num2 = _fixedUpdateEvents.Count - 1; num2 >= 0; num2--)
		{
			if (_fixedUpdateEvents[num2].Active)
			{
				_fixedUpdateEvents[num2].Execute(null);
			}
		}
		if (_fixedUpdateEventsToRemove.Count > 0)
		{
			for (int k = 0; k < _fixedUpdateEventsToRemove.Count; k++)
			{
				_fixedUpdateEvents.Remove(_fixedUpdateEventsToRemove[k]);
			}
			_fixedUpdateEventsToRemove.Clear();
		}
	}

	public void Update()
	{
		if (!_enabled)
		{
			return;
		}
		lock (EventQueueLock)
		{
			for (int num = _updateEvents.Count - 1; num >= 0; num--)
			{
				_updateEvents[num].Execute(null);
			}
			if (_updateOne.Count > 0)
			{
				MilMo_GenericReaction milMo_GenericReaction = _updateOne.Dequeue();
				milMo_GenericReaction.Execute(milMo_GenericReaction.CustomData);
			}
		}
		lock (NextFrameLock)
		{
			for (int i = 0; i < _activeNextFrameCallbacks.Count; i++)
			{
				_activeNextFrameCallbacks[i].Execute(_activeNextFrameCallbacks[i].CustomData);
			}
			_activeNextFrameCallbacks.Clear();
			for (int j = 0; j < _nextFrameCallbacks.Count; j++)
			{
				_activeNextFrameCallbacks.Add(_nextFrameCallbacks[j]);
			}
			_nextFrameCallbacks.Clear();
		}
	}

	public void LateUpdate()
	{
		if (_enabled)
		{
			for (int i = 0; i < _lateUpdateEvents.Count; i++)
			{
				_lateUpdateEvents[i].Execute(null);
			}
		}
	}

	public void OnPreRender()
	{
		if (_enabled)
		{
			for (int i = 0; i < _preRenderEvents.Count; i++)
			{
				_preRenderEvents[i].Execute(null);
			}
		}
	}

	public void PostEvent(string eventName, object data)
	{
		if (!_enabled || !_reactions.TryGetValue(eventName, out var value))
		{
			return;
		}
		List<MilMo_GenericReaction> list = new List<MilMo_GenericReaction>();
		for (int i = 0; i < value.Count; i++)
		{
			MilMo_GenericReaction milMo_GenericReaction = value[i];
			milMo_GenericReaction.Execute(data);
			if (milMo_GenericReaction.Repeating)
			{
				list.Add(milMo_GenericReaction);
			}
		}
		value.Clear();
		value.AddRange(list);
	}

	public void AsyncPostEvent(string e, object arg = null)
	{
		if (!_enabled)
		{
			return;
		}
		lock (EventQueueLock)
		{
			_asyncEventQueue.Add(new MilMo_GenericEvent(e, arg));
		}
	}

	public static MilMo_GenericReaction Listen(string eventString, MilMo_EventCallback callback)
	{
		MilMo_GenericReaction milMo_GenericReaction = new MilMo_GenericReaction(eventString, callback);
		if (Instance._reactions.TryGetValue(eventString, out var value))
		{
			value.Add(milMo_GenericReaction);
		}
		else
		{
			value = new List<MilMo_GenericReaction> { milMo_GenericReaction };
			Instance._reactions.Add(eventString, value);
		}
		return milMo_GenericReaction;
	}

	public static void UpdateOne(MilMo_EventCallback callback)
	{
		MilMo_GenericReaction item = new MilMo_GenericReaction("", callback);
		Instance._updateOne.Enqueue(item);
	}

	public static MilMo_TimerEvent At(float delay, MilMo_Callback callback)
	{
		return At(delay, new MilMo_SimpleAction(callback));
	}

	public static MilMo_TimerEvent At(float aDelay, MilMo_GenericAction.Callback aCallback, object aArg)
	{
		return At(aDelay, new MilMo_GenericAction(aCallback, aArg));
	}

	private static MilMo_TimerEvent At(float aDelay, MilMo_EventAction aAction)
	{
		MilMo_TimerEvent milMo_TimerEvent = new MilMo_TimerEvent(aDelay, aAction);
		lock (TimerQueueLock)
		{
			if (Instance._timers == null)
			{
				Instance._timers = milMo_TimerEvent;
				return milMo_TimerEvent.Copy();
			}
			if (milMo_TimerEvent.TriggerTime < Instance._timers.TriggerTime)
			{
				milMo_TimerEvent.Next = Instance._timers;
				Instance._timers = milMo_TimerEvent;
				return milMo_TimerEvent.Copy();
			}
			MilMo_TimerEvent milMo_TimerEvent2 = Instance._timers;
			MilMo_TimerEvent milMo_TimerEvent3 = null;
			int num = 0;
			while (num < 1000 && milMo_TimerEvent2 != null)
			{
				if (milMo_TimerEvent2.TriggerTime > milMo_TimerEvent.TriggerTime)
				{
					milMo_TimerEvent.Next = milMo_TimerEvent2;
					if (milMo_TimerEvent3 != null)
					{
						milMo_TimerEvent3.Next = milMo_TimerEvent;
					}
					return milMo_TimerEvent.Copy();
				}
				milMo_TimerEvent3 = milMo_TimerEvent2;
				milMo_TimerEvent2 = milMo_TimerEvent2.Next;
				num++;
				num++;
			}
			if (milMo_TimerEvent3 != null)
			{
				milMo_TimerEvent3.Next = milMo_TimerEvent;
			}
		}
		return milMo_TimerEvent.Copy();
	}

	public static void NextFrame(MilMo_EventCallback callback)
	{
		lock (NextFrameLock)
		{
			Instance._nextFrameCallbacks.Add(new MilMo_GenericReaction("", callback));
		}
	}

	public static MilMo_GenericReaction RegisterUpdate(MilMo_EventCallback callback)
	{
		MilMo_GenericReaction milMo_GenericReaction = new MilMo_GenericReaction("", callback)
		{
			CustomData = null
		};
		Instance._updateEvents.Add(milMo_GenericReaction);
		return milMo_GenericReaction;
	}

	public static void UnregisterUpdate(MilMo_GenericReaction reaction)
	{
		if (reaction != null)
		{
			reaction.Active = false;
			Instance._updateEvents.Remove(reaction);
		}
	}

	public static MilMo_GenericReaction RegisterFixedUpdate(MilMo_EventCallback callback)
	{
		MilMo_GenericReaction milMo_GenericReaction = new MilMo_GenericReaction("", callback)
		{
			CustomData = null
		};
		Instance._fixedUpdateEvents.Add(milMo_GenericReaction);
		return milMo_GenericReaction;
	}

	public static void UnregisterFixedUpdate(MilMo_GenericReaction reaction)
	{
		if (reaction != null)
		{
			reaction.Active = false;
			Instance._fixedUpdateEventsToRemove.Add(reaction);
		}
	}

	public static MilMo_GenericReaction RegisterLateUpdate(MilMo_EventCallback callback)
	{
		MilMo_GenericReaction milMo_GenericReaction = new MilMo_GenericReaction("", callback)
		{
			CustomData = null
		};
		Instance._lateUpdateEvents.Add(milMo_GenericReaction);
		return milMo_GenericReaction;
	}

	public static MilMo_GenericReaction RegisterPreRender(MilMo_EventCallback callback)
	{
		MilMo_GenericReaction milMo_GenericReaction = new MilMo_GenericReaction("", callback)
		{
			CustomData = null
		};
		Instance._preRenderEvents.Add(milMo_GenericReaction);
		return milMo_GenericReaction;
	}

	public static void UnregisterPreRender(MilMo_GenericReaction reaction)
	{
		if (reaction != null)
		{
			Instance._preRenderEvents.Remove(reaction);
		}
	}

	public static void UnregisterLateUpdate(MilMo_GenericReaction reaction)
	{
		if (reaction != null)
		{
			reaction.Active = false;
			Instance._lateUpdateEvents.Remove(reaction);
		}
	}

	public static void AddReaction(MilMo_GenericReaction reaction)
	{
		if (reaction != null && Instance._reactions.TryGetValue(reaction.Event, out var value))
		{
			value.Add(reaction);
		}
	}

	public static void RemoveReaction(MilMo_GenericReaction reaction)
	{
		if (reaction != null && Instance._reactions.TryGetValue(reaction.Event, out var value))
		{
			value.Remove(reaction);
		}
	}

	public static void RemoveTimerEvent(MilMo_TimerEvent timerEvent)
	{
		if (timerEvent == null)
		{
			return;
		}
		lock (TimerQueueLock)
		{
			if (timerEvent.Equals(Instance._timers))
			{
				Instance._timers = Instance._timers.Next;
				return;
			}
			MilMo_TimerEvent milMo_TimerEvent = Instance._timers;
			MilMo_TimerEvent milMo_TimerEvent2 = null;
			int num = 0;
			while (num < 1000 && milMo_TimerEvent != null)
			{
				if (milMo_TimerEvent.Equals(timerEvent))
				{
					if (milMo_TimerEvent2 != null)
					{
						milMo_TimerEvent2.Next = milMo_TimerEvent.Next;
					}
					milMo_TimerEvent.Next = null;
					break;
				}
				milMo_TimerEvent2 = milMo_TimerEvent;
				milMo_TimerEvent = milMo_TimerEvent.Next;
				num++;
				num++;
			}
		}
	}

	public bool SetRunner(MilMo_EventSystemRunner runner)
	{
		if (_runner != null)
		{
			return false;
		}
		_runner = runner;
		return true;
	}
}
