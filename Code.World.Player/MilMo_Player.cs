using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Code.Apps.Fade;
using Code.Core.Avatar;
using Code.Core.Avatar.Ragdoll;
using Code.Core.Command;
using Code.Core.Config;
using Code.Core.Emote;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.GUI;
using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.Items;
using Code.Core.Monetization;
using Code.Core.Network;
using Code.Core.Network.messages.client;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.PlayerState;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.Core.Utility;
using Code.World.Achievements;
using Code.World.CharacterShop;
using Code.World.CharacterShop.RemoteShop;
using Code.World.Climbing;
using Code.World.Feeds;
using Code.World.Gameplay;
using Code.World.GUI;
using Code.World.GUI.FriendInvites;
using Code.World.GUI.Hub;
using Code.World.GUI.LoadingScreen;
using Code.World.GUI.QuestLog;
using Code.World.Home;
using Code.World.Inventory;
using Code.World.Level;
using Code.World.Level.LevelInfo;
using Code.World.Level.PVP;
using Code.World.Quest;
using Code.World.Slides;
using Code.World.Tutorial;
using Core;
using Core.Analytics;
using Core.GameEvent;
using Core.Input;
using Core.State;
using Localization;
using Player;
using Player.Moods;
using UI.HUD.Dialogues;
using UI.HUD.Dialogues.Modal;
using UI.HUD.States;
using UI.LockState;
using UI.Sprites;
using UnityEngine;

namespace Code.World.Player;

public class MilMo_Player : IPlayer, IDialogueUser
{
	public delegate void AvatarCreatedCallback(bool successful);

	public delegate void LoginCallback(MilMo_LoginInfo info);

	public delegate void LogoutCallback();

	private enum ToggleDirection
	{
		Left,
		Right
	}

	public delegate void LocalPlayerInfoCallback(bool success, ServerLocalPlayerInfo localPlayerInfo);

	private AvatarCreatedCallback _avatarCreatedCallback;

	private MilMo_TimerEvent _createAvatarTimeout;

	private bool _requestingCreateAvatar;

	private const string TOGGLE_WIELDABLE_SOUND = "Content/Sounds/Batch01/Inventory/ToggleWieldable";

	private const string TOGGLE_WIELDABLE_PVP_SOUND = "Content/Sounds/Batch01/GUI/Generic/Swap";

	private const string WIELDABLE_EMPTY_SOUND = "Content/Sounds/Batch01/Inventory/CloseBag";

	private const float UPDATE_TO_SERVER_FREQUENCY_NORMAL = 0.33f;

	private const float UPDATE_TO_SERVER_FREQUENCY_PVP = 0.1f;

	public const float UPDATE_TO_SERVER_ANYWAY_FREQUENCY = 10f;

	public static MilMo_GenericReaction DisconnectListener = null;

	public Action<int> OnNextAvatarLevelExpRequirementUpdated;

	public Action<int> OnExpUpdated;

	public Action<int> OnCurrentAvatarLevelExpRequirementUpdated;

	private readonly List<ExposedVariableUpdate> _pendingVariableUpdates = new List<ExposedVariableUpdate>();

	private float _lastRespawnRequest;

	private MilMo_GameplayObject _onMovingPlatform;

	private int _killingLowLevelCritters;

	private int _killingHighLevelCritters = 10;

	private MilMo_GenericReaction _respawnButtonReaction;

	private float _afkTimer;

	private bool _afk;

	private const float AFK_TIMEOUT = 300f;

	private static int _textureOffset;

	private sbyte _role;

	private bool _hasPlayed;

	private long _playedSeconds;

	private long _lastUpdatedPlayInterval;

	private int _currentAvatarLevelExpRequirement;

	private int _exp;

	private int _nextAvatarLevelExpRequirement = 1;

	private IList<InventoryEntry> _pendingInvetoryData;

	private IList<AmmoType> _pendingAmmoTypes;

	private IList<string> _pendingHotkeys;

	private const float PVP_RESPAWN_TIME = 7f;

	private const float PVE_RESPAWN_TIME = 3f;

	private bool _inShop;

	private int _membershipDaysLeft = -1;

	private float _membershipLastUpdatedTime = -10000f;

	private LocalPlayerInfoCallback _localPlayerInfoCallback;

	private MilMo_GenericReaction _localPlayerInfoListener;

	private MilMo_TimerEvent _playerInfoTimeOutListener;

	private int _memberShipStatus;

	private static bool _shopEnabled = MilMo_Config.Instance.IsTrue("Shop.Enabled", defaultValue: true);

	public static MilMo_Player Instance { get; } = new MilMo_Player();


	public static float UpdateToServerFrequency
	{
		get
		{
			if (MilMo_Level.CurrentLevel != null && MilMo_LevelInfo.IsPvp(MilMo_Level.CurrentLevel.VerboseName))
			{
				return 0.1f;
			}
			return 0.33f;
		}
	}

	public string Id { get; private set; }

	public MilMo_Avatar Avatar { get; private set; }

	public MilMo_AvatarSilhouette Silhouette { get; private set; }

	public MilMo_Inventory Inventory { get; private set; }

	public MilMo_QuestLog QuestLog { get; private set; }

	public MilMo_ClimbingSurface ClimbingSurface { get; private set; }

	public WeaponSlots EquipSlots { get; }

	public AmmoManager PlayerAmmoManager { get; }

	private SkillManager PlayerSkillManager { get; }

	public MilMo_QuestManager Quests { get; }

	public PlayerClassManager PlayerClassManager { get; }

	public bool InSinglePlayerArea { get; set; }

	public bool AnyShopState
	{
		get
		{
			if (!_inShop && !LeavingShop)
			{
				return EnteringShop;
			}
			return true;
		}
	}

	public bool InSpline { get; private set; }

	public bool IsTalking { get; private set; }

	public bool InDialogue { get; private set; }

	public bool InCombat => Avatar?.InCombat ?? false;

	public bool IsTooHappy => Avatar?.InHappyPickup ?? false;

	public bool IsExhausted { get; private set; }

	public bool IsClimbing => ClimbingSurface != null;

	public int NpCsInRange { get; private set; }

	public string Tweet { get; private set; }

	public int AvatarLevel => Avatar?.AvatarLevel ?? 0;

	public int NextAvatarLevelExpRequirement
	{
		get
		{
			return _nextAvatarLevelExpRequirement;
		}
		private set
		{
			_nextAvatarLevelExpRequirement = value;
			OnNextAvatarLevelExpRequirementUpdated?.Invoke(value);
		}
	}

	public int Exp
	{
		get
		{
			return _exp;
		}
		private set
		{
			_exp = value;
			OnExpUpdated?.Invoke(value);
		}
	}

	public int CurrentAvatarLevelExpRequirement
	{
		get
		{
			return _currentAvatarLevelExpRequirement;
		}
		private set
		{
			_currentAvatarLevelExpRequirement = value;
			OnCurrentAvatarLevelExpRequirementUpdated?.Invoke(value);
		}
	}

	public bool IsDone { get; private set; }

	public bool IsOnAnyMovingPlatform => _onMovingPlatform != null;

	public bool IsCloseToMovingPlatform => CloseToGameplayObject?.IsMovingPlatform ?? false;

	public MilMo_GameplayObject CloseToGameplayObject { get; set; }

	public bool IsLocalPlayer => true;

	public bool InInstance { get; set; }

	public bool InRoom { get; private set; }

	public bool InWorld
	{
		get
		{
			if (!InGUIApp)
			{
				return !InHome;
			}
			return false;
		}
	}

	public bool InGUIApp
	{
		get
		{
			if (!InCharBuilder && !InCharBuilderWorld && !EnteringCharBuilderWorld && !LeavingCharBuilderWorld && !InNavigator && !EnteringNavigator && !LeavingNavigator && !InShop && !EnteringShop && !LeavingShop && !InHub && !EnteringHub)
			{
				return LeavingHub;
			}
			return true;
		}
	}

	public bool EnteringOrLeaving
	{
		get
		{
			if (!EnteringHub && !LeavingHub && !EnteringNavigator && !LeavingNavigator && !EnteringCharBuilderWorld && !LeavingCharBuilderWorld && !EnteringShop && !LeavingShop)
			{
				return Teleporting;
			}
			return true;
		}
	}

	public bool InHub { get; set; }

	public bool EnteringHub { get; set; }

	public bool LeavingHub { get; set; }

	public MilMo_TimerEvent PendingHubRequest { get; set; }

	public bool InNavigator { get; set; }

	public bool EnteringNavigator { get; set; }

	public bool LeavingNavigator { get; set; }

	public MilMo_TimerEvent PendingNavigatorRequest { get; set; }

	public bool InCharBuilder { get; set; }

	public bool InCharBuilderWorld { get; set; }

	public bool EnteringCharBuilderWorld { get; set; }

	public bool LeavingCharBuilderWorld { get; set; }

	public MilMo_TimerEvent PendingCharBuilderRequest { get; set; }

	public bool InShop
	{
		get
		{
			return _inShop;
		}
		set
		{
			bool inShop = _inShop;
			_inShop = value;
			if (inShop && !_inShop)
			{
				MilMo_EventSystem.Instance.PostEvent("shop_exited", null);
			}
		}
	}

	public bool EnteringShop { get; set; }

	public bool LeavingShop { get; set; }

	public MilMo_TimerEvent PendingShopRequest { get; set; }

	public bool Teleporting { get; set; }

	public MilMo_TimerEvent PendingTeleportRequest { get; set; }

	public bool InPVP { get; set; }

	public bool HasPendingRequests
	{
		get
		{
			if (PendingNavigatorRequest == null && PendingShopRequest == null && PendingCharBuilderRequest == null && PendingHubRequest == null)
			{
				return PendingTeleportRequest != null;
			}
			return true;
		}
	}

	public MilMo_BanList HomeBanList { get; private set; }

	public bool PendingHomeKick { get; set; }

	public static bool InHome => MilMo_Home.CurrentHome != null;

	public static bool InMyHome
	{
		get
		{
			if (MilMo_Home.CurrentHome != null)
			{
				return MilMo_Home.CurrentHome.OwnerID == Instance.Id;
			}
			return false;
		}
	}

	public TemplateReference NextHomeDeliveryBox { get; private set; }

	public DateTime NextHomeDeliveryBoxSpawnTime { get; private set; }

