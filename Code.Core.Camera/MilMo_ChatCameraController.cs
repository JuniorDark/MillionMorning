using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.EventSystem;
using Code.Core.Input;
using Code.Core.Utility;
using Code.World.Chat.ChatRoom;
using Code.World.Player;
using Core.Settings;
using UnityEngine;

namespace Code.Core.Camera;

public class MilMo_ChatCameraController : MilMo_ActionCameraController
{
	private const float DEFAULT_LOOKUP = 7f;

	private const float LOOKUP_SPEED = 5.42f;

	private const float PAN_SPEED = 5.42f;

	private const int MOUSE_SMOOTHING_BUFFER_SIZE = 4;

	private const float ORBIT_DAMPING = 5f;

	private static readonly LinkedList<float> LookupBuffer = new LinkedList<float>();

	private static readonly LinkedList<float> PanBuffer = new LinkedList<float>();

	private static float _pan;

	private static float _wantedPan;

	private static float _lookup = 7f;

	private static float _wantedLookup = 7f;

	private const float MIN_LOOKUP = -75f;

	private const float MAX_LOOKUP = 87.5f;

	private Vector3 _orbitLookAtPosition;

	private Vector3 _targetOrbitLookAtPosition;

	private const float OVERVIEW_FOV = 45f;

	private const float ONE_TALKER_FOV = 30f;

	private const float PULL = 0.3f;

	private const float DRAG = 0.05f;

	private const float ANGLE_PULL = 0.5f;

	private const float ANGLE_DRAG = 0.1f;

	private const float Y_OFFSET = -0.15f;

	private const float MAX_SLIDE_DISTANCE = 0.7f;

	private const float MAX_SLIDE_DISTANCE_SQUARED = 0.48999998f;

	private MilMo_GenericReaction _focusSitPointReaction;

	private MilMo_GenericReaction _unFocusSitPointReaction;

	private MilMo_ChatRoom ChatRoom { get; set; }

	public MilMo_ChatCameraController()
	{
		MilMo_CameraController.Distance = 2.5f;
		Mover.StopLookAt();
		Mover.Drag = 0.05f;
		Mover.Pull = 0.3f;
		Mover.AnglePull = 0.5f;
		Mover.AngleDrag = 0.1f;
		Mover.MinVel = 1E-05f * Vector3.one;
		Mover.Shakes = false;
		for (int i = PanBuffer.Count; i < 4; i++)
		{
			PanBuffer.AddLast(0f);
		}
		for (int j = LookupBuffer.Count; j < 4; j++)
		{
			LookupBuffer.AddLast(0f);
		}
	}

	public void HookUp(MilMo_ChatRoom chatroom)
	{
		ChatRoom = chatroom;
		HookUp();
	}

	public override void HookUp()
	{
		base.HookUp();
		FocusMain(guaranteedSlide: true);
		Mover.StopLookAt();
		_focusSitPointReaction = MilMo_EventSystem.Listen("sitpoint_camera_focus", Focus);
		_focusSitPointReaction.Repeating = true;
		_unFocusSitPointReaction = MilMo_EventSystem.Listen("sitpoint_camera_release_focus", StopFocus);
		_unFocusSitPointReaction.Repeating = true;
		_targetOrbitLookAtPosition = MilMo_Player.Instance.Avatar.Head.position;
		_orbitLookAtPosition = _targetOrbitLookAtPosition;
		Quaternion rotation = MilMo_CameraController.CameraTransform.rotation;
		_pan = rotation.eulerAngles.y;
		_lookup = rotation.eulerAngles.x;
		_wantedPan = _pan;
		_wantedLookup = _lookup;
		MilMo_CameraController.CameraComponent.nearClipPlane = 0.1f;
	}

	public override void Unhook()
	{
		base.Unhook();
		MilMo_EventSystem.RemoveReaction(_focusSitPointReaction);
		_focusSitPointReaction = null;
		MilMo_EventSystem.RemoveReaction(_unFocusSitPointReaction);
		_unFocusSitPointReaction = null;
		if (Cursor.lockState == CursorLockMode.Locked)
		{
			MilMo_Utility.SetUnlockedMode();
			Cursor.visible = true;
		}
	}

