using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.Core.Avatar;
using Code.Core.BuddyBackend;
using Code.Core.Emote;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.GUI.Core;
using Code.Core.Input;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.Core.Template;
using Code.Core.Visual;
using Code.Core.Visual.Effect;
using Code.World.Chat.ChatRoom;
using Code.World.EntityStates;
using Code.World.Environment;
using Code.World.GUI.Hub;
using Code.World.Home;
using Code.World.Level.LevelInfo;
using Code.World.Level.LevelObject;
using Code.World.Player;
using Code.World.WorldMap;
using Core;
using Core.Analytics;
using Core.GameEvent;
using Core.Input;
using Localization;
using Player;
using UI.HUD.Dialogues;
using UI.HUD.Dialogues.Modal;
using UI.Sprites;
using UnityEngine;

namespace Code.World.Level;

public abstract class MilMo_Instance
{
	public enum Type
	{
		Level,
		Home
	}

	public static MilMo_Instance CurrentInstance;

	protected const float CLIMBING_SURFACE_COLLISION_DISTANCE_SQR = 4f;

	public readonly MilMo_Environment Environment = new MilMo_Environment();

	protected string ChannelName;

	protected float LastClickedOnObject;

	protected bool PlayMusic;

	private bool _remotePlayerCollision = true;

	protected readonly Dictionary<string, MilMo_RemotePlayer> PlayersReceived = new Dictionary<string, MilMo_RemotePlayer>();

	private readonly Dictionary<string, float> _requestedPlayers = new Dictionary<string, float>();

	private MilMo_GenericReaction _playerEnterCharBuilderReaction;

	private MilMo_GenericReaction _playerLeaveCharBuilderReaction;

	private MilMo_GenericReaction _avatarUpdatedReaction;

	private MilMo_GenericReaction _playerExhaustedReaction;

	private MilMo_GenericReaction _titleChangeSucceedReaction;

	private MilMo_GenericReaction _remotePlayerJoinReaction;

	private MilMo_GenericReaction _remotePlayerLeaveReaction;

	private MilMo_GenericReaction _updatePositionReaction;

	private MilMo_GenericReaction _updateKnockBackReaction;

	private MilMo_GenericReaction _playerStopReaction;

	private MilMo_GenericReaction _playerEnterShopReaction;

	private MilMo_GenericReaction _playerLeaveShopReaction;

	private MilMo_GenericReaction _playerEnterWorldMapReaction;

	private MilMo_GenericReaction _playerLeaveWorldMapReaction;

	private MilMo_GenericReaction _updateEquipReaction;

	private MilMo_GenericReaction _updateEmoteReaction;

	private MilMo_GenericReaction _showBadgeReaction;

	private MilMo_GenericReaction _hideBadgeReaction;

	private MilMo_GenericReaction _remotePlayerInfoReaction;

	private MilMo_GenericReaction _itemWieldOKReaction;

	private MilMo_GenericReaction _itemUnwieldOKReaction;

	private MilMo_GenericReaction _enterChatroomReaction;

	private MilMo_GenericReaction _leaveChatroomReaction;

	private MilMo_GenericReaction _playerHealthUpdateReaction;

	private MilMo_GenericReaction _playerStateActivatedReaction;

	private MilMo_GenericReaction _playerStateDeactivatedReaction;

	private MilMo_GenericReaction _teleportPlayerReaction;

	private MilMo_GenericReaction _playerEatReaction;

	private MilMo_GenericReaction _playerEnterHubReaction;

	private MilMo_GenericReaction _playerLeaveHubReaction;

	private MilMo_GenericReaction _remoteEntityStateUpdate;

	private MilMo_GenericReaction _playAnimationReaction;

	public Vector3 EntryPoint { get; protected set; }

	public abstract Type InstanceType { get; }

	public List<MilMo_Avatar> Avatars { get; } = new List<MilMo_Avatar>();


	public Dictionary<string, MilMo_RemotePlayer> Players { get; } = new Dictionary<string, MilMo_RemotePlayer>();


	public int NumberOfPlayers => Players.Count + 1;

	public abstract MilMo_LocString ShopDisplayName { get; }

	public abstract Texture2D Icon { get; }

	public bool IgnoreClickOnObjects { get; set; }

	public virtual List<MilMo_MovableObject> Creatures => null;

	public virtual List<MilMo_LevelNpc> NPCs => null;

	public virtual List<MilMo_LevelItem> Items => null;

	protected static MilMo_Player PlayerInstance => MilMo_Player.Instance;

	public virtual void Unload()
	{
		UnloadEventListeners();
		MilMo_RoomPlopManager.Instance.Clear();
		UnloadChatRooms();
		UnloadRemotePlayers();
		MilMo_RemotePlayerCollisionHandler.Instance.Clear();
		CurrentInstance = null;
	}

