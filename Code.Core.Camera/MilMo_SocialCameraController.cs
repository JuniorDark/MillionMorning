using System;
using System.Collections.Generic;
using Code.Core.Avatar;
using Code.Core.Input;
using Code.Core.Sound;
using Code.Core.Utility;
using Code.Core.Visual;
using Code.World;
using Code.World.Level.LevelObject;
using Code.World.Player;
using Core.Input;
using Core.Settings;
using UnityEngine;

namespace Code.Core.Camera;

public class MilMo_SocialCameraController : MilMo_CameraController
{
	protected float FieldOfView = 20f;

	protected float Lookup = 50f;

	private float _lodFactor = 0.04f;

	private float _defaultPan;

	public float BodyPartScaleFactor = 2f;

	private const float FIXED_DISTANCE = 35f;

	private const float PAN_SPEED = 5.42f;

	private const float ROTATION_SPEED = 200f;

	protected const float ORBIT_DAMPING = 5f;

	private const int MOUSE_SMOOTHING_BUFFER_SIZE = 4;

	protected float Pan;

	protected float WantedPan;

	protected static readonly LinkedList<float> PanBuffer = new LinkedList<float>();

	private bool _gotFreeLookInput;

	private float _lodFactorResetVal = 1f;

	private Color _backgroundColorResetVal;

	public MilMo_SocialCameraController()
	{
		for (int i = PanBuffer.Count; i < 4; i++)
		{
			PanBuffer.AddLast(0f);
		}
	}

	public virtual void Update()
	{
		if (base.HookedUp)
		{
			HandleInput();
			UpdateTransform();
			MilMo_CameraController.UpdateAudioListenerPosition();
		}
	}

	public override void HookUp()
	{
		base.HookUp();
		CreateRotateButtons();
		MilMo_CameraController.CameraComponent.fieldOfView = FieldOfView;
		MilMo_CameraController.CameraComponent.nearClipPlane = 0.5f;
		BodyPartScaleFactor = 2f;
		ApplyBodyPartScale();
		_lodFactorResetVal = MilMo_Lod.GlobalLodFactor;
		MilMo_Lod.GlobalLodFactor = _lodFactor;
		_backgroundColorResetVal = MilMo_CameraController.CameraComponent.backgroundColor;
		MilMo_CameraController.CameraComponent.backgroundColor = Color.black;
		SetupPosition();
		MilMo_Player.Instance.EnterRoom();
	}

	public void SetupPosition()
	{
		MilMo_CameraController.Orbit = true;
		LookAtPosition = MilMo_CameraController.Player.position;
		LookAtPosition.y += 1.046f;
		Pan = _defaultPan;
		WantedPan = Pan;
		Quaternion quaternion = Quaternion.Euler(Lookup, Pan, 0f);
		Vector3 position = LookAtPosition + quaternion * new Vector3(0f, 0f, -35f);
		MilMo_CameraController.CameraTransform.position = position;
		MilMo_CameraController.CameraTransform.rotation = quaternion;
	}

	public override void Unhook()
	{
		base.Unhook();
		DestroyRotateButtons();
		MilMo_Avatar.HeadScale = 1f;
		MilMo_Avatar.FeetScale = 1f;
		MilMo_Avatar.HandsScale = 1f;
		MilMo_LevelNpc.HeadScale = 1f;
		MilMo_Lod.GlobalLodFactor = _lodFactorResetVal;
		MilMo_CameraController.CameraComponent.backgroundColor = _backgroundColorResetVal;
		MilMo_Player.Instance.ExitRoom();
		if (Cursor.lockState == CursorLockMode.Locked)
		{
			MilMo_Utility.SetUnlockedMode();
			Cursor.visible = true;
		}
	}

	public void ApplyBodyPartScale()
	{
		MilMo_Avatar.HeadScale = BodyPartScaleFactor;
		MilMo_Avatar.HandsScale = BodyPartScaleFactor;
		MilMo_Avatar.FeetScale = BodyPartScaleFactor;
		MilMo_LevelNpc.HeadScale = BodyPartScaleFactor;
	}

