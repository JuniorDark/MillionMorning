using System;
using Code.Core.Camera;
using Code.Core.EventSystem;
using Code.Core.Input;
using Code.Core.Utility;
using Code.Core.Visual;
using Core.Utilities;
using UnityEngine;

namespace Code.World.Player;

public class MilMo_PlayerControllerGame : MilMo_PlayerControllerBase
{
	protected bool GotMoveInput;

	protected Vector3 CurrentMoveDirection = Vector3.zero;

	private bool _rotateToCamera;

	protected readonly MilMo_GenericReaction JumpButtonReaction;

	protected readonly MilMo_GenericReaction AttackButtonReaction;

	protected readonly MilMo_GenericReaction ActionButtonReaction;

	protected readonly MilMo_GenericReaction RunWalkButtonReaction;

	protected readonly MilMo_GenericReaction AttackRejectedReaction;

	protected readonly MilMo_GenericReaction PickupFailReaction;

	public override ControllerType Type => ControllerType.Game;

	protected virtual bool AllowRotateToCamera => true;

	public MilMo_PlayerControllerGame()
	{
		JumpButtonReaction = MilMo_EventSystem.Listen("button_Jump", Jump);
		JumpButtonReaction.Repeating = true;
		AttackButtonReaction = MilMo_EventSystem.Listen("button_Attack", base.Attack);
		AttackButtonReaction.Repeating = true;
		ActionButtonReaction = MilMo_EventSystem.Listen("button_Action", base.AttackOrUseClickedUsable);
		ActionButtonReaction.Repeating = true;
		RunWalkButtonReaction = MilMo_EventSystem.Listen("button_ToggleRunWalk", MilMo_PlayerControllerBase.ToggleRunWalk);
		RunWalkButtonReaction.Repeating = true;
		AttackRejectedReaction = MilMo_EventSystem.Listen("player_attack_rejected", base.AttackRejected);
		AttackRejectedReaction.Repeating = true;
		PickupFailReaction = MilMo_EventSystem.Listen("pickup_fail", base.Unlock);
		PickupFailReaction.Repeating = true;
		MilMo_PlayerControllerBase.MovementState = MovementStates.Idle;
		if (!MilMo_PlayerControllerBase.IsLocked)
		{
			MilMo_PlayerControllerBase.PlayMoveAnimation();
		}
	}

	public override void Exit()
	{
		base.Exit();
		MilMo_EventSystem.RemoveReaction(JumpButtonReaction);
		MilMo_EventSystem.RemoveReaction(AttackButtonReaction);
		MilMo_EventSystem.RemoveReaction(AttackRejectedReaction);
		MilMo_EventSystem.RemoveReaction(RunWalkButtonReaction);
		MilMo_EventSystem.RemoveReaction(PickupFailReaction);
	}

	public override void UpdatePlayer()
	{
		base.UpdatePlayer();
		MilMo_PlayerControllerBase.UpdateGroundTypeConfig();
		HandleInput();
		UpdateTransform();
	}

	private void HandleInput()
	{
		if (MilMo_PlayerControllerBase.IsLocked || MilMo_PlayerControllerBase.Player.IsExhausted)
		{
			MilMo_PlayerControllerBase.MovementState = MovementStates.Idle;
			return;
		}
		if (MilMo_PlayerControllerBase.HasLockingImpulse)
		{
			GotMoveInput = false;
			return;
		}
		float horizontalAxis = MilMo_Input.HorizontalAxis;
		float verticalAxis = MilMo_Input.VerticalAxis;
		GotMoveInput = horizontalAxis != 0f || verticalAxis != 0f;
		if (MilMo_World.Instance.Camera.movieCameraController.HookedUp)
		{
			GotMoveInput = false;
		}
		if (GotMoveInput)
		{
			MilMo_PlayerControllerBase.TargetLocked = false;
			if (MilMo_PlayerControllerBase.MovementState != MovementStates.Forward)
			{
				MilMo_PlayerControllerBase.MovementState = MovementStates.Forward;
				if (!MilMo_Input.GetKeyDown("Jump") && (MilMo_PlayerControllerBase.WasGrounded || MilMo_PlayerControllerBase.IsOnTheGround || (MilMo_PlayerControllerBase.IsInWater && (MilMo_PlayerControllerBase.WaterMover.Paused || (double)Mathf.Abs(MilMo_PlayerControllerBase.WaterMover.Pos.y) <= 0.5))))
				{
					MilMo_PlayerControllerBase.PlayMoveAnimation();
				}
			}
		}
		else if (MilMo_PlayerControllerBase.MovementState == MovementStates.Forward)
		{
			Stop();
		}
		float modifier = MilMo_PlayerControllerBase.Player.Avatar.EntityStateManager.GetModifier(MilMo_PlayerControllerBase.IsInWater ? "SWIMSPEED" : "SPEED");
		MilMo_PlayerControllerBase.TargetSpeed.z = MilMo_PlayerControllerBase.CurrentMoveSpeed * MilMo_PlayerControllerBase.TemporarySpeedModifier * modifier * verticalAxis;
		MilMo_PlayerControllerBase.TargetSpeed.x = MilMo_PlayerControllerBase.CurrentMoveSpeed * MilMo_PlayerControllerBase.TemporarySpeedModifier * modifier * horizontalAxis;
	}

