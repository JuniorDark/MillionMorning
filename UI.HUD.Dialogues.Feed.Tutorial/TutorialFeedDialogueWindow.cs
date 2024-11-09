using Core.Analytics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.Dialogues.Feed.Tutorial;

public class TutorialFeedDialogueWindow : FeedDialogueWindow
{
	[Header("Dialogue SO")]
	[SerializeField]
	private TutorialFeedDialogueSO tutorialFeedDialogueSO;

	[Header("Header")]
	[SerializeField]
	private Image face;

	[SerializeField]
	private Image slot1;

	[SerializeField]
	private Image slot2;

	[Header("Body")]
	[SerializeField]
	private TMP_Text contentHeadline;

	[SerializeField]
	private TMP_Text contentDescription;

	public override void Init(DialogueSO so)
	{
		tutorialFeedDialogueSO = (TutorialFeedDialogueSO)so;
		if (tutorialFeedDialogueSO == null)
		{
			Debug.LogError(base.name + ": DialogueSO is of wrong type");
			return;
		}
		base.Init(so);
		base.OnShow += tutorialFeedDialogueSO.Triggered;
		base.OnClose += tutorialFeedDialogueSO.Finished;
	}

	protected override void Awake()
	{
		base.Awake();
		if (face == null)
		{
			Debug.LogError(base.name + ": Missing face");
			return;
		}
		face.enabled = false;
		if (slot1 == null)
		{
			Debug.LogError(base.name + ": Missing slot1");
			return;
		}
		slot1.enabled = false;
		if (slot2 == null)
		{
			Debug.LogError(base.name + ": Missing slot2");
			return;
		}
		slot2.enabled = false;
		if (contentHeadline == null)
		{
			Debug.LogError(base.name + ": Missing contentHeadline");
			return;
		}
		contentHeadline.text = "";
		if (contentDescription == null)
		{
			Debug.LogError(base.name + ": Missing contentDescription");
		}
		else
		{
			contentDescription.text = "";
		}
	}

	public void Start()
	{
		MilMoAnalyticsHandler.OpenTutorial(tutorialFeedDialogueSO.GetTutorialIdentifier());
	}

	public void OnDestroy()
	{
		if (tutorialFeedDialogueSO != null)
		{
			MilMoAnalyticsHandler.CloseTutorial(tutorialFeedDialogueSO.GetTutorialIdentifier());
		}
	}

	protected override async void RefreshHeader()
	{
		Image target = face;
		SetIcon(target, await tutorialFeedDialogueSO.GetFaceIcon());
		target = slot1;
		SetIcon(target, await tutorialFeedDialogueSO.GetSlot1Icon());
		target = slot2;
		SetIcon(target, await tutorialFeedDialogueSO.GetSlot2Icon());
	}

	protected override void RefreshBody()
	{
		if (contentHeadline != null)
		{
			contentHeadline.SetText(tutorialFeedDialogueSO.GetContentHeadline());
		}
		if (contentDescription != null)
		{
			contentDescription.SetText(tutorialFeedDialogueSO.GetContentDescription());
		}
	}

	protected override void RefreshFooter()
	{
		RefreshActions();
		EnableButtons(shouldEnable: true);
	}
}