	protected virtual void UpdateTransform()
	{
		LookAtPosition = MilMo_CameraController.Player.position;
		LookAtPosition.y += 1.046f;
		Pan = Mathf.LerpAngle(Pan, WantedPan, 5f * Time.deltaTime);
		Quaternion quaternion = Quaternion.Euler(Lookup, Pan, 0f);
		Vector3 position = LookAtPosition + quaternion * new Vector3(0f, 0f, -35f);
		MilMo_CameraController.CameraTransform.position = position;
		MilMo_CameraController.CameraTransform.rotation = quaternion;
	}

	protected virtual void ReadOrbitInput()
	{
		float value = 5.42f * (InputSwitch.GetAxisRaw("Mouse X") + InputSwitch.GetAxis("Camera X")) * Settings.CameraSensitivity;
		PanBuffer.AddLast(value);
		PanBuffer.RemoveFirst();
		float num = 0f;
		foreach (float item in PanBuffer)
		{
			num += item;
		}
		float num2 = WantedPan + num / (float)PanBuffer.Count;
		float f = Mathf.DeltaAngle(Pan, num2);
		float f2 = Mathf.DeltaAngle(Pan, WantedPan);
		if (Mathf.Sign(f) != Mathf.Sign(f2) && Mathf.Sign(f2) == (float)Math.Sign(num))
		{
			num2 = Pan + Mathf.Sign(num) * 179.9f;
		}
		WantedPan = num2;
	}

	private void HandleInput()
	{
		bool gotFreeLookInput = _gotFreeLookInput;
		_gotFreeLookInput = MilMo_Input.GetKey(KeyCode.Mouse1, useKeyboardFocus: false) || (MilMo_Input.DefaultMode == MilMo_Input.ControlMode.Mmorpg && MilMo_Input.GetKey(KeyCode.Mouse0, useKeyboardFocus: false, useMouseFocus: true));
		if (_gotFreeLookInput)
		{
			if (!gotFreeLookInput)
			{
				FreeLookStartTime = Time.time;
				for (LinkedListNode<float> linkedListNode = PanBuffer.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
				{
					linkedListNode.Value = 0f;
				}
			}
			ReadOrbitInput();
		}
		if (MilMo_Input.RotationAxis != 0f)
		{
			float num = WantedPan + MilMo_Input.RotationAxis * Time.deltaTime * 200f;
			float f = Mathf.DeltaAngle(Pan, num);
			float f2 = Mathf.DeltaAngle(Pan, WantedPan);
			if (Mathf.Sign(f) != Mathf.Sign(f2) && Mathf.Sign(f2) == (float)Math.Sign(MilMo_Input.RotationAxis))
			{
				num = Pan + Mathf.Sign(MilMo_Input.RotationAxis) * 179.9f;
			}
			WantedPan = num;
		}
		HandleCursorLock(_gotFreeLookInput);
	}

	private void CreateRotateButtons()
	{
		MilMo_World.HudHandler.AddRotateCameraButtons(RotateLeftCallback, RotateRightCallback);
	}

	protected virtual void RotateLeftCallback(object o)
	{
		for (LinkedListNode<float> linkedListNode = PanBuffer.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			linkedListNode.Value = 0f;
		}
		Pan = Mathf.Repeat(Pan, 360f);
		float num = Pan / 90f;
		float num2 = Mathf.Ceil(num);
		if (Mathf.Abs(num - num2) < 0.25f)
		{
			num2 = Mathf.Repeat(num2 + 1f, 4f);
		}
		WantedPan = 90f * num2;
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.SelectMainCat);
	}

	protected virtual void RotateRightCallback(object o)
	{
		for (LinkedListNode<float> linkedListNode = PanBuffer.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			linkedListNode.Value = 0f;
		}
		Pan = Mathf.Repeat(Pan, 360f);
		float num = Pan / 90f;
		float num2 = Mathf.Floor(num);
		if (Mathf.Abs(num - num2) < 0.25f)
		{
			num2 = Mathf.Repeat(num2 - 1f, 4f);
		}
		WantedPan = 90f * num2;
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.SelectMainCat);
	}

	private static void DestroyRotateButtons()
	{
		MilMo_World.HudHandler.RemoveRotateCameraButtons();
	}
}
