using System.Collections.Generic;
using System.Linq;
using Code.Core.EventSystem;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Core;
using Core.Analytics;
using Localization;
using UI.HUD.Dialogues.NPC.Shop;
using UnityEngine;

namespace UI.HUD.Dialogues.NPC.Message;

public class OfferShopNPCMessageDialogueSO : NPCMessageDialogueSO
{
	private MilMo_TimerEvent _timeOut;

	private int _npcId;

	private string _actorName;

	private string _portraitKey;

	public override void Init(NPCMessageData npcMessageData)
	{
		base.Init(npcMessageData);
		_npcId = npcMessageData.GetNpcId();
		_actorName = npcMessageData.GetActorName();
		_portraitKey = npcMessageData.GetPortraitKey();
	}

	private void Confirm()
	{
		OpenNPCShop();
	}

	private void OpenNPCShop()
	{
		MilMo_GenericReaction waitingForShopItems = MilMo_EventSystem.Listen("npc_shop_items", ShopItemsReceived);
		_timeOut = MilMo_EventSystem.At(8f, delegate
		{
			Debug.LogWarning($"NPC shop items timed out for npc with id {_npcId}.");
			MilMo_EventSystem.RemoveReaction(waitingForShopItems);
			Fail();
		});
		Singleton<GameNetwork>.Instance.AcceptEnterNpcShop(_npcId);
	}

	private void Fail()
	{
		DialogueWindow.Close();
	}

	private void ShopItemsReceived(object itemsAsObject)
	{
		MilMo_EventSystem.RemoveTimerEvent(_timeOut);
		_timeOut = null;
		if (!(itemsAsObject is ServerNPCShopItems serverNPCShopItems))
		{
			Fail();
			return;
		}
		Analytics.NPCStoreOpened();
		DialogueSpawner.SpawnShopDialogue(new NPCShopMessageData(_npcId, _actorName, _portraitKey, serverNPCShopItems.GetInGameShopItems().ToList()));
		DialogueWindow.Close();
	}

	private void Cancel()
	{
		DialogueWindow.Close();
	}

	public override List<DialogueButtonInfo> GetActions()
	{
		return new List<DialogueButtonInfo>
		{
			new DialogueButtonInfo(Cancel, new LocalizedStringWithArgument("Generic_Later")),
			new DialogueButtonInfo(Confirm, new LocalizedStringWithArgument("Generic_Yes"), isDefault: true)
		};
	}
}
