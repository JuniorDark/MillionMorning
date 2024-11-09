using System.Collections.Generic;
using UnityEngine;

namespace Code.Core.Camera;

public static class MilMo_CameraOcclusion
{
	private static float _sphereSize;

	private static float _stickSize;

	private static float _startMoveSpeed;

	private static float _moveSpeed;

	private static float _moveAcceleration;

	private static float _maxMoveSpeed;

	private static int _smoothingBufferSize;

	private static int _maxCollisionIterations;

	private static Vector3 _lastFoundDirection;

	private static bool _haveLastDirection;

	private static int _loopCounter;

	private static readonly LinkedList<Vector3> LastDirectionsBuffer;

	private static Vector3 _velocity;

	private const int CAMERA_COLLISION_LAYERS = -1;

	static MilMo_CameraOcclusion()
	{
		_sphereSize = 0.48f;
		_stickSize = 0.4f;
		_startMoveSpeed = 4f;
		_moveAcceleration = 16f;
		_maxMoveSpeed = 32f;
		_smoothingBufferSize = 3;
		_maxCollisionIterations = 20;
		LastDirectionsBuffer = new LinkedList<Vector3>();
		for (int i = 0; i < _smoothingBufferSize; i++)
		{
			LastDirectionsBuffer.AddLast(Vector3.zero);
		}
	}

	public static bool DoMove(Vector3 from, Vector3 to, Vector3 lookAt, Quaternion currentRot, bool doCollision, bool searchVertically, bool searchFullCircle, float wantedDistance, out Vector3 cameraPosition)
	{
		cameraPosition = to;
		if (doCollision)
		{
			_loopCounter = 0;
			if (Physics.Linecast(from, to, out var hitInfo, -1))
			{
				if (Physics.Linecast(lookAt, to, out hitInfo, -1))
				{
					Vector3 normalized = (lookAt - to).normalized;
					cameraPosition = hitInfo.point + normalized * _sphereSize;
				}
				else
				{
					cameraPosition = from;
				}
				return true;
			}
			_moveSpeed = _startMoveSpeed;
			return false;
		}
		if (Physics.CheckCapsule(cameraPosition, lookAt, _stickSize, -1))
		{
			Vector3 vector = cameraPosition - lookAt;
			vector.Normalize();
			if (_haveLastDirection)
			{
				Vector3 start = lookAt + _lastFoundDirection * wantedDistance;
				Vector3 end = lookAt + _lastFoundDirection * _stickSize;
				if (!Physics.CheckCapsule(start, end, _stickSize, -1))
				{
					Vector3 vector2 = LerpDirection(vector, SmoothDirection(vector));
					cameraPosition = lookAt + vector2 * wantedDistance;
					_haveLastDirection = true;
					return true;
				}
				_loopCounter = 0;
			}
			int i = 0;
			float num = 1.013417f + (float)_loopCounter * 1.013417f;
			float num2 = 0.82f * (float)_loopCounter;
			for (; i < _maxCollisionIterations; i++)
			{
				Vector3 vector3 = Vector3.Cross(vector, Vector3.up);
				float num3 = Mathf.Cos(num) * num2;
				if (searchFullCircle)
				{
					num3 *= 2.5f;
				}
				Quaternion quaternion = Quaternion.Euler(0f, num3, 0f);
				if (searchVertically)
				{
					float num4 = Mathf.Sin(num) * num2;
					quaternion *= Quaternion.Euler(vector3 * num4);
				}
				Vector3 vector4 = vector;
				vector4 = quaternion * vector4;
				vector4.Normalize();
				Vector3 start2 = lookAt + vector4 * wantedDistance;
				Vector3 end2 = lookAt;
				end2 += vector4 * _stickSize;
				if (!Physics.CheckCapsule(start2, end2, _stickSize, -1))
				{
					_lastFoundDirection = vector4;
					Vector3 vector5 = LerpDirection(vector, SmoothDirection(vector));
					cameraPosition = lookAt + vector5 * wantedDistance;
					_haveLastDirection = true;
					_loopCounter = 0;
					return true;
				}
				num2 += 0.82f;
				num += 1.013417f;
				_loopCounter++;
			}
			_moveSpeed = _startMoveSpeed;
			_haveLastDirection = false;
			if (_loopCounter > 100)
			{
				_loopCounter = 0;
			}
			return false;
		}
		_moveSpeed = _startMoveSpeed;
		_haveLastDirection = false;
		_loopCounter = 0;
		return false;
	}

	private static Vector3 LerpDirection(Vector3 from, Vector3 to)
	{
		_moveSpeed = Mathf.Min(_moveSpeed + Time.deltaTime * _moveAcceleration, _maxMoveSpeed);
		return Vector3.Lerp(from, to, Time.deltaTime * _moveSpeed);
	}

	private static Vector3 SmoothDirection(Vector3 from)
	{
		if (!_haveLastDirection)
		{
			for (LinkedListNode<Vector3> linkedListNode = LastDirectionsBuffer.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value = from;
			}
		}
		LastDirectionsBuffer.AddLast(_lastFoundDirection);
		LastDirectionsBuffer.RemoveFirst();
		Vector3 zero = Vector3.zero;
		foreach (Vector3 item in LastDirectionsBuffer)
		{
			zero += item;
		}
		return zero / LastDirectionsBuffer.Count;
	}
}
