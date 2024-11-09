using System.Collections.Generic;

namespace Code.Core.Items;

public class MilMo_NameChangeTemplate : MilMo_ItemTemplate
{
	public static MilMo_NameChangeTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_NameChangeTemplate(category, path, filePath);
	}

	protected MilMo_NameChangeTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "NameChange")
	{
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_NameChange(this, modifiers);
	}
}
