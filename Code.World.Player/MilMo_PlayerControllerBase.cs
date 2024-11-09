using Code.Core.Avatar;
using Code.Core.Camera;
using Code.Core.Collision;
using Code.Core.EventSystem;
using Code.Core.Input;
using Code.Core.Items;
using Code.Core.Network;
using Code.Core.Network.messages.client;
using Code.Core.Network.types;
using Code.Core.Utility;
using Code.Core.Visual.Water;
using Code.World.Level;
using Code.World.Level.LevelObject;
using Core;
using Core.Interaction;
using UI.HUD.States;
using UnityEngine;

namespace Code.World.Player;

public abstract class MilMo_PlayerControllerBase
{
	protected enum MovementStates
	{
		Idle,
		Forward,
		Climb,
		FurniturePush,
		FurniturePull,
		FurnitureStrafeLeft,
		FurnitureStrafeRight,
		FurnitureWallStrafeLeft,
		FurnitureWallStrafeRight
	}

	public enum ControllerType
	{
		Game,
		Chat,
		Social,
		SplineRide,
		Climb,
		InGUIApp,
		Home
	}

	public delegate void UnlockCallback();

	protected static readonly MilMo_Player Player;

	protected static readonly MilMo_ObjectMover WaterMover;

	private float _lastWrongSoundTime = -1000f;

	protected static float RotationSpeed;

	protected static float AccelerationSpeedX;

	protected static float AccelerationSpeedZ;

	protected static float DecelerationSpeedX;

	protected static float DecelerationSpeedZ;

	protected static float ImpulseFalloffSpeed;

	protected static float MinStandingVelocity;

	protected static float HardImpactVelocityLimit;

	protected static float WaterPull;

	protected static float WaterDrag;

	protected static float WaterImpactFactor;

	protected static string WalkAnimation;

	protected static string SwimAnimation;

	protected static string IdleAnimation;

	protected static string WaterIdleAnimation;

	protected static bool RunMode;

	protected static MovementStates MovementState;

	protected static float PreviousVelocityY;

	protected static float PreviousHeightAboveGround;

	protected static Vector3 TargetSpeed;

	protected static bool TargetLocked;

	protected static bool WasGrounded;

	protected static bool WasInWater;

	protected static bool HasLockingImpulse;

	protected static float WaterSurfaceHeight;

	private static Vector3 _impulseVelocity;

	protected static float CurrentMoveSpeed;

	protected static float TemporarySpeedModifier;

	protected static bool HadMoveInput;

	protected static bool WasMovedByPlayform;

	protected static bool DidJump;

	protected static bool LastCollisionMoveFailed;

	protected static float CollisionMoveStartFailTime;

	protected static Transform LookAtTarget;

	protected static bool PlayMoveAnimationOnUnlock;

	protected static MilMo_TimerEvent UnlockReaction;

	protected static MilMo_TimerEvent RemoveLookatTargetEvent;

	protected static MilMo_TimerEvent RemoveSpeedModifierEvent;

	private static MilMo_GenericReaction _toggleGUIListener;

	private static readonly InteractionManager InteractionManager;

	private static IMilMo_AttackTarget _currentTarget;

	protected static Vector3 ImpulseVelocity
	{
		get
		{
			return _impulseVelocity;
		}
		set
		{
			_impulseVelocity = value;
			if (_impulseVelocity == Vector3.zero)
			{
				Singleton<GameNetwork>.Instance.SendToGameServer(new ClientUpdateKnockBackState(0));
			}
		}
	}

	public abstract ControllerType Type { get; }

	public static float WalkSpeed
	{
		get
		{
			return Player.Avatar.GetVariableValue("WalkSpeed");
		}
		set
		{
			Player.Avatar.SetVariableValue("WalkSpeed", value);
			SetMovementMode();
		}
	}

	public static float RunSpeed
	{
		get
		{
			return Player.Avatar.GetVariableValue("RunSpeed");
		}
		set
		{
			Player.Avatar.SetVariableValue("RunSpeed", value);
			SetMovementMode();
		}
	}

	public static float WalkJumpSpeed
	{
		get
		{
			return Player.Avatar.GetVariableValue("WalkJumpSpeed");
		}
		set
		{
			Player.Avatar.SetVariableValue("WalkJumpSpeed", value);
		}
	}

