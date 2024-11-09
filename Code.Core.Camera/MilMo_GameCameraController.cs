using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.Core.Collision;
using Code.Core.EventSystem;
using Code.Core.Input;
using Code.Core.Utility;
using Code.Core.Visual.Water;
using Code.World.Level;
using Code.World.Player;
using Core.Settings;
using UnityEngine;

namespace Code.Core.Camera;

public abstract class MilMo_GameCameraController : MilMo_CameraController
{
	protected enum ResetSpeed
	{
		Slow,
		Medium,
		Fast
	}

	protected const float FIELD_OF_VIEW = 60f;

	protected const float DEFAULT_LOOKUP = 7f;

	protected const float MIN_LOOKUP = -50f;

	protected const float MAX_LOOKUP = 87.5f;

	protected const float DEFAULT_EDGE_LOOKUP = 65f;

	protected const float MAX_WANTED_LOOKUP = 80f;

	private const float TERRAIN_ADJUSTMENT_HEIGHT_ABOVE_GROUND_LIMIT = 0.5f;

	private static readonly Vector3 EdgeDetectionRayCastOffset;

	private static readonly Vector3 EdgeDetectionRayCastOffsetOnTerrain;

	private const float EDGE_HEIGHT_LIMIT = 1.5f;

	protected const float LOOKUP_SPEED = 4.42f;

	protected const float PAN_SPEED = 4.42f;

	private const float TERRAIN_ADJUSTMENT_SPEED = 5f;

	private const float MAX_DISTANCE_RESET_SPEED = 5f;

	private const float DEFAULT_MAX_LOOKUP_RESET_SPEED = 2f;

	private const float DEFAULT_MAX_PAN_RESET_SPEED = 2f;

	private const float DISTANCE_RESET_ACCELERATION = 3f;

	protected const float LOOKUP_RESET_ACCELERATION = 1f;

	protected const float PAN_RESET_ACCELERATION = 1f;

	private const float FOV_RESET_SPEED = 10f;

	private const int MOUSE_SMOOTHING_BUFFER_SIZE = 4;

	private const float TERRAIN_ADJUSTMENT_SAMPLE_RADIUS = 2f;

	protected const float TERRAIN_LOOKUP_DAMPING = 5f;

	protected static float WantedDistance;

	protected static bool ResettingPan;

	protected static bool ResettingLookup;

	protected static bool ResettingDistance;

	protected bool ResetPaused;

	protected bool AutoReset = true;

	private static float _distanceResetSpeed;

	protected static float LookupResetSpeed;

	protected static float PanResetSpeed;

	private static float _currentDistanceResetAcceleration;

	protected static float CurrentLookupResetAcceleration;

	protected static float CurrentPanResetAcceleration;

	protected static float CurrentMaxLookupResetSpeed;

	protected static float CurrentMaxPanResetSpeed;

	protected static float CurrentDefaultLookup;

	protected static float Lookup;

	protected static float WantedLookup;

	private static float _wantedTerrainLookup;

	protected static float CurrentTerrainLookup;

	protected static float RawTerrainLookup;

	protected static float LookupBeforeTerrain;

	protected bool UseEdgeLookDown = true;

	protected static float Pan;

	protected static float WantedPan;

	protected bool HasMagnetTarget;

	protected static readonly LinkedList<float> LookupBuffer;

	protected static readonly LinkedList<float> PanBuffer;

	private MilMo_GenericReaction _resetButtonReaction;

	protected const int CAMERA_COLLISION_LAYERS = -838860833;

	protected const float MIN_COLLISION_DISTANCE = 0.425f;

	public Vector3 EffectRotation;

	public Vector3 EffectPosition;

	static MilMo_GameCameraController()
	{
		EdgeDetectionRayCastOffset = new Vector3(0f, 4.5f, 1.12f);
		EdgeDetectionRayCastOffsetOnTerrain = new Vector3(0f, 4.5f, 2f);
		_currentDistanceResetAcceleration = 3f;
		CurrentLookupResetAcceleration = 1f;
		CurrentPanResetAcceleration = 1f;
		CurrentMaxLookupResetSpeed = 2f;
		CurrentMaxPanResetSpeed = 2f;
		CurrentDefaultLookup = 7f;
		Lookup = 7f;
		WantedLookup = 7f;
		LookupBeforeTerrain = 7f;
		LookupBuffer = new LinkedList<float>();
		PanBuffer = new LinkedList<float>();
	}