	public bool IsMember
	{
		get
		{
			if (Avatar == null)
			{
				return _membershipDaysLeft > -1;
			}
			return Avatar.IsMember;
		}
	}

	public int MembershipDaysLeft => _membershipDaysLeft;

	public float MembershipDaysLastSetTime => _membershipLastUpdatedTime;

	public int JuneCash { get; private set; }

	public ServerTeleportToFriendOk PendingTeleportToFriendMessage { get; private set; }

	public event Action OnStopTalkingWithNPC;

	public void CreateAvatar(string name, byte gender, string skinColor, string eyeColor, int hairColor, string mouth, string eyes, string eyeBrows, string hair, float height, string mood, List<Item> clothes, AvatarCreatedCallback callback)
	{
		if (Avatar == null && !_requestingCreateAvatar)
		{
			_requestingCreateAvatar = true;
			_avatarCreatedCallback = callback;
			MilMo_GenericReaction createResponse = MilMo_EventSystem.Listen("avatar_created", AvatarCreated);
			MilMo_GenericReaction createFailedResponse = MilMo_EventSystem.Listen("avatar_created_failed", AvatarCreatedFailed);
			_createAvatarTimeout = MilMo_EventSystem.At(25f, delegate
			{
				UnityEngine.Debug.LogWarning("Creating avatar name " + name + " timed out.");
				MilMo_EventSystem.RemoveReaction(createResponse);
				MilMo_EventSystem.RemoveReaction(createFailedResponse);
				_avatarCreatedCallback(successful: false);
			});
			Singleton<GameNetwork>.Instance.RequestCreateAvatar(name, gender, skinColor, hairColor, eyeColor, mouth, eyes, eyeBrows, hair, height, mood, clothes, MilMo_Global.AuthorizationToken);
		}
	}

	private void AvatarCreated(object obj)
	{
		if (!(obj is ServerAvatarCreated serverAvatarCreated))
		{
			return;
		}
		UnityEngine.Debug.Log("MilMo_Player::AvatarCreated");
		MilMo_Global.AuthorizationToken = serverAvatarCreated.getToken();
		MilMo_EventSystem.RemoveTimerEvent(_createAvatarTimeout);
		UnloadPlayerInfo();
		Instance.RequestLocalPlayerInfo(delegate(bool infoSuccess, ServerLocalPlayerInfo _)
		{
			if (!infoSuccess)
			{
				WarningDialog(MilMo_Localization.GetLocString("World_420"), MilMo_Localization.GetLocString("World_421"));
			}
			else
			{
				_avatarCreatedCallback(successful: true);
			}
		});
	}

	private void AvatarCreatedFailed(object msgAsObject)
	{
		UnityEngine.Debug.Log("MilMo_Player::AvatarCreatedFailed");
		_avatarCreatedCallback(successful: false);
	}

	public void RecreateAvatar(Code.Core.Network.types.Avatar avatarMsg, AvatarCreatedCallback callback)
	{
		Avatar.SetInitializedCallback(delegate
		{
			if (Silhouette != null)
			{
				UnityEngine.Object.Destroy(Silhouette.gameObject);
			}
			GameObject gameObject = new GameObject("Silhouette");
			Silhouette = gameObject.AddComponent<MilMo_AvatarSilhouette>();
			float num = Avatar.Health;
			if (num <= 1f)
			{
				num = 1f;
			}
			GlobalStates.Instance.playerState.health.Set((int)num);
			Avatar.EmoteManager.RegisterWieldCallbacks(EquipCurrentWieldItemLocal, UnequipCurrentWieldItemLocal);
			MilMo_World.Instance.Camera.HookupTransforms();
			Inventory.EquipAll();
			callback(Avatar != null);
		});
		Avatar.Recreate(avatarMsg);
	}

	private MilMo_Player()
	{
		Tweet = "";
		EquipSlots = new WeaponSlots(this);
		Quests = new MilMo_QuestManager();
		PlayerClassManager = new PlayerClassManager();
		Register_Commands();
		SetupListeners();
		MilMo_LevelInfo.StartLevelInfoSystem();
		PlayerAmmoManager = AmmoManager.GetPlayerAmmoManager();
		PlayerSkillManager = SkillManager.GetPlayerSkillManager();
		_playedSeconds = 0L;
		UpdateLastUpdatedPlayInterval();
	}

	private void SetupListeners()
	{
		MilMo_EventSystem.Listen("junecash_updated", JuneCashUpdated).Repeating = true;
		MilMo_EventSystem.Listen("voucher_points_updated", CoinsUpdated).Repeating = true;
		MilMo_EventSystem.Listen("update_gems", GemsUpdated).Repeating = true;
		MilMo_EventSystem.Listen("wield_fail", WieldFailed).Repeating = true;
		MilMo_EventSystem.Listen("npc_enter_range", EnterNpcRange).Repeating = true;
		MilMo_EventSystem.Listen("npc_leave_range", ExitNpcRange).Repeating = true;
		MilMo_EventSystem.Listen("junecash_need_update", JuneCashNeedUpdate).Repeating = true;
		MilMo_EventSystem.Listen("toggle_wearable", ToggleWearable).Repeating = true;
		MilMo_EventSystem.Listen("member_update", MemberStatusUpdated).Repeating = true;
		MilMo_EventSystem.Listen("member_update_from_webshop", GotMemberUpdateFromWebShop).Repeating = true;
		MilMo_EventSystem.Listen("teleportstones_updated", TeleportStonesUpdated).Repeating = true;
		MilMo_EventSystem.Listen("yourtweet", GotTweetUpdate).Repeating = true;
		MilMo_EventSystem.Listen("update_exposed_variable", HandleExposedVariableUpdate).Repeating = true;
		MilMo_EventSystem.Listen("player_kicked_from_home", PlayerKickedFromHome).Repeating = true;
		MilMo_EventSystem.Listen("teleport_to_friend_ok", OnTeleportToFriendSuccess).Repeating = true;
		MilMo_EventSystem.Listen("teleport_to_friend_failed", OnTeleportToFriendFailed).Repeating = true;
		MilMo_EventSystem.Listen("recreate_inventory_and_ammo", RecreateInventoryAndAmmo).Repeating = true;
		MilMo_EventSystem.Listen("player_end_invulnerable", EndInvulnerable).Repeating = true;
		MilMo_EventSystem.Listen("home_remote_player_unbanned", HomeRemotePlayerUnbanned).Repeating = true;
		MilMo_EventSystem.Listen("home_remote_player_banned", HomeRemotePlayerBanned).Repeating = true;
		MilMo_EventSystem.Listen("server_exp_update", ServerExpUpdate).Repeating = true;
		MilMo_EventSystem.Listen("server_avatarlevel_update", ServerAvatarLevelUpdate).Repeating = true;
		MilMo_EventSystem.Listen("play_emote", PlayEmote).Repeating = true;
		GameEvent.PrevWeaponEvent.RegisterAction(ToggleWieldableLeft);
		GameEvent.NextWeaponEvent.RegisterAction(ToggleWieldableRight);
		MilMo_EventSystem.Listen("playerstate_activated", HandleActivateStateMessage).Repeating = true;
		MilMo_EventSystem.Listen("playerstate_deactivated", HandleDeactivateStateMessage).Repeating = true;
		MilMo_EventSystem.Listen("spline_ride_start", SplineRideStartReaction).Repeating = true;
		MilMo_EventSystem.Listen("spline_ride_end", SplineRideEndReaction).Repeating = true;
		MilMo_EventSystem.Listen("home_delivery_box_picked_up", HomeDeliveryBoxPickedUp).Repeating = true;
		MilMo_EventSystem.Listen("creature_to_high_level_to_damage", CreatureToHighLevelToDamage).Repeating = true;
		MilMo_EventSystem.Listen("creature_to_low_level_to_xp", CreatureToLowLevelToXp).Repeating = true;
		MilMo_EventSystem.Listen("reset_afk_timer", ResetAfkTimer).Repeating = true;
		MilMo_EventSystem.Listen("set_afk", SetAfk).Repeating = true;
	}

	private void RemoveListeners()
	{
		GameEvent.PrevWeaponEvent.UnregisterAction(ToggleWieldableLeft);
		GameEvent.NextWeaponEvent.UnregisterAction(ToggleWieldableRight);
	}

	private void EndInvulnerable(object msgAsObj)
	{
		if (msgAsObj is ServerPlayerEndInvulnerability serverPlayerEndInvulnerability && !(serverPlayerEndInvulnerability.getPlayerId() != Id))
		{
			Avatar?.SetInvulnerable(value: false);
		}
	}

	public void RequestRespawn(object o)
	{
		if (IsExhausted && Time.time - _lastRespawnRequest > 3f)
		{
			_lastRespawnRequest = Time.time;
			Singleton<GameNetwork>.Instance.RequestRespawn();
		}
		else
		{
			UnityEngine.Debug.LogWarning("Can't request respawn: " + ((!IsExhausted) ? "Player is not exhausted. " : "") + ((Time.time - _lastRespawnRequest <= 3f) ? ("Last respawn request was " + (Time.time - _lastRespawnRequest) + " seconds ago.") : ""));
		}
	}

	private void PlayEmote(object emoteNameAsObj)
	{
		if (Avatar != null && !IsExhausted && emoteNameAsObj is string name)
		{
			MilMo_Emote emoteByName = MilMo_EmoteSystem.GetEmoteByName(name);
			if (emoteByName != null)
			{
				Avatar.PlayEmote(emoteByName);
			}
		}
	}

	private void EnterNpcRange(object o)
	{
		NpCsInRange++;
	}

	private void ExitNpcRange(object o)
	{
		NpCsInRange--;
		if (NpCsInRange < 0)
		{
			NpCsInRange = 0;
		}
	}

	private void CoinsUpdated(object msgAsObject)
	{
		ServerUpdateCoins serverUpdateCoins = (ServerUpdateCoins)msgAsObject;
		if (serverUpdateCoins != null)
		{
			int num = GlobalStates.Instance.playerState.coins.Get();
			int coins = serverUpdateCoins.getCoins();
			int num2 = coins - num;
			GlobalStates.Instance.playerState.coins.Set(coins);
			MilMo_EventSystem.Instance.AsyncPostEvent("voucher_points_changed");
			if (num2 > 0)
			{
				MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_ReceiveCoin");
				GameEvent.ShowCoinCounterEvent?.RaiseEvent(args: true);
			}
			if (num2 < 0)
			{
				MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_UseCoins");
				Singleton<LockStateManager>.Instance.HasUnlockedShop = true;
			}
		}
	}