	private static void Jump(object o)
	{
		if (MilMo_PlayerControllerBase.IsLocked || MilMo_PlayerControllerBase.Player.IsExhausted || ((!MilMo_PlayerControllerBase.IsInWater || !(MilMo_PlayerControllerBase.PreviousHeightAboveGround < MilMo_PlayerControllerBase.IsOnGroundTolerance)) && !Physics.CheckSphere(MilMo_PlayerControllerBase.Player.Avatar.Position, MilMo_PlayerControllerBase.IsOnGroundTolerance, -805306401)))
		{
			return;
		}
		MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_Jump", "");
		float num = (MilMo_PlayerControllerBase.RunMode ? MilMo_PlayerControllerBase.RunJumpSpeed : MilMo_PlayerControllerBase.WalkJumpSpeed);
		num += MilMo_PlayerControllerBase.Player.Avatar.EntityStateManager.GetModifier("JUMP");
		if (!MilMo_PlayerControllerBase.IsInWater)
		{
			Vector3 position = MilMo_PlayerControllerBase.Player.Avatar.Position;
			position.y += 0.5f;
			if (Physics.Raycast(position, Vector3.down, out var hitInfo, 100f, -805306417))
			{
				bool flag = false;
				if (hitInfo.collider is TerrainCollider)
				{
					flag = true;
				}
				else
				{
					MilMo_VisualRepComponent component = MilMo_Utility.GetAncestor(hitInfo.collider.gameObject).GetComponent<MilMo_VisualRepComponent>();
					MilMo_VisualRepData milMo_VisualRepData = ((component != null) ? component.GetData() : null);
					if (milMo_VisualRepData != null && milMo_VisualRepData.treatAsTerrainForJump)
					{
						flag = true;
					}
				}
				if (flag)
				{
					Vector3 normal = hitInfo.normal;
					normal.Normalize();
					if (normal.y < 0.5625f)
					{
						normal.y = 0f;
						normal.Normalize();
						normal *= num * 4f;
						MilMo_PlayerControllerBase.ImpulseVelocity = new Vector3(normal.x, MilMo_PlayerControllerBase.ImpulseVelocity.y, normal.z);
						num = 0f;
					}
				}
			}
			MilMo_PlayerControllerBase.PreviousVelocityY = MilMo_PlayerControllerBase.CurrentSpeed.y;
			Vector3 currentSpeed = MilMo_PlayerControllerBase.CurrentSpeed;
			currentSpeed.y = num;
			MilMo_PlayerControllerBase.CurrentSpeed = currentSpeed;
			MilMo_PlayerControllerBase.Player.Avatar.PlayParticleEffect(MilMo_PlayerControllerBase.Player.Avatar.JumpParticle);
			MilMo_PlayerControllerBase.DidJump = true;
		}
		else
		{
			if (Mathf.Abs(MilMo_PlayerControllerBase.WaterMover.Vel.y) <= 0.1f && (double)Mathf.Abs(MilMo_PlayerControllerBase.WaterMover.Pos.y) <= 0.1)
			{
				MilMo_PlayerControllerBase.Player.Avatar.PlayAnimation("SurfaceJump02");
				MilMo_PlayerControllerBase.WaterMover.Vel.y = num * MilMo_PlayerControllerBase.WaterImpactFactor;
			}
			else
			{
				MilMo_PlayerControllerBase.Player.Avatar.PlayAnimation("SurfaceJump02");
				MilMo_PlayerControllerBase.WaterMover.Vel.y = num * MilMo_PlayerControllerBase.WaterImpactFactor * 0.5f;
			}
			MilMo_PlayerControllerBase.Player.Avatar.EmitPuff("SoftImpact", "Water", MilMo_PlayerControllerBase.WaterSurfaceHeight);
			MilMo_PlayerControllerBase.WaterMover.UnPause();
		}
	}

