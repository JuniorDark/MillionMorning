using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.Dialogues.Feed.Item;

public class ItemFeedDialogueWindow : FeedDialogueWindow
{
	[Header("Dialogue SO")]
	[SerializeField]
	private ItemFeedDialogueSO itemFeedDialogueSO;

	[Header("Header")]
	[SerializeField]
	private Image icon;

	[SerializeField]
	private TMP_Text headline;

	[SerializeField]
	private TMP_Text description;

	[Header("Body")]
	[SerializeField]
	private TMP_Text contentHeadline;

	[SerializeField]
	private TMP_Text contentDescription;

	public override void Init(DialogueSO so)
	{
		itemFeedDialogueSO = (ItemFeedDialogueSO)so;
		if (itemFeedDialogueSO == null)
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

	public override void Close()
	{
		CreateFlyingClone();
		base.Close();
	}

	private void CreateFlyingClone()
	{
		SpawnFlyingIcon(icon, base.transform.parent, itemFeedDialogueSO.GetObjectDestination());
	}

	protected override async void RefreshHeader()
	{
		if (icon != null)
		{
			Image target = icon;
			SetIcon(target, await itemFeedDialogueSO.GetIconAsync(itemFeedDialogueSO.GetIconPath()));
		}
		if (headline != null)
		{
			headline.SetText(itemFeedDialogueSO.GetHeadline());
		}
		if (description != null)
		{
			description.SetText(itemFeedDialogueSO.GetDescription());
		}
	}

	protected override void RefreshBody()
	{
		if (contentHeadline != null)
		{
			contentHeadline.SetText(itemFeedDialogueSO.GetContentHeadline());
		}
		if (contentDescription != null)
		{
			contentDescription.SetText(itemFeedDialogueSO.GetContentDescription());
		}
	}

	protected override void RefreshFooter()
	{
		RefreshActions();
		EnableButtons(shouldEnable: true);
	}
}
