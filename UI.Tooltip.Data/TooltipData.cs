using System;
using UnityEngine;

namespace UI.Tooltip.Data;

[Serializable]
public class TooltipData
{
	[SerializeField]
	protected string title;

	public TooltipData(string title)
	{
		this.title = title;
	}

	public string GetTitle()
	{
		return title;
	}
}
