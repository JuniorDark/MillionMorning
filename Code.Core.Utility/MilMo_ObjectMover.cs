using System.Collections.Generic;
using System.Linq;
using Code.Core.EventSystem;
using UnityEngine;

namespace Code.Core.Utility;

public sealed class MilMo_ObjectMover
{
	public delegate void UpdateFunc();

	public delegate void ArriveFunc();

	public const int NOTHING = 0;

	public const int LINEAR = 1;

	public const int SPRING = 2;

	public const int LERP = 3;

	public const int BOUNCE = 4;

	public const int SOFTSPRING = 5;

	public const int ACCELERATE = 6;

	public const int SURFACE = 7;

	public const int SINUS = 8;

	public bool Paused;

	public readonly List<GameObject> GameObjects = new List<GameObject>();

	private readonly MilMo_EventSystem.MilMo_Callback _shake1;

	private readonly MilMo_EventSystem.MilMo_Callback _shake2;

	private readonly MilMo_EventSystem.MilMo_Callback _realign;

	private readonly MilMo_EventSystem.MilMo_Callback _lookAt;

	public Vector3 Pos;

	public Vector3 Target;

	public float Pull;

	public float Drag;

	public Vector3 Vel;

	public Vector3 MinVel;

	public Vector3 MaxVel;

	public Vector3 Accel;

	public Vector3 SinRate;

	public Vector3 SinAmp;

	public Vector3 SinVel;

	private Vector3 _internalPos;

	public Vector3 Angle;

	public Vector3 TargetAngle;

	public float AnglePull;

	public float AngleDrag;

	public Vector3 AngleVel;

	public Vector3 Scale;

	public Vector3 TargetScale;

	public float ScalePull;

	public float ScaleDrag;

	public Vector3 ScaleVel;

	private bool _shakes;

	private Vector3 _shakeAcc;

	private Vector3 _shakeAmp1;

	private readonly Vector3 _minShakeAmp1;

	private readonly Vector3 _maxShakeAmp1;

	private readonly float _minShakeTime1;

	private readonly float _maxShakeTime1;

	private Vector3 _shakeAmp2;

	private readonly Vector3 _minShakeAmp2;

	private readonly Vector3 _maxShakeAmp2;

	private readonly float _minShakeTime2;

	private readonly float _maxShakeTime2;

	private readonly bool _realigns;

	private readonly float _minRealignInterval;

	private readonly float _maxRealignInterval;

	private bool _looksAt;

	private GameObject _lookAtTarget;

	private readonly float _minLookAtInterval;

	private readonly float _maxLookAtInterval;

	public Vector3 LookAtPoint;

	private readonly Vector2 _loopVal;

	public UpdateFunc Update;

	public ArriveFunc Arrive;

