using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Collision;
using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.Core.Input;
using Code.Core.Utility;
using Code.World.Level;
using Core.Input;
using UnityEngine;

namespace Code.Core.Camera;

public sealed class MilMo_PlatformCameraController : MilMo_GameCameraController
{
	private static float _lastFreeLookTime;

	private const float AUTO_RESET_RIGHT_CLICK_TIMEOUT = 60f;

	private const float MIN_DISTANCE = 2.7f;

	private const float MAX_DISTANCE = 6f;

	private const float MAGNET_TARGET_MAX_LOOKUP_RESET_SPEED = 4f;

	private const float MAGNET_TARGET_MAX_PAN_RESET_SPEED = 4f;

	private const float POSITION_DAMPING = 10f;

	private const float ACCELERATION = 10f;

	private const int POSITION_BUFFER_SIZE = 4;

	private const float ORBIT_DAMPING = 4f;

	private static Vector3 _velocity;

	private static Vector3 _noCollisionPosition;

	private static Vector3 _noCollisionForward;

	private static readonly LinkedList<Vector3> PositionBuffer = new LinkedList<Vector3>();

	private static bool _stickCollision;

	private static bool _useStickCollision = true;

	private static float _lastCollisionTime;

	private static bool _moveInputSinceLastCollision = true;

	private static bool _hadMoveInput;

	private static bool _didResetSinceLastMagnetTarget;

	private static Vector3 _magnetTarget = Vector3.zero;

	private static float _lastNewMagnetTargetTime;

	private MilMo_GenericReaction _useRangedWeaponReaction;

	private MilMo_GenericReaction _useMeleeWeaponReaction;

	private bool _gotFreeLookInput;

	public override void HookUp()
	{
		base.HookUp();
		if (MilMo_Level.CurrentLevel != null)
		{
			MilMo_CameraController.CameraComponent.backgroundColor = MilMo_Level.CurrentLevel.Environment.BackgroundColor;
		}
		MilMo_CameraController.CameraComponent.nearClipPlane = 0.1f;
		if (Vector3.SqrMagnitude(MilMo_CameraController.Player.position - MilMo_CameraController.CameraTransform.position) > 625f)
		{
			SetupPosition();
		}
		EnterReset(resetDistance: true, ResetSpeed.Slow);
		MilMo_GameCameraController.Pan = MilMo_CameraController.CameraTransform.eulerAngles.y;
		MilMo_GameCameraController.ResettingPan = false;
		MilMo_GameCameraController.WantedPan = MilMo_GameCameraController.Pan;
		MilMo_CameraController.Orbit = MilMo_Input.GetKey(KeyCode.Mouse1, useKeyboardFocus: false);
		_useRangedWeaponReaction = MilMo_EventSystem.Listen("use_ranged_weapon", base.ResetFast);
		_useRangedWeaponReaction.Repeating = true;
		_useMeleeWeaponReaction = MilMo_EventSystem.Listen("use_melee_weapon", MeleeAttack);
		_useMeleeWeaponReaction.Repeating = true;
		for (LinkedListNode<Vector3> linkedListNode = PositionBuffer.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			linkedListNode.Value = MilMo_CameraController.CameraTransform.position;
		}
		_noCollisionPosition = MilMo_CameraController.CameraTransform.position;
		_noCollisionForward = MilMo_CameraController.CameraTransform.forward;
	}

	public override void Unhook()
	{
		base.Unhook();
		MilMo_EventSystem.RemoveReaction(_useMeleeWeaponReaction);
		_useMeleeWeaponReaction = null;
		MilMo_EventSystem.RemoveReaction(_useRangedWeaponReaction);
		_useRangedWeaponReaction = null;
	}

	public override void SetupPosition()
	{
		base.SetupPosition();
		for (int i = PositionBuffer.Count; i < 4; i++)
		{
			PositionBuffer.AddLast(MilMo_CameraController.CameraTransform.position);
		}
		MilMo_GameCameraController.ValidatePosition();
	}