	private void GemsUpdated(object msgAsObject)
	{
		ServerUpdateGems serverUpdateGems = (ServerUpdateGems)msgAsObject;
		if (serverUpdateGems == null)
		{
			return;
		}
		int num = serverUpdateGems.getGemCount() - GlobalStates.Instance.playerState.gems.Get();
		ServerUpdateGems.EventTypes eventType = serverUpdateGems.getEventType();
		short bonusGems = serverUpdateGems.getBonusGems();
		GlobalStates.Instance.playerState.gems.Set(serverUpdateGems.getGemCount());
		switch (eventType)
		{
		case ServerUpdateGems.EventTypes.GemReceived:
			if (bonusGems > 0)
			{
				GameEvent.GemsIncreasedBonusEvent.RaiseEvent(num);
			}
			else
			{
				GameEvent.GemsIncreasedEvent.RaiseEvent(num);
			}
			break;
		case ServerUpdateGems.EventTypes.GemSpent:
			GameEvent.GemsSpentEvent.RaiseEvent(num);
			break;
		case ServerUpdateGems.EventTypes.GemStolen:
			GameEvent.GemsStolenEvent.RaiseEvent(num);
			break;
		}
		MilMo_EventSystem.Instance.PostEvent("gems_changed", eventType);
		if (num > 0)
		{
			MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_ReceiveGem");
			GameEvent.ShowGemCounterEvent?.RaiseEvent(args: true);
		}
	}

	private void GotTweetUpdate(object tweetStrAsObj)
	{
		Tweet = (string)tweetStrAsObj;
	}

	private void SplineRideStartReaction(object o)
	{
		InSpline = true;
		DisableAllWieldables();
	}

	private void SplineRideEndReaction(object o)
	{
		InSpline = false;
		EnableWieldables();
	}

	private void HomeRemotePlayerBanned(object msg)
	{
		HomeBanList.SetBanned(((ServerPlayerBannedFromHome)msg).getBannedPlayer());
	}

	private void ServerExpUpdate(object msgAsObject)
	{
		if (msgAsObject is ServerExpUpdate serverExpUpdate)
		{
			int exp = serverExpUpdate.getExp();
			float num = exp - Exp;
			Exp = exp;
			MilMo_EventSystem.Instance.PostEvent("exp_updated", num);
			MilMo_EventSystem.Instance.PostEvent("tutorial_GainExp", null);
		}
	}

	private void ServerAvatarLevelUpdate(object msgAsObject)
	{
		if (!(msgAsObject is ServerAvatarLevelUpdate serverAvatarLevelUpdate))
		{
			return;
		}
		string playerId = serverAvatarLevelUpdate.getPlayerId();
		int avatarLevel = serverAvatarLevelUpdate.getAvatarLevel();
		if (playerId == Instance.Id)
		{
			CurrentAvatarLevelExpRequirement = serverAvatarLevelUpdate.getCurrentLevelExp();
			NextAvatarLevelExpRequirement = serverAvatarLevelUpdate.getNextLevelExp();
			if (Avatar != null)
			{
				Avatar.SetAvatarLevel(avatarLevel);
				Avatar.DisplayLevelUp();
			}
			if (serverAvatarLevelUpdate.getAvailableClassLevels() > 0)
			{
				PlayerClassManager.CheckForAvailableSelections();
			}
			MilMo_EventSystem.Instance.PostEvent("avatarlevel_updated", MilMo_Localization.GetLocString(serverAvatarLevelUpdate.getLevelDescription()));
			MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_LevelUp");
			return;
		}
		MilMo_Avatar milMo_Avatar = MilMo_Instance.CurrentInstance?.Avatars.FirstOrDefault((MilMo_Avatar a) => a.Id == playerId);
		if (milMo_Avatar != null)
		{
			milMo_Avatar.SetAvatarLevel(avatarLevel);
			milMo_Avatar.DisplayLevelUp();
			GroupManager groupManager = GroupManager.Get();
			if (groupManager.InGroup(playerId))
			{
				groupManager.MemberLevelUp(playerId, avatarLevel);
			}
		}
	}

	private void CreatureToHighLevelToDamage(object data)
	{
		_killingHighLevelCritters++;
		if (_killingHighLevelCritters >= 10)
		{
			DialogueSpawner.SpawnQuickInfoDialogue(new LocalizedStringWithArgument("World_9301"), new LocalizedStringWithArgument("World_9302"), "IconNoXpHigh", 4);
			_killingHighLevelCritters = 0;
		}
	}

	private void CreatureToLowLevelToXp(object data)
	{
		if (Instance.NextAvatarLevelExpRequirement > -1)
		{
			_killingLowLevelCritters++;
			if (_killingLowLevelCritters >= 10)
			{
				DialogueSpawner.SpawnQuickInfoDialogue(new LocalizedStringWithArgument("World_9303"), new LocalizedStringWithArgument("World_9304"), "IconNoXpLow", 4);
				_killingLowLevelCritters = 0;
			}
		}
	}

	private void ResetAfkTimer(object data)
	{
		_afkTimer = 0f;
		SetAfk(afk: false);
	}

	private void SetAfk(object data)
	{
		SetAfk(afk: true);
	}

	private void HomeRemotePlayerUnbanned(object msg)
	{
		HomeBanList.SetUnbanned(((ServerPlayerUnBannedFromHome)msg).getUnbannedPlayer());
	}

	private void RecreateInventoryAndAmmo(object msgAsObj)
	{
		if (msgAsObj is ServerRecreateInventoryAndAmmo serverRecreateInventoryAndAmmo)
		{
			IList<InventoryEntry> items = serverRecreateInventoryAndAmmo.getItems();
			IList<AmmoType> ammoTypes = serverRecreateInventoryAndAmmo.getAmmoTypes();
			IList<string> hotkeys = serverRecreateInventoryAndAmmo.getHotkeys();
			if (Avatar == null)
			{
				UnityEngine.Debug.Log("Got recreate inventory and ammo message before avatar is created.");
				_pendingInvetoryData = items;
				_pendingAmmoTypes = ammoTypes;
				_pendingHotkeys = hotkeys;
			}
			else
			{
				RecreateInventoryAndAmmo(items);
				RecreateAmmoTypes(ammoTypes);
				RebindHotkeys(hotkeys);
			}
		}
	}

	private void RecreateInventoryAndAmmo(IList<InventoryEntry> inventoryData)
	{
		if (Inventory == null)
		{
			Inventory = new MilMo_Inventory(this, Avatar);
		}
		else
		{
			Inventory.Clear();
		}
		Inventory.AddItems(_pendingInvetoryData ?? inventoryData);
		_pendingInvetoryData = null;
	}

	private void RecreateAmmoTypes(IList<AmmoType> ammoTypes)
	{
		PlayerAmmoManager.RecreateAmmo(_pendingAmmoTypes ?? ammoTypes);
		_pendingAmmoTypes = null;
	}

	private void RebindHotkeys(IList<string> hotkeys)
	{
		if (EquipSlots?.CurrentItem != null)
		{
			UnwieldLocal();
		}
		EquipSlots?.Clear();
		GameEvent.SetActionbarHotkeysEvent.RaiseEvent(_pendingHotkeys ?? hotkeys);
		_pendingHotkeys = null;
	}

	private void HandleExposedVariableUpdate(object updateAsObj)
	{
		if (!(updateAsObj is ExposedVariableUpdate exposedVariableUpdate))
		{
			return;
		}
		if (Avatar == null)
		{
			_pendingVariableUpdates.Add(exposedVariableUpdate);
			return;
		}
		if (exposedVariableUpdate.GetName() == "Health")
		{
			Avatar.UpdateHealth(exposedVariableUpdate.GetValue());
		}
		else if (exposedVariableUpdate.GetName() == "MaxHealth")
		{
			Avatar.UpdateMaxHealth(exposedVariableUpdate.GetValue());
		}
		else
		{
			Avatar.SetVariableValue(exposedVariableUpdate.GetName(), exposedVariableUpdate.GetValue());
		}
		if (exposedVariableUpdate.GetName() == "RunSpeed" || exposedVariableUpdate.GetName() == "WalkSpeed" || exposedVariableUpdate.GetName() == "SwimSpeed")
		{
			MilMo_PlayerControllerBase.SetMovementMode();
		}
	}

	private void HandleActivateStateMessage(object msgAsObj)
	{
		if (msgAsObj is ServerActivatePlayerState serverActivatePlayerState && !(serverActivatePlayerState.getPlayerId() != Instance.Avatar.Id))
		{
			HandleExposedVariableUpdate(serverActivatePlayerState.getVariableUpdate());
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(serverActivatePlayerState.getStateTemplate(), ActivateState);
		}
	}

	private void HandleDeactivateStateMessage(object msgAsObj)
	{
		if (msgAsObj is ServerDeactivatePlayerState serverDeactivatePlayerState && !(serverDeactivatePlayerState.getPlayerId() != Instance.Avatar.Id))
		{
			HandleExposedVariableUpdate(serverDeactivatePlayerState.getVariableUpdate());
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(serverDeactivatePlayerState.getStateTemplate(), DeactivateState);
		}
	}

	public static void LogoutGame(LogoutCallback callback)
	{
		Singleton<GameNetwork>.Instance.DisconnectFromGameServer();
		MilMo_EventSystem.At(2f, delegate
		{
			callback();
		});
	}

	public static void LoginGame(string host, string token, LoginCallback callback)
	{
		MilMo_EventSystem.Listen("login_info_game", delegate(object loginResponseAsObject)
		{
			LoginResponse loginResponse = (LoginResponse)loginResponseAsObject;
			callback(new MilMo_LoginInfo(loginResponse));
		});
		Singleton<GameNetwork>.Instance.ConnectToGameServer(host, token);
	}

	public void SendPlayerFrame()
	{
		if (Avatar != null && (bool)Avatar.GameObject)
		{
			Vector3 position = Avatar.GameObject.transform.position;
			Vector3 velocity = Avatar.Controller.velocity;
			Singleton<GameNetwork>.Instance.SendPlayerUpdate(position, velocity);
			Singleton<Analytics>.Instance.PositionUpdateSent();
		}
	}

