using Code.Core.ResourceSystem;
using TMPro;
using UI.HUD.Dialogues.Feed.Medal;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.Dialogues.Feed;

public class RewardFeedDialogueContent : MonoBehaviour
{
	[SerializeField]
	private TMP_Text caption;

	[SerializeField]
	private TMP_Text rewardName;

	[SerializeField]
	private TMP_Text amount;

	[SerializeField]
	private Image icon;

	public bool Init(MedalFeedDialogueWindow parent, MedalFeedDialogueSO.RewardFeedData reward)
	{
		if (reward == null)
		{
			return false;
		}
		if (caption != null)
		{
			caption.SetText(MilMo_Localization.GetLocString("World_365").String);
		}
		if (rewardName != null)
		{
			rewardName.SetText(reward.name);
		}
		if (amount != null)
		{
			amount.SetText(reward.amount);
		}
		if (icon != null)
		{
			parent.SetIcon(icon, reward.icon);
		}
		return true;
	}
}
