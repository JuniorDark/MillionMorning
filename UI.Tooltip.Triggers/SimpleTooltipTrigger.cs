using UI.Tooltip.Data;
using UnityEngine.Localization;

namespace UI.Tooltip.Triggers;

public class SimpleTooltipTrigger : TooltipTrigger
{
	public LocalizedString localizedString;

	protected override TooltipData GetData()
	{
		return new TooltipData(localizedString?.GetLocalizedString());
	}
}
