using System.Collections.Generic;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.HomeSurface;

namespace Code.Core.Items.Home.HomeEquipmentTemplate.HomeSurfaceTemplate;

public sealed class MilMo_FloorTemplate : MilMo_HomeSurfaceTemplate
{
	public override bool IsSkin => true;

	private MilMo_FloorTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public static MilMo_FloorTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_FloorTemplate(category, path, filePath, "Floor");
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_Floor(this, modifiers);
	}
}