	public void Update()
	{
		if (!base.HookedUp)
		{
			return;
		}
		Vector3 position = MilMo_CameraController.CameraTransform.position;
		Quaternion rotation = MilMo_CameraController.CameraTransform.rotation;
		_useStickCollision = true;
		Vector3 vector = MilMo_CameraController.Player.position + MilMo_CameraController.HeadOffset;
		if (!MilMo_Input.GetKey(KeyCode.Mouse1, useKeyboardFocus: false))
		{
			CheckMagnets(vector);
		}
		CheckInput();
		UseEdgeLookDown = !HasMagnetTarget;
		CheckEdgeLookDown();
		ApplyResetting();
		ApplyPlayerPosition(vector);
		if (!MilMo_CameraController.Orbit)
		{
			MilMo_CameraController.SetupRotation(vector);
			ApplyLookup(vector);
		}
		if (_useStickCollision)
		{
			_stickCollision = MilMo_CameraOcclusion.DoMove(position, MilMo_CameraController.CameraTransform.position, vector, rotation, doCollision: false, searchVertically: true, searchFullCircle: false, MilMo_CameraController.Distance, out var cameraPosition);
			if (_stickCollision)
			{
				MilMo_GameCameraController.ResettingDistance = false;
				MilMo_GameCameraController.ResettingPan = false;
				MilMo_CameraController.CameraTransform.position = cameraPosition + EffectPosition;
				if ((bool)Terrain.activeTerrain)
				{
					MilMo_CameraController.MoveAbove(MilMo_Physics.GetTerrainHeight(cameraPosition));
				}
				MilMo_CameraController.SetupRotation(vector);
				_moveInputSinceLastCollision = false;
				MilMo_GameCameraController.Lookup = MilMo_CameraController.CameraTransform.rotation.eulerAngles.x;
				if (MilMo_GameCameraController.Lookup > 270f)
				{
					MilMo_GameCameraController.Lookup -= 360f;
				}
				MilMo_GameCameraController.Pan = MilMo_CameraController.CameraTransform.rotation.eulerAngles.y;
				MilMo_GameCameraController.WantedLookup = MilMo_GameCameraController.Lookup;
				MilMo_GameCameraController.WantedPan = MilMo_GameCameraController.Pan;
				_velocity = Vector3.zero;
				_lastCollisionTime = Time.time;
			}
		}
		else
		{
			_stickCollision = false;
		}
		ResetPaused = _stickCollision;
		MilMo_CameraController.UpdateAudioListenerPosition();
	}

	protected override bool PreventTerrainLookup()
	{
		if (!(Time.time - _lastCollisionTime < 2f))
		{
			return !_moveInputSinceLastCollision;
		}
		return true;
	}

	private void CheckMagnets(Vector3 lookAtPosition)
	{
		List<Vector3> list = (from magnet in MilMo_CameraController.Magnets
			where magnet.IsInside(MilMo_CameraController.Player.position)
			select magnet.CameraTarget).ToList();
		if (list.Count == 0)
		{
			if (HasMagnetTarget && !_didResetSinceLastMagnetTarget)
			{
				MilMo_GameCameraController.ResettingPan = false;
			}
			HasMagnetTarget = false;
			_magnetTarget = new Vector3(float.NaN, float.NaN, float.NaN);
			return;
		}
		Vector3 vector = list.Aggregate(Vector3.zero, (Vector3 current, Vector3 magnet) => current + magnet);
		if (list.Count > 0)
		{
			vector /= (float)list.Count;
		}
		bool flag = !MilMo_Utility.Equals(vector, _magnetTarget);
		if (!_didResetSinceLastMagnetTarget || flag)
		{
			_magnetTarget = vector;
			Vector3 forward = lookAtPosition - _magnetTarget;
			MilMo_GameCameraController.WantedDistance = forward.magnitude;
			Quaternion quaternion = Quaternion.LookRotation(forward);
			MilMo_GameCameraController.WantedPan = quaternion.eulerAngles.y;
			MilMo_GameCameraController.WantedLookup = quaternion.eulerAngles.x;
			MilMo_GameCameraController.ResettingDistance = true;
			MilMo_GameCameraController.ResettingPan = true;
			MilMo_GameCameraController.ResettingLookup = true;
			_didResetSinceLastMagnetTarget = false;
			MilMo_GameCameraController.CurrentLookupResetAcceleration = 3f;
			MilMo_GameCameraController.CurrentPanResetAcceleration = 3f;
			MilMo_GameCameraController.CurrentMaxLookupResetSpeed = 4f;
			MilMo_GameCameraController.CurrentMaxPanResetSpeed = 4f;
			if (!MilMo_CameraController.Orbit)
			{
				MilMo_CameraController.Orbit = true;
				MilMo_GameCameraController.Pan = MilMo_CameraController.CameraTransform.rotation.eulerAngles.y;
			}
			if (flag)
			{
				_lastNewMagnetTargetTime = Time.time;
			}
			HasMagnetTarget = true;
		}
	}