	public MilMo_ObjectMover()
	{
		_shake1 = Shake1;
		_shake2 = Shake2;
		_realign = Realign;
		_lookAt = DoLookAt;
		Vel.x = 0f;
		Vel.y = 0f;
		Vel.z = 0f;
		Pos.x = 0f;
		Pos.y = 0f;
		Pos.z = 0f;
		_internalPos = Vector3.zero;
		Target.x = 0f;
		Target.y = 0f;
		Target.z = 0f;
		Pull = 0.05f;
		Drag = 0.95f;
		MinVel.x = 1E-05f;
		MinVel.y = 1E-05f;
		MinVel.z = 1E-05f;
		MaxVel.x = 100f;
		MaxVel.y = 100f;
		MaxVel.z = 100f;
		Accel.x = 0f;
		Accel.y = 0f;
		Accel.z = 0f;
		AngleVel.x = 0f;
		AngleVel.y = 0f;
		AngleVel.z = 0f;
		Angle.x = 0f;
		Angle.y = 0f;
		Angle.z = 0f;
		TargetAngle.x = 0f;
		TargetAngle.y = 0f;
		TargetAngle.z = 0f;
		AnglePull = 0.05f;
		AngleDrag = 0.95f;
		ScaleVel.x = 0f;
		ScaleVel.y = 0f;
		ScaleVel.z = 0f;
		Scale.x = 0f;
		Scale.y = 0f;
		Scale.z = 0f;
		TargetScale.x = 0f;
		TargetScale.y = 0f;
		TargetScale.z = 0f;
		ScalePull = 0.05f;
		ScaleDrag = 0.95f;
		_shakes = false;
		_minShakeTime1 = 0.4f;
		_maxShakeTime1 = 0.6f;
		_minShakeAmp1.x = -0.0005f;
		_minShakeAmp1.y = -0.0005f;
		_minShakeAmp1.z = -0.0005f;
		_maxShakeAmp1.x = 0.0005f;
		_maxShakeAmp1.y = 0.0005f;
		_maxShakeAmp1.z = 0.0005f;
		_minShakeTime2 = 0.2f;
		_maxShakeTime2 = 0.3f;
		_minShakeAmp2.x = -0.0005f;
		_minShakeAmp2.y = -0.0005f;
		_minShakeAmp2.z = -0.0005f;
		_maxShakeAmp2.x = 0.0005f;
		_maxShakeAmp2.y = 0.0005f;
		_maxShakeAmp2.z = 0.0005f;
		_shakeAcc.x = 0f;
		_shakeAcc.y = 0f;
		_shakeAcc.z = 0f;
		_realigns = true;
		_minRealignInterval = 5f;
		_maxRealignInterval = 10f;
		_looksAt = false;
		_minLookAtInterval = 0.1f;
		_maxLookAtInterval = 0.1f;
		_loopVal = Vector2.zero;
		Update = Nothing;
		Arrive = Nothing;
	}

	public bool IsAttached(GameObject o)
	{
		return GameObjects.Contains(o);
	}

	public void AttachObject(GameObject o)
	{
		GameObjects.Add(o);
	}

	public void DetachObject(GameObject o)
	{
		GameObjects.Remove(o);
	}

	public void DetachAll()
	{
		GameObjects.Clear();
	}

	public void SetUpdateFunc(int func)
	{
		switch (func)
		{
		case 0:
			Update = Nothing;
			break;
		case 1:
			Update = Linear;
			break;
		case 2:
			Update = Spring;
			Pull = 0.05f;
			Drag = 0.95f;
			break;
		case 4:
			Update = Bounce;
			Pull = 0.003f;
			Drag = 0.2f;
			break;
		case 6:
			Update = Accelerate;
			break;
		case 7:
			Update = Surface;
			Pull = 0.05f;
			Drag = 0.1f;
			break;
		case 8:
			Update = Sinus;
			break;
		case 3:
		case 5:
			break;
		}
	}

	private void Nothing()
	{
	}

	private void Linear()
	{
		if (Paused)
		{
			return;
		}
		_internalPos += Vel;
		if (_internalPos.x >= _loopVal.x)
		{
			_internalPos.x = 0f;
		}
		if (_internalPos.y >= _loopVal.y)
		{
			_internalPos.y = 0f;
		}
		foreach (GameObject gameObject in GameObjects)
		{
			if (gameObject != null)
			{
				gameObject.transform.position = _internalPos;
			}
		}
		SpringRotate();
		Pos = _internalPos;
	}

	private void Accelerate()
	{
		if (Paused)
		{
			return;
		}
		Vel += Accel;
		_internalPos += Vel;
		if (Mathf.Abs(Vel.x - MaxVel.x) > 0.0001f || Mathf.Abs(Vel.y - MaxVel.y) > 0.0001f || Mathf.Abs(Vel.z - MaxVel.z) > 0.0001f)
		{
			Vel = Vector3.zero;
			Pause();
		}
		foreach (GameObject item in GameObjects.Where((GameObject o) => o != null))
		{
			item.transform.position = _internalPos;
		}
		SpringRotate();
		Pos = _internalPos;
	}

