using System;
using System.Collections.Generic;
using Core;
using UnityEngine;

namespace UI.Notification;

public class NotificationManager : Singleton<NotificationManager>
{
	[SerializeField]
	private List<NotificationType> types;

	private readonly Dictionary<NotificationType, bool> _unlockedTypes = new Dictionary<NotificationType, bool>();

	private readonly Dictionary<NotificationType, List<object>> _queuedNotifications = new Dictionary<NotificationType, List<object>>();

	public event Action<NotificationType, object> OnNotificationReceived;

	public void Receive(NotificationType type, object data)
	{
		if (IsTypeLocked(type))
		{
			Debug.LogWarning($"NotificationType={type} locked. Queuing");
			AddToQueue(type, data);
		}
		else
		{
			this.OnNotificationReceived?.Invoke(type, data);
		}
	}

	private bool IsTypeLocked(NotificationType type)
	{
		return !_unlockedTypes.ContainsKey(type);
	}

	private void AddToQueue(NotificationType type, object data)
	{
		if (!_queuedNotifications.ContainsKey(type))
		{
			List<object> value = new List<object> { data };
			_queuedNotifications.Add(type, value);
		}
		else
		{
			List<object> list = _queuedNotifications[type];
			list.Add(data);
			_queuedNotifications[type] = list;
		}
	}

	public NotificationType GetNotificationTypeByName(string typeName)
	{
		return types.Find((NotificationType i) => i.name == typeName);
	}

	public void Unlock(NotificationType type)
	{
		_unlockedTypes.Add(type, value: true);
		if (!_queuedNotifications.ContainsKey(type))
		{
			return;
		}
		_queuedNotifications.TryGetValue(type, out var value);
		if (value == null)
		{
			return;
		}
		foreach (object item in value)
		{
			this.OnNotificationReceived?.Invoke(type, item);
		}
	}

	public void Lock(NotificationType type)
	{
		_unlockedTypes.Remove(type);
	}
}
