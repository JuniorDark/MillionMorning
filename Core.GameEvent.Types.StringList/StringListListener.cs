using System.Collections.Generic;
using Core.GameEvent.Types.Base;

namespace Core.GameEvent.Types.StringList;

public class StringListListener : BaseGameEventListener<IList<string>, StringListEvent, UnityStringListEvent>
{
}
