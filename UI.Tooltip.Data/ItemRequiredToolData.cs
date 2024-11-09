using System;
using UnityEngine;

namespace UI.Tooltip.Data;

[Serializable]
public class ItemRequiredToolData
{
	[SerializeField]
	private Texture2D icon;

	[SerializeField]
	private string name;

	public string GetText()
	{
		return name;
	}

	public Texture2D GetIcon()
	{
		return icon;
	}

	public ItemRequiredToolData(Texture2D icon, string name)
	{
		this.icon = icon;
		this.name = name;
	}
}
