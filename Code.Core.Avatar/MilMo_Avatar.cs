using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.Core.Avatar.AnimationSystem;
using Code.Core.Avatar.Badges;
using Code.Core.Avatar.BlobShadow;
using Code.Core.Avatar.HappyPickup;
using Code.Core.Avatar.Ragdoll;
using Code.Core.BodyPack;
using Code.Core.BodyPack.ColorSystem;
using Code.Core.BodyPack.SkinPartSystem;
using Code.Core.BuddyBackend;
using Code.Core.Camera;
using Code.Core.Combat;
using Code.Core.Emote;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.Items;
using Code.Core.Network;
using Code.Core.Network.types;
using Code.Core.ObjectEffectSystem;
using Code.Core.PlayerState;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.Core.Template;
using Code.Core.Utility;
using Code.Core.Visual.Effect;
using Code.World;
using Code.World.EntityStates;
using Code.World.Gameplay;
using Code.World.GUI.PVP;
using Code.World.Level;
using Code.World.Level.LevelObject;
using Code.World.Level.PVP;
using Code.World.Player;
using Core;
using Core.State;
using Player;
using Player.Moods;
using UI;
using UI.Marker.Combat;
using UnityEngine;

namespace Code.Core.Avatar;

public class MilMo_Avatar : IMilMo_AttackTarget, IMilMo_Entity
{
	public delegate void AvatarCallback(MilMo_Avatar avatar, string userTag);

	public const byte MALE = 0;

	public const byte FEMALE = 1;

	private PlayerMarker _marker;

	public readonly Code.Core.Combat.Combat Combat;

	private static readonly Vector2 RandomBlinkInterval = new Vector2(2.5f, 7.5f);

	private static readonly Vector2 RandomIdleVariationInterval = new Vector2(15f, 28f);

	public const float WEAPON_COLLISION_BODY_HEIGHT = 2f;

	public static Vector3 WeaponCollisionBodyBottomOffset = new Vector3(0f, -1f, 0f);

	private const float CONSTANT_IMPACT_RADIUS = -0.2f;

	private const float CONSTANT_IMPACT_RADIUS_SQR = 0.040000003f;

	private const float RED_FLASH_FADE_SPEED = 5f;

	public static float HeadScale = 1f;

	public static float HandsScale = 1f;

	public static float FeetScale = 1f;

	public const float DEFAULT_BASE_CONTROLLER_RADIUS = 0.4f;

	public const float DEFAULT_BASE_CONTROLLER_HEIGHT = 1.3f;

	private const float DEFAULT_BASE_SWIM_CONTROLLER_HEIGHT = 0.4f;

	public static Vector3 DefaultBaseControllerCenter = new Vector3(0f, 0.7f, 0f);

	public static Vector2 SwimControllerCenter = new Vector3(0f, 0.15f, 0f);

	private const float ROLLOFF_FACTOR = 0.2f;

	private const float CLIFF_SLOPE_LIMIT = 20f;

	private const float CLIFF_STEP_OFFSET = 0f;

	public const float CLIFF_ON_GROUND_TOLERANCE = 0.24f;

	private const float DEFAULT_SLOPE_LIMIT = 60f;

	private const float DEFAULT_STEP_OFFSET = 0.2f;

	public const float DEFAULT_ON_GROUND_TOLERANCE = 0.24f;

	private const float PROP_SLOPE_LIMIT = 55f;

	private const float PROP_STEP_OFFSET = 0.3f;

	public const float PROP_ON_GROUND_TOLERANCE = 0.24f;

	private const float DEFAULT_HEALTH = 6f;

	private const float DEFAULT_WALK_SPEED = 3f;

	public const float DEFAULT_RUN_SPEED = 7.5f;

	private const float DEFAULT_SWIM_SPEED = 3f;

	private const float DEFAULT_RUN_JUMP_SPEED = 3.112f;

	private const float DEFAULT_WALK_JUMP_SPEED = 3.112f;

	private const float PVP_FLAG_RUN_SPEED = 3.75f;

	private string _currentRoom = "";

	private readonly Dictionary<string, float> _exposedVariables = new Dictionary<string, float>();

	private readonly Dictionary<string, List<string>> _exposedAnimations = new Dictionary<string, List<string>>();

	private readonly Dictionary<string, List<string>> _exposedParticles = new Dictionary<string, List<string>>();

	private readonly List<MilMo_PlayerState> _activeStates = new List<MilMo_PlayerState>();

	private readonly List<GameObject> _loopingSoundEffects = new List<GameObject>();

	private readonly List<string> _loopingSoundEffectsToRemove = new List<string>();

	private readonly List<string> _loopingSoundEffectsQueued = new List<string>();

	private readonly List<MilMo_ObjectEffect> _objectEffects = new List<MilMo_ObjectEffect>();

	private string _currentPlayingAnimation = "Idle";

	private string _fallbackAnimation;

	private string _idleAnimation = "LandIdle";

	private string _moveAnimation = "Run";

	private string _jumpAnimation = "RunJump";

	private string _fallAnimation = "RunFall";

	private string _hoverAnimation = "RunHover";

	private string _moveParticle = "Run";

	private string _jumpParticle = "RunJump";

	private readonly MilMo_SuperAlivenessManager _superAlivenessManager = new MilMo_SuperAlivenessManager();

	private readonly AnimationManager _animationManager = new AnimationManager();

	private readonly MilMo_EyeSpecManager _eyeSpecManager = new MilMo_EyeSpecManager();

	private readonly RagdollManager _ragdollManager;

	private readonly BlobShadowHandler _blobShadowHandler;

	private MilMo_Wieldable _wieldedItem;

	private int _lastAttackAnimationIndex;

	private float _lastAttackTime;

	public float ReadyAttackTime;

	private bool _invulnerable;

	private float _lastLevelUpTime;

	private bool _shouldCollide = true;

	private AvatarCallback _initializeCallback;

	private string _initializeUserTag = "";

	private readonly bool _thumbnailMode;

	private float _blinkTimer = float.MaxValue;

	private float _idleVariationTimer = float.MaxValue;

	private MilMo_ParticleDamageEffect _damageEffect;

	private Color _materialColor;

	private jb_DamageEffect _damageNumberEffect;

	private Transform _neckBase;

	private Transform _leftFoot;

	private Transform _rightFoot;

	private const string SPINE_START_NODE_PATH = "Root/SpineLumbarBase/SpineLumbarLo";

	private const string SPINE_END_NODE_PATH = "Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase";

	private const string HEAD_START_NODE_PATH = "Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase/NeckBaseUp/NeckLo";

	private const string HEAD_END_NODE_PATH = "Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase/NeckBaseUp/NeckLo/NeckTip";

	private const string HEAD_NODE_PATH = "Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase/NeckBaseUp/NeckLo/NeckTip/NodeHeadTip";

	private const string SKULL_TIP_BONE_PATH = "Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase/NeckBaseUp/NeckLo/NeckTip/SkullTip";

	private const string NECK_NODE_PATH = "Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase/NeckBaseUp/NeckLo";

	private const string RIGHT_HAND_NODE_PATH = "Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase/NeckBaseUp/RClavicle/RShoulder/RElbow/RWrist";

	private const string LEFT_HAND_NODE_PATH = "Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase/NeckBaseUp/LClavicle/LShoulder/LElbow/LWrist";

	private const string RIGHT_FOOT_NODE_PATH = "Root/RHip/RKnee/RAnkle";

	private const string LEFT_FOOT_NODE_PATH = "Root/LHip/LKnee/LAnkle";

	private const string LEFT_SHOULDER_NODE_PATH = "Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase/NeckBaseUp/LClavicle/LShoulder";

	private const string RIGHT_SHOULDER_NODE_PATH = "Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase/NeckBaseUp/RClavicle/RShoulder";

	private Bounds _boundingBox;

	private static readonly Dictionary<string, int> SurfacePuffIndices = new Dictionary<string, int>(10)
	{
		{ "Grass", 0 },
		{ "Rock", 1 },
		{ "Gravel", 2 },
		{ "Sand", 3 },
		{ "Water", 4 },
		{ "Stone", 5 },
		{ "Mud", 6 },
		{ "Wood", 7 },
		{ "Metal", 8 },
		{ "ThickGrass", 9 },
		{ "GenericHomeFloor", 10 },
		{ "Snow", 11 },
		{ "ThickSnow", 12 }
	};

	private static readonly Dictionary<string, int> ActionPuffIndices = new Dictionary<string, int>(8)
	{
		{ "Walk", 0 },
		{ "Run", 1 },
		{ "SoftImpact", 2 },
		{ "HardImpact", 3 },
		{ "Stop", 4 },
		{ "SurfaceSwim", 5 },
		{ "SurfaceIdle", 6 },
		{ "Dig", 7 }
	};

	private MilMo_Effect[][] _puffs;

	private readonly List<MilMo_Effect> _activeParticleEffects = new List<MilMo_Effect>();

	private bool _hasWaterEffect;

	private bool _puffsEnabled = true;

	private static readonly int Alpha = Shader.PropertyToID("_Alpha");

	public BadgeManager BadgeManager;

	private int _avatarLevel;

	private string _title;

	private bool _showRole;

	private IList<Item> _items = new List<Item>();

	public MilMo_Player PlayerInstance => MilMo_Player.Instance;

	public IPlayer Player { get; private set; }

	public Armor Armor { get; set; }