	public override void Update()
	{
		if (base.HookedUp && ChatRoom != null)
		{
			HandleInput();
			if (MilMo_CameraController.Orbit)
			{
				ApplyOrbit();
			}
			MilMo_CameraController.UpdateAudioListenerPosition();
		}
	}

	public override void FixedUpdate()
	{
		if (base.HookedUp && !MilMo_CameraController.Orbit)
		{
			Mover.FixedUpdate();
		}
	}

	private void Focus(object sitPointAsObj)
	{
		if (!MilMo_CameraController.Orbit)
		{
			CalculateSitPointFocus(guaranteedSlide: false);
		}
	}

	private void StopFocus(object sitPointAsObj)
	{
		if (!MilMo_CameraController.Orbit)
		{
			CalculateSitPointFocus(guaranteedSlide: true);
		}
	}

	private void FocusMain(bool guaranteedSlide)
	{
		if (ChatRoom != null && ChatRoom.MainCameraPositions != null && ChatRoom.MainCameraPositions.Count > 0)
		{
			int index = ((ChatRoom.MainCameraPositions.Count != 1) ? UnityEngine.Random.Range(0, ChatRoom.MainCameraPositions.Count) : 0);
			GoTo(ChatRoom.MainCameraPositions[index].Position, ChatRoom.MainCameraPositions[index].EulerRotation, 45f, guaranteedSlide);
		}
	}

	private void GoTo(Vector3 position, Vector3 eulerRotation, float fov, bool guaranteedSlide)
	{
		if (guaranteedSlide || UnityEngine.Random.Range(0, 5) == 0)
		{
			Vector3 vector = position - MilMo_CameraController.CameraTransform.position;
			if (vector.sqrMagnitude > 0.48999998f)
			{
				vector.Normalize();
				GoToNow(position - vector * 0.7f);
			}
			GoTo(position);
		}
		else
		{
			GoToNow(position);
		}
		Mover.AngleNow(eulerRotation.x, eulerRotation.y, eulerRotation.z);
		Mover.ZoomToNow(fov);
	}

	private void CalculateSitPointFocus(bool guaranteedSlide)
	{
		List<MilMo_SitPoint> list = new List<MilMo_SitPoint>();
		foreach (MilMo_SitPoint sitPoint in ChatRoom.SitPoints)
		{
			if (sitPoint.CameraFocus)
			{
				list.Add(sitPoint);
			}
		}
		if (list.Count == 0)
		{
			FocusMain(guaranteedSlide);
			return;
		}
		Vector3 vector;
		Vector3 eulerAngles;
		float fov;
		if (list.Count == 1)
		{
			vector = list[0].GetPosition() + MilMo_CameraController.Distance * list[0].Occupant.GameObject.transform.forward;
			vector.y = ChatRoom.AverageHeadHeight + -0.15f;
			vector += (float)(UnityEngine.Random.Range(0, 2) * 2 - 1) * 1f * list[0].Occupant.GameObject.transform.right;
			Vector3 position = list[0].Occupant.Head.position;
			position.y = vector.y;
			eulerAngles = Quaternion.LookRotation(position - vector).eulerAngles;
			fov = 30f;
		}
		else
		{
			Vector3 zero = Vector3.zero;
			foreach (MilMo_SitPoint item in list)
			{
				zero += item.GetPosition();
			}
			zero /= (float)list.Count;
			Vector3 zero2 = Vector3.zero;
			foreach (MilMo_SitPoint item2 in list)
			{
				zero2 += item2.Occupant.GameObject.transform.forward;
			}
			zero2.Normalize();
			vector = zero + zero2 * MilMo_CameraController.Distance;
			vector.y = ChatRoom.AverageHeadHeight + -0.15f;
			Vector3 vector2 = zero;
			vector2.y = vector.y;
			eulerAngles = Quaternion.LookRotation(vector2 - vector).eulerAngles;
			float num = 0f;
			for (int i = 0; i < list.Count - 1; i++)
			{
				for (int j = i + 1; j < list.Count; j++)
				{
					float sqrMagnitude = (list[j].GetPosition() - list[i].GetPosition()).sqrMagnitude;
					if (sqrMagnitude > num)
					{
						num = sqrMagnitude;
					}
				}
			}
			float num2 = Mathf.Sqrt(num);
			fov = Mathf.Max(30f + num2, 45f);
		}
		GoTo(vector, eulerAngles, fov, guaranteedSlide);
	}

