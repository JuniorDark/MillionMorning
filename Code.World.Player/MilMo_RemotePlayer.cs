using System.Collections.Generic;
using Code.Core.Avatar;
using Code.Core.Collision;
using Code.Core.Emote;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.Items;
using Code.Core.Network.messages.server;
using Code.Core.Network.nexus;
using Code.Core.Network.types;
using Code.Core.ObjectEffectSystem;
using Code.Core.PlayerState;
using Code.Core.Template;
using Code.Core.Utility;
using Code.Core.Visual.Water;
using Code.World.Climbing;
using Code.World.Gameplay;
using Code.World.Home;
using Code.World.Level;
using Code.World.Level.LevelObject;
using Core;
using Player.Moods;
using UnityEngine;

namespace Code.World.Player;

public class MilMo_RemotePlayer : IPlayer, IIdentity
{
	private enum AnimationState
	{
		Idle,
		Walk,
		Run,
		Strafe,
		Jump,
		Hover,
		Land,
		Climb,
		Swim,
		Fly,
		Bash,
		Knockback
	}

	public delegate void RemotePlayerDone(bool success);

	public const float TIMEOUT_LIMIT = 24f;

	public static bool DebugMovement = false;

	public static float RotationInterpolationSpeed = 6.5f;

	public static float MaxExtrapolationTime = 0.2f;

	public static float StandStillInterpolationSpeed = 5f;

	public static float StandStillInterpolationThreshold = 0.1f;

	public static float VelocityInterpolationSpeed = 0.4f;

	public static float DirectionInterpolationSpeed = 5f;

	public static float JumpVelocityThreshold = 0.1f;

	public static float TeleportSqrtDistanceLimit = 6400f;

	private const float STOP_WHEN_NO_UPDATES_SINCE_TIME = 3f;

	private const float MAX_VELOCITY = 10f;

	private static readonly string[] SpawnEffectTemplates = new string[3] { "SpawnPlayer", "CoinSpawnSound", "PlayerSpawnSound" };

	private bool _isInWater;

	private sbyte _knockBackState;

	private string _moveSlowAnimation = "Walk";

	private string _moveFastAnimation = "Run";

	private bool _isInvulnerable;

	private bool _isClimbing;

	private bool _playClimbingAnimation;

	private MilMo_ClimbingSurface _climbingSurface;

	public const int NO_GAMEPLAY_OBJECT = -1;

	private MilMo_GameplayObject _onGameplayObject;

	private int _onGameplayObjectId = -1;

	private readonly Vector3 _onGameplayObjectOffset;

	private Vector3 _onCurrentGameplayObjectOffset;

	private bool _onCurrentGameplayObjectOffsetIsSet;

	private readonly bool _useSpawnEffects;

	private MilMo_PlayerSpline _spline;

	private float _splineTime;

	private MilMo_GameObjectSpline.SplinePoint _currentSplinePoint;

	private const float SPLINE_ROTATION_INTERPOLATION_SPEED = 10f;

	private AnimationState _animationState;

	private readonly List<MilMo_ObjectEffect> _spawnEffects = new List<MilMo_ObjectEffect>();

	private bool _isSpawning = true;

	private readonly List<MilMo_LevelProjectile> _projectiles = new List<MilMo_LevelProjectile>();

	private Vector3 _targetPosition = Vector3.zero;

	private Vector3 _sourcePosition = Vector3.zero;

	private Vector3 _sourceVelocity = Vector3.zero;

	private float _timeToTargetPosition;

	private Quaternion _targetRotation = Quaternion.identity;

	private Vector3 _targetVelocity = Vector3.zero;

	private bool _standingStill = true;

	private bool _isExtrapolating;

	private float _startExtrapolateTime;

	private Vector3 _velocity = Vector3.zero;

	private float _previousVelocityY;

	private MilMo_GenericReaction _positionUpdateReaction;

	private ServerUpdatePosition _lastPositionUpdateMessage;

	public bool InSpline => _spline != null;

	public string Id { get; }

	public MilMo_Avatar Avatar { get; private set; }

	public bool IsLocalPlayer => false;

	public bool InShop { get; set; }

	public bool InWorldMap { get; set; }

	public bool InCharBuilder { get; set; }

	public float TimeSinceLastUpdate { get; set; }

