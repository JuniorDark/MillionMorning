using System.Collections.Generic;
using Core.GameEvent.Types.Base;

namespace Core.GameEvent.Types.StringList;

public class StringListEventReference : EventReference<IList<string>, BaseGameEvent<IList<string>>>
{
	public StringListEventReference(string addressableKey)
		: base(addressableKey)
	{
	}
}
