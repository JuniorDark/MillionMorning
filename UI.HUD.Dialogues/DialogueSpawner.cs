using System;
using System.Collections.Generic;
using Code.Core.BuddyBackend;
using Code.Core.Items;
using Code.Core.Sound;
using Code.World.Achievements;
using Code.World.Feeds;
using Code.World.Tutorial;
using Core;
using Core.GameEvent;
using Core.Utilities;
using Localization;
using UI.HUD.Dialogues.ClassChoice;
using UI.HUD.Dialogues.Feed.Item;
using UI.HUD.Dialogues.Feed.Medal;
using UI.HUD.Dialogues.Feed.OpenableBox;
using UI.HUD.Dialogues.Feed.Tutorial;
using UI.HUD.Dialogues.Modal;
using UI.HUD.Dialogues.NPC;
using UI.HUD.Dialogues.NPC.Message;
using UI.HUD.Dialogues.NPC.Shop;
using UI.HUD.Dialogues.NPC.Travel;
using UI.HUD.QuickInfo;
using UI.Sprites;
using UnityEngine;
using UnityEngine.Events;

namespace UI.HUD.Dialogues;

public static class DialogueSpawner
{
	public static void Spawn(DialogueSO so)
	{
		if (!(so == null) && !(Singleton<DialogueManager>.Instance == null))
		{
			DialogueWindow dialogueWindow = Instantiator.Instantiate<DialogueWindow>(so.GetAddressableKey(), Singleton<DialogueManager>.Instance.transform);
			dialogueWindow.gameObject.SetActive(value: false);
			dialogueWindow.Init(so);
			GameEvent.SpawnDialogueEvent.RaiseEvent(dialogueWindow);
		}
	}

	public static void SpawnQuickInfo(QuickInfoSO so)
	{
		Debug.Log("SpawnQuickInfo");
		if (!(so == null))
		{
			Singleton<QuickInfoManager>.Instance.AddQuickInfo(so);
		}
	}

	public static void SpawnMedalAcquired(MilMo_AchievementTemplate template)
	{
		MedalFeedDialogueSO medalFeedDialogueSO = ScriptableObject.CreateInstance<MedalFeedDialogueSO>();
		medalFeedDialogueSO.Init(template);
		Spawn(medalFeedDialogueSO);
	}

	public static void SpawnImportantItemAcquired(MilMo_ItemTemplate template)
	{
		ItemFeedDialogueSO itemFeedDialogueSO = ScriptableObject.CreateInstance<ItemFeedDialogueSO>();
		itemFeedDialogueSO.Init(template);
		Spawn(itemFeedDialogueSO);
	}

	public static void SpawnTutorialDialogue(IMilMo_Tutorial tutorial)
	{
		Debug.LogWarning($"Spawning tutorial: {tutorial.GetTemplate().Headline}");
		TutorialFeedDialogueSO tutorialFeedDialogueSO = ScriptableObject.CreateInstance<TutorialFeedDialogueSO>();
		tutorialFeedDialogueSO.Init(tutorial);
		Spawn(tutorialFeedDialogueSO);
	}

	public static void SpawnOpenableBoxDialogue(List<MilMo_BoxLoot> receivedItems, IMilMo_OpenableBox box)
	{
		OpenableBoxFeedDialogueSO openableBoxFeedDialogueSO = ScriptableObject.CreateInstance<OpenableBoxFeedDialogueSO>();
		openableBoxFeedDialogueSO.Init(receivedItems, box);
		Spawn(openableBoxFeedDialogueSO);
	}

	public static void SpawnShopDialogue(NPCShopMessageData npcShopMessageData)
	{
		NPCShopDialogueSO nPCShopDialogueSO = ScriptableObject.CreateInstance<NPCShopDialogueSO>();
		nPCShopDialogueSO.Init(npcShopMessageData);
		Spawn(nPCShopDialogueSO);
	}