	public void SendPlayerStopFrame()
	{
		if (Avatar != null && (bool)Avatar.GameObject)
		{
			Vector3 position = Avatar.GameObject.transform.position;
			float y = MilMo_PlayerControllerBase.TargetRotation.eulerAngles.y;
			Singleton<GameNetwork>.Instance.SendPlayerStop(position, y);
		}
	}

	public void SetHasPlayed()
	{
		if (!_hasPlayed)
		{
			_hasPlayed = true;
			Singleton<GameNetwork>.Instance.SendToGameServer(new ClientSetHasPlayed());
		}
	}

	public static void WarningDialog(MilMo_LocString caption, MilMo_LocString message)
	{
		MilMo_Dialog dialogue = new MilMo_Dialog(MilMo_Fade.Instance.UserInterface);
		MilMo_Fade.Instance.UserInterface.AddChild(dialogue);
		dialogue.DoOK("Batch01/Textures/Dialog/Warning", caption, message, delegate
		{
			dialogue.CloseAndRemove(null);
		});
		dialogue.IgnoreGlobalFade = true;
	}

	public void UpdateAfkTimer()
	{
		_afkTimer += 1f;
		if (_afkTimer > 300f)
		{
			SetAfk(afk: true);
		}
	}

	private void SetAfk(bool afk)
	{
		if (afk == _afk)
		{
			return;
		}
		_afk = afk;
		Singleton<GameNetwork>.Instance.SendClientSetAfk((sbyte)(afk ? 1 : 0));
		MilMo_MoodHandler milMo_MoodHandler = Avatar?.MoodHandler;
		if (milMo_MoodHandler != null)
		{
			if (_afk)
			{
				Mood moodByKey = milMo_MoodHandler.GetMoodByKey("Sad");
				milMo_MoodHandler.SetMood(moodByKey, send: false, persist: false);
			}
			else
			{
				milMo_MoodHandler.RestorePersistedMood();
			}
		}
	}

	private void ToggleWearable(object inventoryIdAsObject)
	{
		if (IsExhausted)
		{
			return;
		}
		if (Avatar == null || Inventory == null)
		{
			UnityEngine.Debug.LogWarning("Avatar or Inventory is null when trying to toggle wearable item on local player");
			return;
		}
		int num = (int)inventoryIdAsObject;
		MilMo_InventoryEntry entry = Inventory.GetEntry(num);
		if (entry == null)
		{
			UnityEngine.Debug.LogWarning("Failed to toggle wearable. Item does not exist in the inventory.");
			return;
		}
		if (!(entry.Item is MilMo_Wearable milMo_Wearable))
		{
			UnityEngine.Debug.LogWarning("Trying to toggle wearable for " + entry.Item.Template.Identifier + ". It is not a wearable item.");
			return;
		}
		if (entry.IsEquipped)
		{
			Avatar.UnequipLocal(milMo_Wearable);
			Avatar.AsyncApply();
			Singleton<GameNetwork>.Instance.SendUnequipUpdate(num);
			if (Instance.EquipSlots.CurrentItemInventoryId == entry.Id)
			{
				Instance.EquipSlots.UnwieldCurrent();
			}
			return;
		}
		if (milMo_Wearable is MilMo_Wieldable milMo_Wieldable)
		{
			Instance.EquipSlots.Wield(milMo_Wieldable.Identifier);
		}
		for (MilMo_InventoryEntry equippedEntryByBodyPackCategory = Inventory.GetEquippedEntryByBodyPackCategory(milMo_Wearable); equippedEntryByBodyPackCategory != null; equippedEntryByBodyPackCategory = Inventory.GetEquippedEntryByBodyPackCategory(milMo_Wearable))
		{
			equippedEntryByBodyPackCategory.IsEquipped = false;
			Singleton<GameNetwork>.Instance.SendUnequipUpdate(equippedEntryByBodyPackCategory.Id);
		}
		Avatar.EquipLocal(milMo_Wearable);
		Avatar.AsyncApply();
		Singleton<GameNetwork>.Instance.SendEquipUpdate(num);
	}

	public void StartClimbing(MilMo_ClimbingSurface.MilMo_AttachInfo attachInfo)
	{
		if (ClimbingSurface != null)
		{
			UnityEngine.Debug.LogWarning("Trying to start climbing when already climbing");
		}
		if (attachInfo.ClimbingSurface == null)
		{
			UnityEngine.Debug.LogWarning("Trying to start climbing without a valid climbing surface.");
			return;
		}
		DisableAllWieldables();
		Avatar.DisableBlobShadows();
		ClimbingSurface = attachInfo.ClimbingSurface;
		MilMo_World.Instance.ChangePlayerController(MilMo_PlayerControllerBase.ControllerType.Climb, attachInfo);
		Singleton<GameNetwork>.Instance.SendPlayerStartClimb(ClimbingSurface.Identifier);
	}

	public void EndClimbing()
	{
		if (ClimbingSurface == null)
		{
			UnityEngine.Debug.LogWarning("Trying to stop climbing when not climbing");
			return;
		}
		EnableWieldables();
		Avatar.EnableBlobShadows();
		ClimbingSurface = null;
		Singleton<GameNetwork>.Instance.SendPlayerStopClimb();
	}

	public void EnterGameplayObject(MilMo_GameplayObject gameplayObject)
	{
		if (gameplayObject != _onMovingPlatform)
		{
			if (_onMovingPlatform != null)
			{
				LeaveGameplayObject();
			}
			_onMovingPlatform = gameplayObject;
			Singleton<GameNetwork>.Instance.SendPlayerEnterGameplayObject(gameplayObject.Id);
		}
	}

	public void LeaveGameplayObject()
	{
		_onMovingPlatform = null;
		Singleton<GameNetwork>.Instance.SendPlayerLeaveGameplayObject();
	}

	public void EnterRoom()
	{
		InRoom = true;
		SetWieldableModeFood();
	}

	public void ExitRoom()
	{
		InRoom = false;
		SetWieldableModeAll();
	}

	public void Destroy()
	{
		RemoveListeners();
		MilMo_EventSystem.RemoveReaction(DisconnectListener);
		Avatar?.Destroy();
	}

	public void Update()
	{
		Avatar.Update();
	}

	public bool IsNewPlayer()
	{
		return (DateTime.Now - MilMo_Utility.GetDateTimeFromMilliseconds(_lastUpdatedPlayInterval).ToLocalTime()).Add(TimeSpan.FromSeconds(_playedSeconds)).TotalHours < 2.0;
	}

	public void ExitAllNpcRange()
	{
		NpCsInRange = 0;
	}

	public void PlayDigEffects()
	{
		if (EquipSlots?.CurrentItem != null)
		{
			string randomWieldAnimation = EquipSlots.CurrentItem.GetRandomWieldAnimation();
			Avatar.PlayAnimation(randomWieldAnimation);
			Avatar.EmitPuff("Dig");
		}
	}

	public void PlayEatEffects(sbyte useNumber)
	{
		if (!(EquipSlots.CurrentItem is MilMo_WieldableFood milMo_WieldableFood))
		{
			return;
		}
		milMo_WieldableFood.UpdateLastEatTime();
		string useEmote = milMo_WieldableFood.GetUseEmote(useNumber);
		if (!string.IsNullOrEmpty(useEmote))
		{
			MilMo_Emote milMo_Emote = null;
			if (InRoom)
			{
				milMo_Emote = MilMo_EmoteSystem.GetEmoteByName(useEmote + "Indoor");
			}
			if (milMo_Emote == null)
			{
				milMo_Emote = MilMo_EmoteSystem.GetEmoteByName(useEmote);
			}
			if (milMo_Emote != null)
			{
				Avatar.PlayEmoteLocal(milMo_Emote);
			}
		}
	}

	private void SetRespawnReaction()
	{
		if (_respawnButtonReaction == null)
		{
			_respawnButtonReaction = MilMo_EventSystem.Listen("button_Respawn", RequestRespawn);
			_respawnButtonReaction.Repeating = true;
		}
	}

	private void RemoveRespawnReaction()
	{
		if (_respawnButtonReaction != null)
		{
			MilMo_EventSystem.RemoveReaction(_respawnButtonReaction);
			_respawnButtonReaction = null;
		}
	}

	public void Respawn(float newHealth, Vector3 position, float rotation, bool setInvulnerable)
	{
		UnityEngine.Debug.Log("Player respawn with new health " + newHealth);
		RemoveRespawnReaction();
		if (MilMo_LoadingScreen.Instance.LoadingState == MilMo_LoadingScreen.State.Disabled)
		{
			MilMo_LoadingScreen.Instance.RespawnFade(1f);
		}
		MilMo_EventSystem.At(0.75f, delegate
		{
			if (Avatar != null)
			{
				Avatar.DisableRagdoll();
				MilMo_World.Instance.PlayerController.Respawn();
				Avatar.UpdateHealth(newHealth);
				IsExhausted = false;
				if (MilMo_World.Instance.PlayerController.Type == MilMo_PlayerControllerBase.ControllerType.Climb)
				{
					MilMo_World.Instance.ChangePlayerController(((MilMo_PlayerControllerClimb)MilMo_World.Instance.PlayerController).PreviousControllerType);
				}
				EnableWieldables();
				Avatar.UnstackAnimation("LandIdle", "Down01", unstackAll: true);
				Avatar.PlayAnimation(Avatar.IdleAnimation);
				Avatar.GameObject.transform.position = position;
				Vector3 vector = position;
				UnityEngine.Debug.Log("Set player position to " + vector.ToString());
				MilMo_PlayerControllerBase.SetRotation(Quaternion.AngleAxis(rotation, Vector3.up));
				if (setInvulnerable)
				{
					Avatar.SetInvulnerable(value: true);
				}
			}
		});
	}

	private void ActivateState(MilMo_Template template, bool timeout)
	{
		if (timeout || !(template is MilMo_PlayerStateTemplate milMo_PlayerStateTemplate))
		{
			UnityEngine.Debug.LogWarning("Failed to activate player state: State template could not be found");
			return;
		}
		new MilMo_PlayerState(Avatar, milMo_PlayerStateTemplate).Activate();
		MilMo_EventSystem.Instance.PostEvent("tutorial_EnterPlayerState", milMo_PlayerStateTemplate.Name);
	}

	private void DeactivateState(MilMo_Template template, bool timeout)
	{
		if (timeout || !(template is MilMo_PlayerStateTemplate))
		{
			UnityEngine.Debug.LogWarning("Failed to deactivate player state: State template could not be found");
		}
		else
		{
			Avatar.GetActiveState(template.Identifier)?.Deactivate();
		}
	}