	private void ApplyPlayerPosition(Vector3 aTargetCenter)
	{
		Vector3 position = MilMo_CameraController.CameraTransform.position;
		Vector3 vector = position - aTargetCenter;
		if (!MilMo_GameCameraController.ResettingDistance && !_stickCollision)
		{
			MilMo_CameraController.Distance = (Vector3.Dot(_noCollisionPosition - aTargetCenter, _noCollisionForward) * _noCollisionForward).magnitude;
			if (MilMo_CameraController.Distance > 6f)
			{
				MilMo_CameraController.Distance = Mathf.Lerp(MilMo_CameraController.Distance, 6f, 10f * Time.deltaTime);
			}
			else if (MilMo_CameraController.Distance < 2.7f)
			{
				MilMo_CameraController.Distance = Mathf.Lerp(MilMo_CameraController.Distance, 2.7f, 10f * Time.deltaTime);
			}
		}
		vector.y = 0f;
		vector.Normalize();
		Vector3 vector2 = vector * MilMo_CameraController.Distance + aTargetCenter;
		Vector3 vector3 = new Vector3(0f, 0f, 0f);
		if (MilMo_CameraController.Orbit)
		{
			if (!MilMo_GameCameraController.ResettingLookup)
			{
				MilMo_GameCameraController.Lookup = Mathf.LerpAngle(MilMo_GameCameraController.Lookup, MilMo_GameCameraController.WantedLookup, 4f * Time.deltaTime);
			}
			else
			{
				CalculateTerrainLookup();
				MilMo_GameCameraController.Lookup = Mathf.LerpAngle(MilMo_GameCameraController.Lookup, MilMo_GameCameraController.CurrentTerrainLookup, 5f * Time.deltaTime);
			}
			MilMo_GameCameraController.Lookup = Mathf.Repeat(MilMo_GameCameraController.Lookup, 360f);
			if (MilMo_GameCameraController.Lookup > 180f)
			{
				MilMo_GameCameraController.Lookup -= 360f;
			}
			MilMo_GameCameraController.Lookup = Mathf.Clamp(MilMo_GameCameraController.Lookup, -50f, 87.5f);
			MilMo_GameCameraController.Pan = Mathf.LerpAngle(MilMo_GameCameraController.Pan, MilMo_GameCameraController.WantedPan, (MilMo_GameCameraController.ResettingPan ? MilMo_GameCameraController.PanResetSpeed : 4f) * Time.deltaTime);
			Quaternion quaternion = Quaternion.Euler(MilMo_GameCameraController.Lookup, MilMo_GameCameraController.Pan, 0f);
			vector3 = (_noCollisionPosition = aTargetCenter + quaternion * new Vector3(0f, 0f, 0f - MilMo_CameraController.Distance));
			_noCollisionForward = (quaternion * Vector3.forward).normalized;
			if (!_stickCollision && Physics.Linecast(MilMo_CameraController.CameraTransform.position, vector3, out var hitInfo, -838860833) && Physics.Linecast(aTargetCenter, vector3, out hitInfo, -838860833))
			{
				_useStickCollision = false;
				Vector3 normalized = (aTargetCenter - vector3).normalized;
				vector3 = hitInfo.point + normalized * 0.425f;
			}
			Vector3 pos = vector3;
			pos.y = (aTargetCenter + Quaternion.Euler(7f, MilMo_GameCameraController.Pan, 0f) * new Vector3(0f, 0f, 0f - MilMo_CameraController.Distance)).y;
			float num = pos.y - MilMo_Physics.GetDistanceToGround(pos);
			if ((bool)Terrain.activeTerrain)
			{
				num = Mathf.Max(MilMo_Physics.GetTerrainHeight(pos), num);
			}
			MilMo_CameraController.CameraTransform.position = vector3 + EffectPosition;
			MilMo_CameraController.MoveAbove(num);
			MilMo_CameraController.CameraTransform.rotation = quaternion;
		}
		else
		{
			vector3.x = Mathf.SmoothDamp(position.x, vector2.x, ref _velocity.x, 10f, 4.5f);
			vector3.y = Mathf.SmoothDamp(position.y, aTargetCenter.y + 2.865f, ref _velocity.y, 10f, 4.5f);
			vector3.z = Mathf.SmoothDamp(position.z, vector2.z, ref _velocity.z, 10f, 4.5f);
			MilMo_CameraController.CameraTransform.position = vector3 + EffectPosition;
		}
	}

