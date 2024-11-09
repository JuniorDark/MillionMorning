using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Core.Items;
using Code.Core.ResourceSystem;
using Code.World.Achievements;
using Code.World.Feeds;
using Code.World.Player;
using Core;
using Core.Utilities;
using Localization;
using UI.HUD.TopMenu;
using UnityEngine;

namespace UI.HUD.Dialogues.Feed.Medal;

[CreateAssetMenu(menuName = "Dialogues/MedalFeedDialogueSO", fileName = "MedalFeedDialogueSO")]
public class MedalFeedDialogueSO : FeedDialogueSO
{
	[Serializable]
	public class AchievementFeedData
	{
		public string caption;

		public string criteria;

		public RewardFeedData reward;
	}

	[Serializable]
	public class RewardFeedData
	{
		public string name;

		public string amount;

		public Texture2D icon;
	}

	[SerializeField]
	protected string headline;

	[SerializeField]
	protected string description;

	[SerializeField]
	protected string iconPath;

	[SerializeField]
	protected string nextMedalCaption;

	private Transform _objectDestination;

	[SerializeField]
	protected AchievementFeedData medalWon;

	[SerializeField]
	protected AchievementFeedData nextMedal;

	public override string GetAddressableKey()
	{
		return "MedalFeedDialogueWindow";
	}

	public override int GetPriority()
	{
		return 5;
	}

	public override Transform GetObjectDestination()
	{
		return _objectDestination;
	}

	public async void Init(MilMo_AchievementTemplate achievementTemplate)
	{
		_objectDestination = UnityEngine.Object.FindObjectOfType<ProfileButton>()?.transform;
		headline = MilMo_FeedExclamations.GetExclamation().String;
		description = MilMo_Localization.GetLocString("Items_4605").String;
		if (!string.IsNullOrEmpty(achievementTemplate.FeedEventIngame.String))
		{
			description = achievementTemplate.FeedEventIngame.String;
		}
		iconPath = achievementTemplate.IconPath;
		MilMo_LocString copy = MilMo_Localization.GetLocString("World_366").GetCopy();
		copy.SetFormatArgs(achievementTemplate.AchievementCategoryDisplayName);
		nextMedalCaption = copy.String;
		medalWon = await GetMedal(achievementTemplate);
		nextMedal = await GetNextMedal(achievementTemplate);
	}

	private async Task<AchievementFeedData> GetMedal(MilMo_AchievementTemplate achievementTemplate)
	{
		AchievementFeedData achievementFeedData = new AchievementFeedData
		{
			caption = achievementTemplate.DisplayName.String,
			criteria = achievementTemplate.Description.String
		};
		AchievementFeedData achievementFeedData2 = achievementFeedData;
		achievementFeedData2.reward = await GetReward(achievementTemplate);
		return achievementFeedData;
	}

	private async Task<AchievementFeedData> GetNextMedal(MilMo_AchievementTemplate achievementTemplate)
	{
		if (achievementTemplate.AchievementCategoryIdentifier.Equals("Miscellaneous") || achievementTemplate.AchievementCategoryIdentifier.Equals("LevelExploration"))
		{
			return null;
		}
		MilMo_AchievementTemplate nextInSameCategory = Singleton<MilMo_AchievementHandler>.Instance.GetNextInSameCategory(achievementTemplate);
		if (nextInSameCategory == null)
		{
			return null;
		}
		return await GetMedal(nextInSameCategory);
	}

	private async Task<RewardFeedData> GetReward(MilMo_AchievementTemplate achievementTemplate)
	{
		bool valueOrDefault = (MilMo_Player.Instance?.Avatar?.IsBoy).GetValueOrDefault();
		MilMo_Item reward = achievementTemplate.GetReward(valueOrDefault)?.Instantiate(new Dictionary<string, string>());
		if (reward == null)
		{
			return null;
		}
		int rewardAmount = achievementTemplate.GetRewardAmount(valueOrDefault);
		bool haveReward = rewardAmount > 0;
		bool rewardIsItem = haveReward && !(reward is MilMo_Gem) && !(reward is MilMo_Coin);
		Texture2D icon;
		if (rewardIsItem)
		{
			icon = await reward.AsyncGetIcon();
		}
		else
		{
			Texture2D texture2D = ((reward is MilMo_Gem) ? (await Core.Utilities.UI.GetIcon("IconGem")) : ((!(reward is MilMo_Coin)) ? null : (await Core.Utilities.UI.GetIcon("IconVoucherPointCounter"))));
			icon = texture2D;
		}
		return new RewardFeedData
		{
			name = ((haveReward && rewardIsItem) ? reward.Template.DisplayName.String : ""),
			icon = icon,
			amount = rewardAmount.ToString()
		};
	}

	public string GetHeadline()
	{
		return headline;
	}

	public string GetDescription()
	{
		return description;
	}

	public string GetIconPath()
	{
		return iconPath;
	}

	public string GetNextMedalCaption()
	{
		return nextMedalCaption;
	}

	public AchievementFeedData GetMedalWon()
	{
		return medalWon;
	}

	public AchievementFeedData GetNextMedal()
	{
		return nextMedal;
	}

	private void Confirm()
	{
		DialogueWindow.Close();
	}

	public override List<DialogueButtonInfo> GetActions()
	{
		return new List<DialogueButtonInfo>
		{
			new DialogueButtonInfo(Confirm, new LocalizedStringWithArgument("Generic_OK"), isDefault: true)
		};
	}

	public override bool CanBeShown()
	{
		return true;
	}
}
