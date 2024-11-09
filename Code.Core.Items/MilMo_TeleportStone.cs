using System.Collections.Generic;

namespace Code.Core.Items;

public class MilMo_TeleportStone : MilMo_Item
{
	public override string PickupParticle => "GemPickup";

	public new MilMo_TeleportStoneTemplate Template => (MilMo_TeleportStoneTemplate)base.Template;

	public MilMo_TeleportStone(MilMo_TeleportStoneTemplate template, Dictionary<string, string> modifiers)
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