	public static void SpawnNPCTravelDialogue(NPCTravelMessageData npcTravelMessageData)
	{
		NPCTravelDialogueSO nPCTravelDialogueSO = ScriptableObject.CreateInstance<NPCTravelDialogueSO>();
		nPCTravelDialogueSO.Init(npcTravelMessageData);
		Spawn(nPCTravelDialogueSO);
	}

	public static void SpawnNPCMessageDialogue(NPCMessageData npcMessageData)
	{
		NPCMessageDialogueSO messageNPCDialogueSO = GetMessageNPCDialogueSO(npcMessageData.GetMessageType());
		messageNPCDialogueSO.Init(npcMessageData);
		Spawn(messageNPCDialogueSO);
	}

	private static NPCMessageDialogueSO GetMessageNPCDialogueSO(NpcMessageTypes? messageType)
	{
		return messageType switch
		{
			NpcMessageTypes.OfferQuest => ScriptableObject.CreateInstance<OfferQuestNPCMessageDialogueSO>(), 
			NpcMessageTypes.OfferShop => ScriptableObject.CreateInstance<OfferShopNPCMessageDialogueSO>(), 
			NpcMessageTypes.MemberTeleport => ScriptableObject.CreateInstance<MemberTeleportNPCMessageDialogueSO>(), 
			NpcMessageTypes.EnterCharbuilder => ScriptableObject.CreateInstance<EnterCharbuilderNPCMessageDialogueSO>(), 
			_ => ScriptableObject.CreateInstance<OkNPCMessageDialogueSO>(), 
		};
	}

	public static void SpawnGoToShopDialogue(EnterShopModalMessageData modalMessageData)
	{
		EnterShopModalDialogueSO enterShopModalDialogueSO = ScriptableObject.CreateInstance<EnterShopModalDialogueSO>();
		enterShopModalDialogueSO.Init(modalMessageData);
		Spawn(enterShopModalDialogueSO);
	}

	public static void SpawnWarningModalDialogue(LocalizedStringWithArgument caption, LocalizedStringWithArgument message)
	{
		Debug.LogWarning("SpawnWarningModal: " + caption.GetMessage() + ":" + message.GetMessage());
		AddressableSpriteLoader spriteReference = new AddressableSpriteLoader("WarningIcon");
		SpawnOkModal(caption, message, spriteReference, null);
	}

	public static void SpawnErrorModalDialogue(LocalizedStringWithArgument caption, LocalizedStringWithArgument message)
	{
		Debug.LogError("SpawnErrorModal: " + caption.GetMessage() + ":" + message.GetMessage());
		AddressableSpriteLoader spriteReference = new AddressableSpriteLoader("ErrorIcon");
		SpawnOkModal(caption, message, spriteReference, null);
	}

	public static void SpawnKeepVideoOptionsModal()
	{
		DialogueButtonInfo confirm = new DialogueButtonInfo(GameEvent.OnVideoApplyEvent.RaiseEvent, new LocalizedStringWithArgument("Generic_Yes"));
		DialogueButtonInfo cancel = new DialogueButtonInfo(GameEvent.OnVideoRevertEvent.RaiseEvent, new LocalizedStringWithArgument("Generic_No"), isDefault: true);
		ModalMessageData modalMessageData = new ModalMessageData(new LocalizedStringWithArgument("Options_413"), new LocalizedStringWithArgument("Options_414"), confirm, cancel, null, null, 7);
		ModalDialogueSO modalDialogueSO = ScriptableObject.CreateInstance<ModalDialogueSO>();
		modalDialogueSO.Init(modalMessageData);
		Spawn(modalDialogueSO);
	}

	public static void SpawnOkModal(LocalizedStringWithArgument caption, LocalizedStringWithArgument message, IHaveSprite spriteReference, Action confirm, int lifetime = 0)
	{
		DialogueButtonInfo confirm2 = new DialogueButtonInfo((confirm != null) ? new UnityAction(confirm.Invoke) : null, new LocalizedStringWithArgument("Generic_OK"), isDefault: true);
		ModalMessageData modalMessageData = new ModalMessageData(caption, message, confirm2, null, null, spriteReference, lifetime);
		ModalDialogueSO modalDialogueSO = ScriptableObject.CreateInstance<ModalDialogueSO>();
		modalDialogueSO.Init(modalMessageData);
		Spawn(modalDialogueSO);
	}

