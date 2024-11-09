using System;
using System.Collections;
using Code.Core.EventSystem;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Core;
using UnityEngine;

namespace UI.HUD.Dialogues.NPC.Shop;

public class NPCShopTransactionHandler
{
	private readonly NPCShop _shop;

	public Action OnProcessStarted;

	public Action<bool> OnProcessCompleted;

	private const int TIMEOUT_SECONDS = 10;

	private Coroutine _timeOut;

	private MilMo_GenericReaction _buyFailReaction;

	private MilMo_GenericReaction _buySuccessReaction;

	private MilMo_GenericReaction _sellFailReaction;

	private MilMo_GenericReaction _sellSuccessReaction;

	public bool IsProcessing => _timeOut != null;

	public NPCShopTransactionHandler(NPCShop shop)
	{
		_shop = shop;
	}

	private void StartProcessing()
	{
		StartTimeout();
		OnProcessStarted?.Invoke();
	}

	private void ProcessCompleted(bool success)
	{
		CancelTimeout();
		ClearReactions();
		OnProcessCompleted?.Invoke(success);
	}

	private void StartTimeout()
	{
		_timeOut = _shop.StartCoroutine(TimeWentOut());
	}

	private void CancelTimeout()
	{
		if (_timeOut != null)
		{
			_shop.StopCoroutine(_timeOut);
			_timeOut = null;
		}
	}

	private IEnumerator TimeWentOut()
	{
		yield return new WaitForSeconds(10f);
		Debug.LogWarning("Ingame shop buy/sell request timed out");
		ProcessCompleted(success: false);
	}

	public void BuyItem(NPCShopEntry shopItem, int amount)
	{
		StartProcessing();
		ListenForReplies();
		Singleton<GameNetwork>.Instance.RequestBuyInGameShopItem(shopItem.NPCId, shopItem.ItemId, (short)amount);
	}

	public void SellItem(NPCShopEntry shopItem, int amount)
	{
		StartProcessing();
		ListenForReplies();
		Singleton<GameNetwork>.Instance.RequestSellInGameShopItem(shopItem.NPCId, shopItem.ItemId, (short)amount);
	}

	private void ClearReactions()
	{
		MilMo_EventSystem.RemoveReaction(_buyFailReaction);
		_buyFailReaction = null;
		MilMo_EventSystem.RemoveReaction(_buySuccessReaction);
		_buySuccessReaction = null;
		MilMo_EventSystem.RemoveReaction(_sellFailReaction);
		_sellFailReaction = null;
		MilMo_EventSystem.RemoveReaction(_sellSuccessReaction);
		_sellSuccessReaction = null;
	}

	private void ListenForReplies()
	{
		_buyFailReaction = MilMo_EventSystem.Listen("npc_shop_buy_fail", BuyFailReaction);
		_buySuccessReaction = MilMo_EventSystem.Listen("npc_shop_buy_success", BuySuccessReaction);
		_sellFailReaction = MilMo_EventSystem.Listen("npc_shop_sell_fail", SellFailReaction);
		_sellSuccessReaction = MilMo_EventSystem.Listen("npc_shop_sell_success", SellSuccessReaction);
	}

	private void BuySuccessReaction(object msgAsObj)
	{
		if (msgAsObj is ServerNPCBuySuccess)
		{
			ProcessCompleted(success: true);
		}
	}

	private void BuyFailReaction(object msgAsObj)
	{
		if (msgAsObj is ServerNPCBuyFail)
		{
			ProcessCompleted(success: false);
		}
	}

	private void SellSuccessReaction(object msgAsObj)
	{
		if (msgAsObj is ServerNPCSellSuccess)
		{
			ProcessCompleted(success: true);
		}
	}

	private void SellFailReaction(object msgAsObj)
	{
		if (msgAsObj is ServerNPCSellFail)
		{
			ProcessCompleted(success: false);
		}
	}
}
