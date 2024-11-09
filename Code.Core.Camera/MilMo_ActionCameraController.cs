using System;
using System.Collections.Generic;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Camera;

public class MilMo_ActionCameraController : MilMo_CameraController
{
	protected readonly MilMo_CameraMover Mover = new MilMo_CameraMover();

	private MilMo_FadeCamera _fadeThing;

	private const float HAPPY_PICKUP_HEIGHT = 2f;

	private const float HAPPY_PICKUP_DISTANCE = 3.35f;

	private const float HAPPY_PICKUP_FOV = 40f;

	private const float HAPPY_PICKUP_PULL = 0.5f;

	private const float HAPPY_PICKUP_DRAG = 0.1f;

	private const float HAPPY_PICKUP_ANGLE_PULL = 0.5f;

	private const float HAPPY_PICKUP_ANGLE_DRAG = 0.1f;

	public override void HookUp()
	{
		base.HookUp();
		_fadeThing = MilMo_CameraController.CameraTransform.gameObject.AddComponent<MilMo_FadeCamera>();
		_fadeThing.enabled = false;
		Mover.CameraObject = MilMo_CameraController.CameraComponent.gameObject;
		Mover.LookAtTargetDynamic = MilMo_CameraController.Player.gameObject;
		Mover.LookAtTargetOffset = MilMo_CameraController.HeadOffset;
		Vector3 position = MilMo_CameraController.CameraTransform.position;
		Mover.Pos = position;
		Mover.Target = position;
		Mover.Angle = MilMo_CameraController.CameraTransform.eulerAngles;
		Mover.Zoom = MilMo_CameraController.CameraComponent.fieldOfView;
		Mover.Vel = Vector3.zero;
		Mover.AngleVel = Vector3.zero;
		Mover.ZoomVel = 0f;
	}

	public override void Unhook()
	{
		base.Unhook();
		UnityEngine.Object.Destroy(_fadeThing);
	}

	public virtual void FixedUpdate()
	{
		if (base.HookedUp)
		{
			Mover.FixedUpdate();
		}
	}

	public virtual void Update()
	{
		MilMo_CameraController.UpdateAudioListenerPosition();
	}

	public void HappyPickup()
	{
		Mover.LooksAt = true;
		Mover.MinLookAtInterval = 0.1f;
		Mover.MaxLookAtInterval = 0.1f;
		Mover.Drag = 0.1f;
		Mover.Pull = 0.5f;
		Mover.AnglePull = 0.5f;
		Mover.AngleDrag = 0.1f;
		Mover.Shakes = false;
		Vector3 vector = -MilMo_CameraController.CameraTransform.forward;
		vector.y = 0f;
		vector.Normalize();
		Vector3 position = MilMo_CameraController.Player.position;
		position += vector * 3.35f;
		position.y += 2f;
		Mover.GoTo(position.x, position.y, position.z);
		Mover.ZoomTo(40f);
		Mover.LookAtNow();
	}

	public void Shake(int level)
	{
		Mover.LooksAt = false;
		Mover.GoTo(MilMo_CameraController.CameraTransform.position);
		Mover.AngleNow(MilMo_CameraController.CameraTransform.eulerAngles);
		Mover.MinShakeTime1 = 0.8f * (float)level;
		Mover.MaxShakeTime1 = 1f * (float)level;
		Mover.MinShakeAmp1.x = -0.0005f * (float)level;
		Mover.MinShakeAmp1.y = -0.0005f * (float)level;
		Mover.MinShakeAmp1.z = -0.0005f * (float)level;
		Mover.MaxShakeAmp1.x = 0.0005f * (float)level;
		Mover.MaxShakeAmp1.y = 0.0005f * (float)level;
		Mover.MaxShakeAmp1.z = 0.0005f * (float)level;
		Mover.MinShakeTime2 = 0.8f * (float)level;
		Mover.MaxShakeTime2 = 1f * (float)level;
		Mover.MinShakeAmp2.x = -0.0005f * (float)level;
		Mover.MinShakeAmp2.y = -0.0005f * (float)level;
		Mover.MinShakeAmp2.z = -0.0005f * (float)level;
		Mover.MaxShakeAmp2.x = 0.0005f * (float)level;
		Mover.MaxShakeAmp2.y = 0.0005f * (float)level;
		Mover.MaxShakeAmp2.z = 0.0005f * (float)level;
		Mover.IsShakyCam(s: true);
	}

