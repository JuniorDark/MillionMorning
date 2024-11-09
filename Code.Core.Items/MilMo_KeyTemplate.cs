using System.Collections.Generic;

namespace Code.Core.Items;

public class MilMo_KeyTemplate : MilMo_ItemTemplate
{
	public static MilMo_KeyTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_KeyTemplate(category, path, filePath);
	}

	protected MilMo_KeyTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "Key")
	{
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_Key(this, modifiers);
	}
}
