using System;
using UnityEngine;

namespace UI.Tooltip.Data;

[Serializable]
public class ConverterTooltipData : ItemTooltipData
{
	[SerializeField]
	private ItemIngredientData[] components;

	[SerializeField]
	private ItemTooltipData result;

	[SerializeField]
	private ItemRequiredToolData requiredTool;

	public ItemRequiredToolData GetRequiredTool()
	{
		return requiredTool;
	}

	public ItemIngredientData[] GetItemComponentData()
	{
		return components;
	}

	public ItemTooltipData GetResult()
	{
		return result;
	}

	public ConverterTooltipData(string title, string description, Texture2D icon, ItemRequiredToolData requiredTool, ItemIngredientData[] components, ItemTooltipData result)
		: base(title, description, icon)
	{
		this.requiredTool = requiredTool;
		this.components = components;
		this.result = result;
	}
}