	private void UpdateTransform()
	{
		if (MilMo_PlayerControllerBase.IsLocked)
		{
			HandleLockedMode();
			UpdateImpulseVelocity();
			return;
		}
		if (MilMo_PlayerControllerBase.HasLockingImpulse)
		{
			HandleLockingImpulse();
			UpdateImpulseVelocity();
			return;
		}
		Vector3 newPos = MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.position;
		Transform cameraTransform = MilMo_CameraController.CameraTransform;
		Vector3 vector = newPos - cameraTransform.position;
		vector.y = 0f;
		vector.Normalize();
		Vector3 vector2 = new Vector3(vector.z, 0f, 0f - vector.x);
		vector = cameraTransform.forward;
		vector.y = 0f;
		vector.Normalize();
		if (MilMo_PlayerControllerBase.LookAtTarget != null)
		{
			Vector3 vector3 = MilMo_PlayerControllerBase.LookAtTarget.position - MilMo_PlayerControllerBase.Player.Avatar.Position;
			vector3.y = 0f;
			Quaternion rotation = MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation;
			MilMo_PlayerControllerBase.TargetRotation = ((!MilMo_Utility.Equals(vector3, Vector3.zero)) ? Quaternion.LookRotation(vector3) : rotation);
			rotation = Quaternion.Lerp(rotation, MilMo_PlayerControllerBase.TargetRotation, MilMo_PlayerControllerBase.RotationSpeed * Time.deltaTime);
			MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation = rotation;
		}
		else if (!MilMo_World.Instance.Camera.movieCameraController.HookedUp && (!MilMo_Camera.Orbit() || GotMoveInput || MilMo_PlayerControllerBase.TargetLocked))
		{
			_rotateToCamera = false;
			UpdateTargetRotation(vector, vector2);
			MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation = Quaternion.Lerp(MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation, MilMo_PlayerControllerBase.TargetRotation, MilMo_PlayerControllerBase.RotationSpeed * Time.deltaTime);
		}
		else if (MilMo_Input.DefaultMode == MilMo_Input.ControlMode.Mmorpg)
		{
			if (!AllowRotateToCamera)
			{
				_rotateToCamera = false;
			}
			else if (MilMo_Input.RotationAxis != 0f || MilMo_Input.GetKey(KeyCode.Mouse1, useKeyboardFocus: false))
			{
				_rotateToCamera = true;
			}
			else if (MilMo_Input.GetKey(KeyCode.Mouse0, useKeyboardFocus: false))
			{
				_rotateToCamera = false;
			}
			if (_rotateToCamera)
			{
				Vector3 eulerAngles = MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation.eulerAngles;
				eulerAngles.y = cameraTransform.rotation.eulerAngles.y;
				MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation = Quaternion.Euler(eulerAngles);
			}
		}
		UpdateVelocity();
		UpdateImpulseVelocity();
		CurrentMoveDirection = Vector3.zero;
		if (!MilMo_World.Instance.Camera.movieCameraController.HookedUp)
		{
			CurrentMoveDirection += vector2 * MilMo_PlayerControllerBase.CurrentSpeed.x;
			CurrentMoveDirection += vector * MilMo_PlayerControllerBase.CurrentSpeed.z;
			CurrentMoveDirection.y = 0f;
			CurrentMoveDirection.Normalize();
		}
		float num = Mathf.Max(Mathf.Abs(MilMo_PlayerControllerBase.CurrentSpeed.x), Mathf.Abs(MilMo_PlayerControllerBase.CurrentSpeed.z)) * Time.deltaTime;
		newPos += CurrentMoveDirection * num;
		newPos += MilMo_PlayerControllerBase.ImpulseVelocity * Time.deltaTime;
		CheckWater(ref newPos);
		if (MilMo_PlayerControllerBase.IsInWater)
		{
			MilMo_PlayerControllerBase.Collision(newPos);
			UpdateY(ref newPos);
		}
		else
		{
			UpdateY(ref newPos);
			MilMo_PlayerControllerBase.Collision(newPos);
		}
	}