	public void FadeIn(float duration)
	{
		_fadeThing.FadeIn(duration);
	}

	public void FadeOut(float duration)
	{
		_fadeThing.FadeOut(duration);
	}

	public void GoTo(Vector3 position)
	{
		Mover.GoTo(position);
	}

	public void GoToNow(Vector3 position)
	{
		Mover.GoToNow(position);
	}

	public void Impulse(Vector3 impulse)
	{
		Mover.Impulse(impulse);
	}

	public void RandomImpulse(Vector3 min, Vector3 max)
	{
		Mover.Impulse(min, max);
	}

	public void RotateTo(Vector3 target)
	{
		Mover.SetAngle(target);
	}

	public void RotateToNow(Vector3 target)
	{
		Mover.AngleNow(target);
	}

	public void LookAt(Vector3 target)
	{
		Mover.LookAt(target);
	}

	public void LookAtNow(Vector3 target)
	{
		Mover.LookAtNow(target);
	}

	public void StopLookAt()
	{
		Mover.StopLookAt();
	}

	public void AngleImpulse(Vector3 impulse)
	{
		Mover.AngleImpulse(impulse);
	}

	public void RandomAngleImpulse(Vector3 min, Vector3 max)
	{
		Mover.AngleImpulse(min, max);
	}

	public void ZoomTo(float fov)
	{
		Mover.ZoomTo(fov);
	}

	public void ZoomToNow(float fov)
	{
		Mover.ZoomToNow(fov);
	}

	public void Shake(bool shouldShake)
	{
		Mover.IsShakyCam(shouldShake);
	}

	protected void ReadSettingsScript(MilMo_SFFile file)
	{
		while (file.NextRow())
		{
			string @string = file.GetString();
			List<string> list = new List<string>();
			while (file.HasMoreTokens())
			{
				list.Add(file.GetString());
			}
			string[] array = new string[list.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = list[i];
			}
			try
			{
				if (!ApplySetting(@string, array))
				{
					Debug.LogWarning("Got invalid action camera setting in '" + file.Path + "(" + file.Name + ")' at line " + file.GetLineNumber());
				}
			}
			catch (FormatException ex)
			{
				Debug.LogWarning("Failed to read float in '" + file.Path + "(" + file.Name + ")' at line " + file.GetLineNumber() + "\n" + ex);
			}
		}
	}