	public static float RunJumpSpeed
	{
		get
		{
			return Player.Avatar.GetVariableValue("RunJumpSpeed");
		}
		set
		{
			Player.Avatar.SetVariableValue("RunJumpSpeed", value);
		}
	}

	public static float SwimSpeed
	{
		get
		{
			return Player.Avatar.GetVariableValue("SwimSpeed");
		}
		set
		{
			Player.Avatar.SetVariableValue("SwimSpeed", value);
			SetMovementMode();
		}
	}

	public static string RunAnimation { get; set; }

	protected static float IsOnGroundTolerance { get; private set; }

	private static float StartHoverAnimationLimit { get; }

	private static float StartLandingAnimationLimit { get; }

	protected static float MinVelocityY { get; }

	protected static bool IsLocked { get; private set; }

	protected static bool IsOnTheGround => Player.Avatar.IsGrounded;

	public static Quaternion TargetRotation { get; set; }

	protected static bool IsTurning { get; set; }

	public static bool IsInWater { get; private set; }

	public static Vector3 CurrentSpeed { get; set; }

	static MilMo_PlayerControllerBase()
	{
		WaterMover = new MilMo_ObjectMover();
		RotationSpeed = 15f;
		AccelerationSpeedX = 5f;
		AccelerationSpeedZ = 5f;
		DecelerationSpeedX = 10f;
		DecelerationSpeedZ = 10f;
		ImpulseFalloffSpeed = 3f;
		MinStandingVelocity = -3.112f;
		HardImpactVelocityLimit = -7f;
		WaterPull = 0.03f;
		WaterDrag = 0.95f;
		WaterImpactFactor = 0.05f;
		WalkAnimation = "Walk";
		SwimAnimation = "Swim";
		IdleAnimation = "LandIdle";
		WaterIdleAnimation = "WaterIdle";
		RunMode = true;
		MovementState = MovementStates.Idle;
		TargetSpeed = Vector3.zero;
		TargetLocked = false;
		WasGrounded = true;
		_impulseVelocity = Vector3.zero;
		CurrentMoveSpeed = 7.5f;
		TemporarySpeedModifier = 1f;
		InteractionManager = InteractionManager.Get();
		CurrentSpeed = Vector3.zero;
		MinVelocityY = -1000f;
		StartLandingAnimationLimit = -1f;
		StartHoverAnimationLimit = 0.573f;
		IsOnGroundTolerance = 0.191f;
		RunAnimation = "Run";
		Player = MilMo_Player.Instance;
		WaterMover.SetUpdateFunc(2);
		WaterMover.Drag = WaterDrag;
		WaterMover.Pull = WaterPull;
		MilMo_CameraController.ThePlayerLockedCallback = PlayerLocked;
		_toggleGUIListener = MilMo_EventSystem.Listen("button_ToggleGUI", delegate
		{
			if ((bool)MilMo_World.Instance && MilMo_World.Instance.PlayerController.Type == ControllerType.InGUIApp)
			{
				MilMo_World.Instance.ChangePlayerController(MilMo_PlayerControllerInGUIApp.PreviousControllerType);
				MilMo_Pointer.GamepadPointer = false;
			}
			else
			{
				MilMo_Pointer.PointerIcon.SetEnabled(e: true);
				MilMo_World.Instance.ChangePlayerController(ControllerType.InGUIApp);
				MilMo_Pointer.GamepadPointer = true;
			}
		});
		_toggleGUIListener.Repeating = true;
	}

	private static bool PlayerLocked()
	{
		return IsLocked;
	}

	public virtual void UpdatePlayer()
	{
		if (Player != null)
		{
			Player.Update();
			HadMoveInput = !IsLocked && (MilMo_Input.VerticalAxis != 0f || MilMo_Input.HorizontalAxis != 0f);
			UpdateCombatTarget();
		}
	}

	public void LateUpdatePlayer()
	{
		if (Player != null && Player.Avatar != null)
		{
			Player.Avatar.LateUpdate();
		}
	}

	protected static void ToggleRunWalk(object o = null)
	{
		RunMode = !RunMode;
		SetMovementMode();
		if (MovementState == MovementStates.Forward)
		{
			PlayMoveAnimation();
		}
	}

