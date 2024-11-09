using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace Core.GameEvent.Types.Void;

public class VoidEventReference : EventReference<VoidType, VoidEvent>
{
	private readonly List<KeyValuePair<UnityAction, UnityAction<VoidType>>> _actions = new List<KeyValuePair<UnityAction, UnityAction<VoidType>>>();

	public VoidEventReference(string addressableKey)
		: base(addressableKey)
	{
	}

	public void RaiseEvent()
	{
		RaiseEvent(default(VoidType));
	}

	public void RegisterAction(UnityAction action)
	{
		if (!_actions.Any((KeyValuePair<UnityAction, UnityAction<VoidType>> a) => a.Key == action))
		{
			KeyValuePair<UnityAction, UnityAction<VoidType>> item = new KeyValuePair<UnityAction, UnityAction<VoidType>>(action, delegate
			{
				action();
			});
			RegisterAction(item.Value);
			_actions.Add(item);
		}
	}

	public void UnregisterAction(UnityAction action)
	{
		KeyValuePair<UnityAction, UnityAction<VoidType>> item = _actions.First((KeyValuePair<UnityAction, UnityAction<VoidType>> a) => a.Key == action);
		if (item.Key != null)
		{
			UnregisterAction(item.Value);
			_actions.Remove(item);
		}
	}
}
