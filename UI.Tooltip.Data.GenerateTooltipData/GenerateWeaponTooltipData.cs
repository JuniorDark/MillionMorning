using System.Threading.Tasks;
using Code.Core.Items;
using Code.World.GUI.ShopPopups;
using UI.Elements.Slot;
using UnityEngine;

namespace UI.Tooltip.Data.GenerateTooltipData;

public class GenerateWeaponTooltipData : GenerateTooltipData
{
	private readonly WeaponStatBrackets _brackets;

	public GenerateWeaponTooltipData(IEntryItem item, Texture2D icon)
		: base(item, icon)
	{
		_brackets = new WeaponStatBrackets();
	}

	public override async Task<TooltipData> CreateTooltip()
	{
		return await Task.Run(delegate
		{
			if (!(Item is MilMo_Weapon milMo_Weapon))
			{
				return (WeaponTooltipData)null;
			}
			if (milMo_Weapon.Template == null)
			{
				return (WeaponTooltipData)null;
			}
			string title = milMo_Weapon.Template.DisplayName?.String;
			string description = milMo_Weapon.Template.Description?.String;
			StatData[] stats = new StatData[4]
			{
				new StatData("Attack", _brackets.GetAttackPoints(milMo_Weapon.Template.NormalDamage)),
				new StatData("Magic", _brackets.GetMagicPoints(milMo_Weapon.Template.MagicDamage)),
				new StatData("Speed", _brackets.GetSpeedPoints(milMo_Weapon.Template.Cooldown)),
				new StatData("Range", _brackets.GetRangePoints(milMo_Weapon.Template.Range))
			};
			return new WeaponTooltipData(title, description, Icon, stats);
		});
	}
}
