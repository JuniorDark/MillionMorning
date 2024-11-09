namespace Code.World.Achievements;

public class MilMo_AchievementObjectiveListener
{
	private readonly MilMo_AchievementTemplate _achievementTemplate;

	public string Object { get; private set; }

	public MilMo_AchievementObjectiveListener(MilMo_AchievementTemplate template, string obj)
	{
		_achievementTemplate = template;
		Object = obj;
	}

	public void Notify()
	{
		MilMo_AchievementHandler.Get().TestCompletion(_achievementTemplate);
	}
}
