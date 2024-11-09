using System;
using System.Linq;
using Code.Core.GUI.Core;
using Code.Core.Input;
using Code.World.Level;
using Core.Input;
using Core.Settings;
using UnityEngine;

namespace Code.Core.Camera;

public sealed class MilMo_RpgCameraController : MilMo_GameCameraController
{
	private const float ZOOM_SPEED = 1f;

	private const float ZOOM_DAMPING = 3f;

	private const float ZOOM_LOOKUP_SPEED = 5f;

	private const float ACCELERATION = 10f;

	private const float ORBIT_DAMPING = 8f;

	private const float ROTATION_SPEED = 200f;

	private const float MIN_DISTANCE = 1.146f;

	private const float MAX_DISTANCE = 11.46f;

	private const float AUTO_RESET_FREE_LOOK_TIMEOUT = 60f;

	private const float DEFAULT_ZOOM = 4.5f;

	private const float MIN_ZOOM_LOOKUP = -10f;

	private const float MAX_ZOOM_LOOKUP = 30f;

	private const float LOOKUP_FUNCTION_COEFFICIENT = 3.8782237f;

	private const float LOOKUP_FUNCTION_OFFSET = -17.452007f;

	private float _freeLookLookupModifier;

	private float _terrainLookupModifier;

	private static Vector3 _noCollisionPosition;

	private static Vector3 _noCollisionForward;

	private bool _gotFreeLookInput;

	private float _lastFreeLookTime;

	private float _wantedZoom = 4.5f;

	public MilMo_RpgCameraController()
	{
		DefaultDistance = 4.5f;
	}

	public override void HookUp()
	{
		base.HookUp();
		if (MilMo_Input.DefaultMode == MilMo_Input.ControlMode.Mmorpg)
		{
			MilMo_Input.SetControlMode(MilMo_Input.ControlMode.Mmorpg);
		}
		if (MilMo_Level.CurrentLevel != null)
		{
			MilMo_CameraController.CameraComponent.backgroundColor = MilMo_Level.CurrentLevel.Environment.BackgroundColor;
		}
		MilMo_CameraController.CameraComponent.nearClipPlane = 0.1f;
		MilMo_CameraController.CameraComponent.fieldOfView = 60f;
		EnterReset(resetDistance: false, ResetSpeed.Slow);
		MilMo_GameCameraController.Pan = MilMo_CameraController.CameraTransform.eulerAngles.y;
		MilMo_GameCameraController.ResettingPan = false;
		MilMo_GameCameraController.WantedPan = MilMo_GameCameraController.Pan;
		MilMo_CameraController.Orbit = true;
		_noCollisionPosition = MilMo_CameraController.CameraTransform.position;
		_noCollisionForward = MilMo_CameraController.CameraTransform.forward;
	}

	public override void SetupPosition()
	{
		base.SetupPosition();
		MilMo_CameraController.Distance = _wantedZoom;
		MilMo_GameCameraController.WantedDistance = _wantedZoom;
		MilMo_GameCameraController.ResettingDistance = false;
		MilMo_GameCameraController.ValidatePosition();
	}

	public void Update()
	{
		if (base.HookedUp)
		{
			CheckInput();
			CheckEdgeLookDown();
			ApplyResetting();
			ApplyPlayerPosition(MilMo_CameraController.Player.position + MilMo_CameraController.HeadOffset);
			MilMo_CameraController.UpdateAudioListenerPosition();
		}
	}

