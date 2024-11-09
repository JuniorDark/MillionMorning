using System.Collections.Generic;

namespace Code.Core.Items;

public class MilMo_GenericItemTemplate : MilMo_ItemTemplate
{
	public static MilMo_GenericItemTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_GenericItemTemplate(category, path, filePath);
	}

	protected MilMo_GenericItemTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "GenericItem")
	{
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_GenericItem(this, modifiers);
	}
}
