using System;
using UnityEngine;

namespace UI.Tooltip.Data;

[Serializable]
public class WeaponTooltipData : ItemTooltipData
{
	[SerializeField]
	private StatData[] stats;

	public StatData[] GetStats()
	{
		return stats;
	}

	public WeaponTooltipData(string title, string description, Texture2D icon, StatData[] stats)
		: base(title, description, icon)
	{
		this.stats = stats;
	}
}