	protected MilMo_GameCameraController()
	{
		WantedDistance = DefaultDistance;
		for (int i = PanBuffer.Count; i < 4; i++)
		{
			PanBuffer.AddLast(0f);
		}
		for (int j = LookupBuffer.Count; j < 4; j++)
		{
			LookupBuffer.AddLast(0f);
		}
	}

	public override void HookUp()
	{
		base.HookUp();
		_resetButtonReaction = MilMo_EventSystem.Listen("button_ResetCamera", delegate
		{
			Reset();
		});
		_resetButtonReaction.Repeating = true;
	}

	public override void Unhook()
	{
		base.Unhook();
		MilMo_EventSystem.RemoveReaction(_resetButtonReaction);
		_resetButtonReaction = null;
		if (Cursor.lockState == CursorLockMode.Locked)
		{
			MilMo_Utility.SetUnlockedMode();
			Cursor.visible = true;
		}
	}

	public virtual void SetupPosition()
	{
		MilMo_CameraController.Distance = DefaultDistance;
		Pan = MilMo_CameraController.Player.eulerAngles.y;
		WantedPan = Pan;
		Lookup = 7f;
		WantedLookup = Lookup;
		Vector3 position = MilMo_CameraController.Player.position;
		Vector3 position2 = position + MilMo_CameraController.CenterOffset;
		position2 += Quaternion.Euler(7f, Pan, 0f) * new Vector3(0f, 0f, 0f - MilMo_CameraController.Distance);
		MilMo_CameraController.CameraTransform.position = position2;
		MilMo_CameraController.SetupRotation(position + MilMo_CameraController.HeadOffset);
	}

	public static void ValidatePosition()
	{
		if (Physics.Linecast(MilMo_CameraController.Player.position + MilMo_CameraController.HeadOffset, MilMo_CameraController.CameraTransform.position, out var hitInfo, -838860833))
		{
			Vector3 normalized = (MilMo_CameraController.Player.position + MilMo_CameraController.HeadOffset - MilMo_CameraController.CameraTransform.position).normalized;
			MilMo_CameraController.CameraTransform.position = hitInfo.point + normalized * 0.425f;
		}
	}

	protected void ApplyResetting()
	{
		if (ResettingDistance && !ResetPaused)
		{
			_distanceResetSpeed = Mathf.Min(_distanceResetSpeed + Time.deltaTime * _currentDistanceResetAcceleration, 5f);
			MilMo_CameraController.Distance = Mathf.Lerp(MilMo_CameraController.Distance, DefaultDistance, _distanceResetSpeed * Time.deltaTime);
			if ((double)Mathf.Abs(MilMo_CameraController.Distance - DefaultDistance) < 0.01)
			{
				MilMo_CameraController.Distance = DefaultDistance;
				ResettingDistance = false;
			}
		}
		HandleLookupReset();
		if (ResettingPan && !ResetPaused)
		{
			PanResetSpeed = Mathf.Min(PanResetSpeed + Time.deltaTime * CurrentPanResetAcceleration, CurrentMaxPanResetSpeed);
			if (!HasMagnetTarget)
			{
				WantedPan = MilMo_CameraController.Player.transform.rotation.eulerAngles.y;
			}
			if (Mathf.Abs(Mathf.DeltaAngle(Pan, WantedPan)) < 0.01f)
			{
				Pan = WantedPan;
				ResettingPan = false;
			}
		}
		if ((double)Math.Abs(MilMo_CameraController.CameraComponent.fieldOfView - 60f) > 0.0)
		{
			MilMo_CameraController.CameraComponent.fieldOfView = Mathf.Lerp(MilMo_CameraController.CameraComponent.fieldOfView, 60f, 10f * Time.deltaTime);
			if ((double)Mathf.Abs(MilMo_CameraController.CameraComponent.fieldOfView - 60f) < 0.01)
			{
				MilMo_CameraController.CameraComponent.fieldOfView = 60f;
			}
		}
	}

	protected abstract void HandleLookupReset();

