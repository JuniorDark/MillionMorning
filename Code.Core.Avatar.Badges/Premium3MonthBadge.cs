namespace Code.Core.Avatar.Badges;

public class Premium3MonthBadge : BaseBadge
{
	public Premium3MonthBadge(MilMo_Avatar avatar)
		: base(avatar)
	{
	}

	public override void Destroy()
	{
	}

	public override string GetIconPath()
	{
		return "TagPremiumGreen";
	}

	public override string GetIdentifier()
	{
		return "3MonthPremium";
	}

	public override bool IsEarned()
	{
		int membershipDaysLeft = Avatar.MembershipDaysLeft;
		if (membershipDaysLeft > 31)
		{
			return membershipDaysLeft <= 90;
		}
		return false;
	}
}
