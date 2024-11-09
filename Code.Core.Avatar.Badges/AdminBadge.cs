namespace Code.Core.Avatar.Badges;

public class AdminBadge : BaseBadge
{
	public AdminBadge(MilMo_Avatar avatar)
		: base(avatar)
	{
		Avatar.OnShowRoleChanged += base.TriggerOnChange;
	}

	public override void Destroy()
	{
		Avatar.OnShowRoleChanged -= base.TriggerOnChange;
	}

	public override string GetIconPath()
	{
		return "TagGMBadge";
	}

	public override string GetIdentifier()
	{
		return "Admin";
	}

	public override bool IsEarned()
	{
		sbyte role = Avatar.Role;
		if (role == 3 || role == 4)
		{
			return Avatar.ShowRole;
		}
		return false;
	}
}
