using System.Collections.Generic;

namespace Code.Core.Items;

public class MilMo_GemTemplate : MilMo_ItemTemplate
{
	public static MilMo_GemTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_GemTemplate(category, path, filePath);
	}

	protected MilMo_GemTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "Gem")
	{
		CustomIdiotIconPath = "Content/GUI/Batch01/Textures/GameDialog/IconGem";
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_Gem(this, modifiers);
	}
}