	public static void SpawnOkCancelModal(string caption, string message, IHaveSprite spriteReference, Action confirm, Action cancel, int lifetime = 0)
	{
		DialogueButtonInfo confirm2 = new DialogueButtonInfo((confirm != null) ? new UnityAction(confirm.Invoke) : null, new LocalizedStringWithArgument("Generic_OK"), isDefault: true);
		DialogueButtonInfo cancel2 = new DialogueButtonInfo((cancel != null) ? new UnityAction(cancel.Invoke) : null, new LocalizedStringWithArgument("Generic_Cancel"));
		ModalMessageData modalMessageData = new ModalMessageData(new LocalizedStringWithArgument(caption), new LocalizedStringWithArgument(message), confirm2, cancel2, null, spriteReference, lifetime);
		ModalDialogueSO modalDialogueSO = ScriptableObject.CreateInstance<ModalDialogueSO>();
		modalDialogueSO.Init(modalMessageData);
		Spawn(modalDialogueSO);
	}

	public static void SpawnYesNoModal(string captionIdentifier, string messageIdentifier, IHaveSprite spriteReference, Action confirm, Action cancel, int lifetime = 0)
	{
		DialogueButtonInfo confirm2 = new DialogueButtonInfo((confirm != null) ? new UnityAction(confirm.Invoke) : null, new LocalizedStringWithArgument("Generic_Yes"));
		DialogueButtonInfo cancel2 = new DialogueButtonInfo((cancel != null) ? new UnityAction(cancel.Invoke) : null, new LocalizedStringWithArgument("Generic_No"), isDefault: true);
		ModalMessageData modalMessageData = new ModalMessageData(new LocalizedStringWithArgument(captionIdentifier), new LocalizedStringWithArgument(messageIdentifier), confirm2, cancel2, null, spriteReference, lifetime);
		ModalDialogueSO modalDialogueSO = ScriptableObject.CreateInstance<ModalDialogueSO>();
		modalDialogueSO.Init(modalMessageData);
		Spawn(modalDialogueSO);
	}

	public static void SpawnAcceptIgnoreLaterModalDialogue(string captionIdentifier, string messageIdentifier, Action accept, Action later, Action ignore, IHaveSprite spriteReference)
	{
		DialogueButtonInfo confirm = new DialogueButtonInfo((accept != null) ? new UnityAction(accept.Invoke) : null, new LocalizedStringWithArgument("Messenger_FriendCard_317"), isDefault: true);
		DialogueButtonInfo alternate = new DialogueButtonInfo((later != null) ? new UnityAction(later.Invoke) : null, new LocalizedStringWithArgument("Messenger_FriendCard_334"));
		DialogueButtonInfo cancel = new DialogueButtonInfo((ignore != null) ? new UnityAction(ignore.Invoke) : null, new LocalizedStringWithArgument("Messenger_FriendCard_Decline"));
		ModalMessageData modalMessageData = new ModalMessageData(new LocalizedStringWithArgument(captionIdentifier), new LocalizedStringWithArgument(messageIdentifier), confirm, cancel, alternate, spriteReference);
		ModalDialogueSO modalDialogueSO = ScriptableObject.CreateInstance<ModalDialogueSO>();
		modalDialogueSO.Init(modalMessageData);
		Spawn(modalDialogueSO);
	}

	public static void SpawnAcceptDeclineModalDialogue(string captionIdentifier, string messageIdentifier, Action accept, Action ignore, IHaveSprite spriteReference)
	{
		DialogueButtonInfo confirm = new DialogueButtonInfo((accept != null) ? new UnityAction(accept.Invoke) : null, new LocalizedStringWithArgument("Messenger_FriendCard_317"), isDefault: true);
		DialogueButtonInfo cancel = new DialogueButtonInfo((ignore != null) ? new UnityAction(ignore.Invoke) : null, new LocalizedStringWithArgument("Messenger_FriendCard_Decline"));
		ModalMessageData modalMessageData = new ModalMessageData(new LocalizedStringWithArgument(captionIdentifier), new LocalizedStringWithArgument(messageIdentifier), confirm, cancel, null, spriteReference);
		ModalDialogueSO modalDialogueSO = ScriptableObject.CreateInstance<ModalDialogueSO>();
		modalDialogueSO.Init(modalMessageData);
		Spawn(modalDialogueSO);
	}

