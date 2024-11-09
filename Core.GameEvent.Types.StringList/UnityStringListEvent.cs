using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Core.GameEvent.Types.StringList;

[Serializable]
public class UnityStringListEvent : UnityEvent<IList<string>>
{
}
