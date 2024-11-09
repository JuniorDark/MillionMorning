using System;
using Core.GameEvent.Types.Base;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace Core.GameEvent;

public abstract class EventReference<T, TE> where TE : BaseGameEvent<T>
{
	private readonly string _addressableKey;

	private BaseGameEvent<T> _event;

	private readonly BaseActionListener<T> _listener = new BaseActionListener<T>();

	protected EventReference(string addressableKey)
	{
		_addressableKey = addressableKey;
		LoadEvent();
		_event.RegisterListener(_listener);
	}

	protected void LoadEvent()
	{
		try
		{
			_event = Addressables.LoadAssetAsync<TE>(_addressableKey).WaitForCompletion();
		}
		catch (Exception)
		{
			Debug.LogWarning("Could not get the addressable event: " + _addressableKey);
			throw;
		}
	}

	public void RaiseEvent(T args)
	{
		try
		{
			_event.Raise(args);
		}
		catch (Exception)
		{
			Debug.LogWarning("Failed to raise event: " + _addressableKey);
		}
	}

	public void RegisterAction(UnityAction<T> action)
	{
		try
		{
			_listener.RegisterAction(action);
		}
		catch (Exception)
		{
			Debug.LogWarning("Failed to register action on event: " + _addressableKey);
		}
	}

	public void UnregisterAction(UnityAction<T> action)
	{
		try
		{
			_listener.UnregisterAction(action);
		}
		catch (Exception)
		{
			Debug.LogWarning("Failed to unregister action on event: " + _addressableKey);
		}
	}

	public void RemoveAllActions()
	{
		try
		{
			_listener.Clear();
		}
		catch (Exception)
		{
			Debug.LogWarning("Failed to clear actions from event: " + _addressableKey);
		}
	}
}