	protected static void UpdateGroundTypeConfig()
	{
		if (Player.Avatar.GroundConfigChanged)
		{
			if (Player.Avatar.OnTerrain)
			{
				IsOnGroundTolerance = ((Player.Avatar.CurrentGroundType == "Rock") ? 0.24f : 0.24f);
			}
			else
			{
				IsOnGroundTolerance = 0.24f;
			}
		}
	}

	protected static void PlayMoveAnimation()
	{
		if (MilMo_Player.Instance.IsDone)
		{
			if (MovementState == MovementStates.Forward)
			{
				Player.Avatar.PlayAnimation(Player.Avatar.MoveAnimation);
				Player.Avatar.PlayParticleEffect(Player.Avatar.MoveParticle);
			}
			else
			{
				Player.Avatar.PlayAnimation(Player.Avatar.IdleAnimation);
				Player.Avatar.StopParticleEffect(Player.Avatar.MoveParticle);
			}
		}
	}

	protected void Stop()
	{
		HasLockingImpulse = false;
		TargetSpeed.z = 0f;
		TargetSpeed.x = 0f;
		if (WasGrounded || IsOnTheGround || IsInWater)
		{
			Player.Avatar.PlayAnimation(Player.Avatar.IdleAnimation);
		}
		MovementState = MovementStates.Idle;
	}

	public static void FixedUpdate()
	{
		Player.Avatar.FixedUpdate();
		WaterMover.Update();
	}

	public static void AddKnockBack(Vector3 impulse)
	{
		Singleton<GameNetwork>.Instance.SendToGameServer(new ClientUpdateKnockBackState(1));
		Vector3 currentSpeed = CurrentSpeed;
		currentSpeed.y = impulse.y;
		impulse.y = 0f;
		ImpulseVelocity = impulse;
		CurrentSpeed = currentSpeed;
	}

	public static void SetRotation(Quaternion rotation)
	{
		Player.Avatar.GameObject.transform.rotation = rotation;
		TargetRotation = rotation;
	}

	public static void Teleport(Vector3 position, Quaternion rotation)
	{
		if (Player.EnteringOrLeaving || Player.InGUIApp)
		{
			MilMo_World.Instance.returnPosition = position;
			MilMo_World.Instance.returnRotation = rotation;
		}
		else
		{
			SetRotation(rotation);
			SetPosition(position);
		}
	}

	public static void SetPosition(Vector3 position)
	{
		Player.Avatar.GameObject.transform.position = position;
	}

	private static void UpdateCombatTarget()
	{
		if (MilMo_Level.CurrentLevel == null || ((bool)MilMo_World.HudHandler && MilMo_World.HudHandler.hudState != HudState.States.Pvp && HadMoveInput && !Player.Avatar.InCombat))
		{
			return;
		}
		if (Player.Avatar.WieldedItem is MilMo_Weapon milMo_Weapon)
		{
			IMilMo_AttackTarget closestTarget = MilMo_Level.CurrentLevel.GetClosestTarget(Player.Avatar.Position, Player.Avatar.GameObject.transform.forward, milMo_Weapon.Template, useHitRange: true);
			if (closestTarget != _currentTarget)
			{
				_currentTarget?.UnTarget();
				_currentTarget = closestTarget;
				_currentTarget?.Target();
			}
		}
		else
		{
			_currentTarget?.UnTarget();
		}
	}

	protected void CheckWater(ref Vector3 newPos)
	{
		float surfaceY = 0f;
		Vector3 position = newPos;
		if (!WaterMover.Paused)
		{
			position.y -= WaterMover.Pos.y;
		}
		MilMo_WaterManager.WaterLevel waterLevel = ((MilMo_Level.CurrentLevel != null) ? MilMo_WaterManager.GetWaterLevel(position, out surfaceY) : MilMo_WaterManager.WaterLevel.Land);
		if (waterLevel == MilMo_WaterManager.WaterLevel.Deep != IsInWater)
		{
			if (!IsInWater)
			{
				Player.Avatar.WaterSurfaceHeight = surfaceY;
				EnterWater(ref newPos);
			}
			else if (ExitWater(ref newPos))
			{
				Player.Avatar.WaterSurfaceHeight = surfaceY;
			}
		}
		else if (waterLevel == MilMo_WaterManager.WaterLevel.Deep)
		{
			Player.Avatar.WaterSurfaceHeight = surfaceY;
			WaterSurfaceHeight = Player.Avatar.WaterSurfaceHeight;
			newPos.y = Player.Avatar.WaterSurfaceHeight;
		}
	}

