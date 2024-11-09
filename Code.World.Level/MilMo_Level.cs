using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Avatar;
using Code.Core.Avatar.Ragdoll;
using Code.Core.Camera;
using Code.Core.Collision;
using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.Core.Items;
using Code.Core.Music;
using Code.Core.Network;
using Code.Core.Network.messages.debug;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.Core.Utility;
using Code.Core.Visual;
using Code.Core.Visual.Water;
using Code.World.Achievements;
using Code.World.AmbientSound;
using Code.World.Attack;
using Code.World.Chat.ChatRoom;
using Code.World.Climbing;
using Code.World.Gameplay;
using Code.World.GroundMaterials;
using Code.World.GUI.LoadingScreen;
using Code.World.Inventory;
using Code.World.Level.LevelInfo;
using Code.World.Level.LevelObject;
using Code.World.Level.PVP;
using Code.World.Player;
using Code.World.Tutorial;
using Core;
using Core.Analytics;
using Core.GameEvent;
using Core.Utilities;
using Localization;
using Player;
using UI.HUD.Chat;
using UI.HUD.Dialogues;
using UI.HUD.Dialogues.NPC;
using UI.HUD.Dialogues.NPC.Travel;
using UnityEngine;

namespace Code.World.Level;

public class MilMo_Level : MilMo_Instance
{
	public delegate void LevelLoadDone(bool success);

	private delegate void LevelLoadInternalDone(bool success);

	protected delegate void LevelLoadPropsDone(bool success);

	public static string LastAdventureLevel = "World01:Level12";

	public static bool FirstTimeEnterLevel = true;

	public const float WALKABLE_GROUND_MINIMUM_DISTANCE = 2f;

	private static readonly Dictionary<string, List<MilMo_AchievementObjectiveListener>> AllExplorationTokensFoundListeners = new Dictionary<string, List<MilMo_AchievementObjectiveListener>>();

	private bool _isChatroom;

	protected MilMo_LevelData _data;

	private int _queuedPropCounter;

	private int _queuedRoomPropCounter;

	private readonly List<MilMo_MusicArea> _musicAreas = new List<MilMo_MusicArea>();

	private MilMo_MusicArea _currentMusicArea;

	private const string DEFAULT_MUSIC_CLIP = "Batch01/Music/Adventure01";

	private readonly List<string> _defaultMusic = new List<string>();

	private int _currentDefaultMusicIndex;

	private MilMo_TimerEvent _playNextTrackEvent;

	private readonly List<MilMo_LevelProp> _roomProps = new List<MilMo_LevelProp>();

	private readonly List<MilMo_LevelItem> _items = new List<MilMo_LevelItem>();

	private readonly Dictionary<int, byte> _removedItems = new Dictionary<int, byte>();

	private readonly List<MilMo_LevelNpc> _npcs = new List<MilMo_LevelNpc>();

	private readonly List<MilMo_MovableObject> _creatures = new List<MilMo_MovableObject>();

	private readonly Dictionary<int, byte> _removedCreatures = new Dictionary<int, byte>();

	private static List<string> _usedExits = new List<string>();

	private readonly List<MilMo_GameplayObject> _gameplayObjects = new List<MilMo_GameplayObject>();

	private readonly List<MilMo_LevelProjectile> _playerProjectiles = new List<MilMo_LevelProjectile>();

	private readonly List<MilMo_LevelProjectile> _levelProjectiles = new List<MilMo_LevelProjectile>();

	private readonly List<MilMo_ExplorationToken> _explorationTokens = new List<MilMo_ExplorationToken>();

	private readonly List<MilMo_CoinToken> _coinTokens = new List<MilMo_CoinToken>();

	private readonly List<MilMo_StaticGem> _staticGems = new List<MilMo_StaticGem>();

	private MilMo_PremiumToken _premiumToken;

	private readonly Dictionary<int, MilMo_ClimbingSurface> _climbingSurfaces = new Dictionary<int, MilMo_ClimbingSurface>();

	private MilMo_GenericReaction _propLoadedReaction;

	private MilMo_GenericReaction _roomPropLoadedReaction;

	private MilMo_GenericReaction _objectCreateReaction;

	private MilMo_GenericReaction _objectMovableCreateReaction;

	private MilMo_GenericReaction _creatureProjectileCreateReaction;

	private MilMo_GenericReaction _movableHealthUpdateReaction;

	private MilMo_GenericReaction _movableObjectImpulseReaction;

	private MilMo_GenericReaction _movableObjectStunReaction;

	private MilMo_GenericReaction _objectUpdateReaction;

	private MilMo_GenericReaction _objectDestroyReaction;

	private MilMo_GenericReaction _startClimbReaction;

	private MilMo_GenericReaction _stopClimbReaction;

	private MilMo_GenericReaction _enterGameplayObjectReaction;

	private MilMo_GenericReaction _leaveGameplayObjectReaction;

	private MilMo_GenericReaction _levelObjectPickupReaction;

	private MilMo_GenericReaction _explorationTokenFoundReaction;

	private MilMo_GenericReaction _coinTokenFoundReaction;

	private MilMo_GenericReaction _staticGemFoundReaction;

	private MilMo_GenericReaction _premiumTokenFoundReaction;

	private MilMo_GenericReaction _levelObjectCreatureSpawnReaction;

	private MilMo_GenericReaction _pickupFailReaction;

	private MilMo_GenericReaction _playerRespawnedReaction;

	private MilMo_GenericReaction _movableAttackReaction;

	private MilMo_GenericReaction _movablePrepareAttackReaction;

	private MilMo_GenericReaction _movableBeginAttackReaction;

	private MilMo_GenericReaction _movablePreparedAttackImpactReaction;

	private MilMo_GenericReaction _movableAttackDoneReaction;

	private MilMo_GenericReaction _playersHitByCapsuleReaction;

	private MilMo_GenericReaction _playerHitByProjectileReaction;

	private MilMo_GenericReaction _projectileDestroyedReaction;

	private MilMo_GenericReaction _creatureHitByProjectileReaction;

	private MilMo_GenericReaction _playerAttackHitReaction;

	private MilMo_GenericReaction _playerAttackMissedReaction;

	private MilMo_GenericReaction _playerAttackUntargetedReaction;

	private MilMo_GenericReaction _playerKilledPlayerReaction;

	private MilMo_GenericReaction _playerDigReaction;

	private MilMo_GenericReaction _creatureAggroReaction;

	private MilMo_GenericReaction _creatureNoAggroReaction;

	private MilMo_GenericReaction _activateGameplayTriggerReaction;

	private MilMo_GenericReaction _gameplayObjectStartMovingReaction;

	private MilMo_GenericReaction _gameplayObjectChangeDirectionReaction;

	private MilMo_GenericReaction _spawnVisualRepAtReaction;

	private MilMo_GenericReaction _playerChangeRoomReaction;

	private MilMo_GenericReaction _npcChangeInteractionStateReaction;

	private MilMo_GenericReaction _debugCreatureSyncReaction;

	private MilMo_GenericReaction _debugCapusleVolumeSyncReaction;

	private MilMo_GenericReaction _endPlayerInvulnerabilityReaction;

	private MilMo_GenericReaction _scoreBoardArrivedReaction;

	private MilMo_GenericReaction _killsdeathsArrivedReaction;

	private MilMo_GenericReaction _updateMatchStateReaction;

	private MilMo_GenericReaction _pvpLevelInstanceInfo;

	private MilMo_GenericReaction _setGameplayObjectPosReaction;

	private MilMo_GenericReaction _shrinkGameplayObjectReaction;

	private MilMo_GenericReaction _usedExitInfoListener;

	private MilMo_GenericReaction _npcMessageListener;

	private MilMo_GenericReaction _levelNPCCreateListener;

	private MilMo_GenericReaction _levelGameplayobjectCreateListener;

	private MilMo_GenericReaction _levelExplorationTokenCreateListener;

	private MilMo_GenericReaction _levelCoinTokenCreateListener;

	private MilMo_GenericReaction _levelPremiumTokenCreateListener;

	private MilMo_GenericReaction _levelStaticGemsCreateListener;

	private MilMo_GenericReaction _inventoryBagAddListener;

	public const int TOKEN_FOUND = 1;

	public List<string> UsedExits
	{
		get
		{
			return _usedExits;
		}
		private set
		{
			_usedExits = value;
			this.OnUsedExitsUpdated?.Invoke();
		}
	}

	public static MilMo_Level CurrentLevel => MilMo_Instance.CurrentInstance as MilMo_Level;

	public override Type InstanceType => Type.Level;

	public MilMo_LocString DisplayName => MilMo_LevelInfo.GetLevelInfoData(VerboseName).DisplayName;

	public override MilMo_LocString ShopDisplayName => MilMo_LevelInfo.GetLevelInfoData(VerboseName).ShopDisplayName;

	public string VerboseName { get; protected set; }

	public string WorldContentName { get; protected set; }

	public string LevelContentName { get; protected set; }

	public override Texture2D Icon => _data.Icon;

	public string World { get; protected set; }

	public string Name { get; protected set; }

	public string Language { get; private set; }

	public string InstanceId { get; private set; }

	protected MilMo_GroundMaterialManager GroundMaterialManager { get; set; }

	public MilMo_AmbientSoundManager AmbientSoundManager { get; private set; }

	public override List<MilMo_LevelItem> Items => _items;

	private List<MilMo_LevelProp> Props { get; } = new List<MilMo_LevelProp>();


	public override List<MilMo_MovableObject> Creatures => _creatures;

	public override List<MilMo_LevelNpc> NPCs => _npcs;

	public MilMo_PVPHandler PvpHandler { get; } = new MilMo_PVPHandler();


	public event Action OnUsedExitsUpdated = delegate
	{
	};

	public static float GetWalkableHeight(Vector3 pos, out Vector3 normal)
	{
		return MilMo_Physics.GetWorldHeight(new Vector3(pos.x, pos.y, pos.z), out normal, 8912896);
	}

	public static float GetWalkableHeight(Vector3 pos)
	{
		Vector3 normal;
		return GetWalkableHeight(pos, out normal);
	}

	public void StartGlobalListeners()
	{
		_levelNPCCreateListener = MilMo_EventSystem.Listen("level_npc_create", CreateNpCs);
		_levelNPCCreateListener.Repeating = true;
		_levelGameplayobjectCreateListener = MilMo_EventSystem.Listen("level_gameplayobject_create", CreateGameplayObjects);
		_levelGameplayobjectCreateListener.Repeating = true;
		_levelExplorationTokenCreateListener = MilMo_EventSystem.Listen("level_explorationtoken_create", CreateExplorationTokens);
		_levelExplorationTokenCreateListener.Repeating = true;
		_levelCoinTokenCreateListener = MilMo_EventSystem.Listen("level_cointoken_create", CreateCoinTokens);
		_levelCoinTokenCreateListener.Repeating = true;
		_levelPremiumTokenCreateListener = MilMo_EventSystem.Listen("level_premiumtoken_create", CreatePremiumToken);
		_levelPremiumTokenCreateListener.Repeating = true;
		_levelStaticGemsCreateListener = MilMo_EventSystem.Listen("level_static_gems_create", CreateStaticGems);
		_levelStaticGemsCreateListener.Repeating = true;
		_inventoryBagAddListener = MilMo_EventSystem.Listen("inventory_bag_add", ItemAddedToPlayerInventory);
		_inventoryBagAddListener.Repeating = true;
	}

	public void StopGlobalListeners()
	{
		MilMo_EventSystem.RemoveReaction(_levelNPCCreateListener);
		_levelNPCCreateListener = null;
		MilMo_EventSystem.RemoveReaction(_levelGameplayobjectCreateListener);
		_levelGameplayobjectCreateListener = null;
		MilMo_EventSystem.RemoveReaction(_levelExplorationTokenCreateListener);
		_levelExplorationTokenCreateListener = null;
		MilMo_EventSystem.RemoveReaction(_levelCoinTokenCreateListener);
		_levelCoinTokenCreateListener = null;
		MilMo_EventSystem.RemoveReaction(_levelPremiumTokenCreateListener);
		_levelPremiumTokenCreateListener = null;
		MilMo_EventSystem.RemoveReaction(_levelStaticGemsCreateListener);
		_levelStaticGemsCreateListener = null;
		MilMo_EventSystem.RemoveReaction(_inventoryBagAddListener);
		_inventoryBagAddListener = null;
	}

	private bool IsCorrectLevel(string fullLevelName)
	{
		MilMo_Instance currentInstance = MilMo_Instance.CurrentInstance;
		if (currentInstance == null || currentInstance.InstanceType != 0)
		{
			return false;
		}
		return fullLevelName == VerboseName;
	}