	private bool HasValidAvatar
	{
		get
		{
			MilMo_Avatar avatar = Avatar;
			if (avatar != null && !avatar.IsDestroyed)
			{
				return Avatar.GameObject != null;
			}
			return false;
		}
	}

	public bool Invulnerable => _isInvulnerable;

	public int UserIdentifier => int.Parse(Id);

	public string Name => Avatar.Name;

	public void SetInvulnerable(bool value)
	{
		_isInvulnerable = value;
		Avatar?.SetInvulnerable(value);
	}

	public MilMo_RemotePlayer(string id, int membershipDaysLeft, sbyte role, string title, bool useSpawnEffects, int avatarLevel)
	{
		_useSpawnEffects = useSpawnEffects;
		Id = id;
		Avatar = new MilMo_Avatar(this, id, isLocalAvatar: false, membershipDaysLeft, role, title, avatarLevel);
		Avatar.InitDamageNumberEffect();
		float x = MilMo_Utility.RandomFloat(-0.24f, 0.24f);
		float z = MilMo_Utility.RandomFloat(-0.24f, 0.24f);
		_onGameplayObjectOffset = new Vector3(x, 0f, z);
		_positionUpdateReaction = MilMo_EventSystem.RegisterFixedUpdate(UpdatePositionFromLastMessage);
	}

	public void Read(RemotePlayer remotePlayer, RemotePlayerDone callback)
	{
		Debug.Log("Reading remote player " + remotePlayer.GetAvatar().GetName());
		Vector3 position = new Vector3(remotePlayer.GetPosition().GetX(), remotePlayer.GetPosition().GetY(), remotePlayer.GetPosition().GetZ());
		float rotation = remotePlayer.GetRotation();
		_isClimbing = remotePlayer.GetIsClimbing() == 1;
		_climbingSurface = null;
		if (MilMo_Level.CurrentLevel != null)
		{
			_climbingSurface = MilMo_Level.CurrentLevel.GetClimbingSurface(remotePlayer.GetClimbingSurface());
		}
		if (_isClimbing && _climbingSurface == null)
		{
			Debug.LogWarning("Remote player is climbing but climbing surface id is invalid (" + remotePlayer.GetClimbingSurface() + ")");
		}
		InShop = remotePlayer.GetIsInShop() != 0;
		if (remotePlayer.GetOnGameplayObject() == -1)
		{
			LeaveGameplayObject();
		}
		else
		{
			EnterGameplayObject(remotePlayer.GetOnGameplayObject());
		}
		Avatar.SetInitializedCallback(delegate(MilMo_Avatar avatar, string userTag)
		{
			if (avatar == null)
			{
				Debug.LogWarning("Failed to initialize avatar for remote player");
				callback(success: false);
			}
			else
			{
				Debug.Log("Avatar " + avatar.Name + " initialized for remote player");
				if (remotePlayer.GetWieldedItem() != null)
				{
					Avatar.WieldFromNetworkMessage(remotePlayer.GetWieldedItem());
				}
				Avatar.Room = remotePlayer.GetRoom();
				UpdateWithStopPosition(position, rotation);
				Avatar.GameObject.transform.position = position;
				Avatar.GameObject.transform.rotation = Quaternion.Euler(0f, rotation, 0f);
				if (_useSpawnEffects)
				{
					if (MilMo_Home.CurrentHome == null)
					{
						string[] spawnEffectTemplates = SpawnEffectTemplates;
						foreach (string effectName in spawnEffectTemplates)
						{
							MilMo_ObjectEffect objectEffect = MilMo_ObjectEffectSystem.GetObjectEffect(Avatar.GameObject, effectName);
							if (objectEffect != null)
							{
								_spawnEffects.Add(objectEffect);
							}
						}
					}
					else
					{
						MilMo_Home.CurrentHome.PlayRemotePlayerJoinSound();
					}
				}
				CheckWater();
				SetAfk(remotePlayer.IsAfk());
				Avatar.PlayAnimation(Avatar.IdleAnimation);
				if (_isClimbing)
				{
					StartClimbing(_climbingSurface);
				}
				Avatar.EmoteManager.RegisterWieldCallbacks(EquipWieldItemBodypack, UnwieldVisually);
				Avatar.SetInvulnerable(_isInvulnerable);
				callback(success: true);
			}
		});
		Avatar.Read(this, remotePlayer.GetAvatar());
		MilMo_ProfileManager.AddAPlayerToNameList(Avatar.Name, Id);
	}