	protected virtual void EnterWater(ref Vector3 newPos)
	{
		IsInWater = true;
		SetMovementMode();
		PlayMoveAnimation();
		WaterSurfaceHeight = Player.Avatar.WaterSurfaceHeight;
		newPos.y = Player.Avatar.WaterSurfaceHeight;
		float num = (Player.Avatar.IsGrounded ? 1f : CurrentSpeed.y);
		if (num > HardImpactVelocityLimit)
		{
			Player.Avatar.EmitPuff("SoftImpact", "Water", WaterSurfaceHeight);
			Player.Avatar.PlaySoundEffect("Content/Sounds/Batch01/Surface/Impact/WaterSplashSoft");
		}
		else
		{
			Player.Avatar.EmitPuff("HardImpact", "Water", WaterSurfaceHeight);
			Player.Avatar.PlaySoundEffect("Content/Sounds/Batch01/Surface/Impact/WaterSplashHard");
		}
		WaterMover.Vel = new Vector3(0f, Mathf.Clamp(num * WaterImpactFactor, -0.5f, 0.5f), 0f);
		WaterMover.UnPause();
		Player.Avatar.Controller.center = MilMo_Avatar.SwimControllerCenter;
		Player.Avatar.Controller.height = Player.Avatar.SwimControllerHeight;
	}

	protected virtual bool ExitWater(ref Vector3 newPos)
	{
		Vector3 position = newPos;
		float distanceToGround = MilMo_Physics.GetDistanceToGround(newPos);
		position.y -= distanceToGround;
		if (MilMo_Level.CurrentLevel != null && MilMo_WaterManager.GetWaterLevel(position, out var _) == MilMo_WaterManager.WaterLevel.Deep)
		{
			return false;
		}
		IsInWater = false;
		WasInWater = true;
		SetMovementMode();
		PlayMoveAnimation();
		WaterMover.Vel = Vector3.zero;
		WaterMover.Pos = Vector3.zero;
		WaterMover.Pause();
		Player.Avatar.Controller.center = Player.Avatar.DefaultControllerCenter;
		Player.Avatar.Controller.height = Player.Avatar.DefaultControllerHeight;
		Player.Avatar.PlaySoundEffect("Content/Sounds/Batch01/Surface/Exit/WaterExit");
		return true;
	}

	protected void AttackOrUseClickedUsable(object o)
	{
		if (!InteractionManager.HasCurrentInteractable || Cursor.lockState == CursorLockMode.Locked || !InteractionManager.InteractWithMouse())
		{
			Attack(null);
		}
	}

	protected void Attack(object o)
	{
		if (IsLocked || Player.IsExhausted || IsInWater)
		{
			return;
		}
		if (Player.Avatar.WieldedItem != null)
		{
			if (Player.Avatar.WieldedItem is MilMo_Weapon && Player.NpCsInRange == 0 && Player.Avatar.WieldedItem.CanUse())
			{
				Bash();
			}
			else if (Player.Avatar.WieldedItem.IsFood() && Player.Avatar.WieldedItem.CanUse())
			{
				Eat();
			}
			else if (Player.Avatar.WieldedItem.Template.Identifier == "Item:BasicShovel" && Player.Avatar.WieldedItem.CanUse())
			{
				Dig();
			}
		}
		else if (!MilMo_Player.InHome && !(_lastWrongSoundTime < Time.time + 1f))
		{
			Player.Avatar.PlaySoundEffect("Content/Sounds/Batch01/Core/Wrong");
			_lastWrongSoundTime = Time.time;
		}
	}