	private void ApplyLookup(Vector3 targetCenter)
	{
		if (!_stickCollision)
		{
			CalculateTerrainLookup();
			MilMo_GameCameraController.Lookup = Mathf.LerpAngle(MilMo_GameCameraController.Lookup, MilMo_GameCameraController.CurrentTerrainLookup, 5f * Time.deltaTime);
		}
		Quaternion quaternion = Quaternion.Euler(MilMo_GameCameraController.Lookup, MilMo_CameraController.CameraTransform.rotation.eulerAngles.y, 0f);
		Vector3 vector = (_noCollisionPosition = targetCenter + quaternion * new Vector3(0f, 0f, 0f - MilMo_CameraController.Distance));
		_noCollisionForward = (quaternion * Vector3.forward).normalized;
		if (!_stickCollision && Physics.Linecast(MilMo_CameraController.CameraTransform.position, vector, out var hitInfo, -838860833) && Physics.Linecast(targetCenter, vector, out hitInfo, -838860833))
		{
			_useStickCollision = false;
			Vector3 normalized = (targetCenter - vector).normalized;
			vector = hitInfo.point + normalized * 0.425f;
		}
		Vector3 pos = vector;
		float num = pos.y - MilMo_Physics.GetDistanceToGround(pos);
		if ((bool)Terrain.activeTerrain)
		{
			num = Mathf.Max(MilMo_Physics.GetTerrainHeight(pos), num);
		}
		MilMo_CameraController.CameraTransform.position = vector + EffectPosition;
		MilMo_CameraController.MoveAbove(num);
		MilMo_CameraController.CameraTransform.rotation = quaternion;
	}

