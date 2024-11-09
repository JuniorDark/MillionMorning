using Player;

namespace Code.Core.Avatar.Badges;

public class GroupLeaderBadge : BaseBadge
{
	private readonly GroupManager _groupManager;

	public GroupLeaderBadge(MilMo_Avatar avatar)
		: base(avatar)
	{
		_groupManager = GroupManager.Get();
		_groupManager.OnLeaderChange += base.TriggerOnChange;
	}

	public override void Destroy()
	{
		if (!(_groupManager == null))
		{
			_groupManager.OnLeaderChange -= base.TriggerOnChange;
		}
	}

	public override string GetIconPath()
	{
		return "IconGroupLeader";
	}

	public override string GetIdentifier()
	{
		return "Leader";
	}

	public override bool IsEarned()
	{
		return _groupManager.IsGroupLeader(Avatar.Id);
	}
}
