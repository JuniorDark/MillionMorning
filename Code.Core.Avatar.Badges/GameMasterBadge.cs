namespace Code.Core.Avatar.Badges;

public class GameMasterBadge : BaseBadge
{
	public GameMasterBadge(MilMo_Avatar avatar)
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
		return "GM";
	}

	public override bool IsEarned()
	{
		sbyte role = Avatar.Role;
		if (role == 1 || role == 2)
		{
			return Avatar.ShowRole;
		}
		return false;
	}
}