	private void SetAfk(bool afk)
	{
		MilMo_MoodHandler milMo_MoodHandler = Avatar?.MoodHandler;
		TemplateReference reference = new TemplateReference("PlayerState", "AFK");
		if (afk)
		{
			Mood mood = milMo_MoodHandler?.GetMoodByKey("Sad");
			if ((bool)mood)
			{
				milMo_MoodHandler?.SetMood(mood, send: false, persist: false);
			}
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(reference, ActivateState);
		}
		else
		{
			milMo_MoodHandler?.RestorePersistedMood();
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(reference, DeactivateState);
		}
	}

	public void RecreateAvatar(MilMo_Player.AvatarCreatedCallback callback, Code.Core.Network.types.Avatar avatar)
	{
		Avatar.SetInitializedCallback(delegate(MilMo_Avatar newAvatar, string userTag)
		{
			callback?.Invoke(newAvatar != null);
		});
		Avatar.Recreate(avatar);
	}

	public void Destroy()
	{
		Avatar?.Destroy();
		if (_positionUpdateReaction != null)
		{
			MilMo_EventSystem.UnregisterFixedUpdate(_positionUpdateReaction);
			_positionUpdateReaction = null;
		}
	}

	public void Update()
	{
		TimeSinceLastUpdate += Time.deltaTime;
		if (Avatar == null || Avatar.IsDestroyed || Avatar.GameObject == null)
		{
			return;
		}
		if (_isSpawning)
		{
			HandleSpawnEffects();
		}
		if (Avatar.IsRagdollActive)
		{
			Avatar.Update();
			return;
		}
		if (Avatar.IsSitting)
		{
			HandleSitting();
			return;
		}
		if (_spline != null)
		{
			HandleSplineRide();
			Avatar.Update();
			return;
		}
		CheckWater();
		Avatar.Update();
		Vector3 position = Avatar.GameObject.transform.position;
		if (_onGameplayObjectId != -1)
		{
			if (_onGameplayObject == null && MilMo_Level.CurrentLevel != null)
			{
				_onGameplayObject = MilMo_Level.CurrentLevel.GetGameplayObject(_onGameplayObjectId);
			}
			if (_onGameplayObject != null && HandleOnGameplayObject())
			{
				return;
			}
		}
		_playClimbingAnimation = false;
		if (_isClimbing && _climbingSurface != null)
		{
			Vector3 position2 = Avatar.GameObject.transform.position;
			if (MilMo_Utility.Equals(position2, _targetPosition, 0.01f))
			{
				Avatar.GameObject.transform.position = _targetPosition;
			}
			else
			{
				Vector3 vector = _targetPosition - position2;
				float sqrMagnitude = vector.sqrMagnitude;
				vector.Normalize();
				float num = Time.deltaTime * 2f;
				Avatar.GameObject.transform.position = position2 + vector * num;
				Vector3 position3 = Avatar.GameObject.transform.position;
				if ((_targetPosition - position3).sqrMagnitude > sqrMagnitude)
				{
					Avatar.GameObject.transform.position = _targetPosition;
				}
			}
			_playClimbingAnimation = _climbingSurface.HandleRemotePlayer(this);
		}
		else
		{
			float magnitude = (_targetPosition - Avatar.GameObject.transform.position).magnitude;
			if (_standingStill)
			{
				if (magnitude > StandStillInterpolationThreshold)
				{
					Vector3 position4 = Avatar.GameObject.transform.position;
					Vector3 normalized = (_targetPosition - position4).normalized;
					position4 += StandStillInterpolationSpeed * Time.deltaTime * normalized;
					Avatar.GameObject.transform.position = position4;
					if ((_targetPosition - Avatar.GameObject.transform.position).magnitude > magnitude)
					{
						Avatar.GameObject.transform.position = _targetPosition;
					}
				}
				else
				{
					Avatar.GameObject.transform.position = _targetPosition;
				}
				Quaternion rotation = Quaternion.Lerp(Avatar.GameObject.transform.rotation, _targetRotation, Time.deltaTime * RotationInterpolationSpeed);
				Avatar.GameObject.transform.rotation = rotation;
			}
			else
			{
				_timeToTargetPosition -= Time.deltaTime;
				if (_timeToTargetPosition < 0f)
				{
					if (!_isExtrapolating)
					{
						_startExtrapolateTime = Time.realtimeSinceStartup;
					}
					_isExtrapolating = true;
					_timeToTargetPosition = 0f;
				}
				bool flag = false;
				if (_isExtrapolating)
				{
					if (Time.realtimeSinceStartup - _startExtrapolateTime < MaxExtrapolationTime)
					{
						flag = true;
						Avatar.GameObject.transform.position += _targetVelocity * Time.deltaTime;
					}
				}
				else
				{
					flag = true;
					float num2 = Mathf.Clamp01(1f - _timeToTargetPosition / MilMo_Player.UpdateToServerFrequency);
					if (MilMo_Global.CubicInterpolation)
					{
						float num3 = num2 * num2;
						float num4 = num3 * num2;
						Avatar.GameObject.transform.position = (2f * num4 - 3f * num3 + 1f) * _sourcePosition + (num4 - 2f * num3 + num2) * _sourceVelocity / 3f + (num4 - num3) * _targetVelocity / 3f + (-2f * num4 + 3f * num3) * _targetPosition;
					}
					else
					{
						Avatar.GameObject.transform.position = Vector3.Lerp(_sourcePosition, _targetPosition, num2);
					}
				}
				if (flag)
				{
					Vector3 position5 = Avatar.GameObject.transform.position;
					Vector3 vector2 = position5;
					vector2.y += 0.5f;
					float worldHeight = MilMo_Physics.GetWorldHeight(vector2);
					vector2.y = Mathf.Max(position5.y, worldHeight);
					position5 = vector2;
					Avatar.GameObject.transform.position = position5;
					Quaternion rotation2 = Quaternion.Lerp(Avatar.GameObject.transform.rotation, _targetRotation, Time.deltaTime * RotationInterpolationSpeed);
					Avatar.GameObject.transform.rotation = rotation2;
				}
			}
		}
		Vector3 position6 = Avatar.GameObject.transform.position;
		_velocity.x = (position6.x - position.x) / Time.deltaTime;
		_velocity.y = (position6.y - position.y) / Time.deltaTime;
		_velocity.z = (position6.z - position.z) / Time.deltaTime;
		_velocity.x = Mathf.Clamp(_velocity.x, -10f, 10f);
		_velocity.z = Mathf.Clamp(_velocity.z, -10f, 10f);
		HandleAnimations(position);
		_previousVelocityY = _velocity.y;
		if (DebugMovement)
		{
			Debug.DrawLine(Avatar.GameObject.transform.position, _targetPosition, Color.red);
		}
	}

