using System.Collections.Generic;
using Code.Core.Items;
using Code.World.Feeds;
using Core.Utilities;
using TMPro;
using UI.Elements.Slot;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace UI.HUD.Dialogues.Feed.OpenableBox;

public class FeedDialogueOpenableBoxWindow : FeedDialogueWindow
{
	[Header("Dialogue SO")]
	[SerializeField]
	private OpenableBoxFeedDialogueSO boxFeedDialogueSO;

	[SerializeField]
	private AssetReference slotPrefab;

	[SerializeField]
	private Image boxIcon;

	[SerializeField]
	private TMP_Text caption;

	[SerializeField]
	private Transform grid;

	[SerializeField]
	private Button button;

	[SerializeField]
	private TMP_Text buttonText;

	private readonly List<OpenableBoxSlot> _loot = new List<OpenableBoxSlot>();

	private Slot _slot;

	public override void Init(DialogueSO so)
	{
		boxFeedDialogueSO = (OpenableBoxFeedDialogueSO)so;
		if (boxFeedDialogueSO == null)
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
		if (boxIcon == null)
		{
			Debug.LogError(base.name + ": Missing icon");
			return;
		}
		boxIcon.enabled = false;
		if (caption == null)
		{
			Debug.LogError(base.name + ": Missing caption");
			return;
		}
		caption.text = "";
		if (grid == null)
		{
			Debug.LogError(base.name + ": Missing grid");
		}
		else if (button == null)
		{
			Debug.LogError(base.name + ": Missing button");
		}
		else if (buttonText == null)
		{
			Debug.LogError(base.name + ": Missing buttonText");
		}
		else
		{
			DestroyChildren();
		}
	}

	private void DestroyChildren()
	{
		for (int i = 0; i < grid.childCount; i++)
		{
			Transform child = grid.GetChild(i);
			if ((bool)child)
			{
				Object.Destroy(child.gameObject);
			}
		}
	}

	private void OnDestroy()
	{
		DestroyChildren();
	}

	protected override async void RefreshHeader()
	{
		if (boxIcon != null)
		{
			Image target = boxIcon;
			SetIcon(target, await boxFeedDialogueSO.GetIconAsync(boxFeedDialogueSO.GetIconPathOpen()));
		}
		if (caption != null)
		{
			caption.SetText(boxFeedDialogueSO.GetHeadline());
		}
	}

	protected override void RefreshBody()
	{
		if (_loot.Count == boxFeedDialogueSO.LootCount())
		{
			return;
		}
		foreach (MilMo_BoxLoot receivedItem in boxFeedDialogueSO.GetReceivedItems())
		{
			InitLoot(receivedItem);
		}
	}

	private void InitLoot(MilMo_BoxLoot loot)
	{
		if (loot != null)
		{
			MilMo_Item item = loot.Item;
			int amount = loot.Amount;
			OpenableBoxSlot openableBoxSlot = Instantiator.Instantiate<OpenableBoxSlot>(slotPrefab, grid);
			openableBoxSlot.Init(item, amount);
			_loot.Add(openableBoxSlot);
		}
	}

	protected override void RefreshFooter()
	{
		RefreshActions();
		EnableButtons(shouldEnable: true);
	}
}