	private void CheckInput()
	{
		bool gotFreeLookInput = _gotFreeLookInput;
		_gotFreeLookInput = false;
		if (!MilMo_UserInterface.KeyboardFocus)
		{
			bool key = MilMo_Input.GetKey(KeyCode.Mouse1, useKeyboardFocus: false);
			bool key2 = MilMo_Input.GetKey(KeyCode.Mouse0, useKeyboardFocus: false, useMouseFocus: true);
			if ((double)Math.Abs(MilMo_Input.VerticalAxis) > 0.0 || ((double)Math.Abs(MilMo_Input.HorizontalAxis) > 0.0 && (MilMo_CameraController.ThePlayerLockedCallback == null || !MilMo_CameraController.ThePlayerLockedCallback())))
			{
				MilMo_GameCameraController.ResettingPan = false;
			}
			float axis = InputSwitch.GetAxis("Camera X");
			float axis2 = InputSwitch.GetAxis("Camera Y");
			if (key || key2 || (double)Math.Abs(axis) > 0.0 || (double)Math.Abs(axis2) > 0.0)
			{
				if (!gotFreeLookInput)
				{
					EnterOrbit();
				}
				_lastFreeLookTime = Time.time;
				AutoReset = false;
				_gotFreeLookInput = true;
				ReadOrbitInput();
			}
			else if (gotFreeLookInput)
			{
				ExitOrbit();
				_gotFreeLookInput = false;
			}
			if (!AutoReset && Time.time - _lastFreeLookTime > 60f)
			{
				AutoReset = true;
			}
			if ((double)Math.Abs(MilMo_Input.RotationAxis) > 0.0)
			{
				float num = MilMo_GameCameraController.WantedPan + MilMo_Input.RotationAxis * Time.deltaTime * 200f;
				float f = Mathf.DeltaAngle(MilMo_GameCameraController.Pan, num);
				float f2 = Mathf.DeltaAngle(MilMo_GameCameraController.Pan, MilMo_GameCameraController.WantedPan);
				if ((double)Math.Abs(Mathf.Sign(f) - Mathf.Sign(f2)) > 0.0 && (double)Math.Abs(Mathf.Sign(f2) - (float)Math.Sign(MilMo_Input.RotationAxis)) < 0.0)
				{
					num = MilMo_GameCameraController.Pan + Mathf.Sign(MilMo_Input.RotationAxis) * 179.9f;
				}
				MilMo_GameCameraController.WantedPan = num;
			}
			Zoom();
		}
		HandleCursorLock(_gotFreeLookInput);
	}

	protected override void HandleLookupReset()
	{
		if (MilMo_GameCameraController.ResettingLookup && !ResetPaused)
		{
			MilMo_GameCameraController.LookupResetSpeed = Mathf.Min(MilMo_GameCameraController.LookupResetSpeed + Time.deltaTime * MilMo_GameCameraController.CurrentLookupResetAcceleration, MilMo_GameCameraController.CurrentMaxLookupResetSpeed);
			_freeLookLookupModifier = Mathf.Lerp(_freeLookLookupModifier, 0f, MilMo_GameCameraController.LookupResetSpeed * Time.deltaTime);
			if ((double)Mathf.Abs(_freeLookLookupModifier) < 0.01)
			{
				_freeLookLookupModifier = 0f;
				MilMo_GameCameraController.ResettingLookup = false;
			}
		}
	}

	protected override void ReadOrbitInput(bool readLookup = true, bool readPan = true)
	{
		if (readLookup)
		{
			float num = MilMo_CameraController.GetAxisY();
			if (Mathf.Abs(num) > 3f)
			{
				num = Mathf.Sign(num) * 3f;
			}
			float value = -4.42f * num * Settings.CameraSensitivity;
			MilMo_GameCameraController.LookupBuffer.AddLast(value);
			MilMo_GameCameraController.LookupBuffer.RemoveFirst();
			float num2 = MilMo_GameCameraController.LookupBuffer.Sum();
			_freeLookLookupModifier += num2 / (float)MilMo_GameCameraController.LookupBuffer.Count;
			_freeLookLookupModifier = Mathf.Clamp(_freeLookLookupModifier, -50f, 80f);
		}
		if (readPan)
		{
			base.ReadOrbitInput(readLookup: false);
		}
	}

