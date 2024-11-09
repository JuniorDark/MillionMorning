using System.Threading.Tasks;
using Code.Core.Items;
using Code.Core.Network.types;
using Code.Core.Template;
using Core;
using UI.Elements.Slot;
using UnityEngine;
using UnityEngine.Events;

namespace UI.HUD.Dialogues.NPC.Shop;

public class NPCShopEntry
{
	public readonly int NPCId;

	public readonly int ItemId;

	public readonly int Price;

	public readonly bool ForSale;

	public readonly string TemplateIdentifier;

	public readonly string CurrencyIdentifier;

	public MilMo_ItemTemplate Template;

	public UnityAction<NPCShopEntry> OnTemplateLoaded;

	public ISlotItemEntry InventoryEntry;

	private bool _afforded;

	public UnityAction<NPCShopEntry> OnSelected;

	public UnityAction<NPCShopEntry> OnDeselected;

	public NPCShopItem ShopItem;

	private bool HaveItem
	{
		get
		{
			if (InventoryEntry != null)
			{
				return InventoryEntry.GetAmount() > 0;
			}
			return false;
		}
	}

	private bool IsUnique => Template?.IsUnique ?? false;

	public bool IsHidden
	{
		get
		{
			if (!ForSale)
			{
				return !HaveItem;
			}
			if (IsUnique)
			{
				return HaveItem;
			}
			return false;
		}
	}

	public bool IsEnabled
	{
		get
		{
			if (ForSale)
			{
				return _afforded;
			}
			return true;
		}
	}

	public void SetAfforded(bool isAfforded)
	{
		_afforded = isAfforded;
		if (ShopItem != null)
		{
			ShopItem.SetEnabled(isAfforded);
		}
	}

	public NPCShopEntry(int npcId, InGameShopItem item)
	{
		NPCId = npcId;
		ItemId = item.GetId();
		Price = item.GetPrice();
		ForSale = item.GetForSale() != 0;
		TemplateIdentifier = item.GetItem();
		CurrencyIdentifier = item.GetCurrency();
	}

	private async Task LoadTemplateAsync()
	{
		MilMo_Template milMo_Template = await Singleton<MilMo_TemplateContainer>.Instance.GetTemplateAsync("Item:" + TemplateIdentifier);
		if (milMo_Template == null)
		{
			Debug.LogWarning("Failed to get template Item:" + TemplateIdentifier + " for ingame shop.");
			return;
		}
		if (!(milMo_Template is MilMo_ItemTemplate template))
		{
			Debug.LogWarning("Got wrong template type when loading template Item:" + TemplateIdentifier + " for ingame shop.");
			return;
		}
		Template = template;
		OnTemplateLoaded?.Invoke(this);
	}

	public void LoadTemplate()
	{
		LoadTemplateAsync();
	}
}