	public MilMo_EntityStateManager EntityStateManager { get; private set; }

	public int AvatarLevel => _avatarLevel;

	public string Title => _title;

	public int MembershipDaysLeft { get; set; }

	public bool IsMember => MembershipDaysLeft > -1;

	public sbyte Role { get; set; }

	public bool ShowRole => _showRole;

	public float DefaultControllerHeight => 1.3f / Height;

	public float MarkerYOffset => Head.position.y - GameObject.transform.position.y;

	public float SwimControllerHeight => 0.4f / Height;

	private float DefaultControllerRadius => 0.4f / Height;

	public Vector3 DefaultControllerCenter => new Vector3(0f, Mathf.Max(DefaultControllerHeight / 2f, -0.8f * Height + 1.53f), 0f);

	public float ImpactRadius => -0.2f;

	public float ImpactRadiusSqr => 0.040000003f;

	public string MoveParticle
	{
		get
		{
			return GetParticleEffectName(_moveParticle);
		}
		set
		{
			StopParticleEffect(GetParticleEffectName(_moveParticle));
			_moveParticle = value;
		}
	}

	public string JumpParticle
	{
		get
		{
			return GetParticleEffectName(_jumpParticle);
		}
		set
		{
			StopParticleEffect(GetParticleEffectName(_jumpParticle));
			_jumpParticle = value;
		}
	}

	public string IdleAnimation
	{
		get
		{
			return GetAnimationName(_idleAnimation);
		}
		set
		{
			_idleAnimation = value;
		}
	}

	public string MoveAnimation
	{
		get
		{
			return GetAnimationName(_moveAnimation);
		}
		set
		{
			_moveAnimation = value;
		}
	}

	public string JumpAnimation
	{
		get
		{
			return GetAnimationName(_jumpAnimation);
		}
		set
		{
			_jumpAnimation = value;
		}
	}

	public string FallAnimation
	{
		get
		{
			return GetAnimationName(_fallAnimation);
		}
		set
		{
			_fallAnimation = value;
		}
	}

	public string HoverAnimation
	{
		get
		{
			return GetAnimationName(_hoverAnimation);
		}
		set
		{
			_hoverAnimation = value;
		}
	}

	public MilMo_EmoteManager EmoteManager { get; private set; }

	public MilMo_MoodHandler MoodHandler { get; private set; }

	private AudioSourceWrapper AudioSourceWrapper { get; set; }

	public MilMo_SuperAlivenessManager SuperAlivenessManager => _superAlivenessManager;

	public int TeleportStones { get; set; }

	public string Room
	{
		get
		{
			return _currentRoom;
		}
		set
		{
			_currentRoom = value;
		}
	}

	public CharacterController Controller { get; private set; }

	public GameObject ClickGameObject { get; private set; }

	public bool ShouldCollideWithPlayers
	{
		set
		{
			if (!(Controller == null) && !IsTheLocalPlayer)
			{
				_shouldCollide = value;
				if (_shouldCollide)
				{
					Controller.radius = DefaultControllerRadius;
					Controller.height = DefaultControllerHeight;
					Controller.center = DefaultControllerCenter;
				}
				else
				{
					Controller.radius = 0f;
					Controller.height = 0f;
					Controller.center = Vector3.zero;
				}
			}
		}
	}

	public bool IsTheLocalPlayer { get; }

	public bool IsDestroyed { get; private set; }

	public bool IsGrounded
	{
		get
		{
			if (Controller != null)
			{
				return Controller.isGrounded;
			}
			return false;
		}
	}

	public bool PuffsEnabled => _puffsEnabled;

	public SkinnedMeshRenderer Renderer { get; private set; }

	public MilMo_AvatarQuality Quality { get; set; }

	public string Id { get; private set; }

	public string Name { get; private set; }

	public byte Gender { get; private set; }

	public bool IsBoy => Gender == 0;

	public bool IsGirl => !IsBoy;

	public string SkinColor { get; private set; }

	public string EyeColor { get; private set; }

	public int HairColor { get; private set; }

	public string Mouth { get; private set; }

	public string Eyes { get; private set; }

	public string EyeBrows { get; private set; }

	public MilMo_Wearable Hair { get; private set; }

	public float Height { get; set; }

	public string Mood { get; private set; }

	public MilMo_Wieldable WieldedItem => _wieldedItem;

	public bool Invulnerable => _invulnerable;

	public bool InHappyPickup { get; set; }

	public Transform Head { get; private set; }

	public Transform HeadTip { get; private set; }

	public Transform Ass { get; private set; }

	public Transform SpineStart { get; private set; }

	public Transform SpineEnd { get; private set; }

	public Transform HeadStart { get; private set; }

	public Transform HeadEnd { get; private set; }

	public Transform LeftShoulder { get; private set; }

	public Transform RightShoulder { get; private set; }

	public Transform LeftHand { get; private set; }

	public Transform RightHand { get; private set; }

	public MilMo_BodyPackManager BodyPackManager { get; private set; }

	public GameObject GameObject { get; private set; }

	public Vector3 Position => GameObject.transform.position;

	public HappyPickupItem PickUpItem { get; set; }

	public float WaterSurfaceHeight { get; set; }

	public string CurrentGroundType { get; private set; }

	public bool OnTerrain { get; private set; }

	public bool GroundConfigChanged { get; private set; }

	public bool InCombat => Combat.InCombat;

	public bool IsSitting { get; private set; }

	public string SitPose { get; private set; }

	public Transform SitPoint { get; private set; }

	public float Health
	{
		get
		{
			return GetVariableValue("Health");
		}
		private set
		{
			SetVariableValue("Health", value);
		}
	}

	public float MaxHealth
	{
		get
		{
			return GetVariableValue("MaxHealth");
		}
		private set
		{
			SetVariableValue("MaxHealth", value);
		}
	}

	public bool Enabled { get; private set; }

	public bool UsePvpFlagRunSpeed { private get; set; }

	public bool IsRagdollActive => _ragdollManager.IsActive;

	public Color TargetColor
	{
		get
		{
			MilMo_PVPHandler milMo_PVPHandler = ((MilMo_Level.CurrentLevel != null) ? MilMo_Level.CurrentLevel.PvpHandler : null);
			if (milMo_PVPHandler != null && milMo_PVPHandler.IsTeamMode)
			{
				return milMo_PVPHandler.GetTeamColor(Id);
			}
			if (Singleton<GroupManager>.Instance.InGroup(Id))
			{
				return Color.yellow;
			}
			IPlayer player = Player;
			if ((player != null && player.IsLocalPlayer) || Singleton<MilMo_BuddyBackend>.Instance.GetBuddy(Id) != null)
			{
				return Color.green;
			}
			return Color.white;
		}
	}

	public float CollisionRadius => 0.5f;

	public Bounds BoundingBox => _boundingBox;

	public bool ShouldBeKilled
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public float ImpactHeight => 0.5f;

	public event Action<int> OnAvatarLevelUpdated;

	public event Action OnAvatarTitleUpdated;

	public event Action OnShowRoleChanged;

	public event Action OnHealthChanged;

	public event Action OnHealthDecreased;

	public MilMo_Avatar(bool thumbnailMode)
		: this(null, "-1", isLocalAvatar: false, thumbnailMode, -1, 0, "", 0)
	{
	}

	public MilMo_Avatar(bool thumbnailMode, bool treatAsLocalPlayer)
		: this(null, "-1", treatAsLocalPlayer, thumbnailMode, -1, 0, "", 0)
	{
	}

	public MilMo_Avatar(IPlayer player, string id, bool isLocalAvatar, int membershipDaysLeft, sbyte role, string title, int avatarLevel)
		: this(player, id, isLocalAvatar, thumbnailMode: false, membershipDaysLeft, role, title, avatarLevel)
	{
	}

	private MilMo_Avatar(IPlayer player, string id, bool isLocalAvatar, bool thumbnailMode, int membershipDaysLeft, sbyte role, string title, int avatarLevel)
	{
		Player = player;
		Enabled = true;
		OnTerrain = true;
		Height = 1f;
		Name = "";
		Quality = MilMo_AvatarQuality.High;
		Id = id;
		Role = role;
		MembershipDaysLeft = membershipDaysLeft;
		_title = title;
		_avatarLevel = avatarLevel;
		if (isLocalAvatar)
		{
			Armor = new Armor(PlayerInstance);
		}
		Combat = new Code.Core.Combat.Combat(this);
		EmoteManager = new MilMo_EmoteManager(_superAlivenessManager);
		MoodHandler = new MilMo_MoodHandler(this);
		_ragdollManager = new RagdollManager(this);
		_blobShadowHandler = new BlobShadowHandler();
		BadgeManager = new BadgeManager(this);
		_thumbnailMode = thumbnailMode;
		IsTheLocalPlayer = isLocalAvatar;
		BodyPackManager = new MilMo_BodyPackManager();
		BodyPackManager.SetMainTextureSize(IsTheLocalPlayer ? MilMo_AvatarGlobalLODSettings.LocalAvatarTextureSize : MilMo_AvatarGlobalLODSettings.RemoteAvatarTextureSize);
		BodyPackManager.SetAddonLayer(IsTheLocalPlayer ? 12 : 14);
		BodyPackManager.SetHairLayer(IsTheLocalPlayer ? 12 : 14);
		BodyPackManager.CompressMainTexture = !IsTheLocalPlayer;
		_exposedVariables.Add("MaxHealth", 6f);
		_exposedVariables.Add("Health", 6f);
		_exposedVariables.Add("WalkSpeed", 3f);
		_exposedVariables.Add("RunSpeed", 7.5f);
		_exposedVariables.Add("SwimSpeed", 3f);
		_exposedVariables.Add("WalkJumpSpeed", 3.112f);
		_exposedVariables.Add("RunJumpSpeed", 3.112f);
		_exposedVariables.Add("WeaponCooldownModifier", 0f);
		_exposedAnimations.Add("LandIdle", new List<string> { "Idle" });
		_exposedAnimations.Add("WaterIdle", new List<string> { "SurfaceIdle" });
		_exposedAnimations.Add("Run", new List<string> { "Run" });
		_exposedAnimations.Add("Walk", new List<string> { "Walk" });
		_exposedAnimations.Add("Swim", new List<string> { "SurfaceSwim" });
		_exposedAnimations.Add("RunJump", new List<string> { "GenericJump01" });
		_exposedAnimations.Add("RunFall", new List<string> { "GenericFall01" });
		_exposedAnimations.Add("RunHover", new List<string> { "GenericHover01" });
		_exposedAnimations.Add("WalkJump", new List<string> { "GenericJump01" });
		_exposedAnimations.Add("WalkFall", new List<string> { "GenericFall01" });
		_exposedAnimations.Add("WalkHover", new List<string> { "GenericHover01" });
		_exposedParticles.Add("Run", new List<string>());
		_exposedParticles.Add("RunJump", new List<string>());
		_exposedParticles.Add("WalkJump", new List<string>());
		_exposedParticles.Add("Walk", new List<string>());
		_exposedParticles.Add("Swim", new List<string>());
	}

