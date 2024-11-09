using TMPro;
using UI.Tooltip.Data;
using UnityEngine;

namespace UI.Tooltip;

public class SimpleTooltip : Tooltip
{
	[Header("Elements")]
	[SerializeField]
	private TMP_Text title;

	public override void SetData(TooltipData data)
	{
		title.text = data.GetTitle();
	}

	public override void Show()
	{
		if ((bool)title && !string.IsNullOrEmpty(title.text))
		{
			base.Show();
		}
	}
}
