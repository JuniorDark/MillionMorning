using System;
using System.Collections.Generic;
using System.Threading;
using Code.Core.Command;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.Network.messages;
using Code.Core.Network.messages.client;
using Code.Core.Network.messages.client.pvp;
using Code.Core.Network.messages.debug;
using Code.Core.Network.types;
using Code.World.CharBuilder;
using Code.World.Level.LevelObject;
using Core;
using Player;
using UnityEngine;

namespace Code.Core.Network;

public class GameNetwork : Singleton<GameNetwork>, IZenListener
{
	private const int GAME_SERVER_PORT = 1139;

	private static GameObject _gameObject;

	private static GameNetwork _gameNetwork;

	private INetworkLayer _networkLayer;

	private string _username;

	private string _password;

	private bool _waitingForPing;

	private float _pingTime;

	private float _lastAliveSent;

	private float _startLoginTime = -1f;

	private float _finishLoginTime = -1f;

	private bool _hasReportedLoadAndLoginTime;

	private const float UPDATE_CONNECTION_TIME = 30f;

	private bool _hasFocus = true;

	public string Host => _networkLayer.Hostname + ":" + _networkLayer.HostPort;

	public bool IsConnectedToGameServer
	{
		get
		{
			if (_networkLayer != null)
			{
				return _networkLayer.IsConnected;
			}
			return false;
		}
	}

	public static void RegisterCommands()
	{
		MilMo_Command.Instance.RegisterUnhandledCommandCallback((string command) => Singleton<GameNetwork>.Instance.SendCommand(command));
		MilMo_Command.Instance.RegisterCommand("Network.GameHost", Debug_GameHost, isCheat: false);
		MilMo_Command.Instance.RegisterCommand("Network.GameLoginState", Debug_GameLoginState);
		MilMo_Command.Instance.RegisterCommand("Network.GameConnectedState", Debug_GameConnectedState);
		MilMo_Command.Instance.RegisterCommand("Network.Extrapolation", PositionExtrapolation);
		MilMo_Command.Instance.RegisterCommand("Network.CubicInterpolation", CubicInterpolation);
		MilMo_Command.Instance.RegisterCommand("RequestScoreBoard", RequestScoreBoard);
		MilMo_Command.Instance.RegisterCommand("invite", delegate(string[] tokens)
		{
			if (tokens.Length < 2)
			{
				return "No email given.";
			}
			return (!Singleton<GameNetwork>.Instance.SendInviteRequest(tokens[1])) ? "Could not send request." : "Request sent.";
		});
	}

	public void Create()
	{
		MilMo_GenericReaction updateReaction = MilMo_EventSystem.RegisterUpdate(UpdateConnection);
		updateReaction.HighPriority = true;
		MilMo_EventSystem.Listen("ping_response", delegate
		{
			if (!_waitingForPing)
			{
				Debug.LogWarning("Got ping response without listening for ping response");
				_waitingForPing = false;
			}
			else
			{
				float num = Time.realtimeSinceStartup - _pingTime;
				MilMo_EventSystem.Instance.AsyncPostEvent("command_response", (num * 1000f).ToString() ?? "");
				_waitingForPing = false;
			}
		}).Repeating = true;
		MilMo_EventSystem.Listen("connect_info_game", delegate
		{
			if (_finishLoginTime < 0f)
			{
				_finishLoginTime = Time.realtimeSinceStartup;
			}
		});
		MilMo_Command.Instance.RegisterCommand("Ping", PingCommand, isCheat: false);
		MilMo_Command.Instance.RegisterCommand("Network.StopClientAliveUpdates", delegate
		{
			MilMo_EventSystem.UnregisterUpdate(updateReaction);
			return "Client alive updates stopped";
		});
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		_hasFocus = hasFocus;
	}

