using UI.Tooltip.Data;
using UnityEngine;

namespace UI.Tooltip.Triggers;

public class ConverterTooltipTrigger : TooltipTrigger
{
	[SerializeField]
	private ConverterTooltipData data;

	protected override TooltipData GetData()
	{
		return data;
	}
}