	public void ProjectileHit(int projectileId, MilMo_LevelCreature critter)
	{
		for (int num = _projectiles.Count; num >= 0; num--)
		{
			if (_projectiles[num].Id == projectileId)
			{
				_projectiles[num].ImpactKill();
				critter?.DamageEffectLocal(_projectiles[num]);
			}
		}
	}

	public void LateUpdate()
	{
		if (Avatar != null && !Avatar.IsDestroyed)
		{
			Avatar.LateUpdate();
		}
	}

	public void FixedUpdate()
	{
		if (Avatar != null && !Avatar.IsDestroyed)
		{
			Avatar.FixedUpdate();
		}
	}

	public void StartSplineRide(MilMo_PlayerSpline spline)
	{
		if (Avatar != null && !Avatar.IsDestroyed)
		{
			_spline = spline;
			_splineTime = 0f;
			_currentSplinePoint = _spline.GetPointAtTime(0f);
			Avatar.PlayAnimation(_currentSplinePoint.Animation);
			Avatar.SuperAlivenessManager.Disable();
			UnwieldVisually();
		}
	}

	private void HandleAnimations(Vector3 lastPos)
	{
		if (Avatar == null || Avatar.IsDestroyed)
		{
			return;
		}
		if (_playClimbingAnimation)
		{
			Avatar.GameObject.GetComponent<Animation>()["GenericClimb01"].speed = ((_velocity.magnitude > 0.01f) ? 1.2f : 0f);
			return;
		}
		Vector3 vector = Avatar.GameObject.transform.position + MilMo_Avatar.DefaultBaseControllerCenter;
		Vector3 start = vector - new Vector3(0f, 0.65f, 0f);
		Vector3 end = vector + new Vector3(0f, 0.65f, 0f);
		if (DebugMovement)
		{
			Debug.DrawLine(start, end, Color.red);
		}
		bool flag = !Physics.CheckCapsule(start, end, 0.41f, -1);
		if (_knockBackState == 1)
		{
			if (_animationState != AnimationState.Knockback)
			{
				Avatar.PlayAnimation("Stance");
				_animationState = AnimationState.Knockback;
			}
			return;
		}
		if (!_isInWater && flag)
		{
			if (_velocity.y > 0.01f)
			{
				if (!Avatar.InHappyPickup && _animationState != AnimationState.Jump)
				{
					Avatar.PlayAnimation(Avatar.JumpAnimation);
					_animationState = AnimationState.Jump;
				}
			}
			else if (_velocity.y < -0.01f)
			{
				if (!Avatar.InHappyPickup && _animationState != AnimationState.Land)
				{
					Avatar.PlayAnimation(Avatar.FallAnimation);
					_animationState = AnimationState.Land;
				}
			}
			else if (!Avatar.InHappyPickup && _animationState != AnimationState.Hover)
			{
				Avatar.PlayAnimation(Avatar.HoverAnimation);
				_animationState = AnimationState.Hover;
			}
			return;
		}
		Vector3 position = Avatar.GameObject.transform.position;
		if (Vector3.Distance(new Vector3(lastPos.x, 0f, lastPos.z), new Vector3(position.x, 0f, position.z)) / Time.deltaTime >= 0.2f)
		{
			if (_animationState != AnimationState.Run)
			{
				_animationState = AnimationState.Run;
				Avatar.PlayAnimation(Avatar.GetAnimationName(_moveFastAnimation));
			}
		}
		else if (!Avatar.InHappyPickup && _animationState != 0 && _animationState != AnimationState.Bash)
		{
			_animationState = AnimationState.Idle;
			Avatar.PlayAnimation(Avatar.IdleAnimation);
		}
	}

