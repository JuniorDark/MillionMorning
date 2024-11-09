using System;
using UnityEngine;

namespace UI.Tooltip.Data;

[Serializable]
public class ItemTooltipData : TooltipData
{
	[SerializeField]
	private string description;

	[SerializeField]
	private Texture2D icon;

	public string GetDescription()
	{
		return description;
	}

	public Texture2D GetIcon()
	{
		return icon;
	}

	public ItemTooltipData(string title, string description, Texture2D icon)
		: base(title)
	{
		this.description = description;
		this.icon = icon;
	}

	public ItemTooltipData(string title, string description, Sprite icon)
		: base(title)
	{
		this.description = description;
		this.icon = icon.texture;
	}
}
