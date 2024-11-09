using System;
using System.Collections.Generic;
using Core.Utilities;
using UnityEngine;

namespace UI.Tooltip.Data;

[Serializable]
public class StatData
{
	[SerializeField]
	private string label;

	[SerializeField]
	private int value;

	private Dictionary<string, string> _locales = new Dictionary<string, string>
	{
		{ "Attack", "Bag_9343" },
		{ "Magic", "Bag_9344" },
		{ "Speed", "Bag_9345" },
		{ "Range", "Bag_9346" }
	};

	public string GetLabel()
	{
		return LocalizationHelper.GetLocaleString(label, _locales);
	}

	public int GetValue()
	{
		return value;
	}

	public StatData(string label, int value)
	{
		this.label = label;
		this.value = value;
	}
}
