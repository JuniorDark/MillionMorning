using Core.GameEvent.Types.Base;

namespace Core.GameEvent.Types.String;

public class StringEventReference : EventReference<string, BaseGameEvent<string>>
{
	public StringEventReference(string addressableKey)
		: base(addressableKey)
	{
	}
}
