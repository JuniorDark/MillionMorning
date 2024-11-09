using UI.Tooltip.Data;
using UnityEngine;

namespace UI.Tooltip.Triggers;

public class ItemTooltipTrigger : TooltipTrigger
{
	[SerializeField]
	private ItemTooltipData data;

	protected override TooltipData GetData()
	{
		return data;
	}
}
