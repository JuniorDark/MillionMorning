using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.EventSystem;
using Code.Core.GUI.Widget;
using Code.Core.Items;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Core;
using Core.Analytics;
using UnityEngine;

namespace Code.World.CharacterShop.RemoteShop;

public sealed class MilMo_RemoteShop
{
	public delegate void DataArrived(bool gotData);

	public delegate void BuyCallback(MilMo_ShopItem item);

	public delegate void BuyFailedCallback(MilMo_ShopItem item, sbyte failCode);

	public delegate void BuyTokenArrivedCallback();

	private DataArrived _dataCallback;

	private readonly MilMo_GenericReaction _shopBuyReaction;

	private readonly MilMo_GenericReaction _shopBuyFailReaction;

	private MilMo_GenericReaction _requestCategoriesReaction;

	private MilMo_GenericReaction _requestItemsReaction;

	private MilMo_TimerEvent _timeoutReaction;

	private int _numberOfItemMessages = int.MaxValue;

	private int _currentReceivedItemMessages;

	private bool _isBuying;

	private MilMo_ShopItem _currentBuyItem;

	private readonly BuyCallback _buySuccessCallback;

	private readonly BuyFailedCallback _buyFailedCallback;

	private readonly List<ServerShopItems> _receivedShopItems = new List<ServerShopItems>();

	public MilMo_ShopCategory Root { get; }

	public bool HaveData { get; private set; }

	public List<MilMo_ShopCategory> MainCategories => Root.SubCategories;

	public bool IsGift { get; private set; }

	public MilMo_RemoteShop(BuyCallback success, BuyFailedCallback fail)
	{
		Root = new MilMo_ShopCategory();
		_shopBuyReaction = MilMo_EventSystem.Listen("shop_buy_success", BuySuccess);
		_shopBuyFailReaction = MilMo_EventSystem.Listen("shop_buy_fail", BuyFailed);
		_shopBuyReaction.Repeating = true;
		_shopBuyFailReaction.Repeating = true;
		_buySuccessCallback = success;
		_buyFailedCallback = fail;
	}

	public void Destroy()
	{
		MilMo_EventSystem.RemoveReaction(_shopBuyReaction);
		MilMo_EventSystem.RemoveReaction(_shopBuyFailReaction);
		MilMo_EventSystem.RemoveReaction(_requestCategoriesReaction);
		MilMo_EventSystem.RemoveReaction(_requestItemsReaction);
		MilMo_EventSystem.RemoveTimerEvent(_timeoutReaction);
	}

	public void RequestRemoteData(DataArrived callback)
	{
		_dataCallback = callback;
		_requestCategoriesReaction = MilMo_EventSystem.Listen("shop_categories", ReceivedShopCategories);
		_requestItemsReaction = MilMo_EventSystem.Listen("shop_items", ReceivedShopItems);
		_requestItemsReaction.Repeating = true;
		_timeoutReaction = MilMo_EventSystem.At(20f, RequestTimeOut);
		_receivedShopItems.Clear();
		Singleton<GameNetwork>.Instance.RequestShopData();
	}

	public void Buy(MilMo_ShopItem item, bool useCoins)
	{
		IsGift = false;
		Buy(item, useCoins, null);
	}

	public void BuyAsGift(MilMo_ShopItem item, bool useCoins, string avatarName)
	{
		IsGift = true;
		Buy(item, useCoins, avatarName);
	}

	private void Buy(MilMo_ShopItem item, bool useCoins, string avatarName)
	{
		if (_isBuying)
		{
			return;
		}
		if (Root.GetItemWithId(item.Id) == null)
		{
			Debug.LogWarning("Trying to buy item (" + item.Id + ":" + item.Item.Template.Identifier + ") that does not exist in the shop.");
			return;
		}
		_isBuying = true;
		_currentBuyItem = item;
		sbyte itemGender = 0;
		switch (item.Item.UseableGender)
		{
		case MilMo_Item.ItemGender.Boy:
			itemGender = 1;
			break;
		case MilMo_Item.ItemGender.Girl:
			itemGender = 2;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case MilMo_Item.ItemGender.Both:
			break;
		}
		Singleton<GameNetwork>.Instance.RequestBuyItem(item.Id, item.Item.ModifiersAsList, itemGender, useCoins, (avatarName == null) ? null : new NullableString(avatarName));
	}

