using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Network.handlers;
using Code.Core.Network.handlers.PVP;
using Code.Core.Network.messages;
using Code.Core.Network.messages.debug;
using Code.Core.Network.messages.server;
using Code.Core.Network.messages.server.PVP;
using UnityEngine;

namespace Code.Core.Network;

public static class MessageDispatcher
{
	private readonly struct MessageType
	{
		public readonly IMessageFactory Factory;

		public readonly IHandler Handler;

		public int OpCode => Factory.GetOpCode();

		public MessageType(IMessageFactory factory, IHandler handler)
		{
			Factory = factory;
			Handler = handler;
		}
	}

	private static readonly List<MessageType> MessageTypes;

	private static void AddMessageType(IMessageFactory factory, IHandler handler)
	{
		MessageTypes.Add(new MessageType(factory, handler));
	}

	static MessageDispatcher()
	{
		MessageTypes = new List<MessageType>();
		AddMessageType(new AuthenticationMessageFactory(), new AuthenticationMessageHandler());
		AddMessageType(new ServerCommand.Factory(), new ServerCommandHandler());
		AddMessageType(new ServerChatToAll.Factory(), new ServerChatToAllHandler());
		AddMessageType(new ServerQuestAccepted.Factory(), new ServerQuestAcceptedHandler());
		AddMessageType(new ServerQuestAdded.Factory(), new ServerQuestAddedHandler());
		AddMessageType(new ServerNPCMessage.Factory(), new ServerNPCMessageHandler());
		AddMessageType(new ServerPickupFail.Factory(), new ServerPickupFailHandler());
		AddMessageType(new ServerNPCShopItems.Factory(), new ServerNPCShopItemsHandler());
		AddMessageType(new ServerNPCBuySuccess.Factory(), new ServerNPCBuySuccessHandler());
		AddMessageType(new ServerNPCBuyFail.Factory(), new ServerNPCBuyFailHandler());
		AddMessageType(new ServerNPCSellSuccess.Factory(), new ServerNPCSellSuccessHandler());
		AddMessageType(new ServerNPCSellFail.Factory(), new ServerNPCSellFailHandler());
		AddMessageType(new ServerLocalPlayerInfo.Factory(), new ServerLocalPlayerInfoHandler());
		AddMessageType(new ServerAvatarCreated.Factory(), new ServerAvatarCreatedHandler());
		AddMessageType(new ServerAvatarCreatedFail.Factory(), new ServerAvatarCreatedFailHandler());
		AddMessageType(new ServerTemplate.Factory(), new ServerTemplateHandler());
		AddMessageType(new ServerItemWieldOK.Factory(), new ServerItemWieldOKHandler());
		AddMessageType(new ServerItemWieldFail.Factory(), new ServerItemWieldFailHandler());
		AddMessageType(new ServerInventoryAdd.Factory(), new ServerInventoryAddHandler());
		AddMessageType(new ServerJuneCashUpdate.Factory(), new ServerJuneCashUpdateHandler());
		AddMessageType(new ServerItemUnwieldOK.Factory(), new ServerItemUnwieldOKHandler());
		AddMessageType(new ServerInventoryAmount.Factory(), new ServerInventoryAmountHandler());
		AddMessageType(new ServerInventoryEquip.Factory(), new ServerInventoryEquipHandler());
		AddMessageType(new ServerInventoryUnequip.Factory(), new ServerInventoryUnequipHandler());
		AddMessageType(new ServerInventoryRemove.Factory(), new ServerInventoryRemoveHandler());
		AddMessageType(new ServerReceiveObjectHappy.Factory(), new ServerReceiveObjectHappyHandler());
		AddMessageType(new ServerRemotePlayerJoinInstance.Factory(), new ServerRemotePlayerJoinInstanceHandler());
		AddMessageType(new ServerLocalPlayerJoinLevel.Factory(), new ServerLocalPlayerJoinLevelHandler());
		AddMessageType(new ServerRemotePlayerLeaveInstance.Factory(), new ServerRemotePlayerLeaveInstanceHandler());
		AddMessageType(new ServerLoginInfoLevel.Factory(), new ServerLoginInfoLevelHandler());
		AddMessageType(new ServerChangeLevelFail.Factory(), new ServerChangeLevelFailHandler());
		AddMessageType(new ServerUpdatePosition.Factory(), new ServerUpdatePositionHandler());
		AddMessageType(new ServerPlayerStop.Factory(), new ServerPlayerStopHandler());
		AddMessageType(new ServerObjectCreate.Factory(), new ServerObjectCreateHandler());
		AddMessageType(new ServerObjectUpdate.Factory(), new ServerObjectUpdateHandler());
		AddMessageType(new ServerObjectDestroy.Factory(), new ServerObjectDestroyHandler());
		AddMessageType(new ServerUpdateEmote.Factory(), new ServerUpdateEmoteHandler());
		AddMessageType(new ServerUpdateMood.Factory(), new ServerUpdateMoodHandler());
		AddMessageType(new ServerEquipUpdate.Factory(), new ServerEquipUpdateHandler());
		AddMessageType(new ServerRemotePlayerInfo.Factory(), new ServerRemotePlayerInfoHandler());
		AddMessageType(new ServerLevelObjectPickup.Factory(), new ServerLevelObjectPickupHandler());
		AddMessageType(new ServerObjectMoveableCreate.Factory(), new ServerObjectMoveableCreateHandler());
		AddMessageType(new ServerMoveableHealthUpdate.Factory(), new ServerMoveableHealthUpdateHandler());
		AddMessageType(new ServerNPCCreate.Factory(), new ServerNPCCreateHandler());
		AddMessageType(new ServerShopCategories.Factory(), new ServerShopCategoriesHandler());
		AddMessageType(new ServerShopItems.Factory(), new ServerShopItemsHandler());
		AddMessageType(new ServerBuyItemSuccess.Factory(), new ServerBuyItemSuccessHandler());
		AddMessageType(new ServerBuyItemFail.Factory(), new ServerBuyItemFailHandler());
		AddMessageType(new ServerCharbuilderData.Factory(), new ServerCharbuilderDataHandler());
		AddMessageType(new ServerEnterChatroom.Factory(), new ServerEnterChatroomHandler());
		AddMessageType(new ServerLeaveChatroom.Factory(), new ServerLeaveChatroomHandler());
		AddMessageType(new ServerOnLand.Factory(), new ServerOnLandHandler());
		AddMessageType(new ServerCreaturePrepareAttack.Factory(), new ServerCreaturePrepareAttackHandler());
		AddMessageType(new ServerCreatureBeginAttack.Factory(), new ServerCreatureBeginAttackHandler());
		AddMessageType(new ServerCreatureAttackInstant.Factory(), new ServerCreatureAttackInstantHandler());
		AddMessageType(new ServerCreatureAttackImpact.Factory(), new ServerCreatureAttackImpactHandler());
		AddMessageType(new ServerCapsuleAttackDamage.Factory(), new ServerCapsuleAttackDamageHandler());
		AddMessageType(new ServerPlayerHitByProjectile.Factory(), new ServerPlayerHitByProjectileHandler());
		AddMessageType(new ServerProjectileSpawned.Factory(), new ServerProjectileSpawnedHandler());
		AddMessageType(new ServerPlayerHealthUpdate.Factory(), new ServerPlayerHealthUpdateHandler());
		AddMessageType(new ServerPlayerExhausted.Factory(), new ServerPlayerExhaustedHandler());
		AddMessageType(new ServerPlayerRespawn.Factory(), new ServerPlayerRespawnHandler());
		AddMessageType(new ServerDig.Factory(), new ServerDigHandler());
		AddMessageType(new ServerStartClimb.Factory(), new ServerStartClimbHandler());
		AddMessageType(new ServerStopClimb.Factory(), new ServerStopClimbHandler());
		AddMessageType(new ServerEnterGameplayObject.Factory(), new ServerEnterGameplayObjectHandler());
		AddMessageType(new ServerLeaveGameplayObject.Factory(), new ServerLeaveGameplayObjectHandler());
		AddMessageType(new ServerInviteResponse.Factory(), new ServerInviteResponseHandler());
		AddMessageType(new ServerActivateAbility.Factory(), new ServerActivateAbilityHandler());
		AddMessageType(new ServerDeactivateAbility.Factory(), new ServerDeactivateAbilityHandler());
		AddMessageType(new ServerActivatePlayerState.Factory(), new ServerActivatePlayerStateHandler());
		AddMessageType(new ServerDeactivatePlayerState.Factory(), new ServerDeactivatePlayerStateHandler());
		AddMessageType(new ServerPlayerMissedAttack.Factory(), new ServerPlayerMissedAttackHandler());
		AddMessageType(new ServerPlayerMissedAttackOnPlayer.Factory(), new ServerPlayerMissedAttackOnPlayerHandler());
		AddMessageType(new ServerPlayerHitAttack.Factory(), new ServerPlayerHitAttackHandler());
		AddMessageType(new ServerPlayerHitAttackOnPlayer.Factory(), new ServerPlayerHitAttackOnPlayerHandler());
		AddMessageType(new ServerAttackRejected.Factory(), new ServerAttackRejectedHandler());
		AddMessageType(new ServerPlayerUntargetedAttack.Factory(), new ServerPlayerUntargetedAttackHandler());
		AddMessageType(new ServerThumbnailAvatarInfo.Factory(), new ServerThumbnailAvatarInfoHandler());
		AddMessageType(new ServerThumbnailAvatarInfoNoAvatar.Factory(), new ServerThumbnailAvatarInfoNoAvatarHandler());
		AddMessageType(new ServerEquipArmor.Factory(), new ServerEquipArmorHandler());
		AddMessageType(new ServerCollectables.Factory(), new ServerCollectablesHandler());
		AddMessageType(new ServerLevelObjectCreatureSpawn.Factory(), new ServerLevelObjectCreatureSpawnHandler());
		AddMessageType(new ServerShopResponse.Factory(), new ServerShopResponseHandler());
		AddMessageType(new ServerPlayerEnterShop.Factory(), new ServerPlayerEnterShopHandler());
		AddMessageType(new ServerPlayerLeaveShop.Factory(), new ServerPlayerLeaveShopHandler());
		AddMessageType(new ServerWorldMapResponse.Factory(), new ServerWorldMapResponseHandler());
		AddMessageType(new ServerPlayerEnterWorldMap.Factory(), new ServerPlayerEnterWorldMapHandler());
		AddMessageType(new ServerPlayerLeaveWorldMap.Factory(), new ServerPlayerLeaveWorldMapHandler());
		AddMessageType(new ServerMoveableImpulse.Factory(), new ServerMoveableImpulseHandler());
		AddMessageType(new ServerUpdateAchievementCounter.Factory(), new ServerUpdateAchievementCounterHandler());
		AddMessageType(new ServerAchievementCompleted.Factory(), new ServerAchievementCompletedHandler());
		AddMessageType(new ServerCreatureAggro.Factory(), new ServerCreatureAggroHandler());
		AddMessageType(new ServerCreatureNoAggro.Factory(), new ServerCreatureNoAggroHandler());
		AddMessageType(new ServerUseConverterFailed.Factory(), new ServerUseConverterFailedHandler());
		AddMessageType(new DebugServerCreatureSync.Factory(), new DebugServerCreatureSyncHandler());
		AddMessageType(new ServerAdminMessage.Factory(), new ServerAdminMessageHandler());
		AddMessageType(new ServerGameplayObjectsCreate.Factory(), new ServerGameplayObjectsCreateHandler());
		AddMessageType(new ServerActivateGameplayTrigger.Factory(), new ServerActivateGameplayTriggerHandler());
		AddMessageType(new ServerTeleportPlayer.Factory(), new ServerTeleportPlayerHandler());
		AddMessageType(new ServerExplorationTokensCreate.Factory(), new ServerExplorationTokensCreateHandler());
		AddMessageType(new ServerCoinTokensCreate.Factory(), new ServerCoinTokensCreateHandler());
		AddMessageType(new ServerExplorationTokenFound.Factory(), new ServerExplorationTokenFoundHandler());
		AddMessageType(new ServerCoinTokenFound.Factory(), new ServerCoinTokenFoundHandler());
		AddMessageType(new ServerUpdateGems.Factory(), new ServerUpdateGemsHandler());
		AddMessageType(new ServerUpdateTeleportStones.Factory(), new ServerUpdateTeleportStonesHandler());
		AddMessageType(new ServerPlayerChangeRoom.Factory(), new ServerPlayerChangeRoomHandler());
		AddMessageType(new ServerCreatureStunned.Factory(), new ServerCreatureStunnedHandler());
		AddMessageType(new ServerLevelStateUnlocked.Factory(), new ServerLevelStateUnlockedHandler());
		AddMessageType(new ServerBossChangeMode.Factory(), new ServerBossChangeModeHandler());
		AddMessageType(new ServerQuestAreaSubscribe.Factory(), new ServerQuestAreaSubscribeHandler());
		AddMessageType(new ServerQuestAreaUnsubscribe.Factory(), new ServerQuestAreaUnsubscribeHandler());
		AddMessageType(new ServerGameplayObjectStartMoving.Factory(), new ServerGameplayObjectStartMovingHandler());
		AddMessageType(new ServerGameplayObjectChangeDirection.Factory(), new ServerGameplayObjectChangeDirectionHandler());
		AddMessageType(new DebugServerCapsuleVolumeSync.Factory(), new DebugServerCapsuleVolumeSyncHandler());
		AddMessageType(new ServerCheckAvatarNameResponse.Factory(), new ServerCheckAvatarNameResponseHandler());
		AddMessageType(new ServerPing.Factory(), new ServerPingHandler());
		AddMessageType(new ServerSafeDisconnectResponse.Factory(), new ServerSafeDisconnectResponseHandler());
		AddMessageType(new ServerWearableItemsConfirm.Factory(), new ServerWearableItemsConfirmHandler());
		AddMessageType(new ServerMemberUpdate.Factory(), new ServerMemberUpdateHandler());
		AddMessageType(new ServerLevelPlayerCounts.Factory(), new ServerLevelPlayerCountsHandler());
		AddMessageType(new ServerPremiumTokens.Factory(), new ServerPremiumTokensHandler());
		AddMessageType(new ServerPlayerChangeTitle.Factory(), new ServerPlayerChangeTitleHandler());
		AddMessageType(new ServerChangeTitleFail.Factory(), new ServerChangeTitleFailHandler());
		AddMessageType(new ServerPlayerActivateBadge.Factory(), new ServerPlayerActivateBadgeHandler());
		AddMessageType(new ServerPlayerDeactivateBadge.Factory(), new ServerPlayerDeactivateBadgeHandler());
		AddMessageType(new ServerPremiumTokenFound.Factory(), new ServerPremiumTokenFoundHandler());
		AddMessageType(new ServerPlayerProfile.Factory(), new ServerPlayerProfileHandler());
		AddMessageType(new ServerStaticGems.Factory(), new ServerStaticGemsHandler());
		AddMessageType(new ServerStaticGemFound.Factory(), new ServerStaticGemFoundHandler());
		AddMessageType(new ServerEat.Factory(), new ServerEatHandler());
		AddMessageType(new ServerInventoryModifierChanged.Factory(), new ServerInventoryModifierChangedHandler());
		AddMessageType(new ServerNewNotification.Factory(), new ServerNewNotificationHandler());
		AddMessageType(new ServerEnterCharBuilderResponse.Factory(), new ServerEnterCharBuilderResponseHandler());
		AddMessageType(new ServerPlayerEnterCharBuilder.Factory(), new ServerPlayerEnterCharBuilderHandler());
		AddMessageType(new ServerPlayerLeaveCharBuilder.Factory(), new ServerPlayerLeaveCharBuilderHandler());
		AddMessageType(new ServerUpdateAvatarResult.Factory(), new ServerUpdateAvatarResultHandler());
		AddMessageType(new ServerUpdateAvatar.Factory(), new ServerUpdateAvatarHandler());
		AddMessageType(new ServerAmmoTypes.Factory(), new ServerAmmoTypesHandler());
		AddMessageType(new ServerAmmoUpdate.Factory(), new ServerAmmoUpdateHandler());
		AddMessageType(new ServerMovableAttackDone.Factory(), new ServerMovableAttackDoneHandler());
		AddMessageType(new ServerWorldStates.Factory(), new ServerWorldStatesHandler());
		AddMessageType(new ServerWorldLevelUpdate.Factory(), new ServerWorldLevelUpdateHandler());
		AddMessageType(new ServerUsedLevelExits.Factory(), new ServerUsedLevelExitsHandler());
		AddMessageType(new ServerUpdateCoins.Factory(), new ServerUpdateCoinsHandler());
		AddMessageType(new ServerCurrencyInfo.Factory(), new ServerCurrencyInfoHandler());
		AddMessageType(new ServerLoginInfoHome.Factory(), new ServerLoginInfoHomeHandler());
		AddMessageType(new ServerHomeInstanceInfo.Factory(), new ServerHomeInstanceInfoHandler());
		AddMessageType(new ServerEnterHomeFail.Factory(), new ServerEnterHomeFailHandler());
		AddMessageType(new ServerLocalPlayerJoinHome.Factory(), new ServerLocalPlayerJoinHomeHandler());
		AddMessageType(new ServerJoinHomeFail.Factory(), new ServerJoinHomeFailHandler());
		AddMessageType(new ServerHomeEquipment.Factory(), new ServerHomeEquipmentHandler());
		AddMessageType(new ServerAllEquipmentInRoomSent.Factory(), new ServerAllEquipmentInRoomSentHandler());
		AddMessageType(new ServerMoveHomeEquipment.Factory(), new ServerMoveHomeEquipmentHandler());
		AddMessageType(new ServerApplyRoomSkin.Factory(), new ServerApplyRoomSkinHandler());
		AddMessageType(new ServerPlayerGrabFurniture.Factory(), new ServerPlayerGrabFurnitureHandler());
		AddMessageType(new ServerPlayerRotateFurniture.Factory(), new ServerPlayerRotateFurnitureHandler());
		AddMessageType(new ServerMoveHomeEquipmentFromStorage.Factory(), new ServerMoveHomeEquipmentFromStorageHandler());
		AddMessageType(new ServerKickPlayerFromHomeResult.Factory(), new ServerKickPlayerFromHomeResultHandler());
		AddMessageType(new ServerPlayerKickedFromHome.Factory(), new ServerPlayerKickedFromHomeHandler());
		AddMessageType(new ServerTeleportToFriendOk.Factory(), new ServerTeleportToFriendOkHandler());
		AddMessageType(new ServerTeleportToFriendFail.Factory(), new ServerTeleportToFriendFailHandler());
		AddMessageType(new ServerFurnitureActivated.Factory(), new ServerFurnitureActivatedHandler());
		AddMessageType(new ServerRequestPlayerNameResponse.Factory(), new ServerRequestPlayerNameResponseHandler());
		AddMessageType(new ServerLevelInstanceInfo.Factory(), new ServerLevelInstanceInfoHandler());
		AddMessageType(new ServerBoxOpened.Factory(), new ServerBoxOpenedHandler());
		AddMessageType(new ServerMOTD.Factory(), new ServerMOTDHandler());
		AddMessageType(new ServerSpawnVisualRepAt.Factory(), new ServerSpawnVisualRepAtHandler());
		AddMessageType(new ServerInviteItemReceived.Factory(), new ServerInviteItemReceivedHandler());
		AddMessageType(new ServerHubResponse.Factory(), new ServerHubResponseHandler());
		AddMessageType(new ServerPlayerEnterHub.Factory(), new ServerPlayerEnterHubHandler());
		AddMessageType(new ServerPlayerLeaveHub.Factory(), new ServerPlayerLeaveHubHandler());
		AddMessageType(new ServerNPCChangedInteractionState.Factory(), new ServerNPCChangedInteractionStateHandler());
		AddMessageType(new ServerLastActionTimeUpdated.Factory(), new ServerLastActionTimeUpdatedHandler());
		AddMessageType(new ServerStartScreenInfo.Factory(), new ServerStartScreenInfoHandler());
		AddMessageType(new ServerRequestHomeBoxPosition.Factory(), new ServerRequestHomeBoxPositionHandler());
		AddMessageType(new ServerHomeDeliveryBoxPickedUp.Factory(), new ServerHomeDeliveryBoxPickedUpHandler());
		AddMessageType(new ServerKeepalive.Factory(), new ServerKeepaliveHandler());
		AddMessageType(new ServerQuestConditionUpdate.Factory(), new ServerQuestConditionUpdateHandler());
		AddMessageType(new ServerExpUpdate.Factory(), new ServerExpUpdateHandler());
		AddMessageType(new ServerAvatarLevelUpdate.Factory(), new ServerAvatarLevelUpdateHandler());
		AddMessageType(new ServerUpdateKnockBackState.Factory(), new ServerUpdateKnockBackStateHandler());
		AddMessageType(new ServerInventoryEntryUpgraded.Factory(), new ServerInventoryEntryUpgradedHandler());
		AddMessageType(new ServerRecreateInventoryAndAmmo.Factory(), new ServerRecreateInventoryAndAmmoHandler());
		AddMessageType(new ServerUpdateMatchState.Factory(), new ServerUpdateMatchStateHandler());
		AddMessageType(new ServerScoreBoard.Factory(), new ServerScoreBoardHandler());
		AddMessageType(new ServerLevelExpRecieved.Factory(), new ServerLevelExpRecievedHandler());
		AddMessageType(new ServerUpdateKillsDeaths.Factory(), new ServerUpdateKillsDeathsHandler());
		AddMessageType(new ServerLadderPositionFor.Factory(), new ServerLadderPositionForHandler());
		AddMessageType(new ServerLadder.Factory(), new ServerLadderHandler());
		AddMessageType(new ServerPlayerEndInvulnerability.Factory(), new ServerPlayerEndInvulnerabilityHandler());
		AddMessageType(new ServerCreatureToLowLevelForXp.Factory(), new ServerCreatureToLowLevelForXpHandler());
		AddMessageType(new ServerCreatureToHighLevelToDamage.Factory(), new ServerCreatureToHighLevelToDamageHandler());
		AddMessageType(new ServerCreatureHitByProjectile.Factory(), new ServerCreatureHitByProjectileHandler());
		AddMessageType(new ServerPlayerKilledByPlayer.Factory(), new ServerPlayerKilledByPlayerHandler());
		AddMessageType(new ServerProjectileDestroyed.Factory(), new ServerProjectileDestroyedHandler());
		AddMessageType(new ServerUpdateExposedVariable.Factory(), new ServerUpdateExposedVariableHandler());
		AddMessageType(new ServerConverterUsed.Factory(), new ServerConverterUsedHandler());
		AddMessageType(new ServerChangeNameResponse.Factory(), new ServerChangeNameResponseHandler());
		AddMessageType(new ServerEntityStateUpdate.Factory(), new ServerEntityStateUpdateHandler());
		AddMessageType(new ServerRemoteEntityStateUpdate.Factory(), new ServerRemoteEntityStateUpdateHandler());
		AddMessageType(new ServerEntityStateEffectAdded.Factory(), new ServerEntityStateEffectAddedHandler());
		AddMessageType(new ServerSkillActivated.Factory(), new ServerSkillActivatedHandler());
		AddMessageType(new ServerSkillActivationFailed.Factory(), new ServerSkillActivationFailedHandler());
		AddMessageType(new ServerSkillsAvailableUpdate.Factory(), new ServerSkillsAvailableUpdateHandler());
		AddMessageType(new ServerClassSelect.Factory(), new ServerClassSelectHandler());
		AddMessageType(new ServerClassLevelUp.Factory(), new ServerClassLevelUpHandler());
		AddMessageType(new ServerGroupForm.Factory(), new ServerGroupFormHandler());
		AddMessageType(new ServerGroupMembers.Factory(), new ServerGroupMembersHandler());
		AddMessageType(new ServerGroupInvite.Factory(), new ServerGroupInviteHandler());
		AddMessageType(new ServerGroupInviteResponse.Factory(), new ServerGroupInviteResponseHandler());
		AddMessageType(new ServerGroupJoin.Factory(), new ServerGroupJoinHandler());
		AddMessageType(new ServerGroupLeave.Factory(), new ServerGroupLeaveHandler());
		AddMessageType(new ServerGroupKicked.Factory(), new ServerGroupKickedHandler());
		AddMessageType(new ServerGroupNewLeader.Factory(), new ServerGroupNewLeaderHandler());
		AddMessageType(new ServerGroupInitiateTravel.Factory(), new ServerGroupInitiateTravelHandler());
		AddMessageType(new ServerGroupTravelResponse.Factory(), new ServerGroupTravelResponseHandler());
		AddMessageType(new ServerGroupTravelFailed.Factory(), new ServerGroupTravelFailedHandler());
		AddMessageType(new ServerPlayAnimation.Factory(), new ServerPlayAnimationHandler());
		AddMessageType(new ServerNexusToken.Factory(), new ServerNexusTokenHandler());
		AddMessageType(new ServerVoteStatus.Factory(), new ServerVoteStatusHandler());
		AddMessageType(new ServerVoteScore.Factory(), new ServerVoteScoreHandler());
		AddMessageType(new ServerNumVotes.Factory(), new ServerNumVotesHandler());
		AddMessageType(new ServerLadderPage.Factory(), new ServerLadderPageHandler());
		AddMessageType(new ServerPlayerChangeRoomInHome.Factory(), new ServerPlayerChangeRoomInHomeHandler());
		AddMessageType(new ServerPlayerRoomChangeError.Factory(), new ServerPlayerRoomChangeErrorHandler());
		AddMessageType(new ServerPvPLadder.Factory(), new ServerPvPLadderHandler());
		AddMessageType(new ServerRoomEntranceChanged.Factory(), new ServerRoomEntranceChangedHandler());
		AddMessageType(new ServerPlayerBannedFromHome.Factory(), new ServerPlayerBannedFromHomeHandler());
		AddMessageType(new ServerPlayerUnBannedFromHome.Factory(), new ServerPlayerUnBannedFromHomeHandler());
		AddMessageType(new ServerHomeAccessResponse.Factory(), new ServerHomeAccessResponseHandler());
		AddMessageType(new ServerMoveHomeEquipmentFromOtherRoom.Factory(), new ServerMoveHomeEquipmentFromOtherRoomHandler());
		AddMessageType(new ServerAddPlayerToHomeOfTheDayRaffleResponse.Factory(), new ServerAddPlayerToHomeOfTheDayRaffleResponseHandler());
		AddMessageType(new ServerHomeOfTheDayResponse.Factory(), new ServerHomeOfTheDayResponseHandler());
		AddMessageType(new ServerSteamTransactionError.Factory(), new ServerSteamTransactionErrorHandler());
		AddMessageType(new ServerJuneCashItems.Factory(), new ServerJuneCashItemsHandler());
		AddMessageType(new ServerPlayerJoinsTeam.Factory(), new ServerPlayerJoinsTeamHandler());
		AddMessageType(new ServerPlayerLeavesTeam.Factory(), new ServerPlayerLeavesTeamHandler());
		AddMessageType(new ServerTeamsComposition.Factory(), new ServerTeamsCompositionHandler());
		AddMessageType(new ServerPvPLevelInstanceInfo.Factory(), new ServerPvPLevelInstanceInfoHandler());
		AddMessageType(new ServerPvPQueueSizeUpdate.Factory(), new ServerPvPQueueSizeUpdateHandler());
		AddMessageType(new ServerSetGameplayObjectPosition.Factory(), new ServerSetGameplayObjectPositionHandler());
		AddMessageType(new ServerPvPQueues.Factory(), new ServerPvPQueuesHandler());
		AddMessageType(new ServerStartCaptureZoneCountdown.Factory(), new ServerStartCaptureZoneCountdownHandler());
		AddMessageType(new ServerStopCaptureZoneCountdown.Factory(), new ServerStopCaptureZoneCountdownHandler());
		AddMessageType(new ServerTeamCapturedZone.Factory(), new ServerTeamCapturedZoneHandler());
		AddMessageType(new ServerGameplayObjectShrink.Factory(), new ServerGameplayObjectShrinkHandler());
		AddMessageType(new ServerPvPOkToJoin.Factory(), new ServerPvPOkToJoinHandler());
		AddMessageType(new ServerPlayerDeliveredFlag.Factory(), new ServerPlayerDeliveredFlagHandler());
		AddMessageType(new ServerInitialSettings.Factory(), new ServerInitialSettingsHandler());
		AddMessageType(new ServerPlayerDied.Factory(), new ServerPlayerDiedHandler());
		AddMessageType(new ServerPlayersAliveCount.Factory(), new ServerPlayersAliveCountHandler());
		AddMessageType(new ServerSkillItemActivated.Factory(), new ServerSkillItemActivatedHandler());
		AddMessageType(new ServerSkillItemActivationFailed.Factory(), new ServerSkillItemActivationFailedHandler());
	}

