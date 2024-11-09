using System.Collections.Generic;
using Code.World.Player;

namespace Code.Core.Items;

public class MilMo_HealthPickup : MilMo_PowerUp
{
	public new MilMo_HealthPickupTemplate Template => ((MilMo_Item)this).Template as MilMo_HealthPickupTemplate;

	public MilMo_HealthPickup(MilMo_HealthPickupTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public override bool MayPickUp()
	{
		return MilMo_Player.Instance.Avatar.Health < MilMo_Player.Instance.Avatar.MaxHealth;
	}
}
