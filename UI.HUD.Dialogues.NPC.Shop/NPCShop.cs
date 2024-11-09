using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Network.types;
using Code.World.Inventory;
using Core.Utilities;
using TMPro;
using UI.Elements.Slot;
using UI.Inventory;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.HUD.Dialogues.NPC.Shop;

public class NPCShop : MonoBehaviour
{
	[Header("Currency")]
	[SerializeField]
	private Image currencyIcon;

	[SerializeField]
	private TMP_Text currencyText;

	private readonly List<Currency> _currencies = new List<Currency>();

	private Currency _currentCurrency;

	[Header("Elements")]
	[SerializeField]
	private NPCShopTabContent buyTab;

	[SerializeField]
	private NPCShopTabContent sellTab;

	private NPCShopTabContent _currentTab;

	[SerializeField]
	private AssetReference shopItemPrefab;

	private GameObject _shopItemObject;

	[SerializeField]
	private AmountDialogue buyAmountDialogue;

	[SerializeField]
	private AmountDialogue sellAmountDialogue;

	[Header("Events")]
	[SerializeField]
	private UnityEvent onTabSelected;

	[SerializeField]
	private UnityEvent onConfirm;

	[SerializeField]
	private UnityEvent onFail;

	private NPCShopTransactionHandler _transactionHandler;

	private readonly List<NPCShopEntry> _shopEntries = new List<NPCShopEntry>();

	private NPCShopEntry _selectedShopEntry;

	private UI.Inventory.Inventory _inventory;

	protected void Awake()
	{
		_transactionHandler = new NPCShopTransactionHandler(this);
		if (currencyIcon == null)
		{
			Debug.LogError(base.name + ": Could not create NPCShopTransactionHandler");
			return;
		}
		if (currencyIcon == null)
		{
			Debug.LogError(base.name + ": Missing currencyIcon");
			return;
		}
		if (currencyText == null)
		{
			Debug.LogError(base.name + ": Missing currencyText");
			return;
		}
		if (buyTab == null)
		{
			Debug.LogError(base.name + ": Missing buyTab");
			return;
		}
		_currentTab = buyTab;
		if (sellTab == null)
		{
			Debug.LogError(base.name + ": Missing sellTab");
			return;
		}
		if (buyAmountDialogue == null)
		{
			Debug.LogError(base.name + ": Missing buyAmountDialogue");
			return;
		}
		if (sellAmountDialogue == null)
		{
			Debug.LogError(base.name + ": Missing sellAmountDialogue");
			return;
		}
		if (!shopItemPrefab.RuntimeKeyIsValid())
		{
			Debug.LogError(base.name + ": Missing shop item prefab!");
			return;
		}
		_shopItemObject = shopItemPrefab.LoadAssetAsync<GameObject>().WaitForCompletion();
		if (!_shopItemObject)
		{
			Debug.LogError(base.name + ": Could not load shop item prefab!");
			return;
		}
		ClearShopItems();
		buyAmountDialogue.gameObject.SetActive(value: false);
		sellAmountDialogue.gameObject.SetActive(value: false);
	}

	protected void OnEnable()
	{
		_inventory = UI.Inventory.Inventory.GetPlayerInventory();
		if (_inventory != null)
		{
			_inventory.OnEntriesChanged += InventoryChanged;
		}
		if (buyTab != null)
		{
			NPCShopTabContent nPCShopTabContent = buyTab;
			nPCShopTabContent.OnEnabled = (UnityAction<NPCShopTabContent>)Delegate.Combine(nPCShopTabContent.OnEnabled, new UnityAction<NPCShopTabContent>(OnTabEnabled));
		}
		if (sellTab != null)
		{
			NPCShopTabContent nPCShopTabContent2 = sellTab;
			nPCShopTabContent2.OnEnabled = (UnityAction<NPCShopTabContent>)Delegate.Combine(nPCShopTabContent2.OnEnabled, new UnityAction<NPCShopTabContent>(OnTabEnabled));
		}
		if (_transactionHandler != null)
		{
			NPCShopTransactionHandler transactionHandler = _transactionHandler;
			transactionHandler.OnProcessStarted = (Action)Delegate.Combine(transactionHandler.OnProcessStarted, new Action(OnProcessStarted));
			NPCShopTransactionHandler transactionHandler2 = _transactionHandler;
			transactionHandler2.OnProcessCompleted = (Action<bool>)Delegate.Combine(transactionHandler2.OnProcessCompleted, new Action<bool>(OnProcessCompleted));
		}
	}