	public void InitDamageNumberEffect()
	{
		if (!(MilMo_World.Instance == null))
		{
			_damageNumberEffect = new jb_DamageEffect(MilMo_World.Instance.UI, this);
			MilMo_World.Instance.UI.AddChild(_damageNumberEffect);
		}
	}

	public void SetAvatarLevel(int avatarLevel)
	{
		_avatarLevel = avatarLevel;
		this.OnAvatarLevelUpdated?.Invoke(avatarLevel);
	}

	public void SetTitle(string title)
	{
		_title = title;
		this.OnAvatarTitleUpdated?.Invoke();
	}

	public void SetShowRole(bool showRole)
	{
		_showRole = showRole;
		this.OnShowRoleChanged?.Invoke();
	}

	public bool IsCritter()
	{
		return false;
	}

	private void SetWieldedItem(MilMo_Wieldable value)
	{
		SheathWeapon();
		_wieldedItem?.Unwield();
		_wieldedItem = value;
		if (Combat.InCombat)
		{
			DrawWeapon();
		}
		_wieldedItem?.Wield(this, _wieldedItem.IsFood() && !IsSitting);
	}

	public void SetInvulnerable(bool value)
	{
		if (value != _invulnerable)
		{
			_invulnerable = value;
			UpdateInvulnerableFX();
		}
	}

	private void UpdateInvulnerableFX()
	{
		if ((bool)GameObject)
		{
			if (_invulnerable)
			{
				AddParticleEffect(MilMo_EffectContainer.GetEffect("PvpNoDamage", GameObject));
				return;
			}
			StopParticleEffect("PvpNoDamage");
			AddParticleEffect(MilMo_EffectContainer.GetEffect("PvpNoDamagePop", GameObject));
		}
	}

	public void UpdateHealth(float value)
	{
		float health = Health;
		float num = value;
		if (value <= 1f && value >= 0.1f)
		{
			num = 1f;
		}
		Health = num;
		this.OnHealthChanged?.Invoke();
		if (_thumbnailMode)
		{
			return;
		}
		if (Player.IsLocalPlayer)
		{
			GlobalStates.Instance.playerState.health.Set((int)num);
			if (health / MaxHealth > 0.2f && num / MaxHealth <= 0.2f)
			{
				MilMo_EventSystem.Instance.PostEvent("low_health", null);
			}
			if (value > 0f && value < Health && (double)Math.Abs(value - MaxHealth) > 0.01)
			{
				MilMo_EventSystem.Instance.PostEvent("tutorial_TakeDamage", null);
				this.OnHealthDecreased?.Invoke();
			}
		}
		if (value <= 0f)
		{
			StackAnimation("LandIdle", "Down01");
			PlayAnimation(IdleAnimation);
			if (Player.IsLocalPlayer)
			{
				PlayerInstance.Exhausted();
			}
		}
	}

	public void UpdateMaxHealth(float health)
	{
		MaxHealth = health;
		this.OnHealthChanged?.Invoke();
		if (Player.IsLocalPlayer)
		{
			GlobalStates.Instance.playerState.maxHealth.Set((int)MaxHealth);
		}
	}

	public void ShowHair()
	{
		foreach (MilMo_Addon item in BodyPackManager.Equipped.Where((MilMo_BodyPack bp) => bp.IsHair).SelectMany((MilMo_BodyPack bp) => bp.Addons))
		{
			item.Enable(Renderer);
		}
	}

	public void HideHair()
	{
		foreach (MilMo_Addon item in BodyPackManager.Equipped.Where((MilMo_BodyPack bp) => bp.IsHair).SelectMany((MilMo_BodyPack bp) => bp.Addons))
		{
			item.Disable(Renderer);
		}
	}

	public void DisableRefraction()
	{
		int num = (IsTheLocalPlayer ? 13 : 15);
		Renderer.gameObject.layer = num;
		BodyPackManager.SetAddonLayer(num);
		BodyPackManager.SetHairLayer(num);
	}

	public void EnableRefraction()
	{
		int num = (IsTheLocalPlayer ? 12 : 14);
		Renderer.gameObject.layer = num;
		BodyPackManager.SetAddonLayer(num);
		BodyPackManager.SetHairLayer(num);
	}

	public void DisablePuffs()
	{
		_puffsEnabled = false;
		if (_puffs == null)
		{
			return;
		}
		MilMo_Effect[][] puffs = _puffs;
		foreach (MilMo_Effect[] array in puffs)
		{
			for (int j = 0; j < array.Length; j++)
			{
				array[j]?.Stop();
			}
		}
	}

	public void EnablePuffs()
	{
		_puffsEnabled = true;
		if (_puffs == null)
		{
			return;
		}
		MilMo_Effect[][] puffs = _puffs;
		foreach (MilMo_Effect[] array in puffs)
		{
			for (int j = 0; j < array.Length; j++)
			{
				array[j]?.Restart();
			}
		}
	}

	public void ResetCombatTimer()
	{
		Combat.ResetCombatTimer();
	}

	public void RemoveFromCombat()
	{
		Combat.EndCombat();
		StackAnimation("LandIdle", "Down01");
		PlayAnimation(IdleAnimation);
	}

	public void PlayAttackEffects()
	{
		if (!(_wieldedItem is MilMo_Weapon milMo_Weapon) || milMo_Weapon.Template.AttackAnimations.Count == 0)
		{
			return;
		}
		int num = 0;
		if (Time.time - _lastAttackTime < milMo_Weapon.Template.AnimationSequenceTimeout)
		{
			num = _lastAttackAnimationIndex + 1;
			if (num > milMo_Weapon.Template.AttackAnimations.Count - 1)
			{
				num = 0;
				SetVariableValue("WeaponCooldownModifier", 0f);
			}
			else if (num == milMo_Weapon.Template.AttackAnimations.Count - 1)
			{
				SetVariableValue("WeaponCooldownModifier", milMo_Weapon.Template.Cooldown * 0.5f);
			}
		}
		else if (_lastAttackAnimationIndex == milMo_Weapon.Template.AttackAnimations.Count - 1)
		{
			SetVariableValue("WeaponCooldownModifier", 0f);
		}
		string text = milMo_Weapon.Template.AttackAnimations[num];
		PlayAnimation(text);
		if (_animationManager.HasAnimation(text))
		{
			MilMo_EventSystem.At(_animationManager.GetAnimationDuration(text), StopLookAtTarget);
		}
		PlaySoundEffect(MilMo_BashAnimationSoundSystem.GetSound(text));
		_lastAttackAnimationIndex = num;
		_lastAttackTime = Time.time;
	}

	private void StopLookAtTarget()
	{
	}

	public float GetVariableValue(string variableName)
	{
		if (!_exposedVariables.ContainsKey(variableName))
		{
			return 0f;
		}
		return _exposedVariables[variableName];
	}

	public void SetVariableValue(string variableName, float value)
	{
		if (variableName == "RunSpeed" && UsePvpFlagRunSpeed)
		{
			value = 3.75f;
		}
		if (_exposedVariables.ContainsKey(variableName))
		{
			_exposedVariables[variableName] = value;
		}
		else
		{
			_exposedVariables.Add(variableName, value);
		}
	}

	public void StackParticleEffect(string particle, string newEffectName)
	{
		if (_exposedParticles.ContainsKey(particle))
		{
			List<string> list = _exposedParticles[particle];
			if (list.Count > 0)
			{
				string text = list[^1];
				if (!string.IsNullOrEmpty(text))
				{
					for (int num = _activeParticleEffects.Count - 1; num >= 0; num--)
					{
						if (!(_activeParticleEffects[num].Name != text))
						{
							_activeParticleEffects[num].Stop();
							_activeParticleEffects[num].Destroy();
							_activeParticleEffects.RemoveAt(num);
							PlayParticleEffect(newEffectName);
							break;
						}
					}
				}
			}
			list.Add(newEffectName);
		}
		else
		{
			List<string> value = new List<string> { newEffectName };
			_exposedParticles.Add(particle, value);
		}
	}

