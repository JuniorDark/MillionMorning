using System;
using UnityEngine;

namespace UI.Tooltip.Data;

[Serializable]
public class ItemIngredientData
{
	[SerializeField]
	private Texture2D icon;

	[SerializeField]
	private string name;

	[SerializeField]
	private int currentAmount;

	[SerializeField]
	private int neededAmount;

	public string GetText()
	{
		if (currentAmount > neededAmount)
		{
			currentAmount = neededAmount;
		}
		return $"{name} {currentAmount}/{neededAmount}";
	}

	public Texture2D GetIcon()
	{
		return icon;
	}

	public ItemIngredientData(Texture2D icon, string name, int currentAmount, int neededAmount)
	{
		this.icon = icon;
		this.name = name;
		this.currentAmount = currentAmount;
		this.neededAmount = neededAmount;
	}
}
