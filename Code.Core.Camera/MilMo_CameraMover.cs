using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Camera;

public sealed class MilMo_CameraMover
{
	public enum MovementMode
	{
		PullDrag,
		Lerp,
		Linear
	}

	public GameObject CameraObject;

	public GameObject LookAtTargetDynamic;

	public Vector3 LookAtTargetOffset;

	private Vector3 _lookAtTargetStatic;

	private bool _staticTarget;

	public bool LooksAt;

	public float MinLookAtInterval;

	public float MaxLookAtInterval;

	private readonly MilMo_EventSystem.MilMo_Callback _shake1;

	private readonly MilMo_EventSystem.MilMo_Callback _shake2;

	private readonly MilMo_EventSystem.MilMo_Callback _realign;

	private readonly MilMo_EventSystem.MilMo_Callback _lookAt;

	public Vector3 Pos;

	public Vector3 Target;

	public Vector3 Vel;

	private MovementMode _goToMode;

	public float LinearMoveSpeed;

	private float _currentLerpPosSpeed;

	public float LerpPosAcceleration = 0.01f;

	public float MaxLerpPosSpeed = 0.8f;

	public float LerpPosStartSpeed;

	public float Pull;

	public float Drag;

	public Vector3 MinVel;

	public Vector3 Angle;

	private Vector3 _targetAngle;

	public Vector3 AngleVel;

	private MovementMode _angleMode;

	public float LinearAngleSpeed;

	private float _currentLerpAngleSpeed;

	public float LerpAngleAcceleration = 0.01f;

	public float MaxLerpAngleSpeed = 0.8f;

	public float LerpAngleStartSpeed;

	public float AnglePull;

	public float AngleDrag;

	public float Zoom;

	private float _targetZoom;

	public float ZoomVel;

	private MovementMode _zoomMode;

	public float LinearZoomSpeed;

	private float _currentLerpZoomSpeed;

	public float LerpZoomAcceleration = 0.01f;

	public float MaxLerpZoomSpeed = 0.8f;

	public float LerpZoomStartSpeed;

	public float ZoomPull;

	public float ZoomDrag;

	private bool _orbitsTo;

	private float _orbitPan;

	private float _orbitPanVel;

	private float _orbitLookup;

	private float _orbitLookupVel;

	private float _orbitDistance;

	private float _orbitDistanceVel;

	private float _targetOrbitPan;

	private float _targetOrbitLookup;

	private float _targetOrbitDistance;

	private float _currentLerpOrbitPanSpeed;

	private float _currentLerpOrbitLookupSpeed;

	private float _currentLerpOrbitDistanceSpeed;

	private MovementMode _orbitMode = MovementMode.Lerp;

	public float LinearOrbitPanSpeed = 1f;

	public float LerpOrbitPanAcceleration = 0.01f;

	public float MaxLerpOrbitPanSpeed = 0.8f;

	public float LerpOrbitPanStartSpeed;

	public float OrbitPanPull = 0.05f;

	public float OrbitPanDrag = 0.95f;

	public float LinearOrbitLookupSpeed = 1f;

	public float LerpOrbitLookupAcceleration = 0.01f;

	public float MaxLerpOrbitLookupSpeed = 0.8f;

	public float LerpOrbitLookupStartSpeed;

	public float OrbitLookupPull = 0.05f;

	public float OrbitLookupDrag = 0.95f;

	public float LinearOrbitDistanceSpeed = 1f;

	public float LerpOrbitDistanceAcceleration = 0.01f;

	public float MaxLerpOrbitDistanceSpeed = 0.8f;

	public float LerpOrbitDistanceStartSpeed;

	public float OrbitDistancePull = 0.05f;

	public float OrbitDistanceDrag = 0.95f;

	public bool Shakes;

	public float MinShakeTime1;

	public float MaxShakeTime1;

	private Vector3 _shakeAmp1;

	public Vector3 MinShakeAmp1;

	public Vector3 MaxShakeAmp1;

	public float MinShakeTime2;

	public float MaxShakeTime2;

	private Vector3 _shakeAmp2;

	public Vector3 MinShakeAmp2;

	public Vector3 MaxShakeAmp2;

