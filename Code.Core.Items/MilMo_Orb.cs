using System.Collections.Generic;

namespace Code.Core.Items;

public class MilMo_Orb : MilMo_PowerUp
{
	public override string PickupParticle => "OrbPickup";

	public new MilMo_OrbTemplate Template => ((MilMo_Item)this).Template as MilMo_OrbTemplate;

	public MilMo_Orb(MilMo_OrbTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}
}