	protected void OnDisable()
	{
		if (_inventory != null)
		{
			_inventory.OnEntriesChanged -= InventoryChanged;
		}
		if (buyTab != null)
		{
			NPCShopTabContent nPCShopTabContent = buyTab;
			nPCShopTabContent.OnEnabled = (UnityAction<NPCShopTabContent>)Delegate.Remove(nPCShopTabContent.OnEnabled, new UnityAction<NPCShopTabContent>(OnTabEnabled));
		}
		if (sellTab != null)
		{
			NPCShopTabContent nPCShopTabContent2 = sellTab;
			nPCShopTabContent2.OnEnabled = (UnityAction<NPCShopTabContent>)Delegate.Remove(nPCShopTabContent2.OnEnabled, new UnityAction<NPCShopTabContent>(OnTabEnabled));
		}
		if (_transactionHandler != null)
		{
			NPCShopTransactionHandler transactionHandler = _transactionHandler;
			transactionHandler.OnProcessStarted = (Action)Delegate.Remove(transactionHandler.OnProcessStarted, new Action(OnProcessStarted));
			NPCShopTransactionHandler transactionHandler2 = _transactionHandler;
			transactionHandler2.OnProcessCompleted = (Action<bool>)Delegate.Remove(transactionHandler2.OnProcessCompleted, new Action<bool>(OnProcessCompleted));
		}
		ClearCurrencies();
	}

	private void OnTabEnabled(NPCShopTabContent tabContent)
	{
		if (_currentTab != tabContent)
		{
			onTabSelected?.Invoke();
		}
		_currentTab = tabContent;
		RefreshSelection();
		CloseAmountDialogues();
		SetCurrentCurrencyToTopItem();
	}

	public void Setup(int npcId, IList<InGameShopItem> items)
	{
		if (items == null)
		{
			return;
		}
		foreach (InGameShopItem item in items)
		{
			_shopEntries.Add(new NPCShopEntry(npcId, item));
		}
		InitializeCurrencies();
		if (buyAmountDialogue != null)
		{
			AmountDialogue amountDialogue = buyAmountDialogue;
			amountDialogue.OnConfirm = (UnityAction<NPCShopEntry, int>)Delegate.Combine(amountDialogue.OnConfirm, new UnityAction<NPCShopEntry, int>(OnAmountDialogueConfirm));
			AmountDialogue amountDialogue2 = buyAmountDialogue;
			amountDialogue2.OnClose = (UnityAction)Delegate.Combine(amountDialogue2.OnClose, new UnityAction(OnAmountDialogueClose));
		}
		if (sellAmountDialogue != null)
		{
			AmountDialogue amountDialogue3 = sellAmountDialogue;
			amountDialogue3.OnConfirm = (UnityAction<NPCShopEntry, int>)Delegate.Combine(amountDialogue3.OnConfirm, new UnityAction<NPCShopEntry, int>(OnAmountDialogueConfirm));
			AmountDialogue amountDialogue4 = sellAmountDialogue;
			amountDialogue4.OnClose = (UnityAction)Delegate.Combine(amountDialogue4.OnClose, new UnityAction(OnAmountDialogueClose));
		}
		RefreshInventoryConnections();
		RefreshAfforded();
		foreach (NPCShopEntry shopEntry in _shopEntries)
		{
			shopEntry.OnTemplateLoaded = (UnityAction<NPCShopEntry>)Delegate.Combine(shopEntry.OnTemplateLoaded, new UnityAction<NPCShopEntry>(RefreshShopItem));
			shopEntry.OnSelected = (UnityAction<NPCShopEntry>)Delegate.Combine(shopEntry.OnSelected, new UnityAction<NPCShopEntry>(OnItemSelected));
			shopEntry.OnDeselected = (UnityAction<NPCShopEntry>)Delegate.Combine(shopEntry.OnDeselected, new UnityAction<NPCShopEntry>(OnItemDeselected));
			shopEntry.LoadTemplate();
		}
	}

