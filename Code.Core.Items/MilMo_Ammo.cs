using System.Collections.Generic;

namespace Code.Core.Items;

public sealed class MilMo_Ammo : MilMo_Item
{
	public override string PickupParticle => "GemPickup";

	public new MilMo_AmmoTemplate Template => (MilMo_AmmoTemplate)base.Template;

	public MilMo_Ammo(MilMo_ItemTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public override bool IsWieldable()
	{
		return false;
	}

	public override bool IsWearable()
	{
		return false;
	}

	public override bool AutoPickup()
	{
		return true;
	}
}
