using System;
using Code.Core.Avatar;
using Code.Core.Collision;
using UnityEngine;

namespace Code.World.Player;

public class MilMo_PlayerController : MonoBehaviour
{
	private enum State
	{
		Idle,
		Walking,
		Running
	}

	private const float WALK_SPEED = 8f;

	private const float RUN_SPEED = 16f;

	private const float TURN_SPEED = 10f;

	private Vector3 _wantedPosition;

	private float _wantedRotation;

	private MilMo_Avatar _avatar;

	private State _state;

	private State _prevState;

	private void Update()
	{
		Vector3 dir = _wantedPosition - _avatar.GameObject.transform.position;
		if (dir.magnitude < 0.05f)
		{
			_state = State.Idle;
		}
		switch (_state)
		{
		case State.Idle:
			IdleState();
			break;
		case State.Walking:
			WalkingState(dir);
			break;
		case State.Running:
			RunningState(dir);
			break;
		}
		_prevState = _state;
	}

	private void RunningState(Vector3 dir)
	{
		dir.Normalize();
		Vector3 position = _avatar.GameObject.transform.position;
		position += 16f * Time.deltaTime * dir;
		position.y = MilMo_Physics.GetDistanceToGround(position);
		_avatar.GameObject.transform.position = position;
		_avatar.GameObject.transform.rotation = Quaternion.Lerp(_avatar.GameObject.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
		_avatar.PlayAnimation("Run");
	}

	private void WalkingState(Vector3 dir)
	{
		dir.Normalize();
		Vector3 position = _avatar.GameObject.transform.position;
		position += 8f * Time.deltaTime * dir;
		position.y = MilMo_Physics.GetDistanceToGround(position);
		_avatar.GameObject.transform.position = position;
		_avatar.GameObject.transform.rotation = Quaternion.Lerp(_avatar.GameObject.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
		_avatar.PlayAnimation("Walk");
	}

	private void IdleState()
	{
		if (Mathf.Abs(_wantedRotation - _avatar.GameObject.transform.eulerAngles.y) > 0.05f)
		{
			_avatar.GameObject.transform.rotation = Quaternion.Lerp(_avatar.GameObject.transform.rotation, Quaternion.Euler(0f, _wantedRotation * 180f / MathF.PI, 0f), Time.deltaTime * 10f);
		}
		if (_state != _prevState)
		{
			_avatar.PlayAnimation("Idle");
		}
	}
}