	public void UnstackParticleEffect(string particle, string effectName)
	{
		if (!_exposedParticles.ContainsKey(particle))
		{
			return;
		}
		List<string> list = _exposedParticles[particle];
		if (list.Count <= 0)
		{
			return;
		}
		bool flag = true;
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num] == effectName)
			{
				list.RemoveAt(num);
				break;
			}
			flag = false;
		}
		if (!flag)
		{
			return;
		}
		for (int num2 = _activeParticleEffects.Count - 1; num2 >= 0; num2--)
		{
			if (!(_activeParticleEffects[num2].Name != effectName))
			{
				_activeParticleEffects[num2].Stop();
				_activeParticleEffects[num2].Destroy();
				_activeParticleEffects.RemoveAt(num2);
				break;
			}
		}
		if (list.Count > 0)
		{
			PlayParticleEffect(list[^1]);
		}
	}

	private string GetParticleEffectName(string particle)
	{
		if (!_exposedParticles.ContainsKey(particle))
		{
			return "";
		}
		List<string> list = _exposedParticles[particle];
		if (list != null && list.Count > 0)
		{
			return list[^1];
		}
		return "";
	}

	public void StackAnimation(string animation, string newAnimationName)
	{
		if (!GameObject || !_animationManager.HasAnimation(newAnimationName))
		{
			Debug.Log("No animation named " + newAnimationName);
		}
		else if (_exposedAnimations.ContainsKey(animation))
		{
			List<string> list = _exposedAnimations[animation];
			if (list.Count > 0)
			{
				string text = list[^1];
				if (!string.IsNullOrEmpty(text))
				{
					string currentLoopingAnimation = _animationManager.GetCurrentLoopingAnimation();
					if (currentLoopingAnimation == text || currentLoopingAnimation == MilMo_EmoteSystem.GetMoodAnimation(text, Mood))
					{
						PlayAnimation(newAnimationName);
					}
				}
			}
			list.Add(newAnimationName);
		}
		else
		{
			List<string> value = new List<string> { newAnimationName };
			_exposedAnimations.Add(animation, value);
		}
	}

	public void UnstackAnimation(string animation, string animationName, bool unstackAll = false)
	{
		List<string> list = _exposedAnimations[animation];
		if (list.Count <= 0)
		{
			return;
		}
		bool flag = false;
		bool flag2 = true;
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num] == animationName)
			{
				list.RemoveAt(num);
				if (flag2)
				{
					flag = true;
				}
				if (!unstackAll)
				{
					break;
				}
			}
			flag2 = false;
		}
		string currentLoopingAnimation = _animationManager.GetCurrentLoopingAnimation();
		if (flag && (currentLoopingAnimation == animationName || currentLoopingAnimation == MilMo_EmoteSystem.GetMoodAnimation(animationName, Mood)) && list.Count > 0)
		{
			PlayAnimation(list[^1]);
		}
	}

	public void DrawWeapon()
	{
		if (_wieldedItem is MilMo_Weapon milMo_Weapon && !string.IsNullOrEmpty(milMo_Weapon.Template.AttackIdleAnimation))
		{
			StackAnimation("LandIdle", milMo_Weapon.Template.AttackIdleAnimation);
		}
	}

	public void SheathWeapon()
	{
		if (_wieldedItem is MilMo_Weapon milMo_Weapon)
		{
			UnstackAnimation("LandIdle", milMo_Weapon.Template.AttackIdleAnimation);
		}
	}

	public string GetAnimationName(string animation)
	{
		if (!_exposedAnimations.ContainsKey(animation))
		{
			return "";
		}
		List<string> list = _exposedAnimations[animation];
		if (list != null && list.Count > 0)
		{
			return list[^1];
		}
		return "";
	}

	public void AddActiveState(MilMo_PlayerState state)
	{
		_activeStates.Add(state);
	}

	public void RemoveActiveState(MilMo_PlayerState state)
	{
		_activeStates.Remove(state);
	}

	public MilMo_PlayerState GetActiveState(string templateIdentifier)
	{
		return _activeStates.FirstOrDefault((MilMo_PlayerState s) => s.Template.Identifier == templateIdentifier);
	}

	public void Recreate(Code.Core.Network.types.Avatar avatar)
	{
		BodyPackManager.Unload();
		BodyPackManager = new MilMo_BodyPackManager();
		Read(Player, avatar);
	}

	public void Destroy()
	{
		if (EntityStateManager != null)
		{
			EntityStateManager.Destroy();
			EntityStateManager = null;
		}
		if (MoodHandler != null)
		{
			MoodHandler.Destroy();
			MoodHandler = null;
		}
		if (EmoteManager != null)
		{
			EmoteManager.Destroy();
			EmoteManager = null;
		}
		foreach (MilMo_ObjectEffect objectEffect in _objectEffects)
		{
			objectEffect.Destroy();
		}
		_objectEffects.Clear();
		if (_damageNumberEffect != null)
		{
			_damageNumberEffect.Remove();
		}
		if ((bool)GameObject)
		{
			MilMo_GameplayObjectSaver[] componentsInChildren = GameObject.transform.GetComponentsInChildren<MilMo_GameplayObjectSaver>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].OnParentAboutToBeDestroyed();
			}
			UnityEngine.Object.Destroy(GameObject);
		}
		if ((bool)ClickGameObject)
		{
			UnityEngine.Object.Destroy(ClickGameObject);
		}
		if (BodyPackManager != null)
		{
			BodyPackManager.Unload();
			BodyPackManager = null;
		}
		IsDestroyed = true;
		if (_marker != null)
		{
			_marker.Remove();
			_marker = null;
		}
		Combat.EndCombat();
		if (BadgeManager != null)
		{
			BadgeManager.Destroy();
			BadgeManager = null;
		}
		Resources.UnloadUnusedAssets();
	}

	public void SetInitializedCallback(AvatarCallback callback, string tag)
	{
		_initializeCallback = callback;
		_initializeUserTag = tag;
	}

	public void SetInitializedCallback(AvatarCallback callback)
	{
		_initializeCallback = callback;
		_initializeUserTag = "AvatarCreated";
	}

	public void Disable()
	{
		if (Enabled)
		{
			if ((bool)ClickGameObject)
			{
				ClickGameObject.SetActive(value: false);
			}
			Renderer.enabled = false;
			BodyPackManager.Disable();
			Enabled = false;
		}
	}

	public void Enable()
	{
		if (!Enabled)
		{
			GameObject.SetActive(value: true);
			if ((bool)ClickGameObject)
			{
				ClickGameObject.SetActive(value: true);
			}
			Renderer.enabled = true;
			BodyPackManager.Enable();
			Enabled = true;
			if (_animationManager.IsEnabled())
			{
				string currentLoopingAnimation = _animationManager.GetCurrentLoopingAnimation();
				_animationManager.Play((!string.IsNullOrEmpty(currentLoopingAnimation)) ? currentLoopingAnimation : IdleAnimation);
			}
		}
	}

	public void EnableAnimations()
	{
		if (_animationManager.EnableAnimations())
		{
			string currentLoopingAnimation = _animationManager.GetCurrentLoopingAnimation();
			_animationManager.Play((!string.IsNullOrEmpty(currentLoopingAnimation)) ? currentLoopingAnimation : IdleAnimation);
		}
	}

	public void DisableAnimations()
	{
		_animationManager.DisableAnimations();
	}

	public void EnableRagdoll(Vector3 force, ForcePosition forcePosition)
	{
		_ragdollManager.Activate(force, forcePosition);
	}

	public void DisableRagdoll()
	{
		_ragdollManager.Deactivate();
	}

	public void EnableBlobShadows()
	{
		_blobShadowHandler?.Enable();
	}

	public void DisableBlobShadows()
	{
		_blobShadowHandler?.Disable();
	}

	public void DisplayLevelUp()
	{
		if ((bool)GameObject && _lastLevelUpTime < Time.time - 6f)
		{
			AddParticleEffect(MilMo_EffectContainer.GetEffect("LevelUpEffect", GameObject));
			PlayAnimation("Activation01");
			_lastLevelUpTime = Time.time;
		}
	}

	public void Update()
	{
		if (!GameObject || !Enabled)
		{
			return;
		}
		SampleGroundType();
		if (!IsSitting && _puffsEnabled)
		{
			UpdatePuffs();
		}
		HappyPickupItem pickUpItem = PickUpItem;
		if (pickUpItem != null && pickUpItem.VisualRep != null)
		{
			PickUpItem.VisualRep.Update();
		}
		for (int num = _activeParticleEffects.Count - 1; num >= 0; num--)
		{
			if (!_activeParticleEffects[num].Update())
			{
				_activeParticleEffects.RemoveAt(num);
			}
		}
		_damageEffect?.Update();
		foreach (MilMo_PlayerState activeState in _activeStates)
		{
			activeState.Update();
		}
		for (int num2 = _objectEffects.Count - 1; num2 >= 0; num2--)
		{
			if (!_objectEffects[num2].Update())
			{
				_objectEffects.RemoveAt(num2);
			}
		}
		_blinkTimer -= Time.deltaTime;
		if (_blinkTimer <= 0f)
		{
			PlayEmoteLocal(MilMo_Global.TheBlinks.GetRandomEmote(), isBlink: true);
		}
		if (_currentPlayingAnimation == "Idle")
		{
			_idleVariationTimer -= Time.deltaTime;
			if (_idleVariationTimer <= 0f)
			{
				PlayEmoteLocal(MilMo_Global.TheIdleVariations.GetRandomEmote(), isBlink: false);
			}
		}
		if (Renderer.material.color != _materialColor)
		{
			UpdateDamageEffect();
		}
		BodyPackManager?.Update();
		_superAlivenessManager?.Update();
		_ragdollManager?.Update();
	}

	public void FixedUpdate()
	{
		_superAlivenessManager?.FixedUpdate();
		if (_damageEffect != null)
		{
			_damageEffect.FixedUpdate();
		}
	}

	public void LateUpdate()
	{
		_eyeSpecManager.LateUpdate();
		ApplyBodyPartScale();
	}

	public async void Read(IPlayer player, Code.Core.Network.types.Avatar avatar)
	{
		if (!_thumbnailMode)
		{
			Player = player;
		}
		LoadAvatarInfo(avatar);
		List<MilMo_Wearable> wearables = await LoadWearables();
		LoadGameObject();
		SetupBodyPack();
		EquipAllWearables(wearables);
		InitBodyPackManager();
		if (!_thumbnailMode)
		{
			LoadPuffsForGameObject();
			LoadDamageStarsForGameObject();
		}
		EntityStateManager = new MilMo_EntityStateManager(this);
	}

	public void EnableSuperAliveness()
	{
		_superAlivenessManager.SetLodLevelEnabled();
	}

	public void DisableSuperAliveness()
	{
		_superAlivenessManager.SetLodLevelDisabled();
	}

	private void SampleGroundType()
	{
		if (!GameObject || MilMo_Instance.CurrentInstance == null)
		{
			return;
		}
		bool terrain;
		string materialAtPosition = MilMo_Instance.CurrentInstance.GetMaterialAtPosition(GameObject.transform.position, 0.5f, out terrain);
		GroundConfigChanged = terrain != OnTerrain || (materialAtPosition != CurrentGroundType && (materialAtPosition == "Rock" || CurrentGroundType == "Rock"));
		if (GroundConfigChanged && (bool)Controller)
		{
			float height = Controller.height;
			if (terrain)
			{
				if (materialAtPosition == "Rock")
				{
					Controller.stepOffset = ((0f < height) ? 0f : height);
					Controller.slopeLimit = 20f;
				}
				else
				{
					Controller.stepOffset = ((0.2f < height) ? 0.2f : height);
					Controller.slopeLimit = 60f;
				}
			}
			else
			{
				Controller.stepOffset = ((0.3f < height) ? 0.3f : height);
				Controller.slopeLimit = 55f;
			}
		}
		OnTerrain = terrain;
		CurrentGroundType = materialAtPosition;
	}

	public void AddObjectEffect(MilMo_ObjectEffect effect)
	{
		if (effect != null)
		{
			_objectEffects.Add(effect);
		}
	}

	public void AddParticleEffect(MilMo_Effect effect)
	{
		if (effect != null)
		{
			_activeParticleEffects.Add(effect);
		}
	}

	private void ApplyBodyPartScale()
	{
		if ((bool)_neckBase)
		{
			_neckBase.localScale = Vector3.one * (HeadScale / Height);
		}
		if ((bool)LeftHand)
		{
			LeftHand.localScale = Vector3.one * HandsScale;
		}
		if ((bool)RightHand)
		{
			RightHand.localScale = Vector3.one * HandsScale;
		}
		if ((bool)_leftFoot)
		{
			_leftFoot.localScale = Vector3.one * FeetScale;
		}
		if ((bool)_rightFoot)
		{
			_rightFoot.localScale = Vector3.one * FeetScale;
		}
	}

	private void PlayCurrentAnimation()
	{
		PlayAnimation(_currentPlayingAnimation);
	}

	public void PlayAnimation(string anim)
	{
		if (anim != "Run" || _wieldedItem == null)
		{
			string moodAnimation = MilMo_EmoteSystem.GetMoodAnimation(anim, Mood);
			_animationManager.Play(moodAnimation);
		}
		else
		{
			_animationManager.Play(anim);
		}
		if (!(anim == IdleAnimation) && !(anim == MoveAnimation))
		{
			switch (anim)
			{
			case "Jump":
			case "GenericJump01":
			case "GenericHover01":
			case "GenericFall01":
			case "SitChair":
			case "SitLieChair":
			case "Down01":
			case "GenericClimb01":
				break;
			default:
				goto IL_00ee;
			}
		}
		_currentPlayingAnimation = anim;
		if (UnityEngine.Random.Range(0f, 1f) <= 0.2f)
		{
			PlayEmoteLocal(MilMo_Global.TheBlinks.GetRandomEmote(), isBlink: true);
		}
		goto IL_00ee;
		IL_00ee:
		if (anim != "Idle" && !anim.StartsWith("IdleVariation"))
		{
			_idleVariationTimer = float.MaxValue;
		}
		else
		{
			_idleVariationTimer = UnityEngine.Random.Range(RandomIdleVariationInterval.x, RandomIdleVariationInterval.y);
		}
	}

	public void PlaySoundEffectLooping(string sound)
	{
		if (_loopingSoundEffectsQueued.Any((string queuedSound) => queuedSound == sound))
		{
			for (int i = 0; i < _loopingSoundEffectsToRemove.Count; i++)
			{
				if (!(_loopingSoundEffectsToRemove[i] != sound))
				{
					_loopingSoundEffectsToRemove.RemoveAt(i);
					break;
				}
			}
		}
		else
		{
			_loopingSoundEffectsQueued.Add(sound);
			LoadSoundEffectAsync(sound);
		}
	}

	private async void LoadSoundEffectAsync(string sound)
	{
		AudioClip audioClip = await MilMo_ResourceManager.Instance.LoadAudioAsync(sound);
		for (int i = 0; i < _loopingSoundEffectsQueued.Count; i++)
		{
			if (!(_loopingSoundEffectsQueued[i] != sound))
			{
				_loopingSoundEffectsQueued.RemoveAt(i);
				break;
			}
		}
		for (int j = 0; j < _loopingSoundEffectsToRemove.Count; j++)
		{
			if (!(_loopingSoundEffectsToRemove[j] != sound))
			{
				_loopingSoundEffectsToRemove.RemoveAt(j);
				return;
			}
		}
		if (audioClip == null)
		{
			Debug.LogWarning("Failed to play looping sound effect '" + sound + "': no such audio clip");
			return;
		}
		GameObject gameObject = new GameObject(sound);
		gameObject.transform.parent = GameObject.transform;
		AudioSourceWrapper audioSourceWrapper = gameObject.AddComponent<AudioSourceWrapper>();
		audioSourceWrapper.Clip = audioClip;
		audioSourceWrapper.Loop = true;
		audioSourceWrapper.Play();
		_loopingSoundEffects.Add(gameObject);
	}

	public void StopLoopingSoundEffect(string sound)
	{
		for (int num = _loopingSoundEffects.Count - 1; num >= 0; num--)
		{
			if (!(_loopingSoundEffects[num].name != sound))
			{
				_loopingSoundEffects[num].GetComponent<AudioSourceWrapper>().Stop();
				MilMo_Global.Destroy(_loopingSoundEffects[num]);
				_loopingSoundEffects.RemoveAt(num);
				return;
			}
		}
		if (!_loopingSoundEffectsQueued.All((string streamedSound) => streamedSound != sound) && !_loopingSoundEffectsToRemove.Any((string soundToRemove) => sound == soundToRemove))
		{
			_loopingSoundEffectsToRemove.Add(sound);
		}
	}

	public async void PlaySoundEffect(string sound)
	{
		if (!string.IsNullOrEmpty(sound))
		{
			AudioClip audioClip = await MilMo_ResourceManager.Instance.LoadAudioAsync(sound);
			if (audioClip == null)
			{
				Debug.LogWarning("Failed to play sound effect '" + sound + "': no such audio clip.");
			}
			else
			{
				PlaySoundEffect(audioClip);
			}
		}
	}

	public void PlaySoundEffect(AudioClip sound)
	{
		if ((bool)GameObject && (bool)sound && (bool)AudioSourceWrapper)
		{
			AudioSourceWrapper.enabled = true;
			AudioSourceWrapper.Clip = sound;
			AudioSourceWrapper.Loop = false;
			AudioSourceWrapper.Play();
		}
	}

	public void EmitPuff(string puffAction)
	{
		if (MilMo_Level.CurrentLevel != null && (bool)GameObject)
		{
			EmitPuff(puffAction, CurrentGroundType);
		}
	}

	public void EmitPuff(string puffAction, string material)
	{
		if ((bool)GameObject && !string.IsNullOrEmpty(material) && !string.IsNullOrEmpty(puffAction) && SurfacePuffIndices.TryGetValue(material, out var value) && ActionPuffIndices.TryGetValue(puffAction, out var value2) && _puffs[value][value2] != null)
		{
			_puffs[value][value2].Restart();
			_activeParticleEffects.Add(_puffs[value][value2]);
		}
	}

	public void EmitPuff(string puffAction, string material, float yHeight)
	{
		if ((bool)GameObject && !string.IsNullOrEmpty(material) && !string.IsNullOrEmpty(puffAction) && SurfacePuffIndices.TryGetValue(material, out var value) && ActionPuffIndices.TryGetValue(puffAction, out var value2) && _puffs[value][value2] != null)
		{
			_puffs[value][value2].Restart(yHeight);
			_activeParticleEffects.Add(_puffs[value][value2]);
		}
	}

	public void PlayParticleEffect(string effectName)
	{
		if ((bool)GameObject)
		{
			MilMo_Effect effect = MilMo_EffectContainer.GetEffect(effectName, GameObject);
			if (effect != null)
			{
				_activeParticleEffects.Add(effect);
			}
		}
	}

	public void PlayParticleEffect(string effectName, Vector3 offset)
	{
		if ((bool)GameObject)
		{
			MilMo_Effect effect = MilMo_EffectContainer.GetEffect(effectName, GameObject);
			if (effect != null)
			{
				effect.SetOffset(offset);
				_activeParticleEffects.Add(effect);
			}
		}
	}

	public void StopParticleEffect(string effectName)
	{
		for (int num = _activeParticleEffects.Count - 1; num >= 0; num--)
		{
			if (!(_activeParticleEffects[num].Name != effectName))
			{
				_activeParticleEffects[num].Stop();
				_activeParticleEffects[num].Destroy();
				_activeParticleEffects.RemoveAt(num);
			}
		}
	}

	private void ChangeWaterEffect(string anim, string currentSurface)
	{
		if (MilMo_Level.CurrentLevel == null || !GameObject || _puffs == null)
		{
			return;
		}
		int num = SurfacePuffIndices["Water"];
		for (int i = 0; i < ActionPuffIndices.Count; i++)
		{
			if (_puffs[num][i] != null)
			{
				_puffs[num][i].Stop();
			}
			_hasWaterEffect = false;
		}
		if (!(currentSurface != "Water") && ActionPuffIndices.TryGetValue(anim, out var value) && _puffs[num][value] != null)
		{
			_puffs[num][value].Restart();
			_hasWaterEffect = true;
		}
	}

	public void Damaged(float amount, Vector3 pointOfOrigin)
	{
		if ((bool)GameObject && !(Math.Abs(amount) < 0f))
		{
			if (_damageEffect == null)
			{
				_damageEffect = new MilMo_ParticleDamageEffect(GameObject.transform, Height);
			}
			else
			{
				_damageEffect.Validate(GameObject.transform, Height);
			}
			Vector3 position = GameObject.transform.position;
			_damageNumberEffect?.ShowDamage(amount, Vector3.Distance(MilMo_Camera.Instance.transform.position, position));
			pointOfOrigin.y = 0f;
			Vector3 vector = position;
			vector.y = 0f;
			Vector3 vector2 = ((!MilMo_Utility.Equals(pointOfOrigin, vector)) ? (pointOfOrigin - vector).normalized : Vector3.up);
			Vector3 impactPosition = vector + -0.2f * vector2;
			impactPosition.y = position.y + Height / 2f;
			Vector3 position2 = MilMo_CameraController.CameraTransform.position;
			Vector3 vector3 = position - position2;
			vector3.y = 0f;
			int num = UnityEngine.Random.Range(0, 2) * 2 - 1;
			Vector3 vector4 = new Vector3((float)num * vector3.z, 0f, (float)(-1 * num) * vector3.x);
			vector4.Normalize();
			float num2 = (float)Math.Acos(vector4.x);
			float minInclusive = num2 - MathF.PI / 4f;
			float maxInclusive = num2 + MathF.PI / 4f;
			float f = UnityEngine.Random.Range(minInclusive, maxInclusive);
			Vector3 starDirection = new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
			_damageEffect.Emit(amount, impactPosition, starDirection, 0.75f, "Content/Particles/Batch01/BashRed");
			Renderer.material.color = Color.red;
		}
	}

	public void Sit(Transform moveToTransform, string snapBone, string sitPose)
	{
		if (IsSitting)
		{
			StopSitting();
		}
		IsSitting = true;
		SitPose = sitPose;
		SitPoint = moveToTransform;
		StackAnimation("LandIdle", sitPose);
		PlayAnimation(sitPose);
		Transform transform = Renderer.bones.FirstOrDefault((Transform bone) => bone.name == snapBone);
		Vector3 vector = Vector3.zero;
		if (transform != null)
		{
			vector = GameObject.transform.position - transform.position;
		}
		GameObject.transform.position = moveToTransform.position + vector;
		GameObject.transform.rotation = Quaternion.Euler(moveToTransform.rotation.eulerAngles);
	}

	public void StopSitting()
	{
		IsSitting = false;
		UnstackAnimation("LandIdle", SitPose);
	}

	public void EquipLocal(MilMo_Wearable wearable)
	{
		if (wearable != null && wearable.BodyPack != null)
		{
			BodyPackManager.Equip(wearable.BodyPack, wearable.ColorIndices);
		}
	}

	public void UnequipAll()
	{
		BodyPackManager.UnEquipAll();
	}

	public void UnequipLocal(MilMo_Wearable wearable)
	{
		if (wearable != null && wearable.BodyPack != null)
		{
			BodyPackManager.Unequip(wearable.BodyPack);
		}
	}

	public void StripLocal()
	{
		if (BodyPackManager != null)
		{
			BodyPackManager.UnEquipAll();
			BodyPackManager.AsyncApply(GetPriority(), null);
		}
	}

	public void PlayEmote(MilMo_Emote emote)
	{
		if (emote != null)
		{
			PlayEmoteLocal(emote, isBlink: false);
			Singleton<GameNetwork>.Instance.SendEmoteUpdate(emote.Path);
		}
	}

	public void PlayEmoteLocal(MilMo_Emote emote, float forcedDuration = 0f)
	{
		if (emote != null && EmoteManager != null)
		{
			EmoteManager.PlayEmote(emote);
			_blinkTimer = UnityEngine.Random.Range(Mathf.Max(((forcedDuration > 0.1f) ? forcedDuration : emote.Duration) + 1f, RandomBlinkInterval.x), RandomBlinkInterval.y);
			_idleVariationTimer = UnityEngine.Random.Range(Mathf.Max(((forcedDuration > 0.1f) ? forcedDuration : emote.Duration) + 1f, RandomIdleVariationInterval.x), RandomIdleVariationInterval.y);
			if (forcedDuration > 0.1f)
			{
				MilMo_EventSystem.At(forcedDuration, EmoteManager.AbortEmote);
			}
		}
	}

	public void PlayEmoteLocal(MilMo_Emote emote, bool isBlink)
	{
		if (emote != null && EmoteManager != null)
		{
			EmoteManager.PlayEmote(emote);
			_blinkTimer = UnityEngine.Random.Range(Mathf.Max(emote.Duration + 1f, RandomBlinkInterval.x), RandomBlinkInterval.y);
			if (!isBlink && _currentPlayingAnimation == "Idle")
			{
				_idleVariationTimer = UnityEngine.Random.Range(Mathf.Max(emote.Duration + 1f, RandomIdleVariationInterval.x), RandomIdleVariationInterval.y);
			}
		}
	}

	public void SetMood(Mood mood)
	{
		Mood = mood.GetKey();
		EmoteManager?.SetMoodByName(mood.GetKey());
		PlayCurrentAnimation();
	}

	public void SetMood(string mood)
	{
		Mood = mood;
		EmoteManager?.SetMoodByName(mood);
		PlayCurrentAnimation();
	}

	public bool ChangeSkinColor(string color)
	{
		MilMo_Color skinColor = MilMo_ColorSystem.GetSkinColor(color);
		if (skinColor == null)
		{
			Debug.LogWarning("Trying to set unknown skin color for avatar");
			return false;
		}
		SkinColor = color;
		BodyPackManager.SkinColor = skinColor;
		return true;
	}

	public bool ChangeHairColor(int color)
	{
		HairColor = color;
		BodyPackManager.HairColor = color;
		return true;
	}

	public bool ChangeEyeColor(string color)
	{
		MilMo_Color eyeColor = MilMo_ColorSystem.GetEyeColor(color);
		if (eyeColor == null)
		{
			Debug.LogWarning("Trying to set unknown eye color for avatar");
			return false;
		}
		EyeColor = color;
		BodyPackManager.EyeColor = eyeColor;
		return true;
	}

	public bool ChangeMouth(string mouth)
	{
		MilMo_SkinPart mouth2 = MilMo_SkinPartSystem.GetMouth(mouth, Gender == 0);
		if (mouth2 == null)
		{
			Debug.LogWarning("Trying to set unknown mouth for avatar");
			return false;
		}
		Mouth = mouth;
		BodyPackManager.Mouth = mouth2;
		return true;
	}

	public bool ChangeEyes(string eyes)
	{
		MilMo_SkinPart eyes2 = MilMo_SkinPartSystem.GetEyes(eyes, Gender == 0);
		if (eyes2 == null)
		{
			Debug.LogWarning("Trying to set unknown eyes '" + eyes + "' for avatar");
			return false;
		}
		Eyes = eyes;
		BodyPackManager.Eyes = eyes2;
		return true;
	}

	public bool ChangeEyeBrows(string eyebrows)
	{
		MilMo_SkinPart eyeBrows = MilMo_SkinPartSystem.GetEyeBrows(eyebrows, Gender == 0);
		if (eyeBrows == null)
		{
			Debug.LogWarning("Trying to set unknown eye brows '" + eyebrows + "' for avatar");
			return false;
		}
		EyeBrows = eyebrows;
		BodyPackManager.EyeBrows = eyeBrows;
		return true;
	}

	public void ChangeHeight(float height)
	{
		Height = height;
		GameObject.transform.localScale = new Vector3(height, height, height);
		if (!(Controller == null))
		{
			Controller.height = DefaultControllerHeight;
			Controller.radius = DefaultControllerRadius;
			Controller.center = DefaultControllerCenter;
		}
	}

	public void SetAlpha(float alpha)
	{
		Renderer.materials[0].SetColor(Alpha, new Color(1f, 1f, 1f, alpha));
		Renderer.materials[1].SetColor(Alpha, new Color(1f, 1f, 1f, alpha));
	}

	public void AsyncApply(AvatarCallback callback = null, string userTag = "")
	{
		if (BodyPackManager == null)
		{
			callback?.Invoke(this, userTag);
			return;
		}
		BodyPackManager.AsyncApply(GetPriority(), delegate
		{
			callback?.Invoke(this, userTag);
		});
	}

	private void LoadAvatarInfo(Code.Core.Network.types.Avatar avatar)
	{
		Name = avatar.GetName();
		Gender = (byte)avatar.GetGender();
		SkinColor = avatar.GetSkinColor();
		HairColor = avatar.GetHairColor();
		EyeColor = avatar.GetEyeColor();
		Mouth = avatar.GetMouth();
		Eyes = avatar.GetEyes();
		EyeBrows = avatar.GetEyeBrows();
		Height = avatar.GetHeight();
		if (!_thumbnailMode)
		{
			Health = avatar.GetHealth();
			MaxHealth = avatar.GetMaxHealth();
		}
		Mood = avatar.GetMood();
		_avatarLevel = avatar.GetAvatarLevel();
		TeleportStones = avatar.GetTeleportStones();
		if (MilMo_ColorSystem.GetSkinColor(SkinColor) == null)
		{
			Debug.LogWarning("Got non existing skin color '" + SkinColor + "' when loading avatar info");
		}
		if (MilMo_ColorSystem.GetEyeColor(EyeColor) == null)
		{
			Debug.LogWarning("Got non existing eye color '" + EyeColor + "' when loading avatar info");
		}
		_items = avatar.GetItems();
	}

	private async Task<List<MilMo_Wearable>> LoadWearables()
	{
		List<MilMo_Wearable> wearables = new List<MilMo_Wearable>();
		foreach (Item itemRef in _items)
		{
			if (await Singleton<MilMo_TemplateContainer>.Instance.GetTemplateAsync(itemRef.GetTemplate()) is MilMo_WearableTemplate milMo_WearableTemplate)
			{
				Dictionary<string, string> dictionary = MilMo_Item.ReadModifiers(itemRef.GetModifiers());
				MilMo_Wearable milMo_Wearable = milMo_WearableTemplate.Instantiate(dictionary) as MilMo_Wearable;
				wearables.Add(milMo_Wearable);
				if (dictionary.Keys.Any((string modifierKey) => modifierKey.Equals("ColorGroup:Hair", StringComparison.InvariantCultureIgnoreCase)))
				{
					Hair = milMo_Wearable;
				}
			}
		}
		return wearables;
	}

	private void SetupBodyPack()
	{
		BodyPackManager.SetGender(Gender == 0);
		BodyPackManager.Renderer = Renderer;
	}

	private void EquipAllWearables(List<MilMo_Wearable> wearables)
	{
		foreach (MilMo_Wearable wearable in wearables)
		{
			EquipLocal(wearable);
		}
	}

	private void InitBodyPackManager()
	{
		BodyPackManager.AsyncInit(GetPriority(), Renderer, Gender == 0, SkinColor, HairColor, EyeColor, Mouth, Eyes, EyeBrows, Height, InitFinish);
	}

	public void WieldFromNetworkMessage(Item itemStruct)
	{
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(itemStruct.GetTemplate(), delegate(MilMo_Template template, bool timeOut)
		{
			if (!timeOut)
			{
				if (!(template is MilMo_WieldableTemplate milMo_WieldableTemplate))
				{
					Debug.LogWarning("Got item to wield that does not exist in template database");
				}
				else
				{
					Dictionary<string, string> modifiers = MilMo_Item.ReadModifiers(itemStruct.GetModifiers());
					SetWieldedItem(milMo_WieldableTemplate.Instantiate(modifiers) as MilMo_Wieldable);
					WieldInternal(applyBodyPack: true);
				}
			}
		});
	}

	public void Unwield()
	{
		if (_wieldedItem != null)
		{
			BodyPackManager.Unequip(_wieldedItem.BodyPack);
			BodyPackManager.AsyncApply(GetPriority(), null);
			SetWieldedItem(null);
		}
	}

	private void WieldInternal(bool applyBodyPack)
	{
		if (_wieldedItem != null && BodyPackManager != null && !(Renderer == null) && _wieldedItem.BodyPack != null && applyBodyPack)
		{
			ApplyWieldItemBodypack();
		}
	}

	private void ApplyWieldItemBodypack()
	{
		if (_wieldedItem != null && !InHappyPickup)
		{
			BodyPackManager.Equip(_wieldedItem.BodyPack, _wieldedItem.ColorIndices);
			BodyPackManager.AsyncApply(GetPriority(), null);
		}
	}

	public void Wield(MilMo_Wieldable item, bool applyBodyPack)
	{
		SetWieldedItem(item);
		WieldInternal(applyBodyPack);
	}

	private void LoadGameObject()
	{
		if ((bool)GameObject)
		{
			UnityEngine.Object.Destroy(GameObject);
		}
		GameObject = UnityEngine.Object.Instantiate((Gender == 0) ? MilMo_BodyPackSystem.MaleGameObject : MilMo_BodyPackSystem.FemaleGameObject);
		GameObject.SetActive(value: false);
		GameObject.transform.localScale = new Vector3(Height, Height, Height);
		Renderer = GameObject.GetComponentInChildren<SkinnedMeshRenderer>();
		Renderer.gameObject.SetActive(value: false);
		EnableRefraction();
		Renderer.quality = SkinQuality.Auto;
		Renderer.updateWhenOffscreen = false;
		Quality = MilMo_AvatarQuality.Unknown;
		AudioSourceWrapper = GameObject.AddComponent<AudioSourceWrapper>();
		Head = GameObject.transform.Find("Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase/NeckBaseUp/NeckLo/NeckTip/NodeHeadTip");
		HeadTip = GameObject.transform.Find("Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase/NeckBaseUp/NeckLo/NeckTip/SkullTip");
		_neckBase = GameObject.transform.Find("Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase/NeckBaseUp/NeckLo");
		Ass = GameObject.transform.Find("Root");
		RightHand = GameObject.transform.Find("Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase/NeckBaseUp/RClavicle/RShoulder/RElbow/RWrist");
		LeftHand = GameObject.transform.Find("Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase/NeckBaseUp/LClavicle/LShoulder/LElbow/LWrist");
		_rightFoot = GameObject.transform.Find("Root/RHip/RKnee/RAnkle");
		_leftFoot = GameObject.transform.Find("Root/LHip/LKnee/LAnkle");
		SpineStart = GameObject.transform.Find("Root/SpineLumbarBase/SpineLumbarLo");
		SpineEnd = GameObject.transform.Find("Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase");
		HeadStart = GameObject.transform.Find("Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase/NeckBaseUp/NeckLo");
		HeadEnd = GameObject.transform.Find("Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase/NeckBaseUp/NeckLo/NeckTip");
		LeftShoulder = GameObject.transform.Find("Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase/NeckBaseUp/LClavicle/LShoulder");
		RightShoulder = GameObject.transform.Find("Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase/NeckBaseUp/RClavicle/RShoulder");
		if (!Head)
		{
			Debug.LogWarning("MY HEAD IS GONE. It should be at Root/SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase/NeckBaseUp/NeckLo/NeckTip/NodeHeadTip");
		}
		Controller = GameObject.AddComponent<CharacterController>();
		Controller.radius = DefaultControllerRadius;
		Controller.height = DefaultControllerHeight;
		Controller.center = DefaultControllerCenter;
		Controller.stepOffset = 0.2f;
		Controller.slopeLimit = 60f;
		Controller.gameObject.layer = 28;
		if ((bool)ClickGameObject)
		{
			UnityEngine.Object.Destroy(ClickGameObject);
		}
		ClickGameObject = new GameObject(GameObject.name + " ClickCollider")
		{
			layer = 29
		};
		ClickGameObject.transform.parent = GameObject.transform;
		CapsuleCollider capsuleCollider = ClickGameObject.AddComponent<CapsuleCollider>();
		if ((bool)capsuleCollider)
		{
			capsuleCollider.isTrigger = true;
			capsuleCollider.radius = 0.4f;
			capsuleCollider.height = 1.3f;
			capsuleCollider.center = DefaultBaseControllerCenter;
		}
		_blobShadowHandler?.Init(this);
		_eyeSpecManager.Initialize(EmoteManager, GameObject);
		_superAlivenessManager.Init(this, MilMo_SuperAlivenessManager.LodLevel.Enabled);
		if (IsTheLocalPlayer)
		{
			MilMo_EventSystem.Instance.PostEvent("local_avatar_reloaded", this);
		}
	}

	public void LoadPuffsForGameObject()
	{
		_puffs = new MilMo_Effect[SurfacePuffIndices.Count][];
		for (int i = 0; i < SurfacePuffIndices.Count; i++)
		{
			_puffs[i] = new MilMo_Effect[ActionPuffIndices.Count];
		}
		foreach (string key in SurfacePuffIndices.Keys)
		{
			foreach (string key2 in ActionPuffIndices.Keys)
			{
				if (!(key != "Water") || (!(key2 == "SurfaceSwim") && !(key2 == "SurfaceIdle")))
				{
					_puffs[SurfacePuffIndices[key]][ActionPuffIndices[key2]] = MilMo_EffectContainer.GetEffect(key + "Puff" + key2, GameObject, warnIfNotFound: false);
				}
				else
				{
					_puffs[SurfacePuffIndices[key]][ActionPuffIndices[key2]] = null;
				}
			}
		}
	}

	private void LoadDamageStarsForGameObject()
	{
		if (_damageEffect == null)
		{
			_damageEffect = new MilMo_ParticleDamageEffect(GameObject.transform, Height);
		}
	}

	private void InitFinish()
	{
		Debug.Log("Init finished for " + Name);
		if (BodyPackManager == null || BodyPackManager.IsUnloaded())
		{
			Destroy();
			return;
		}
		Renderer.gameObject.SetActive(value: true);
		GameObject.SetActive(value: true);
		MilMo_AudioUtils.SetRollOffFactor(AudioSourceWrapper, 0.2f);
		_blinkTimer = UnityEngine.Random.Range(RandomBlinkInterval.x, RandomBlinkInterval.y);
		_idleVariationTimer = UnityEngine.Random.Range(RandomIdleVariationInterval.x, RandomIdleVariationInterval.y);
		EmoteManager.Init(Renderer, PlayAnimation);
		_animationManager.Init(GameObject, _thumbnailMode);
		MoodHandler?.SetInitialMood(Mood);
		EmoteManager.SetMoodByName(Mood);
		EmoteManager.ForceMood();
		EmoteManager.ForceOneUpdate();
		if (IsSitting)
		{
			StackAnimation("LandIdle", SitPose);
		}
		PlayAnimation("Idle");
		if (!_thumbnailMode)
		{
			MilMo_EventSystem.Instance.PostEvent("avatar_loaded", this);
		}
		if (!Enabled)
		{
			Disable();
		}
		_initializeCallback?.Invoke(this, _initializeUserTag);
		_materialColor = Renderer.material.color;
		_boundingBox = Renderer.bounds;
		_boundingBox.center = GameObject.transform.InverseTransformPoint(_boundingBox.center);
		if (_marker == null && !_thumbnailMode)
		{
			_marker = WorldSpaceManager.GetWorldSpaceObject<PlayerMarker>("PlayerMarker");
			_marker.Initialize(this);
		}
	}

	public IEnumerable<BaseBadge> GetAllBadges()
	{
		return BadgeManager.GetAll();
	}

	public void InitLocal(string name, byte gender, string skinColor, string eyeColor, int hairColor, string mouth, string eyes, string eyebrows, MilMo_Wearable hairStyle, float height)
	{
		Name = name;
		Gender = gender;
		SkinColor = skinColor;
		EyeColor = eyeColor;
		HairColor = hairColor;
		Mouth = mouth;
		Eyes = eyes;
		EyeBrows = eyebrows;
		Hair = hairStyle;
		Height = height;
		LoadGameObject();
		SetupBodyPack();
		if (Hair != null)
		{
			Hair.UpdateColorIndex(Hair.BodyPack.Path + ":Hair", HairColor);
			EquipLocal(Hair);
		}
		InitBodyPackManager();
	}

	public MilMo_Effect GetPuff(string action, string surface)
	{
		if (action == null || surface == null)
		{
			return null;
		}
		if (ActionPuffIndices.TryGetValue(action, out var value) && SurfacePuffIndices.TryGetValue(surface, out var value2))
		{
			return _puffs[value2][value];
		}
		return null;
	}

	public void ChangePuff(string action, string surface, MilMo_Effect newPuff)
	{
		if (action != null && surface != null && ActionPuffIndices.TryGetValue(action, out var value) && SurfacePuffIndices.TryGetValue(surface, out var value2))
		{
			if (_puffs[value2][value] != null)
			{
				_puffs[value2][value].Stop();
				_puffs[value2][value].Destroy();
			}
			_puffs[value2][value] = newPuff;
			if (surface == "Water" && _hasWaterEffect && _currentPlayingAnimation == action)
			{
				newPuff.Restart(WaterSurfaceHeight + 0.01f);
			}
		}
	}

	private void UpdatePuffs()
	{
		if (MilMo_Instance.CurrentInstance == null || _puffs == null)
		{
			return;
		}
		string text = _animationManager.GetCurrentVisibleLoopingAnimation();
		if (text.StartsWith("Run"))
		{
			text = "Run";
		}
		if (text.StartsWith("Walk"))
		{
			text = "Walk";
		}
		if (!string.IsNullOrEmpty(CurrentGroundType))
		{
			if ((text != "Run" && text != "Walk" && text != "SurfaceSwim" && text != "SurfaceIdle") || !SurfacePuffIndices.TryGetValue(CurrentGroundType, out var value) || !ActionPuffIndices.TryGetValue(text, out var value2))
			{
				return;
			}
			if (_puffs[value][value2] != null)
			{
				float animationTime = _animationManager.GetAnimationTime();
				if (CurrentGroundType == "Water" && text != "Walk" && text != "Run")
				{
					if (!_hasWaterEffect)
					{
						ChangeWaterEffect(text, CurrentGroundType);
					}
				}
				else if (_hasWaterEffect && CurrentGroundType != "Water")
				{
					ChangeWaterEffect(text, CurrentGroundType);
				}
				_puffs[value][value2].Update(animationTime);
			}
			else if (_hasWaterEffect)
			{
				ChangeWaterEffect(text, CurrentGroundType);
			}
		}
		else if (_hasWaterEffect)
		{
			ChangeWaterEffect(text, CurrentGroundType);
		}
	}

	private void UpdateDamageEffect()
	{
		Color materialColor = _materialColor;
		materialColor.r = Mathf.Lerp(Renderer.material.color.r, _materialColor.r, 5f * Time.deltaTime);
		materialColor.g = Mathf.Lerp(Renderer.material.color.g, _materialColor.g, 5f * Time.deltaTime);
		materialColor.b = Mathf.Lerp(Renderer.material.color.b, _materialColor.b, 5f * Time.deltaTime);
		Renderer.material.color = materialColor;
		if ((double)Mathf.Abs(Renderer.material.color.r - _materialColor.r) < 0.001 && (double)Mathf.Abs(Renderer.material.color.g - _materialColor.g) < 0.001 && (double)Mathf.Abs(Renderer.material.color.b - _materialColor.b) < 0.001)
		{
			Renderer.material.color = _materialColor;
		}
	}

	private MilMo_Priority GetPriority()
	{
		if (IsTheLocalPlayer)
		{
			return MilMo_Priority.High;
		}
		if (!GameObject)
		{
			return MilMo_Priority.Low;
		}
		if (!((MilMo_Global.MainCamera.transform.position - GameObject.transform.position).sqrMagnitude < 100f))
		{
			return MilMo_Priority.Low;
		}
		return MilMo_Priority.Normal;
	}

	public float GetDamage(List<MilMo_Damage> damage)
	{
		return damage.Sum((MilMo_Damage d) => d.Value);
	}

	public bool IsDeadOrDying()
	{
		return Health <= 0f;
	}

	public bool HasKnockBack()
	{
		return true;
	}

	public void Damage(float damage)
	{
		UpdateHealth(Health - damage);
	}

	public bool IsDangerous()
	{
		return true;
	}

	public bool IsBoss()
	{
		return false;
	}

	public void Target()
	{
		if (_marker != null)
		{
			_marker.ShowTargetArrow(shouldShow: true);
		}
	}

	public void UnTarget()
	{
		if (_marker != null)
		{
			_marker.ShowTargetArrow(shouldShow: false);
		}
	}

	public void DamageEffectLocal(MilMo_Avatar attacker, MilMo_MeleeWeapon hitWeapon, float damage)
	{
		Damaged(damage, attacker.Position);
		PlaySoundEffect("Content/Sounds/Batch01/Combat/Slap01");
		if (hitWeapon.Template.Impact > 0f && !(Id != MilMo_Player.Instance.Id))
		{
			MilMo_PlayerControllerBase.AddKnockBack(((!MilMo_Utility.Equals(Position, attacker.Position)) ? (Position - attacker.Position).normalized : GameObject.transform.forward) * (hitWeapon.Template.Impact * 20f));
		}
	}

	public void DamageEffectLocal(MilMo_LevelProjectile projectile, float damage)
	{
		Damaged(damage, projectile.Position);
	}

	public void DamageEffectLocal(float damage)
	{
		Damaged(damage, Position);
	}

	public void Kill()
	{
	}

	public AttackTarget AsNetworkAttackTarget()
	{
		return new PlayerTarget(Id);
	}

	public void AddStateEffect(string particleEffect)
	{
		AddParticleEffect(MilMo_EffectContainer.GetEffect(particleEffect, GameObject));
	}

	public void RemoveStateEffect(string particleEffect)
	{
		StopParticleEffect(particleEffect);
	}

	public MilMo_EntityStateManager GetEntityStateManager()
	{
		return EntityStateManager;
	}
}
