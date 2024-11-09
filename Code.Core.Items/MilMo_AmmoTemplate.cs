using System.Collections.Generic;

namespace Code.Core.Items;

public sealed class MilMo_AmmoTemplate : MilMo_ItemTemplate
{
	public static MilMo_AmmoTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_AmmoTemplate(category, path, filePath);
	}

	private MilMo_AmmoTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "Ammo")
	{
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_Ammo(this, modifiers);
	}
}