	public void Exhausted()
	{
		MilMo_EventSystem.At(InPVP ? 7f : 3f, delegate
		{
			if (InPVP)
			{
				RequestRespawn(null);
			}
			else
			{
				SetRespawnReaction();
			}
		});
		if (!IsExhausted)
		{
			if (IsClimbing && MilMo_World.Instance.PlayerController is MilMo_PlayerControllerClimb milMo_PlayerControllerClimb)
			{
				milMo_PlayerControllerClimb.ExhaustedKnockoff();
			}
			Analytics.Exhausted(MilMo_Level.CurrentLevel.VerboseName, Avatar.Position);
			IsExhausted = true;
			if (Avatar.InCombat)
			{
				MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_Die", "");
			}
			Avatar.RemoveFromCombat();
			DisableAllWieldables();
		}
		MilMo_World.Instance.PlayerController.Lock(0f, playMoveAnimationOnUnlock: true);
	}

	public bool IsOnMovingPlatform(MilMo_GameplayObject gameplayObject)
	{
		return gameplayObject == _onMovingPlatform;
	}

	public bool CanUseConverter(MilMo_Converter converter)
	{
		if (GlobalStates.Instance.playerState.gems.Get() < converter.Template.RequiredGems)
		{
			return false;
		}
		if (converter.Template.RequiredTool != null)
		{
			string templateIdentifier = converter.Template.RequiredTool.GetCategory() + ":" + converter.Template.RequiredTool.GetPath();
			if (Inventory.GetEntry(templateIdentifier) == null)
			{
				return false;
			}
		}
		foreach (TemplateCountPair ingredient in converter.Template.Ingredients)
		{
			TemplateReference template = ingredient.GetTemplate();
			string templateIdentifier2 = template.GetCategory() + ":" + template.GetPath();
			MilMo_InventoryEntry entry = Inventory.GetEntry(templateIdentifier2);
			if (entry == null || entry.Amount < ingredient.GetCount())
			{
				return false;
			}
		}
		return true;
	}

	public void UpdateInDialogue(bool inDialogue)
	{
		bool inDialogue2 = InDialogue;
		InDialogue = inDialogue;
		if (inDialogue2 != InDialogue && inDialogue2)
		{
			this.OnStopTalkingWithNPC?.Invoke();
		}
	}

	public void UpdateIsTalking(bool isTalking)
	{
		IsTalking = isTalking;
	}

	private void Register_Commands()
	{
		MilMo_Command.Instance.RegisterCommand("Emote", Debug_PlayEmote);
		MilMo_Command.Instance.RegisterCommand("RemotePlayer.DebugMovement", Debug_RemotePlayerDebugMovement);
		MilMo_Command.Instance.RegisterCommand("Player.TeleportToSelf", Debug_TeleportToSelf);
		MilMo_Command.Instance.RegisterCommand("Player.TeleportToFriend", Debug_TeleportToFriend);
		MilMo_Command.Instance.RegisterCommand("Player.Avatar.Ragdoll", delegate(string[] args)
		{
			if (args.Length < 2)
			{
				return "usage: Player.Avatar.Ragdoll <1|0>";
			}
			if (Instance.Avatar == null)
			{
				return "No avatar";
			}
			float num = 1f;
			if (args.Length > 2)
			{
				num = MilMo_Utility.StringToFloat(args[2]);
			}
			if (MilMo_Utility.StringToBool(args[1]))
			{
				Vector3 force = new Vector3(MilMo_Utility.Random(), MilMo_Utility.Random(), MilMo_Utility.Random()) * num;
				Instance.Avatar.EnableRagdoll(force, ForcePosition.Torso);
				return "Ragdoll activated";
			}
			Instance.Avatar.DisableRagdoll();
			return "Ragdoll deactivated";
		});
		MilMo_Command.Instance.RegisterCommand("Debug.AFK", delegate
		{
			SetAfk(!_afk);
			return _afk.ToString();
		});
		MilMo_Command.Instance.RegisterCommand("Debug.PrintTeams", (string[] _003Cp0_003E) => MilMo_Level.CurrentLevel.PvpHandler.Teams.Values.Aggregate("", (string current, MilMo_PVPTeam team) => current + "Team " + team.Id + ": [ " + team.Players.Aggregate((string a, string b) => a + ", " + b) + " ]"));
	}

	private static string Debug_PlayEmote(string[] args)
	{
		if (args.Length < 2)
		{
			return "usage: Emote <emote name>";
		}
		if (Instance?.Avatar == null)
		{
			return "No valid player to play emote on";
		}
		MilMo_Emote emoteByName = MilMo_EmoteSystem.GetEmoteByName(args[1]);
		if (emoteByName == null)
		{
			return "No such emote";
		}
		Instance.Avatar.PlayEmote(emoteByName);
		return "Playing emote " + emoteByName?.ToString() + "...";
	}

	private static string Debug_RemotePlayerDebugMovement(string[] args)
	{
		if (args.Length < 2)
		{
			return MilMo_RemotePlayer.DebugMovement.ToString();
		}
		MilMo_RemotePlayer.DebugMovement = MilMo_Utility.StringToInt(args[1]) != 0;
		return MilMo_RemotePlayer.DebugMovement.ToString();
	}

	private static string Debug_TeleportToSelf(string[] args)
	{
		Singleton<GameNetwork>.Instance.RequestTeleportToFriend(int.Parse(Instance.Id));
		return "Teleporting to player " + Instance.Id;
	}

	private static string Debug_TeleportToFriend(string[] args)
	{
		if (args.Length < 2)
		{
			return "usage: TeleportToFriend <friendId>";
		}
		Singleton<GameNetwork>.Instance.RequestTeleportToFriend(int.Parse(args[1]));
		return "Teleporting to player " + args[1];
	}

	private static void SetPendingRequestCooldown(MilMo_TimerEvent target)
	{
		if (target == null)
		{
			target = MilMo_EventSystem.At(5f, delegate
			{
				target = null;
			});
		}
	}

	public void RequestEnterHub()
	{
		if (!InHub && !HasPendingRequests && !EnteringOrLeaving)
		{
			SetPendingRequestCooldown(PendingHubRequest);
			Singleton<GameNetwork>.Instance.RequestEnterHub();
		}
	}

	public void RequestLeaveHub()
	{
		if (InHub && !HasPendingRequests && !EnteringOrLeaving)
		{
			SetPendingRequestCooldown(PendingHubRequest);
			Singleton<GameNetwork>.Instance.RequestLeaveHub();
		}
	}

	public void RequestEnterNavigator()
	{
		if (!InNavigator && !HasPendingRequests && !EnteringOrLeaving)
		{
			SetPendingRequestCooldown(PendingNavigatorRequest);
			Singleton<GameNetwork>.Instance.RequestEnterWorldMap();
		}
	}

	public void RequestLeaveNavigator()
	{
		if (InNavigator && !HasPendingRequests && !EnteringOrLeaving)
		{
			SetPendingRequestCooldown(PendingNavigatorRequest);
			Singleton<GameNetwork>.Instance.RequestLeaveWorldMap();
		}
	}

	public void RequestEnterCharBuilder(int npcId = -1)
	{
		if (!InCharBuilderWorld && !HasPendingRequests && !EnteringOrLeaving)
		{
			SetPendingRequestCooldown(PendingCharBuilderRequest);
			Singleton<GameNetwork>.Instance.AcceptNpcEnterCharBuilder(npcId);
		}
	}

	public void RequestLeaveCharBuilder()
	{
		if (InCharBuilderWorld && !HasPendingRequests && !EnteringOrLeaving)
		{
			SetPendingRequestCooldown(PendingCharBuilderRequest);
			Singleton<GameNetwork>.Instance.RequestLeaveCharBuilder();
		}
	}

	public void RequestEnterShop()
	{
		if (!_shopEnabled)
		{
			MilMo_HubItemFunctionality.ShowComingSoonPopup();
		}
		else if (!InShop && !HasPendingRequests && !EnteringOrLeaving)
		{
			SetPendingRequestCooldown(PendingShopRequest);
			Singleton<GameNetwork>.Instance.RequestEnterShop(MilMo_Monetization.Instance.Currency.Id);
		}
	}

	public void RequestLeaveShop()
	{
		if (InShop && !HasPendingRequests && !EnteringOrLeaving)
		{
			SetPendingRequestCooldown(PendingShopRequest);
			Singleton<GameNetwork>.Instance.RequestLeaveShop();
		}
	}

	public void RequestTeleportToFriend(string friendPlayerId)
	{
		if (!Teleporting && !HasPendingRequests && !EnteringOrLeaving)
		{
			SetPendingRequestCooldown(PendingTeleportRequest);
			Singleton<GameNetwork>.Instance.RequestTeleportToFriend(int.Parse(friendPlayerId));
		}
	}

	public void RequestMemberTeleport(int npcId = -1)
	{
		if (!Teleporting && !HasPendingRequests && !EnteringOrLeaving)
		{
			SetPendingRequestCooldown(PendingTeleportRequest);
			Singleton<GameNetwork>.Instance.AcceptNpcMemberTeleport(npcId);
		}
	}

	public void RequestLevelChange(int npcId, int levelIndex)
	{
		if (!Teleporting && !HasPendingRequests && !EnteringOrLeaving)
		{
			SetPendingRequestCooldown(PendingTeleportRequest);
			Singleton<GameNetwork>.Instance.SendAcceptLevelChangeOffer(npcId, levelIndex);
		}
	}

	public bool OkToTeleport()
	{
		if (!Teleporting && !HasPendingRequests && !EnteringOrLeaving && !InSpline)
		{
			MilMo_Avatar avatar = Avatar;
			if (avatar != null)
			{
				return !avatar.InCombat;
			}
			return false;
		}
		return false;
	}

	public bool OkToEnterHub()
	{
		if (!InHub && !HasPendingRequests && !EnteringOrLeaving && !InSpline && !InputSwitch.IsDisabled())
		{
			MilMo_Avatar avatar = Avatar;
			if (avatar != null)
			{
				return !avatar.InCombat;
			}
			return false;
		}
		return false;
	}

	public bool OkToLeaveHub()
	{
		if (InHub && !HasPendingRequests)
		{
			return !EnteringOrLeaving;
		}
		return false;
	}

