using Core.Utilities;
using TMPro;
using UI.Tooltip.Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tooltip;

public class ItemTooltip : Tooltip
{
	[Header("Elements")]
	[SerializeField]
	private TMP_Text title;

	[SerializeField]
	private TMP_Text description;

	[SerializeField]
	private Image icon;

	public override void Show()
	{
		if (!string.IsNullOrEmpty(title.text) && !string.IsNullOrEmpty(description.text))
		{
			base.Show();
		}
	}

	public override void SetData(TooltipData data)
	{
		title.text = data.GetTitle();
		ItemTooltipData itemTooltipData = (ItemTooltipData)data;
		description.text = itemTooltipData.GetDescription();
		Core.Utilities.UI.SetIcon(icon, itemTooltipData.GetIcon());
	}
}