	protected void CheckEdgeLookDown()
	{
		if (!MilMo_CameraController.Player)
		{
			return;
		}
		CurrentDefaultLookup = 7f;
		if (MilMo_PlayerControllerBase.IsInWater || !UseEdgeLookDown)
		{
			return;
		}
		Vector3 position = MilMo_CameraController.Player.position;
		Vector3 pos = position;
		pos.y += 0.5f;
		float worldHeight = MilMo_Physics.GetWorldHeight(pos, 95158273);
		if (!(position.y - worldHeight > 0.5f))
		{
			float terrainHeight = MilMo_Physics.GetTerrainHeight(position);
			bool flag = Mathf.Abs(position.y - terrainHeight) < 0.2f;
			Vector3 vector = MilMo_CameraController.Player.TransformPoint(flag ? EdgeDetectionRayCastOffsetOnTerrain : EdgeDetectionRayCastOffset);
			float num = MilMo_Physics.GetWorldHeight(vector, 6815745);
			Vector3 position2 = vector;
			position2.y = num;
			if (MilMo_Level.CurrentLevel != null && MilMo_WaterManager.GetWaterLevel(position2, out var surfaceY) != 0)
			{
				num = surfaceY;
			}
			if (position.y - num > 1.5f)
			{
				CurrentDefaultLookup = 65f;
			}
			if (AutoReset && (double)Math.Abs(LookupBeforeTerrain - CurrentDefaultLookup) > 0.0)
			{
				ResettingLookup = true;
			}
		}
	}

	protected virtual bool PreventTerrainLookup()
	{
		return false;
	}

