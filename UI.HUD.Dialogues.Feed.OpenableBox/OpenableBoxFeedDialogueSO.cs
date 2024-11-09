using System.Collections.Generic;
using Code.Core.Items;
using Code.World.Feeds;
using Localization;

namespace UI.HUD.Dialogues.Feed.OpenableBox;

public class OpenableBoxFeedDialogueSO : FeedDialogueSO
{
	private string _iconPathOpen;

	private string _iconPathClosed;

	private string _headline;

	private List<MilMo_BoxLoot> _receivedItems;

	public override string GetAddressableKey()
	{
		return "FeedDialogueOpenableBox";
	}

	public override int GetPriority()
	{
		return 5;
	}

	public string GetIconPathOpen()
	{
		return _iconPathOpen;
	}

	public string GetIconPathClosed()
	{
		return _iconPathClosed;
	}

	public string GetHeadline()
	{
		return _headline;
	}

	public List<MilMo_BoxLoot> GetReceivedItems()
	{
		return _receivedItems;
	}

	public int LootCount()
	{
		return _receivedItems.Count;
	}

	public bool Init(List<MilMo_BoxLoot> receivedItems, IMilMo_OpenableBox box)
	{
		box.PostOpenEvent();
		_iconPathOpen = box.IconPathOpen;
		_iconPathClosed = box.IconPathClosed;
		_receivedItems = receivedItems;
		return true;
	}

	public override List<DialogueButtonInfo> GetActions()
	{
		LocalizedStringWithArgument label = new LocalizedStringWithArgument("Generic_OK");
		return new List<DialogueButtonInfo>
		{
			new DialogueButtonInfo(Confirm, label, isDefault: true)
		};
	}

	private void Confirm()
	{
		DialogueWindow.Close();
	}
}
