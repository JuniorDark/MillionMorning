using System.Collections.Generic;
using Code.Core.Items.Home.HomeEquipment;

namespace Code.Core.Items.Home.HomeEquipmentTemplate;

public sealed class MilMo_RoomTemplate : MilMo_HomeEquipmentTemplate
{
	public override bool IsRoom => true;

	private MilMo_RoomTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "Room")
	{
	}

	public static MilMo_RoomTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_RoomTemplate(category, path, filePath);
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_Room(this, modifiers);
	}
}
