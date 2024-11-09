using System.Collections.Generic;

namespace Code.Core.Items;

public class MilMo_Gem : MilMo_Item
{
	public override string PickupParticle => "GemPickup";

	public new MilMo_GemTemplate Template => (MilMo_GemTemplate)base.Template;

	public MilMo_Gem(MilMo_GemTemplate template, Dictionary<string, string> modifiers)
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