	private void RefreshShopItem(NPCShopEntry shopEntry)
	{
		if (shopEntry.ShopItem != null)
		{
			shopEntry.ShopItem.RefreshAmount();
			shopEntry.ShopItem.Show(!shopEntry.IsHidden);
		}
		else
		{
			if (shopEntry.IsHidden)
			{
				return;
			}
			bool forSale = shopEntry.ForSale;
			if ((!forSale || !(buyTab == null)) && (forSale || !(sellTab == null)))
			{
				Transform targetTransform = (forSale ? buyTab.GetTransform() : sellTab.GetTransform());
				shopEntry.ShopItem = CreateShopItem(targetTransform);
				if (!(shopEntry.ShopItem == null))
				{
					shopEntry.ShopItem.Setup(shopEntry, GetCurrency(shopEntry.CurrencyIdentifier));
				}
			}
		}
	}

	private void InitializeCurrencies()
	{
		foreach (IGrouping<string, NPCShopEntry> item in from item in _shopEntries
			group item by item.CurrencyIdentifier)
		{
			AddCurrency(item.Key);
		}
		foreach (Currency currency in _currencies)
		{
			currency.LoadIconTexture();
		}
		SetCurrentCurrencyToTopItem();
	}

	private void SetCurrentCurrencyToTopItem()
	{
		bool inBuyTab = _currentTab == buyTab;
		NPCShopEntry nPCShopEntry = _shopEntries.FirstOrDefault((NPCShopEntry entry) => entry.ForSale == inBuyTab && !entry.IsHidden);
		if (nPCShopEntry != null)
		{
			Currency currency = GetCurrency(nPCShopEntry.CurrencyIdentifier);
			if (currency != null)
			{
				SetCurrentCurrency(currency);
			}
		}
	}

	private Currency GetCurrency(string currencyIdentifier)
	{
		return _currencies.FirstOrDefault((Currency c) => string.Equals(c?.GetIdentifier() ?? "", currencyIdentifier, StringComparison.CurrentCultureIgnoreCase));
	}

	private void AddCurrency(string currencyIdentifier)
	{
		if (GetCurrency(currencyIdentifier) == null)
		{
			string text = currencyIdentifier.ToUpper();
			Currency currency = ((text == "GEM") ? new GemCurrency() : ((!(text == "COIN")) ? ((Currency)new ItemCurrency(_inventory, currencyIdentifier)) : ((Currency)new CoinCurrency())));
			Currency currency2 = currency;
			currency2.RegisterOnChange(UpdateCurrencyAmount);
			_currencies.Add(currency2);
		}
	}

	private void ClearCurrencies()
	{
		foreach (Currency currency in _currencies)
		{
			currency.UnregisterOnChange(UpdateCurrencyAmount);
		}
		_currencies.Clear();
	}

	private void UpdateCurrencyAmount(Currency currency)
	{
		if (_currentCurrency == currency)
		{
			SetCurrencyText(currency?.GetAmount().ToString());
		}
		if (buyAmountDialogue != null)
		{
			buyAmountDialogue.Recalculate();
		}
		if (sellAmountDialogue != null)
		{
			sellAmountDialogue.Recalculate();
		}
		RefreshAfforded();
	}

	private void SetCurrentCurrency(Currency currency)
	{
		_currentCurrency = currency;
		SetCurrencyText(_currentCurrency?.GetAmount().ToString());
		SetCurrencyIcon(_currentCurrency?.GetTexture2D());
	}

	private void SetCurrencyText(string amount)
	{
		if (!(currencyText == null))
		{
			currencyText.text = amount;
		}
	}

	private void SetCurrencyIcon(Texture2D texture2D)
	{
		if (!(currencyIcon == null))
		{
			currencyIcon.enabled = false;
			if (texture2D != null)
			{
				Core.Utilities.UI.SetIcon(currencyIcon, texture2D);
			}
		}
	}

	private void RefreshInventoryConnections()
	{
		foreach (NPCShopEntry shopEntry in _shopEntries)
		{
			RefreshInventoryConnection(shopEntry);
		}
	}

	private void RefreshInventoryConnection(NPCShopEntry shopEntry)
	{
		shopEntry.InventoryEntry = GetInventoryEntry(shopEntry.TemplateIdentifier);
	}

	private void RefreshAfforded()
	{
		foreach (IGrouping<string, NPCShopEntry> item in from item in _shopEntries
			group item by item.CurrencyIdentifier)
		{
			Currency currency = GetCurrency(item.Key);
			if (currency == null)
			{
				continue;
			}
			foreach (NPCShopEntry item2 in item)
			{
				RefreshAfforded(item2, currency);
			}
		}
	}