	protected virtual void HandleLockedMode()
	{
		float num = Mathf.Abs(MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation.eulerAngles.y - MilMo_PlayerControllerBase.TargetRotation.eulerAngles.y);
		if (num > 0.05f)
		{
			float num2 = 2f * num * (MathF.PI / 180f) * (MathF.PI / 2f) + 1f;
			MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation = Quaternion.Lerp(MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation, MilMo_PlayerControllerBase.TargetRotation, num2 * Time.deltaTime);
			MilMo_PlayerControllerBase.IsTurning = true;
		}
		else
		{
			MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation = MilMo_PlayerControllerBase.TargetRotation;
			MilMo_PlayerControllerBase.IsTurning = false;
		}
		Vector3 newPos = MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.position;
		newPos += MilMo_PlayerControllerBase.ImpulseVelocity * Time.deltaTime;
		CheckWater(ref newPos);
		if (MilMo_PlayerControllerBase.IsInWater)
		{
			MilMo_PlayerControllerBase.Collision(newPos);
			UpdateY(ref newPos);
		}
		else
		{
			UpdateY(ref newPos);
			MilMo_PlayerControllerBase.Collision(newPos);
		}
	}

	private void HandleLockingImpulse()
	{
		Vector3 pos = MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.position;
		pos += MilMo_PlayerControllerBase.ImpulseVelocity * Time.deltaTime;
		UpdateY(ref pos);
		MilMo_PlayerControllerBase.Collision(pos);
	}

	private static void UpdateVelocity()
	{
		Vector3 currentSpeed = MilMo_PlayerControllerBase.CurrentSpeed;
		if (Maths.FloatNotEquals(currentSpeed.z, MilMo_PlayerControllerBase.TargetSpeed.z))
		{
			float num = ((Mathf.Abs(currentSpeed.z) > Mathf.Abs(MilMo_PlayerControllerBase.TargetSpeed.z)) ? MilMo_PlayerControllerBase.DecelerationSpeedZ : MilMo_PlayerControllerBase.AccelerationSpeedZ);
			currentSpeed.z = Mathf.Lerp(currentSpeed.z, MilMo_PlayerControllerBase.TargetSpeed.z, num * Time.deltaTime);
			if ((double)Mathf.Abs(MilMo_PlayerControllerBase.CurrentSpeed.z - MilMo_PlayerControllerBase.TargetSpeed.z) < 0.01)
			{
				currentSpeed.z = MilMo_PlayerControllerBase.TargetSpeed.z;
			}
			MilMo_PlayerControllerBase.CurrentSpeed = currentSpeed;
		}
		if (Maths.FloatNotEquals(currentSpeed.x, MilMo_PlayerControllerBase.TargetSpeed.x))
		{
			float num2 = ((Mathf.Abs(currentSpeed.x) > Mathf.Abs(MilMo_PlayerControllerBase.TargetSpeed.x)) ? MilMo_PlayerControllerBase.DecelerationSpeedX : MilMo_PlayerControllerBase.AccelerationSpeedX);
			currentSpeed.x = Mathf.Lerp(currentSpeed.x, MilMo_PlayerControllerBase.TargetSpeed.x, num2 * Time.deltaTime);
			if ((double)Mathf.Abs(currentSpeed.x - MilMo_PlayerControllerBase.TargetSpeed.x) < 0.01)
			{
				currentSpeed.x = MilMo_PlayerControllerBase.TargetSpeed.x;
			}
			MilMo_PlayerControllerBase.CurrentSpeed = currentSpeed;
		}
	}

	private static void UpdateImpulseVelocity()
	{
		if (!(MilMo_PlayerControllerBase.ImpulseVelocity == Vector3.zero))
		{
			MilMo_PlayerControllerBase.ImpulseVelocity = Vector3.Lerp(MilMo_PlayerControllerBase.ImpulseVelocity, Vector3.zero, MilMo_PlayerControllerBase.ImpulseFalloffSpeed * Time.deltaTime);
			if ((double)MilMo_PlayerControllerBase.ImpulseVelocity.sqrMagnitude < 0.1)
			{
				MilMo_PlayerControllerBase.ImpulseVelocity = Vector3.zero;
			}
		}
	}

	private static void UpdateTargetRotation(Vector3 forward, Vector3 right)
	{
		Vector3 forward2 = forward * MilMo_PlayerControllerBase.TargetSpeed.z + right * MilMo_PlayerControllerBase.TargetSpeed.x;
		if (forward2.sqrMagnitude > 0f && !MilMo_PlayerControllerBase.TargetLocked)
		{
			MilMo_PlayerControllerBase.TargetRotation = Quaternion.LookRotation(forward2);
		}
	}

	protected override void EnterWater(ref Vector3 newPos)
	{
		base.EnterWater(ref newPos);
		MilMo_PlayerControllerBase.Player.DisableAllWieldables();
	}

	protected override bool ExitWater(ref Vector3 newPos)
	{
		if (!base.ExitWater(ref newPos))
		{
			return false;
		}
		MilMo_PlayerControllerBase.Player.EnableWieldables();
		return true;
	}
}