	private void BuyFailed(object messageAsObject)
	{
		ServerBuyItemFail serverBuyItemFail = messageAsObject as ServerBuyItemFail;
		if (serverBuyItemFail != null && serverBuyItemFail.getItemId() != _currentBuyItem.Id)
		{
			Debug.LogWarning("Got buy failed message with unknown id " + serverBuyItemFail.getItemId() + " was expecting " + _currentBuyItem.Id);
		}
		_isBuying = false;
		_buyFailedCallback?.Invoke(_currentBuyItem, serverBuyItemFail?.getFailCode() ?? 0);
	}

	private void BuySuccess(object messageAsObject)
	{
		if (messageAsObject is ServerBuyItemSuccess serverBuyItemSuccess)
		{
			if (serverBuyItemSuccess.getItemId() != _currentBuyItem.Id)
			{
				Debug.LogWarning("Got buy success message with unknown id " + serverBuyItemSuccess.getItemId() + " was expecting " + _currentBuyItem.Id);
			}
			if (serverBuyItemSuccess.getIsGift() == 1)
			{
				MilMo_QuickInfoDialog.CreateOkQuickInfoDialog(MilMo_Localization.GetLocString("Generic_4619"), MilMo_Localization.GetLocString("CharacterShop_7285"));
			}
		}
		_isBuying = false;
		_buySuccessCallback?.Invoke(_currentBuyItem);
		Analytics.ItemAcquired(_currentBuyItem.GetAmount(), _currentBuyItem.Item.Identifier, _currentBuyItem.Item.Template.BagCategory);
	}

	private void ReceivedShopCategories(object msg)
	{
		if (msg is ServerShopCategories serverShopCategories)
		{
			ShopCategory root = serverShopCategories.getRoot();
			Root.Read(root);
			_numberOfItemMessages = serverShopCategories.getNumberOfItemMessages();
		}
		HandleShopItems();
	}

	private void ReceivedShopItems(object msg)
	{
		if (msg is ServerShopItems item)
		{
			_receivedShopItems.Add(item);
		}
		_currentReceivedItemMessages++;
		HandleShopItems();
	}

	private void HandleShopItems()
	{
		if (_currentReceivedItemMessages < _numberOfItemMessages)
		{
			return;
		}
		MilMo_EventSystem.RemoveTimerEvent(_timeoutReaction);
		int numberOfCategoriesLoaded = 0;
		int numberOfCategoriesToLoad = _receivedShopItems.Select((ServerShopItems items) => items.getItems().GetParentId()).Count((int parentId) => Root.GetCategoryWithId(parentId) != null);
		foreach (ServerShopItems receivedShopItem in _receivedShopItems)
		{
			int parentId2 = receivedShopItem.getItems().GetParentId();
			Root.GetCategoryWithId(parentId2)?.ReadItems(receivedShopItem.getItems(), delegate
			{
				numberOfCategoriesLoaded++;
				if (numberOfCategoriesLoaded >= numberOfCategoriesToLoad)
				{
					HaveData = true;
					_dataCallback?.Invoke(gotData: true);
				}
			});
		}
	}

	private void RequestTimeOut()
	{
		Debug.LogWarning("Shop data request timed out");
		MilMo_EventSystem.RemoveReaction(_requestCategoriesReaction);
		MilMo_EventSystem.RemoveReaction(_requestItemsReaction);
		_dataCallback?.Invoke(gotData: false);
	}
}
