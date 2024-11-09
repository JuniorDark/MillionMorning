using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.Dialogues.Feed.Medal;

public class MedalFeedDialogueWindow : FeedDialogueWindow
{
	[Header("Dialogue SO")]
	[SerializeField]
	private MedalFeedDialogueSO medalFeedDialogueSO;

	[Header("Header")]
	[SerializeField]
	private Image icon;

	[SerializeField]
	private TMP_Text headline;

	[SerializeField]
	private TMP_Text description;

	[Header("Body")]
	[SerializeField]
	private TMP_Text medalCaption;

	[SerializeField]
	private TMP_Text medalCriteria;

	[SerializeField]
	private RewardFeedDialogueContent medalReward;

	[SerializeField]
	private GameObject nextMedalBox;

	[SerializeField]
	private TMP_Text nextMedalHeadline;

	[SerializeField]
	private TMP_Text nextMedalCaption;

	[SerializeField]
	private TMP_Text nextMedalCriteria;

	[SerializeField]
	private RewardFeedDialogueContent nextReward;

	public override void Init(DialogueSO so)
	{
		medalFeedDialogueSO = (MedalFeedDialogueSO)so;
		if (medalFeedDialogueSO == null)
		{
			Debug.LogError(base.name + ": DialogueSO is of wrong type");
		}
		else
		{
			base.Init(so);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (icon == null)
		{
			Debug.LogError(base.name + ": Missing icon");
			return;
		}
		icon.enabled = false;
		if (headline == null)
		{
			Debug.LogError(base.name + ": Missing headline");
			return;
		}
		headline.text = "";
		if (description == null)
		{
			Debug.LogError(base.name + ": Missing description");
			return;
		}
		description.text = "";
		if (medalCaption == null)
		{
			Debug.LogError(base.name + ": Missing medalTitle");
			return;
		}
		medalCaption.text = "";
		if (medalCriteria == null)
		{
			Debug.LogError(base.name + ": Missing medalCriteria");
			return;
		}
		medalCriteria.text = "";
		if (medalReward == null)
		{
			Debug.LogError(base.name + ": Missing medalReward");
			return;
		}
		if (nextMedalBox == null)
		{
			Debug.LogError(base.name + ": Missing nextMedalBox");
			return;
		}
		if (nextMedalHeadline == null)
		{
			Debug.LogError(base.name + ": Missing nextMedalHeadline");
			return;
		}
		nextMedalHeadline.text = "";
		if (nextMedalCaption == null)
		{
			Debug.LogError(base.name + ": Missing nextMedalCaption");
			return;
		}
		nextMedalCaption.text = "";
		if (nextMedalCriteria == null)
		{
			Debug.LogError(base.name + ": Missing nextMedalCriteria");
			return;
		}
		nextMedalCriteria.text = "";
		if (nextReward == null)
		{
			Debug.LogError(base.name + ": Missing nextReward");
		}
	}

	protected override async void RefreshHeader()
	{
		if (icon != null)
		{
			Image target = icon;
			SetIcon(target, await medalFeedDialogueSO.GetIconAsync(medalFeedDialogueSO.GetIconPath()));
		}
		if (headline != null)
		{
			headline.SetText(medalFeedDialogueSO.GetHeadline());
		}
		if (description != null)
		{
			description.SetText(medalFeedDialogueSO.GetDescription());
		}
	}

	protected override void RefreshBody()
	{
		if (nextMedalHeadline != null)
		{
			nextMedalHeadline.SetText(medalFeedDialogueSO.GetNextMedalCaption());
		}
		InitMedal(medalFeedDialogueSO.GetMedalWon());
		if (!InitNextMedal(medalFeedDialogueSO.GetNextMedal()))
		{
			nextMedalBox.SetActive(value: false);
		}
	}

	protected override void RefreshFooter()
	{
		RefreshActions();
		EnableButtons(shouldEnable: true);
	}

	private bool InitNextMedal(MedalFeedDialogueSO.AchievementFeedData medal)
	{
		if (medal == null)
		{
			return false;
		}
		if (nextMedalCaption != null)
		{
			nextMedalCaption.SetText(medal.caption);
		}
		if (nextMedalCriteria != null)
		{
			nextMedalCriteria.SetText(medal.criteria);
		}
		if (nextReward != null)
		{
			bool active = nextReward.Init(this, medal.reward);
			nextReward.gameObject.SetActive(active);
		}
		return true;
	}

	private void InitMedal(MedalFeedDialogueSO.AchievementFeedData medal)
	{
		if (medal != null)
		{
			if (medalCaption != null)
			{
				medalCaption.SetText(medal.caption);
			}
			if (medalCriteria != null)
			{
				medalCriteria.SetText(medal.criteria);
			}
			if (!(medalReward == null))
			{
				bool active = medalReward.Init(this, medal.reward);
				medalReward.gameObject.SetActive(active);
			}
		}
	}

	public override void Close()
	{
		CreateFlyingClone();
		base.Close();
	}

	private void CreateFlyingClone()
	{
		SpawnFlyingIcon(icon, base.transform.parent, medalFeedDialogueSO.GetObjectDestination());
	}
}
