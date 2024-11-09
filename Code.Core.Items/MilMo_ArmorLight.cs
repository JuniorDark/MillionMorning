using System.Collections.Generic;

namespace Code.Core.Items;

public sealed class MilMo_ArmorLight : MilMo_Armor
{
	public MilMo_ArmorLight(MilMo_WearableTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
		MDurability = 12f;
	}

	public override void Equip()
	{
	}
}