	private Vector3 _shakeAcc;

	public bool Realigns;

	public float MinRealignInterval;

	public float MaxRealignInterval;

	private GameObject _targetTemp;

	public MovementMode GoToMode
	{
		set
		{
			_goToMode = value;
			_currentLerpPosSpeed = LerpPosStartSpeed;
		}
	}

	public MovementMode AngleMode
	{
		set
		{
			_angleMode = value;
			_currentLerpAngleSpeed = LerpAngleStartSpeed;
		}
	}

	public MovementMode ZoomMode
	{
		set
		{
			_zoomMode = value;
			_currentLerpZoomSpeed = LerpZoomStartSpeed;
		}
	}

	public MovementMode OrbitMode
	{
		set
		{
			_orbitMode = value;
			_currentLerpOrbitPanSpeed = LerpOrbitPanStartSpeed;
			_currentLerpOrbitLookupSpeed = LerpOrbitLookupStartSpeed;
			_currentLerpOrbitDistanceSpeed = LerpOrbitDistanceStartSpeed;
		}
	}

	public MilMo_CameraMover()
	{
		_shake1 = Shake1;
		_shake2 = Shake2;
		_realign = Realign;
		_lookAt = DoLookAt;
		LinearMoveSpeed = 0f;
		LinearAngleSpeed = 0f;
		LinearZoomSpeed = 0f;
		Vel.x = 0f;
		Vel.y = 0f;
		Vel.z = 0f;
		Pos.x = 0f;
		Pos.y = 0f;
		Pos.z = 0f;
		Target.x = 0f;
		Target.y = 0f;
		Target.z = 0f;
		Pull = 0.05f;
		Drag = 0.95f;
		MinVel.x = 0.001f;
		MinVel.y = 0.001f;
		MinVel.z = 0.001f;
		AngleVel.x = 0f;
		AngleVel.y = 0f;
		AngleVel.z = 0f;
		Angle.x = 0f;
		Angle.y = 0f;
		Angle.z = 0f;
		_targetAngle.x = 0f;
		_targetAngle.y = 0f;
		_targetAngle.z = 0f;
		AnglePull = 0.05f;
		AngleDrag = 0.95f;
		Zoom = 90f;
		_targetZoom = 90f;
		ZoomDrag = 0.65f;
		ZoomPull = 0.01f;
		ZoomVel = 0f;
		Shakes = false;
		MinShakeTime1 = 0.4f;
		MaxShakeTime1 = 0.6f;
		MinShakeAmp1.x = -0.05f;
		MinShakeAmp1.y = -0.05f;
		MinShakeAmp1.z = -0.05f;
		MaxShakeAmp1.x = 0.05f;
		MaxShakeAmp1.y = 0.05f;
		MaxShakeAmp1.z = 0.05f;
		MinShakeTime2 = 0.2f;
		MaxShakeTime2 = 0.3f;
		MinShakeAmp2.x = -0.05f;
		MinShakeAmp2.y = -0.05f;
		MinShakeAmp2.z = -0.05f;
		MaxShakeAmp2.x = 0.05f;
		MaxShakeAmp2.y = 0.05f;
		MaxShakeAmp2.z = 0.05f;
		_shakeAcc.x = 0f;
		_shakeAcc.y = 0f;
		_shakeAcc.z = 0f;
		Realigns = true;
		MinRealignInterval = 5f;
		MaxRealignInterval = 10f;
		LooksAt = false;
		MinLookAtInterval = 0.1f;
		MaxLookAtInterval = 0.1f;
	}

	public void FixedUpdate()
	{
		if (!_orbitsTo)
		{
			UpdateGoTo();
			UpdateAngleToAndShake();
		}
		else
		{
			UpdateOrbitTo();
		}
		UpdateZoomTo();
		Angle.x = Mathf.Repeat(Angle.x, 360f);
		Angle.y = Mathf.Repeat(Angle.y, 360f);
		Angle.z = Mathf.Repeat(Angle.z, 360f);
		CameraObject.transform.position = Pos;
		CameraObject.transform.eulerAngles = Angle;
		MilMo_Global.MainCamera.fieldOfView = Zoom;
	}