	private void Spring()
	{
		if (Paused)
		{
			return;
		}
		bool flag = false;
		if (Mathf.Abs(Vel.x - MinVel.x) > 0.0001f || Mathf.Abs(Vel.y - MinVel.y) > 0.0001f || Mathf.Abs(Vel.z - MinVel.z) > 0.0001f)
		{
			Vel.x += (Target.x - _internalPos.x) * Pull;
			Vel.y += (Target.y - _internalPos.y) * Pull;
			Vel.z += (Target.z - _internalPos.z) * Pull;
			Vel.x += Vel.x * Drag - Vel.x;
			Vel.y += Vel.y * Drag - Vel.y;
			Vel.z += Vel.z * Drag - Vel.z;
			_internalPos += Vel;
		}
		else if (Update != new UpdateFunc(Sinus))
		{
			Vel.x = 0f;
			Vel.y = 0f;
			Vel.z = 0f;
			_internalPos.x = Target.x;
			_internalPos.y = Target.y;
			_internalPos.z = Target.z;
			flag = true;
		}
		if (_shakes)
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
		foreach (GameObject item in GameObjects.Where((GameObject o) => o != null))
		{
			item.transform.position = _internalPos;
		}
		bool flag2 = SpringRotate();
		bool flag3 = SpringScale();
		if (!_shakes && flag && flag2 && flag3)
		{
			Paused = true;
		}
		if (Update != new UpdateFunc(Sinus))
		{
			Pos = _internalPos;
		}
	}

	private void Sinus()
	{
		Spring();
		SinVel.x = Mathf.Sin(Time.time * SinRate.x) * SinAmp.x;
		SinVel.y = Mathf.Sin(Time.time * SinRate.y) * SinAmp.y;
		SinVel.z = Mathf.Sin(Time.time * SinRate.z) * SinAmp.z;
		Pos = _internalPos + SinVel;
	}

	public bool SpringRotate()
	{
		if (Paused)
		{
			return true;
		}
		bool result = false;
		if (Mathf.Abs(AngleVel.x) > 0.0001f || Mathf.Abs(AngleVel.y) > 0.0001f || Mathf.Abs(AngleVel.z) > 0.0001f)
		{
			AngleVel.x += (TargetAngle.x - Angle.x) * AnglePull;
			AngleVel.y += (TargetAngle.y - Angle.y) * AnglePull;
			AngleVel.z += (TargetAngle.z - Angle.z) * AnglePull;
			AngleVel.x += AngleVel.x * AngleDrag - AngleVel.x;
			AngleVel.y += AngleVel.y * AngleDrag - AngleVel.y;
			AngleVel.z += AngleVel.z * AngleDrag - AngleVel.z;
			Angle += AngleVel * 2f;
		}
		else
		{
			AngleVel.x = 0f;
			AngleVel.y = 0f;
			AngleVel.z = 0f;
			Angle.x = TargetAngle.x;
			Angle.y = TargetAngle.y;
			Angle.z = TargetAngle.z;
			result = true;
		}
		foreach (GameObject gameObject in GameObjects)
		{
			if (gameObject != null)
			{
				gameObject.transform.eulerAngles = Angle;
			}
		}
		return result;
	}

	private bool SpringScale()
	{
		if (Paused)
		{
			return true;
		}
		bool result = false;
		if (Mathf.Abs(ScaleVel.x) > 0.0001f || Mathf.Abs(ScaleVel.y) > 0.0001f || Mathf.Abs(ScaleVel.z) > 0.0001f)
		{
			ScaleVel.x += (TargetScale.x - Scale.x) * ScalePull;
			ScaleVel.y += (TargetScale.y - Scale.y) * ScalePull;
			ScaleVel.z += (TargetScale.z - Scale.z) * ScalePull;
			ScaleVel.x += ScaleVel.x * ScaleDrag - ScaleVel.x;
			ScaleVel.y += ScaleVel.y * ScaleDrag - ScaleVel.y;
			ScaleVel.z += ScaleVel.z * ScaleDrag - ScaleVel.z;
			Scale += ScaleVel * 2f;
		}
		else
		{
			ScaleVel.x = 0f;
			ScaleVel.y = 0f;
			ScaleVel.z = 0f;
			Scale.x = TargetScale.x;
			Scale.y = TargetScale.y;
			Scale.z = TargetScale.z;
			result = true;
		}
		return result;
	}

