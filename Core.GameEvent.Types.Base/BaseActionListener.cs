using System;
using UnityEngine.Events;

namespace Core.GameEvent.Types.Base;

public class BaseActionListener<T> : IGameEventListener<T>
{
	private UnityAction<T> _action;

	public void RegisterAction(UnityAction<T> action)
	{
		_action = (UnityAction<T>)Delegate.Combine(_action, action);
	}

	public void UnregisterAction(UnityAction<T> action)
	{
		_action = (UnityAction<T>)Delegate.Remove(_action, action);
	}

	public void OnEventRaised(T item)
	{
		_action?.Invoke(item);
	}

	public void Clear()
	{
		_action = null;
	}
}