	public void AttachObject(GameObject o)
	{
		if ((bool)o)
		{
			CameraObject = o;
		}
		else
		{
			Debug.LogWarning("'MilMo_CameraMover:AttachObject' failed : object is null.");
		}
	}

	private static void At(float time, MilMo_EventSystem.MilMo_Callback func)
	{
		MilMo_EventSystem.At(time, func);
	}

	public void GoTo(float x, float y, float z)
	{
		if (LooksAt && (_staticTarget || (bool)LookAtTargetDynamic))
		{
			OrbitTo(x, y, z);
			return;
		}
		Target.x = x;
		Target.y = y;
		Target.z = z;
		switch (_goToMode)
		{
		case MovementMode.Linear:
			Vel = (Target - Pos).normalized * LinearMoveSpeed;
			break;
		case MovementMode.PullDrag:
			Vel.x = 0.002f;
			break;
		case MovementMode.Lerp:
			_currentLerpPosSpeed = LerpPosStartSpeed;
			break;
		}
	}

	public void GoTo(Vector3 target)
	{
		GoTo(target.x, target.y, target.z);
	}

	private void OrbitTo(float x, float y, float z)
	{
		Vector3 vector = ((!_staticTarget) ? (LookAtTargetDynamic.transform.position + LookAtTargetOffset) : _lookAtTargetStatic);
		Vector3 eulerAngles = CameraObject.transform.eulerAngles;
		Vector3 position = CameraObject.transform.position;
		_orbitPan = eulerAngles.y;
		_orbitLookup = eulerAngles.x;
		_orbitDistance = (position - vector).magnitude;
		if (!_targetTemp)
		{
			_targetTemp = new GameObject();
		}
		_targetTemp.transform.position.Set(x, y, z);
		_targetTemp.transform.LookAt(vector);
		Vector3 eulerAngles2 = _targetTemp.transform.eulerAngles;
		_targetOrbitPan = eulerAngles2.y;
		_targetOrbitLookup = eulerAngles2.x;
		_targetOrbitDistance = (new Vector3(x, y, z) - vector).magnitude;
		if (_orbitMode == MovementMode.Linear)
		{
			_orbitPanVel = Mathf.Sign(Mathf.DeltaAngle(_orbitPan, _targetOrbitPan)) * LinearOrbitPanSpeed;
			_orbitLookupVel = Mathf.Sign(Mathf.DeltaAngle(_orbitLookup, _targetOrbitLookup)) * LinearOrbitLookupSpeed;
			_orbitDistanceVel = Mathf.Sign(_targetOrbitDistance - _orbitDistance) * LinearOrbitDistanceSpeed;
		}
		else if (_orbitMode == MovementMode.Lerp)
		{
			_currentLerpOrbitPanSpeed = LerpOrbitPanStartSpeed;
			_currentLerpOrbitLookupSpeed = LerpOrbitLookupStartSpeed;
			_currentLerpOrbitDistanceSpeed = LerpOrbitDistanceStartSpeed;
		}
		_orbitsTo = true;
	}

	private void GoToNow(float x, float y, float z)
	{
		Target.x = x;
		Target.y = y;
		Target.z = z;
		Pos.x = Target.x;
		Pos.y = Target.y;
		Pos.z = Target.z;
		Vel.x = 0f;
		Vel.y = 0f;
		Vel.z = 0f;
		_orbitsTo = false;
	}

	public void GoToNow(Vector3 target)
	{
		GoToNow(target.x, target.y, target.z);
	}