	public void UpdatePositionMessageReceived(ServerUpdatePosition msg)
	{
		if (HasValidAvatar)
		{
			_lastPositionUpdateMessage = msg;
		}
	}

	private void UpdatePositionFromLastMessage(object obj)
	{
		if (_lastPositionUpdateMessage == null)
		{
			return;
		}
		float x = (float)_lastPositionUpdateMessage.getX() * 0.125f;
		float y = (float)_lastPositionUpdateMessage.getY() * (1f / 32f);
		float z = (float)_lastPositionUpdateMessage.getZ() * 0.125f;
		vector3 velocity = _lastPositionUpdateMessage.getVelocity();
		Vector3 vector = new Vector3(velocity.GetX(), velocity.GetY(), velocity.GetZ());
		_lastPositionUpdateMessage = null;
		Vector3 vector2 = new Vector3(x, y, z);
		if ((Avatar.Position - vector2).sqrMagnitude >= TeleportSqrtDistanceLimit)
		{
			Teleport(vector2, Avatar.GameObject.transform.rotation);
			return;
		}
		_standingStill = false;
		_sourcePosition = Avatar.Position;
		_sourceVelocity = _targetVelocity;
		_targetVelocity = vector;
		_timeToTargetPosition = MilMo_Player.UpdateToServerFrequency;
		_targetPosition = vector2;
		if (MilMo_Global.PositionExtrapolation)
		{
			_targetPosition += vector * MilMo_Player.UpdateToServerFrequency;
		}
		Vector3 normalized = vector.normalized;
		if (new Vector2(normalized.x, normalized.z).magnitude / MilMo_Player.UpdateToServerFrequency > JumpVelocityThreshold && _knockBackState == 0)
		{
			Vector3 forward = normalized;
			forward.y = 0f;
			forward.Normalize();
			_targetRotation = Quaternion.LookRotation(forward);
		}
		else
		{
			_targetRotation = Avatar.GameObject.transform.rotation;
		}
		_isExtrapolating = false;
		TimeSinceLastUpdate = 0f;
	}

	public void UpdateKnockBackState(sbyte knockBackState)
	{
		_knockBackState = knockBackState;
	}

