using System;
using UnityEngine;

namespace UI.Notification;

[CreateAssetMenu(menuName = "Notification/Create new", fileName = "new notification")]
public class NotificationType : ScriptableObject, IEquatable<NotificationType>
{
	public string type;

	public bool Equals(NotificationType other)
	{
		if ((object)other == null)
		{
			return false;
		}
		if ((object)this == other)
		{
			return true;
		}
		if (base.Equals((object)other))
		{
			return type == other.type;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((NotificationType)obj);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.GetHashCode(), type);
	}
}
