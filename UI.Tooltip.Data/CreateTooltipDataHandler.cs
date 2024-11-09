using System.Threading.Tasks;
using Code.Core.Items;
using Code.World.Achievements;
using UI.Elements.Slot;
using UI.Tooltip.Data.GenerateTooltipData;
using UnityEngine;

namespace UI.Tooltip.Data;

public class CreateTooltipDataHandler
{
	private readonly IEntryItem _item;

	private readonly Texture2D _icon;

	public CreateTooltipDataHandler(IEntryItem item, Texture2D icon)
	{
		_item = item;
		_icon = icon;
	}

	public async Task<TooltipData> GetTooltipData()
	{
		if (_item == null)
		{
			Debug.LogWarning("Unable to get IEntryItem");
			return null;
		}
		if (_icon == null)
		{
			Debug.LogWarning("Unable to get icon");
			return null;
		}
		IEntryItem item = _item;
		return (item is MilMo_Medal) ? (await new GenerateMedalTooltipData(_item, _icon).CreateTooltip()) : ((item is MilMo_Weapon) ? (await new GenerateWeaponTooltipData(_item, _icon).CreateTooltip()) : ((!(item is MilMo_Converter)) ? (await new UI.Tooltip.Data.GenerateTooltipData.GenerateTooltipData(_item, _icon).CreateTooltip()) : (await new GenerateConverterTooltipData(_item, _icon).CreateTooltip())));
	}
}