	private void UpdateConnection(object o)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (!(realtimeSinceStartup - _lastAliveSent < 30f) && IsConnectedToGameServer && _networkLayer.IsLoggedIn)
		{
			_networkLayer.SendMessage(new ClientAlive());
			_lastAliveSent = realtimeSinceStartup;
			if (!_hasFocus)
			{
				_networkLayer.SendMessage(new ClientKeepalive());
			}
		}
	}

	public void ConnectToGameServer(string host, string token)
	{
		if (_startLoginTime < 0f)
		{
			_startLoginTime = Time.realtimeSinceStartup;
		}
		string[] array = host.Split(new char[1] { ':' }, 2);
		string hostname = array[0];
		int hostPort = ((array.Length == 2) ? int.Parse(array[1]) : 1139);
		_networkLayer = CreateLoginLayer(token);
		StartLoginConnection(_networkLayer, hostname, hostPort);
	}

	private void StartLoginConnection(INetworkLayer theNetworkLayer, string hostname, int hostPort)
	{
		new Thread((ThreadStart)delegate
		{
			if (theNetworkLayer is NetworkTcpLayer networkTcpLayer)
			{
				networkTcpLayer.Connect(hostname, hostPort);
			}
		}).Start();
	}

	private INetworkLayer CreateLoginLayer(string token)
	{
		return new NetworkTcpLayer(this, token);
	}

	public void DisconnectFromGameServer()
	{
		if (_networkLayer != null && _networkLayer.IsConnected)
		{
			_networkLayer.Disconnect();
		}
	}

	public void OnConnect()
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("connect_info_game", ConnectResponse.Success);
	}

	public void OnConnectFail(ConnectResponse response)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("connect_info_game", response);
	}

	public void OnDisconnect(bool wasConnected)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("disconnected_game", wasConnected);
	}

	public void OnLogout(LogoutResponse response)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("logout_info_game", response);
	}

	public void OnLogin(LoginResponse response)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("login_info_game", response);
	}

	public void OnLoginFail(LoginResponse response)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("login_info_game", response);
	}

	public void SendToGameServer(IMessage message)
	{
		if (_networkLayer != null && _networkLayer.IsConnected && _networkLayer.IsLoggedIn)
		{
			_networkLayer.SendMessage(message);
		}
	}

	private bool Ping()
	{
		if (_waitingForPing)
		{
			return false;
		}
		_waitingForPing = true;
		_pingTime = Time.realtimeSinceStartup;
		if (_networkLayer != null)
		{
			return _networkLayer.SendMessage(new ClientPing());
		}
		return false;
	}

	private bool SendCommand(string command)
	{
		if (_networkLayer != null)
		{
			return _networkLayer.SendMessage(new ClientCommand(command));
		}
		return false;
	}

	public void SendPlayerUpdate(Vector3 position, Vector3 velocity)
	{
		_networkLayer.SendMessage(new ClientUpdatePosition(new vector3(position.x, position.y, position.z), new vector3(velocity.x, velocity.y, velocity.z)));
	}

	public void SendPlayerStop(Vector3 position, float rotation)
	{
		_networkLayer.SendMessage(new ClientPlayerStop(new vector3(position.x, position.y, position.z), rotation));
	}

	public void SendPlayerStartClimb(int climbingSurface)
	{
		_networkLayer.SendMessage(new ClientStartClimb(climbingSurface));
	}

	public void SendClientSetAfk(sbyte status)
	{
		_networkLayer.SendMessage(new ClientSetAfk(status));
	}

	public void SendPlayerStopClimb()
	{
		_networkLayer.SendMessage(new ClientStopClimb());
	}

	public void SendPlayerEnterGameplayObject(int gameplayObjectId)
	{
		_networkLayer.SendMessage(new ClientEnterGameplayObject(gameplayObjectId));
	}

	public void SendPlayerLeaveGameplayObject()
	{
		_networkLayer.SendMessage(new ClientLeaveGameplayObject());
	}

	public void SendEmoteUpdate(string emote)
	{
		_networkLayer.SendMessage(new ClientUpdateEmote(emote));
	}

	public void SendClientSetItemFavorite(int itemId, sbyte favorite)
	{
		_networkLayer.SendMessage(new ClientSetItemFavorite(itemId, favorite));
	}

	public void SendMoodUpdate(string moodName, bool persist)
	{
		_networkLayer.SendMessage(new ClientUpdateMood(moodName, (sbyte)(persist ? 1 : 0)));
	}

	public void SendActionTimeUpdate(string action)
	{
		_networkLayer.SendMessage(new ClientUpdateLastActionTime(action));
	}

	public void SendSaveSeenHotItems(int hashCode)
	{
		_networkLayer.SendMessage(new ClientSaveSeenHotItems(hashCode));
	}

	public void SendSaveReadNews(int hashCode)
	{
		_networkLayer.SendMessage(new ClientSaveReadNews(hashCode));
	}

	public void SendPlayerPrivacyLevelUpdate(int newPrivacyLevel)
	{
		_networkLayer.SendMessage(new ClientUpdatePrivacyLevel(newPrivacyLevel));
	}

	public void SendItemCheckRequest(int itemId)
	{
		_networkLayer.SendMessage(new ClientRequestInventoryItemCheck(itemId));
	}

	public void SendAmmoCheckRequest()
	{
		_networkLayer.SendMessage(new ClientRequestAmmoCheck());
	}

	public void RequestTelepods()
	{
		_networkLayer.SendMessage(new ClientRequestTeleportStones());
	}

	public bool RequestNotifications()
	{
		return _networkLayer.SendMessage(new ClientRequestNotifications());
	}

	public void SendNotificationRead(int notificationId)
	{
		_networkLayer.SendMessage(new ClientNotificationRead(notificationId));
	}

	public bool RequestPlayerProfile(string playerId, bool isFriend)
	{
		return _networkLayer.SendMessage(new ClientRequestProfile(playerId, (sbyte)(isFriend ? 1 : 0)));
	}

	public void SendAttack(int weaponInventoryId, IMilMo_AttackTarget target)
	{
		_networkLayer.SendMessage(new ClientAttack(weaponInventoryId, target.AsNetworkAttackTarget()));
	}

	public void SendAttackUntargeted(int weaponInventoryId)
	{
		_networkLayer.SendMessage(new ClientUntargetedAttack(weaponInventoryId));
	}

	public void SendGrabFurniture(long furnitureId, string playerTile)
	{
		IMessage message = new ClientGrabFurniture(furnitureId, playerTile);
		_networkLayer.SendMessage(message);
	}

	public void RequestUseFurniture(long furnitureId)
	{
		_networkLayer.SendMessage(new ClientActivateFurniture(furnitureId));
	}

	public void RequestUseDoor(long furnitureId)
	{
		_networkLayer.SendMessage(new ClientUseDoor(furnitureId));
	}

	public void RequestPickupHomeDeliveryBox(long furnitureId)
	{
		_networkLayer.SendMessage(new ClientRequestPickupHomeDeliveryBox(furnitureId));
	}

	public void SendTutorialCompleted(string tutorial)
	{
		_networkLayer.SendMessage(new ClientCompleteTutorial(tutorial));
	}

	public void SendLevelExitUsed(string levelTemplateIdentifier, string exitIdentifier)
	{
		_networkLayer.SendMessage(new ClientSaveUsedLevelExit(levelTemplateIdentifier, exitIdentifier));
	}

	public void SendInsideQuestAreaRequest(string areaName, Vector3 pos)
	{
		_networkLayer.SendMessage(new ClientInsideQuestArea(areaName, new vector3(pos.x, pos.y, pos.z)));
	}

	public void SendLevelLoadTimeReport(string levelTemplateIdentifier, float seconds)
	{
		_networkLayer.SendMessage(new ClientReportLevelLoadTime(levelTemplateIdentifier, seconds));
	}

	public void SendLevelFPSReport(string levelTemplateIdentifier, float medianFPS)
	{
		_networkLayer.SendMessage(new ClientReportLevelFPS(levelTemplateIdentifier, medianFPS));
	}

	public void SendLoadAndLoginTimeReport()
	{
		if (!_hasReportedLoadAndLoginTime && !(_startLoginTime < 0f) && !(_finishLoginTime < 0f))
		{
			_hasReportedLoadAndLoginTime = true;
			_networkLayer.SendMessage(new ClientReportLoadAndLoginTime(Time.realtimeSinceStartup, _finishLoginTime - _startLoginTime));
		}
	}

	public void SendSessionFPSAndPingReport(float medianFPS, float medianPing)
	{
		_networkLayer.SendMessage(new ClientReportSessionFPSAndPing(medianFPS, medianPing));
	}

	public void RequestPickup(Vector3 playerPosition, IList<int> pickups)
	{
		_networkLayer.SendMessage(new ClientPickupRequest(new vector3(playerPosition.x, playerPosition.y, playerPosition.z), pickups));
	}

	public void RequestBecameMemberVerification()
	{
		_networkLayer.SendMessage(new ClientRequestBecameMember());
	}

	public void RequestFindExplorationToken(sbyte tokenIndex, Vector3 position)
	{
		_networkLayer.SendMessage(new ClientExplorationTokenFoundRequest(tokenIndex, new vector3(position.x, position.y, position.z)));
	}

	public void RequestFindCoinToken(sbyte tokenIndex, Vector3 position)
	{
		_networkLayer.SendMessage(new ClientCoinTokenFoundRequest(tokenIndex, new vector3(position.x, position.y, position.z)));
	}

	public void RequestFindStaticGem(sbyte tokenIndex, Vector3 position)
	{
		_networkLayer.SendMessage(new ClientStaticGemFoundRequest(tokenIndex, new vector3(position.x, position.y, position.z)));
	}

	public void RequestFindPremiumToken(Vector3 position)
	{
		_networkLayer.SendMessage(new ClientPremiumTokenFoundRequest(new vector3(position.x, position.y, position.z)));
	}

	public void RequestJuneCash()
	{
		_networkLayer.SendMessage(new ClientRequestCoins());
	}

	public void RequestScoreBoard()
	{
		if (_networkLayer != null && _networkLayer.IsConnected && _networkLayer.IsLoggedIn)
		{
			_networkLayer.SendMessage(new ClientRequestScoreBoard());
		}
	}

	public void RequestChangeTitle(string achievementTemplateIdentifier)
	{
		_networkLayer.SendMessage(new ClientRequestSetTitleFromMedal(achievementTemplateIdentifier));
	}

	public void RequestSaveSeenSlide(string slide)
	{
		_networkLayer.SendMessage(new ClientAddSeenSlide(slide));
	}

	public void RequestTemplate(string category, string path)
	{
		_networkLayer.SendMessage(new ClientRequestTemplate(category, path));
	}

	public void RequestEnterChatRoom(long chatRoomId, short sitPointId)
	{
		_networkLayer.SendMessage(new ClientEnterChatroom(chatRoomId, sitPointId));
	}

	public void RequestActivateGameplayTrigger(int gameplayObjectId, int triggerId)
	{
		_networkLayer.SendMessage(new ClientActivateGameplayTrigger(gameplayObjectId, triggerId));
	}

	public void RequestLeaveChatRoom(long chatRoomId)
	{
		_networkLayer.SendMessage(new ClientLeaveChatroom(chatRoomId));
	}

	public void RequestCollectables()
	{
		_networkLayer.SendMessage(new ClientRequestCollectables());
	}

	public void SendWearablesCheck(IList<int> wearables)
	{
		_networkLayer.SendMessage(new ClientReportWearables(wearables));
	}

	public void RequestCompleteAchievement(string achievementIdentifier)
	{
		_networkLayer.SendMessage(new ClientRequestCompleteAchievement(achievementIdentifier));
	}

	public void RequestNexusLoginInfo()
	{
		_networkLayer.SendMessage(new ClientRequestNexusToken());
	}

	public bool RequestPlayerInfo()
	{
		try
		{
			return _networkLayer.SendMessage(new ClientRequestLocalPlayerInfo());
		}
		catch (Exception ex)
		{
			Debug.LogWarning(ex.Message);
			return false;
		}
	}

	public void RequestPlayerName(string playerId)
	{
		_networkLayer.SendMessage(new ClientRequestPlayerName(playerId));
	}

	public void RequestRemotePlayerInfo(string id)
	{
		_networkLayer.SendMessage(new ClientRequestRemotePlayerInfo(id));
	}

	public void RequestCreateAvatar(AvatarSelection selection)
	{
		if (selection == null)
		{
			Debug.LogError("Selection is null");
			return;
		}
		List<Item> clothes = new List<Item>
		{
			selection.ShirtItem.ItemStruct,
			selection.PantsItem.ItemStruct,
			selection.ShoesItem.ItemStruct,
			selection.HairStyleItem.ItemStruct
		};
		RequestCreateAvatar(selection.AvatarName, selection.Gender, selection.SkinColor, selection.HairColor, selection.EyeColor, selection.Mouth, selection.Eyes, selection.EyeBrows, selection.HairStyle.GetIdentifier(), selection.Height, selection.Mood, clothes, MilMo_Global.AuthorizationToken);
	}

	public void RequestCreateAvatar(string avatarName, byte gender, string skinColor, int hairColor, string eyeColor, string mouth, string eyes, string eyeBrows, string hair, float height, string mood, List<Item> clothes, string token)
	{
		_networkLayer.SendMessage(new ClientRequestCreateAvatar(new Code.Core.Network.types.Avatar(avatarName, (sbyte)gender, skinColor, hairColor, eyeColor, mouth, eyes, eyeBrows, hair, height, 0f, 0f, mood, "", 0, 0, 0, clothes), token));
	}

	public void RequestUpdateAvatar(byte gender, string skinColor, int hairColor, string eyeColor, string mouth, string eyes, string eyebrows, float height, List<Item> clothes)
	{
		_networkLayer.SendMessage(new ClientRequestUpdateAvatar(new Code.Core.Network.types.Avatar("", (sbyte)gender, skinColor, hairColor, eyeColor, mouth, eyes, eyebrows, "", height, 0f, 0f, "", "", 0, 0, 0, clothes)));
	}

	public void CheckAvatarName(string avatarName)
	{
		_networkLayer.SendMessage(new ClientCheckAvatarName(avatarName));
	}

	public bool RequestThumbnailAvatarInfo(string userId)
	{
		return _networkLayer.SendMessage(new ClientRequestThumbnailAvatarInfo(userId));
	}

	public void RequestSpawnLevelProjectile(int weaponId, vector3 startPosition, AttackTarget target, float collisionDistance)
	{
		_networkLayer.SendMessage(new ClientRequestSpawnLevelProjectile(weaponId, startPosition, target, collisionDistance));
	}

	public void RequestRespawn()
	{
		_networkLayer.SendMessage(new ClientRequestRespawn());
	}

	public void RequestDig(Vector3 position)
	{
		_networkLayer.SendMessage(new ClientDig(new vector3(position.x, position.y, position.z)));
	}

	public void RequestEat()
	{
		_networkLayer.SendMessage(new ClientRequestEat());
	}

	public void RequestActivateAbility(int inventoryId, vector3 position, vector3 rotation)
	{
		_networkLayer.SendMessage(new ClientActivateAbility(inventoryId, position, rotation));
	}

	public void RequestActiveSkillItem(int inventoryId, AttackTarget currentTarget, vector3 position, vector3 rotation)
	{
		_networkLayer.SendMessage(new ClientActivateSkillItem(inventoryId, currentTarget, position, rotation));
	}

	public void RequestEquipAbility(sbyte hotkeyIndex, string equipIdentifier = "")
	{
		_networkLayer.SendMessage(new ClientEquipAbility(hotkeyIndex, equipIdentifier));
	}

	public void RequestEquipArmor(int inventoryId)
	{
		_networkLayer.SendMessage(new ClientRequestEquipArmor(inventoryId));
	}

	public void RequestOpenBox(int inventoryId)
	{
		_networkLayer.SendMessage(new ClientRequestOpenBox(inventoryId));
	}

	public void AcceptEnterNpcShop(int id)
	{
		_networkLayer.SendMessage(new ClientAcceptEnterShop(id));
	}

	public void AcceptNpcMemberTeleport(int id)
	{
		_networkLayer.SendMessage(new ClientAcceptMemberTeleport(id));
	}

	public void AcceptNpcEnterCharBuilder(int id)
	{
		_networkLayer.SendMessage(new ClientAcceptEnterCharBuilder(id));
	}

	public void RequestLeaveCharBuilder()
	{
		_networkLayer.SendMessage(new ClientRequestLeaveCharBuilder());
	}

	public void RequestBuyInGameShopItem(int npc, int item, short amount)
	{
		_networkLayer.SendMessage(new ClientNPCBuyItem(npc, item, amount));
	}

	public void RequestSellInGameShopItem(int npc, int item, short amount)
	{
		_networkLayer.SendMessage(new ClientNPCSellItem(npc, item, amount));
	}

	public void RequestCurrencyInfo(string currencyId)
	{
		_networkLayer.SendMessage(new ClientRequestCurrencyInfo(currencyId));
	}

	public void RequestSteamPurchase(string steamId, string language, int package)
	{
		_networkLayer.SendMessage(new ClientRequestSteamPurchase(steamId, language, package));
	}

	public void FinalizeSteamPurchase(string orderId, string appId)
	{
		_networkLayer.SendMessage(new ClientFinalizeSteamPurchase(orderId, appId));
	}

	public void AbortSteamPurchase(string orderId)
	{
		_networkLayer.SendMessage(new ClientAbortSteamTransaction(orderId));
	}

	public void RequestBuyItem(int id, IList<string> modifiers, sbyte itemGender, bool useCoins, NullableString avatarName)
	{
		_networkLayer.SendMessage(new ClientRequestBuyItem(id, modifiers, itemGender, (sbyte)(useCoins ? 1 : 0), avatarName));
	}

	public void RequestExchangeVoucher(int inventoryId)
	{
		_networkLayer.SendMessage(new ClientExchangeVoucher(inventoryId));
	}

	public void RequestJoinInstance()
	{
		_networkLayer.SendMessage(new ClientRequestJoinInstance());
	}

	public void RequestHomeEquipment(string homeOwnerId, long roomId)
	{
		_networkLayer.SendMessage(new ClientRequestHomeEquipment(homeOwnerId, roomId));
	}

	public void RequestHomeEquipmentItem(long itemId)
	{
		_networkLayer.SendMessage(new ClientRequestHomeEquipmentItem(itemId));
	}

	public void RequestStartScreenInfo()
	{
		_networkLayer.SendMessage(new ClientRequestStartScreenInfo());
	}

	public void Debug_RequestChangeLevel(string world, string level, string language)
	{
		_networkLayer.SendMessage(new ClientRequestChangeLevel(world, level, language));
	}

	public void SendLevelTravelGUIUnlocked(string fullLevelName)
	{
		_networkLayer.SendMessage(new ClientLevelTravelGUIUnlocked(fullLevelName));
	}

	public void SendLevelTravelRequest(string world, string level)
	{
		_networkLayer.SendMessage(new ClientLevelTravelRequest(world, level));
	}

	public void SendWorldTravelRequest(string world)
	{
		_networkLayer.SendMessage(new ClientWorldTravelRequest(world));
	}

	public void RequestEnterShop(string currencyId)
	{
		_networkLayer.SendMessage(new ClientRequestEnterShop(currencyId));
	}

	public void RequestLeaveShop()
	{
		_networkLayer.SendMessage(new ClientRequestLeaveShop());
	}

	public void RequestEnterHub()
	{
		_networkLayer.SendMessage(new ClientRequestEnterHub());
	}

	public void RequestLeaveHub()
	{
		_networkLayer.SendMessage(new ClientRequestLeaveHub());
	}

	public void RequestEnterWorldMap()
	{
		_networkLayer.SendMessage(new ClientRequestEnterWorldMap());
	}

	public void RequestLeaveWorldMap()
	{
		_networkLayer.SendMessage(new ClientRequestLeaveWorldMap());
	}

	public void RequestLevelPlayerCounts(List<string> levelsToRequest)
	{
		_networkLayer.SendMessage(new ClientRequestLevelPlayerCounts(levelsToRequest));
	}

	public void RequestCharBuilderData()
	{
		_networkLayer.SendMessage(new ClientRequestCharbuilderData());
	}

	public void EnterHomeAsGuest(string homeOwnerId)
	{
	}

	public void RequestNotificationUpdate(string playerID)
	{
	}

	public void RequestTeleportToFriend(int playerId)
	{
		_networkLayer.SendMessage(new ClientRequestTeleportToFriend(playerId));
	}

	public void RequestTeleport(string token, string host, int port)
	{
		_networkLayer.SendMessage(new ClientRequestTeleport(token, host, port));
	}

	public void RequestShopData()
	{
		_networkLayer.SendMessage(new ClientRequestShopData());
	}

	public void RequestJuneCashItems()
	{
		_networkLayer.SendMessage(new ClientRequestJuneCashItems());
	}

	public void RequestWield(int inventoryItemId)
	{
		_networkLayer.SendMessage(new ClientRequestItemWield(inventoryItemId));
	}

	public void RequestUnwield()
	{
		_networkLayer.SendMessage(new ClientRequestItemUnwield());
	}

	public void RequestUseConsumable(int inventoryId, vector3 position, vector3 rotation)
	{
		_networkLayer.SendMessage(new ClientUseConsumable(inventoryId, position, rotation));
	}

	public void RequestHomeServerInfo(string homeOwnerId, bool useTeleportStone)
	{
		_networkLayer.SendMessage(new ClientRequestServerInfoForHome(homeOwnerId, (sbyte)(useTeleportStone ? 1 : 0)));
	}

	public void RequestHomeStorage(string homeOwnerId)
	{
		_networkLayer.SendMessage(new ClientRequestHomeStorage(homeOwnerId));
	}

	public void RequestCheckForNewHomeItems()
	{
		_networkLayer.SendMessage(new ClientRequestCheckForNewHomeItems());
	}

	public void RequestMoveHomeEquipment(long id, string gridCell, float rotation, bool inStorage, long inRoom)
	{
		_networkLayer.SendMessage(new ClientRequestMoveHomeEquipment(id, gridCell, rotation, (sbyte)(inStorage ? 1 : 0), inRoom));
	}

	public void RequestApplyDoorSkin(long newDoorId, long oldDoorId)
	{
		_networkLayer.SendMessage(new ClientRequestApplyDoorSkin(newDoorId, oldDoorId));
	}

	public void RequestSwapDoorPair(long doorId)
	{
		_networkLayer.SendMessage(new ClientRequestSwapDoorPair(doorId));
	}

	public void RequestApplyRoomSkin(long skinItemId, long roomId)
	{
		_networkLayer.SendMessage(new ClientRequestApplyRoomSkin(skinItemId, roomId));
	}

	public void RequestKickPlayerFromHome(string playerId)
	{
		_networkLayer.SendMessage(new ClientKickPlayerFromHome(playerId));
	}

	public void RequestEndInvulnerability()
	{
		_networkLayer.SendMessage(new ClientRequestEndInvulnerability());
	}

	public void SendChatMessage(string chatMessage, ChatManager.EChatChanel chanel)
	{
		_networkLayer.SendMessage(new ClientChatToAll(chatMessage, (sbyte)chanel));
	}

	public void SendTalkToNpc(int id, Vector3 position)
	{
		_networkLayer.SendMessage(new ClientTalkToNPC(id, new vector3(position.x, position.y, position.z)));
	}

	public void SendAcceptQuestMessage()
	{
		_networkLayer.SendMessage(new ClientAcceptQuest());
	}

	public void SendRejectQuestMessage()
	{
		_networkLayer.SendMessage(new ClientRejectQuest());
	}

	public void SendAcceptLevelChangeOffer(int npc, int levelIndex)
	{
		_networkLayer.SendMessage(new ClientAcceptChangeLevel(npc, levelIndex));
	}

	public void SendFriendRemoved(string friendIdentifier)
	{
		_networkLayer.SendMessage(new ClientFriendRemoved(friendIdentifier));
	}

	public void SendEquipUpdate(int item)
	{
		_networkLayer.SendMessage(new ClientEquipUpdate(item));
	}

	public void SendClassSelectionRequest(string className, sbyte level)
	{
		_networkLayer.SendMessage(new ClientClassSelect(className, level));
	}

	public void SendUnequipUpdate(int item)
	{
		_networkLayer.SendMessage(new ClientUnequipUpdate(item));
	}

	public bool SendInviteRequest(string email)
	{
		return _networkLayer.SendMessage(new ClientInviteRequest(email));
	}

	public void SendSteamInvite(string steamId)
	{
		_networkLayer.SendMessage(new ClientSteamInvite(steamId));
	}

	public void RequestClassLevelUpdate()
	{
		_networkLayer.SendMessage(new ClientRequestClassLevelUpdate());
	}

	public void Debug_RequestCheckCreatureSync(int creatureId, Vector3 creaturePosition)
	{
		_networkLayer.SendMessage(new DebugClientCheckCreatureSync(creatureId, new vector3(creaturePosition.x, creaturePosition.y, creaturePosition.z)));
	}

	public void SendSteamLogin(byte[] steamTicket)
	{
		_networkLayer.SendMessage(new ClientSteamLogin(steamTicket));
	}

	public void RequestJoinPvPQueue(int matchMode)
	{
		_networkLayer.SendMessage(new ClientRequestEnterPvPQueue((sbyte)matchMode));
	}

	public void RequestPvPQueues()
	{
		_networkLayer.SendMessage(new ClientRequestPvPQueues());
	}

	public void SendLeavePvPQueue()
	{
		_networkLayer.SendMessage(new ClientLeavePvPQueue());
	}

	public void RequestInitialSettings()
	{
		_networkLayer.SendMessage(new ClientRequestInitialSettings());
	}

	public void SendPlayerEvent(string eventTag, string data)
	{
		_networkLayer.SendMessage(new ClientPlayerEvent(eventTag, data));
	}

	public void UpdateTrackedQuest(int questId, bool track)
	{
		_networkLayer.SendMessage(new ClientUpdateTrackedQuest(questId, track));
	}

	private static string PingCommand(string[] args)
	{
		if (!Singleton<GameNetwork>.Instance.Ping())
		{
			return "Failed to ping game server. Check your connection.";
		}
		return "";
	}

	private static string Debug_GameHost(string[] args)
	{
		return Singleton<GameNetwork>.Instance._networkLayer.Hostname + ":" + Singleton<GameNetwork>.Instance._networkLayer.HostPort;
	}

	private static string Debug_GameLoginState(string[] args)
	{
		return Singleton<GameNetwork>.Instance._networkLayer.IsLoggedIn.ToString();
	}

	private static string Debug_GameConnectedState(string[] args)
	{
		return Singleton<GameNetwork>.Instance._networkLayer.IsConnected.ToString();
	}

	public void Debug_CreateState(ClientDebugCreateState stateMessage)
	{
		_networkLayer.SendMessage(stateMessage);
	}

	private static string RequestScoreBoard(string[] args)
	{
		Singleton<GameNetwork>.Instance.RequestScoreBoard();
		return "ScoreBoard requested";
	}

	private static string PositionExtrapolation(string[] args)
	{
		if (args.Length == 1)
		{
			if (!MilMo_Global.PositionExtrapolation)
			{
				return "Extrapolation is off";
			}
			return "Extrapolation is on";
		}
		if (args.Length > 2)
		{
			return "Usage: Network.Extrapolation on/off";
		}
		MilMo_Global.PositionExtrapolation = string.Compare(args[1], "on", StringComparison.OrdinalIgnoreCase) == 0;
		if (!MilMo_Global.PositionExtrapolation)
		{
			return "Extrapolation is now off";
		}
		return "Extrapolation is now on";
	}

	private static string CubicInterpolation(string[] args)
	{
		if (args.Length == 1)
		{
			if (!MilMo_Global.CubicInterpolation)
			{
				return "CubicInterpolation is off";
			}
			return "CubicInterpolation is on";
		}
		if (args.Length > 2)
		{
			return "Usage: Network.CubicInterpolation on/off";
		}
		MilMo_Global.CubicInterpolation = string.Compare(args[1], "on", StringComparison.OrdinalIgnoreCase) == 0;
		if (!MilMo_Global.CubicInterpolation)
		{
			return "CubicInterpolation is now off";
		}
		return "CubicInterpolation is now on";
	}
}