	protected void UpdateY(ref Vector3 pos)
	{
		if (IsInWater)
		{
			if (HasLockingImpulse)
			{
				HasLockingImpulse = false;
			}
			pos = Player.Avatar.Position;
			if (!WaterMover.Paused)
			{
				pos.y = WaterSurfaceHeight + WaterMover.Pos.y;
			}
			else
			{
				pos.y = WaterSurfaceHeight;
			}
			if ((double)Mathf.Abs(WaterMover.Vel.y) < 0.01 && (double)Mathf.Abs(PreviousVelocityY) >= 0.01 && (double)Mathf.Abs(WaterMover.Pos.y) < 0.1)
			{
				PlayMoveAnimation();
			}
			PreviousHeightAboveGround = pos.y - WaterSurfaceHeight;
			PreviousVelocityY = WaterMover.Vel.y;
			Player.Avatar.GameObject.transform.position = pos;
			return;
		}
		float distanceToGround = MilMo_Physics.GetDistanceToGround(pos);
		float num = Player.Avatar.GameObject.transform.position.y - distanceToGround;
		bool num2 = distanceToGround <= IsOnGroundTolerance || IsOnTheGround;
		pos.y += CurrentSpeed.y * Time.deltaTime;
		if (num - pos.y < 20f)
		{
			pos.y = Mathf.Max(num, pos.y);
		}
		if (num2)
		{
			if (!WasGrounded)
			{
				if (HasLockingImpulse)
				{
					HasLockingImpulse = false;
				}
				PlayMoveAnimation();
				DidJump = false;
				if (!WasInWater)
				{
					Player.Avatar.StopParticleEffect(Player.Avatar.JumpParticle);
					if (CurrentSpeed.y < -0.1f)
					{
						Player.Avatar.EmitPuff((CurrentSpeed.y > HardImpactVelocityLimit) ? "SoftImpact" : "HardImpact");
					}
				}
				else
				{
					WasInWater = false;
				}
			}
			Vector3 currentSpeed = CurrentSpeed;
			currentSpeed.y += -7.64f * Time.deltaTime;
			currentSpeed.y = Mathf.Max(currentSpeed.y, MinStandingVelocity);
			CurrentSpeed = currentSpeed;
		}
		else
		{
			if (!WasMovedByPlayform || DidJump)
			{
				if ((WasGrounded || PreviousVelocityY <= 0f) && CurrentSpeed.y > 0f)
				{
					Player.Avatar.PlayAnimation(Player.Avatar.JumpAnimation);
				}
				else if (PreviousVelocityY > StartHoverAnimationLimit && CurrentSpeed.y <= StartHoverAnimationLimit)
				{
					Player.Avatar.PlayAnimation(Player.Avatar.HoverAnimation);
				}
				else if ((PreviousVelocityY > StartLandingAnimationLimit && CurrentSpeed.y <= StartLandingAnimationLimit) || WasGrounded)
				{
					Player.Avatar.PlayAnimation(Player.Avatar.FallAnimation);
				}
			}
			PreviousVelocityY = CurrentSpeed.y;
			Vector3 currentSpeed2 = CurrentSpeed;
			currentSpeed2.y += -7.64f * Time.deltaTime;
			currentSpeed2.y = Mathf.Max(currentSpeed2.y, MinVelocityY);
			CurrentSpeed = currentSpeed2;
		}
		PreviousHeightAboveGround = distanceToGround;
		WasGrounded = num2;
	}

	public static void StartMovingPlatformFrame()
	{
		WasMovedByPlayform = false;
	}

	public static void PlatformMovePlayer(Vector3 pos, bool printDebug)
	{
		Vector3 position;
		Vector3 vector = (position = MilMo_Player.Instance.Avatar.GameObject.transform.position);
		Collision(pos, printDebug);
		Vector3 b = vector;
		if (!MilMo_Utility.Equals(position, b))
		{
			WasMovedByPlayform = true;
		}
	}