	public bool ApplySetting(string setting, string[] value)
	{
		if (setting.Equals("GoToMode", StringComparison.InvariantCultureIgnoreCase))
		{
			if (value[0].Equals("Linear", StringComparison.InvariantCultureIgnoreCase))
			{
				Mover.GoToMode = MilMo_CameraMover.MovementMode.Linear;
				return true;
			}
			if (value[0].Equals("PullDrag", StringComparison.InvariantCultureIgnoreCase))
			{
				Mover.GoToMode = MilMo_CameraMover.MovementMode.PullDrag;
				return true;
			}
			if (value[0].Equals("Lerp", StringComparison.InvariantCultureIgnoreCase))
			{
				Mover.GoToMode = MilMo_CameraMover.MovementMode.Lerp;
				return true;
			}
		}
		else
		{
			if (setting.Equals("LinearMoveSpeed", StringComparison.InvariantCultureIgnoreCase))
			{
				Mover.LinearMoveSpeed = MilMo_Utility.StringToFloat(value[0]);
				return true;
			}
			if (setting.Equals("Pull", StringComparison.InvariantCultureIgnoreCase))
			{
				Mover.Pull = MilMo_Utility.StringToFloat(value[0]);
				return true;
			}
			if (setting.Equals("Drag", StringComparison.InvariantCultureIgnoreCase))
			{
				Mover.Drag = MilMo_Utility.StringToFloat(value[0]);
				return true;
			}
			if (setting.Equals("LerpAcceleration", StringComparison.InvariantCultureIgnoreCase))
			{
				Mover.LerpPosAcceleration = MilMo_Utility.StringToFloat(value[0]);
				return true;
			}
			if (setting.Equals("LerpStartSpeed", StringComparison.InvariantCultureIgnoreCase))
			{
				Mover.LerpPosStartSpeed = MilMo_Utility.StringToFloat(value[0]);
				return true;
			}
			if (setting.Equals("LerpMaxSpeed", StringComparison.InvariantCultureIgnoreCase))
			{
				Mover.MaxLerpPosSpeed = MilMo_Utility.StringToFloat(value[0]);
				return true;
			}
			if (setting.Equals("AngleMode", StringComparison.InvariantCultureIgnoreCase))
			{
				if (value[0].Equals("Linear", StringComparison.InvariantCultureIgnoreCase))
				{
					Mover.AngleMode = MilMo_CameraMover.MovementMode.Linear;
					return true;
				}
				if (value[0].Equals("PullDrag", StringComparison.InvariantCultureIgnoreCase))
				{
					Mover.AngleMode = MilMo_CameraMover.MovementMode.PullDrag;
					return true;
				}
				if (value[0].Equals("Lerp", StringComparison.InvariantCultureIgnoreCase))
				{
					Mover.AngleMode = MilMo_CameraMover.MovementMode.Lerp;
					return true;
				}
			}
			else
			{
				if (setting.Equals("LinearAngleSpeed", StringComparison.InvariantCultureIgnoreCase))
				{
					Mover.LinearAngleSpeed = MilMo_Utility.StringToFloat(value[0]);
					return true;
				}
				if (setting.Equals("AnglePull", StringComparison.InvariantCultureIgnoreCase))
				{
					Mover.AnglePull = MilMo_Utility.StringToFloat(value[0]);
					return true;
				}
				if (setting.Equals("AngleDrag", StringComparison.InvariantCultureIgnoreCase))
				{
					Mover.AngleDrag = MilMo_Utility.StringToFloat(value[0]);
					return true;
				}
				if (setting.Equals("LerpAngleAcceleration", StringComparison.InvariantCultureIgnoreCase))
				{
					Mover.LerpAngleAcceleration = MilMo_Utility.StringToFloat(value[0]);
					return true;
				}
				if (setting.Equals("LerpAngleStartSpeed", StringComparison.InvariantCultureIgnoreCase))
				{
					Mover.LerpAngleStartSpeed = MilMo_Utility.StringToFloat(value[0]);
					return true;
				}
				if (setting.Equals("LerpAngleMaxSpeed", StringComparison.InvariantCultureIgnoreCase))
				{
					Mover.MaxLerpAngleSpeed = MilMo_Utility.StringToFloat(value[0]);
					return true;
				}
				if (setting.Equals("Zoom", StringComparison.InvariantCultureIgnoreCase))
				{
					Mover.ZoomToNow(MilMo_Utility.StringToFloat(value[0]));
					return true;
				}
				if (setting.Equals("ZoomMode", StringComparison.InvariantCultureIgnoreCase))
				{
					if (value[0].Equals("Linear", StringComparison.InvariantCultureIgnoreCase))
					{
						Mover.ZoomMode = MilMo_CameraMover.MovementMode.Linear;
						return true;
					}
					if (value[0].Equals("PullDrag", StringComparison.InvariantCultureIgnoreCase))
					{
						Mover.ZoomMode = MilMo_CameraMover.MovementMode.PullDrag;
						return true;
					}
					if (value[0].Equals("Lerp", StringComparison.InvariantCultureIgnoreCase))
					{
						Mover.ZoomMode = MilMo_CameraMover.MovementMode.Lerp;
						return true;
					}
				}
				else
				{
					if (setting.Equals("LinearZoomSpeed", StringComparison.InvariantCultureIgnoreCase))
					{
						Mover.LinearZoomSpeed = MilMo_Utility.StringToFloat(value[0]);
						return true;
					}
					if (setting.Equals("ZoomPull", StringComparison.InvariantCultureIgnoreCase))
					{
						Mover.ZoomPull = MilMo_Utility.StringToFloat(value[0]);
						return true;
					}
					if (setting.Equals("ZoomDrag", StringComparison.InvariantCultureIgnoreCase))
					{
						Mover.ZoomDrag = MilMo_Utility.StringToFloat(value[0]);
						return true;
					}
					if (setting.Equals("LerpZoomAcceleration", StringComparison.InvariantCultureIgnoreCase))
					{
						Mover.LerpZoomAcceleration = MilMo_Utility.StringToFloat(value[0]);
						return true;
					}
					if (setting.Equals("LerpZoomStartSpeed", StringComparison.InvariantCultureIgnoreCase))
					{
						Mover.LerpZoomStartSpeed = MilMo_Utility.StringToFloat(value[0]);
						return true;
					}
					if (setting.Equals("LerpZoomMaxSpeed", StringComparison.InvariantCultureIgnoreCase))
					{
						Mover.MaxLerpZoomSpeed = MilMo_Utility.StringToFloat(value[0]);
						return true;
					}
					if (setting.Equals("OrbitMode", StringComparison.InvariantCultureIgnoreCase))
					{
						if (value[0].Equals("Linear", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.OrbitMode = MilMo_CameraMover.MovementMode.Linear;
							return true;
						}
						if (value[0].Equals("PullDrag", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.OrbitMode = MilMo_CameraMover.MovementMode.PullDrag;
							return true;
						}
						if (value[0].Equals("Lerp", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.OrbitMode = MilMo_CameraMover.MovementMode.Lerp;
							return true;
						}
					}
					else
					{
						if (setting.Equals("LinearOrbitPanSpeed", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.LinearOrbitPanSpeed = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("OrbitPanPull", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.OrbitPanPull = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("OrbitPanDrag", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.OrbitPanDrag = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("LerpOrbitPanAcceleration", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.LerpOrbitPanAcceleration = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("LerpOrbitPanStartSpeed", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.LerpOrbitPanStartSpeed = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("LerpOrbitPanMaxSpeed", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.MaxLerpOrbitPanSpeed = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("LinearOrbitLookupSpeed", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.LinearOrbitLookupSpeed = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("OrbitLookupPull", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.OrbitLookupPull = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("OrbitLookupDrag", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.OrbitLookupDrag = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("LerpOrbitLookupAcceleration", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.LerpOrbitLookupAcceleration = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("LerpOrbitLookupStartSpeed", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.LerpOrbitLookupStartSpeed = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("LerpOrbitLookupMaxSpeed", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.MaxLerpOrbitLookupSpeed = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("LinearOrbitDistanceSpeed", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.LinearOrbitDistanceSpeed = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("OrbitDistancePull", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.OrbitDistancePull = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("OrbitDistanceDrag", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.OrbitDistanceDrag = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("LerpOrbitDistanceAcceleration", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.LerpOrbitDistanceAcceleration = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("LerpOrbitDistanceStartSpeed", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.LerpOrbitDistanceStartSpeed = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("LerpOrbitDistanceMaxSpeed", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.MaxLerpOrbitDistanceSpeed = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("Shake", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.IsShakyCam(MilMo_Utility.StringToBool(value[0]));
							return true;
						}
						if (setting.Equals("MinShakeTime1", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.MinShakeTime1 = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("MaxShakeTime1", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.MaxShakeTime1 = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("MinShakeAmp1", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.MinShakeAmp1 = MilMo_Utility.StringArrayToVector(value, 0);
							return true;
						}
						if (setting.Equals("MaxShakeAmp1", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.MaxShakeAmp1 = MilMo_Utility.StringArrayToVector(value, 0);
							return true;
						}
						if (setting.Equals("MinShakeTime2", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.MinShakeTime2 = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("MaxShakeTime2", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.MaxShakeTime2 = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("MinShakeAmp2", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.MinShakeAmp2 = MilMo_Utility.StringArrayToVector(value, 0);
							return true;
						}
						if (setting.Equals("MaxShakeAmp2", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.MaxShakeAmp2 = MilMo_Utility.StringArrayToVector(value, 0);
							return true;
						}
						if (setting.Equals("Realigns", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.Realigns = MilMo_Utility.StringToBool(value[0]);
							return true;
						}
						if (setting.Equals("MinRealignInterval", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.MinRealignInterval = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("MaxRealignInterval", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.MaxRealignInterval = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("MinLookAtInterval", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.MinLookAtInterval = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
						if (setting.Equals("MaxLookAtInterval", StringComparison.InvariantCultureIgnoreCase))
						{
							Mover.MaxLookAtInterval = MilMo_Utility.StringToFloat(value[0]);
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	protected string Debug_Set(string[] args)
	{
		if (!base.HookedUp)
		{
			return "Action cam must be hooked up. (The command for hooking up action cam is 'ActionCam'";
		}
		if (args.Length < 3)
		{
			return "Usage: ActionCam.Set <setting> <value>";
		}
		string setting = args[1];
		string[] array = new string[args.Length - 2];
		for (int i = 0; i < array.Length && i + 2 < args.Length; i++)
		{
			array[i] = args[i + 2];
		}
		try
		{
			if (!ApplySetting(setting, array))
			{
				return "Unknown setting or invalid value.";
			}
		}
		catch (FormatException)
		{
			return "Failed to parse value: Invalid number format. Did you use ',' instead of '.'?";
		}
		return "Action camera setting applied.";
	}
}