	private void HandleInput()
	{
		if (MilMo_Input.GetKey(KeyCode.Mouse1, useKeyboardFocus: false))
		{
			if (Cursor.lockState != CursorLockMode.Locked)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			if (!MilMo_CameraController.Orbit)
			{
				EnterOrbit();
			}
		}
		else if ((double)Math.Abs(MilMo_CameraController.GetAxisCameraX()) > 0.0 || (double)Math.Abs(MilMo_CameraController.GetAxisCameraY()) > 0.0)
		{
			if (!MilMo_CameraController.Orbit)
			{
				EnterOrbit();
			}
		}
		else
		{
			if (Cursor.lockState == CursorLockMode.Locked)
			{
				MilMo_Utility.SetUnlockedMode();
				Cursor.visible = true;
			}
			if (MilMo_CameraController.Orbit)
			{
				ExitOrbit();
			}
		}
		if (MilMo_CameraController.Orbit)
		{
			ReadOrbitInput();
		}
	}

	private static void ReadOrbitInput()
	{
		if (MilMo_CameraController.Orbit)
		{
			float value = -5.42f * MilMo_CameraController.GetAxisY() * Settings.CameraSensitivity;
			LookupBuffer.AddLast(value);
			LookupBuffer.RemoveFirst();
			float num = LookupBuffer.Sum();
			_wantedLookup += num / (float)LookupBuffer.Count;
			float value2 = 5.42f * MilMo_CameraController.GetAxisX() * Settings.CameraSensitivity;
			PanBuffer.AddLast(value2);
			PanBuffer.RemoveFirst();
			float num2 = PanBuffer.Sum();
			_wantedPan += num2 / (float)PanBuffer.Count;
		}
	}

	private void EnterOrbit()
	{
		MilMo_CameraController.Orbit = true;
		Quaternion rotation = MilMo_CameraController.CameraTransform.rotation;
		_pan = rotation.eulerAngles.y;
		_lookup = rotation.eulerAngles.x;
		_wantedPan = _pan;
		_wantedLookup = _lookup;
		MilMo_CameraController.CameraComponent.fieldOfView = 45f;
		_orbitLookAtPosition = MilMo_CameraController.CameraTransform.position + MilMo_CameraController.CameraTransform.forward * MilMo_CameraController.Distance;
		for (LinkedListNode<float> linkedListNode = LookupBuffer.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			linkedListNode.Value = 0f;
		}
		for (LinkedListNode<float> linkedListNode2 = PanBuffer.First; linkedListNode2 != null; linkedListNode2 = linkedListNode2.Next)
		{
			linkedListNode2.Value = 0f;
		}
	}

	private void ExitOrbit()
	{
		MilMo_CameraController.Orbit = false;
		FocusMain(guaranteedSlide: true);
	}

	private void ApplyOrbit()
	{
		_orbitLookAtPosition = Vector3.Lerp(_orbitLookAtPosition, _targetOrbitLookAtPosition, 5f * Time.deltaTime);
		_lookup = Mathf.Lerp(_lookup, _wantedLookup, 5f * Time.deltaTime);
		_lookup = Mathf.Clamp(_lookup, -75f, 87.5f);
		_pan = Mathf.Lerp(_pan, _wantedPan, 5f * Time.deltaTime);
		Quaternion quaternion = Quaternion.Euler(_lookup, _pan, 0f);
		Vector3 position = _orbitLookAtPosition + quaternion * new Vector3(0f, 0f, 0f - MilMo_CameraController.Distance);
		MilMo_CameraController.CameraTransform.position = position;
		MilMo_CameraController.CameraTransform.rotation = quaternion;
	}
}
