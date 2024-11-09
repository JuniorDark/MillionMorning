using System.Collections.Generic;
using Code.Core.Global;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using Core.Input;
using UnityEngine;

namespace Code.Core.Camera;

public abstract class MilMo_CameraController
{
	public delegate bool PlayerLockedCallback();

	public static Transform Player;

	public static Transform PlayerHead;

	public static Transform CameraTransform;

	public static UnityEngine.Camera CameraComponent;

	public static PlayerLockedCallback ThePlayerLockedCallback;

	protected static Vector3 CenterOffset = Vector3.zero;

	protected const float LOOK_AT_HEIGHT_OFFSET = 1.046f;

	protected static Vector3 HeadOffset;

	protected const float HEIGHT = 2.865f;

	private const float MIN_HEIGHT_ABOVE_GROUND = 0.5f;

	protected float DefaultDistance = 3.85f;

	private const float AUDIO_LISTENER_DISTANCE_FROM_PLAYER = 2.7f;

	private const float AUDIO_LISTENER_SQR_DISTANCE_FROM_PLAYER = 7.2900004f;

	private const float HIDE_CURSOR_TIME = 0.4f;

	private static bool _groundCollision;

	protected Vector3 LookAtPosition;

	protected static readonly List<MilMo_CameraMagnet> Magnets = new List<MilMo_CameraMagnet>();

	private static string _currentMagnetsFilePath = "";

	protected float FreeLookStartTime;

	public static bool Orbit { get; protected set; }

	public bool HookedUp { get; private set; }

	public static float Distance { get; set; }

	protected MilMo_CameraController()
	{
		Distance = DefaultDistance;
		HeadOffset = CenterOffset;
		HeadOffset.y += 1.046f;
		Orbit = false;
	}

	public virtual void HookUp()
	{
		HookedUp = true;
	}

	public virtual void Unhook()
	{
		HookedUp = false;
	}

	public static void UnloadCameraMagnets()
	{
		foreach (MilMo_CameraMagnet magnet in Magnets)
		{
			magnet.Destroy();
		}
		Magnets.Clear();
		_currentMagnetsFilePath = "";
	}

	public static void LoadCameraMagnets(string levelPath)
	{
		if (Magnets.Count > 0)
		{
			UnloadCameraMagnets();
		}
		_currentMagnetsFilePath = levelPath;
		MilMo_SimpleFormat.AsyncLoad(levelPath + "CameraMagnets", "Level", MilMo_ResourceManager.Priority.Medium, delegate(MilMo_SFFile file)
		{
			if (file != null && !(_currentMagnetsFilePath != levelPath))
			{
				while (file.NextRow())
				{
					MilMo_CameraMagnet milMo_CameraMagnet = MilMo_CameraMagnet.CreateFromFile(file);
					if (milMo_CameraMagnet != null)
					{
						Magnets.Add(milMo_CameraMagnet);
					}
				}
			}
		});
	}

	protected static void UpdateAudioListenerPosition()
	{
		if ((bool)CameraTransform && (bool)PlayerHead && (bool)MilMo_Global.AudioListener)
		{
			Vector3 vector = CameraTransform.position - PlayerHead.position;
			if (vector.sqrMagnitude < 7.2900004f)
			{
				MilMo_Global.AudioListener.transform.position = CameraTransform.position;
				return;
			}
			vector.Normalize();
			MilMo_Global.AudioListener.transform.position = PlayerHead.position + vector * 2.7f;
		}
	}

	protected static void SetupRotation(Vector3 target)
	{
		Vector3 position = CameraTransform.position;
		if (MilMo_Utility.Equals(target, position))
		{
			return;
		}
		float num = position.y - target.y;
		float num2 = Vector3.Distance(new Vector3(position.x, 0f, position.z), new Vector3(target.x, 0f, target.z));
		Vector3 vector = Vector3.forward * num2 + Vector3.down * num;
		if (!MilMo_Utility.Equals(vector, Vector3.zero))
		{
			CameraTransform.rotation = Quaternion.LookRotation(vector);
			Vector3 vector2 = target - position;
			vector2.y = 0f;
			if (!MilMo_Utility.Equals(vector2, Vector3.zero))
			{
				Quaternion quaternion = Quaternion.LookRotation(vector2);
				CameraTransform.rotation = quaternion * CameraTransform.rotation;
			}
		}
	}

	protected static void MoveAbove(float height)
	{
		Vector3 position = CameraTransform.position;
		if (position.y <= height + 0.5f && height - position.y < 20f)
		{
			position.y = height + 0.5f;
			_groundCollision = true;
			CameraTransform.position = position;
		}
		else if (_groundCollision)
		{
			_groundCollision = false;
		}
	}

	protected void HandleCursorLock(bool gotFreeLookInput)
	{
		if (!InputSwitch.IsMouseOverGameWindow)
		{
			return;
		}
		if (gotFreeLookInput)
		{
			if (Time.time - FreeLookStartTime <= 0.4f)
			{
				MilMo_Utility.SetUnlockedMode(ignoreFullScreen: true);
			}
			else if (Cursor.lockState != CursorLockMode.Locked)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}
		else if (Cursor.lockState == CursorLockMode.Locked)
		{
			MilMo_Utility.SetUnlockedMode();
			Cursor.visible = true;
		}
	}

	public static float GetAxisX()
	{
		return InputSwitch.GetAxisRaw("Mouse X") + InputSwitch.GetAxis("Camera X");
	}

	public static float GetAxisY()
	{
		return InputSwitch.GetAxisRaw("Mouse Y") + InputSwitch.GetAxis("Camera Y");
	}

	public static float GetAxisCameraX()
	{
		return InputSwitch.GetAxis("Camera X");
	}

	public static float GetAxisCameraY()
	{
		return InputSwitch.GetAxis("Camera Y");
	}
}
