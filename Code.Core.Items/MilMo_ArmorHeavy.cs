using System.Collections.Generic;

namespace Code.Core.Items;

public sealed class MilMo_ArmorHeavy : MilMo_Armor
{
	public MilMo_ArmorHeavy(MilMo_WearableTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
		MDurability = 18f;
	}

	public override void Equip()
	{
	}
}
