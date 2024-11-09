using UnityEngine;
using UnityEngine.Events;

namespace Core.GameEvent.Types.Base;

public abstract class BaseGameEventListener<T, E, UER> : MonoBehaviour, IGameEventListener<T> where E : BaseGameEvent<T> where UER : UnityEvent<T>
{
	[SerializeField]
	private E gameEvent;

	[SerializeField]
	private bool stayActivated;

	[SerializeField]
	private UER unityEventResponse;

	private void Awake()
	{
		if (!(gameEvent == null) && stayActivated)
		{
			gameEvent.RegisterListener(this);
		}
	}

	private void OnDestroy()
	{
		if (!(gameEvent == null) && stayActivated)
		{
			gameEvent.UnregisterListener(this);
		}
	}

	private void OnEnable()
	{
		if (!(gameEvent == null) && !stayActivated)
		{
			gameEvent.RegisterListener(this);
		}
	}

	private void OnDisable()
	{
		if (!(gameEvent == null) && !stayActivated)
		{
			gameEvent.UnregisterListener(this);
		}
	}

	public void OnEventRaised(T item)
	{
		unityEventResponse?.Invoke(item);
	}
}
