using System.Collections.Generic;
using Core.GameEvent.Types.Base;
using UnityEngine;

namespace Core.GameEvent.Types.StringList;

[CreateAssetMenu(menuName = "GameEvents/StringList Event")]
public class StringListEvent : BaseGameEvent<IList<string>>
{
}