	public bool OkToEnterNavigator()
	{
		if (!InNavigator && !HasPendingRequests && !EnteringOrLeaving && !InCharBuilderWorld && !InPVP && !InSpline)
		{
			MilMo_Avatar avatar = Avatar;
			if (avatar != null && !avatar.InCombat)
			{
				return !InputSwitch.IsDisabled();
			}
		}
		return false;
	}

	public bool OkToLeaveNavigator()
	{
		if (InNavigator && !HasPendingRequests)
		{
			return !EnteringOrLeaving;
		}
		return false;
	}

	public bool OkToEnterCharBuilder()
	{
		if (!InCharBuilderWorld && !HasPendingRequests && !EnteringOrLeaving && !InSpline && !InputSwitch.IsDisabled())
		{
			MilMo_Avatar avatar = Avatar;
			if (avatar != null)
			{
				return !avatar.InCombat;
			}
			return false;
		}
		return false;
	}

	public bool OkToLeaveCharBuilder()
	{
		if (InCharBuilderWorld && !HasPendingRequests)
		{
			return !EnteringOrLeaving;
		}
		return false;
	}

	public bool OkToEnterShop()
	{
		if (!InShop && !HasPendingRequests && !EnteringOrLeaving && !InSpline && !InputSwitch.IsDisabled())
		{
			MilMo_Avatar avatar = Avatar;
			if (avatar != null)
			{
				return !avatar.InCombat;
			}
			return false;
		}
		return false;
	}

	public bool OkToLeaveShop()
	{
		if (InShop && !HasPendingRequests)
		{
			return !EnteringOrLeaving;
		}
		return false;
	}

	public static void ShowKickedFromHomeQuickInfo()
	{
		DialogueSpawner.SpawnQuickInfoDialogue(new LocalizedStringWithArgument("Homes_6809"), new LocalizedStringWithArgument("Homes_6810"), "Kick60", 7);
	}

	private void PlayerKickedFromHome(object msgAsObject)
	{
		if (!(msgAsObject is ServerPlayerKickedFromHome serverPlayerKickedFromHome) || serverPlayerKickedFromHome.getPlayerId() != Id || !InHome)
		{
			return;
		}
		if (!InNavigator && !InShop && !InCharBuilderWorld && !InHub)
		{
			MilMo_LoadingScreen.Instance.LevelLoadFade(12f);
			MilMo_LevelInfoData levelInfo = MilMo_LevelInfo.GetLevelInfoData(MilMo_Level.LastAdventureLevel);
			MilMo_EventSystem.At(0.25f, delegate
			{
				MilMo_LocString locString = MilMo_Localization.GetLocString("Generic_56");
				locString.SetFormatArgs(levelInfo.DisplayName);
				MilMo_LoadingScreen.Instance.SetLoadingText(locString);
				MilMo_LevelData.LoadAndSetLevelIcon(levelInfo.World, levelInfo.Level);
				PendingHomeKick = false;
				ShowKickedFromHomeQuickInfo();
			});
		}
		else
		{
			PendingHomeKick = true;
		}
	}

	private void HomeDeliveryBoxPickedUp(object msgAsObj)
	{
		if (msgAsObj is ServerHomeDeliveryBoxPickedUp serverHomeDeliveryBoxPickedUp && !(serverHomeDeliveryBoxPickedUp.getPlayerId() != Id))
		{
			NextHomeDeliveryBox = serverHomeDeliveryBoxPickedUp.getNextDeliveryBox();
			long millisecondsUntilNextBoxSpawn = serverHomeDeliveryBoxPickedUp.getMillisecondsUntilNextBoxSpawn();
			NextHomeDeliveryBoxSpawnTime = DateTime.Now.AddMilliseconds(millisecondsUntilNextBoxSpawn);
			if (millisecondsUntilNextBoxSpawn > 0)
			{
				MilMo_EventSystem.At((float)millisecondsUntilNextBoxSpawn / 1000f, HomeDeliveryTimerExpired);
			}
			string iconKey = NextHomeDeliveryBox.GetIdentifier().Replace("Item:", "Icon");
			int num = Mathf.RoundToInt((float)millisecondsUntilNextBoxSpawn / 60000f);
			LocalizedStringWithArgument message = new LocalizedStringWithArgument("World_8826", num.ToString());
			DialogueSpawner.SpawnQuickInfoDialogue(new LocalizedStringWithArgument("World_8827"), message, iconKey);
			MilMo_Hub.Instance.SetNeedsNewInfo();
		}
	}

	private void HomeDeliveryTimerExpired()
	{
		if (MilMo_World.HudHandler != null && MilMo_World.HudHandler.hudState == HudState.States.Normal)
		{
			return;
		}
		MilMo_Hub.Instance.SetNeedsNewInfo();
		string identifier = NextHomeDeliveryBox.GetIdentifier();
		string iconKey = "Icon" + identifier;
		DialogueButtonInfo callToAction = null;
		if (!InMyHome || InGUIApp)
		{
			callToAction = new DialogueButtonInfo(delegate
			{
				MilMo_World.Instance.GoToHome(Id, Avatar.Name);
			}, new LocalizedStringWithArgument("LoginScreen_16"));
		}
		DialogueSpawner.SpawnQuickInfo(new LocalizedStringWithArgument("World_8828"), new LocalizedStringWithArgument("World_8829"), 5, iconKey, "GamePlayDialogInviteJingleSound", callToAction);
	}

	public void SetMembershipDaysLeft(int value)
	{
		_membershipDaysLeft = value;
		if (Avatar != null)
		{
			Avatar.MembershipDaysLeft = value;
		}
		_membershipLastUpdatedTime = Time.time;
	}

	private static void GotMemberUpdateFromWebShop(object o)
	{
		Singleton<GameNetwork>.Instance.RequestBecameMemberVerification();
		UnityEngine.Debug.Log("Network.Instance.RequestNotifications called");
		Singleton<GameNetwork>.Instance.RequestNotifications();
	}

	private static void MemberStatusUpdated(object msgAsObject)
	{
		if (!(msgAsObject is ServerMemberUpdate serverMemberUpdate))
		{
			return;
		}
		if (serverMemberUpdate.getMembershipTimeLeftInDays() >= 0)
		{
			Instance.SetMembershipDaysLeft(serverMemberUpdate.getMembershipTimeLeftInDays() + 1);
		}
		if (serverMemberUpdate.getGemBonusTimeLeftInSeconds() > 0)
		{
			GlobalStates.Instance.playerState.gemBonusTimeLeftInSeconds.Set(serverMemberUpdate.getGemBonusTimeLeftInSeconds());
		}
		MilMo_EventSystem.Instance.PostEvent("member_status_activated", null);
		if (MilMo_Level.CurrentLevel != null)
		{
			MilMo_LevelInfoData levelInfoData = MilMo_LevelInfo.GetLevelInfoData(MilMo_Level.CurrentLevel.VerboseName);
			if (levelInfoData != null && levelInfoData.PremiumToken != null && levelInfoData.PremiumToken.GetIsFound() == 0)
			{
				MilMo_EventSystem.Instance.PostEvent("level_premiumtoken_create", levelInfoData.PremiumToken);
			}
		}
	}

	public void OpenMembershipShop()
	{
		RequestEnterShop();
		MilMo_CharacterShop.SelectItem("Shop:Batch01.Subscriptions.SubscriptionSixMonths");
	}

	public void RequestLocalPlayerInfo(LocalPlayerInfoCallback infoCallback)
	{
		_localPlayerInfoCallback = infoCallback;
		_localPlayerInfoListener = MilMo_EventSystem.Listen("player_info", LocalPlayerInfoArrived);
		if (!Singleton<GameNetwork>.Instance.RequestPlayerInfo())
		{
			MilMo_EventSystem.RemoveReaction(_localPlayerInfoListener);
			infoCallback(success: false, null);
		}
		_playerInfoTimeOutListener = MilMo_EventSystem.At(40f, delegate
		{
			MilMo_EventSystem.RemoveReaction(_localPlayerInfoListener);
			Singleton<GameNetwork>.Instance.DisconnectFromGameServer();
			infoCallback(success: false, null);
		});
	}

	private bool IsPlayerUsingCheatEngine()
	{
		try
		{
			Process[] processes = Process.GetProcesses();
			foreach (Process process in processes)
			{
				if (process != null && !string.IsNullOrEmpty(process.ProcessName) && process.ProcessName.ToLower().Contains("cheat") && process.ProcessName.ToLower().Contains("engine"))
				{
					return true;
				}
			}
		}
		catch (Exception)
		{
		}
		return false;
	}

	private void LocalPlayerInfoArrived(object obj)
	{
		if (IsPlayerUsingCheatEngine())
		{
			Singleton<GameNetwork>.Instance.DisconnectFromGameServer();
		}
		else if (obj is ServerLocalPlayerInfo serverLocalPlayerInfo)
		{
			MilMo_EventSystem.RemoveTimerEvent(_playerInfoTimeOutListener);
			_pendingInvetoryData = null;
			_pendingAmmoTypes = null;
			_pendingHotkeys = null;
			Id = serverLocalPlayerInfo.GetId();
			UnityEngine.Debug.Log("Player id is " + Id);
			JuneCash = serverLocalPlayerInfo.GetJuneCoins();
			MilMo_JuneCash.Instance.UserAccountBalance = JuneCash;
			GlobalStates.Instance.playerState.coins.Set(serverLocalPlayerInfo.GetCoins());
			MilMo_EventSystem.Instance.PostEvent("voucher_points_changed", null);
			if (serverLocalPlayerInfo.GetMembershipTimeLeftInDays() >= 0)
			{
				_membershipDaysLeft = serverLocalPlayerInfo.GetMembershipTimeLeftInDays() + 1;
			}
			_memberShipStatus = serverLocalPlayerInfo.GetMembershipStatus();
			_role = serverLocalPlayerInfo.GetRole();
			MilMo_NewsManager.Instance.ReadNews(serverLocalPlayerInfo.GetMessageOfTheDay(), serverLocalPlayerInfo.GetNewsImage(), serverLocalPlayerInfo.GetNewsImageLink(), serverLocalPlayerInfo.GetFeaturedNews());
			_hasPlayed = serverLocalPlayerInfo.GetHasPlayed() != 0;
			_playedSeconds = serverLocalPlayerInfo.PlayedSeconds;
			UpdateLastUpdatedPlayInterval();
			MilMo_SlideManager.Instance.SetSeenSlides(serverLocalPlayerInfo.GetSeenSlides());
			MilMoFriendInviteDialog.GetInstance().SetItems(serverLocalPlayerInfo.GetSentInviteRewards(), serverLocalPlayerInfo.GetAcceptedInviteRewards());
			if (QuestLog == null)
			{
				QuestLog = new MilMo_QuestLog(MilMo_GlobalUI.GetSystemUI);
				QuestLog.IsActive = false;
			}
			RebindHotkeys(serverLocalPlayerInfo.GetHotkeys());
			if (serverLocalPlayerInfo.HasAvatar())
			{
				ReadAvatarFromPlayerInfo(serverLocalPlayerInfo.GetAvatar());
			}
			MilMo_Monetization.Instance.RequestExchangeRate();
			_localPlayerInfoCallback(success: true, serverLocalPlayerInfo);
			MilMo_EventSystem.Instance.PostEvent("player_info_loaded", null);
		}
	}