	private void Surface()
	{
		if (Paused)
		{
			return;
		}
		bool flag = false;
		if (Mathf.Abs(Vel.y - MinVel.y) > 0.0001f)
		{
			if (_internalPos.y >= Target.y)
			{
				Vel.y -= Pull;
			}
			else if (Vel.y < 0f)
			{
				Vel.y *= 0.85f;
			}
			else
			{
				Vel.y += 0.01f;
			}
			_internalPos += Vel;
		}
		else
		{
			Vel.y = 0f;
			_internalPos.y = Target.y;
			flag = true;
		}
		foreach (GameObject item in GameObjects.Where((GameObject o) => o != null))
		{
			item.transform.position = _internalPos;
		}
		if (flag)
		{
			Paused = true;
		}
		Pos = _internalPos;
	}

	private void Bounce()
	{
		if (Paused)
		{
			return;
		}
		if (_internalPos.y >= Target.y)
		{
			Vel.y -= Pull;
			_internalPos += Vel;
		}
		else
		{
			_internalPos.y = Target.y;
			Vel.y = Mathf.Abs(Vel.y) * Drag;
		}
		if (Mathf.Abs(Vel.y) < 0.01f && _internalPos.y <= Target.y)
		{
			Pause();
			_internalPos.y = Target.y;
		}
		foreach (GameObject item in GameObjects.Where((GameObject o) => o != null))
		{
			item.transform.position = _internalPos;
		}
		SpringRotate();
		Pos = _internalPos;
	}

	private void GoTo(float x, float y, float z)
	{
		UnPause();
		Target.x = x;
		Target.y = y;
		Target.z = z;
		Vel.x = 0.002f;
	}

	public void GoTo(Vector3 pos)
	{
		GoTo(pos.x, pos.y, pos.z);
	}

	private void GoToNow(float x, float y, float z)
	{
		Target.x = x;
		Target.y = y;
		Target.z = z;
		Pos.x = Target.x;
		Pos.y = Target.y;
		Pos.z = Target.z;
		_internalPos = Target;
		Vel.x = 0f;
		Vel.y = 0f;
		Vel.z = 0f;
	}

	public void GoToNow(Vector3 pos)
	{
		GoToNow(pos.x, pos.y, pos.z);
	}

	public void SetAcceleration(float x, float y, float z)
	{
		UnPause();
		Vel = Vector3.zero;
		Accel.x = x;
		Accel.y = y;
		Accel.z = z;
	}

	public void Pause()
	{
		Paused = true;
	}

	public void UnPause()
	{
		Paused = false;
	}

	public void FallTo(float y)
	{
		UnPause();
		if (y < Pos.y)
		{
			Target.y = y;
		}
		Vel.y = -0.1f;
	}

	public void Impulse(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
	{
		UnPause();
		Vector3 vector = default(Vector3);
		vector.x = Random.Range(minX, maxX);
		vector.y = Random.Range(minY, maxY);
		vector.z = Random.Range(minZ, maxZ);
		Vel += vector;
	}

	public void Impulse(float x, float y, float z)
	{
		UnPause();
		Vel += new Vector3(x, y, z);
	}

	public void Impulse(Vector3 i)
	{
		UnPause();
		Vel += i;
	}

	public void SetAngle(float x, float y, float z)
	{
		UnPause();
		TargetAngle.x = x;
		TargetAngle.y = y;
		TargetAngle.z = z;
		_shakeAcc.x = 0f;
		_shakeAcc.y = 0f;
		_shakeAcc.z = 0f;
		AngleVel.x = 0.002f;
	}

	public void SetAngle(Vector3 v)
	{
		SetAngle(v.x, v.y, v.z);
	}

	public void AngleNow(float x, float y, float z)
	{
		TargetAngle.x = x;
		TargetAngle.y = y;
		TargetAngle.z = z;
		Angle.x = TargetAngle.x;
		Angle.y = TargetAngle.y;
		Angle.z = TargetAngle.z;
		AngleVel.x = 0.02f;
		AngleVel.y = 0f;
		_shakeAcc.x = 0f;
		_shakeAcc.y = 0f;
		_shakeAcc.z = 0f;
		AngleVel.z = 0f;
	}

	public void AngleNow(Vector3 v)
	{
		AngleNow(v.x, v.y, v.z);
	}

	public void LookAt(string target)
	{
		if (target.Length == 0)
		{
			_looksAt = false;
			_lookAtTarget = null;
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag(target);
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				_lookAtTarget = gameObject;
			}
		}
		_looksAt = true;
		DoLookAt();
	}

