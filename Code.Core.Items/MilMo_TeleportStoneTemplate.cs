using System.Collections.Generic;

namespace Code.Core.Items;

public class MilMo_TeleportStoneTemplate : MilMo_ItemTemplate
{
	public static MilMo_TeleportStoneTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_TeleportStoneTemplate(category, path, filePath);
	}

	protected MilMo_TeleportStoneTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "TeleportStone")
	{
		CustomIdiotIconPath = "Content/Items/Batch01/SpecialItems/IconTeleportStone";
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_TeleportStone(this, modifiers);
	}
}