	protected static void Collision(Vector3 pos, bool printDebug = false)
	{
		if (MilMo_Player.Instance.Avatar.IsRagdollActive)
		{
			LastCollisionMoveFailed = false;
			return;
		}
		Vector3 position = MilMo_Player.Instance.Avatar.GameObject.transform.position;
		Vector3 vector = pos - position;
		if (MilMo_Utility.Equals(vector, Vector3.zero))
		{
			LastCollisionMoveFailed = false;
			return;
		}
		CollisionFlags collisionFlags = MilMo_Player.Instance.Avatar.Controller.Move(vector);
		Vector3 position2 = MilMo_Player.Instance.Avatar.GameObject.transform.position;
		if (float.IsNaN(position2.x) || float.IsNaN(position2.y) || float.IsNaN(position2.z) || float.IsPositiveInfinity(position2.x) || float.IsPositiveInfinity(position2.y) || float.IsPositiveInfinity(position2.z) || float.IsNegativeInfinity(position2.x) || float.IsNegativeInfinity(position2.y) || float.IsNegativeInfinity(position2.z))
		{
			string[] obj = new string[6] { "An invalid position was assigned by CharacterController.Move when moving the player with collision. Assigned position: ", null, null, null, null, null };
			Vector3 vector2 = position2;
			obj[1] = vector2.ToString();
			obj[2] = ", move: ";
			vector2 = vector;
			obj[3] = vector2.ToString();
			obj[4] = ", position before move: ";
			vector2 = position;
			obj[5] = vector2.ToString();
			Debug.LogWarning(string.Concat(obj));
			MilMo_Player.Instance.Avatar.GameObject.transform.position = position;
		}
		if (printDebug)
		{
			if ((collisionFlags & CollisionFlags.Above) != 0)
			{
				Debug.Log("Collided above");
			}
			if ((collisionFlags & CollisionFlags.Below) != 0)
			{
				Debug.Log("Collided below");
			}
			if (collisionFlags == CollisionFlags.None)
			{
				Debug.Log("No collision");
			}
			if ((collisionFlags & CollisionFlags.Sides) != 0)
			{
				Debug.Log("Collided sides");
			}
			Vector3 vector2 = pos;
			Debug.Log("Wanted position: " + vector2.ToString());
			Debug.Log("Actual position: " + MilMo_Player.Instance.Avatar.GameObject.transform.position.ToString());
		}
		if ((collisionFlags & CollisionFlags.Below) != 0 && pos != MilMo_Player.Instance.Avatar.GameObject.transform.position)
		{
			Vector3 position3 = MilMo_Player.Instance.Avatar.GameObject.transform.position;
			float distanceToGround = MilMo_Physics.GetDistanceToGround(position3);
			if ((double)distanceToGround > 0.01 && pos.y <= position3.y - distanceToGround)
			{
				if (printDebug)
				{
					Debug.Log("Trying to put the player on the ground");
				}
				pos.y = position3.y - distanceToGround + 0.01f;
				vector = pos - position3;
				collisionFlags = MilMo_Player.Instance.Avatar.Controller.Move(vector);
				if (printDebug)
				{
					if ((collisionFlags & CollisionFlags.Above) != 0)
					{
						Debug.Log("Collided above");
					}
					if ((collisionFlags & CollisionFlags.Below) != 0)
					{
						Debug.Log("Collided below");
					}
					if (collisionFlags == CollisionFlags.None)
					{
						Debug.Log("No collision");
					}
					if ((collisionFlags & CollisionFlags.Sides) != 0)
					{
						Debug.Log("Collided sides");
					}
					Vector3 vector2 = pos;
					Debug.Log("Wanted position: " + vector2.ToString());
					Debug.Log("Actual position: " + MilMo_Player.Instance.Avatar.GameObject.transform.position.ToString());
				}
			}
		}
		if (MilMo_Utility.Equals(Player.Avatar.Position, position))
		{
			if (printDebug)
			{
				Debug.Log("Collision move failed");
			}
			if (!LastCollisionMoveFailed)
			{
				CollisionMoveStartFailTime = Time.time;
			}
			LastCollisionMoveFailed = true;
		}
		else
		{
			LastCollisionMoveFailed = false;
		}
	}

	private void Bash()
	{
		if (!(Player.Avatar.WieldedItem is MilMo_Weapon milMo_Weapon) || MilMo_Level.CurrentLevel == null)
		{
			return;
		}
		if (Player.Avatar.Invulnerable)
		{
			Singleton<GameNetwork>.Instance.RequestEndInvulnerability();
			return;
		}
		IMilMo_AttackTarget closestTarget = MilMo_Level.CurrentLevel.GetClosestTarget(Player.Avatar.Position, Player.Avatar.GameObject.transform.forward, milMo_Weapon.Template, useHitRange: true);
		if (closestTarget != null)
		{
			LockRotationToLookAtTarget(closestTarget.GameObject.transform, GetCooldown());
			SetTemporarySpeedModifier(milMo_Weapon.AttackMovementSpeedModifier, GetCooldown());
			milMo_Weapon.Ready();
			RequestAttack(milMo_Weapon, closestTarget);
		}
		else if (!MilMo_Level.CurrentLevel.RemotePlayerInAttackRadius())
		{
			SetTemporarySpeedModifier(milMo_Weapon.AttackMovementSpeedModifier, GetCooldown());
			RequestAttack(milMo_Weapon, null);
		}
	}

