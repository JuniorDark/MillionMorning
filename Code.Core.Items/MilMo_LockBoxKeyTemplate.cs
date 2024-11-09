using System.Collections.Generic;

namespace Code.Core.Items;

public class MilMo_LockBoxKeyTemplate : MilMo_ItemTemplate
{
	public static MilMo_LockBoxKeyTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_LockBoxKeyTemplate(category, path, filePath);
	}

	protected MilMo_LockBoxKeyTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "LockBoxKey")
	{
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_LockBoxKey(this, modifiers);
	}
}
