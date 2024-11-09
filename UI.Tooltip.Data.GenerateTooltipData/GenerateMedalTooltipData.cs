using System.Threading.Tasks;
using Code.World.Achievements;
using UI.Elements.Slot;
using UnityEngine;

namespace UI.Tooltip.Data.GenerateTooltipData;

public class GenerateMedalTooltipData : GenerateTooltipData
{
	public GenerateMedalTooltipData(IEntryItem item, Texture2D icon)
		: base(item, icon)
	{
	}

	public override async Task<TooltipData> CreateTooltip()
	{
		return await Task.Run(delegate
		{
			if (!(Item is MilMo_Medal milMo_Medal))
			{
				return (MedalTooltipData)null;
			}
			if (milMo_Medal.Template == null)
			{
				return (MedalTooltipData)null;
			}
			string title = milMo_Medal.Template.DisplayName?.String;
			string description = ((!milMo_Medal.Acquired) ? milMo_Medal.Template.NotCompleteDescription?.String : milMo_Medal.GetDescription()?.String);
			int current = 1;
			int target = 1;
			if (!milMo_Medal.Acquired)
			{
				milMo_Medal.CalculateProgress(out target, out current);
			}
			return new MedalTooltipData(title, description, Icon, target, current);
		});
	}
}
