using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.GameEvent.Types.Base;

public abstract class BaseGameEvent<T> : ScriptableObject
{
	private readonly List<IGameEventListener<T>> _eventListeners = new List<IGameEventListener<T>>();

	public void Raise(T item)
	{
		try
		{
			for (int num = _eventListeners.Count - 1; num >= 0; num--)
			{
				_eventListeners[num].OnEventRaised(item);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.StackTrace);
		}
	}

	public void RegisterListener(IGameEventListener<T> listener)
	{
		if (!_eventListeners.Contains(listener))
		{
			_eventListeners.Add(listener);
		}
	}

	public void UnregisterListener(IGameEventListener<T> listener)
	{
		if (_eventListeners.Contains(listener))
		{
			_eventListeners.Remove(listener);
		}
	}
}
