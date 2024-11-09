using UnityEngine;

namespace Code.Core.Avatar.Badges;

public class PremiumLifetimeBaseBadge : BaseBadge
{
	public PremiumLifetimeBaseBadge(MilMo_Avatar avatar)
		: base(avatar)
	{
	}

	public override void Destroy()
	{
	}

	public override string GetIconPath()
	{
		return "TagPremiumRed";
	}

	public override string GetIdentifier()
	{
		return "LifetimePremium";
	}

	public override bool IsEarned()
	{
		Debug.LogWarning($"{GetIdentifier()}.MembershipDaysLeft: {Avatar.MembershipDaysLeft}");
		return Avatar.MembershipDaysLeft > 30000;
	}
}