	private async void ReadAvatarFromPlayerInfo(FullAvatar fullAvatar)
	{
		UnityEngine.Debug.Log("Got avatar in player info message");
		Exp = fullAvatar.GetExp();
		CurrentAvatarLevelExpRequirement = fullAvatar.GetAvatarLevelExpRequirement();
		NextAvatarLevelExpRequirement = fullAvatar.GetNextAvatarLevelExpRequirement();
		QuestLog.Gender = fullAvatar.GetGender();
		NextHomeDeliveryBox = fullAvatar.GetNextHomeDeliveryBox();
		if (NextHomeDeliveryBox != null)
		{
			if (!(await Singleton<MilMo_TemplateContainer>.Instance.GetTemplateAsync(NextHomeDeliveryBox) is MilMo_ItemTemplate milMo_ItemTemplate))
			{
				return;
			}
			milMo_ItemTemplate.Instantiate(new Dictionary<string, string>()).AsyncGetIcon(delegate
			{
			});
		}
		NextHomeDeliveryBoxSpawnTime = DateTime.Now.AddMilliseconds(fullAvatar.GetMillisecondsToNextHomeDelivery());
		if (fullAvatar.GetMillisecondsToNextHomeDelivery() > 0)
		{
			MilMo_EventSystem.At((float)fullAvatar.GetMillisecondsToNextHomeDelivery() / 1000f, HomeDeliveryTimerExpired);
		}
		Avatar = new MilMo_Avatar(this, Id, isLocalAvatar: true, _membershipDaysLeft, _role, fullAvatar.GetTitle(), fullAvatar.GetAvatarLevel());
		Avatar.SetInitializedCallback(delegate
		{
			UnityEngine.Debug.Log("Avatar initialized");
			GameObject gameObject = new GameObject("Silhouette")
			{
				layer = 0
			};
			Silhouette = gameObject.AddComponent<MilMo_AvatarSilhouette>();
			RecreateInventoryAndAmmo(fullAvatar.GetItems());
			RecreateAmmoTypes(fullAvatar.GetAmmoTypes());
			Quests.ReadAll(fullAvatar.GetActiveQuests(), fullAvatar.GetCompletedQuests());
			if (_memberShipStatus == -1)
			{
				DialogueSpawner.SpawnGoToShopDialogue(new EnterShopModalMessageData(new LocalizedStringWithArgument("CharacterShop_253"), new LocalizedStringWithArgument("World_8133"), "Shop:Batch01.Subscriptions.SubscriptionSixMonths", new AddressableSpriteLoader("IconPremium")));
			}
			Singleton<MilMo_TutorialManager>.Instance.ReadAll(fullAvatar.GetCompletedTutorials());
			MilMo_LevelInfo.ReadAll(fullAvatar.GetLevelStates(), fullAvatar.GetWorldLevels(), fullAvatar.GetCurrentExplorationTokens(), fullAvatar.GetCurrentCoinTokens(), fullAvatar.GetPremiumTokens());
			Singleton<MilMo_AchievementHandler>.Instance.ReadAll(fullAvatar.GetAchievementTemplates(), fullAvatar.GetAchievements(), fullAvatar.GetCounters());
			MilMo_LastActionTimeManager.Instance.Read(fullAvatar.GetLastActionTimes());
			MilMo_HotItems.ReadSeenHotItems(fullAvatar.GetSeenHotItemsHashCodes());
			MilMo_NewsManager.Instance.SetReadNewsHashCodes(fullAvatar.GetReadNewsHashCodes());
			Avatar.UpdateHealth(Avatar.Health);
			Avatar.UpdateMaxHealth(Avatar.MaxHealth);
			if (Avatar.Health > 0f && IsExhausted)
			{
				Respawn(Avatar.Health, Avatar.Position, Avatar.GameObject.transform.eulerAngles.y, setInvulnerable: false);
			}
			Avatar.EmoteManager.RegisterWieldCallbacks(EquipCurrentWieldItemLocal, UnequipCurrentWieldItemLocal);
			IsDone = true;
			Inventory.HandleNotFullyAdded();
			foreach (ExposedVariableUpdate pendingVariableUpdate in _pendingVariableUpdates)
			{
				HandleExposedVariableUpdate(pendingVariableUpdate);
			}
			_pendingVariableUpdates.Clear();
			GlobalStates.Instance.playerState.gems.Set(fullAvatar.GetGems());
			MilMo_EventSystem.Instance.PostEvent("gems_changed", null);
			HomeBanList = new MilMo_BanList(fullAvatar.GetPlayersBannedFromHome());
			if (fullAvatar.GetUnusedAvailableClassLevels() > 0)
			{
				PlayerClassManager.CheckForAvailableSelections();
			}
			PlayerSkillManager.CreateSkills(fullAvatar.GetSkillTemplates());
			MilMo_GameDialogCreator.RequestNotifications();
			Singleton<GameNetwork>.Instance.SendToGameServer(new ClientRequestInviteItem());
			MilMo_LoadingScreen.Instance.PlayerLoaded();
		});
		Avatar.Read(this, new Code.Core.Network.types.Avatar(fullAvatar));
		Singleton<GameNetwork>.Instance.RequestNexusLoginInfo();
	}

	public long GetPlayedSeconds()
	{
		return _playedSeconds;
	}

	private void UpdateLastUpdatedPlayInterval()
	{
		_lastUpdatedPlayInterval = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
	}

	public void UnloadPlayerInfo()
	{
		UnityEngine.Debug.Log("Unloading player info");
		Inventory?.Clear();
		EquipSlots?.Clear();
		Avatar?.Destroy();
		Avatar = null;
		Quests.Destroy();
		if (Silhouette != null)
		{
			UnityEngine.Object.Destroy(Silhouette.gameObject);
		}
		IsDone = false;
	}

	private void JuneCashUpdated(object msgAsObject)
	{
		ServerJuneCashUpdate serverJuneCashUpdate = (ServerJuneCashUpdate)msgAsObject;
		if (serverJuneCashUpdate != null)
		{
			int change = serverJuneCashUpdate.getJuneCash() - JuneCash;
			JuneCash = serverJuneCashUpdate.getJuneCash();
			MilMo_JuneCash.Instance.UserAccountBalance = JuneCash;
			MilMo_CoinGUIUpdateInfo milMo_CoinGUIUpdateInfo = new MilMo_CoinGUIUpdateInfo(serverJuneCashUpdate.getGUIUpdateType(), change);
			if (milMo_CoinGUIUpdateInfo.QuickInfoDialog)
			{
				LocalizedStringWithArgument message = new LocalizedStringWithArgument("World_4792", milMo_CoinGUIUpdateInfo.Change.ToString());
				DialogueSpawner.SpawnQuickInfoDialogue(new LocalizedStringWithArgument("World_4791"), message, "IconPremium", 4);
			}
		}
	}

	private static void JuneCashNeedUpdate(object msgAsObject)
	{
		Singleton<GameNetwork>.Instance.RequestJuneCash();
	}

	private void TeleportStonesUpdated(object msgAsObject)
	{
		ServerUpdateTeleportStones serverUpdateTeleportStones = (ServerUpdateTeleportStones)msgAsObject;
		if (serverUpdateTeleportStones == null)
		{
			return;
		}
		int change = serverUpdateTeleportStones.getTeleportStoneCount() - Avatar.TeleportStones;
		Avatar.TeleportStones = serverUpdateTeleportStones.getTeleportStoneCount();
		GameEvent.OnTeleportStoneUpdate?.Invoke(Avatar.TeleportStones);
		if (change <= 0)
		{
			return;
		}
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate("Item:TeleportStone", delegate(MilMo_Template template, bool timeout)
		{
			if (!(template == null || timeout) && template is MilMo_ItemTemplate milMo_ItemTemplate)
			{
				MilMo_LocString milMo_LocString = ((change == 1) ? milMo_ItemTemplate.PickupMessageSingle : milMo_ItemTemplate.PickupMessageSeveral);
				if (!milMo_LocString.IsEmpty)
				{
					GameEvent.ThinkEvent?.RaiseEvent(milMo_LocString.String);
				}
			}
		});
		MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_ReceiveTelepod");
		GameEvent.ShowTeleportCounterEvent?.RaiseEvent(args: true);
	}