	private void RequestAttack(MilMo_Weapon weapon, IMilMo_AttackTarget target)
	{
		MilMo_RangedWeapon milMo_RangedWeapon = weapon as MilMo_RangedWeapon;
		if (milMo_RangedWeapon != null && milMo_RangedWeapon.Template.ProjectileTemplate == null)
		{
			return;
		}
		if (milMo_RangedWeapon != null && !milMo_RangedWeapon.Template.ProjectileTemplate.InstantHit)
		{
			Vector3 direction = ((target == null || MilMo_Utility.Equals(Player.Avatar.Position, target.Position)) ? Player.Avatar.GameObject.transform.forward : (target.Position - Player.Avatar.Position).normalized);
			Vector3 projectilePosition = milMo_RangedWeapon.GetProjectilePosition();
			float num = milMo_RangedWeapon.Template.Range;
			if (Physics.Raycast(projectilePosition, direction, out var hitInfo, num, 95158273))
			{
				num = hitInfo.distance;
			}
			Singleton<GameNetwork>.Instance.RequestSpawnLevelProjectile(Player.EquipSlots.CurrentItemInventoryId, new vector3(projectilePosition.x, projectilePosition.y, projectilePosition.z), target?.AsNetworkAttackTarget(), num);
		}
		else if (target == null)
		{
			Singleton<GameNetwork>.Instance.SendAttackUntargeted(Player.EquipSlots.CurrentItemInventoryId);
		}
		else
		{
			Singleton<GameNetwork>.Instance.SendAttack(Player.EquipSlots.CurrentItemInventoryId, target);
		}
		if (weapon != null)
		{
			Player.Avatar.PlayAttackEffects();
			Player.Avatar.AddParticleEffect(weapon.Attack());
			if (weapon is MilMo_RangedWeapon)
			{
				MilMo_EventSystem.Instance.PostEvent("use_ranged_weapon", null);
			}
			if (weapon is MilMo_MeleeWeapon)
			{
				MilMo_EventSystem.Instance.PostEvent("use_melee_weapon", null);
			}
		}
	}

	private float GetCooldown()
	{
		if (!(Player.Avatar.WieldedItem is MilMo_Weapon milMo_Weapon))
		{
			return 0f;
		}
		if (milMo_Weapon is MilMo_RangedWeapon && milMo_Weapon.Template.AttackAnimations.Count > 0)
		{
			AnimationState animationState = Player.Avatar.GameObject.GetComponent<Animation>()[milMo_Weapon.Template.AttackAnimations[0]];
			if (animationState != null)
			{
				return Mathf.Min(animationState.length, milMo_Weapon.Template.Cooldown);
			}
		}
		return milMo_Weapon.Template.Cooldown;
	}

	private void Dig()
	{
		Lock(Player.Avatar.WieldedItem.Template.Cooldown, playMoveAnimationOnUnlock: false);
		Player.PlayDigEffects();
		Vector3 digPosition = Player.Avatar.Position + 0.5f * Player.Avatar.GameObject.transform.forward;
		digPosition.y = MilMo_Level.GetWalkableHeight(digPosition);
		MilMo_EventSystem.At(0.6f + Mathf.Max(0f, MilMo_Utility.Random() - 0.2f), delegate
		{
			Singleton<GameNetwork>.Instance.RequestDig(digPosition);
		});
	}

	private void Eat()
	{
		Singleton<GameNetwork>.Instance.RequestEat();
	}

	protected void LockAndTurn(Quaternion rotation, float timeOut, bool playMoveAnimationOnUnlock, UnlockCallback callback)
	{
		if (!MilMo_Player.Instance.IsDone)
		{
			return;
		}
		Stop();
		CurrentSpeed = Vector2.zero;
		IsLocked = true;
		Quaternion targetRotation = rotation;
		targetRotation.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
		TargetRotation = targetRotation;
		PlayMoveAnimationOnUnlock = playMoveAnimationOnUnlock;
		if (!(timeOut > 0f))
		{
			return;
		}
		UnlockReaction = MilMo_EventSystem.At(timeOut, delegate
		{
			Unlock();
			if (callback != null)
			{
				callback();
			}
		});
	}

