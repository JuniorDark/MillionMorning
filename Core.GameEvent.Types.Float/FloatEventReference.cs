using Core.GameEvent.Types.Base;

namespace Core.GameEvent.Types.Float;

public class FloatEventReference : EventReference<float, BaseGameEvent<float>>
{
	public FloatEventReference(string addressableKey)
		: base(addressableKey)
	{
	}
}