	protected virtual void LoadEventListeners()
	{
		_avatarUpdatedReaction = MilMo_EventSystem.Listen("avatar_updated", AvatarUpdated);
		_avatarUpdatedReaction.Repeating = true;
		_playerEnterCharBuilderReaction = MilMo_EventSystem.Listen("player_enter_charbuilder", PlayerEnterCharBuilder);
		_playerEnterCharBuilderReaction.Repeating = true;
		_playerLeaveCharBuilderReaction = MilMo_EventSystem.Listen("player_leave_charbuilder", PlayerLeaveCharBuilder);
		_playerLeaveCharBuilderReaction.Repeating = true;
		_titleChangeSucceedReaction = MilMo_EventSystem.Listen("title_change_success", TitleChangeSucceeded);
		_titleChangeSucceedReaction.Repeating = true;
		_remotePlayerJoinReaction = MilMo_EventSystem.Listen("remote_player_join_level", RemotePlayerJoin);
		_remotePlayerJoinReaction.Repeating = true;
		_remotePlayerLeaveReaction = MilMo_EventSystem.Listen("remote_player_leave_instance", RemotePlayerLeave);
		_remotePlayerLeaveReaction.Repeating = true;
		_updatePositionReaction = MilMo_EventSystem.Listen("update_position", UpdatePosition);
		_updatePositionReaction.Repeating = true;
		_updateKnockBackReaction = MilMo_EventSystem.Listen("update_knockback", UpdateKnockBack);
		_updateKnockBackReaction.Repeating = true;
		_playerStopReaction = MilMo_EventSystem.Listen("player_stop", PlayerStop);
		_playerStopReaction.Repeating = true;
		_playerEnterShopReaction = MilMo_EventSystem.Listen("player_enter_shop", PlayerEnterShop);
		_playerEnterShopReaction.Repeating = true;
		_playerLeaveShopReaction = MilMo_EventSystem.Listen("player_leave_shop", PlayerLeaveShop);
		_playerLeaveShopReaction.Repeating = true;
		_playerEnterWorldMapReaction = MilMo_EventSystem.Listen("player_enter_worldmap", PlayerEnterWorldMap);
		_playerEnterWorldMapReaction.Repeating = true;
		_playerLeaveWorldMapReaction = MilMo_EventSystem.Listen("player_leave_worldmap", PlayerLeaveWorldMap);
		_playerLeaveWorldMapReaction.Repeating = true;
		_updateEquipReaction = MilMo_EventSystem.Listen("equip_update", UpdateEquip);
		_updateEquipReaction.Repeating = true;
		_updateEmoteReaction = MilMo_EventSystem.Listen("update_emote", UpdateEmote);
		_updateEmoteReaction.Repeating = true;
		_showBadgeReaction = MilMo_EventSystem.Listen("show_badge", ShowBadge);
		_showBadgeReaction.Repeating = true;
		_hideBadgeReaction = MilMo_EventSystem.Listen("hide_badge", HideBadge);
		_hideBadgeReaction.Repeating = true;
		_remotePlayerInfoReaction = MilMo_EventSystem.Listen("remote_player_info", RemotePlayerInfo);
		_remotePlayerInfoReaction.Repeating = true;
		_itemWieldOKReaction = MilMo_EventSystem.Listen("item_wield_ok", ItemWieldOK);
		_itemWieldOKReaction.Repeating = true;
		_itemUnwieldOKReaction = MilMo_EventSystem.Listen("item_unwield_ok", ItemUnwieldOK);
		_itemUnwieldOKReaction.Repeating = true;
		_enterChatroomReaction = MilMo_EventSystem.Listen("enter_chatroom", EnterChatroom);
		_enterChatroomReaction.Repeating = true;
		_leaveChatroomReaction = MilMo_EventSystem.Listen("leave_chatroom", LeaveChatroom);
		_leaveChatroomReaction.Repeating = true;
		_playerHealthUpdateReaction = MilMo_EventSystem.Listen("player_health_update", PlayerHealthUpdate);
		_playerHealthUpdateReaction.Repeating = true;
		_playerStateActivatedReaction = MilMo_EventSystem.Listen("playerstate_activated", ActivatePlayerState);
		_playerStateActivatedReaction.Repeating = true;
		_playerStateDeactivatedReaction = MilMo_EventSystem.Listen("playerstate_deactivated", DeactivatePlayerState);
		_playerStateDeactivatedReaction.Repeating = true;
		_teleportPlayerReaction = MilMo_EventSystem.Listen("teleport_player", TeleportPlayer);
		_teleportPlayerReaction.Repeating = true;
		_playerEatReaction = MilMo_EventSystem.Listen("player_eat", PlayerEat);
		_playerEatReaction.Repeating = true;
		_playerExhaustedReaction = MilMo_EventSystem.Listen("player_exhausted", PlayerExhausted);
		_playerExhaustedReaction.Repeating = true;
		_playerEnterHubReaction = MilMo_EventSystem.Listen("player_enter_hub", PlayerEnterHub);
		_playerEnterHubReaction.Repeating = true;
		_playerLeaveHubReaction = MilMo_EventSystem.Listen("player_leave_hub", PlayerLeaveHub);
		_playerLeaveHubReaction.Repeating = true;
		_remoteEntityStateUpdate = MilMo_EventSystem.Listen("remote_entity_state_update", RemoteEntityStateUpdate);
		_remoteEntityStateUpdate.Repeating = true;
		_playAnimationReaction = MilMo_EventSystem.Listen("play_animation", PlayAnimation);
		_playAnimationReaction.Repeating = true;
	}