	public void UpdateWithStopPosition(Vector3 position, float rotation)
	{
		_standingStill = true;
		if (HasValidAvatar && (Avatar.Position - position).sqrMagnitude >= TeleportSqrtDistanceLimit)
		{
			Teleport(position, Quaternion.Euler(0f, rotation, 0f));
			return;
		}
		_targetPosition = position;
		_targetRotation = Quaternion.Euler(0f, rotation, 0f);
		_isExtrapolating = false;
		_targetVelocity = Vector3.zero;
		TimeSinceLastUpdate = 0f;
	}

	public void EquipAll(IList<Item> items)
	{
		int numItemsDone = 0;
		List<MilMo_Wearable> wearables = new List<MilMo_Wearable>();
		foreach (Item item3 in items)
		{
			Item item1 = item3;
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(item3.GetTemplate(), delegate(MilMo_Template template, bool timeout)
			{
				numItemsDone++;
				if (timeout)
				{
					if (numItemsDone == items.Count)
					{
						FinishEquipAll(wearables);
					}
				}
				else if (!(template is MilMo_ItemTemplate milMo_ItemTemplate))
				{
					if (numItemsDone == items.Count)
					{
						FinishEquipAll(wearables);
					}
				}
				else
				{
					Dictionary<string, string> modifiers = MilMo_Item.ReadModifiers(item1.GetModifiers());
					MilMo_Wearable item2 = milMo_ItemTemplate.Instantiate(modifiers) as MilMo_Wearable;
					wearables.Add(item2);
					if (numItemsDone == items.Count)
					{
						FinishEquipAll(wearables);
					}
				}
			});
		}
	}

	private void FinishEquipAll(IList<MilMo_Wearable> wearables)
	{
		if (Avatar == null || Avatar.IsDestroyed)
		{
			return;
		}
		Avatar.UnequipAll();
		foreach (MilMo_Wearable wearable in wearables)
		{
			Avatar.EquipLocal(wearable);
		}
		Avatar.AsyncApply();
	}

	public void Wield(Item item)
	{
		if (Avatar != null && !Avatar.IsDestroyed)
		{
			Avatar.WieldFromNetworkMessage(item);
		}
	}

	public void Unwield()
	{
		if (Avatar != null && !Avatar.IsDestroyed && Avatar.WieldedItem != null)
		{
			Avatar.UnequipLocal(Avatar.WieldedItem);
			Avatar.AsyncApply();
		}
	}

	public void PlayAttackEffects()
	{
		PlayWieldItemAnimation();
	}

	private void PlayWieldItemAnimation()
	{
		if (Avatar != null && !Avatar.IsDestroyed && Avatar.WieldedItem != null)
		{
			string randomWieldAnimation = Avatar.WieldedItem.GetRandomWieldAnimation();
			if (!(randomWieldAnimation == "Idle"))
			{
				_animationState = AnimationState.Bash;
				Avatar.PlayAnimation(randomWieldAnimation);
			}
		}
	}

	public void Dig()
	{
		if (Avatar != null && !Avatar.IsDestroyed && Avatar.WieldedItem != null && Avatar.WieldedItem.Template.Identifier.Contains("Shovel"))
		{
			PlayWieldItemAnimation();
			Avatar.EmitPuff("Dig");
		}
	}