	public Vector3 GetNpcPosition(int id)
	{
		using (IEnumerator<MilMo_LevelNpc> enumerator = _npcs.Where((MilMo_LevelNpc npc) => npc.Id == id).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current.Position;
			}
		}
		return new Vector3(0f, 0f, 0f);
	}

	public MilMo_LevelNpc GetNpc(int id)
	{
		return _npcs.FirstOrDefault((MilMo_LevelNpc npc) => npc.Id == id);
	}

	public IMilMo_AttackTarget GetClosestTarget(Vector3 pos, Vector3 forward, MilMo_WeaponTemplate weapon, bool useHitRange)
	{
		float closestSqrDistance = float.MaxValue;
		bool currentClosestIsWithinPrioritizedAngle = false;
		IMilMo_AttackTarget closestTarget = null;
		bool currentClosestIsHighPrio = false;
		if (MilMo_LevelInfo.IsPvp(VerboseName))
		{
			foreach (MilMo_RemotePlayer item in base.Players.Values.Where((MilMo_RemotePlayer player) => !PvpHandler.IsTeamMate(player)))
			{
				GetClosestTarget(item.Avatar, ref closestSqrDistance, ref closestTarget, ref currentClosestIsHighPrio, ref currentClosestIsWithinPrioritizedAngle, pos, forward, weapon, useHitRange);
			}
			if (closestTarget != null)
			{
				return closestTarget;
			}
		}
		foreach (MilMo_MovableObject creature in _creatures)
		{
			GetClosestTarget(creature, ref closestSqrDistance, ref closestTarget, ref currentClosestIsHighPrio, ref currentClosestIsWithinPrioritizedAngle, pos, forward, weapon, useHitRange);
		}
		return closestTarget;
	}

	private static void GetClosestTarget(IMilMo_AttackTarget target, ref float closestSqrDistance, ref IMilMo_AttackTarget closestTarget, ref bool currentClosestIsHighPrio, ref bool currentClosestIsWithinPrioritizedAngle, Vector3 pos, Vector3 forward, MilMo_WeaponTemplate weapon, bool useHitRange)
	{
		bool num = target is MilMo_LevelBoss;
		bool flag = target is MilMo_Avatar;
		float damage = target.GetDamage(weapon.Damage);
		bool flag2 = num || flag || damage > 0f;
		if (!target.IsDeadOrDying() && target.CollisionRadius > 0f && (flag2 || (target.HasKnockBack() && weapon.Impact > 0f)))
		{
			Vector3 vector = target.Position - pos;
			float num2 = Vector3.SqrMagnitude(vector);
			float num3 = (useHitRange ? weapon.GetHitRange(target.CollisionRadius) : weapon.GetAutoAimRange(target.CollisionRadius)) + target.CollisionRadius;
			bool flag3 = Vector3.Angle(forward, vector) <= weapon.PrioritizedTargetingAngle / 2f;
			if (num2 < num3 * num3 && ((flag2 && !currentClosestIsHighPrio) || (flag3 && !currentClosestIsWithinPrioritizedAngle) || (flag3 == currentClosestIsWithinPrioritizedAngle && num2 < closestSqrDistance)))
			{
				closestTarget = target;
				closestSqrDistance = num2;
				currentClosestIsHighPrio = flag2;
				currentClosestIsWithinPrioritizedAngle = flag3;
			}
		}
	}

	public bool RemotePlayerInAttackRadius()
	{
		return base.Players.Select((KeyValuePair<string, MilMo_RemotePlayer> kvp) => (MilMo_Instance.PlayerInstance.Avatar.Position - kvp.Value.Avatar.Position).sqrMagnitude).Any((float playerToAvatar) => playerToAvatar <= 4f);
	}

	public void AsyncLoad(string world, string level, string language, string instanceId, Vector3 entryPoint, LevelLoadDone callback)
	{
		World = world;
		Name = level;
		Language = language;
		InstanceId = instanceId;
		VerboseName = World + ":" + Name;
		ChannelName = World + ":" + Name;
		base.EntryPoint = entryPoint;
		WorldContentName = MilMo_LevelData.GetWorldContentName(World);
		LevelContentName = MilMo_LevelData.GetLevelContentName(Name);
		Unload();
		_data = new MilMo_LevelData(WorldContentName, LevelContentName);
		_data.AsyncLoad(delegate(bool success)
		{
			if (!success)
			{
				callback(success: false);
			}
			else
			{
				AsyncLoadInternal(delegate(bool internalSuccess)
				{
					if (internalSuccess)
					{
						Debug.Log("Level " + VerboseName + " loaded");
						MilMo_Instance.CurrentInstance = this;
						MilMo_LevelInfoData levelInfoData = MilMo_LevelInfo.GetLevelInfoData(VerboseName);
						if (levelInfoData.IsChatRoom)
						{
							_isChatroom = true;
						}
						else
						{
							_isChatroom = false;
							LastAdventureLevel = VerboseName;
						}
						if (!_isChatroom)
						{
							MilMo_World.CurrentWorld = World;
						}
						if (AllExplorationTokensFound() && AllExplorationTokensFoundListeners.ContainsKey(VerboseName))
						{
							foreach (MilMo_AchievementObjectiveListener item in AllExplorationTokensFoundListeners[VerboseName])
							{
								item.Notify();
							}
							AllExplorationTokensFoundListeners.Remove(VerboseName);
						}
						MilMo_EventSystem.At(0f, delegate
						{
							AmbientSoundManager = new MilMo_AmbientSoundManager(WorldContentName, LevelContentName);
						});
						MilMo_CameraController.LoadCameraMagnets("Content/Worlds/" + WorldContentName + "/Levels/" + LevelContentName + "/");
						if (levelInfoData.PremiumToken != null && levelInfoData.PremiumToken.GetIsFound() == 0 && MilMo_Instance.PlayerInstance.IsMember)
						{
							MilMo_EventSystem.Instance.PostEvent("level_premiumtoken_create", levelInfoData.PremiumToken);
						}
						LoadEventListeners();
						callback(success: true);
					}
					else
					{
						Debug.LogWarning("Failed to load level " + VerboseName);
						MilMo_Instance.CurrentInstance = null;
						callback(success: false);
					}
					if (MilMo_Instance.PlayerInstance.Avatar != null)
					{
						base.Avatars.Add(MilMo_Instance.PlayerInstance.Avatar);
						if (!string.IsNullOrEmpty(MilMo_Instance.PlayerInstance.Avatar.Room))
						{
							MilMo_RoomPlopManager.Instance.PlayerEnteredRoom(MilMo_Instance.PlayerInstance.Avatar, MilMo_Instance.PlayerInstance.Avatar.Room);
						}
					}
				});
			}
		});
	}

	public override void Unload()
	{
		if (Singleton<Analytics>.Instance != null)
		{
			Singleton<Analytics>.Instance.LevelQuit(VerboseName);
		}
		base.Unload();
		PlayMusic = false;
		if (Application.isPlaying)
		{
			MilMo_Player playerInstance = MilMo_Instance.PlayerInstance;
			if (playerInstance != null && playerInstance.Avatar != null)
			{
				MilMo_Instance.PlayerInstance.Avatar.Combat.EndCombat();
			}
		}
		Debug.Log("Unloading music for level " + VerboseName);
		UnloadMusic();
		Debug.Log("Unloading climbing surfaces for level " + VerboseName);
		UnloadClimbingSurfaces();
		Debug.Log("Unloading environment for level " + VerboseName);
		Environment.UnloadEnvironment();
		Debug.Log("Unloading props for level " + VerboseName);
		UnloadProps();
		Debug.Log("Unloading rooms for level " + VerboseName);
		UnloadRoom();
		Debug.Log("Unloading gameplay objects for level " + VerboseName);
		UnloadGameplayObjects();
		Debug.Log("Unloading terrain for level " + VerboseName);
		MilMo_Terrain.Unload();
		Debug.Log("Unloading NPCs for level " + VerboseName);
		UnloadNpCs();
		Debug.Log("Unloading creatures for level " + VerboseName);
		UnloadCreatures();
		Debug.Log("Unloading items for level " + VerboseName);
		UnloadItems();
		Debug.Log("Unloading exploration tokens for level " + VerboseName);
		UnloadExplorationTokens();
		Debug.Log("Unloading coin tokens for level " + VerboseName);
		UnloadCoinTokens();
		Debug.Log("Unloading static gems for level " + VerboseName);
		UnloadStaticGems();
		Debug.Log("Unloading premium token for level " + VerboseName);
		UnloadPremiumToken();
		Debug.Log("Unloading tutorial areas for level " + VerboseName);
		UnloadTutorialAreas();
		Debug.Log("Unloading camera magnets for level " + VerboseName);
		MilMo_CameraController.UnloadCameraMagnets();
		UnloadLevelData();
		if (AmbientSoundManager != null)
		{
			AmbientSoundManager.Stop();
			AmbientSoundManager = null;
		}
		this.OnUsedExitsUpdated = null;
		Debug.Log("Unloading assets with tag 'Level' for level " + VerboseName);
		MilMo_ResourceManager.Instance.UnloadAllByTag("Level");
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	private void AsyncLoadInternal(LevelLoadInternalDone callback)
	{
		MilMo_Terrain.Load(_data.TerrainData);
		LoadGroundMaterials();
		Environment.Load(this, async: true, _data.Environment);
		LoadClimbingSurfaces();
		LoadMusic();
		LoadTutorialAreas();
		BeginPreStream();
		LoadProps(async: true, delegate(bool success)
		{
			Debug.Log("Props loaded for level " + VerboseName);
			MilMo_Terrain.InstantiateTreeColliders();
			callback(success);
		});
	}

	protected virtual void LoadGroundMaterials()
	{
		MilMo_GroundMaterials.AsyncLoad(WorldContentName, delegate
		{
			GroundMaterialManager = new MilMo_GroundMaterialManager();
			if (!MilMo_Terrain.GameObject)
			{
				throw new NullReferenceException("Terrain game object is null when loading materials manager");
			}
			if (_data == null)
			{
				throw new NullReferenceException("Level data is null when loading materials manager");
			}
			GroundMaterialManager.Load(_data, MilMo_Terrain.GameObject.transform.position, _data.TerrainData.size.x, _data.TerrainData.size.z);
		});
	}

	protected override void LoadEventListeners()
	{
		base.LoadEventListeners();
		_objectCreateReaction = MilMo_EventSystem.Listen("object_create", ObjectCreate);
		_objectCreateReaction.Repeating = true;
		_objectMovableCreateReaction = MilMo_EventSystem.Listen("object_movable_create", ObjectMovableCreate);
		_objectMovableCreateReaction.Repeating = true;
		_creatureProjectileCreateReaction = MilMo_EventSystem.Listen("level_projectile_create", CreateCreatureProjectile);
		_creatureProjectileCreateReaction.Repeating = true;
		_movableHealthUpdateReaction = MilMo_EventSystem.Listen("movable_health_update", MovableHealthUpdate);
		_movableHealthUpdateReaction.Repeating = true;
		_movableObjectImpulseReaction = MilMo_EventSystem.Listen("movable_impulse", MoveableObjectImpulse);
		_movableObjectImpulseReaction.Repeating = true;
		_movableObjectStunReaction = MilMo_EventSystem.Listen("movable_stun", CreatureStunned);
		_movableObjectStunReaction.Repeating = true;
		_objectUpdateReaction = MilMo_EventSystem.Listen("object_update", ObjectUpdate);
		_objectUpdateReaction.Repeating = true;
		_objectDestroyReaction = MilMo_EventSystem.Listen("object_destroy", ObjectDestroy);
		_objectDestroyReaction.Repeating = true;
		_startClimbReaction = MilMo_EventSystem.Listen("start_climb", StartClimb);
		_startClimbReaction.Repeating = true;
		_stopClimbReaction = MilMo_EventSystem.Listen("stop_climb", StopClimb);
		_stopClimbReaction.Repeating = true;
		_enterGameplayObjectReaction = MilMo_EventSystem.Listen("enter_gameplay_object", EnterGameplayObject);
		_enterGameplayObjectReaction.Repeating = true;
		_leaveGameplayObjectReaction = MilMo_EventSystem.Listen("leave_gameplay_object", LeaveGameplayObject);
		_leaveGameplayObjectReaction.Repeating = true;
		_levelObjectPickupReaction = MilMo_EventSystem.Listen("level_object_pickup", LevelObjectPickup);
		_levelObjectPickupReaction.Repeating = true;
		_explorationTokenFoundReaction = MilMo_EventSystem.Listen("exploration_token_found", ExplorationTokenFound);
		_explorationTokenFoundReaction.Repeating = true;
		_coinTokenFoundReaction = MilMo_EventSystem.Listen("coin_token_found", CoinTokenFound);
		_coinTokenFoundReaction.Repeating = true;
		_staticGemFoundReaction = MilMo_EventSystem.Listen("static_gem_found", StaticGemFound);
		_staticGemFoundReaction.Repeating = true;
		_premiumTokenFoundReaction = MilMo_EventSystem.Listen("premium_token_found", PremiumTokenFound);
		_premiumTokenFoundReaction.Repeating = true;
		_levelObjectCreatureSpawnReaction = MilMo_EventSystem.Listen("level_object_creature_spawn", LevelObjectCreatureSpawn);
		_levelObjectCreatureSpawnReaction.Repeating = true;
		_pickupFailReaction = MilMo_EventSystem.Listen("pickup_fail", PickupFail);
		_pickupFailReaction.Repeating = true;
		_playerRespawnedReaction = MilMo_EventSystem.Listen("player_respawned", PlayerRespawned);
		_playerRespawnedReaction.Repeating = true;
		_movableAttackReaction = MilMo_EventSystem.Listen("movable_object_attack_instant", MovableAttack);
		_movableAttackReaction.Repeating = true;
		_movablePrepareAttackReaction = MilMo_EventSystem.Listen("movable_object_prepare_attack", MovablePrepareAttack);
		_movablePrepareAttackReaction.Repeating = true;
		_movableBeginAttackReaction = MilMo_EventSystem.Listen("movable_object_begin_attack", MovableBeginAttack);
		_movableBeginAttackReaction.Repeating = true;
		_movablePreparedAttackImpactReaction = MilMo_EventSystem.Listen("movable_object_attack_impact", MovablePreparedAttackImpact);
		_movablePreparedAttackImpactReaction.Repeating = true;
		_movableAttackDoneReaction = MilMo_EventSystem.Listen("movable_object_attack_done", MovableAttackDone);
		_movableAttackDoneReaction.Repeating = true;
		_playersHitByCapsuleReaction = MilMo_EventSystem.Listen("capsule_attack_damage", PlayersHitByCapsule);
		_playersHitByCapsuleReaction.Repeating = true;
		_playerHitByProjectileReaction = MilMo_EventSystem.Listen("player_hit_by_projectile", PlayerHitByProjectile);
		_playerHitByProjectileReaction.Repeating = true;
		_projectileDestroyedReaction = MilMo_EventSystem.Listen("projectile_destroyed", ProjectileDestroyed);
		_projectileDestroyedReaction.Repeating = true;
		_creatureHitByProjectileReaction = MilMo_EventSystem.Listen("creature_hit_by_projectile", CreatureHitByProjectile);
		_creatureHitByProjectileReaction.Repeating = true;
		_playerAttackHitReaction = MilMo_EventSystem.Listen("player_attack_hit", PlayerAttackHit);
		_playerAttackHitReaction.Repeating = true;
		_playerAttackMissedReaction = MilMo_EventSystem.Listen("player_attack_miss", PlayerAttackMissed);
		_playerAttackMissedReaction.Repeating = true;
		_playerAttackUntargetedReaction = MilMo_EventSystem.Listen("player_attack_untargeted", PlayerAttackUntargeted);
		_playerAttackUntargetedReaction.Repeating = true;
		_playerKilledPlayerReaction = MilMo_EventSystem.Listen("player_killed_player", PlayerKilledPlayer);
		_playerKilledPlayerReaction.Repeating = true;
		_playerDigReaction = MilMo_EventSystem.Listen("player_dig", PlayerDig);
		_playerDigReaction.Repeating = true;
		_creatureAggroReaction = MilMo_EventSystem.Listen("creature_aggro", CreatureAggro);
		_creatureAggroReaction.Repeating = true;
		_creatureNoAggroReaction = MilMo_EventSystem.Listen("creature_no_aggro", CreatureNoAggro);
		_creatureNoAggroReaction.Repeating = true;
		_activateGameplayTriggerReaction = MilMo_EventSystem.Listen("activate_gameplay_trigger", ActivateGameplayTrigger);
		_activateGameplayTriggerReaction.Repeating = true;
		_gameplayObjectStartMovingReaction = MilMo_EventSystem.Listen("gameplay_object_start_moving", GameplayObjectStartMoving);
		_gameplayObjectStartMovingReaction.Repeating = true;
		_gameplayObjectChangeDirectionReaction = MilMo_EventSystem.Listen("gameplay_object_change_direction", GameplayObjectChangeDirection);
		_gameplayObjectChangeDirectionReaction.Repeating = true;
		_spawnVisualRepAtReaction = MilMo_EventSystem.Listen("visual_rep_spawned", VisualRepSpawned);
		_spawnVisualRepAtReaction.Repeating = true;
		_playerChangeRoomReaction = MilMo_EventSystem.Listen("player_change_room", PlayerChangeRoom);
		_playerChangeRoomReaction.Repeating = true;
		_npcChangeInteractionStateReaction = MilMo_EventSystem.Listen("npc_change_interaction_state", NpcChangeInteractionState);
		_npcChangeInteractionStateReaction.Repeating = true;
		_debugCapusleVolumeSyncReaction = MilMo_EventSystem.Listen("debug_capsule_volume_sync", Debug_CapsuleVolumeSync);
		_debugCapusleVolumeSyncReaction.Repeating = true;
		_endPlayerInvulnerabilityReaction = MilMo_EventSystem.Listen("player_end_invulnerable", PlayerEndInvulnerable);
		_endPlayerInvulnerabilityReaction.Repeating = true;
		_scoreBoardArrivedReaction = MilMo_EventSystem.Listen("scoreboard_arrived", PvpHandler.ScoreBoardArrived);
		_scoreBoardArrivedReaction.Repeating = true;
		_killsdeathsArrivedReaction = MilMo_EventSystem.Listen("update_killsdeaths", PvpHandler.KillsDeathsArrived);
		_killsdeathsArrivedReaction.Repeating = true;
		_updateMatchStateReaction = MilMo_EventSystem.Listen("update_match_state", PvpHandler.UpdateMatchState);
		_updateMatchStateReaction.Repeating = true;
		_pvpLevelInstanceInfo = MilMo_EventSystem.Listen("got_load_pvp_level_info", PvpHandler.GotLoadPvpLevelInfo);
		_pvpLevelInstanceInfo.Repeating = true;
		_setGameplayObjectPosReaction = MilMo_EventSystem.Listen("set_gameplay_object_pos", GameplayObjectSetPos);
		_setGameplayObjectPosReaction.Repeating = true;
		_shrinkGameplayObjectReaction = MilMo_EventSystem.Listen("gameplay_object_shrink", GameplayObjectShrink);
		_shrinkGameplayObjectReaction.Repeating = true;
		_usedExitInfoListener = MilMo_EventSystem.Listen("used_level_exits_info", GotUsedExitsInfo);
		_usedExitInfoListener.Repeating = true;
		_npcMessageListener = MilMo_EventSystem.Listen("npc_message", ReadNPCMessage);
		_npcMessageListener.Repeating = true;
	}

	private void GotUsedExitsInfo(object msgAsObj)
	{
		if (msgAsObj is ServerUsedLevelExits serverUsedLevelExits)
		{
			UsedExits = (List<string>)serverUsedLevelExits.getUsedExitIdentifiers();
		}
	}

	protected override void UnloadEventListeners()
	{
		base.UnloadEventListeners();
		MilMo_EventSystem.RemoveReaction(_objectCreateReaction);
		_objectCreateReaction = null;
		MilMo_EventSystem.RemoveReaction(_objectMovableCreateReaction);
		_objectMovableCreateReaction = null;
		MilMo_EventSystem.RemoveReaction(_creatureProjectileCreateReaction);
		_creatureProjectileCreateReaction = null;
		MilMo_EventSystem.RemoveReaction(_movableHealthUpdateReaction);
		_movableHealthUpdateReaction = null;
		MilMo_EventSystem.RemoveReaction(_movableObjectImpulseReaction);
		_movableObjectImpulseReaction = null;
		MilMo_EventSystem.RemoveReaction(_movableObjectStunReaction);
		_movableObjectStunReaction = null;
		MilMo_EventSystem.RemoveReaction(_objectUpdateReaction);
		_objectUpdateReaction = null;
		MilMo_EventSystem.RemoveReaction(_objectDestroyReaction);
		_objectDestroyReaction = null;
		MilMo_EventSystem.RemoveReaction(_startClimbReaction);
		_startClimbReaction = null;
		MilMo_EventSystem.RemoveReaction(_stopClimbReaction);
		_stopClimbReaction = null;
		MilMo_EventSystem.RemoveReaction(_enterGameplayObjectReaction);
		_enterGameplayObjectReaction = null;
		MilMo_EventSystem.RemoveReaction(_leaveGameplayObjectReaction);
		_leaveGameplayObjectReaction = null;
		MilMo_EventSystem.RemoveReaction(_levelObjectPickupReaction);
		_levelObjectPickupReaction = null;
		MilMo_EventSystem.RemoveReaction(_explorationTokenFoundReaction);
		_explorationTokenFoundReaction = null;
		MilMo_EventSystem.RemoveReaction(_coinTokenFoundReaction);
		_coinTokenFoundReaction = null;
		MilMo_EventSystem.RemoveReaction(_staticGemFoundReaction);
		_staticGemFoundReaction = null;
		MilMo_EventSystem.RemoveReaction(_premiumTokenFoundReaction);
		_premiumTokenFoundReaction = null;
		MilMo_EventSystem.RemoveReaction(_levelObjectCreatureSpawnReaction);
		_levelObjectCreatureSpawnReaction = null;
		MilMo_EventSystem.RemoveReaction(_pickupFailReaction);
		_pickupFailReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerRespawnedReaction);
		_playerRespawnedReaction = null;
		MilMo_EventSystem.RemoveReaction(_movableAttackReaction);
		_movableAttackReaction = null;
		MilMo_EventSystem.RemoveReaction(_movablePrepareAttackReaction);
		_movablePrepareAttackReaction = null;
		MilMo_EventSystem.RemoveReaction(_movableBeginAttackReaction);
		_movableBeginAttackReaction = null;
		MilMo_EventSystem.RemoveReaction(_movablePreparedAttackImpactReaction);
		_movablePreparedAttackImpactReaction = null;
		MilMo_EventSystem.RemoveReaction(_movableAttackDoneReaction);
		_movableAttackDoneReaction = null;
		MilMo_EventSystem.RemoveReaction(_playersHitByCapsuleReaction);
		_playersHitByCapsuleReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerHitByProjectileReaction);
		_playerHitByProjectileReaction = null;
		MilMo_EventSystem.RemoveReaction(_projectileDestroyedReaction);
		_projectileDestroyedReaction = null;
		MilMo_EventSystem.RemoveReaction(_creatureHitByProjectileReaction);
		_creatureHitByProjectileReaction = null;
		MilMo_EventSystem.RemoveReaction(_propLoadedReaction);
		_propLoadedReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerAttackHitReaction);
		_playerAttackHitReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerAttackMissedReaction);
		_playerAttackMissedReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerAttackUntargetedReaction);
		_playerAttackUntargetedReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerKilledPlayerReaction);
		_playerKilledPlayerReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerDigReaction);
		_playerDigReaction = null;
		MilMo_EventSystem.RemoveReaction(_creatureAggroReaction);
		_creatureAggroReaction = null;
		MilMo_EventSystem.RemoveReaction(_creatureNoAggroReaction);
		_creatureNoAggroReaction = null;
		MilMo_EventSystem.RemoveReaction(_activateGameplayTriggerReaction);
		_activateGameplayTriggerReaction = null;
		MilMo_EventSystem.RemoveReaction(_gameplayObjectChangeDirectionReaction);
		_gameplayObjectChangeDirectionReaction = null;
		MilMo_EventSystem.RemoveReaction(_gameplayObjectStartMovingReaction);
		_gameplayObjectStartMovingReaction = null;
		MilMo_EventSystem.RemoveReaction(_spawnVisualRepAtReaction);
		_spawnVisualRepAtReaction = null;
		MilMo_EventSystem.RemoveReaction(_playerChangeRoomReaction);
		_playerChangeRoomReaction = null;
		MilMo_EventSystem.RemoveReaction(_npcChangeInteractionStateReaction);
		_npcChangeInteractionStateReaction = null;
		MilMo_EventSystem.RemoveReaction(_debugCreatureSyncReaction);
		_debugCreatureSyncReaction = null;
		MilMo_EventSystem.RemoveReaction(_debugCapusleVolumeSyncReaction);
		_debugCapusleVolumeSyncReaction = null;
		MilMo_EventSystem.RemoveReaction(_endPlayerInvulnerabilityReaction);
		_endPlayerInvulnerabilityReaction = null;
		MilMo_EventSystem.RemoveReaction(_scoreBoardArrivedReaction);
		_scoreBoardArrivedReaction = null;
		MilMo_EventSystem.RemoveReaction(_killsdeathsArrivedReaction);
		_killsdeathsArrivedReaction = null;
		MilMo_EventSystem.RemoveReaction(_updateMatchStateReaction);
		_updateMatchStateReaction = null;
		MilMo_EventSystem.RemoveReaction(_pvpLevelInstanceInfo);
		_pvpLevelInstanceInfo = null;
		MilMo_EventSystem.RemoveReaction(_setGameplayObjectPosReaction);
		_setGameplayObjectPosReaction = null;
		MilMo_EventSystem.RemoveReaction(_shrinkGameplayObjectReaction);
		_shrinkGameplayObjectReaction = null;
		MilMo_EventSystem.RemoveReaction(_usedExitInfoListener);
		_usedExitInfoListener = null;
		MilMo_EventSystem.RemoveReaction(_npcMessageListener);
		_npcMessageListener = null;
	}

	public override void Update()
	{
		foreach (MilMo_LevelProp roomProp in _roomProps)
		{
			roomProp.VisualRep.Update();
		}
		foreach (MilMo_LevelProp prop in Props)
		{
			prop.VisualRep.Update();
		}
		Vector3 position = MilMo_Instance.PlayerInstance.Avatar.Position;
		MilMo_Instance.PlayerInstance.CloseToGameplayObject = null;
		bool flag = false;
		foreach (MilMo_GameplayObject gameplayObject in _gameplayObjects)
		{
			gameplayObject.Update();
			if (MilMo_Instance.PlayerInstance.InInstance && !MilMo_Instance.PlayerInstance.IsTalking)
			{
				gameplayObject.CheckPlayerContact(MilMo_Instance.PlayerInstance);
			}
			if (gameplayObject.VisualRep == null || gameplayObject.VisualRep.GameObject == null)
			{
				continue;
			}
			float num;
			if (!flag && gameplayObject.VisualRep.GameObject.GetComponent<Collider>() != null)
			{
				num = gameplayObject.VisualRep.GameObject.GetComponent<Collider>().bounds.SqrDistance(position);
				if (num < 2.25f)
				{
					MilMo_Instance.PlayerInstance.CloseToGameplayObject = gameplayObject;
					flag = true;
				}
			}
			else
			{
				num = Vector3.Distance(gameplayObject.VisualRep.GameObject.transform.localPosition, position);
			}
			if (gameplayObject.IsCapturingZone && num < 10f && !MilMo_World.HudHandler.pvpZoneCountdown.IsEnabled())
			{
				MilMo_World.HudHandler.pvpZoneCountdown.setCountdown(gameplayObject.CaptureZoneTimeStamp, gameplayObject.Id, CurrentLevel.PvpHandler.GetTeamColor(gameplayObject.CurrentZoneCapturingTeamId));
			}
		}
		for (int num2 = _items.Count - 1; num2 >= 0; num2--)
		{
			_items[num2].Update();
		}
		TestItemAutoPickup();
		foreach (MilMo_LevelNpc npc in _npcs)
		{
			npc.Update();
		}
		for (int num3 = _creatures.Count - 1; num3 >= 0; num3--)
		{
			_creatures[num3].Update();
			if (_creatures[num3].IsDead())
			{
				if (!_removedCreatures.ContainsKey(_creatures[num3].Id))
				{
					_removedCreatures.Add(_creatures[num3].Id, 0);
				}
				_creatures[num3].Unload();
				_creatures.RemoveAt(num3);
			}
		}
		for (int num4 = _playerProjectiles.Count - 1; num4 >= 0; num4--)
		{
			_playerProjectiles[num4].Update();
			if (_playerProjectiles[num4].IsDead)
			{
				_playerProjectiles[num4].Unload();
				_playerProjectiles.RemoveAt(num4);
			}
		}
		for (int num5 = _levelProjectiles.Count - 1; num5 >= 0; num5--)
		{
			_levelProjectiles[num5].Update();
			if (_levelProjectiles[num5].IsDead)
			{
				_levelProjectiles[num5].Unload();
				_levelProjectiles.RemoveAt(num5);
			}
		}
		UpdateAvatars();
		foreach (MilMo_ExplorationToken explorationToken in _explorationTokens)
		{
			explorationToken.Update(position);
		}
		foreach (MilMo_CoinToken coinToken in _coinTokens)
		{
			coinToken.Update(position);
		}
		_premiumToken?.Update(position);
		foreach (MilMo_StaticGem staticGem in _staticGems)
		{
			staticGem.Update(position);
		}
		bool playerIsCloseToClimbingSurface = false;
		if (!MilMo_Instance.PlayerInstance.IsClimbing && !MilMo_LoadingScreen.Instance.IsLoading)
		{
			foreach (MilMo_ClimbingSurface value in _climbingSurfaces.Values)
			{
				if (value.ShouldAttach(MilMo_Instance.PlayerInstance, out var attachInfo, out var distanceToSurfaceSqr))
				{
					MilMo_Instance.PlayerInstance.StartClimbing(attachInfo);
					playerIsCloseToClimbingSurface = true;
					break;
				}
				if (distanceToSurfaceSqr < 4f)
				{
					playerIsCloseToClimbingSurface = true;
				}
			}
		}
		MilMo_RemotePlayerCollisionHandler.Instance.Update(playerIsCloseToClimbingSurface);
		Environment.UpdateSunFog();
		AmbientSoundManager?.Update();
		if (!PlayMusic)
		{
			return;
		}
		if (_currentMusicArea != null)
		{
			if (!_currentMusicArea.Inside(position))
			{
				StartPlayMusic();
			}
		}
		else
		{
			InsideMusicArea(position);
		}
	}

	public override void FixedUpdate()
	{
		foreach (MilMo_MovableObject creature in _creatures)
		{
			creature.FixedUpdate();
		}
		foreach (MilMo_LevelItem item in _items)
		{
			item.FixedUpdate();
		}
		foreach (MilMo_LevelNpc npc in _npcs)
		{
			npc.FixedUpdate();
		}
		base.FixedUpdate();
	}

	public override void LateUpdate()
	{
		foreach (MilMo_LevelNpc npc in _npcs)
		{
			npc.LateUpdate();
		}
		base.LateUpdate();
	}

	public void ResetZones()
	{
		foreach (MilMo_GameplayObject gameplayObject in _gameplayObjects)
		{
			gameplayObject.ResetZone();
		}
	}

	private void AddLevelItem(LevelItem item, bool useSpawnEffect)
	{
		if (!IsCorrectLevel(item.GetFullLevelName()))
		{
			Debug.Log("Got network item from wrong level (" + item.GetFullLevelName() + "!=" + VerboseName + ")");
			return;
		}
		if (_items.Any((MilMo_LevelItem existingItem) => existingItem.Id == item.GetId()))
		{
			Debug.Log("B) Item already exists " + item.GetItem().GetPath());
			return;
		}
		if (_removedItems.ContainsKey(item.GetId()))
		{
			Debug.Log("B) Item in removed items list " + item.GetItem().GetPath());
			return;
		}
		MilMo_LevelItem newItem = new MilMo_LevelItem(useSpawnEffect);
		newItem.Read(item, delegate(bool success, MilMo_LevelObject obj)
		{
			if (success)
			{
				if (_removedItems.ContainsKey(newItem.Id))
				{
					newItem.Unload();
				}
				else if (newItem.HaveUniqueItem)
				{
					newItem.Unload();
				}
				else if (!IsCorrectLevel(item.GetFullLevelName()))
				{
					Debug.Log("Got loaded item from wrong level (" + item.GetFullLevelName() + "!=" + VerboseName + ")");
					newItem.Unload();
				}
				else
				{
					_items.Add(newItem);
				}
			}
		});
	}

	private void TestItemAutoPickup()
	{
		SortedList<float, int> closeItemDistances = new SortedList<float, int>();
		foreach (MilMo_LevelItem item in _items)
		{
			if (!item.Item.AutoPickup())
			{
				continue;
			}
			float sqrDistanceToPlayer = item.SqrDistanceToPlayer;
			if (item.TestPickup(sqrDistanceToPlayer))
			{
				try
				{
					closeItemDistances.Add(sqrDistanceToPlayer, item.Id);
				}
				catch (ArgumentException)
				{
				}
			}
		}
		if (closeItemDistances.Values.Count <= 0)
		{
			return;
		}
		MilMo_LevelItem milMo_LevelItem = _items.FirstOrDefault((MilMo_LevelItem item) => item.Id == closeItemDistances.Values[0]);
		if (milMo_LevelItem != null)
		{
			if (milMo_LevelItem.HaveUniqueItem)
			{
				GameEvent.ThinkEvent?.RaiseEvent(new LocalizedStringWithArgument("World_429").GetMessage());
			}
			else if (milMo_LevelItem.HaveMaxAmount)
			{
				GameEvent.ThinkEvent?.RaiseEvent(new LocalizedStringWithArgument("World_432").GetMessage());
			}
			else
			{
				Singleton<GameNetwork>.Instance.RequestPickup(MilMo_Instance.PlayerInstance.Avatar.Position, closeItemDistances.Values);
			}
		}
	}

	private void AddNpc(Code.Core.Network.types.LevelObject npc)
	{
		if (!IsCorrectLevel(npc.GetFullLevelName()))
		{
			Debug.Log("Got network NPC from wrong level (" + npc.GetFullLevelName() + "!=" + VerboseName + ")");
			return;
		}
		string templateType = npc.GetTemplateType();
		if (!templateType.Equals("NPC", StringComparison.InvariantCultureIgnoreCase))
		{
			Debug.LogWarning("Got bad NPC, type is " + templateType);
			return;
		}
		new MilMo_LevelNpc(MilMo_Instance.PlayerInstance.Avatar.GameObject.transform, MilMo_Instance.PlayerInstance.Avatar.Head).Read(npc, delegate(bool success, MilMo_LevelObject obj)
		{
			if (success && obj is MilMo_LevelNpc milMo_LevelNpc)
			{
				if (!IsCorrectLevel(npc.GetFullLevelName()))
				{
					Debug.Log("Got loaded NPC from wrong level (" + npc.GetFullLevelName() + "!=" + VerboseName + ")");
					milMo_LevelNpc.Unload();
				}
				else
				{
					_npcs.Add(milMo_LevelNpc);
				}
			}
		});
	}

	private void AddGameplayObject(GameplayObject gameplayObject)
	{
		Debug.Log("AddGameplayObject " + gameplayObject.GetTemplate().GetPath());
		if (!IsCorrectLevel(gameplayObject.GetFullLevelName()))
		{
			Debug.Log("Got network gameplay object from wrong level (" + gameplayObject.GetFullLevelName() + "!=" + VerboseName + ")");
			return;
		}
		string templateType = gameplayObject.GetTemplateType();
		if (!templateType.Equals("GameplayObject", StringComparison.InvariantCultureIgnoreCase))
		{
			Debug.LogWarning("Got bad GameplayObject, type is " + templateType);
			return;
		}
		new MilMo_GameplayObject().Read(gameplayObject, delegate(bool success, MilMo_LevelObject obj)
		{
			if (!success)
			{
				Debug.LogWarning("Failed to load gameplay object " + gameplayObject.GetTemplate().GetPath());
			}
			else if (!(obj is MilMo_GameplayObject milMo_GameplayObject))
			{
				Debug.LogWarning("Gameplayobject " + gameplayObject.GetTemplate().GetPath() + " was null when loaded.");
			}
			else if (!IsCorrectLevel(gameplayObject.GetFullLevelName()))
			{
				Debug.Log("Got loaded gameplay object from wrong level (" + gameplayObject.GetFullLevelName() + "!=" + VerboseName + ")");
				milMo_GameplayObject.Unload();
			}
			else
			{
				milMo_GameplayObject.SetCapturerAsParent();
				milMo_GameplayObject.SetZoneCaptured(playSound: false);
				_gameplayObjects.Add(milMo_GameplayObject);
			}
		});
	}

	private static void FatalFail()
	{
		Singleton<GameNetwork>.Instance.DisconnectFromGameServer();
	}

	private void AddLevelCreature(Code.Core.Network.types.LevelObject gameCreature, Vector3 targetPosition, float speed, bool useSpawnEffect)
	{
		if (!IsCorrectLevel(gameCreature.GetFullLevelName()))
		{
			Debug.LogWarning("Got network game creature from wrong level (" + gameCreature.GetFullLevelName() + "!=" + VerboseName + ")");
			return;
		}
		string templateType = gameCreature.GetTemplateType();
		if (!templateType.Equals("Creature", StringComparison.InvariantCultureIgnoreCase))
		{
			Debug.LogWarning("Got bad Creature, type is " + templateType);
		}
		else
		{
			if (_creatures.Any((MilMo_MovableObject existingCreature) => existingCreature.Id == gameCreature.GetId()) || _removedCreatures.ContainsKey(gameCreature.GetId()))
			{
				return;
			}
			new MilMo_LevelCreature(useSpawnEffect).Read(gameCreature, delegate(bool success, MilMo_LevelObject obj)
			{
				if (!success)
				{
					Debug.LogWarning("Failed to read level creature.");
					FatalFail();
				}
				else if (obj is MilMo_LevelCreature milMo_LevelCreature)
				{
					if (_removedCreatures.ContainsKey(milMo_LevelCreature.Id))
					{
						milMo_LevelCreature.Unload();
					}
					else if (!IsCorrectLevel(gameCreature.GetFullLevelName()))
					{
						Debug.LogWarning("Got loaded game creature from wrong level (" + gameCreature.GetFullLevelName() + "!=" + VerboseName + ")");
						milMo_LevelCreature.Unload();
					}
					else
					{
						milMo_LevelCreature.InitializeMover(targetPosition, speed);
						_creatures.Add(milMo_LevelCreature);
					}
				}
			});
		}
	}

	private void AddLevelBoss(Code.Core.Network.types.LevelObject gameBoss, Vector3 targetPosition, float speed, bool useSpawnEffect)
	{
		if (!IsCorrectLevel(gameBoss.GetFullLevelName()))
		{
			Debug.LogWarning("Got network boss from wrong level (" + gameBoss.GetFullLevelName() + "!=" + VerboseName + ")");
			return;
		}
		string templateType = gameBoss.GetTemplateType();
		if (!templateType.Equals("Boss", StringComparison.InvariantCultureIgnoreCase))
		{
			Debug.LogWarning("Got bad Boss, type is " + templateType);
		}
		else
		{
			if (_creatures.Any((MilMo_MovableObject existingMovable) => existingMovable.Id == gameBoss.GetId()) || _removedCreatures.ContainsKey(gameBoss.GetId()))
			{
				return;
			}
			new MilMo_LevelBoss(useSpawnEffect).Read(gameBoss, delegate(bool success, MilMo_LevelObject obj)
			{
				if (!success)
				{
					Debug.LogWarning("Failed to read level boss.");
				}
				else if (obj is MilMo_LevelBoss milMo_LevelBoss)
				{
					if (_removedCreatures.ContainsKey(milMo_LevelBoss.Id))
					{
						milMo_LevelBoss.Unload();
					}
					else if (!IsCorrectLevel(gameBoss.GetFullLevelName()))
					{
						Debug.LogWarning("Got loaded boss from wrong level (" + gameBoss.GetFullLevelName() + "!=" + VerboseName + ")");
						milMo_LevelBoss.Unload();
					}
					else
					{
						milMo_LevelBoss.InitializeMover(targetPosition, speed);
						_creatures.Add(milMo_LevelBoss);
					}
				}
			});
		}
	}

	private void CreateNpCs(object msgAsObject)
	{
		if (!(msgAsObject is ServerNPCCreate serverNPCCreate))
		{
			Debug.LogWarning("Got null or wrong type in MilMo_Level.CreateNPCs");
			return;
		}
		foreach (Npc npc in serverNPCCreate.getNpcs())
		{
			AddNpc(npc);
		}
	}

	private void CreateGameplayObjects(object msgAsObject)
	{
		if (!(msgAsObject is ServerGameplayObjectsCreate serverGameplayObjectsCreate))
		{
			Debug.LogWarning("Got null message or wrong message type in MilMo_Level.CreateGameplayObjects");
			return;
		}
		foreach (GameplayObject gameplayObject in serverGameplayObjectsCreate.getGameplayObjects())
		{
			AddGameplayObject(gameplayObject);
		}
	}

	private void CreateExplorationTokens(object msgAsObject)
	{
		if (!(msgAsObject is ServerExplorationTokensCreate serverExplorationTokensCreate))
		{
			Debug.LogWarning("Got null message or wrong message type in MilMo_Level.CreateExplorationTokens");
			return;
		}
		if (!serverExplorationTokensCreate.getFullLevelName().Equals(VerboseName, StringComparison.InvariantCultureIgnoreCase))
		{
			Debug.Log("Got network exploration tokens from wrong level (" + serverExplorationTokensCreate.getFullLevelName() + "!=" + VerboseName + ")");
			return;
		}
		sbyte b = 0;
		foreach (ExplorationToken explorationToken in serverExplorationTokensCreate.getExplorationTokens())
		{
			_explorationTokens.Add(new MilMo_ExplorationToken(explorationToken, b));
			b++;
		}
		MilMo_LevelInfo.UpdateExplorationTokens(_explorationTokens, VerboseName);
	}

	private void CreateCoinTokens(object msgAsObject)
	{
		if (!(msgAsObject is ServerCoinTokensCreate serverCoinTokensCreate))
		{
			Debug.LogWarning("Got null message or wrong message type in MilMo_Level.CreateCoinTokens");
			return;
		}
		if (!serverCoinTokensCreate.getFullLevelName().Equals(VerboseName, StringComparison.InvariantCultureIgnoreCase))
		{
			Debug.Log("Got network coin tokens from wrong level (" + serverCoinTokensCreate.getFullLevelName() + "!=" + VerboseName + ")");
			return;
		}
		sbyte b = 0;
		foreach (CoinToken coinToken in serverCoinTokensCreate.getCoinTokens())
		{
			_coinTokens.Add(new MilMo_CoinToken(coinToken, b));
			b++;
		}
		MilMo_LevelInfo.UpdateCoinTokens(_coinTokens, VerboseName);
	}

	private void CreateStaticGems(object msgAsObject)
	{
		if (!(msgAsObject is ServerStaticGems serverStaticGems))
		{
			Debug.LogWarning("Got null message or wrong message type in MilMo_Level.CreateStaticGems");
			return;
		}
		if (!IsCorrectLevel(serverStaticGems.getFullLevelName()))
		{
			Debug.Log("Got network static gems from wrong level (" + serverStaticGems.getFullLevelName() + "!=" + VerboseName + ")");
			return;
		}
		sbyte b = 0;
		foreach (StaticGem gem in serverStaticGems.getGems())
		{
			_staticGems.Add(new MilMo_StaticGem(gem, b));
			b++;
		}
	}

	private void ObjectCreate(object msgAsObj)
	{
		if (msgAsObj is ServerObjectCreate serverObjectCreate)
		{
			AddLevelItem(serverObjectCreate.getItem(), useSpawnEffect: true);
		}
	}

	private void ObjectMovableCreate(object msgAsObj)
	{
		if (msgAsObj is ServerObjectMoveableCreate serverObjectMoveableCreate)
		{
			if (serverObjectMoveableCreate.getTheObject().GetTemplateType().Equals("Creature", StringComparison.InvariantCultureIgnoreCase))
			{
				AddLevelCreature(serverObjectMoveableCreate.getTheObject(), new Vector3(serverObjectMoveableCreate.getTargetPosition().GetX(), serverObjectMoveableCreate.getTargetPosition().GetY(), serverObjectMoveableCreate.getTargetPosition().GetZ()), serverObjectMoveableCreate.getSpeed(), useSpawnEffect: true);
			}
			else if (serverObjectMoveableCreate.getTheObject().GetTemplateType().Equals("Boss", StringComparison.InvariantCultureIgnoreCase))
			{
				AddLevelBoss(serverObjectMoveableCreate.getTheObject(), new Vector3(serverObjectMoveableCreate.getTargetPosition().GetX(), serverObjectMoveableCreate.getTargetPosition().GetY(), serverObjectMoveableCreate.getTargetPosition().GetZ()), serverObjectMoveableCreate.getSpeed(), useSpawnEffect: true);
			}
			else
			{
				Debug.LogWarning("Got ServerObjectMovableCreate message with invalid object type '" + serverObjectMoveableCreate.getTheObject().GetTemplateType() + "' (expected 'Creature' or 'Boss'");
			}
		}
	}

	private void MovableHealthUpdate(object msgAsObj)
	{
		if (msgAsObj is ServerMoveableHealthUpdate serverMoveableHealthUpdate)
		{
			int id = serverMoveableHealthUpdate.getMoveableID();
			MilMo_MovableObject milMo_MovableObject = _creatures.Find((MilMo_MovableObject o) => o.Id == id);
			if (milMo_MovableObject != null)
			{
				milMo_MovableObject.ReadHealthUpdate(serverMoveableHealthUpdate);
			}
			else if (serverMoveableHealthUpdate.getHealth() <= 0f && !_removedCreatures.ContainsKey(serverMoveableHealthUpdate.getMoveableID()))
			{
				_removedCreatures.Add(serverMoveableHealthUpdate.getMoveableID(), 0);
			}
		}
	}

	private void MovableAttack(object msgAsObj)
	{
		if (!(msgAsObj is ServerCreatureAttackInstant serverCreatureAttackInstant))
		{
			return;
		}
		string playerID = serverCreatureAttackInstant.getPlayerID();
		int creatureId = serverCreatureAttackInstant.getCreatureID();
		MilMo_MovableObject milMo_MovableObject = _creatures.FirstOrDefault((MilMo_MovableObject c) => c.Id == creatureId);
		MilMo_Avatar avatar;
		if (playerID == MilMo_Instance.PlayerInstance.Id)
		{
			avatar = MilMo_Instance.PlayerInstance.Avatar;
		}
		else
		{
			MilMo_RemotePlayer remotePlayer = GetRemotePlayer(playerID);
			if (remotePlayer == null)
			{
				return;
			}
			avatar = remotePlayer.Avatar;
		}
		MilMo_CreatureAttackTemplate milMo_CreatureAttackTemplate = null;
		if (milMo_MovableObject != null)
		{
			Transform target = null;
			if (avatar != null && avatar.GameObject != null)
			{
				target = avatar.GameObject.transform;
			}
			milMo_CreatureAttackTemplate = milMo_MovableObject.Attack(serverCreatureAttackInstant.getAttack(), target);
		}
		if (avatar == null)
		{
			return;
		}
		if (milMo_CreatureAttackTemplate == null)
		{
			if (playerID == MilMo_Instance.PlayerInstance.Id)
			{
				avatar.Armor.DamageArmor(serverCreatureAttackInstant.getArmorDamage());
			}
			avatar.UpdateHealth(serverCreatureAttackInstant.getHealthLeft());
		}
		else
		{
			milMo_CreatureAttackTemplate.Instantiate(milMo_MovableObject, avatar, serverCreatureAttackInstant.getIsHit() == 1, serverCreatureAttackInstant.getHealthDamage(), serverCreatureAttackInstant.getArmorDamage(), serverCreatureAttackInstant.getHealthLeft());
		}
	}

	private void MovablePrepareAttack(object msgAsObj)
	{
		if (!(msgAsObj is ServerCreaturePrepareAttack serverCreaturePrepareAttack))
		{
			return;
		}
		string targetPlayerID = serverCreaturePrepareAttack.getTargetPlayerID();
		int creatureId = serverCreaturePrepareAttack.getCreatureID();
		MilMo_MovableObject milMo_MovableObject = _creatures.FirstOrDefault((MilMo_MovableObject c) => c.Id == creatureId);
		MilMo_Avatar avatar;
		if (targetPlayerID == MilMo_Instance.PlayerInstance.Avatar.Id)
		{
			avatar = MilMo_Instance.PlayerInstance.Avatar;
		}
		else
		{
			MilMo_RemotePlayer remotePlayer = GetRemotePlayer(targetPlayerID);
			if (remotePlayer == null)
			{
				return;
			}
			avatar = remotePlayer.Avatar;
		}
		milMo_MovableObject?.PrepareAttack(avatar, serverCreaturePrepareAttack.getAttack());
	}

	private void MovableBeginAttack(object msgAsObj)
	{
		if (!(msgAsObj is ServerCreatureBeginAttack serverCreatureBeginAttack))
		{
			return;
		}
		int creatureId = serverCreatureBeginAttack.getCreatureID();
		MilMo_MovableObject creature = _creatures.FirstOrDefault((MilMo_MovableObject c) => c.Id == creatureId);
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(serverCreatureBeginAttack.getAttack(), delegate(MilMo_Template template, bool timeOut)
		{
			if (!timeOut && template is MilMo_CreatureAttackTemplate milMo_CreatureAttackTemplate && creature != null)
			{
				milMo_CreatureAttackTemplate.Instantiate(creature, new List<MilMo_CreatureAttack.MilMo_DamageToPlayer>());
			}
		});
	}

	private void MovableAttackDone(object msgAsObj)
	{
		if (!(msgAsObj is ServerMovableAttackDone serverMovableAttackDone))
		{
			return;
		}
		int creatureId = serverMovableAttackDone.getId();
		using IEnumerator<MilMo_MovableObject> enumerator = _creatures.Where((MilMo_MovableObject c) => c.Id == creatureId).GetEnumerator();
		if (enumerator.MoveNext())
		{
			enumerator.Current.AttackDone();
		}
	}

	private void MovablePreparedAttackImpact(object msgAsObj)
	{
		if (!(msgAsObj is ServerCreatureAttackImpact serverCreatureAttackImpact))
		{
			return;
		}
		int creatureId = serverCreatureAttackImpact.getCreatureID();
		MilMo_MovableObject creature = _creatures.FirstOrDefault((MilMo_MovableObject c) => c.Id == creatureId);
		List<MilMo_CreatureAttack.MilMo_DamageToPlayer> damagedPlayers = new List<MilMo_CreatureAttack.MilMo_DamageToPlayer>();
		foreach (DamageToPlayer item in serverCreatureAttackImpact.getPlayersHit())
		{
			if (item.GetPlayerID() == MilMo_Instance.PlayerInstance.Avatar.Id)
			{
				damagedPlayers.Add(new MilMo_CreatureAttack.MilMo_DamageToPlayer(MilMo_Instance.PlayerInstance.Avatar, item.GetHealthDamage(), item.GetArmorDamage(), item.GetHealthLeft()));
				continue;
			}
			MilMo_RemotePlayer remotePlayer = GetRemotePlayer(item.GetPlayerID());
			if (remotePlayer != null)
			{
				damagedPlayers.Add(new MilMo_CreatureAttack.MilMo_DamageToPlayer(remotePlayer.Avatar, item.GetHealthDamage(), item.GetArmorDamage(), item.GetHealthLeft()));
			}
		}
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(serverCreatureAttackImpact.getAttack(), delegate(MilMo_Template template, bool timeOut)
		{
			if (timeOut || !(template is MilMo_CreatureAttackTemplate milMo_CreatureAttackTemplate) || creature == null)
			{
				foreach (MilMo_CreatureAttack.MilMo_DamageToPlayer item2 in damagedPlayers.Where((MilMo_CreatureAttack.MilMo_DamageToPlayer damagedPlayer) => damagedPlayer.MAvatar != null))
				{
					item2.MAvatar.UpdateHealth(item2.MHealthLeft);
					if (item2.MAvatar.Id == MilMo_Instance.PlayerInstance.Avatar.Id)
					{
						MilMo_Instance.PlayerInstance.Avatar.Armor.DamageArmor(item2.MArmorDamage);
					}
					if (item2.MAvatar.GameObject != null)
					{
						item2.MAvatar.Damaged(item2.MHealthDamage, creature?.Position ?? item2.MAvatar.Position);
					}
				}
				return;
			}
			milMo_CreatureAttackTemplate.Instantiate(creature, damagedPlayers);
		});
	}

	private void PlayersHitByCapsule(object msgAsObj)
	{
		if (!(msgAsObj is ServerCapsuleAttackDamage serverCapsuleAttackDamage))
		{
			return;
		}
		Vector3 lineStart = new Vector3(serverCapsuleAttackDamage.getStartPos().GetX(), serverCapsuleAttackDamage.getStartPos().GetY(), serverCapsuleAttackDamage.getStartPos().GetZ());
		Vector3 lineEnd = new Vector3(serverCapsuleAttackDamage.getEndPos().GetX(), serverCapsuleAttackDamage.getEndPos().GetY(), serverCapsuleAttackDamage.getEndPos().GetZ());
		foreach (DamageToPlayer item in serverCapsuleAttackDamage.getPlayersHit())
		{
			MilMo_Avatar avatar;
			if (item.GetPlayerID() == MilMo_Instance.PlayerInstance.Id)
			{
				avatar = MilMo_Instance.PlayerInstance.Avatar;
				avatar.Armor.DamageArmor(item.GetArmorDamage());
			}
			else
			{
				MilMo_RemotePlayer remotePlayer = GetRemotePlayer(item.GetPlayerID());
				if (remotePlayer == null)
				{
					continue;
				}
				avatar = remotePlayer.Avatar;
			}
			if (avatar == null)
			{
				continue;
			}
			avatar.UpdateHealth(item.GetHealthLeft());
			if (!(avatar.GameObject == null))
			{
				Vector3 vector = MilMo_Physics.ClosestPointToPointOnLineSegment(lineStart, lineEnd, avatar.Position);
				if (avatar == MilMo_Instance.PlayerInstance.Avatar)
				{
					Vector3 impulse = ((!MilMo_Utility.Equals(avatar.Position, vector)) ? (avatar.Position - vector).normalized : avatar.GameObject.transform.forward);
					impulse *= serverCapsuleAttackDamage.getPlayerImpulseXZ();
					impulse.y = serverCapsuleAttackDamage.getPlayerImpulseY();
					MilMo_PlayerControllerBase.AddKnockBack(impulse);
				}
				avatar.Damaged(item.GetHealthDamage(), vector);
			}
		}
	}

	private void PlayerHitByProjectile(object msgAsObj)
	{
		if (!(msgAsObj is ServerPlayerHitByProjectile serverPlayerHitByProjectile))
		{
			return;
		}
		string playerID = serverPlayerHitByProjectile.getPlayerID();
		int projectileId = serverPlayerHitByProjectile.getProjectileID();
		MilMo_Avatar avatar;
		if (playerID == MilMo_Instance.PlayerInstance.Avatar.Id)
		{
			avatar = MilMo_Instance.PlayerInstance.Avatar;
		}
		else
		{
			MilMo_RemotePlayer remotePlayer = GetRemotePlayer(playerID);
			if (remotePlayer == null)
			{
				return;
			}
			avatar = remotePlayer.Avatar;
		}
		MilMo_LevelProjectile milMo_LevelProjectile = _levelProjectiles.FirstOrDefault((MilMo_LevelProjectile p) => p.Id == projectileId);
		if (avatar != null)
		{
			float health = avatar.Health;
			if (health > 0f)
			{
				float health2 = serverPlayerHitByProjectile.getHealth();
				float armorDurability = serverPlayerHitByProjectile.getArmorDurability();
				float amount = health - health2;
				if (avatar.Id == MilMo_Instance.PlayerInstance.Avatar.Id)
				{
					avatar.Armor.UpdateArmor(armorDurability);
				}
				avatar.UpdateHealth(health2);
				if (avatar.HeadStart != null)
				{
					avatar.Damaged(amount, milMo_LevelProjectile?.Position ?? avatar.HeadStart.position);
				}
				if (health2 <= 0f && MilMo_LevelInfo.IsPvp(VerboseName))
				{
					Vector3 force = Vector3.zero;
					if (milMo_LevelProjectile != null && !MilMo_Utility.Equals(avatar.Position, milMo_LevelProjectile.Position))
					{
						force = (avatar.Position - milMo_LevelProjectile.Position).normalized;
					}
					avatar.EnableRagdoll(force, ForcePosition.Torso);
				}
			}
		}
		milMo_LevelProjectile?.ImpactKill();
	}

	private void ProjectileDestroyed(object msgAsObj)
	{
		if (!(msgAsObj is ServerProjectileDestroyed serverProjectileDestroyed))
		{
			return;
		}
		int projectileId = serverProjectileDestroyed.getProjectileId();
		using IEnumerator<MilMo_LevelProjectile> enumerator = _levelProjectiles.Where((MilMo_LevelProjectile projectile) => projectile.Id == projectileId).GetEnumerator();
		if (enumerator.MoveNext())
		{
			enumerator.Current.ImpactKill();
		}
	}

	private void CreatureHitByProjectile(object msgAsObj)
	{
		if (!(msgAsObj is ServerCreatureHitByProjectile serverCreatureHitByProjectile))
		{
			return;
		}
		int creatureId = serverCreatureHitByProjectile.getCreatureId();
		int projectileId = serverCreatureHitByProjectile.getProjectileId();
		MilMo_MovableObject milMo_MovableObject = _creatures.FirstOrDefault((MilMo_MovableObject movable) => movable.Id == creatureId);
		MilMo_LevelProjectile milMo_LevelProjectile = _levelProjectiles.FirstOrDefault((MilMo_LevelProjectile p) => p.Id == projectileId);
		if (milMo_MovableObject != null && !milMo_MovableObject.IsDeadOrDying())
		{
			float health = milMo_MovableObject.Health;
			float newHealth = serverCreatureHitByProjectile.getNewHealth();
			float damage = health - newHealth;
			milMo_MovableObject.Damage(damage);
			if (milMo_LevelProjectile == null)
			{
				milMo_MovableObject.DamageEffectLocal(damage);
			}
			else
			{
				milMo_MovableObject.DamageEffectLocal(milMo_LevelProjectile, damage);
			}
			if (newHealth <= 0f)
			{
				milMo_MovableObject.Kill();
			}
		}
		milMo_LevelProjectile?.ImpactKill();
	}

	private void CreatureAggro(object msgAsObj)
	{
		if (!(msgAsObj is ServerCreatureAggro serverCreatureAggro))
		{
			return;
		}
		string playerId = serverCreatureAggro.getPlayerId();
		int creatureId = serverCreatureAggro.getCreatureId();
		MilMo_Avatar milMo_Avatar = null;
		if (playerId == MilMo_Instance.PlayerInstance.Avatar.Id)
		{
			milMo_Avatar = MilMo_Instance.PlayerInstance.Avatar;
		}
		else
		{
			MilMo_RemotePlayer remotePlayer = GetRemotePlayer(playerId);
			if (remotePlayer != null)
			{
				milMo_Avatar = remotePlayer.Avatar;
			}
		}
		MilMo_MovableObject milMo_MovableObject = null;
		using (IEnumerator<MilMo_MovableObject> enumerator = _creatures.Where((MilMo_MovableObject c) => c.Id == creatureId).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				MilMo_MovableObject current = enumerator.Current;
				current.Aggro(milMo_Avatar?.Position ?? Vector3.zero);
				milMo_MovableObject = current;
			}
		}
		if (milMo_Avatar == null)
		{
			return;
		}
		if (milMo_MovableObject == null)
		{
			milMo_Avatar.ResetCombatTimer();
			return;
		}
		if (milMo_MovableObject.IsDangerous())
		{
			milMo_Avatar.ResetCombatTimer();
		}
		milMo_Avatar.SuperAlivenessManager.CritterAggro(milMo_MovableObject);
	}

	private void CreatureNoAggro(object msgAsObj)
	{
		ServerCreatureNoAggro msg = msgAsObj as ServerCreatureNoAggro;
		if (msg == null)
		{
			return;
		}
		using IEnumerator<MilMo_MovableObject> enumerator = _creatures.Where((MilMo_MovableObject c) => c.Id == msg.getCreatureId()).GetEnumerator();
		if (!enumerator.MoveNext())
		{
			return;
		}
		MilMo_MovableObject current = enumerator.Current;
		current.NoAggro();
		foreach (MilMo_Avatar avatar in base.Avatars)
		{
			avatar.SuperAlivenessManager.CritterNoAggro(current);
		}
	}

	private void PlayerAttackHit(object msgAsObject)
	{
		ServerPlayerHitAttack msg1 = msgAsObject as ServerPlayerHitAttack;
		if (msg1 != null)
		{
			using (IEnumerator<MilMo_MovableObject> enumerator = _creatures.Where((MilMo_MovableObject c) => c.Id == msg1.getTargetCreatureId()).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					MilMo_MovableObject current = enumerator.Current;
					PlayerAttack(msg1.getPlayerId(), current, msg1.getDamage(), hit: true, msg1.getKillingBlow() != 0, msg1.getUsingWeapon() != 0, null);
				}
				return;
			}
		}
		if (msgAsObject is ServerPlayerHitAttackOnPlayer serverPlayerHitAttackOnPlayer)
		{
			MilMo_Avatar milMo_Avatar = null;
			if (serverPlayerHitAttackOnPlayer.getTargetId() == MilMo_Instance.PlayerInstance.Id)
			{
				milMo_Avatar = MilMo_Instance.PlayerInstance.Avatar;
			}
			else if (base.Players.ContainsKey(serverPlayerHitAttackOnPlayer.getTargetId()))
			{
				milMo_Avatar = base.Players[serverPlayerHitAttackOnPlayer.getTargetId()].Avatar;
			}
			if (milMo_Avatar != null)
			{
				PlayerAttack(serverPlayerHitAttackOnPlayer.getAttackerId(), milMo_Avatar, serverPlayerHitAttackOnPlayer.getDamage(), hit: true, serverPlayerHitAttackOnPlayer.getKillingBlow() != 0, serverPlayerHitAttackOnPlayer.getUsingWeapon() != 0, serverPlayerHitAttackOnPlayer.getWeaponTemplate());
			}
		}
	}

	private void PlayerAttackMissed(object msgAsObject)
	{
		ServerPlayerMissedAttack msg1 = msgAsObject as ServerPlayerMissedAttack;
		if (msg1 != null)
		{
			using (IEnumerator<MilMo_MovableObject> enumerator = _creatures.Where((MilMo_MovableObject c) => c.Id == msg1.getTargetCreatureId()).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					MilMo_MovableObject current = enumerator.Current;
					PlayerAttack(msg1.getPlayerId(), current, 0f, hit: false, killingBlow: false, isUsingWeapon: true, null);
				}
				return;
			}
		}
		if (msgAsObject is ServerPlayerMissedAttackOnPlayer serverPlayerMissedAttackOnPlayer)
		{
			MilMo_Avatar target = null;
			if (serverPlayerMissedAttackOnPlayer.getTargetId() == MilMo_Instance.PlayerInstance.Id)
			{
				target = MilMo_Instance.PlayerInstance.Avatar;
			}
			else if (base.Players.ContainsKey(serverPlayerMissedAttackOnPlayer.getTargetId()))
			{
				target = base.Players[serverPlayerMissedAttackOnPlayer.getTargetId()].Avatar;
			}
			PlayerAttack(serverPlayerMissedAttackOnPlayer.getAttackerId(), target, 0f, hit: false, killingBlow: false, isUsingWeapon: true, null);
		}
	}

	private void PlayerAttackUntargeted(object msgAsObject)
	{
		if (msgAsObject is ServerPlayerUntargetedAttack serverPlayerUntargetedAttack)
		{
			PlayerAttack(serverPlayerUntargetedAttack.getPlayerId(), null, 0f, hit: false, killingBlow: false, isUsingWeapon: true, null);
		}
	}

	private void PlayerAttack(string playerId, IMilMo_AttackTarget target, float damage, bool hit, bool killingBlow, bool isUsingWeapon, string weaponTemplateName)
	{
		MilMo_Avatar attacker;
		if (playerId == MilMo_Instance.PlayerInstance.Avatar.Id)
		{
			attacker = MilMo_Instance.PlayerInstance.Avatar;
		}
		else
		{
			MilMo_RemotePlayer remotePlayer = GetRemotePlayer(playerId);
			if (remotePlayer == null)
			{
				if (hit)
				{
					target?.Damage(damage);
				}
				return;
			}
			attacker = remotePlayer.Avatar;
		}
		MilMo_Weapon milMo_Weapon = attacker.WieldedItem as MilMo_Weapon;
		if (milMo_Weapon == null && isUsingWeapon)
		{
			if (hit)
			{
				target?.Damage(damage);
			}
			return;
		}
		if (!isUsingWeapon)
		{
			milMo_Weapon = null;
		}
		MilMo_PlayerAttack.CreateAttack(VerboseName, attacker, target, hit, damage, killingBlow, milMo_Weapon);
		if (!killingBlow)
		{
			return;
		}
		MilMo_Avatar targetAvatar = target as MilMo_Avatar;
		if (targetAvatar == null)
		{
			return;
		}
		if (weaponTemplateName != null)
		{
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(weaponTemplateName, delegate(MilMo_Template template, bool timeout)
			{
				string localizedString = LocalizationHelper.GetLocalizedString("PVP", "PVP_Defeated", new List<string> { attacker.Name, targetAvatar.Name });
				string iconKey = "";
				if (!timeout && template is MilMo_ItemTemplate milMo_ItemTemplate)
				{
					iconKey = milMo_ItemTemplate.IconPath.Substring(milMo_ItemTemplate.IconPath.LastIndexOf('/') + 1);
				}
				GameEvent.OnChatMessageReceivedEvent.RaiseEvent(new ChatMessageObject("", localizedString, ChatMessageObject.MessageType.PvpKill, "", iconKey));
			});
		}
		float num = (isUsingWeapon ? milMo_Weapon.Template.Impact : 1.5f);
		Vector3 vector = (MilMo_Utility.Equals(targetAvatar.Position, attacker.Position) ? (-targetAvatar.GameObject.transform.forward) : (targetAvatar.Position - attacker.Position).normalized);
		Vector3 force = vector * num;
		targetAvatar.EnableRagdoll(force, ForcePosition.Torso);
	}

	private void PlayerKilledPlayer(object msgAsObject)
	{
		if (!(msgAsObject is ServerPlayerKilledByPlayer serverPlayerKilledByPlayer))
		{
			return;
		}
		MilMo_Avatar attacker;
		if (serverPlayerKilledByPlayer.getKillerIdentifier() == MilMo_Instance.PlayerInstance.Avatar.Id)
		{
			attacker = MilMo_Instance.PlayerInstance.Avatar;
		}
		else
		{
			MilMo_RemotePlayer remotePlayer = GetRemotePlayer(serverPlayerKilledByPlayer.getKillerIdentifier());
			if (remotePlayer == null)
			{
				return;
			}
			attacker = remotePlayer.Avatar;
		}
		MilMo_Avatar target;
		if (serverPlayerKilledByPlayer.getKilledIdentifier() == MilMo_Instance.PlayerInstance.Avatar.Id)
		{
			target = MilMo_Instance.PlayerInstance.Avatar;
		}
		else
		{
			MilMo_RemotePlayer remotePlayer2 = GetRemotePlayer(serverPlayerKilledByPlayer.getKilledIdentifier());
			if (remotePlayer2 == null)
			{
				return;
			}
			target = remotePlayer2.Avatar;
		}
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(serverPlayerKilledByPlayer.getWeapon(), delegate(MilMo_Template template, bool timeout)
		{
			string localizedString = LocalizationHelper.GetLocalizedString("PVP", "PVP_Defeated", new List<string> { attacker.Name, target.Name });
			string iconKey = "";
			if (!timeout && template is MilMo_ItemTemplate milMo_ItemTemplate)
			{
				iconKey = milMo_ItemTemplate.IconPath.Substring(milMo_ItemTemplate.IconPath.LastIndexOf('/') + 1);
			}
			GameEvent.OnChatMessageReceivedEvent.RaiseEvent(new ChatMessageObject("", localizedString, ChatMessageObject.MessageType.PvpKill, "", iconKey));
		});
	}

	private void PlayerDig(object msgAsObject)
	{
		if (msgAsObject is ServerDig serverDig && !(serverDig.getPlayerId() == MilMo_Instance.PlayerInstance.Avatar.Id))
		{
			GetRemotePlayer(serverDig.getPlayerId())?.Dig();
		}
	}

	private void MoveableObjectImpulse(object msgAsObject)
	{
		if (msgAsObject is ServerMoveableImpulse serverMoveableImpulse)
		{
			int id = serverMoveableImpulse.getCreatureId();
			_creatures.Find((MilMo_MovableObject o) => o.Id == id)?.HandleImpulse(serverMoveableImpulse);
		}
	}

	private void CreatureStunned(object msgAsObject)
	{
		if (msgAsObject is ServerCreatureStunned serverCreatureStunned)
		{
			int id = serverCreatureStunned.getCreatureId();
			_creatures.Find((MilMo_MovableObject o) => o.Id == id)?.Stunned();
		}
	}

	private void ObjectUpdate(object msgAsObj)
	{
		if (!(msgAsObj is ServerObjectUpdate serverObjectUpdate))
		{
			Debug.LogWarning("null message in ObjectUpdate");
			return;
		}
		int id = serverObjectUpdate.getUpdate().GetId();
		_creatures.Find((MilMo_MovableObject o) => o.Id == id)?.ReadUpdate((CreatureUpdate)serverObjectUpdate.getUpdate());
	}

	private void ObjectDestroy(object msgAsObj)
	{
		if (!(msgAsObj is ServerObjectDestroy serverObjectDestroy))
		{
			return;
		}
		string type = serverObjectDestroy.getType();
		int id = serverObjectDestroy.getItemID();
		if (type == "Creature" || type == "Boss")
		{
			MilMo_MovableObject creature = _creatures.Find((MilMo_MovableObject o) => o.Id == id);
			if (creature != null)
			{
				if (serverObjectDestroy.getUseDeathEffects() != 0 && MilMo_World.Instance.enabled)
				{
					if (creature.ShouldBeKilled)
					{
						creature.Kill();
						return;
					}
					creature.ShouldBeKilled = true;
					MilMo_EventSystem.At(5f, delegate
					{
						if (!creature.IsDeadOrDying())
						{
							creature.Kill();
						}
					});
				}
				else
				{
					creature.Unload();
					_creatures.Remove(creature);
					if (!_removedCreatures.ContainsKey(creature.Id))
					{
						_removedCreatures.Add(creature.Id, 0);
					}
				}
			}
			else
			{
				Debug.LogWarning("Got destroy message for creature that does not exist on the client.");
				if (!_removedCreatures.ContainsKey(id))
				{
					_removedCreatures.Add(id, 0);
				}
			}
		}
		else
		{
			MilMo_LevelItem milMo_LevelItem = _items.Find((MilMo_LevelItem o) => o.Id == id);
			if (milMo_LevelItem != null)
			{
				milMo_LevelItem.Unload();
				_items.Remove(milMo_LevelItem);
			}
			else
			{
				Debug.Log("Got destroy message for item that does not exist on the client.");
			}
			if (!_removedItems.ContainsKey(id))
			{
				_removedItems.Add(id, 0);
			}
		}
	}

	private void ItemAddedToPlayerInventory(object msgAsObject)
	{
		if (!(msgAsObject is MilMo_InventoryEntry milMo_InventoryEntry) || !milMo_InventoryEntry.Item.Template.IsUnique || CurrentLevel == null)
		{
			return;
		}
		for (int num = CurrentLevel.Items.Count - 1; num >= 0; num--)
		{
			MilMo_LevelItem milMo_LevelItem = CurrentLevel.Items[num];
			if (!(milMo_LevelItem.Item.Identifier != milMo_InventoryEntry.Item.Identifier) && milMo_LevelItem.Item.Template.IsUnique)
			{
				milMo_LevelItem.Unload();
				CurrentLevel.Items.RemoveAt(num);
				if (!_removedItems.ContainsKey(milMo_LevelItem.Id))
				{
					_removedItems.Add(milMo_LevelItem.Id, 0);
				}
			}
		}
	}

	private void StartClimb(object msgAsObj)
	{
		if (!(msgAsObj is ServerStartClimb serverStartClimb))
		{
			return;
		}
		string playerId = serverStartClimb.getPlayerId();
		if (playerId == MilMo_Instance.PlayerInstance.Id)
		{
			return;
		}
		MilMo_RemotePlayer value = GetRemotePlayer(playerId);
		if (value == null)
		{
			PlayersReceived.TryGetValue(playerId, out value);
		}
		if (value != null)
		{
			if (_climbingSurfaces.TryGetValue(serverStartClimb.getClimbingSurface(), out var value2))
			{
				value.StartClimbing(value2);
			}
			else
			{
				Debug.LogWarning("Got invalid climbing surface id " + serverStartClimb.getClimbingSurface() + " for remote player " + playerId);
			}
		}
	}

	private void StopClimb(object msgAsObj)
	{
		if (!(msgAsObj is ServerStopClimb serverStopClimb))
		{
			return;
		}
		string playerId = serverStopClimb.getPlayerId();
		if (!(playerId == MilMo_Instance.PlayerInstance.Id))
		{
			MilMo_RemotePlayer value = GetRemotePlayer(playerId);
			if (value == null)
			{
				PlayersReceived.TryGetValue(playerId, out value);
			}
			value?.StopClimbing();
		}
	}

	public MilMo_ClimbingSurface GetClimbingSurface(int climbingSurfaceIdentifier)
	{
		if (!_climbingSurfaces.TryGetValue(climbingSurfaceIdentifier, out var value))
		{
			return null;
		}
		return value;
	}

	public bool PlayerIsCloseToClimbingSurface()
	{
		return _climbingSurfaces.Values.Any((MilMo_ClimbingSurface climbingSurface) => climbingSurface.PlayerIsClose);
	}

	private void EnterGameplayObject(object msgAsObj)
	{
		if (!(msgAsObj is ServerEnterGameplayObject serverEnterGameplayObject))
		{
			return;
		}
		string playerId = serverEnterGameplayObject.getPlayerId();
		if (!(playerId == MilMo_Instance.PlayerInstance.Id))
		{
			MilMo_RemotePlayer value = GetRemotePlayer(playerId);
			if (value == null)
			{
				PlayersReceived.TryGetValue(playerId, out value);
			}
			value?.EnterGameplayObject(serverEnterGameplayObject.getGameplayObjectId());
		}
	}

	private void LeaveGameplayObject(object msgAsObj)
	{
		if (!(msgAsObj is ServerLeaveGameplayObject serverLeaveGameplayObject))
		{
			return;
		}
		string playerId = serverLeaveGameplayObject.getPlayerId();
		if (!(playerId == MilMo_Instance.PlayerInstance.Id))
		{
			MilMo_RemotePlayer value = GetRemotePlayer(playerId);
			if (value == null)
			{
				PlayersReceived.TryGetValue(playerId, out value);
			}
			value?.LeaveGameplayObject();
		}
	}

	public MilMo_GameplayObject GetGameplayObject(int id)
	{
		return _gameplayObjects.FirstOrDefault((MilMo_GameplayObject gameplayObject) => gameplayObject.Id == id);
	}

	private void LevelObjectPickup(object msgAsObj)
	{
		if (!(msgAsObj is ServerLevelObjectPickup serverLevelObjectPickup))
		{
			return;
		}
		int itemId = serverLevelObjectPickup.getItemID();
		string playerID = serverLevelObjectPickup.getPlayerID();
		MilMo_LevelItem milMo_LevelItem = _items.Find((MilMo_LevelItem o) => o.Id == itemId);
		if (milMo_LevelItem != null)
		{
			MilMo_Avatar milMo_Avatar = null;
			if (playerID != MilMo_Instance.PlayerInstance.Avatar.Id)
			{
				MilMo_RemotePlayer remotePlayer = GetRemotePlayer(playerID);
				if (remotePlayer != null)
				{
					milMo_Avatar = remotePlayer.Avatar;
				}
			}
			else
			{
				milMo_Avatar = MilMo_Instance.PlayerInstance.Avatar;
				if (milMo_Avatar == null || milMo_Avatar.GameObject == null || milMo_LevelItem.Item.AutoPickup())
				{
					MilMo_World.Instance.PlayerController.Unlock();
				}
			}
			if (milMo_Avatar != null)
			{
				if (!milMo_LevelItem.Item.AutoPickup())
				{
					milMo_Avatar.PlayAnimation(milMo_LevelItem.Item.PickupAnimation);
				}
				if (playerID == MilMo_Instance.PlayerInstance.Avatar.Id)
				{
					milMo_Avatar.PlaySoundEffect((milMo_LevelItem.Item.Template.PickupSound != "none") ? milMo_LevelItem.Item.Template.PickupSound : "Content/Sounds/Batch01/Pickups/GenericPickup");
					if (!milMo_LevelItem.Item.AutoPickup())
					{
						Animation component = milMo_Avatar.GameObject.GetComponent<Animation>();
						float timeOut = ((component[milMo_LevelItem.Item.PickupAnimation] != null) ? component[milMo_LevelItem.Item.PickupAnimation].length : 0f);
						MilMo_World.Instance.PlayerController.NewUnlockTime(timeOut);
						if (MilMo_Instance.PlayerInstance.IsNewPlayer())
						{
							MilMoAnalyticsHandler.ItemPickup(milMo_LevelItem.Template.Identifier);
						}
					}
					if (milMo_LevelItem.Item is MilMo_PowerUp)
					{
						MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_PowerUp", milMo_LevelItem.Item.Template.Name);
					}
				}
			}
			milMo_LevelItem.PickedUp(delegate(MilMo_LevelItem i)
			{
				_items.Remove(i);
			});
		}
		if (!_removedItems.ContainsKey(itemId))
		{
			_removedItems.Add(itemId, 0);
		}
	}

	private void ExplorationTokenFound(object msgAsObj)
	{
		if (!(msgAsObj is ServerExplorationTokenFound serverExplorationTokenFound))
		{
			return;
		}
		if (serverExplorationTokenFound.GetPlayerId() == MilMo_Instance.PlayerInstance.Avatar.Id)
		{
			string level = serverExplorationTokenFound.GetLevel();
			if (_explorationTokens.Count > serverExplorationTokenFound.GetTokenIndex())
			{
				_explorationTokens[serverExplorationTokenFound.GetTokenIndex()].SetAsFound();
			}
			PostAnalytics();
			MilMo_LevelInfo.UpdateExplorationTokens(_explorationTokens, level);
			GameEvent.LevelExplorationTokenFoundEvent.RaiseEvent(1);
			if (!AllExplorationTokensFound() || !AllExplorationTokensFoundListeners.ContainsKey(level))
			{
				return;
			}
			foreach (MilMo_AchievementObjectiveListener item in AllExplorationTokensFoundListeners[level])
			{
				item.Notify();
			}
			AllExplorationTokensFoundListeners.Remove(level);
			MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_FindAllExplorationTokens");
		}
		else if (_explorationTokens.Count > serverExplorationTokenFound.GetTokenIndex())
		{
			_explorationTokens[serverExplorationTokenFound.GetTokenIndex()].FoundByRemotePlayer();
		}
	}

	private void PostAnalytics()
	{
		string progress = _explorationTokens.Count((MilMo_ExplorationToken t) => t.IsFound) + "/" + _explorationTokens.Count;
		MilMoAnalyticsHandler.ExplorationTokenCollected(VerboseName, progress);
	}

	private void CoinTokenFound(object msgAsObj)
	{
		if (!(msgAsObj is ServerCoinTokenFound serverCoinTokenFound))
		{
			return;
		}
		if (serverCoinTokenFound.getPlayerId() == MilMo_Instance.PlayerInstance.Avatar.Id)
		{
			if (_coinTokens.Count > serverCoinTokenFound.getTokenIndex())
			{
				_coinTokens[serverCoinTokenFound.getTokenIndex()].SetAsFound();
			}
			string progress = _coinTokens.Count((MilMo_CoinToken t) => t.IsFound) + "/" + _coinTokens.Count;
			MilMoAnalyticsHandler.CoinCollected(VerboseName, progress);
			MilMo_LevelInfo.UpdateCoinTokens(_coinTokens, VerboseName);
			GameEvent.LevelCoinTokenFoundEvent.RaiseEvent(1);
			MilMo_LevelInfoData levelInfoData = MilMo_LevelInfo.GetLevelInfoData(VerboseName);
			if (levelInfoData.TotalNumberOfCoinTokens != 0 && levelInfoData.NumberOfCoinTokensFound == levelInfoData.TotalNumberOfCoinTokens)
			{
				MilMo_EventSystem.At(2f, delegate
				{
					DialogueSpawner.SpawnQuickInfo(new LocalizedStringWithArgument("World_8795"), new LocalizedStringWithArgument("World_8796"), 5, "IconSilverTokenFinished", "FanfareSound");
				});
			}
		}
		else if (_coinTokens.Count > serverCoinTokenFound.getTokenIndex())
		{
			_coinTokens[serverCoinTokenFound.getTokenIndex()].FoundByRemotePlayer();
		}
	}

	private void StaticGemFound(object msgAsObj)
	{
		if (msgAsObj is ServerStaticGemFound serverStaticGemFound && _staticGems.Count > serverStaticGemFound.getGemIndex())
		{
			_staticGems[serverStaticGemFound.getGemIndex()].SetAsFound();
		}
	}

	private void PremiumTokenFound(object msgAsObj)
	{
		if (msgAsObj is ServerPremiumTokenFound serverPremiumTokenFound && _premiumToken != null && !(serverPremiumTokenFound.getPlayerId() != MilMo_Instance.PlayerInstance.Avatar.Id))
		{
			_premiumToken.SetAsFound();
			GameEvent.ReceivedPremiumTokenRewardEvent.RaiseEvent(serverPremiumTokenFound.getValue());
			MilMo_LevelInfo.PremiumTokenFound(VerboseName);
		}
	}

	private void CreatePremiumToken(object tokenAsObj)
	{
		if (tokenAsObj is PremiumToken premiumToken)
		{
			if (_premiumToken != null)
			{
				_premiumToken.SetPosition(new Vector3(premiumToken.GetPosition().GetX(), premiumToken.GetPosition().GetY(), premiumToken.GetPosition().GetZ()));
			}
			else
			{
				_premiumToken = new MilMo_PremiumToken(premiumToken);
			}
		}
	}

	private bool AllExplorationTokensFound()
	{
		if (_explorationTokens != null && _explorationTokens.Count != 0)
		{
			return _explorationTokens.All((MilMo_ExplorationToken token) => token.IsFound);
		}
		return false;
	}

	public void GetExplorationTokenStatus(out int foundCount, out int totalCount)
	{
		totalCount = _explorationTokens.Count;
		foundCount = _explorationTokens.Count((MilMo_ExplorationToken token) => token.IsFound);
	}

	public bool IsPvp()
	{
		return MilMo_LevelInfo.IsPvp(VerboseName);
	}

	public bool IsStarterLevel()
	{
		return MilMo_LevelInfo.IsStarterLevel(VerboseName);
	}

	private void PlayerRespawned(object msgAsObj)
	{
		if (msgAsObj is ServerPlayerRespawn serverPlayerRespawn)
		{
			Vector3 position = new Vector3(serverPlayerRespawn.getPosition().GetX(), serverPlayerRespawn.getPosition().GetY(), serverPlayerRespawn.getPosition().GetZ());
			if (serverPlayerRespawn.getPlayerID() == MilMo_Instance.PlayerInstance.Avatar.Id)
			{
				MilMo_Instance.PlayerInstance.Respawn(serverPlayerRespawn.getHealth(), position, serverPlayerRespawn.getRotation(), serverPlayerRespawn.getIsRespawnInvulnerable() != 0);
			}
			else
			{
				RespawnRemotePlayer(serverPlayerRespawn, position);
			}
		}
	}

	private void RespawnRemotePlayer(ServerPlayerRespawn msg, Vector3 position)
	{
		string playerID = msg.getPlayerID();
		MilMo_RemotePlayer remotePlayer = GetRemotePlayer(playerID);
		if (remotePlayer != null && remotePlayer.Avatar != null)
		{
			float health = msg.getHealth();
			float rotation = msg.getRotation();
			sbyte isRespawnInvulnerable = msg.getIsRespawnInvulnerable();
			MilMo_Avatar avatar = remotePlayer.Avatar;
			avatar.UpdateHealth(health);
			avatar.UnstackAnimation("LandIdle", "Down01", unstackAll: true);
			avatar.PlayAnimation(avatar.IdleAnimation);
			remotePlayer.Teleport(position, Quaternion.AngleAxis(rotation, Vector3.up));
			if (isRespawnInvulnerable != 0)
			{
				avatar.SetInvulnerable(value: true);
			}
			avatar.DisableRagdoll();
		}
		else if (PlayersReceived.ContainsKey(playerID))
		{
			MilMo_EventSystem.At(0.5f, delegate
			{
				RespawnRemotePlayer(msg, position);
			});
		}
	}

	private void PlayerEndInvulnerable(object msgAsObj)
	{
		if (msgAsObj is ServerPlayerEndInvulnerability serverPlayerEndInvulnerability && !(serverPlayerEndInvulnerability.getPlayerId() == MilMo_Instance.PlayerInstance.Id))
		{
			MilMo_RemotePlayer value = GetRemotePlayer(serverPlayerEndInvulnerability.getPlayerId());
			if (value != null || PlayersReceived.TryGetValue(serverPlayerEndInvulnerability.getPlayerId(), out value))
			{
				value.SetInvulnerable(value: false);
			}
		}
	}

	private void PlayerChangeRoom(object msgAsObj)
	{
		if (msgAsObj is ServerPlayerChangeRoom serverPlayerChangeRoom)
		{
			string playerId = serverPlayerChangeRoom.getPlayerId();
			Vector3 position = new Vector3(serverPlayerChangeRoom.getPosition().GetX(), serverPlayerChangeRoom.getPosition().GetY(), serverPlayerChangeRoom.getPosition().GetZ());
			float rotation = serverPlayerChangeRoom.getRotation();
			string room = serverPlayerChangeRoom.getRoom();
			PlayerChangeRoom(playerId, room, position, rotation);
		}
	}

	public void PlayerChangeRoom(string id, string room, Vector3 position, float rotation)
	{
		if (id == MilMo_Instance.PlayerInstance.Avatar.Id)
		{
			LocalPlayerChangeRoom(room, position, rotation);
			return;
		}
		MilMo_RemotePlayer remotePlayer = GetRemotePlayer(id);
		if (remotePlayer?.Avatar != null)
		{
			if (!string.IsNullOrEmpty(remotePlayer.Avatar.Room))
			{
				MilMo_RoomPlopManager.Instance.PlayerLeftRoom(remotePlayer.Avatar, remotePlayer.Avatar.Room);
			}
			remotePlayer.Avatar.Room = room;
			if (!string.IsNullOrEmpty(remotePlayer.Avatar.Room))
			{
				MilMo_RoomPlopManager.Instance.PlayerEnteredRoom(remotePlayer.Avatar, remotePlayer.Avatar.Room);
			}
		}
	}

	private void LocalPlayerChangeRoom(string room, Vector3 position, float rotation)
	{
		float startChangeRoomTime = Time.time;
		if (MilMo_LoadingScreen.Instance.LoadingState != MilMo_LoadingScreen.State.LoadRoom && MilMo_LoadingScreen.Instance.LoadingState != MilMo_LoadingScreen.State.IntraTeleport)
		{
			if (string.IsNullOrEmpty(room) || room == MilMo_Instance.PlayerInstance.Avatar.Room)
			{
				bool fadeToWhite = string.IsNullOrEmpty(room) && !string.IsNullOrEmpty(MilMo_Instance.PlayerInstance.Avatar.Room);
				MilMo_LoadingScreen.Instance.IntraTeleportFade(0f, null, fadeToWhite);
			}
			else
			{
				MilMo_LoadingScreen.Instance.LoadRoomFade(0f, null);
			}
		}
		MilMo_LoadingScreen.Instance.CancelTimeout();
		bool oldRoomUnloaded = _roomProps.Count == 0;
		bool noPropsToLoad = string.IsNullOrEmpty(room);
		if (!oldRoomUnloaded)
		{
			MilMo_EventSystem.At(1f, delegate
			{
				UnloadRoom();
				oldRoomUnloaded = true;
				MilMo_EventSystem.Instance.PostEvent("oldRoomUnloaded", null);
				if (noPropsToLoad)
				{
					ReadyToLoadRoom(null, _data.Environment, null, room, position, rotation, startChangeRoomTime);
				}
			});
		}
		if (!noPropsToLoad)
		{
			int dataFilesLoaded = 0;
			int dataFilesToLoad = 3;
			string text = "Content/Worlds/" + WorldContentName + "/Levels/" + LevelContentName + "/" + room + "/";
			string filename = text + "Environment";
			string filename2 = text + "Props";
			string filename3 = text + "Area";
			MilMo_SFFile props = null;
			MilMo_SFFile environment = null;
			MilMo_SFFile area = null;
			MilMo_EventSystem.Listen("roomDataDone", delegate
			{
				if (oldRoomUnloaded)
				{
					ReadyToLoadRoom(props, environment, area, room, position, rotation, startChangeRoomTime);
				}
				else
				{
					MilMo_EventSystem.Listen("oldRoomUnloaded", delegate
					{
						ReadyToLoadRoom(props, environment, area, room, position, rotation, startChangeRoomTime);
					});
				}
			});
			MilMo_SimpleFormat.AsyncLoad(filename2, "Room", delegate(MilMo_SFFile file)
			{
				props = file;
				dataFilesLoaded++;
				if (dataFilesLoaded == dataFilesToLoad)
				{
					MilMo_EventSystem.Instance.PostEvent("roomDataDone", null);
				}
			});
			MilMo_SimpleFormat.AsyncLoad(filename, "Room", delegate(MilMo_SFFile file)
			{
				environment = file;
				dataFilesLoaded++;
				if (dataFilesLoaded == dataFilesToLoad)
				{
					MilMo_EventSystem.Instance.PostEvent("roomDataDone", null);
				}
			});
			MilMo_SimpleFormat.AsyncLoad(filename3, "Room", delegate(MilMo_SFFile file)
			{
				area = file;
				dataFilesLoaded++;
				if (dataFilesLoaded == dataFilesToLoad)
				{
					MilMo_EventSystem.Instance.PostEvent("roomDataDone", null);
				}
			});
		}
		if (oldRoomUnloaded && noPropsToLoad)
		{
			ReadyToLoadRoom(null, _data.Environment, null, room, position, rotation, startChangeRoomTime);
		}
	}

	private void ReadyToLoadRoom(MilMo_SFFile propsFile, MilMo_SFFile environmentFile, MilMo_SFFile areaFile, string room, Vector3 position, float rotation, float startChangeRoomTime)
	{
		if (environmentFile != null)
		{
			MilMo_EventSystem.At(Math.Max(0f, 1f - (Time.time - startChangeRoomTime)), delegate
			{
				Environment.UnloadEnvironment();
				Environment.Load(this, async: true, environmentFile);
			});
		}
		string camera = "Game";
		bool singlePlayer = false;
		if (areaFile != null)
		{
			camera = "Social";
			while (areaFile.NextRow())
			{
				if (areaFile.IsNext("MembersOnly"))
				{
					continue;
				}
				if (areaFile.IsNext("Camera"))
				{
					if (areaFile.IsNext("Social"))
					{
						camera = "Social";
						continue;
					}
					if (areaFile.IsNext("Game"))
					{
						camera = "Game";
						continue;
					}
					Debug.LogWarning("Invalid camera setting " + areaFile.GetString() + " in room area " + areaFile.Path + ". Will fall back to default 'Social'. (Valid cameras are 'Game' and 'Social'.)");
				}
				else if (areaFile.IsNext("Position"))
				{
					areaFile.GetVector3();
				}
				else if (areaFile.IsNext("SinglePlayer"))
				{
					singlePlayer = true;
				}
			}
		}
		MilMo_EventSystem.At(Math.Max(0f, 1f - (Time.time - startChangeRoomTime)), delegate
		{
			MilMo_World.Instance.Camera.CameraEvent(camera);
			MilMo_World.Instance.Camera.ValidatePosition();
		});
		if (propsFile == null)
		{
			LocalPlayerEnterRoom(room, position, rotation, singlePlayer, startChangeRoomTime);
			return;
		}
		LoadIndoorProps(propsFile, async: true, delegate(bool success)
		{
			if (!success)
			{
				Debug.LogWarning("Failed to load props from file " + propsFile.Path);
			}
			LocalPlayerEnterRoom(room, position, rotation, singlePlayer, startChangeRoomTime);
		});
	}

	private void LocalPlayerEnterRoom(string room, Vector3 position, float rotation, bool roomIsSinglePlayerArea, float startChangeRoomTime)
	{
		MilMo_EventSystem.At(Math.Max(0f, 2f - (Time.time - startChangeRoomTime)), delegate
		{
			if (MilMo_World.Instance.PlayerController != null)
			{
				MilMo_PlayerControllerBase.Teleport(position, Quaternion.Euler(0f, rotation, 0f));
			}
			else
			{
				MilMo_Instance.PlayerInstance.Avatar.GameObject.transform.position = position;
				MilMo_Instance.PlayerInstance.Avatar.GameObject.transform.rotation = Quaternion.Euler(0f, rotation, 0f);
			}
			if (MilMo_World.Instance.Camera != null)
			{
				MilMo_World.Instance.Camera.SetupPosition();
			}
			if (!string.IsNullOrEmpty(MilMo_Instance.PlayerInstance.Avatar.Room))
			{
				MilMo_RoomPlopManager.Instance.PlayerLeftRoom(MilMo_Instance.PlayerInstance.Avatar, MilMo_Instance.PlayerInstance.Avatar.Room);
			}
			MilMo_Instance.PlayerInstance.Avatar.Room = room;
			MilMo_Instance.PlayerInstance.InSinglePlayerArea = roomIsSinglePlayerArea;
			if (!string.IsNullOrEmpty(MilMo_Instance.PlayerInstance.Avatar.Room))
			{
				MilMo_RoomPlopManager.Instance.PlayerEnteredRoom(MilMo_Instance.PlayerInstance.Avatar, MilMo_Instance.PlayerInstance.Avatar.Room);
			}
			MilMo_UserInterface.CurrentRoom = room;
			MilMo_LoadingScreen.Instance.Hide();
		});
	}

	public void SetOutdoorState()
	{
		MilMo_Instance.PlayerInstance.Avatar.Room = "";
		MilMo_Instance.PlayerInstance.InSinglePlayerArea = false;
		MilMo_UserInterface.CurrentRoom = "";
	}

	private void LevelObjectCreatureSpawn(object msgAsObj)
	{
		if (!(msgAsObj is ServerLevelObjectCreatureSpawn serverLevelObjectCreatureSpawn))
		{
			return;
		}
		int id = serverLevelObjectCreatureSpawn.getCreature();
		MilMo_MovableObject milMo_MovableObject = null;
		foreach (MilMo_MovableObject item in _creatures.Where((MilMo_MovableObject m) => m.Id == id))
		{
			milMo_MovableObject = item;
		}
		if (milMo_MovableObject != null)
		{
			milMo_MovableObject.ReadLootObject(serverLevelObjectCreatureSpawn.getItem());
		}
		else
		{
			AddLevelItem(serverLevelObjectCreatureSpawn.getItem(), useSpawnEffect: false);
		}
	}

	private void CreateCreatureProjectile(object msgAsObj)
	{
		if (msgAsObj is ServerProjectileSpawned serverProjectileSpawned)
		{
			new MilMo_LevelProjectile(serverProjectileSpawned.getFullLevelName()).ReadFromNetworkMessage(serverProjectileSpawned, AddLevelProjectile);
		}
	}

	private static void PickupFail(object o)
	{
		MilMo_World.Instance.PlayerController.Unlock();
	}

	private void ActivateGameplayTrigger(object msgAsObj)
	{
		if (!(msgAsObj is ServerActivateGameplayTrigger serverActivateGameplayTrigger))
		{
			return;
		}
		string playerId = serverActivateGameplayTrigger.getPlayerId();
		int objectId = serverActivateGameplayTrigger.getGameplayObjectId();
		int triggerId = serverActivateGameplayTrigger.getTriggerId();
		using IEnumerator<MilMo_GameplayObject> enumerator = _gameplayObjects.Where((MilMo_GameplayObject obj) => obj.Id == objectId).GetEnumerator();
		if (!enumerator.MoveNext())
		{
			return;
		}
		MilMo_GameplayObject current = enumerator.Current;
		if (playerId != MilMo_Instance.PlayerInstance.Avatar.Id)
		{
			MilMo_RemotePlayer remotePlayer = GetRemotePlayer(playerId);
			if (remotePlayer != null)
			{
				current.ActivateTrigger(remotePlayer, triggerId);
			}
		}
		else
		{
			current.ActivateTrigger(MilMo_Instance.PlayerInstance, triggerId);
		}
	}

	private void GameplayObjectStartMoving(object msgAsObj)
	{
		if (!(msgAsObj is ServerGameplayObjectStartMoving serverGameplayObjectStartMoving))
		{
			return;
		}
		int objectId = serverGameplayObjectStartMoving.getObjectId();
		using IEnumerator<MilMo_GameplayObject> enumerator = _gameplayObjects.Where((MilMo_GameplayObject obj) => obj.Id == objectId).GetEnumerator();
		if (enumerator.MoveNext())
		{
			enumerator.Current.MoverStartMoving(serverGameplayObjectStartMoving.getSpline());
		}
	}

	private void GameplayObjectChangeDirection(object msgAsObj)
	{
		if (!(msgAsObj is ServerGameplayObjectChangeDirection serverGameplayObjectChangeDirection))
		{
			return;
		}
		int objectId = serverGameplayObjectChangeDirection.getObjectId();
		using IEnumerator<MilMo_GameplayObject> enumerator = _gameplayObjects.Where((MilMo_GameplayObject obj) => obj.Id == objectId).GetEnumerator();
		if (enumerator.MoveNext())
		{
			enumerator.Current.MoverChangeDirection(serverGameplayObjectChangeDirection.getNewDirection());
		}
	}

	private void GameplayObjectSetPos(object msgAsObj)
	{
		ServerSetGameplayObjectPosition msg = msgAsObj as ServerSetGameplayObjectPosition;
		if (msg == null)
		{
			return;
		}
		using IEnumerator<MilMo_GameplayObject> enumerator = _gameplayObjects.Where((MilMo_GameplayObject obj) => obj.Id == msg.GameplayObjectId).GetEnumerator();
		if (enumerator.MoveNext())
		{
			enumerator.Current.UpdatePosition(msg.Position, msg.ResetScale);
		}
	}

	private void GameplayObjectShrink(object msgAsObj)
	{
		if (!(msgAsObj is ServerGameplayObjectShrink serverGameplayObjectShrink))
		{
			return;
		}
		int objectId = serverGameplayObjectShrink.getObjectId();
		using IEnumerator<MilMo_GameplayObject> enumerator = _gameplayObjects.Where((MilMo_GameplayObject obj) => obj.Id == objectId).GetEnumerator();
		if (enumerator.MoveNext())
		{
			enumerator.Current.SetTargetScale(serverGameplayObjectShrink);
		}
	}

	public bool PlayerHasCapturedObject(string gamePlayObjectIdentifier)
	{
		return _gameplayObjects.Any((MilMo_GameplayObject obj) => obj.GetTemplateIdentifier().Equals(gamePlayObjectIdentifier) && obj.IsCaptured && obj.PlayerIsCapturer);
	}

	public void AddPlayerProjectile(bool success, MilMo_LevelObject projectileLvlObj)
	{
		if (success && projectileLvlObj is MilMo_LevelProjectile milMo_LevelProjectile)
		{
			if (!IsCorrectLevel(milMo_LevelProjectile.FullLevelName))
			{
				Debug.Log("Got projectile from wrong level (" + milMo_LevelProjectile.FullLevelName + "!=" + VerboseName + ")");
				milMo_LevelProjectile.Unload();
			}
			else
			{
				_playerProjectiles.Add(milMo_LevelProjectile);
			}
		}
	}

	public void AddLevelProjectile(bool success, MilMo_LevelObject projectileLvlObj)
	{
		if (success && projectileLvlObj is MilMo_LevelProjectile milMo_LevelProjectile)
		{
			if (!IsCorrectLevel(milMo_LevelProjectile.FullLevelName))
			{
				Debug.Log("Got creature projectile from wrong level (" + milMo_LevelProjectile.FullLevelName + "!=" + VerboseName + ")");
				milMo_LevelProjectile.Unload();
			}
			else
			{
				_levelProjectiles.Add(milMo_LevelProjectile);
			}
		}
	}

	public void AddItem(MilMo_LevelItem item)
	{
		if (!IsCorrectLevel(item.FullLevelName))
		{
			Debug.Log("Got item from wrong level (" + item.FullLevelName + "!=" + VerboseName + ")");
			item.Unload();
		}
		else if (_items.Any((MilMo_LevelItem existingItem) => existingItem.Id == item.Id))
		{
			Debug.Log("A) Item already exists " + item.Item.Template.Identifier);
		}
		else if (_removedItems.ContainsKey(item.Id))
		{
			Debug.Log("A) Item in removed items list " + item.Item.Template.Identifier);
			item.Unload();
		}
		else
		{
			_items.Add(item);
		}
	}

	private void NpcChangeInteractionState(object msgAsObj)
	{
		ServerNPCChangedInteractionState msg = msgAsObj as ServerNPCChangedInteractionState;
		if (msg == null)
		{
			return;
		}
		using IEnumerator<MilMo_LevelNpc> enumerator = _npcs.Where((MilMo_LevelNpc npc) => npc.Id == msg.getNpcId()).GetEnumerator();
		if (enumerator.MoveNext())
		{
			MilMo_LevelNpc current = enumerator.Current;
			current.ChangeInteractionState((byte)msg.getNewState());
			if (MilMo_Instance.PlayerInstance.IsNewPlayer())
			{
				MilMoAnalyticsHandler.NpcInteractionState(current.TemplateIdentifier, msg.getNewState());
			}
		}
	}

	private static void VisualRepSpawned(object msgAsObject)
	{
		if (msgAsObject is ServerSpawnVisualRepAt serverSpawnVisualRepAt)
		{
			MilMo_VisualRepContainer.AsyncCreateVisualRep(position: new Vector3(serverSpawnVisualRepAt.getPosition().GetX(), serverSpawnVisualRepAt.getPosition().GetY(), serverSpawnVisualRepAt.getPosition().GetZ()), rotation: Quaternion.Euler(new Vector3(serverSpawnVisualRepAt.getRotation().GetX(), serverSpawnVisualRepAt.getRotation().GetY(), serverSpawnVisualRepAt.getRotation().GetZ())), fullPath: serverSpawnVisualRepAt.getVisualRep(), callback: MilMo_VisualRepContainer.AddForUpdate);
		}
	}

	public void LoadIndoorPropsLocal(MilMo_SFFile file)
	{
		LoadIndoorProps(file, async: false, null);
	}

	private void LoadIndoorProps(MilMo_SFFile file, bool async, LevelLoadPropsDone callback)
	{
		if (file == null)
		{
			Debug.LogWarning("No props file for level " + VerboseName);
			callback(success: false);
			return;
		}
		if (_roomPropLoadedReaction != null)
		{
			MilMo_EventSystem.RemoveReaction(_roomPropLoadedReaction);
			_roomPropLoadedReaction = null;
		}
		_queuedRoomPropCounter = 0;
		while (file.NextRow())
		{
			if (file.GetString() == "<PROP>")
			{
				LoadProp(file, async, room: true);
			}
		}
		if (!async)
		{
			return;
		}
		if (_queuedRoomPropCounter == 0)
		{
			callback(success: true);
			return;
		}
		_roomPropLoadedReaction = MilMo_EventSystem.Listen("RoomPropDone", delegate
		{
			if (_queuedRoomPropCounter == 0)
			{
				callback(success: true);
			}
		});
		_roomPropLoadedReaction.Repeating = true;
	}

	protected void UnloadNpCs()
	{
		foreach (MilMo_LevelNpc npc in _npcs)
		{
			npc.Unload();
		}
		_npcs.Clear();
	}

	protected void UnloadCreatures()
	{
		foreach (MilMo_MovableObject creature in _creatures)
		{
			creature.Unload();
		}
		_creatures.Clear();
		_removedCreatures.Clear();
	}

	protected void UnloadItems()
	{
		foreach (MilMo_LevelItem item in _items)
		{
			item.Unload();
		}
		_items.Clear();
		_removedItems.Clear();
	}

	protected void LoadProps(bool async, LevelLoadPropsDone callback)
	{
		Debug.Log("Loading props for level " + VerboseName);
		MilMo_SFFile props = _data.Props;
		if (props == null)
		{
			Debug.LogWarning("No props file for level " + VerboseName);
			callback(success: false);
			return;
		}
		if (_propLoadedReaction != null)
		{
			MilMo_EventSystem.RemoveReaction(_propLoadedReaction);
			_propLoadedReaction = null;
		}
		_queuedPropCounter = 0;
		while (props.NextRow())
		{
			if (props.GetString() == "<PROP>")
			{
				LoadProp(props, async, room: false);
			}
		}
		if (!async)
		{
			return;
		}
		if (_queuedPropCounter == 0)
		{
			callback(success: true);
			return;
		}
		_propLoadedReaction = MilMo_EventSystem.Listen("LevelPropDone", delegate
		{
			if (_queuedPropCounter == 0)
			{
				callback(success: true);
			}
		});
		_propLoadedReaction.Repeating = true;
	}

	private void LoadProp(MilMo_SFFile file, bool async, bool room)
	{
		MilMo_ChatRoomTemplate chatRoom = null;
		if (!file.NextRow())
		{
			return;
		}
		string propPath = file.GetString();
		if (!file.NextRow())
		{
			return;
		}
		Vector3 vector = file.GetVector3();
		if (!file.NextRow())
		{
			return;
		}
		Vector3 vector2 = file.GetVector3();
		if (!file.NextRow())
		{
			return;
		}
		Vector3 vector3 = new Vector3(1f, 1f, 1f);
		try
		{
			vector3 = file.GetVector3();
		}
		catch (Exception)
		{
			Debug.LogWarning("Prop '" + propPath + "' is missing scale! ");
		}
		if (!file.NextRow())
		{
			return;
		}
		if (file.IsNext("<CHATROOM>"))
		{
			chatRoom = new MilMo_ChatRoomTemplate(file);
			if (!file.NextRow())
			{
				return;
			}
		}
		while (!file.IsNext("</PROP>"))
		{
			if (file.IsNext("<VISUALREP>"))
			{
				if (room)
				{
					_queuedRoomPropCounter++;
				}
				else
				{
					_queuedPropCounter++;
				}
				if (async)
				{
					MilMo_ResourceManager.Priority priority = MilMo_ResourceManager.Priority.High;
					float sqrMagnitude = (base.EntryPoint - vector).sqrMagnitude;
					if (sqrMagnitude > 2500f)
					{
						priority = MilMo_ResourceManager.Priority.Low;
					}
					else if (sqrMagnitude > 400f)
					{
						priority = MilMo_ResourceManager.Priority.Medium;
					}
					MilMo_VisualRepContainer.AsyncCreateVisualRep(propPath, file, vector, Quaternion.Euler(vector2), vector3, room ? "Room" : "Level", materialPauseMode: true, priority, delegate(MilMo_VisualRep newVisualRep)
					{
						if (newVisualRep != null && newVisualRep.GameObject != null)
						{
							FinishReadingProp(propPath, newVisualRep, chatRoom, room ? _roomProps : Props);
						}
						if (room)
						{
							_queuedRoomPropCounter--;
						}
						else
						{
							_queuedPropCounter--;
						}
						MilMo_EventSystem.Instance.PostEvent(room ? "RoomPropDone" : "LevelPropDone", null);
						MilMo_LoadingScreen.Instance.PropLoaded();
					});
					while (!file.IsNext("</VISUALREP>"))
					{
						file.NextRow();
					}
				}
				else
				{
					MilMo_VisualRep milMo_VisualRep = MilMo_VisualRepContainer.CreateVisualRep(propPath, vector, Quaternion.Euler(vector2), file);
					if (room)
					{
						_queuedRoomPropCounter--;
					}
					else
					{
						_queuedPropCounter--;
					}
					if (milMo_VisualRep == null || milMo_VisualRep.GameObject == null)
					{
						Debug.Log("Failed to load visual rep " + propPath);
					}
					else
					{
						milMo_VisualRep.GameObject.transform.localScale = vector3;
						FinishReadingProp(propPath, milMo_VisualRep, chatRoom, room ? _roomProps : Props);
						MilMo_LoadingScreen.Instance.PropLoaded();
					}
				}
			}
			if (!file.NextRow())
			{
				break;
			}
		}
	}

	private void FinishReadingProp(string propPath, MilMo_VisualRep visualRep, MilMo_ChatRoomTemplate chatRoomTemplate, ICollection<MilMo_LevelProp> listToAddTo)
	{
		if (visualRep.GameObject == null)
		{
			Debug.LogWarning("Failed to load prop " + propPath + ". Visual rep is missing a game object.");
		}
		else if (visualRep.IsActiveForCurrentEvent())
		{
			MilMo_LevelProp milMo_LevelProp = new MilMo_LevelProp(visualRep);
			milMo_LevelProp.GameObject.name = MilMo_Utility.RemoveCloneFromName(milMo_LevelProp.GameObject.name);
			listToAddTo.Add(milMo_LevelProp);
			if (chatRoomTemplate != null)
			{
				MilMo_ChatRoomManager.Instance.AddChatRoom(new MilMo_ChatRoom(chatRoomTemplate, milMo_LevelProp.GameObject));
			}
		}
	}

	protected void UnloadProps()
	{
		foreach (MilMo_LevelProp prop in Props)
		{
			prop.Destroy();
		}
		Props.Clear();
	}

	protected void UnloadRoom()
	{
		foreach (MilMo_LevelProp roomProp in _roomProps)
		{
			MilMo_ChatRoomManager.Instance.UnloadChatRoom(roomProp.GameObject, savePlayersInChatroom: true);
			roomProp.Destroy();
		}
		_roomProps.Clear();
	}

	protected void UnloadGameplayObjects()
	{
		foreach (MilMo_GameplayObject gameplayObject in _gameplayObjects)
		{
			gameplayObject.Unload();
		}
		_gameplayObjects.Clear();
	}

	private void LoadClimbingSurfaces()
	{
		MilMo_SFFile climbingSurfaces = _data.ClimbingSurfaces;
		if (climbingSurfaces == null)
		{
			return;
		}
		int num = 0;
		while (climbingSurfaces.NextRow())
		{
			if (climbingSurfaces.IsNext("Ladder"))
			{
				try
				{
					Vector3 vector = climbingSurfaces.GetVector3();
					Vector3 vector2 = climbingSurfaces.GetVector3();
					Vector3 vector3 = climbingSurfaces.GetVector3();
					Vector3 vector4 = climbingSurfaces.GetVector3();
					bool allowJump = !climbingSurfaces.HasMoreTokens() || climbingSurfaces.GetBool();
					_climbingSurfaces.Add(num, new MilMo_Ladder(num, vector, vector2, vector3, vector4, allowJump));
					num++;
				}
				catch (Exception ex)
				{
					Debug.LogWarning("Failed to load ladder for level " + VerboseName + " at line " + climbingSurfaces.GetLineNumber() + " in ClimbingSurfaces.txt.");
					Debug.LogWarning(ex.Message);
				}
			}
		}
	}

	protected void UnloadClimbingSurfaces()
	{
		_climbingSurfaces.Clear();
	}

	private void BeginPreStream()
	{
		MilMo_SFFile preStream = _data.PreStream;
		if (preStream == null)
		{
			return;
		}
		while (preStream.NextRow())
		{
			string @string = preStream.GetString();
			if (!(@string == "Load"))
			{
				if (!(@string == "Stream"))
				{
					Debug.LogWarning("Got pre stream request without Load or Stream keyword. File is " + preStream.Path + ".");
				}
			}
			else
			{
				MilMo_ResourceManager.Instance.PreloadAsset(preStream.GetString(), "Level");
			}
		}
	}

	private void LoadMusic()
	{
		MilMo_SFFile music = _data.Music;
		if (music != null)
		{
			while (music.NextRow())
			{
				if (music.IsNext("DefaultMusic"))
				{
					string @string = music.GetString();
					if (music.CheckEventTags())
					{
						_defaultMusic.Add(@string);
						Debug.Log("Default music set: " + @string);
					}
				}
			}
		}
		if (_defaultMusic.Count == 0)
		{
			_defaultMusic.Add("Batch01/Music/Adventure01");
		}
		music = _data.MusicAreas;
		if (music != null)
		{
			while (music.NextRow())
			{
				MilMo_MusicArea milMo_MusicArea = new MilMo_MusicArea();
				if (milMo_MusicArea.Load(music))
				{
					_musicAreas.Add(milMo_MusicArea);
				}
			}
		}
		PlayMusic = false;
	}

	private void UnloadMusic()
	{
		MilMo_Music.Instance.UnloadAllByTag("Level");
		_musicAreas.Clear();
		_defaultMusic.Clear();
		RemovePlayNextTrackEvent();
	}

	private void LoadTutorialAreas()
	{
		if (Singleton<MilMo_TutorialManager>.Instance != null)
		{
			Singleton<MilMo_TutorialManager>.Instance.LoadAreas(_data.TutorialAreas, World, Name);
		}
	}

	private void UnloadTutorialAreas()
	{
		if (Singleton<MilMo_TutorialManager>.Instance != null)
		{
			Singleton<MilMo_TutorialManager>.Instance.ClearAreas();
		}
	}

	protected void UnloadLevelData()
	{
		_data?.Unload();
	}

	private bool InsideMusicArea(Vector3 position)
	{
		using (IEnumerator<MilMo_MusicArea> enumerator = _musicAreas.Where((MilMo_MusicArea musicArea) => musicArea.Inside(position)).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				MilMo_MusicArea current = enumerator.Current;
				RemovePlayNextTrackEvent();
				_currentMusicArea = current;
				_currentMusicArea.Play();
				return true;
			}
		}
		return false;
	}

	public override void StartPlayMusic()
	{
		PlayMusic = true;
		if (!InsideMusicArea(MilMo_Instance.PlayerInstance.Avatar.Position))
		{
			if (_currentDefaultMusicIndex >= _defaultMusic.Count)
			{
				_currentDefaultMusicIndex = 0;
			}
			MilMo_Music.Instance.FadeIn(_defaultMusic[_currentDefaultMusicIndex], delegate(float clipLength)
			{
				RemovePlayNextTrackEvent();
				_playNextTrackEvent = MilMo_EventSystem.At(clipLength - 2f, PlayNextTrack);
			});
			_currentMusicArea = null;
		}
	}

	private void PlayNextTrack()
	{
		_currentDefaultMusicIndex++;
		if (_currentDefaultMusicIndex >= _defaultMusic.Count)
		{
			_currentDefaultMusicIndex = 0;
		}
		MilMo_Music.Instance.FadeIn(_defaultMusic[_currentDefaultMusicIndex], delegate(float clipLength)
		{
			RemovePlayNextTrackEvent();
			_playNextTrackEvent = MilMo_EventSystem.At(clipLength - 2f, PlayNextTrack);
		});
		_currentMusicArea = null;
	}

	private void RemovePlayNextTrackEvent()
	{
		if (_playNextTrackEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_playNextTrackEvent);
			_playNextTrackEvent = null;
		}
	}

	public void StartAmbienceSounds()
	{
		AmbientSoundManager?.Start();
	}

	private void ReadNPCMessage(object o)
	{
		if (!(o is ServerNPCMessage serverNPCMessage))
		{
			Debug.LogWarning("Got null or wrong type from npc_message.");
			return;
		}
		NpcMessageTypes theType = (NpcMessageTypes)serverNPCMessage.GetTheType();
		IList<NpcMessagePart> parts = serverNPCMessage.GetParts();
		for (int i = 0; i < parts.Count; i++)
		{
			if (parts[i].GetIsSayMessage() != 1)
			{
				Singleton<ThinkManager>.Instance.PlayerThinksFromServer(parts[i]);
				break;
			}
			List<LocalizedStringWithArgument> list = new List<LocalizedStringWithArgument>();
			foreach (string line in parts[i].GetLines())
			{
				list.Add(new LocalizedStringWithArgument(line));
			}
			if (CurrentLevel == null)
			{
				Debug.LogWarning("Trying to read ServerNPCMessage while not in a level.");
				break;
			}
			int npcID = serverNPCMessage.GetNpcID();
			MilMo_LevelNpc npc = CurrentLevel.GetNpc(npcID);
			if (npc == null)
			{
				Debug.LogWarning(string.Format("Trying to load unknown NPC ({0}) in level {1} while reading {2}.", npcID, CurrentLevel.VerboseName, "ServerNPCMessage"));
				break;
			}
			if (theType == NpcMessageTypes.OfferLevelChange)
			{
				DialogueSpawner.SpawnNPCTravelDialogue(new NPCTravelMessageData(theType, npcID, npc.Name?.String, npc.GetPortraitKey(), serverNPCMessage.GetVoice(), list, serverNPCMessage.GetLevelOffers()));
			}
			else
			{
				DialogueSpawner.SpawnNPCMessageDialogue(new NPCMessageData(theType, npcID, npc.Name?.String, npc.GetPortraitKey(), serverNPCMessage.GetVoice(), list));
			}
		}
	}

	protected void UnloadExplorationTokens()
	{
		foreach (MilMo_ExplorationToken explorationToken in _explorationTokens)
		{
			explorationToken.Unload();
		}
		_explorationTokens.Clear();
	}

	protected void UnloadCoinTokens()
	{
		foreach (MilMo_CoinToken coinToken in _coinTokens)
		{
			coinToken.Unload();
		}
		_coinTokens.Clear();
	}

	protected void UnloadStaticGems()
	{
		foreach (MilMo_StaticGem staticGem in _staticGems)
		{
			staticGem.Unload();
		}
		_staticGems.Clear();
	}

	protected void UnloadPremiumToken()
	{
		if (_premiumToken != null)
		{
			_premiumToken.Unload();
			_premiumToken = null;
		}
	}

	public override string GetMaterialAtPosition(Vector3 pos, float rayCastYOffset, out bool terrain)
	{
		terrain = true;
		if (MilMo_WaterManager.GetWaterLevel(pos, out var _) != 0)
		{
			return "Water";
		}
		if (GroundMaterialManager == null)
		{
			return "";
		}
		if (MilMo_Physics.GetDistanceToGround(pos) > 0.2f)
		{
			return "";
		}
		Vector3 pos2 = pos;
		pos2.y += rayCastYOffset;
		return GroundMaterialManager.GetMaterialAtPosition(pos2, out terrain);
	}

	public static void AddAllExplorationTokensFoundListener(MilMo_AchievementObjectiveListener listener)
	{
		if (!AllExplorationTokensFoundListeners.ContainsKey(listener.Object))
		{
			AllExplorationTokensFoundListeners.Add(listener.Object, new List<MilMo_AchievementObjectiveListener>());
		}
		AllExplorationTokensFoundListeners[listener.Object].Add(listener);
	}

	public static void RemoveAllExplorationTokensFoundListener(MilMo_AchievementObjectiveListener listener)
	{
		if (AllExplorationTokensFoundListeners.ContainsKey(listener.Object))
		{
			if (AllExplorationTokensFoundListeners[listener.Object].Count == 1)
			{
				AllExplorationTokensFoundListeners.Remove(listener.Object);
			}
			else
			{
				AllExplorationTokensFoundListeners[listener.Object].Remove(listener);
			}
		}
	}

	public static void ClearAllExplorationTokensFoundListeners()
	{
		AllExplorationTokensFoundListeners.Clear();
	}

	private static void Debug_CapsuleVolumeSync(object msgAsObj)
	{
		if (msgAsObj is DebugServerCapsuleVolumeSync debugServerCapsuleVolumeSync)
		{
			Vector3 position = new Vector3(debugServerCapsuleVolumeSync.getPosition().GetX(), debugServerCapsuleVolumeSync.getPosition().GetY(), debugServerCapsuleVolumeSync.getPosition().GetZ());
			Vector3 toDirection = new Vector3(debugServerCapsuleVolumeSync.getDirection().GetX(), debugServerCapsuleVolumeSync.getDirection().GetY(), debugServerCapsuleVolumeSync.getDirection().GetZ());
			float length = debugServerCapsuleVolumeSync.getLength();
			float radius = debugServerCapsuleVolumeSync.getRadius();
			GameObject gameObject = new GameObject("Capsule");
			gameObject.transform.position = position;
			gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, toDirection) * gameObject.transform.rotation;
			GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = new Vector3(0f, 0.5f * length, 0f);
			gameObject2.transform.localScale = new Vector3(2f * radius, 0.5f * length, 2f * radius);
			gameObject2.transform.localEulerAngles = Vector3.zero;
			GameObject gameObject3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			gameObject3.transform.parent = gameObject.transform;
			gameObject3.transform.localPosition = Vector3.zero;
			gameObject3.transform.localScale = new Vector3(2f * radius, 2f * radius, 2f * radius);
			gameObject3.transform.localEulerAngles = Vector3.zero;
			GameObject gameObject4 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			gameObject4.transform.parent = gameObject.transform;
			gameObject4.transform.localPosition = new Vector3(0f, length, 0f);
			gameObject4.transform.localScale = new Vector3(2f * radius, 2f * radius, 2f * radius);
			gameObject4.transform.localEulerAngles = Vector3.zero;
			GameObject creature = GameObject.CreatePrimitive(PrimitiveType.Cube);
			creature.name = "AttackingCreature";
			creature.transform.position = new Vector3(debugServerCapsuleVolumeSync.getCreaturePosition().GetX(), debugServerCapsuleVolumeSync.getCreaturePosition().GetY(), debugServerCapsuleVolumeSync.getCreaturePosition().GetZ());
			GetWalkableHeight(creature.transform.position, out var normal);
			Quaternion quaternion = Quaternion.LookRotation(new Vector3(toDirection.x, 0f, toDirection.z));
			quaternion = Quaternion.FromToRotation(Vector3.up, normal) * quaternion;
			creature.transform.rotation = quaternion;
			gameObject.transform.parent = creature.transform;
			MilMo_EventSystem.At(10f, delegate
			{
				UnityEngine.Object.Destroy(creature);
			});
		}
	}
}