	private void Impulse(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
	{
		if (_goToMode == MovementMode.PullDrag)
		{
			Vector3 vector = default(Vector3);
			vector.x = Random.Range(minX, maxX);
			vector.y = Random.Range(minY, maxY);
			vector.z = Random.Range(minZ, maxZ);
			Vel += vector;
		}
	}

	public void Impulse(Vector3 min, Vector3 max)
	{
		Impulse(min.x, min.y, min.z, max.x, max.y, max.z);
	}

	public void Impulse(Vector3 impulse)
	{
		if (_goToMode == MovementMode.PullDrag)
		{
			Vel += impulse;
		}
	}

	private void SetAngle(float x, float y, float z)
	{
		_targetAngle.x = x;
		_targetAngle.y = y;
		_targetAngle.z = z;
		_shakeAcc.x = 0f;
		_shakeAcc.y = 0f;
		_shakeAcc.z = 0f;
		switch (_angleMode)
		{
		case MovementMode.Linear:
			AngleVel.x = Mathf.DeltaAngle(Angle.x, _targetAngle.x);
			AngleVel.y = Mathf.DeltaAngle(Angle.y, _targetAngle.y);
			AngleVel.z = Mathf.DeltaAngle(Angle.z, _targetAngle.z);
			AngleVel = AngleVel.normalized * LinearAngleSpeed;
			break;
		case MovementMode.PullDrag:
			AngleVel.x = 0.002f;
			break;
		case MovementMode.Lerp:
			_currentLerpAngleSpeed = LerpAngleStartSpeed;
			break;
		}
	}

	public void SetAngle(Vector3 targetAngle)
	{
		SetAngle(targetAngle.x, targetAngle.y, targetAngle.z);
	}

	public void AngleNow(float x, float y, float z)
	{
		_targetAngle.x = x;
		_targetAngle.y = y;
		_targetAngle.z = z;
		Angle.x = _targetAngle.x;
		Angle.y = _targetAngle.y;
		Angle.z = _targetAngle.z;
		Angle.x = Mathf.Repeat(Angle.x, 360f);
		Angle.y = Mathf.Repeat(Angle.y, 360f);
		Angle.z = Mathf.Repeat(Angle.z, 360f);
		AngleVel.x = 0f;
		AngleVel.y = 0f;
		_shakeAcc.x = 0f;
		_shakeAcc.y = 0f;
		_shakeAcc.z = 0f;
		AngleVel.z = 0f;
		_orbitsTo = false;
	}

	public void AngleNow(Vector3 targetAngle)
	{
		AngleNow(targetAngle.x, targetAngle.y, targetAngle.z);
	}

	public void LookAt(GameObject target)
	{
		if (!target)
		{
			LooksAt = false;
			LookAtTargetDynamic = null;
			return;
		}
		LookAtTargetDynamic = target;
		LooksAt = true;
		_staticTarget = false;
		DoLookAt();
	}

	public void LookAt(Vector3 position)
	{
		_lookAtTargetStatic = position;
		LooksAt = true;
		_staticTarget = true;
		DoLookAt();
	}

	private void DoLookAt()
	{
		if (LooksAt && (_staticTarget || (bool)LookAtTargetDynamic))
		{
			Vector3 worldPosition = ((!_staticTarget) ? (LookAtTargetDynamic.transform.position + LookAtTargetOffset) : _lookAtTargetStatic);
			if (!_targetTemp)
			{
				_targetTemp = new GameObject();
			}
			_targetTemp.transform.position = CameraObject.transform.position;
			_targetTemp.transform.LookAt(worldPosition);
			Vector3 eulerAngles = _targetTemp.transform.eulerAngles;
			SetAngle(eulerAngles.x, eulerAngles.y, eulerAngles.z);
			At(Random.Range(MinLookAtInterval, MaxLookAtInterval), _lookAt);
		}
	}

	public void LookAtNow()
	{
		LooksAt = true;
		_staticTarget = false;
		CameraObject.transform.LookAt(LookAtTargetDynamic.transform.position + LookAtTargetOffset);
		DoLookAt();
	}

	public void StopLookAt()
	{
		LooksAt = false;
		AngleVel = Vector3.zero;
		_targetAngle = Angle;
	}

	public void LookAtNow(Vector3 target)
	{
		CameraObject.transform.LookAt(target);
		LookAt(target);
	}

	private void AngleImpulse(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
	{
		if (_angleMode == MovementMode.PullDrag)
		{
			Vector3 vector = default(Vector3);
			vector.x = Random.Range(minX, maxX);
			vector.y = Random.Range(minY, maxY);
			vector.z = Random.Range(minZ, maxZ);
			AngleVel += vector;
		}
	}

	public void AngleImpulse(Vector3 impulse)
	{
		if (_angleMode == MovementMode.PullDrag)
		{
			AngleVel += impulse;
		}
	}

	public void AngleImpulse(Vector3 min, Vector3 max)
	{
		AngleImpulse(min.x, min.y, min.z, max.x, max.y, max.z);
	}

	public void ZoomTo(float z)
	{
		_targetZoom = z;
		switch (_zoomMode)
		{
		case MovementMode.Linear:
			ZoomVel = Mathf.Sign(_targetZoom - Zoom) * LinearZoomSpeed;
			break;
		case MovementMode.Lerp:
			_currentLerpZoomSpeed = LerpZoomStartSpeed;
			break;
		}
	}

	public void ZoomToNow(float z)
	{
		Zoom = z;
		_targetZoom = z;
	}

	public void IsShakyCam(bool s)
	{
		if (s)
		{
			Shakes = true;
			Shake1();
			Shake2();
			Realign();
		}
		else
		{
			Shakes = false;
			_shakeAcc.x = 0f;
			_shakeAcc.y = 0f;
			_shakeAcc.z = 0f;
			Realign();
		}
	}

	private void Shake1()
	{
		if (Shakes)
		{
			_shakeAmp1.x = Random.Range(MinShakeAmp1.x, MaxShakeAmp1.x);
			_shakeAmp1.y = Random.Range(MinShakeAmp1.y, MaxShakeAmp1.y);
			_shakeAmp1.z = Random.Range(MinShakeAmp1.z, MaxShakeAmp1.z);
			At(Random.Range(MinShakeTime1, MaxShakeTime1), _shake1);
		}
	}

	private void Shake2()
	{
		if (Shakes)
		{
			_shakeAmp2.x = Random.Range(MinShakeAmp2.x, MaxShakeAmp2.x);
			_shakeAmp2.y = Random.Range(MinShakeAmp2.y, MaxShakeAmp2.y);
			_shakeAmp2.z = Random.Range(MinShakeAmp2.z, MaxShakeAmp2.z);
			At(Random.Range(MinShakeTime2, MaxShakeTime2), _shake2);
		}
	}

	public void Realign()
	{
		if (Shakes && Realigns)
		{
			SetAngle(_targetAngle.x, _targetAngle.y, _targetAngle.z);
			At(Random.Range(MinRealignInterval, MaxRealignInterval), _realign);
		}
	}

	private void UpdateGoTo()
	{
		if (_goToMode == MovementMode.PullDrag)
		{
			if (Mathf.Abs(Vel.x) > MinVel.x || Mathf.Abs(Vel.y) > MinVel.y || Mathf.Abs(Vel.z) > MinVel.z)
			{
				Vel.x += (Target.x - Pos.x) * Pull;
				Vel.y += (Target.y - Pos.y) * Pull;
				Vel.z += (Target.z - Pos.z) * Pull;
				Vel.x += Vel.x * Drag - Vel.x;
				Vel.y += Vel.y * Drag - Vel.y;
				Vel.z += Vel.z * Drag - Vel.z;
				Pos += Vel;
			}
			else
			{
				Vel.x = 0f;
				Vel.y = 0f;
				Vel.z = 0f;
				Pos.x = Target.x;
				Pos.y = Target.y;
				Pos.z = Target.z;
			}
		}
		else
		{
			if (MilMo_Utility.Equals(Pos, Target))
			{
				return;
			}
			switch (_goToMode)
			{
			case MovementMode.Linear:
				if ((Target - Pos).sqrMagnitude <= LinearMoveSpeed * LinearMoveSpeed)
				{
					Pos = Target;
				}
				else
				{
					Pos += Vel;
				}
				break;
			case MovementMode.Lerp:
				_currentLerpPosSpeed = Mathf.Min(_currentLerpPosSpeed + LerpPosAcceleration, MaxLerpPosSpeed);
				Pos = Vector3.Lerp(Pos, Target, _currentLerpPosSpeed);
				break;
			}
		}
	}

	private void UpdateAngleToAndShake()
	{
		if (_angleMode == MovementMode.PullDrag)
		{
			if (Shakes)
			{
				_shakeAcc.x += _shakeAmp1.x;
				_shakeAcc.y += _shakeAmp1.y;
				_shakeAcc.z += _shakeAmp1.z;
				_shakeAcc.x += _shakeAmp2.x;
				_shakeAcc.y += _shakeAmp2.y;
				_shakeAcc.z += _shakeAmp2.z;
				AngleVel.x += _shakeAcc.x;
				AngleVel.y += _shakeAcc.y;
				AngleVel.z += _shakeAcc.z;
			}
			if (Mathf.Abs(AngleVel.x) > 0.0001f || Mathf.Abs(AngleVel.y) > 0.0001f || Mathf.Abs(AngleVel.z) > 0.0001f)
			{
				AngleVel.x += Mathf.DeltaAngle(Angle.x, _targetAngle.x) * AnglePull;
				AngleVel.y += Mathf.DeltaAngle(Angle.y, _targetAngle.y) * AnglePull;
				AngleVel.z += Mathf.DeltaAngle(Angle.z, _targetAngle.z) * AnglePull;
				AngleVel.x += AngleVel.x * AngleDrag - AngleVel.x;
				AngleVel.y += AngleVel.y * AngleDrag - AngleVel.y;
				AngleVel.z += AngleVel.z * AngleDrag - AngleVel.z;
				Angle += AngleVel * (Time.deltaTime * 100f);
			}
			else
			{
				AngleVel.x = 0f;
				AngleVel.y = 0f;
				AngleVel.z = 0f;
				Angle.x = _targetAngle.x;
				Angle.y = _targetAngle.y;
				Angle.z = _targetAngle.z;
			}
		}
		else
		{
			if (MilMo_Utility.Equals(Angle, _targetAngle))
			{
				return;
			}
			switch (_angleMode)
			{
			case MovementMode.Linear:
				if (Mathf.Abs(Mathf.DeltaAngle(_targetAngle.x, Angle.x)) <= Mathf.Abs(AngleVel.x))
				{
					Angle.x = _targetAngle.x;
				}
				else
				{
					Angle.x += AngleVel.x;
				}
				if (Mathf.Abs(Mathf.DeltaAngle(_targetAngle.y, Angle.y)) <= Mathf.Abs(AngleVel.y))
				{
					Angle.y = _targetAngle.y;
				}
				else
				{
					Angle.y += AngleVel.y;
				}
				if (Mathf.Abs(Mathf.DeltaAngle(_targetAngle.z, Angle.z)) <= Mathf.Abs(AngleVel.z))
				{
					Angle.z = _targetAngle.z;
				}
				else
				{
					Angle.z += AngleVel.z;
				}
				break;
			case MovementMode.Lerp:
				_currentLerpAngleSpeed = Mathf.Min(_currentLerpAngleSpeed + LerpAngleAcceleration, MaxLerpAngleSpeed);
				Angle.x = Mathf.LerpAngle(Angle.x, _targetAngle.x, _currentLerpAngleSpeed);
				Angle.y = Mathf.LerpAngle(Angle.y, _targetAngle.y, _currentLerpAngleSpeed);
				Angle.z = Mathf.LerpAngle(Angle.z, _targetAngle.z, _currentLerpAngleSpeed);
				break;
			}
		}
	}

	private void UpdateOrbitTo()
	{
		if (!LooksAt || (!_staticTarget && (bool)LookAtTargetDynamic))
		{
			_orbitsTo = false;
			return;
		}
		if (_orbitMode == MovementMode.PullDrag)
		{
			_orbitPanVel += (_targetOrbitPan - _orbitPan) * OrbitPanPull;
			_orbitPanVel += _orbitPanVel * OrbitPanDrag - _orbitPanVel;
			_orbitPan += _orbitPanVel * (Time.deltaTime * 100f);
			_orbitLookupVel += (_targetOrbitLookup - _orbitLookup) * OrbitLookupPull;
			_orbitLookupVel += _orbitLookupVel * OrbitLookupDrag - _orbitLookupVel;
			_orbitLookup += _orbitLookupVel * (Time.deltaTime * 100f);
			_orbitDistanceVel += (_targetOrbitDistance - _orbitDistance) * OrbitDistancePull;
			_orbitDistanceVel += _orbitDistanceVel * OrbitDistanceDrag - _orbitDistanceVel;
			_orbitDistance += _orbitDistanceVel * (Time.deltaTime * 100f);
		}
		else
		{
			if (!object.Equals(_orbitPan, _targetOrbitPan))
			{
				switch (_orbitMode)
				{
				case MovementMode.Linear:
					if (Mathf.Abs(_targetOrbitPan - _orbitPan) <= LinearOrbitPanSpeed)
					{
						_orbitPan = _targetOrbitPan;
					}
					else
					{
						_orbitPan += _orbitPanVel;
					}
					break;
				case MovementMode.Lerp:
					_currentLerpOrbitPanSpeed = Mathf.Min(_currentLerpOrbitPanSpeed + LerpOrbitPanAcceleration, MaxLerpOrbitPanSpeed);
					_orbitPan = Mathf.LerpAngle(_orbitPan, _targetOrbitPan, _currentLerpOrbitPanSpeed);
					break;
				}
			}
			if (!object.Equals(_orbitLookup, _targetOrbitLookup))
			{
				switch (_orbitMode)
				{
				case MovementMode.Linear:
					if (Mathf.Abs(_targetOrbitLookup - _orbitLookup) <= LinearOrbitLookupSpeed)
					{
						_orbitLookup = _targetOrbitLookup;
					}
					else
					{
						_orbitLookup += _orbitLookupVel;
					}
					break;
				case MovementMode.Lerp:
					_currentLerpOrbitLookupSpeed = Mathf.Min(_currentLerpOrbitLookupSpeed + LerpOrbitLookupAcceleration, MaxLerpOrbitLookupSpeed);
					_orbitLookup = Mathf.LerpAngle(_orbitLookup, _targetOrbitLookup, _currentLerpOrbitLookupSpeed);
					break;
				}
			}
			if (!object.Equals(_orbitDistance, _targetOrbitDistance))
			{
				switch (_orbitMode)
				{
				case MovementMode.Linear:
					if (Mathf.Abs(_targetOrbitDistance - _orbitDistance) <= LinearOrbitDistanceSpeed)
					{
						_orbitDistance = _targetOrbitDistance;
					}
					else
					{
						_orbitDistance += _orbitDistanceVel;
					}
					break;
				case MovementMode.Lerp:
					_currentLerpOrbitDistanceSpeed = Mathf.Min(_currentLerpOrbitDistanceSpeed + LerpOrbitDistanceAcceleration, MaxLerpOrbitDistanceSpeed);
					_orbitDistance = Mathf.Lerp(_orbitDistance, _targetOrbitDistance, _currentLerpOrbitDistanceSpeed);
					break;
				}
			}
		}
		Vector3 vector = ((!_staticTarget) ? (LookAtTargetDynamic.transform.position + LookAtTargetOffset) : _lookAtTargetStatic);
		Angle = new Vector3(_orbitLookup, _orbitPan, 0f);
		Pos = vector + Quaternion.Euler(Angle) * new Vector3(0f, 0f, 0f - _orbitDistance);
	}

	private void UpdateZoomTo()
	{
		if (_zoomMode == MovementMode.PullDrag)
		{
			ZoomVel += (_targetZoom - Zoom) * ZoomPull;
			ZoomVel += ZoomVel * ZoomDrag - ZoomVel;
			Zoom += ZoomVel * (Time.deltaTime * 100f);
			if (!(Zoom >= 30f))
			{
				ZoomVel *= 0.7f;
				if (Zoom < 0f)
				{
					ZoomVel = 0f - ZoomVel;
				}
			}
		}
		else
		{
			if (object.Equals(Zoom, _targetZoom))
			{
				return;
			}
			switch (_zoomMode)
			{
			case MovementMode.Linear:
				if (Mathf.Abs(_targetZoom - Zoom) <= LinearZoomSpeed)
				{
					Zoom = _targetZoom;
				}
				else
				{
					Zoom += ZoomVel;
				}
				break;
			case MovementMode.Lerp:
				_currentLerpZoomSpeed = Mathf.Min(_currentLerpZoomSpeed + LerpZoomAcceleration, MaxLerpZoomSpeed);
				Zoom = Mathf.Lerp(Zoom, _targetZoom, _currentLerpZoomSpeed);
				break;
			}
		}
	}
}
