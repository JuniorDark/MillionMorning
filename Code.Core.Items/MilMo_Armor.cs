using System;
using System.Collections.Generic;

namespace Code.Core.Items;

public abstract class MilMo_Armor : MilMo_Wearable
{
	protected float MDurability;

	public new MilMo_ArmorTemplate Template => ((MilMo_Item)this).Template as MilMo_ArmorTemplate;

	public float Durability => MDurability;

	protected MilMo_Armor(MilMo_WearableTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public static MilMo_Armor Create(MilMo_ArmorTemplate template, Dictionary<string, string> modifiers)
	{
		if (string.Equals(template.ArmorClass, "Light", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_ArmorLight(template, modifiers);
		}
		if (string.Equals(template.ArmorClass, "Heavy", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_ArmorHeavy(template, modifiers);
		}
		return null;
	}

	public void SetDurability(float value)
	{
		MDurability = value;
	}

	public abstract void Equip();

	public void Unequip()
	{
	}
}