	private void RefreshAfforded(NPCShopEntry shopEntry, Currency currency)
	{
		shopEntry.SetAfforded(shopEntry.Price <= currency?.GetAmount());
	}

	private void RefreshShopItems()
	{
		foreach (NPCShopEntry shopEntry in _shopEntries)
		{
			RefreshShopItem(shopEntry);
		}
	}

	private void InventoryChanged()
	{
		RefreshInventoryConnections();
		RefreshAfforded();
		RefreshShopItems();
	}

	private void ClearShopItems()
	{
		if (buyTab != null)
		{
			foreach (Transform item in buyTab.GetTransform())
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
		if (!(sellTab != null))
		{
			return;
		}
		foreach (Transform item2 in sellTab.GetTransform())
		{
			UnityEngine.Object.Destroy(item2.gameObject);
		}
	}

	public NPCShopItem CreateShopItem(Transform targetTransform)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(_shopItemObject, targetTransform);
		if (!gameObject)
		{
			Debug.LogWarning(base.name + ": Unable to instantiate shop item!");
			return null;
		}
		NPCShopItem component = gameObject.GetComponent<NPCShopItem>();
		if (!component)
		{
			Debug.LogWarning(base.name + ": Unable to GetComponent NPCShopItem!");
			UnityEngine.Object.Destroy(gameObject);
			return null;
		}
		return component;
	}

	private ISlotItemEntry GetInventoryEntry(string identifier)
	{
		return _inventory.GetEntryByTemplateIdentifier("Item:" + identifier) as MilMo_InventoryEntry;
	}

	private void RefreshSelection()
	{
		foreach (NPCShopEntry item in _shopEntries.Where((NPCShopEntry e) => e != _selectedShopEntry))
		{
			if (item.ShopItem != null)
			{
				item.ShopItem.Deselect();
			}
		}
	}

	private void OnItemSelected(NPCShopEntry shopEntry)
	{
		_selectedShopEntry = shopEntry;
		Currency currency = GetCurrency(shopEntry.CurrencyIdentifier);
		if (currency != null)
		{
			RefreshSelection();
			SetCurrentCurrency(currency);
			if (shopEntry.ForSale)
			{
				buyAmountDialogue.Open(shopEntry, currency);
			}
			else
			{
				sellAmountDialogue.Open(shopEntry, currency);
			}
		}
	}

	private void OnItemDeselected(NPCShopEntry shopEntry)
	{
		if (_selectedShopEntry == shopEntry)
		{
			_selectedShopEntry = null;
			CloseAmountDialogues();
		}
	}

	private void CloseAmountDialogues()
	{
		buyAmountDialogue.Close();
		sellAmountDialogue.Close();
	}

	private void OnAmountDialogueClose()
	{
		if (_selectedShopEntry != null)
		{
			_selectedShopEntry.ShopItem.Deselect();
			_selectedShopEntry = null;
		}
		RefreshSelection();
	}

	private void OnAmountDialogueConfirm(NPCShopEntry shopItem, int amount)
	{
		if (_transactionHandler == null)
		{
			return;
		}
		if (_transactionHandler.IsProcessing)
		{
			onFail?.Invoke();
			return;
		}
		if (shopItem.NPCId < 1)
		{
			Debug.LogWarning("Attempting to trade in ingame shop when NPC is null");
			onFail?.Invoke();
			return;
		}
		onConfirm?.Invoke();
		if (shopItem.ForSale)
		{
			_transactionHandler.BuyItem(shopItem, amount);
		}
		else
		{
			_transactionHandler.SellItem(shopItem, amount);
		}
	}

	private void OnProcessStarted()
	{
		if (!(buyAmountDialogue == null) && !(sellAmountDialogue == null))
		{
			buyAmountDialogue.Activate(shouldActivate: false);
			sellAmountDialogue.Activate(shouldActivate: false);
		}
	}

	private void OnProcessCompleted(bool success)
	{
		if (!(buyAmountDialogue == null) && !(sellAmountDialogue == null))
		{
			if (success)
			{
				CloseAmountDialogues();
				InventoryChanged();
			}
			else
			{
				buyAmountDialogue.Activate(shouldActivate: true);
				sellAmountDialogue.Activate(shouldActivate: true);
				onFail?.Invoke();
			}
		}
	}
}
