using System.Collections.Generic;
using Code.Core.Items;
using Code.Core.ResourceSystem;
using Code.World.Feeds;
using Localization;
using UI.HUD.BagButton;
using UnityEngine;

namespace UI.HUD.Dialogues.Feed.Item;

[CreateAssetMenu(menuName = "Dialogues/ItemFeedDialogue", fileName = "ItemFeedDialogueSO", order = 0)]
public class ItemFeedDialogueSO : FeedDialogueSO
{
	[SerializeField]
	protected string headline;

	[SerializeField]
	protected string description;

	[SerializeField]
	protected string iconPath;

	[SerializeField]
	protected string contentHeadline;

	[SerializeField]
	protected string contentDescription;

	public override string GetAddressableKey()
	{
		return "ItemFeedDialogueWindow";
	}

	public override int GetPriority()
	{
		return 5;
	}

	public void Init(MilMo_ItemTemplate itemTemplate)
	{
		headline = MilMo_FeedExclamations.GetExclamation().String;
		description = MilMo_Localization.GetLocString("Items_4601").String;
		if (!string.IsNullOrEmpty(itemTemplate.FeedEventIngame.String))
		{
			description = itemTemplate.FeedEventIngame.String;
		}
		iconPath = itemTemplate.IconPath;
		contentHeadline = itemTemplate.DisplayName.String;
		contentDescription = itemTemplate.FeedDescriptionIngame.String;
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

	public string GetContentHeadline()
	{
		return contentHeadline;
	}

	public string GetContentDescription()
	{
		return contentDescription;
	}

	public override Transform GetObjectDestination()
	{
		return Object.FindObjectOfType<UI.HUD.BagButton.BagButton>()?.transform;
	}

	private void Confirm()
	{
		DialogueWindow.Close();
	}

	public override List<DialogueButtonInfo> GetActions()
	{
		return new List<DialogueButtonInfo>
		{
			new DialogueButtonInfo(Confirm, new LocalizedStringWithArgument("Generic_Okay"), isDefault: true)
		};
	}

	public override bool CanBeShown()
	{
		return true;
	}
}