	private void OnTeleportToFriendFailed(object messageAsObject)
	{
		PendingTeleportRequest = null;
		if (!(messageAsObject is ServerTeleportToFriendFail serverTeleportToFriendFail))
		{
			return;
		}
		string addressableKey = "InfoIcon";
		if (serverTeleportToFriendFail.getReason() == 0)
		{
			DialogueSpawner.SpawnGoToShopDialogue(new EnterShopModalMessageData(new LocalizedStringWithArgument("World_6250"), new LocalizedStringWithArgument("World_6251"), "Shop:Batch01.TeleportStones.Telepod5", new AddressableSpriteLoader("IconTeleportStone")));
			return;
		}
		LocalizedStringWithArgument message;
		if (serverTeleportToFriendFail.getReason() == 6)
		{
			string additionalInfo = serverTeleportToFriendFail.getAdditionalInfo();
			message = new LocalizedStringWithArgument("World_8132", MilMo_ProfileManager.GetStoredName(additionalInfo));
			DialogueSpawner.SpawnGoToShopDialogue(new EnterShopModalMessageData(new LocalizedStringWithArgument("World_6250"), message, "Shop:Batch01.Subscriptions.SubscriptionSixMonths", new AddressableSpriteLoader("IconPremium")));
			return;
		}
		if (serverTeleportToFriendFail.getReason() == 1)
		{
			MilMo_LevelInfoData levelInfoData = MilMo_LevelInfo.GetLevelInfoData(serverTeleportToFriendFail.getAdditionalInfo());
			message = new LocalizedStringWithArgument("World_6252", levelInfoData.DisplayName?.String);
			addressableKey = MilMo_LevelData.GetLevelIconPath(levelInfoData.World, levelInfoData.Level);
		}
		else if (serverTeleportToFriendFail.getReason() == 2)
		{
			message = new LocalizedStringWithArgument("World_6254");
			MilMo_LevelInfoData levelInfoData2 = MilMo_LevelInfo.GetLevelInfoData(MilMo_Level.LastAdventureLevel);
			addressableKey = MilMo_LevelData.GetLevelIconAddressableKey(levelInfoData2.World, levelInfoData2.Level);
		}
		else if (serverTeleportToFriendFail.getReason() == 4)
		{
			message = new LocalizedStringWithArgument("World_6789");
			addressableKey = "IconMyHome";
		}
		else if (serverTeleportToFriendFail.getReason() == 5)
		{
			message = new LocalizedStringWithArgument("World_6790");
			addressableKey = "IconMyHome";
		}
		else
		{
			message = new LocalizedStringWithArgument("World_6253");
		}
		DialogueSpawner.SpawnOkModal(new LocalizedStringWithArgument("World_6250"), message, new AddressableSpriteLoader(addressableKey), null);
	}

	private void OnTeleportToFriendSuccess(object messageAsObject)
	{
		PendingTeleportRequest = null;
		PendingTeleportToFriendMessage = null;
		ServerTeleportToFriendOk serverMessage = messageAsObject as ServerTeleportToFriendOk;
		if (serverMessage == null)
		{
			return;
		}
		if (InNavigator)
		{
			PendingTeleportToFriendMessage = serverMessage;
			RequestLeaveNavigator();
			return;
		}
		if (InShop)
		{
			PendingTeleportToFriendMessage = serverMessage;
			RequestLeaveShop();
			return;
		}
		if (InHub)
		{
			PendingTeleportToFriendMessage = serverMessage;
			RequestLeaveHub();
			return;
		}
		if (InCharBuilderWorld)
		{
			PendingTeleportToFriendMessage = serverMessage;
			RequestLeaveCharBuilder();
			return;
		}
		Teleporting = true;
		string fullLevelName = serverMessage.getFullLevelName();
		int homeOwnerId = serverMessage.getHomeOwnerId();
		if (homeOwnerId > 0)
		{
			string homeOwnerName = ((!(homeOwnerId.ToString() == Id)) ? MilMo_ProfileManager.GetStoredName(homeOwnerId.ToString()) : ((Avatar != null) ? Avatar.Name : ""));
			MilMo_LoadingScreen.Instance.LoadHomeFade(40f, homeOwnerName);
			MilMo_EventSystem.At(1f, delegate
			{
				Singleton<GameNetwork>.Instance.RequestHomeServerInfo(homeOwnerId.ToString(), useTeleportStone: true);
			});
			return;
		}
		MilMo_LoadingScreen.Instance.LevelLoadFade(12f);
		MilMo_LevelInfoData levelInfo = MilMo_LevelInfo.GetLevelInfoData(fullLevelName);
		MilMo_EventSystem.At(0.25f, delegate
		{
			MilMo_LocString locString = MilMo_Localization.GetLocString("Generic_56");
			locString.SetFormatArgs(levelInfo.DisplayName);
			MilMo_LoadingScreen.Instance.SetLoadingText(locString);
			MilMo_LevelData.LoadAndSetLevelIcon(levelInfo.World, levelInfo.Level);
		});
		MilMo_EventSystem.At(0.8f, delegate
		{
			Singleton<GameNetwork>.Instance.RequestTeleport(serverMessage.getToken(), serverMessage.getHost(), serverMessage.getPort());
		});
	}

	public void ToggleWieldableRight()
	{
		ToggleWieldable(ToggleDirection.Right);
	}

	private void ToggleWieldableLeft()
	{
		ToggleWieldable(ToggleDirection.Left);
	}

	private void ToggleWieldable(ToggleDirection dir)
	{
		if (MilMo_Instance.CurrentInstance != null && !InGUIApp && EquipSlots.HasItems() && EquipSlots.CurrentMode != IWeaponSlots.Mode.None && !IsExhausted)
		{
			switch (dir)
			{
			case ToggleDirection.Right:
				EquipSlots.ToggleRight();
				break;
			case ToggleDirection.Left:
				EquipSlots.ToggleLeft();
				break;
			}
			if (MilMo_Level.CurrentLevel != null && MilMo_LevelInfo.IsPvp(MilMo_Level.CurrentLevel.VerboseName))
			{
				Avatar.PlaySoundEffect("Content/Sounds/Batch01/GUI/Generic/Swap");
			}
			else
			{
				Avatar.PlaySoundEffect((EquipSlots.CurrentItem != null) ? "Content/Sounds/Batch01/Inventory/ToggleWieldable" : "Content/Sounds/Batch01/Inventory/CloseBag");
			}
			MilMo_EventSystem.Instance.PostEvent("tutorial_ToggleWieldable", "");
		}
	}

	public void WieldWhenReady(MilMo_Wieldable item)
	{
		MilMo_EventSystem.At(Math.Max(Avatar.ReadyAttackTime - Time.time + 0.1f, 0.1f), delegate
		{
			Wield(item);
		});
	}

	public void RefreshWieldable()
	{
		if (EquipSlots?.CurrentItemInventoryEntry?.Item is MilMo_Weapon milMo_Weapon && milMo_Weapon.Template.NeedAmmo)
		{
			string ammoType = milMo_Weapon.Template.AmmoType;
			int newValue = (PlayerAmmoManager ? PlayerAmmoManager.GetAmount(ammoType) : 0);
			GameEvent.ShowAmmoCounterEvent.RaiseEvent(args: true);
			GlobalStates.Instance.playerState.ammoAmount.Set(newValue);
			GlobalStates.Instance.playerState.ammoType.Set(ammoType);
		}
		else
		{
			GameEvent.ShowAmmoCounterEvent.RaiseEvent(args: false);
		}
	}

	public void Wield(MilMo_Wieldable item)
	{
		if (item == null)
		{
			UnityEngine.Debug.LogWarning("Trying to wield null item");
			return;
		}
		if (item.BodyPack == null)
		{
			UnityEngine.Debug.LogWarning("Failed to wield item " + item.Template.Path + " for player " + Avatar.Name + ": Bodypack is null.");
			return;
		}
		MilMo_Avatar avatar = Avatar;
		WeaponSlots equipSlots = EquipSlots;
		avatar.Wield(item, equipSlots == null || equipSlots.CurrentMode != IWeaponSlots.Mode.None);
		WeaponSlots equipSlots2 = EquipSlots;
		if (equipSlots2 != null && equipSlots2.CurrentMode == IWeaponSlots.Mode.None)
		{
			DisableAllWieldables();
		}
		RefreshWieldable();
		MilMo_EventSystem.Instance.PostEvent("wieldable_changed", EquipSlots?.CurrentItemInventoryEntry);
		MilMo_EventSystem.Instance.PostEvent("tutorial_WieldableChanged", EquipSlots?.CurrentItemInventoryEntry);
	}

	public void Unwield(bool sendToServer, bool useCooldown = false)
	{
		if (Avatar.WieldedItem == null)
		{
			return;
		}
		if (useCooldown)
		{
			MilMo_EventSystem.At(Math.Max(Avatar.ReadyAttackTime - Time.time, 0f), delegate
			{
				Avatar.Unwield();
			});
		}
		else
		{
			Avatar.Unwield();
		}
		if (sendToServer)
		{
			Singleton<GameNetwork>.Instance.RequestUnwield();
		}
		GameEvent.ShowAmmoCounterEvent.RaiseEvent(args: false);
		MilMo_EventSystem.Instance.PostEvent("wieldable_changed", null);
	}

	private void UnequipCurrentWieldItemLocal()
	{
		if (EquipSlots?.CurrentItem != null)
		{
			Avatar.UnequipLocal(EquipSlots.CurrentItem);
			Avatar.AsyncApply();
		}
	}

	private void EquipCurrentWieldItemLocal()
	{
		if (Avatar.WieldedItem != null)
		{
			Avatar.EquipLocal(Avatar.WieldedItem);
			Avatar.AsyncApply();
		}
	}

	private void UnwieldLocal()
	{
		Unwield(sendToServer: false);
	}

	private void WieldFailed(object msgAsObject)
	{
		ServerItemWieldFail serverItemWieldFail = (ServerItemWieldFail)msgAsObject;
		if (serverItemWieldFail != null && EquipSlots != null)
		{
			EquipSlots.UnwieldCurrent(sendToServer: false);
			if (serverItemWieldFail.getLowLevel() == 1)
			{
				string addressableKey = Inventory.GetEntry(serverItemWieldFail.getItemID())?.Item?.Template?.IconPath;
				DialogueSpawner.SpawnOkModal(new LocalizedStringWithArgument("World_9360"), new LocalizedStringWithArgument("World_9361"), new AddressableSpriteLoader(addressableKey), null);
			}
		}
	}

	public void DisableAllWieldables()
	{
		if (EquipSlots != null)
		{
			EquipSlots.CurrentMode = IWeaponSlots.Mode.None;
		}
	}

	public void EnableWieldables(bool ignoreControllerType = false)
	{
		if (EquipSlots != null)
		{
			if (InRoom || (!ignoreControllerType && MilMo_World.Instance.PlayerController.Type == MilMo_PlayerControllerBase.ControllerType.Chat))
			{
				EquipSlots.CurrentMode = IWeaponSlots.Mode.FoodOnly;
			}
			else
			{
				EquipSlots.CurrentMode = EquipSlots.LastMode;
			}
		}
	}

	public void SetWieldableModeFood()
	{
		if (EquipSlots != null)
		{
			EquipSlots.CurrentMode = IWeaponSlots.Mode.FoodOnly;
		}
	}

	private void SetWieldableModeAll()
	{
		if (EquipSlots != null)
		{
			EquipSlots.CurrentMode = IWeaponSlots.Mode.All;
		}
	}
}
