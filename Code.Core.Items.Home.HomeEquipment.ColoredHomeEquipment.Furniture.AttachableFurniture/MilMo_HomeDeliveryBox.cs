using System.Collections.Generic;
using Code.Core.Items.Home.HomeEquipmentTemplate;
using Code.Core.Items.Home.HomeEquipmentTemplate.FurnitureTemplate.AttachableFurnitureTemplate;

namespace Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.Furniture.AttachableFurniture;

public sealed class MilMo_HomeDeliveryBox : MilMo_AttachableFurniture
{
	public new MilMo_HomeDeliveryBoxTemplate Template => ((MilMo_Item)this).Template as MilMo_HomeDeliveryBoxTemplate;

	public MilMo_HomeDeliveryBox(MilMo_FurnitureTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}
}
