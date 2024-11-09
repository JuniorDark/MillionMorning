using UI.Tooltip.Data;
using UnityEngine;

namespace UI.Tooltip.Triggers;

public class WeaponTooltipTrigger : TooltipTrigger
{
	[SerializeField]
	private WeaponTooltipData data;

	protected override TooltipData GetData()
	{
		return data;
	}
}
