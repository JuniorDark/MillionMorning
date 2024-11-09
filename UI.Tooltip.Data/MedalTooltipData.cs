using System;
using UnityEngine;

namespace UI.Tooltip.Data;

[Serializable]
public class MedalTooltipData : ItemTooltipData
{
	[SerializeField]
	private int target;

	[SerializeField]
	private int current;

	public MedalTooltipData(string title, string description, Texture2D icon, int target, int current)
		: base(title, description, icon)
	{
		this.target = target;
		this.current = current;
	}

	public float GetProgress()
	{
		return (float)current * 1f / (float)target * 1f;
	}

	public string GetProgressText()
	{
		return $"{current}/{target}";
	}
}
