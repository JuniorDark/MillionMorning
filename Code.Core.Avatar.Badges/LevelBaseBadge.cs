namespace Code.Core.Avatar.Badges;

public class LevelBaseBadge : BaseBadge
{
	public LevelBaseBadge(MilMo_Avatar avatar)
		: base(avatar)
	{
		Avatar.OnAvatarLevelUpdated += LevelUpdated;
	}

	public override void Destroy()
	{
		Avatar.OnAvatarLevelUpdated -= LevelUpdated;
	}

	private void LevelUpdated(int notUsed)
	{
		TriggerOnChange();
	}

	public override string GetIconPath()
	{
		return "IconGuiXp";
	}

	public override string GetIdentifier()
	{
		return "Level";
	}

	public override bool IsEarned()
	{
		return true;
	}

	public override string GetText()
	{
		return $"{Avatar.AvatarLevel}";
	}
}