	public static void SpawnReplyCloseModal(string captionIdentifier, string messageIdentifier, IHaveSprite spriteReference, Action reply, Action close)
	{
		DialogueButtonInfo confirm = new DialogueButtonInfo((reply != null) ? new UnityAction(reply.Invoke) : null, new LocalizedStringWithArgument("Messenger_FriendCard_331"));
		DialogueButtonInfo cancel = new DialogueButtonInfo((close != null) ? new UnityAction(close.Invoke) : null, new LocalizedStringWithArgument("Generic_Close"));
		ModalMessageData modalMessageData = new ModalMessageData(new LocalizedStringWithArgument(captionIdentifier), new LocalizedStringWithArgument(messageIdentifier), confirm, cancel, null, spriteReference);
		ModalDialogueSO modalDialogueSO = ScriptableObject.CreateInstance<ModalDialogueSO>();
		modalDialogueSO.Init(modalMessageData);
		Spawn(modalDialogueSO);
	}

	public static void SpawnInputDialogue(InputModalMessageData inputModalMessageData)
	{
		InputDialogueSO inputDialogueSO = ScriptableObject.CreateInstance<InputDialogueSO>();
		inputDialogueSO.Init(inputModalMessageData);
		Spawn(inputDialogueSO);
	}

	public static void SpawnQuickInfoDialogue(LocalizedStringWithArgument caption, LocalizedStringWithArgument message, string iconKey = null, int duration = 5, DialogueButtonInfo callToAction = null)
	{
		Debug.LogWarning("SpawnQuickInfoDialogue");
		QuickInfoMessageData quickInfoMessageData = new QuickInfoMessageData(caption, message, callToAction, iconKey, duration);
		QuickInfoSO quickInfoSO = ScriptableObject.CreateInstance<QuickInfoSO>();
		quickInfoSO.Init(quickInfoMessageData);
		SpawnQuickInfo(quickInfoSO);
	}

	public static void SpawnQuickInfo(LocalizedStringWithArgument caption, LocalizedStringWithArgument message, int duration = 5, string iconKey = null, string soundKey = null, DialogueButtonInfo callToAction = null)
	{
		QuickInfoMessageData quickInfoMessageData = new QuickInfoMessageData(caption, message, callToAction, iconKey, duration, soundKey);
		QuickInfoSO quickInfoSO = ScriptableObject.CreateInstance<QuickInfoSO>();
		quickInfoSO.Init(quickInfoMessageData);
		SpawnQuickInfo(quickInfoSO);
	}

	public static void SpawnGroupQuickInfoDialogue(LocalizedStringWithArgument message)
	{
		SpawnQuickInfoDialogue(new LocalizedStringWithArgument("Messenger_FriendList_10116"), message, "GroupIcon");
	}

	public static void SpawnSelectClassDialogue(int level)
	{
		ClassChoiceDialogueSO classChoiceDialogueSO = ScriptableObject.CreateInstance<ClassChoiceDialogueSO>();
		classChoiceDialogueSO.Init(level);
		Spawn(classChoiceDialogueSO);
	}

	public static void OpenRemoveFriendDialogue(string name, string id)
	{
		LocalizedStringWithArgument localizedStringWithArgument = new LocalizedStringWithArgument("Messenger_FriendList_337", name);
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Info);
		SpawnYesNoModal("Messenger_FriendList_336", localizedStringWithArgument.GetMessage(), new AddressableSpriteLoader("InfoIcon"), delegate
		{
			Singleton<MilMo_BuddyBackend>.Instance.SendRemoveFriend(id);
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Confirm);
		}, null);
	}
}