	protected virtual void UnloadEventListeners()
	{
		MilMo_EventSystem.RemoveReaction(_avatarUpdatedReaction);
		_avatarUpdatedReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerEnterCharBuilderReaction);
		_playerEnterCharBuilderReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerLeaveCharBuilderReaction);
		_playerLeaveCharBuilderReaction = null;
		MilMo_EventSystem.RemoveReaction(_titleChangeSucceedReaction);
		_titleChangeSucceedReaction = null;
		MilMo_EventSystem.RemoveReaction(_remotePlayerJoinReaction);
		_remotePlayerJoinReaction = null;
		MilMo_EventSystem.RemoveReaction(_remotePlayerLeaveReaction);
		_remotePlayerLeaveReaction = null;
		MilMo_EventSystem.RemoveReaction(_updatePositionReaction);
		_updatePositionReaction = null;
		MilMo_EventSystem.RemoveReaction(_updateKnockBackReaction);
		_updateKnockBackReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerStopReaction);
		_playerStopReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerEnterShopReaction);
		_playerEnterShopReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerLeaveShopReaction);
		_playerLeaveShopReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerEnterWorldMapReaction);
		_playerEnterWorldMapReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerLeaveWorldMapReaction);
		_playerLeaveWorldMapReaction = null;
		MilMo_EventSystem.RemoveReaction(_updateEquipReaction);
		_updateEquipReaction = null;
		MilMo_EventSystem.RemoveReaction(_updateEmoteReaction);
		_updateEmoteReaction = null;
		MilMo_EventSystem.RemoveReaction(_showBadgeReaction);
		_showBadgeReaction = null;
		MilMo_EventSystem.RemoveReaction(_hideBadgeReaction);
		_hideBadgeReaction = null;
		MilMo_EventSystem.RemoveReaction(_remotePlayerInfoReaction);
		_remotePlayerInfoReaction = null;
		MilMo_EventSystem.RemoveReaction(_itemWieldOKReaction);
		_itemWieldOKReaction = null;
		MilMo_EventSystem.RemoveReaction(_itemUnwieldOKReaction);
		_itemUnwieldOKReaction = null;
		MilMo_EventSystem.RemoveReaction(_enterChatroomReaction);
		_enterChatroomReaction = null;
		MilMo_EventSystem.RemoveReaction(_leaveChatroomReaction);
		_leaveChatroomReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerHealthUpdateReaction);
		_playerHealthUpdateReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerStateActivatedReaction);
		_playerStateActivatedReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerStateDeactivatedReaction);
		_playerStateDeactivatedReaction = null;
		MilMo_EventSystem.RemoveReaction(_teleportPlayerReaction);
		_teleportPlayerReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerEatReaction);
		_playerEatReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerExhaustedReaction);
		_playerExhaustedReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerEnterHubReaction);
		_playerEnterHubReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerLeaveHubReaction);
		_playerLeaveHubReaction = null;
		MilMo_EventSystem.RemoveReaction(_remoteEntityStateUpdate);
		_remoteEntityStateUpdate = null;
		MilMo_EventSystem.RemoveReaction(_playAnimationReaction);
		_playAnimationReaction = null;
	}

	public abstract void Update();

	public virtual void FixedUpdate()
	{
		MilMo_ChatRoomManager.Instance.Update();
		foreach (MilMo_RemotePlayer value in Players.Values)
		{
			value.FixedUpdate();
		}
	}

	public virtual void LateUpdate()
	{
		foreach (MilMo_RemotePlayer value in Players.Values)
		{
			value.LateUpdate();
		}
	}

	public bool TestClickOnObject()
	{
		MilMo_Avatar milMo_Avatar = PlayerInstance?.Avatar;
		if (milMo_Avatar == null)
		{
			return false;
		}
		if (IgnoreClickOnObjects || PlayerInstance.InSinglePlayerArea)
		{
			return false;
		}
		if (!MilMo_Input.GetKeyDown(KeyCode.Mouse0, useKeyboardFocus: false) || Cursor.lockState == CursorLockMode.Locked || MilMo_UserInterfaceManager.MouseFocus != null)
		{
			return false;
		}
		if (Time.time - LastClickedOnObject < 0.5f)
		{
			return false;
		}
		if (!Physics.Raycast(MilMo_Global.MainCamera.ScreenPointToRay(InputSwitch.MousePosition), out var hitInfo, 100f, 536870912))
		{
			return false;
		}
		GameObject gameObject = hitInfo.collider.gameObject;
		if (!gameObject)
		{
			return false;
		}
		if (milMo_Avatar.ClickGameObject == gameObject)
		{
			MilMo_EventSystem.Instance.PostEvent("local_player_left_clicked", PlayerInstance);
			LastClickedOnObject = Time.time;
			return false;
		}
		foreach (MilMo_RemotePlayer value in Players.Values)
		{
			if (value.Avatar != null && (bool)value.Avatar.ClickGameObject && !(value.Avatar.ClickGameObject != gameObject))
			{
				if (milMo_Avatar.InCombat && MilMo_World.HudHandler.hasCombatTimer)
				{
					MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
					return false;
				}
				MilMo_EventSystem.Instance.PostEvent("remote_player_left_clicked", value);
				LastClickedOnObject = Time.time;
				GameEvent.ShowContextMenuEvent?.RaiseEvent(value);
				Debug.LogWarning("Clicked on remote player!");
				return true;
			}
		}
		return false;
	}

	public virtual string GetMaterialAtPosition(Vector3 pos, float rayCastYOffset, out bool terrain)
	{
		terrain = false;
		return "";
	}

	public void RemotePlayerCollision(bool enable)
	{
		if (!PlayerInstance.IsDone || _remotePlayerCollision == enable)
		{
			return;
		}
		_remotePlayerCollision = enable;
		foreach (MilMo_RemotePlayer value in Players.Values)
		{
			if (!(value.Avatar.Id == PlayerInstance.Avatar.Id) && value.Avatar != null)
			{
				value.Avatar.ShouldCollideWithPlayers = enable;
			}
		}
	}

	private async void PlayerExhausted(object msgAsObj)
	{
		if (msgAsObj is ServerPlayerExhausted serverPlayerExhausted)
		{
			int millisecondsDelay = (int)serverPlayerExhausted.getDelay() * 1000;
			string playerId = serverPlayerExhausted.getPlayerID();
			await Task.Delay(millisecondsDelay);
			MilMo_Avatar milMo_Avatar = ((playerId == PlayerInstance.Id) ? PlayerInstance.Avatar : GetRemotePlayer(playerId)?.Avatar);
			if (milMo_Avatar == null)
			{
				Debug.LogWarning("MilMo_Instance.PlayerExhausted: No avatar found");
			}
			else
			{
				milMo_Avatar.UpdateHealth(0f);
			}
		}
	}

	public void SocializationEnded(object avatarAsObj)
	{
		if (!(avatarAsObj is MilMo_Avatar player))
		{
			return;
		}
		foreach (MilMo_Avatar avatar in Avatars)
		{
			avatar.SuperAlivenessManager.PlayerStopSocialize(player);
		}
	}

	public abstract void StartPlayMusic();

	protected void UpdateAvatars()
	{
		MilMo_AvatarLODManager.StartLODUpdate();
		List<MilMo_RemotePlayer> list = new List<MilMo_RemotePlayer>();
		foreach (MilMo_RemotePlayer value in Players.Values)
		{
			if (value.TimeSinceLastUpdate > 24f && value.Avatar.Enabled)
			{
				Debug.Log("Player " + value.Id + " timed out");
				list.Add(value);
				continue;
			}
			value.Update();
			if (value.Avatar != null)
			{
				MilMo_AvatarLODManager.AddAvatar(value);
			}
		}
		foreach (MilMo_RemotePlayer item in list)
		{
			Debug.Log("Removing player " + item.Id);
			Players.Remove(item.Id);
			for (int i = 0; i < Avatars.Count; i++)
			{
				if (!(Avatars[i].Id != item.Avatar.Id))
				{
					Avatars.RemoveAt(i);
					break;
				}
			}
			item.Destroy();
		}
		MilMo_AvatarLODManager.UpdateLOD();
	}

	private static void RemoteEntityStateUpdate(object msg)
	{
		ServerRemoteEntityStateUpdate serverRemoteEntityStateUpdate = msg as ServerRemoteEntityStateUpdate;
		if (serverRemoteEntityStateUpdate != null && (serverRemoteEntityStateUpdate.getEntityId() == PlayerInstance.Id || CurrentInstance == null))
		{
			return;
		}
		IMilMo_Entity milMo_Entity = null;
		if (serverRemoteEntityStateUpdate != null && serverRemoteEntityStateUpdate.getIsAPlayer() == 1)
		{
			if (CurrentInstance.Players.ContainsKey(serverRemoteEntityStateUpdate.getEntityId()))
			{
				milMo_Entity = CurrentInstance.Players[serverRemoteEntityStateUpdate.getEntityId()].Avatar;
			}
		}
		else if (serverRemoteEntityStateUpdate != null)
		{
			int id = int.Parse(serverRemoteEntityStateUpdate.getEntityId());
			using IEnumerator<MilMo_MovableObject> enumerator = CurrentInstance.Creatures.Where((MilMo_MovableObject c) => c.Id == id).GetEnumerator();
			if (enumerator.MoveNext())
			{
				milMo_Entity = enumerator.Current;
			}
		}
		if (milMo_Entity == null)
		{
			return;
		}
		if (serverRemoteEntityStateUpdate.getIsToAdd() == 1)
		{
			foreach (EntityStateEffectReference effect in serverRemoteEntityStateUpdate.getEffects())
			{
				milMo_Entity.GetEntityStateManager().AddEffectToRemoteEntity(effect);
			}
			return;
		}
		foreach (EntityStateEffectReference effect2 in serverRemoteEntityStateUpdate.getEffects())
		{
			milMo_Entity.GetEntityStateManager().RemoveEffectFromRemoteEntity(effect2);
		}
	}

	private void RemotePlayerJoin(object msgAsObj)
	{
		if (!(msgAsObj is ServerRemotePlayerJoinInstance serverRemotePlayerJoinInstance))
		{
			return;
		}
		RemotePlayer remotePlayer = serverRemotePlayerJoinInstance.GetRemotePlayer();
		string id = remotePlayer.GetPlayerID();
		if (id == PlayerInstance.Id || Players.ContainsKey(id) || PlayersReceived.ContainsKey(id))
		{
			return;
		}
		MilMo_RemotePlayer remPlayer = new MilMo_RemotePlayer(id, remotePlayer.GetMemberDaysLeft(), remotePlayer.GetRole(), remotePlayer.GetAvatar().GetTitle(), useSpawnEffects: true, remotePlayer.GetAvatar().GetAvatarLevel());
		PlayersReceived.Add(id, remPlayer);
		if (this is MilMo_Level && MilMo_LevelInfo.IsPvp(MilMo_Level.CurrentLevel.VerboseName))
		{
			remPlayer.SetInvulnerable(value: true);
		}
		remPlayer.Read(serverRemotePlayerJoinInstance.GetRemotePlayer(), delegate(bool success)
		{
			if (!success)
			{
				Debug.LogWarning("Failed to read remote player " + id + " from entered level message");
				PlayersReceived.Remove(id);
			}
			else
			{
				Debug.Log("Adding player " + id + " (Enter Level message)");
				Players.Add(id, remPlayer);
				PlayersReceived.Remove(id);
				_requestedPlayers.Remove(id);
				Avatars.Add(remPlayer.Avatar);
				RemotePlayerJoinInstance(remPlayer);
				MilMo_EventSystem.Instance.AsyncPostEvent("remote_player_added", remPlayer);
				if (!string.IsNullOrEmpty(remPlayer.Avatar.Room))
				{
					MilMo_RoomPlopManager.Instance.PlayerEnteredRoom(remPlayer.Avatar, remPlayer.Avatar.Room);
				}
				if (!_remotePlayerCollision)
				{
					remPlayer.Avatar.ShouldCollideWithPlayers = false;
				}
				if (remPlayer.InShop)
				{
					PlayerEnterShop(remPlayer);
				}
			}
		});
	}

	private void RemotePlayerLeave(object msgAsObj)
	{
		if (!(msgAsObj is ServerRemotePlayerLeaveInstance serverRemotePlayerLeaveInstance))
		{
			return;
		}
		string id = serverRemotePlayerLeaveInstance.getPlayerId();
		if (id == PlayerInstance.Avatar.Id)
		{
			return;
		}
		MilMo_RemotePlayer remotePlayer = GetRemotePlayer(id, requestIfUnknown: false);
		if (remotePlayer != null)
		{
			MilMo_Avatar avatar2 = Players[id].Avatar;
			if (avatar2 != null && !string.IsNullOrEmpty(avatar2.Room))
			{
				MilMo_RoomPlopManager.Instance.PlayerLeftRoom(avatar2, avatar2.Room);
			}
			remotePlayer.Destroy();
			RemotePlayerLeaveInstance(Players[id], serverRemotePlayerLeaveInstance.getDestination());
			Players.Remove(id);
		}
		MilMo_Avatar milMo_Avatar = Avatars.FirstOrDefault((MilMo_Avatar avatar) => avatar.Id == id);
		if (milMo_Avatar != null)
		{
			Avatars.Remove(milMo_Avatar);
		}
	}

	private void TitleChangeSucceeded(object msgAsObject)
	{
		if (msgAsObject is ServerPlayerChangeTitle serverPlayerChangeTitle)
		{
			string playerId = serverPlayerChangeTitle.getPlayerId();
			string title = serverPlayerChangeTitle.getTitle();
			MilMo_Avatar milMo_Avatar = ((!(playerId == PlayerInstance?.Id)) ? GetRemotePlayer(playerId)?.Avatar : PlayerInstance?.Avatar);
			if (milMo_Avatar == null)
			{
				Debug.LogError("ServerPlayerChangeTitle: Unable to find player");
			}
			else
			{
				milMo_Avatar.SetTitle(title);
			}
		}
	}

	private void PlayerEnterShop(object msgAsObject)
	{
		if (msgAsObject is ServerPlayerEnterShop serverPlayerEnterShop)
		{
			MilMo_RemotePlayer value = GetRemotePlayer(serverPlayerEnterShop.getPlayerId());
			if (value != null)
			{
				PlayerEnterShop(value);
			}
			else if (PlayersReceived.TryGetValue(serverPlayerEnterShop.getPlayerId(), out value))
			{
				value.InShop = true;
			}
		}
	}

	private void PlayerLeaveShop(object msgAsObject)
	{
		if (!(msgAsObject is ServerPlayerLeaveShop serverPlayerLeaveShop))
		{
			return;
		}
		MilMo_RemotePlayer value = GetRemotePlayer(serverPlayerLeaveShop.getPlayerId());
		if (value != null)
		{
			value.InShop = false;
			value.Avatar.Enable();
			value.TimeSinceLastUpdate = 0f;
			if (value.Id != PlayerInstance.Id)
			{
				PlayRemotePlayerLeaveSound();
			}
		}
		else if (PlayersReceived.TryGetValue(serverPlayerLeaveShop.getPlayerId(), out value))
		{
			value.InShop = false;
		}
	}

	private void PlayerEnterHub(object msgAsObject)
	{
		if (msgAsObject is ServerPlayerEnterHub serverPlayerEnterHub)
		{
			MilMo_RemotePlayer remotePlayer = GetRemotePlayer(serverPlayerEnterHub.getPlayerId(), requestIfUnknown: false);
			if (remotePlayer != null)
			{
				remotePlayer.InWorldMap = true;
				remotePlayer.Avatar.Disable();
			}
		}
	}

	private void PlayerEnterWorldMap(object msgAsObject)
	{
		if (msgAsObject is ServerPlayerEnterWorldMap serverPlayerEnterWorldMap)
		{
			MilMo_RemotePlayer remotePlayer = GetRemotePlayer(serverPlayerEnterWorldMap.getPlayerId(), requestIfUnknown: false);
			if (remotePlayer != null)
			{
				remotePlayer.InWorldMap = true;
				remotePlayer.Avatar.Disable();
			}
		}
	}

	private void PlayerLeaveHub(object msgAsObject)
	{
		if (!(msgAsObject is ServerPlayerLeaveHub serverPlayerLeaveHub))
		{
			return;
		}
		MilMo_RemotePlayer remotePlayer = GetRemotePlayer(serverPlayerLeaveHub.getPlayerId());
		if (remotePlayer != null)
		{
			remotePlayer.InWorldMap = false;
			remotePlayer.Avatar.Enable();
			remotePlayer.TimeSinceLastUpdate = 0f;
			if (remotePlayer.Id != PlayerInstance.Id)
			{
				PlayRemotePlayerJoinSound();
			}
		}
	}

	private void PlayerLeaveWorldMap(object msgAsObject)
	{
		if (!(msgAsObject is ServerPlayerLeaveWorldMap serverPlayerLeaveWorldMap))
		{
			return;
		}
		MilMo_RemotePlayer remotePlayer = GetRemotePlayer(serverPlayerLeaveWorldMap.getPlayerId());
		if (remotePlayer != null)
		{
			remotePlayer.InWorldMap = false;
			remotePlayer.Avatar.Enable();
			remotePlayer.TimeSinceLastUpdate = 0f;
			if (remotePlayer.Id != PlayerInstance.Id)
			{
				PlayRemotePlayerJoinSound();
			}
		}
	}

	private void PlayerEnterCharBuilder(object msgAsObject)
	{
		if (msgAsObject is ServerPlayerEnterCharBuilder serverPlayerEnterCharBuilder)
		{
			MilMo_RemotePlayer remotePlayer = GetRemotePlayer(serverPlayerEnterCharBuilder.getPlayerId(), requestIfUnknown: false);
			if (remotePlayer != null)
			{
				remotePlayer.InCharBuilder = true;
				remotePlayer.Avatar.Disable();
			}
		}
	}

	private void PlayerLeaveCharBuilder(object msgAsObject)
	{
		if (msgAsObject is ServerPlayerLeaveCharBuilder serverPlayerLeaveCharBuilder)
		{
			MilMo_RemotePlayer remotePlayer = GetRemotePlayer(serverPlayerLeaveCharBuilder.getPlayerId());
			if (remotePlayer != null)
			{
				remotePlayer.InCharBuilder = false;
				remotePlayer.Avatar.Enable();
				remotePlayer.TimeSinceLastUpdate = 0f;
			}
		}
	}

	private void UpdatePosition(object msgAsObj)
	{
		if (msgAsObj is ServerUpdatePosition serverUpdatePosition)
		{
			string text = serverUpdatePosition.getPlayerID().ToString();
			if (text == PlayerInstance.Avatar.Id)
			{
				Singleton<Analytics>.Instance.PositionUpdateReceived();
			}
			else
			{
				GetRemotePlayer(text)?.UpdatePositionMessageReceived(serverUpdatePosition);
			}
		}
	}

	private void UpdateKnockBack(object msgAsObj)
	{
		if (msgAsObj is ServerUpdateKnockBackState serverUpdateKnockBackState)
		{
			string text = serverUpdateKnockBackState.getPlayerID().ToString();
			if (!(text == PlayerInstance.Avatar.Id))
			{
				GetRemotePlayer(text)?.UpdateKnockBackState(serverUpdateKnockBackState.getKnockBackState());
			}
		}
	}

	private void PlayerStop(object msgAsObject)
	{
		if (msgAsObject is ServerPlayerStop serverPlayerStop)
		{
			string playerID = serverPlayerStop.getPlayerID();
			if (!(playerID == PlayerInstance.Avatar.Id))
			{
				float x = (float)serverPlayerStop.getX() * 0.125f;
				float y = (float)serverPlayerStop.getY() * (1f / 32f);
				float z = (float)serverPlayerStop.getZ() * 0.125f;
				Vector3 position = new Vector3(x, y, z);
				float rotation = (float)serverPlayerStop.getRotation() * 360f * 0.00390625f;
				GetRemotePlayer(playerID)?.UpdateWithStopPosition(position, rotation);
			}
		}
	}

	private void PlayerHealthUpdate(object msgAsObj)
	{
		if (!(msgAsObj is ServerPlayerHealthUpdate serverPlayerHealthUpdate))
		{
			return;
		}
		string playerID = serverPlayerHealthUpdate.getPlayerID();
		float health = serverPlayerHealthUpdate.getHealth();
		float maxHealth = serverPlayerHealthUpdate.getMaxHealth();
		float armorDurability = serverPlayerHealthUpdate.getArmorDurability();
		MilMo_Avatar milMo_Avatar = ((!(playerID == PlayerInstance?.Id)) ? GetRemotePlayer(playerID)?.Avatar : PlayerInstance?.Avatar);
		if (milMo_Avatar == null)
		{
			Debug.LogWarning("Got ServerPlayerHealthUpdate but could not find any avatar for player with id " + playerID);
			return;
		}
		milMo_Avatar.UpdateHealth(health);
		milMo_Avatar.UpdateMaxHealth(maxHealth);
		if (playerID == PlayerInstance?.Id)
		{
			milMo_Avatar.Armor.UpdateArmor(armorDurability);
		}
	}

	private void UpdateEquip(object msgAsObj)
	{
		if (msgAsObj is ServerEquipUpdate serverEquipUpdate)
		{
			string playerId = serverEquipUpdate.getPlayerId();
			Debug.Log("Got server equip update for player " + playerId);
			if (!(playerId == PlayerInstance.Avatar.Id))
			{
				GetRemotePlayer(playerId)?.EquipAll(serverEquipUpdate.getItems());
			}
		}
	}

	private void UpdateEmote(object msgAsObj)
	{
		if (!(msgAsObj is ServerUpdateEmote serverUpdateEmote))
		{
			return;
		}
		string playerID = serverUpdateEmote.getPlayerID();
		MilMo_Emote emoteByName = MilMo_EmoteSystem.GetEmoteByName(serverUpdateEmote.getEmoteName());
		if (emoteByName == null)
		{
			return;
		}
		if (playerID != PlayerInstance.Avatar.Id)
		{
			MilMo_RemotePlayer remotePlayer = GetRemotePlayer(playerID);
			if (remotePlayer?.Avatar == null)
			{
				return;
			}
			remotePlayer.Avatar.PlayEmoteLocal(emoteByName);
			MilMo_ChatRoomManager.Instance.ChatMessageFromPlayer(remotePlayer.Avatar);
			foreach (MilMo_Avatar avatar in Avatars)
			{
				avatar.SuperAlivenessManager.PlayerSocialized(remotePlayer.Avatar);
			}
			MilMo_EventSystem.At(emoteByName.Duration, SocializationEnded, remotePlayer.Avatar);
			return;
		}
		MilMo_ChatRoomManager.Instance.ChatMessageFromPlayer(PlayerInstance.Avatar);
		foreach (MilMo_Avatar avatar2 in Avatars)
		{
			avatar2.SuperAlivenessManager.PlayerSocialized(PlayerInstance.Avatar);
		}
		MilMo_EventSystem.At(emoteByName.Duration, SocializationEnded, PlayerInstance.Avatar);
	}

	private void ShowBadge(object msgAsObj)
	{
		if (msgAsObj is ServerPlayerActivateBadge serverPlayerActivateBadge)
		{
			string playerID = serverPlayerActivateBadge.getPlayerID();
			((playerID == PlayerInstance.Id) ? PlayerInstance.Avatar : GetRemotePlayer(playerID)?.Avatar)?.SetShowRole(showRole: true);
		}
	}

	private void HideBadge(object msgAsObj)
	{
		if (msgAsObj is ServerPlayerDeactivateBadge serverPlayerDeactivateBadge)
		{
			string playerID = serverPlayerDeactivateBadge.getPlayerID();
			((playerID == PlayerInstance.Id) ? PlayerInstance.Avatar : GetRemotePlayer(playerID)?.Avatar)?.SetShowRole(showRole: false);
		}
	}

	private void RemotePlayerInfo(object msgAsObj)
	{
		ServerRemotePlayerInfo msg = msgAsObj as ServerRemotePlayerInfo;
		if (msg == null)
		{
			return;
		}
		string id = msg.getPlayer().GetPlayerID();
		if (id == PlayerInstance.Id || Players.ContainsKey(id) || PlayersReceived.ContainsKey(id))
		{
			return;
		}
		MilMo_RemotePlayer remPlayer = new MilMo_RemotePlayer(id, msg.getPlayer().GetMemberDaysLeft(), msg.getPlayer().GetRole(), msg.getPlayer().GetAvatar().GetTitle(), useSpawnEffects: false, msg.getPlayer().GetAvatar().GetAvatarLevel());
		PlayersReceived.Add(id, remPlayer);
		remPlayer.Read(msg.getPlayer(), delegate(bool success)
		{
			if (!success)
			{
				Debug.LogWarning("Failed to read remote player " + id + " from remote player info message");
				PlayersReceived.Remove(id);
			}
			else
			{
				Debug.Log("Adding player " + id + " (Player Info Update message)");
				Players.Add(id, remPlayer);
				PlayersReceived.Remove(id);
				_requestedPlayers.Remove(id);
				Avatars.Add(remPlayer.Avatar);
				MilMo_EventSystem.Instance.AsyncPostEvent("remote_player_added", remPlayer);
				if (!string.IsNullOrEmpty(remPlayer.Avatar.Room))
				{
					MilMo_RoomPlopManager.Instance.PlayerEnteredRoom(remPlayer.Avatar, remPlayer.Avatar.Room);
				}
				if (!_remotePlayerCollision)
				{
					remPlayer.Avatar.ShouldCollideWithPlayers = false;
				}
				PlayerInChatRoom chatRoomInfo = msg.getChatRoomInfo();
				if (chatRoomInfo != null)
				{
					EnterChatroom(chatRoomInfo);
				}
				if (remPlayer.InShop)
				{
					PlayerEnterShop(remPlayer);
				}
			}
		});
	}

	private void ItemWieldOK(object msgAsObj)
	{
		if (msgAsObj is ServerItemWieldOK serverItemWieldOK)
		{
			string playerID = serverItemWieldOK.getPlayerID();
			if (!(playerID == PlayerInstance.Avatar.Id))
			{
				GetRemotePlayer(playerID)?.Wield(serverItemWieldOK.getItem());
			}
		}
	}

	private void ItemUnwieldOK(object msgAsObj)
	{
		if (msgAsObj is ServerItemUnwieldOK serverItemUnwieldOK)
		{
			string playerID = serverItemUnwieldOK.getPlayerID();
			if (!(playerID == PlayerInstance.Avatar.Id))
			{
				GetRemotePlayer(playerID)?.Unwield();
			}
		}
	}

	private static void TeleportPlayer(object msgAsObj)
	{
		ServerTeleportPlayer msg = msgAsObj as ServerTeleportPlayer;
		if (msg == null)
		{
			return;
		}
		string playerId = msg.getPlayerId();
		Vector3 position = new Vector3(msg.getPosition().GetX(), msg.getPosition().GetY(), msg.getPosition().GetZ());
		if (playerId != PlayerInstance.Avatar.Id)
		{
			return;
		}
		MilMo_EventSystem.At(1f, delegate
		{
			if (MilMo_World.Instance.PlayerController != null)
			{
				MilMo_PlayerControllerBase.Teleport(position, Quaternion.Euler(0f, msg.getRotation(), 0f));
			}
			else
			{
				PlayerInstance.Avatar.GameObject.transform.position = position;
				PlayerInstance.Avatar.GameObject.transform.rotation = Quaternion.Euler(0f, msg.getRotation(), 0f);
			}
			if (MilMo_World.Instance.Camera != null)
			{
				MilMo_World.Instance.Camera.SetupPosition();
			}
		});
	}

	private void ActivatePlayerState(object msgAsObj)
	{
		if (!(msgAsObj is ServerActivatePlayerState serverActivatePlayerState))
		{
			return;
		}
		string playerId = serverActivatePlayerState.getPlayerId();
		if (!(playerId == PlayerInstance.Avatar.Id))
		{
			MilMo_RemotePlayer remotePlayer = GetRemotePlayer(playerId);
			if (remotePlayer != null)
			{
				remotePlayer.HandleExposedVariableUpdate(serverActivatePlayerState.getVariableUpdate());
				Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(serverActivatePlayerState.getStateTemplate(), remotePlayer.ActivateState);
			}
		}
	}

	private void DeactivatePlayerState(object msgAsObj)
	{
		if (!(msgAsObj is ServerDeactivatePlayerState serverDeactivatePlayerState))
		{
			return;
		}
		string playerId = serverDeactivatePlayerState.getPlayerId();
		if (!(playerId == PlayerInstance.Avatar.Id))
		{
			MilMo_RemotePlayer remotePlayer = GetRemotePlayer(playerId);
			if (remotePlayer != null)
			{
				remotePlayer.HandleExposedVariableUpdate(serverDeactivatePlayerState.getVariableUpdate());
				Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(serverDeactivatePlayerState.getStateTemplate(), remotePlayer.DeactivateState);
			}
		}
	}

	private void EnterChatroom(object o)
	{
		if (!(o is PlayerInChatRoom playerInChatRoom))
		{
			return;
		}
		MilMo_Avatar milMo_Avatar = null;
		if (playerInChatRoom.GetPlayerId() != PlayerInstance.Avatar.Id)
		{
			MilMo_RemotePlayer remotePlayer = GetRemotePlayer(playerInChatRoom.GetPlayerId());
			if (remotePlayer != null)
			{
				milMo_Avatar = remotePlayer.Avatar;
			}
		}
		else
		{
			milMo_Avatar = PlayerInstance.Avatar;
		}
		if (milMo_Avatar != null)
		{
			MilMo_ChatRoomManager.Instance.EnterChatRoom(playerInChatRoom.GetChatroomId(), playerInChatRoom.GetSitPointId(), milMo_Avatar);
		}
	}

	private void LeaveChatroom(object msgAsObj)
	{
		if (!(msgAsObj is ServerLeaveChatroom serverLeaveChatroom))
		{
			return;
		}
		string playerId = serverLeaveChatroom.getPlayerId();
		long chatroomId = serverLeaveChatroom.getChatroomId();
		Debug.Log($"Player {playerId} leave chatroom {chatroomId} in instance {GetHashCode()}");
		MilMo_Avatar milMo_Avatar = null;
		if (playerId != PlayerInstance.Avatar.Id)
		{
			MilMo_RemotePlayer remotePlayer = GetRemotePlayer(playerId);
			if (remotePlayer != null)
			{
				milMo_Avatar = remotePlayer.Avatar;
			}
		}
		else
		{
			milMo_Avatar = PlayerInstance.Avatar;
		}
		if (milMo_Avatar != null)
		{
			MilMo_ChatRoomManager.Instance.LeaveChatRoom(chatroomId, milMo_Avatar);
		}
	}

	private void PlayAnimation(object msgAsObject)
	{
		if (!(msgAsObject is ServerPlayAnimation serverPlayAnimation))
		{
			return;
		}
		string playerId = serverPlayAnimation.getPlayerId();
		string animation = serverPlayAnimation.getAnimation();
		string particles = serverPlayAnimation.getParticles();
		bool flag = serverPlayAnimation.getIsStaticParticleEffect() == 1;
		MilMo_Avatar milMo_Avatar = null;
		if (PlayerInstance.Id == playerId)
		{
			milMo_Avatar = PlayerInstance.Avatar;
		}
		else if (Players.ContainsKey(playerId))
		{
			milMo_Avatar = Players[playerId].Avatar;
		}
		if (milMo_Avatar == null)
		{
			return;
		}
		if (animation.Length > 0)
		{
			milMo_Avatar.PlayAnimation(animation);
		}
		if (particles.Length > 0 && !flag)
		{
			milMo_Avatar.PlayParticleEffect(particles);
		}
		else
		{
			if (!(particles.Length > 0 && flag))
			{
				return;
			}
			Vector3 position = milMo_Avatar.GameObject.transform.position;
			Quaternion rotation = milMo_Avatar.GameObject.transform.rotation;
			MilMo_VisualRepContainer.AsyncCreateVisualRep("Content/Particles/Batch01/InvisibleParticleSpawner", position, rotation, delegate(MilMo_VisualRep visualRep)
			{
				MilMo_VisualRepContainer.AddForUpdate(visualRep);
				MilMo_Effect effect = MilMo_EffectContainer.GetEffect(particles, visualRep.GameObject);
				visualRep.AddParticleEffect(effect, new Vector3(0f, 0f, 0f));
				MilMo_EventSystem.At(effect.Duration + 0.3f, delegate
				{
					MilMo_VisualRepContainer.RemoveFromUpdate(visualRep);
				});
			});
		}
	}

	private static void PlayerEnterShop(MilMo_RemotePlayer player)
	{
		if (player != null)
		{
			player.InShop = true;
			player.Avatar.Disable();
		}
	}

	private void UnloadRemotePlayers()
	{
		foreach (MilMo_RemotePlayer value in Players.Values)
		{
			value.Destroy();
		}
		Players.Clear();
		Avatars.Clear();
		if (Application.isPlaying && PlayerInstance != null && PlayerInstance.Avatar != null)
		{
			Avatars.Add(PlayerInstance.Avatar);
		}
	}

	protected static void UnloadChatRooms()
	{
		MilMo_ChatRoomManager.Instance.UnloadChatRooms();
	}

	private void PlayerEat(object msgAsObject)
	{
		if (msgAsObject is ServerEat serverEat)
		{
			if (serverEat.getPlayerId() == PlayerInstance.Avatar.Id)
			{
				PlayerInstance.PlayEatEffects(serverEat.getUseNumber());
			}
			else
			{
				GetRemotePlayer(serverEat.getPlayerId())?.Eat(serverEat.getUseNumber());
			}
		}
	}

	private void AvatarUpdated(object msgAsObj)
	{
		if (msgAsObj is ServerUpdateAvatar serverUpdateAvatar)
		{
			string playerId = serverUpdateAvatar.getPlayerId();
			if (!(playerId == PlayerInstance.Id))
			{
				GetRemotePlayer(playerId)?.RecreateAvatar(null, serverUpdateAvatar.getAvatar());
			}
		}
	}

	public MilMo_RemotePlayer GetRemotePlayer(string id, bool requestIfUnknown = true)
	{
		if (id == PlayerInstance.Id)
		{
			return null;
		}
		if (Players.ContainsKey(id))
		{
			return Players[id];
		}
		if (!requestIfUnknown || PlayersReceived.ContainsKey(id))
		{
			return null;
		}
		if (!_requestedPlayers.ContainsKey(id))
		{
			_requestedPlayers.Add(id, Time.time);
		}
		else if (Time.time - _requestedPlayers[id] > 5f)
		{
			Debug.Log("Requesting remote player info for player " + id);
			_requestedPlayers[id] = Time.time;
			Singleton<GameNetwork>.Instance.RequestRemotePlayerInfo(id);
		}
		return null;
	}

	protected virtual void PlayRemotePlayerLeaveSound()
	{
	}

	public virtual void PlayRemotePlayerJoinSound()
	{
	}

	private static void RemotePlayerJoinInstance(IPlayer player)
	{
		if (player != null && Singleton<MilMo_BuddyBackend>.Instance.IsBuddy(player.Id))
		{
			OpenFriendJoinedNotification(player);
		}
	}

	private static void OpenFriendJoinedNotification(IPlayer player)
	{
		LocalizedStringWithArgument message = new LocalizedStringWithArgument((MilMo_Player.InHome ? MilMo_Localization.GetLocString("World_8553").GetCopy() : MilMo_Localization.GetLocString("World_7381").GetCopy()).Identifier, player.Avatar.Name);
		DialogueSpawner.SpawnQuickInfoDialogue(new LocalizedStringWithArgument("World_7376"), message, "NoPortrait");
	}

	private void TryTravel(string destination, bool isTravelingToAHome)
	{
		if (isTravelingToAHome)
		{
			string[] array = destination.Split(':');
			MilMo_World.Instance.GoToHome(array[0], array[1]);
		}
		else if (MilMo_LevelInfo.GetLevelInfoData(destination).IsMembersOnlyArea && !PlayerInstance.IsMember)
		{
			DialogueSpawner.SpawnGoToShopDialogue(new EnterShopModalMessageData(new LocalizedStringWithArgument("World_7628"), new LocalizedStringWithArgument("World_7629"), "Shop:Batch01.Subscriptions.SubscriptionSixMonths", new AddressableSpriteLoader("IconPremium")));
		}
		else if (destination != MilMo_Level.LastAdventureLevel && PlayerInstance.Avatar.TeleportStones <= 0)
		{
			LocalizedStringWithArgument message = new LocalizedStringWithArgument("WorldMap_361", MilMo_LevelInfo.GetLevelInfoData(destination).DisplayNameUpperCase.String);
			DialogueSpawner.SpawnOkModal(new LocalizedStringWithArgument("WorldMap_360"), message, new AddressableSpriteLoader("IconTeleportStone"), null);
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Info);
		}
		else if (PlayerInstance.InNavigator)
		{
			if (PlayerInstance.OkToLeaveNavigator())
			{
				MilMo_WorldMap.WasTravelClosed = true;
				MilMo_WorldMap.TravelClosedFullLevelName = destination;
				PlayerInstance.RequestLeaveNavigator();
			}
		}
		else if (PlayerInstance.InHub)
		{
			if (PlayerInstance.OkToLeaveHub())
			{
				MilMo_Hub.WasTravelClosed = true;
				MilMo_Hub.TravelClosedFullLevelName = destination;
				PlayerInstance.RequestLeaveHub();
			}
		}
		else if (PlayerInstance.OkToTeleport())
		{
			MilMo_LevelInfo.Travel(destination);
		}
	}

	private void RemotePlayerLeaveInstance(IPlayer player, string destination)
	{
		if (player == null)
		{
			return;
		}
		string[] array = destination.Split(':');
		if ((MilMo_Level.CurrentLevel != null && MilMo_Level.CurrentLevel.VerboseName == destination) || array.Length < 2 || (MilMo_Home.CurrentHome != null && MilMo_Home.CurrentHome.OwnerID == array[1]) || !Singleton<MilMo_BuddyBackend>.Instance.IsBuddy(player.Id))
		{
			return;
		}
		if (array[0].ToUpper() == "HOME")
		{
			if (array[1] == player.Id)
			{
				QuickInfoRemotePlayerVisitOwnHome(player);
			}
			else if (array[1] == PlayerInstance.Id)
			{
				QuickInfoRemotePlayerVisitLocalPlayerHome(player);
			}
			else
			{
				QuickInfoRemotePlayerVisitOtherRemotePlayerHome(player, array[1]);
			}
		}
		else if (array[0].ToUpper().Contains("WORLD"))
		{
			QuickInfoRemotePlayerLeftForOtherLevel(player, destination);
		}
	}

	private void QuickInfoRemotePlayerLeftForOtherLevel(IPlayer player, string destination)
	{
		if (!Singleton<GroupManager>.Instance.InGroup(player.Id))
		{
			LocalizedStringWithArgument message = new LocalizedStringWithArgument("World_7377", player.Avatar.Name, MilMo_LevelInfo.GetLevelDisplayName(destination));
			DialogueButtonInfo callToAction = new DialogueButtonInfo(delegate
			{
				TryTravel(destination, isTravelingToAHome: false);
			}, new LocalizedStringWithArgument("World_7382"));
			DialogueSpawner.SpawnQuickInfoDialogue(new LocalizedStringWithArgument("World_7375"), message, MilMo_WorldMap.GetLevelIcoAddressable(destination), 5, callToAction);
		}
	}

	private void QuickInfoRemotePlayerVisitOtherRemotePlayerHome(IPlayer player, string destinationId)
	{
		MilMo_ProfileManager.RequestPlayerName(destinationId, delegate(string destinationName, string _)
		{
			LocalizedStringWithArgument message = new LocalizedStringWithArgument("World_7378", player.Avatar.Name, destinationName);
			DialogueButtonInfo callToAction = new DialogueButtonInfo(delegate
			{
				TryTravel(destinationId + ":" + destinationName, isTravelingToAHome: true);
			}, new LocalizedStringWithArgument("World_7382"));
			DialogueSpawner.SpawnQuickInfoDialogue(new LocalizedStringWithArgument("World_7375"), message, "IconMyHome", 5, callToAction);
		});
	}

	private void QuickInfoRemotePlayerVisitLocalPlayerHome(IPlayer player)
	{
		LocalizedStringWithArgument message = new LocalizedStringWithArgument("World_7379", player.Avatar.Name);
		DialogueButtonInfo callToAction = new DialogueButtonInfo(delegate
		{
			TryTravel(PlayerInstance.Id + ":" + PlayerInstance.Avatar.Name, isTravelingToAHome: true);
		}, new LocalizedStringWithArgument("World_7382"));
		DialogueSpawner.SpawnQuickInfoDialogue(new LocalizedStringWithArgument("World_7375"), message, "IconMyHome", 5, callToAction);
	}

	private void QuickInfoRemotePlayerVisitOwnHome(IPlayer player)
	{
		LocalizedStringWithArgument message = new LocalizedStringWithArgument("World_7380", player.Avatar.Name);
		DialogueButtonInfo callToAction = new DialogueButtonInfo(delegate
		{
			TryTravel(player.Id + ":" + player.Avatar.Name, isTravelingToAHome: true);
		}, new LocalizedStringWithArgument("World_7382"));
		DialogueSpawner.SpawnQuickInfoDialogue(new LocalizedStringWithArgument("World_7375"), message, "IconMyHome", 5, callToAction);
	}
}
