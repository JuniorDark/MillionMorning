using System.Collections.Generic;

namespace Code.Core.Items;

public class MilMo_ItemTrackUpgrade : MilMo_PowerUp
{
	public override string PickupParticle => "OrbPickup";

	public new MilMo_ItemTrackUpgradeTemplate Template => ((MilMo_Item)this).Template as MilMo_ItemTrackUpgradeTemplate;

	public MilMo_ItemTrackUpgrade(MilMo_ItemTrackUpgradeTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}
}
