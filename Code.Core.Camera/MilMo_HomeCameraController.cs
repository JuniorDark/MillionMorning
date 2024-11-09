using System;
using System.Linq;
using Code.Core.Input;
using Code.Core.Utility;
using Core.Settings;
using UnityEngine;

namespace Code.Core.Camera;

public sealed class MilMo_HomeCameraController : MilMo_SocialCameraController
{
	private enum Mode
	{
		Game,
		Furnishing
	}

	private const float FOV_GAME = 40f;

	private const float DISTANCE_GAME = 4.5f;

	private const float LOOKUP_GAME = 25f;

	private const float HEAD_SCALE_GAME = 1f;

	private const float FOV_FURNISHING = 20f;

	private const float DISTANCE_FURNISHING = 17f;

	private const float LOOKUP_FURNISHING = 40f;

	private const float HEAD_SCALE_FURNISHING = 1.5f;

	private const float MAX_DISTANCE_RESET_SPEED = 40f;

	private const float MAX_LOOKUP_RESET_SPEED = 32f;

	private const float MAX_FOV_RESET_SPEED = 24f;

	private const float DISTANCE_RESET_ACCELERATION = 18f;

	private const float LOOKUP_RESET_ACCELERATION = 8f;

	private const float FOV_RESET_ACCELERATION = 13f;

	private const float POSITION_DAMPING = 7.5f;

	private const float PAN_SPEED = 10f;

	private float _targetLookup = 25f;

	private float _targetFov = 40f;

	private float _targetDistance = 4.5f;

	private float _distanceResetSpeed;

	private float _lookupResetSpeed;

	private float _fovResetSpeed;

	private float _realWantedPan;

	private Mode _mode;

	public MilMo_HomeCameraController()
	{
		FieldOfView = 40f;
		MilMo_CameraController.Distance = 4.5f;
		Lookup = 25f;
		BodyPartScaleFactor = 1f;
	}

	public override void HookUp()
	{
		base.HookUp();
		SetMode(Mode.Game);
		FieldOfView = _targetFov;
		MilMo_CameraController.Distance = _targetDistance;
		Lookup = _targetLookup;
		_realWantedPan = WantedPan;
	}

	public override void Update()
	{
		if (base.HookedUp)
		{
			ApplyResetting();
			MilMo_CameraController.CameraComponent.fieldOfView = FieldOfView;
			base.Update();
		}
	}

	public void EnterFurnishingMode()
	{
		SetMode(Mode.Furnishing);
		MilMo_Input.ActivateFurnishingModeControlsModifier();
	}

	public void ExitFurnishingMode()
	{
		SetMode(Mode.Game);
		MilMo_Input.ResetToDefaultControlScheme();
	}

	private void SetMode(Mode mode)
	{
		if (mode == Mode.Furnishing)
		{
			GoToNearest90DegreeRotation();
			if (Cursor.lockState == CursorLockMode.Locked)
			{
				MilMo_Utility.SetUnlockedMode();
			}
		}
		switch (mode)
		{
		case Mode.Furnishing:
			_targetLookup = 40f;
			_targetFov = 20f;
			_targetDistance = 17f;
			BodyPartScaleFactor = 1.5f;
			_fovResetSpeed = 0f;
			_distanceResetSpeed = 0f;
			_lookupResetSpeed = 0f;
			break;
		case Mode.Game:
			_targetLookup = 25f;
			_targetFov = 40f;
			_targetDistance = 4.5f;
			BodyPartScaleFactor = 1f;
			_fovResetSpeed = 0f;
			_distanceResetSpeed = 0f;
			_lookupResetSpeed = 0f;
			break;
		}
		ApplyBodyPartScale();
		_realWantedPan = WantedPan;
		_mode = mode;
	}

	protected override void UpdateTransform()
	{
		if (_mode == Mode.Furnishing)
		{
			base.UpdateTransform();
			return;
		}
		LookAtPosition = MilMo_CameraController.Player.position;
		LookAtPosition.y += 1.046f;
		Pan = Mathf.LerpAngle(Pan, WantedPan, 5f * Time.deltaTime);
		bool num = Mathf.Abs(Mathf.DeltaAngle(Pan, WantedPan)) > 0.1f;
		Quaternion quaternion = Quaternion.Euler(Lookup, Pan, 0f);
		Vector3 vector = LookAtPosition + quaternion * new Vector3(0f, 0f, 0f - MilMo_CameraController.Distance);
		if (!num)
		{
			vector = Vector3.Lerp(MilMo_CameraController.CameraTransform.position, vector, 7.5f * Time.deltaTime);
		}
		MilMo_CameraController.CameraTransform.position = vector;
		MilMo_CameraController.CameraTransform.rotation = quaternion;
	}

