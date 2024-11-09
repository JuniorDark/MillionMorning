using System.Collections.Generic;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.Furniture;
using Code.Core.Network.types;

namespace Code.Core.Items.Home.HomeEquipmentTemplate.FurnitureTemplate;

public class MilMo_AttachableFurnitureTemplate : MilMo_FurnitureTemplate
{
	protected MilMo_AttachableFurnitureTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public static MilMo_AttachableFurnitureTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_AttachableFurnitureTemplate(category, path, filePath, "AttachableFurniture");
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_AttachableFurniture(this, modifiers);
	}

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		return base.LoadFromNetwork(t);
	}
}