	private static MessageType GetMessageType(int opCode)
	{
		MessageType messageType = MessageTypes.FirstOrDefault((MessageType t) => t.OpCode == opCode);
		if (object.Equals(messageType, default(MessageType)))
		{
			throw new Exception($"MessageType with opCode {opCode} not found.");
		}
		if (messageType.Factory == null)
		{
			throw new Exception($"MessageType with opCode {opCode} has no Factory.");
		}
		if (messageType.Handler == null)
		{
			throw new Exception($"MessageType with opCode {opCode} has no Handler.");
		}
		return messageType;
	}

	public static void Dispatch(int opCode, MessageReader reader, IZenListener listener)
	{
		try
		{
			MessageType messageType = GetMessageType(opCode);
			IMessage message = messageType.Factory.CreateMessage(reader);
			messageType.Handler.Handle(message, listener);
		}
		catch (Exception exception)
		{
			Debug.LogError($"Got exception while dispatching message. OP-code: {opCode}");
			Debug.LogException(exception);
			throw;
		}
	}

	public static bool GetMessageSize(int opCode, ByteBuffer buffer, out int length, out int lengthSize)
	{
		try
		{
			return GetMessageType(opCode).Factory.GetMessageSize(buffer, out length, out lengthSize);
		}
		catch (Exception exception)
		{
			Debug.LogError($"Got exception while reading message size. OP-code: {opCode}");
			Debug.LogException(exception);
			throw;
		}
	}
}
