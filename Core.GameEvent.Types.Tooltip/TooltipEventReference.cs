using Core.GameEvent.Types.Base;
using UI.Tooltip.Data;

namespace Core.GameEvent.Types.Tooltip;

public class TooltipEventReference : EventReference<TooltipData, BaseGameEvent<TooltipData>>
{
	public TooltipEventReference(string addressableKey)
		: base(addressableKey)
	{
	}
}