	private void GoToNearest90DegreeRotation()
	{
		WantedPan = Mathf.Repeat(Mathf.RoundToInt(Mathf.Repeat(MilMo_CameraController.CameraTransform.eulerAngles.y, 360f) / 90f) * 90, 360f);
		_realWantedPan = WantedPan;
	}

	public void SetPan(float pan)
	{
		WantedPan = pan;
		_realWantedPan = WantedPan;
		if (_mode == Mode.Furnishing)
		{
			GoToNearest90DegreeRotation();
		}
		Pan = WantedPan;
		if (_mode != Mode.Furnishing && MilMo_CameraController.Player != null && MilMo_CameraController.CameraTransform != null)
		{
			LookAtPosition = MilMo_CameraController.Player.position;
			LookAtPosition.y += 1.046f;
			Quaternion quaternion = Quaternion.Euler(Lookup, Pan, 0f);
			Vector3 position = LookAtPosition + quaternion * new Vector3(0f, 0f, 0f - MilMo_CameraController.Distance);
			MilMo_CameraController.CameraTransform.position = position;
			MilMo_CameraController.CameraTransform.rotation = quaternion;
		}
	}

	public void RotateTo(float targetPan)
	{
		if (!MilMo_Input.GetKey(KeyCode.Mouse1, useKeyboardFocus: false))
		{
			WantedPan = targetPan;
			_realWantedPan = WantedPan;
		}
	}

	private void ApplyResetting()
	{
		if (!MilMo_Utility.Equals(MilMo_CameraController.Distance, _targetDistance, 0.01f))
		{
			_distanceResetSpeed = Mathf.Min(_distanceResetSpeed + Time.deltaTime * 18f, 40f);
			MilMo_CameraController.Distance = Mathf.Lerp(MilMo_CameraController.Distance, _targetDistance, _distanceResetSpeed * Time.deltaTime);
		}
		else
		{
			_distanceResetSpeed = 0f;
		}
		if (!MilMo_Utility.Equals(Lookup, _targetLookup, 0.01f))
		{
			_lookupResetSpeed = Mathf.Min(_lookupResetSpeed + Time.deltaTime * 8f, 32f);
			Lookup = Mathf.Lerp(Lookup, _targetLookup, _lookupResetSpeed * Time.deltaTime);
		}
		else
		{
			_lookupResetSpeed = 0f;
		}
		if (!MilMo_Utility.Equals(FieldOfView, _targetFov, 0.01f))
		{
			_fovResetSpeed = Mathf.Min(_fovResetSpeed + Time.deltaTime * 13f, 24f);
			FieldOfView = Mathf.Lerp(FieldOfView, _targetFov, _fovResetSpeed * Time.deltaTime);
		}
		else
		{
			_fovResetSpeed = 0f;
		}
	}

	protected override void ReadOrbitInput()
	{
		if (_mode == Mode.Game)
		{
			base.ReadOrbitInput();
			return;
		}
		float value = 10f * MilMo_CameraController.GetAxisX() * Settings.CameraSensitivity;
		MilMo_SocialCameraController.PanBuffer.AddLast(value);
		MilMo_SocialCameraController.PanBuffer.RemoveFirst();
		float num = MilMo_SocialCameraController.PanBuffer.Sum();
		_realWantedPan += num / (float)MilMo_SocialCameraController.PanBuffer.Count;
		float num2 = Mathf.Repeat(Mathf.RoundToInt(Mathf.Repeat(_realWantedPan, 360f) / 90f) * 90, 360f);
		float f = Mathf.DeltaAngle(Pan, num2);
		float f2 = Mathf.DeltaAngle(Pan, WantedPan);
		if ((double)Math.Abs(Mathf.Sign(f) - Mathf.Sign(f2)) >= 0.0 && (double)Math.Abs(Mathf.Sign(f2) - (float)Math.Sign(num)) <= 0.0)
		{
			num2 = WantedPan;
			_realWantedPan = WantedPan;
		}
		WantedPan = num2;
	}

	protected override void RotateLeftCallback(object o)
	{
		base.RotateLeftCallback(o);
		_realWantedPan = WantedPan;
	}

	protected override void RotateRightCallback(object o)
	{
		base.RotateRightCallback(o);
		_realWantedPan = WantedPan;
	}
}
