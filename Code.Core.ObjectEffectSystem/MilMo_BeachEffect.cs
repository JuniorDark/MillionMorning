using System;
using System.Collections.Generic;
using Code.Core.Collision;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_BeachEffect : MilMo_ObjectEffect
{
	private const int LOWEST_POINT_SAMPLES = 16;

	private float _speed;

	private float _acceleration;

	private readonly float _jerk;

	private readonly float _duration;

	private readonly Vector3 _targetPosition;

	private Vector3 _direction;

	private bool _isSetup;

	private float _totalDistanceMoved;

	public override float Duration => _duration;

	private MilMo_BeachEffectTemplate Template => EffectTemplate as MilMo_BeachEffectTemplate;

	public MilMo_BeachEffect(GameObject gameObject, MilMo_BeachEffectTemplate template)
		: base(gameObject, template)
	{
		_jerk = template.CurveSteepness;
		_targetPosition = gameObject.transform.position;
		float num = (float)Math.Pow(6f * template.Distance / _jerk, 1.0 / 3.0);
		_acceleration = (0f - _jerk) * num;
		_speed = (0f - _acceleration) * num - _jerk * num * num / 2f;
		_duration = num;
	}

	public override bool Update()
	{
		if (GameObject == null)
		{
			Destroy();
			return false;
		}
		if (!_isSetup)
		{
			Setup();
		}
		float deltaTime = Time.deltaTime;
		float num = _speed * deltaTime;
		float num2 = _acceleration * deltaTime * deltaTime * 0.5f;
		float num3 = _jerk * deltaTime * deltaTime * deltaTime * 0.2f;
		float num4 = Math.Max(0.01f, num + num2 + num3);
		Vector3 targetPosition;
		if ((double)Mathf.Abs(_totalDistanceMoved + num4 - Template.Distance) < 0.1)
		{
			targetPosition = _targetPosition;
			targetPosition.y = MilMo_Physics.GetTerrainHeight(targetPosition);
			GameObject.transform.position = targetPosition;
			Destroy();
			return false;
		}
		targetPosition = GameObject.transform.position + _direction * num4;
		targetPosition.y = MilMo_Physics.GetTerrainHeight(targetPosition);
		GameObject.transform.position = targetPosition;
		_totalDistanceMoved += num4;
		if (_speed > 0f)
		{
			float num5 = _speed + _acceleration * deltaTime;
			if (num5 > 0f)
			{
				_speed = num5;
			}
		}
		if (_acceleration < 0f)
		{
			_acceleration += _jerk * deltaTime;
			if (_acceleration > 0f)
			{
				_acceleration = 0f;
			}
		}
		return true;
	}

	private void Setup()
	{
		Vector3 lowestPoint = GetLowestPoint();
		_direction = (_targetPosition - lowestPoint).normalized;
		GameObject.transform.position = lowestPoint;
		_isSetup = true;
	}

	private Vector3 GetLowestPoint()
	{
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < 16; i++)
		{
			float f = (float)(i * 2) * MathF.PI / 16f;
			list.Add(new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f)));
		}
		Vector3 result = _targetPosition;
		float num = float.MaxValue;
		foreach (Vector3 item in list)
		{
			Vector3 vector = _targetPosition + item * Template.Distance;
			float terrainHeight = MilMo_Physics.GetTerrainHeight(vector);
			if (terrainHeight < num)
			{
				result = vector;
				num = terrainHeight;
			}
		}
		return result;
	}
}
