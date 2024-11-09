using Core;
using TMPro;
using UnityEngine;

namespace UI.Notification;

public class AmountNotificationListener : MonoBehaviour
{
	[SerializeField]
	private GameObject element;

	[SerializeField]
	private string notificationType;

	[SerializeField]
	private TMP_Text amountText;

	private NotificationManager _manager;

	private NotificationType _notificationType;

	private int _count;

	private void Start()
	{
		_manager = Singleton<NotificationManager>.Instance;
		_count = 0;
		_manager.OnNotificationReceived += NotificationReceived;
		_notificationType = _manager.GetNotificationTypeByName(notificationType);
		if (_notificationType == null)
		{
			Debug.LogWarning("Unable to get notification type: " + notificationType);
		}
		else
		{
			_manager.Unlock(_notificationType);
		}
	}

	private void OnDestroy()
	{
		if (!(_manager == null) && !(_notificationType == null))
		{
			_manager.Lock(_notificationType);
			_manager.OnNotificationReceived -= NotificationReceived;
		}
	}

	private void NotificationReceived(NotificationType type, object notificationData)
	{
		if (!(type != _notificationType) && notificationData is int change)
		{
			UpdateElement(change);
		}
	}

	private void UpdateText()
	{
		amountText.text = _count.ToString();
	}

	private void UpdateCount(int change)
	{
		_count += change;
		if (_count <= 0)
		{
			_count = 0;
		}
	}

	private void UpdateElement(int change)
	{
		UpdateCount(change);
		UpdateText();
		element.SetActive(_count > 0);
	}
}
