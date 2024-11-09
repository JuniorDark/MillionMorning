using System;
using System.Collections.Generic;

namespace Code.Core.Items;

public class MilMo_MemberSubscription : MilMo_Item
{
	public override string PickupParticle
	{
		get
		{
			throw new NotSupportedException("Pickup particles are not supported for Member subscriptions.");
		}
	}

	public new MilMo_MemberSubscriptionTemplate Template => (MilMo_MemberSubscriptionTemplate)base.Template;

	public MilMo_MemberSubscription(MilMo_MemberSubscriptionTemplate template, Dictionary<string, string> modifiers)
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
		throw new NotSupportedException("Picking up Member subscriptions are not supported.");
	}
}