	public void Eat(sbyte useNumber)
	{
		if (Avatar == null || Avatar.IsDestroyed || !(Avatar.WieldedItem is MilMo_WieldableFood milMo_WieldableFood))
		{
			return;
		}
		string useEmote = milMo_WieldableFood.GetUseEmote(useNumber);
		if (!string.IsNullOrEmpty(useEmote))
		{
			bool num = MilMo_Player.Instance != null && MilMo_Player.Instance.Avatar != null && Avatar.Room == MilMo_Player.Instance.Avatar.Room && MilMo_Player.Instance.InRoom;
			MilMo_Emote milMo_Emote = null;
			if (num)
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

	public void StartClimbing(MilMo_ClimbingSurface climbingSurface)
	{
		_isClimbing = true;
		_climbingSurface = climbingSurface;
		if (Avatar != null && !Avatar.IsDestroyed)
		{
			Avatar.DisableBlobShadows();
			Avatar.PlayAnimation("GenericClimb01");
		}
	}

	public void StopClimbing()
	{
		_isClimbing = false;
		_climbingSurface = null;
		if (Avatar != null && !Avatar.IsDestroyed)
		{
			Avatar.EnableBlobShadows();
		}
	}

	public void EnterGameplayObject(int gameplayObjectId)
	{
		_onGameplayObjectId = gameplayObjectId;
		_onGameplayObject = null;
		_onCurrentGameplayObjectOffsetIsSet = false;
	}

	public void LeaveGameplayObject()
	{
		_onGameplayObjectId = -1;
		_onGameplayObject = null;
	}

	public void Teleport(Vector3 position, Quaternion rotation)
	{
		_targetPosition = position;
		_targetRotation = rotation;
		_targetVelocity = Vector3.zero;
		if (Avatar != null && !Avatar.IsDestroyed)
		{
			Avatar.GameObject.transform.position = position;
			Avatar.GameObject.transform.rotation = rotation;
		}
	}

	public void HandleExposedVariableUpdate(ExposedVariableUpdate update)
	{
		if (update != null && Avatar != null && !Avatar.IsDestroyed)
		{
			Avatar.SetVariableValue(update.GetName(), update.GetValue());
			if (update.GetName() == "Health" && Avatar.Health <= 0f)
			{
				Avatar.StackAnimation("LandIdle", "Down01");
				Avatar.PlayAnimation(Avatar.IdleAnimation);
			}
		}
	}

	public void ActivateState(MilMo_Template template, bool timeout)
	{
		if (Avatar != null && !Avatar.IsDestroyed)
		{
			if (timeout || !(template is MilMo_PlayerStateTemplate))
			{
				Debug.LogWarning("Failed to activate player state: state template could not be found");
			}
			else
			{
				new MilMo_PlayerState(Avatar, (MilMo_PlayerStateTemplate)template).Activate();
			}
		}
	}

	public void DeactivateState(MilMo_Template template, bool timeout)
	{
		if (Avatar != null && !Avatar.IsDestroyed)
		{
			if (timeout || !(template is MilMo_PlayerStateTemplate))
			{
				Debug.LogWarning("Failed to deactivate player state: state template could not be found");
			}
			else
			{
				Avatar.GetActiveState(template.Identifier)?.Deactivate();
			}
		}
	}

	private void CheckWater()
	{
		if (Avatar == null || Avatar.IsDestroyed)
		{
			return;
		}
		float surfaceY = 0f;
		MilMo_WaterManager.WaterLevel waterLevel = MilMo_WaterManager.WaterLevel.Land;
		if (MilMo_Level.CurrentLevel != null)
		{
			waterLevel = MilMo_WaterManager.GetWaterLevel(Avatar.Position, out surfaceY);
		}
		if (waterLevel == MilMo_WaterManager.WaterLevel.Deep != _isInWater)
		{
			Avatar.WaterSurfaceHeight = surfaceY;
			if (!_isInWater)
			{
				EnterWater();
			}
			else
			{
				ExitWater();
			}
		}
	}

	private void EnterWater()
	{
		_moveFastAnimation = "Swim";
		_moveSlowAnimation = "Swim";
		Avatar.IdleAnimation = "WaterIdle";
		_isInWater = true;
		if (Avatar != null && !Avatar.IsDestroyed)
		{
			switch (_animationState)
			{
			case AnimationState.Run:
				Avatar.PlayAnimation(Avatar.GetAnimationName(_moveFastAnimation));
				break;
			case AnimationState.Walk:
				Avatar.PlayAnimation(Avatar.GetAnimationName(_moveSlowAnimation));
				break;
			case AnimationState.Idle:
				Avatar.PlayAnimation(Avatar.IdleAnimation);
				break;
			}
		}
	}

	private void ExitWater()
	{
		_moveFastAnimation = "Run";
		_moveSlowAnimation = "Walk";
		Avatar.IdleAnimation = "LandIdle";
		_isInWater = false;
		if (Avatar != null && !Avatar.IsDestroyed)
		{
			switch (_animationState)
			{
			case AnimationState.Run:
				Avatar.PlayAnimation(Avatar.GetAnimationName(_moveFastAnimation));
				break;
			case AnimationState.Walk:
				Avatar.PlayAnimation(Avatar.GetAnimationName(_moveSlowAnimation));
				break;
			case AnimationState.Idle:
				Avatar.PlayAnimation(Avatar.IdleAnimation);
				break;
			}
		}
	}

	private void HandleSpawnEffects()
	{
		for (int num = _spawnEffects.Count - 1; num >= 0; num--)
		{
			if (!_spawnEffects[num].Update())
			{
				_spawnEffects.RemoveAt(num);
				if (_spawnEffects.Count == 0)
				{
					_isSpawning = false;
				}
			}
		}
	}

	private void HandleSitting()
	{
		if (Avatar != null && !Avatar.IsDestroyed && !(Avatar.GameObject == null) && !(Avatar.GameObject.transform == null))
		{
			Vector3 vector = Vector3.zero;
			if (Avatar.Ass != null)
			{
				vector = Avatar.GameObject.transform.position - Avatar.Ass.position;
			}
			Avatar.GameObject.transform.position = Avatar.SitPoint.position + vector;
			Avatar.GameObject.transform.rotation = Quaternion.Euler(Avatar.SitPoint.eulerAngles);
			Avatar.Update();
		}
	}

	private void HandleSplineRide()
	{
		if (_spline != null && Avatar != null && !Avatar.IsDestroyed)
		{
			MilMo_GameObjectSpline.SplinePoint pointAtTime = _spline.GetPointAtTime(_splineTime);
			Avatar.GameObject.transform.position = pointAtTime.Position;
			if (pointAtTime.Animation != _currentSplinePoint.Animation)
			{
				Avatar.PlayAnimation(pointAtTime.Animation);
			}
			if (pointAtTime.Sound != _currentSplinePoint.Sound)
			{
				Avatar.PlaySoundEffect(pointAtTime.Sound);
			}
			_currentSplinePoint = pointAtTime;
			Quaternion b = _currentSplinePoint.AlignAxis switch
			{
				2 => Quaternion.LookRotation(_currentSplinePoint.Binormal, _currentSplinePoint.Tangent), 
				3 => Quaternion.LookRotation(_currentSplinePoint.Tangent, -_currentSplinePoint.Binormal), 
				1 => Quaternion.LookRotation(_currentSplinePoint.Normal, -_currentSplinePoint.Binormal), 
				_ => Quaternion.Euler(0f, Quaternion.LookRotation(_currentSplinePoint.Tangent, Vector3.up).eulerAngles.y, 0f), 
			};
			Avatar.GameObject.transform.rotation = Quaternion.Slerp(Avatar.GameObject.transform.rotation, b, 10f * Time.deltaTime);
			if (_spline.IsAtEnd(_splineTime))
			{
				_spline = null;
				Avatar.SuperAlivenessManager.Enable();
				EquipWieldItemBodypack();
			}
			_splineTime += Time.deltaTime;
		}
	}

	private bool HandleOnGameplayObject()
	{
		if (_onGameplayObject == null || _onGameplayObject.GameObject == null || _onGameplayObject.GameObject.GetComponent<Collider>() == null)
		{
			return false;
		}
		if (Avatar == null || Avatar.IsDestroyed || Avatar.GameObject == null)
		{
			return false;
		}
		if (!_onCurrentGameplayObjectOffsetIsSet)
		{
			Vector3 vector = _onGameplayObject.GameObject.transform.position + _onGameplayObjectOffset;
			vector.y += 100f;
			if (_onGameplayObject.GameObject.GetComponent<Collider>().bounds.IntersectRay(new Ray(vector, Vector3.down), out var distance))
			{
				Vector3 vector2 = vector + Vector3.down * distance;
				_onCurrentGameplayObjectOffset = vector2 - _onGameplayObject.GameObject.transform.position;
				_onCurrentGameplayObjectOffsetIsSet = true;
			}
		}
		Avatar.GameObject.transform.position = _onGameplayObject.GameObject.transform.position + _onCurrentGameplayObjectOffset;
		_animationState = AnimationState.Idle;
		Avatar.PlayAnimation(Avatar.IdleAnimation);
		return true;
	}

	private void UnwieldVisually()
	{
		if (Avatar != null && !Avatar.IsDestroyed && Avatar.WieldedItem != null)
		{
			Avatar.UnequipLocal(Avatar.WieldedItem);
			Avatar.AsyncApply();
		}
	}

	private void EquipWieldItemBodypack()
	{
		if (Avatar != null && !Avatar.IsDestroyed && Avatar.WieldedItem != null)
		{
			Avatar.EquipLocal(Avatar.WieldedItem);
			Avatar.AsyncApply();
		}
	}
}
