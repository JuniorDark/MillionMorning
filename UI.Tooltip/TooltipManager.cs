using System.Collections.Generic;
using System.Linq;
using UI.Tooltip.Data;
using UnityEngine;

namespace UI.Tooltip;

public class TooltipManager : MonoBehaviour
{
	private static TooltipManager _instance;

	[SerializeField]
	private List<Tooltip> tooltips;

	public void Initialize()
	{
		_instance = this;
		base.gameObject.SetActive(value: true);
		if (tooltips.Count < 1)
		{
			Debug.LogWarning(base.gameObject.name + " Missing Tooltips!");
			return;
		}
		tooltips.ForEach(delegate(Tooltip t)
		{
			t.gameObject.SetActive(value: false);
		});
	}

	public static void Hide()
	{
		if ((bool)_instance && _instance.tooltips.Count >= 1)
		{
			_instance.tooltips.ForEach(delegate(Tooltip t)
			{
				t.Hide();
			});
		}
	}

	public static void Show(TooltipData data)
	{
		if ((bool)_instance && _instance.tooltips.Count >= 1)
		{
			Tooltip tooltip = ((data is MedalTooltipData) ? _instance.tooltips.FirstOrDefault((Tooltip t) => t is MedalTooltip) : ((data is WeaponTooltipData) ? _instance.tooltips.FirstOrDefault((Tooltip t) => t is WeaponTooltip) : ((data is ConverterTooltipData) ? _instance.tooltips.FirstOrDefault((Tooltip t) => t is ConverterTooltip) : ((!(data is ItemTooltipData)) ? _instance.tooltips.FirstOrDefault((Tooltip t) => t is SimpleTooltip) : _instance.tooltips.FirstOrDefault((Tooltip t) => t is ItemTooltip)))));
			Tooltip tooltip2 = tooltip;
			if (!(tooltip2 == null))
			{
				tooltip2.SetData(data);
				tooltip2.Show();
				_instance.transform.SetAsLastSibling();
			}
		}
	}
}
