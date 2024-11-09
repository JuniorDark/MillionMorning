namespace Code.Core.Avatar.Badges;

public class Premium6MonthBadge : BaseBadge
{
	public Premium6MonthBadge(MilMo_Avatar avatar)
		: base(avatar)
	{
	}

	public override void Destroy()
	{
	}

	public override string GetIconPath()
	{
		return "TagPremiumYellow";
	}

	public override string GetIdentifier()
	{
		return "6MonthPremium";
	}

	public override bool IsEarned()
	{
		int membershipDaysLeft = Avatar.MembershipDaysLeft;
		if (membershipDaysLeft > 90)
		{
			return membershipDaysLeft <= 180;
		}
		return false;
	}
}
