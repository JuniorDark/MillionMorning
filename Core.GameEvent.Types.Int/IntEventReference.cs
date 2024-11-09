using Core.GameEvent.Types.Base;

namespace Core.GameEvent.Types.Int;

public class IntEventReference : EventReference<int, BaseGameEvent<int>>
{
	public IntEventReference(string addressableKey)
		: base(addressableKey)
	{
	}
}