	private void Zoom()
	{
		float zoomAxis = MilMo_Input.ZoomAxis;
		if ((double)Math.Abs(zoomAxis) < 0.0 && MilMo_GameCameraController.ResettingDistance)
		{
			_wantedZoom = MilMo_CameraController.Distance;
		}
		else if ((double)Math.Abs(zoomAxis) > 0.0)
		{
			if (MilMo_GameCameraController.ResettingDistance)
			{
				_wantedZoom = MilMo_CameraController.Distance;
				MilMo_GameCameraController.ResettingDistance = false;
			}
			_wantedZoom += zoomAxis * 1f;
			_wantedZoom = Mathf.Min(Mathf.Max(_wantedZoom, 1.146f), 11.46f);
		}
	}

	private void ApplyPlayerPosition(Vector3 aTargetCenter)
	{
		if (!MilMo_GameCameraController.ResettingDistance)
		{
			MilMo_CameraController.Distance = (Vector3.Dot(_noCollisionPosition - aTargetCenter, _noCollisionForward) * _noCollisionForward).magnitude;
			if ((double)Math.Abs(MilMo_CameraController.Distance - _wantedZoom) > 0.0)
			{
				MilMo_CameraController.Distance = Mathf.Lerp(MilMo_CameraController.Distance, _wantedZoom, 10f * Time.deltaTime);
			}
		}
		float num = 3.8782237f * MilMo_CameraController.Distance + -17.452007f;
		if ((double)Math.Abs(MilMo_GameCameraController.CurrentDefaultLookup - 65f) < 0.0)
		{
			if (AutoReset)
			{
				_terrainLookupModifier = MilMo_GameCameraController.CurrentDefaultLookup;
			}
			else
			{
				_terrainLookupModifier = 0f;
			}
		}
		else
		{
			CalculateTerrainLookup();
			_terrainLookupModifier = MilMo_GameCameraController.RawTerrainLookup;
		}
		float num2 = _freeLookLookupModifier;
		if ((_terrainLookupModifier < 0f && num2 > _terrainLookupModifier) || (_terrainLookupModifier > 0f && num2 < _terrainLookupModifier))
		{
			num2 += Mathf.Sign(_terrainLookupModifier) * Mathf.Min(Mathf.Abs(num2 - _terrainLookupModifier), Mathf.Abs(_terrainLookupModifier));
		}
		MilMo_GameCameraController.WantedLookup = num + num2;
		MilMo_GameCameraController.Lookup = Mathf.LerpAngle(MilMo_GameCameraController.Lookup, MilMo_GameCameraController.WantedLookup, 8f * Time.deltaTime);
		MilMo_GameCameraController.Lookup = Mathf.Repeat(MilMo_GameCameraController.Lookup, 360f);
		if (MilMo_GameCameraController.Lookup > 180f)
		{
			MilMo_GameCameraController.Lookup -= 360f;
		}
		MilMo_GameCameraController.Lookup = Mathf.Clamp(MilMo_GameCameraController.Lookup, -50f, 87.5f);
		float num3 = (MilMo_GameCameraController.ResettingPan ? MilMo_GameCameraController.PanResetSpeed : 8f);
		MilMo_GameCameraController.Pan = Mathf.LerpAngle(MilMo_GameCameraController.Pan, MilMo_GameCameraController.WantedPan, num3 * Time.deltaTime);
		Quaternion quaternion = Quaternion.Euler(MilMo_GameCameraController.Lookup + EffectRotation.x, MilMo_GameCameraController.Pan + EffectRotation.y, 0f);
		Vector3 vector = (_noCollisionPosition = aTargetCenter + quaternion * new Vector3(0f, 0f, 0f - MilMo_CameraController.Distance));
		_noCollisionForward = (quaternion * Vector3.forward).normalized;
		if (Physics.Linecast(MilMo_CameraController.CameraTransform.position, vector, out var hitInfo, -838860833) && Physics.Linecast(aTargetCenter, vector, out hitInfo, -838860833))
		{
			Vector3 normalized = (aTargetCenter - vector).normalized;
			vector = hitInfo.point + normalized * 0.425f;
		}
		MilMo_CameraController.CameraTransform.position = vector + EffectPosition;
		MilMo_CameraController.CameraTransform.rotation = quaternion;
	}
}