	private void DoLookAt()
	{
		if (!_looksAt)
		{
			return;
		}
		UnPause();
		bool flag = false;
		foreach (GameObject gameObject in GameObjects)
		{
			if (!flag && !(gameObject == null))
			{
				Vector3 eulerAngles = gameObject.transform.eulerAngles;
				gameObject.transform.LookAt(_lookAtTarget.transform);
				Vector3 eulerAngles2 = gameObject.transform.eulerAngles;
				gameObject.transform.eulerAngles = eulerAngles;
				SetAngle(eulerAngles2.x, eulerAngles2.y, eulerAngles2.z);
				flag = true;
			}
		}
		MilMo_EventSystem.At(Random.Range(_minLookAtInterval, _maxLookAtInterval), _lookAt);
	}

	public void AngleImpulse(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
	{
		UnPause();
		Vector3 vector = default(Vector3);
		vector.x = Random.Range(minX, maxX);
		vector.y = Random.Range(minY, maxY);
		vector.z = Random.Range(minZ, maxZ);
		AngleVel += vector;
	}

	public void AngleImpulse(Vector3 impulse)
	{
		UnPause();
		AngleVel += impulse;
	}

	public void ScaleTo(float x, float y, float z)
	{
		ScaleTo(new Vector3(x, y, z));
	}

	public void ScaleTo(Vector3 scale)
	{
		UnPause();
		TargetScale = scale;
		ScaleVel.x = 0.002f;
	}

	public void ScaleImpulse(Vector3 impulse)
	{
		UnPause();
		ScaleVel += impulse;
	}

	public void IsShaky(bool s)
	{
		if (s)
		{
			_shakes = true;
			Shake1();
			Shake2();
			Realign();
		}
		else
		{
			_shakes = false;
			_shakeAcc.x = 0f;
			_shakeAcc.y = 0f;
			_shakeAcc.z = 0f;
			Realign();
		}
	}

	private void Shake1()
	{
		if (_shakes && !Paused)
		{
			_shakeAmp1.x = Random.Range(_minShakeAmp1.x, _maxShakeAmp1.x);
			_shakeAmp1.y = Random.Range(_minShakeAmp1.y, _maxShakeAmp1.y);
			_shakeAmp1.z = Random.Range(_minShakeAmp1.z, _maxShakeAmp1.z);
			MilMo_EventSystem.At(Random.Range(_minShakeTime1, _maxShakeTime1), _shake1);
		}
	}

	private void Shake2()
	{
		if (_shakes && !Paused)
		{
			_shakeAmp2.x = Random.Range(_minShakeAmp2.x, _maxShakeAmp2.x);
			_shakeAmp2.y = Random.Range(_minShakeAmp2.y, _maxShakeAmp2.y);
			_shakeAmp2.z = Random.Range(_minShakeAmp2.z, _maxShakeAmp2.z);
			MilMo_EventSystem.At(Random.Range(_minShakeTime2, _maxShakeTime2), _shake2);
		}
	}

	private void Realign()
	{
		if (_shakes && _realigns)
		{
			SetAngle(TargetAngle.x, TargetAngle.y, TargetAngle.z);
			MilMo_EventSystem.At(Random.Range(_minRealignInterval, _maxRealignInterval), _realign);
		}
	}
}