	protected void CalculateTerrainLookup()
	{
		if (!MilMo_CameraController.Player || !Terrain.activeTerrain)
		{
			CurrentTerrainLookup = CurrentDefaultLookup;
			RawTerrainLookup = 0f;
			return;
		}
		if (PreventTerrainLookup())
		{
			CurrentTerrainLookup = Lookup;
			return;
		}
		if (MilMo_CameraController.Player.transform.position.y - MilMo_Physics.GetTerrainHeight(MilMo_CameraController.Player.transform.position) <= 0.5f)
		{
			Vector3 zero = Vector3.zero;
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i < 8; i++)
			{
				float f = MathF.PI / 180f * (MilMo_CameraController.Player.transform.rotation.eulerAngles.y + (float)(i * 45));
				list.Add(new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f)));
			}
			bool flag = true;
			int num = 0;
			foreach (Vector3 item in list)
			{
				Vector3 pos = MilMo_CameraController.Player.transform.position + item * 1f;
				zero += MilMo_Physics.GetTerrainNormal(pos);
				num++;
				if (flag)
				{
					pos = MilMo_CameraController.Player.transform.position + item * 2f;
					zero += MilMo_Physics.GetTerrainNormal(pos);
					num++;
				}
				flag = !flag;
			}
			Vector3 vector = zero / num;
			vector.Normalize();
			float y = vector.y;
			Vector3 forward = MilMo_CameraController.CameraTransform.forward;
			forward.y = 0f;
			forward = Vector3.Normalize(forward);
			Vector3 position = MilMo_CameraController.Player.position;
			Vector3 pos2 = position - forward;
			Vector3 pos3 = position + forward;
			float terrainHeight = MilMo_Physics.GetTerrainHeight(pos2);
			float terrainHeight2 = MilMo_Physics.GetTerrainHeight(pos3);
			int num2 = ((!(terrainHeight < terrainHeight2)) ? 1 : (-1));
			RawTerrainLookup = (float)(num2 * 90) * (1f - Mathf.Abs(y));
		}
		else
		{
			RawTerrainLookup = 0f;
		}
		_wantedTerrainLookup = RawTerrainLookup + LookupBeforeTerrain;
		_wantedTerrainLookup = Mathf.Clamp(_wantedTerrainLookup, -50f, 87.5f);
		CurrentTerrainLookup = Mathf.Lerp(CurrentTerrainLookup, _wantedTerrainLookup, 5f * Time.deltaTime);
	}

	protected virtual void ReadOrbitInput(bool readLookup = true, bool readPan = true)
	{
		if (readLookup)
		{
			float num = MilMo_CameraController.GetAxisY();
			if (Mathf.Abs(num) > 3f)
			{
				num = Mathf.Sign(num) * 3f;
			}
			float value = -4.42f * num * Settings.CameraSensitivity;
			LookupBuffer.AddLast(value);
			LookupBuffer.RemoveFirst();
			float num2 = LookupBuffer.Sum();
			WantedLookup += num2 / (float)LookupBuffer.Count;
			WantedLookup = Mathf.Clamp(WantedLookup, -50f, 80f);
		}
		if (readPan)
		{
			float num3 = MilMo_CameraController.GetAxisX();
			if (Mathf.Abs(num3) > 3f)
			{
				num3 = Mathf.Sign(num3) * 3f;
			}
			float value2 = 4.42f * num3 * Settings.CameraSensitivity;
			PanBuffer.AddLast(value2);
			PanBuffer.RemoveFirst();
			float num4 = PanBuffer.Sum();
			float num5 = WantedPan + num4 / (float)PanBuffer.Count;
			float f = Mathf.DeltaAngle(Pan, num5);
			float f2 = Mathf.DeltaAngle(Pan, WantedPan);
			if ((double)Math.Abs(Mathf.Sign(f) - Mathf.Sign(f2)) > 0.0 && (double)Math.Abs(Mathf.Sign(f2) - (float)Math.Sign(num4)) < 0.0)
			{
				num5 = Pan + Mathf.Sign(num4) * 179.9f;
			}
			WantedPan = num5;
		}
	}

	protected virtual void EnterReset(bool resetDistance, ResetSpeed speed)
	{
		CurrentMaxLookupResetSpeed = 2f;
		CurrentMaxPanResetSpeed = 2f;
		if (speed == ResetSpeed.Fast)
		{
			_distanceResetSpeed = 5f;
			LookupResetSpeed = CurrentMaxLookupResetSpeed;
			PanResetSpeed = CurrentMaxPanResetSpeed;
		}
		else
		{
			if (speed == ResetSpeed.Medium)
			{
				_currentDistanceResetAcceleration = 9f;
				CurrentLookupResetAcceleration = 3f;
				CurrentPanResetAcceleration = 3f;
			}
			else
			{
				_currentDistanceResetAcceleration = 3f;
				CurrentLookupResetAcceleration = 1f;
				CurrentPanResetAcceleration = 1f;
			}
			if (!ResettingDistance)
			{
				_distanceResetSpeed = 0f;
			}
			if (!ResettingLookup)
			{
				LookupResetSpeed = 0f;
			}
			if (!ResettingPan)
			{
				PanResetSpeed = 0f;
			}
		}
		ResettingPan = true;
		ResettingLookup = true;
		ResettingDistance = resetDistance;
		WantedLookup = CurrentDefaultLookup;
		if (!MilMo_CameraController.Orbit)
		{
			MilMo_CameraController.Orbit = true;
			Pan = MilMo_CameraController.CameraTransform.rotation.eulerAngles.y;
		}
		else
		{
			CurrentTerrainLookup = Lookup;
			CalculateTerrainLookup();
			LookupBeforeTerrain = Lookup - RawTerrainLookup;
		}
	}

	protected void ResetFast(object o = null)
	{
		if (!MilMo_Input.GetKey(KeyCode.Mouse0, useKeyboardFocus: false))
		{
			EnterReset(resetDistance: true, ResetSpeed.Fast);
		}
	}

	public void Reset()
	{
		if (!MilMo_Input.GetKey(KeyCode.Mouse1, useKeyboardFocus: false))
		{
			EnterReset(resetDistance: true, ResetSpeed.Slow);
		}
	}

	protected void EnterOrbit()
	{
		ResettingLookup = false;
		ResettingPan = false;
		MilMo_CameraController.Orbit = true;
		Pan = MilMo_CameraController.CameraTransform.rotation.eulerAngles.y;
		WantedPan = Pan;
		WantedLookup = Lookup;
		FreeLookStartTime = Time.time;
		for (LinkedListNode<float> linkedListNode = LookupBuffer.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			linkedListNode.Value = 0f;
		}
		for (LinkedListNode<float> linkedListNode2 = PanBuffer.First; linkedListNode2 != null; linkedListNode2 = linkedListNode2.Next)
		{
			linkedListNode2.Value = 0f;
		}
	}

	protected virtual void ExitOrbit()
	{
		MilMo_CameraController.Orbit = false;
	}

	public void ShakeCameraPosition(float duration, float amount)
	{
		new GameObject().AddComponent<MolMo_CameraShake>().Init(duration, amount, this);
	}

	public async void BossCameraShake()
	{
		ShakeCameraPosition(3.5f, 0.15f);
		await Task.Delay(3500);
		ShakeCameraPosition(0.55f, 0.05f);
		await Task.Delay(550);
		ShakeCameraPosition(0.55f, 0.05f);
	}
}