	private void LockRotationToLookAtTarget(Transform lookAtTarget, float timeOut)
	{
		if (RemoveLookatTargetEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(RemoveLookatTargetEvent);
			RemoveLookatTargetEvent = null;
		}
		LookAtTarget = lookAtTarget;
		if (timeOut > 0f)
		{
			RemoveLookatTargetEvent = MilMo_EventSystem.At(timeOut, delegate
			{
				LookAtTarget = null;
				RemoveLookatTargetEvent = null;
			});
		}
	}

	private void SetTemporarySpeedModifier(float modifier, float timeOut)
	{
		if (RemoveSpeedModifierEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(RemoveSpeedModifierEvent);
			RemoveSpeedModifierEvent = null;
		}
		TemporarySpeedModifier = modifier;
		if (timeOut > 0f)
		{
			RemoveSpeedModifierEvent = MilMo_EventSystem.At(timeOut, delegate
			{
				RemoveSpeedModifierEvent = null;
				TemporarySpeedModifier = 1f;
			});
		}
	}

	public void Lock(float timeOut, bool playMoveAnimationOnUnlock)
	{
		if (!MilMo_Player.Instance.IsDone)
		{
			return;
		}
		Stop();
		CurrentSpeed = Vector2.zero;
		IsLocked = true;
		TargetRotation = Player.Avatar.GameObject.transform.rotation;
		PlayMoveAnimationOnUnlock = playMoveAnimationOnUnlock;
		if (UnlockReaction != null)
		{
			if (timeOut > 0f)
			{
				NewUnlockTime(timeOut);
			}
			else
			{
				CancelTimedUnlock();
			}
		}
		else if (timeOut > 0f)
		{
			NewUnlockTime(timeOut);
		}
	}

	public void NewUnlockTime(float timeOut)
	{
		CancelTimedUnlock();
		UnlockReaction = MilMo_EventSystem.At(timeOut, Unlock);
	}

	protected void Unlock(object o)
	{
		Unlock();
	}

	public void Unlock()
	{
		IsLocked = false;
		IsTurning = false;
		CancelTimedUnlock();
		if (Type != ControllerType.InGUIApp && PlayMoveAnimationOnUnlock)
		{
			PlayMoveAnimation();
		}
	}

	private void CancelTimedUnlock()
	{
		if (UnlockReaction != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(UnlockReaction);
			UnlockReaction = null;
		}
	}

	public virtual void Exit()
	{
	}

	public static void SetMovementMode()
	{
		if (IsInWater)
		{
			CurrentMoveSpeed = SwimSpeed;
			Player.Avatar.MoveAnimation = SwimAnimation;
			Player.Avatar.IdleAnimation = WaterIdleAnimation;
			Player.Avatar.MoveParticle = "Swim";
			return;
		}
		Player.Avatar.IdleAnimation = IdleAnimation;
		if (RunMode)
		{
			CurrentMoveSpeed = RunSpeed;
			Player.Avatar.MoveAnimation = RunAnimation;
			Player.Avatar.JumpAnimation = "RunJump";
			Player.Avatar.FallAnimation = "RunFall";
			Player.Avatar.HoverAnimation = "RunHover";
			Player.Avatar.JumpParticle = "RunJump";
			Player.Avatar.MoveParticle = "Run";
		}
		else
		{
			CurrentMoveSpeed = WalkSpeed;
			Player.Avatar.MoveAnimation = WalkAnimation;
			Player.Avatar.JumpAnimation = "WalkJump";
			Player.Avatar.FallAnimation = "WalkFall";
			Player.Avatar.HoverAnimation = "WalkHover";
			Player.Avatar.JumpParticle = "WalkJump";
			Player.Avatar.MoveParticle = "Run";
		}
	}

	protected void AttackRejected(object o)
	{
		Unlock();
	}

	public void Respawn()
	{
		ImpulseVelocity = Vector3.zero;
		CurrentSpeed = Vector3.zero;
		TargetSpeed = Vector3.zero;
		Unlock();
	}
}
