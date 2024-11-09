using System.Collections.Generic;
using Code.Core.Items.Home.HomeEquipmentTemplate;
using Code.Core.Items.Home.HomeEquipmentTemplate.FurnitureTemplate.FloorFurnitureTemplate;

namespace Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.Furniture.FloorFurniture;

public sealed class MilMo_Seat : MilMo_FloorFurniture
{
	public new MilMo_SeatTemplate Template => ((MilMo_Item)this).Template as MilMo_SeatTemplate;

	public override bool IsChatroom => Template.SitNodes.Count > 0;

	public MilMo_Seat(MilMo_FurnitureTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}
}
