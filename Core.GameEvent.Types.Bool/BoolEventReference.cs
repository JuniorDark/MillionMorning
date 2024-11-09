using Core.GameEvent.Types.Base;

namespace Core.GameEvent.Types.Bool;

public class BoolEventReference : EventReference<bool, BaseGameEvent<bool>>
{
	public BoolEventReference(string addressableKey)
		: base(addressableKey)
	{
	}
}