	private void CheckInput()
	{
		bool gotFreeLookInput = _gotFreeLookInput;
		_gotFreeLookInput = false;
		if (!MilMo_UserInterface.KeyboardFocus)
		{
			bool key = MilMo_Input.GetKey(KeyCode.Mouse1, useKeyboardFocus: false);
			bool key2 = MilMo_Input.GetKey(KeyCode.Mouse0, useKeyboardFocus: false, useMouseFocus: true);
			float axis = InputSwitch.GetAxis("Camera X");
			float axis2 = InputSwitch.GetAxis("Camera Y");
			bool flag = ((double)Math.Abs(MilMo_Input.VerticalAxis) > 0.0 || (double)Math.Abs(MilMo_Input.HorizontalAxis) > 0.0 || MilMo_Input.GetKey("Jump")) && (MilMo_CameraController.ThePlayerLockedCallback == null || !MilMo_CameraController.ThePlayerLockedCallback());
			if (flag)
			{
				_moveInputSinceLastCollision = true;
				if (!HasMagnetTarget)
				{
					if (MilMo_GameCameraController.ResettingPan)
					{
						MilMo_GameCameraController.ResettingPan = false;
						MilMo_GameCameraController.WantedPan = MilMo_GameCameraController.Pan;
						MilMo_CameraController.Orbit = false;
					}
					if (MilMo_GameCameraController.ResettingDistance)
					{
						MilMo_GameCameraController.ResettingDistance = false;
					}
				}
			}
			if (key || key2 || (double)Math.Abs(axis) > 0.0 || (double)Math.Abs(axis2) > 0.0)
			{
				if (!gotFreeLookInput)
				{
					EnterOrbit();
				}
				_lastFreeLookTime = Time.time;
				AutoReset = false;
				_gotFreeLookInput = true;
			}
			if (MilMo_CameraController.Orbit && !_gotFreeLookInput && !MilMo_GameCameraController.ResettingPan && flag && Mathf.Abs(MilMo_GameCameraController.Pan - MilMo_GameCameraController.WantedPan) < 10f && Mathf.Abs(MilMo_GameCameraController.Lookup - MilMo_GameCameraController.WantedLookup) < 1f)
			{
				ExitOrbit();
			}
			if (MilMo_CameraController.Orbit && _gotFreeLookInput)
			{
				ReadOrbitInput();
				_useStickCollision = false;
			}
			if (!AutoReset && Time.time - _lastFreeLookTime > 60f)
			{
				AutoReset = true;
			}
			if (AutoReset && _hadMoveInput && !flag && (!HasMagnetTarget || Time.time - _lastNewMagnetTargetTime > 2f))
			{
				MilMo_EventSystem.At(0.5f, delegate
				{
					if (base.HookedUp && !_hadMoveInput && AutoReset && (!HasMagnetTarget || !(Time.time - _lastNewMagnetTargetTime <= 2f)))
					{
						bool flag2 = false;
						if (MilMo_Level.CurrentLevel != null)
						{
							flag2 = MilMo_Level.CurrentLevel.PlayerIsCloseToClimbingSurface();
						}
						if (!flag2 && (double)Math.Abs(MilMo_GameCameraController.CurrentDefaultLookup - 65f) > 0.0)
						{
							Vector3 position = MilMo_CameraController.Player.position;
							position.y += 0.5f;
							if (!Physics.Raycast(position, Vector3.down, out var hitInfo2, 10000f, 27787265) || hitInfo2.distance - 0.5f > 0.2f)
							{
								return;
							}
						}
						EnterReset(resetDistance: false, ResetSpeed.Medium);
					}
				});
			}
			if (MilMo_CameraController.Orbit && _useStickCollision)
			{
				if (!flag && (MilMo_GameCameraController.ResettingLookup || MilMo_GameCameraController.ResettingPan))
				{
					Vector3 vector = MilMo_CameraController.Player.position + MilMo_CameraController.HeadOffset;
					Quaternion quaternion = Quaternion.Euler(MilMo_GameCameraController.WantedLookup, MilMo_GameCameraController.WantedPan, 0f);
					if (!Physics.Linecast(vector + quaternion * new Vector3(0f, 0f, 0f - MilMo_GameCameraController.WantedDistance), vector, out var _, -838860833))
					{
						_useStickCollision = false;
					}
				}
				else
				{
					_useStickCollision = false;
				}
			}
			_hadMoveInput = flag;
		}
		else
		{
			_hadMoveInput = false;
		}
		HandleCursorLock(_gotFreeLookInput);
	}

	protected override void ExitOrbit()
	{
		base.ExitOrbit();
		MilMo_GameCameraController.CurrentTerrainLookup = MilMo_GameCameraController.Lookup;
		CalculateTerrainLookup();
		MilMo_GameCameraController.LookupBeforeTerrain = MilMo_GameCameraController.Lookup - MilMo_GameCameraController.RawTerrainLookup;
		EnterReset(resetDistance: false, ResetSpeed.Slow);
	}

	protected override void HandleLookupReset()
	{
		if (MilMo_GameCameraController.ResettingLookup && !ResetPaused)
		{
			MilMo_GameCameraController.LookupResetSpeed = Mathf.Min(MilMo_GameCameraController.LookupResetSpeed + Time.deltaTime * MilMo_GameCameraController.CurrentLookupResetAcceleration, MilMo_GameCameraController.CurrentMaxLookupResetSpeed);
			MilMo_GameCameraController.LookupBeforeTerrain = Mathf.Lerp(MilMo_GameCameraController.LookupBeforeTerrain, MilMo_GameCameraController.CurrentDefaultLookup, MilMo_GameCameraController.LookupResetSpeed * Time.deltaTime);
			if ((double)Mathf.Abs(MilMo_GameCameraController.LookupBeforeTerrain - MilMo_GameCameraController.CurrentDefaultLookup) < 0.01)
			{
				MilMo_GameCameraController.LookupBeforeTerrain = MilMo_GameCameraController.CurrentDefaultLookup;
				MilMo_GameCameraController.ResettingLookup = false;
			}
		}
	}

	protected override void EnterReset(bool resetDistance, ResetSpeed speed)
	{
		if (HasMagnetTarget)
		{
			HasMagnetTarget = false;
			_didResetSinceLastMagnetTarget = true;
		}
		base.EnterReset(resetDistance, speed);
	}

	private void MeleeAttack(object o)
	{
		if (!MilMo_CameraController.Orbit)
		{
			EnterReset(resetDistance: true, ResetSpeed.Medium);
		}
	}
}
