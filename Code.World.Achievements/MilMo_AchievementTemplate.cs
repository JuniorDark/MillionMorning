using System.Collections.Generic;
using Code.Core.Items;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Template;

namespace Code.World.Achievements;

public class MilMo_AchievementTemplate : MilMo_ItemTemplate
{
	private const string GLOBAL_MEDAL_WORLD = "Global";

	private MilMo_ItemTemplate _boyReward;

	private int _boyRewardAmount;

	private MilMo_ItemTemplate _girlReward;

	private int _girlRewardAmount;

	public string AchievementCategoryIdentifier { get; private set; }

	public int CategoryIndex { get; private set; }

	public List<string> Worlds { get; private set; }

	public List<MilMo_AchievementObjective> Objectives { get; private set; }

	public MilMo_LocString NotCompleteDescription { get; private set; }

	public MilMo_LocString AchievementCategoryDisplayName => MilMo_AchievementHandler.Get().MedalCategoryDisplayName(AchievementCategoryIdentifier);

	public bool IsGlobal => Worlds.Contains("Global");

	public override bool LoadFromNetwork(Template t)
	{
		AchievementTemplate template = t as AchievementTemplate;
		base.LoadFromNetwork((Template)template);
		if (template != null)
		{
			MilMo_TemplateContainer milMo_TemplateContainer = MilMo_TemplateContainer.Get();
			AchievementCategoryIdentifier = template.GetCategory();
			CategoryIndex = template.GetCategoryIndex();
			Worlds = (List<string>)template.GetWorlds();
			foreach (AchievementObjective objective in template.GetObjectives())
			{
				Objectives.Add(new MilMo_AchievementObjective(objective));
			}
			NotCompleteDescription = MilMo_Localization.GetLocString(template.GetNotCompleteDescription());
			if (!string.IsNullOrEmpty(template.GetBoyReward()))
			{
				milMo_TemplateContainer.GetTemplate(template.GetBoyReward(), delegate(MilMo_Template rewardTemplate, bool timeout)
				{
					if (!(rewardTemplate == null || timeout))
					{
						_boyReward = rewardTemplate as MilMo_ItemTemplate;
						if (_boyReward != null)
						{
							_boyRewardAmount = template.GetBoyRewardAmount();
						}
					}
				});
			}
			if (!string.IsNullOrEmpty(template.GetGirlReward()))
			{
				milMo_TemplateContainer.GetTemplate(template.GetGirlReward(), delegate(MilMo_Template rewardTemplate, bool timeout)
				{
					if (!(rewardTemplate == null || timeout))
					{
						_girlReward = rewardTemplate as MilMo_ItemTemplate;
						if (_girlReward != null)
						{
							_girlRewardAmount = template.GetGirlRewardAmount();
						}
					}
				});
			}
		}
		if (base.FeedDescriptionIngame == null || string.IsNullOrEmpty(base.FeedDescriptionIngame.String))
		{
			base.FeedDescriptionIngame = base.Description;
		}
		return true;
	}

	public MilMo_ItemTemplate GetReward(bool isBoy)
	{
		if (!isBoy)
		{
			return _girlReward;
		}
		return _boyReward;
	}

	public int GetRewardAmount(bool isBoy)
	{
		if (!isBoy)
		{
			return _girlRewardAmount;
		}
		return _boyRewardAmount;
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_Medal(this, modifiers);
	}

	public static MilMo_AchievementTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_AchievementTemplate(category, path, filePath, "Medal");
	}

	private MilMo_AchievementTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
		AchievementCategoryIdentifier = "";
		Worlds = new List<string>();
		Objectives = new List<MilMo_AchievementObjective>();
	}
}
