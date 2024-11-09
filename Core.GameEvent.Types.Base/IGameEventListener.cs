namespace Core.GameEvent.Types.Base;

public interface IGameEventListener<T>
{
	void OnEventRaised(T item);
}
