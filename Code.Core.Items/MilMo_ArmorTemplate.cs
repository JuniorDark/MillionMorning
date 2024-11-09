using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Items;

public class MilMo_ArmorTemplate : MilMo_WearableTemplate
{
	public string ArmorClass { get; private set; }

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		ArmorTemplate armorTemplate = t as ArmorTemplate;
		base.LoadFromNetwork((Code.Core.Network.types.Template)armorTemplate);
		if (armorTemplate == null)
		{
			return false;
		}
		ArmorClass = armorTemplate.GetArmorClass();
		return true;
	}

	public new static MilMo_ArmorTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_ArmorTemplate(category, path, filePath, "Armor");
	}

	private MilMo_ArmorTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return MilMo_Armor.Create(this, modifiers);
	}
}
