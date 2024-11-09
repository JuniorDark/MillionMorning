using System.Collections.Generic;

namespace Code.Core.Items;

public class MilMo_RespecTemplate : MilMo_ItemTemplate
{
	public static MilMo_RespecTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_RespecTemplate(category, path, filePath);
	}

	protected MilMo_RespecTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "Respec")
	{
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_Respec(this, modifiers);
	}
}
