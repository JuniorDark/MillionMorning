using System;
using UI.Tooltip.Data;
using UnityEngine.Events;

namespace Core.GameEvent.Types.Tooltip;

[Serializable]
public class UnityTooltipEvent : UnityEvent<TooltipData>
{
}
