using System.Threading.Tasks;
using UI.Elements.Slot;
using UnityEngine;

namespace UI.Tooltip.Data.GenerateTooltipData;

public class GenerateTooltipData
{
	protected readonly IEntryItem Item;

	protected readonly Texture2D Icon;

	public GenerateTooltipData(IEntryItem item, Texture2D icon)
	{
		Item = item;
		Icon = icon;
	}

	public virtual async Task<TooltipData> CreateTooltip()
	{
		if (Item == null || Icon == null)
		{
			Debug.LogWarning("Unable to generate tooltip data");
			return null;
		}
		return await Task.Run(delegate
		{
			string title = Item.GetDisplayName()?.String;
			string description = Item.GetDescription()?.String;
			return new ItemTooltipData(title, description, Icon);
		});
	}
}
