using UnityEngine;

namespace Code.Core.Avatar.Badges;

public class Premium1MonthBadge : BaseBadge
{
	public Premium1MonthBadge(MilMo_Avatar avatar)
		: base(avatar)
	{
	}

	public override void Destroy()
	{
	}

	public override string GetIconPath()
	{
		return "TagPremium";
	}

	public override string GetIdentifier()
	{
		return "1MonthPremium";
	}

	public override bool IsEarned()
	{
		Debug.LogWarning($"{GetIdentifier()}.MembershipDaysLeft: {Avatar.MembershipDaysLeft}");
		int membershipDaysLeft = Avatar.MembershipDaysLeft;
		if (membershipDaysLeft > 1)
		{
			return membershipDaysLeft <= 30;
		}
		return false;
	}
}
