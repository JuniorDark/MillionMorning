using Code.Core.EventSystem;
using Code.Core.Input;
using Code.World.Climbing;
using UnityEngine;

namespace Code.World.Player;

public class MilMo_PlayerControllerClimb : MilMo_PlayerControllerBase
{
	public const float CLIMB_VELOCITY = 2f;

	public const float CLIMB_ANIMATION_SPEED = 1.2f;

	private const float ATTACH_VELOCITY = 4f;

	private const float ATTACH_ROTATION_VELOCITY = 4f;

	private const float ATTACH_LIMIT_SQUARED = 0.010000001f;

	private const float ATTACH_ROTATION_LIMIT = 5f;

	private const float LEAVE_VELOCITY = 6f;

	private const float LEAVE_LIMIT_SQUARED = 0.0064f;

	private readonly MilMo_GenericReaction _jumpButtonReaction;

	private readonly Transform _playerTransform;

	private readonly MilMo_ClimbingSurface _climbingSurface;

	private bool _attaching;

	private readonly MilMo_ClimbingSurface.MilMo_AttachInfo _attachInfo;

	private bool _leaving;

	private bool _leavingWithJump;

	private Vector3 _leavePosition;

	public override ControllerType Type => ControllerType.Climb;

	public ControllerType PreviousControllerType { get; }

	public MilMo_PlayerControllerClimb(MilMo_ClimbingSurface.MilMo_AttachInfo attachInfo, ControllerType previousControllerType)
	{
		PreviousControllerType = previousControllerType;
		_playerTransform = MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform;
		_climbingSurface = attachInfo.ClimbingSurface;
		_attachInfo = attachInfo;
		_attaching = true;
		if (_climbingSurface.AllowJump)
		{
			_jumpButtonReaction = MilMo_EventSystem.Listen("button_Jump", Jump);
			_jumpButtonReaction.Repeating = true;
		}
		MilMo_PlayerControllerBase.MovementState = MovementStates.Climb;
		MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_Climb", "");
		MilMo_PlayerControllerBase.Player.Avatar.PlayAnimation("GenericClimb01");
	}

	public override void Exit()
	{
		base.Exit();
		MilMo_EventSystem.RemoveReaction(_jumpButtonReaction);
		if (MilMo_PlayerControllerBase.Player.IsClimbing)
		{
			MilMo_PlayerControllerBase.Player.EndClimbing();
		}
		_attaching = false;
		_leaving = false;
		_leavingWithJump = false;
	}

	public override void UpdatePlayer()
	{
		base.UpdatePlayer();
		MilMo_PlayerControllerBase.UpdateGroundTypeConfig();
		HandleInput();
		HandleAttach();
		HandleLeave();
	}

	private void HandleAttach()
	{
		if (_attaching)
		{
			Vector3 position = _playerTransform.position;
			Quaternion rotation = _playerTransform.rotation;
			position = Vector3.Lerp(position, _attachInfo.Position, 4f * Time.deltaTime);
			rotation = Quaternion.Lerp(rotation, Quaternion.LookRotation(_attachInfo.Direction), 4f * Time.deltaTime);
			_playerTransform.position = position;
			_playerTransform.rotation = rotation;
			float sqrMagnitude = (position - _attachInfo.Position).sqrMagnitude;
			float num = Vector3.Angle(_playerTransform.forward, _attachInfo.Direction);
			if (sqrMagnitude < 0.010000001f && num < 5f)
			{
				_playerTransform.position = _attachInfo.Position;
				_playerTransform.forward = _attachInfo.Direction;
				_attaching = false;
			}
		}
	}

	private void HandleLeave()
	{
		if (_leaving)
		{
			if (_leavingWithJump)
			{
				MilMo_PlayerControllerBase.Player.Avatar.PlayAnimation(MilMo_PlayerControllerBase.Player.Avatar.JumpAnimation);
			}
			else if (!object.Equals(MilMo_Input.VerticalAxis, 0f) || !object.Equals(MilMo_Input.HorizontalAxis, 0f))
			{
				MilMo_PlayerControllerBase.Player.Avatar.PlayAnimation(MilMo_PlayerControllerBase.Player.Avatar.MoveAnimation);
			}
			else
			{
				MilMo_PlayerControllerBase.Player.Avatar.PlayAnimation(MilMo_PlayerControllerBase.Player.Avatar.IdleAnimation);
			}
			Vector3 position = _playerTransform.position;
			position = Vector3.Lerp(position, _leavePosition, 6f * Time.deltaTime);
			_playerTransform.position = position;
			if ((position - _leavePosition).sqrMagnitude < 0.0064f)
			{
				_playerTransform.position = _leavePosition;
				_leaving = false;
				MilMo_World.Instance.ChangePlayerController(PreviousControllerType);
			}
		}
	}

	private void HandleInput()
	{
		if (_attaching || _leaving)
		{
			return;
		}
		float verticalAxis = MilMo_Input.VerticalAxis;
		float horizontalAxis = MilMo_Input.HorizontalAxis;
		Vector3 position = MilMo_Player.Instance.Avatar.Position;
		position.y += verticalAxis * 2f * Time.deltaTime;
		position.x += horizontalAxis * 2f * Time.deltaTime;
		if (!object.Equals(verticalAxis, 0f) || (_climbingSurface.SupportHorizontalMovement && !object.Equals(horizontalAxis, 0f)))
		{
			MilMo_PlayerControllerBase.Player.Avatar.GameObject.GetComponent<Animation>()["GenericClimb01"].speed = ((verticalAxis > 0.5f) ? 1.2f : (-1.2f));
			if (!MilMo_Player.Instance.ClimbingSurface.Move(MilMo_Player.Instance, position, out _leavePosition))
			{
				_leaving = true;
			}
		}
		else
		{
			MilMo_PlayerControllerBase.Player.Avatar.GameObject.GetComponent<Animation>()["GenericClimb01"].speed = 0f;
		}
	}

	private void Jump(object o)
	{
		MilMo_ClimbingSurface climbingSurface = _climbingSurface;
		if (climbingSurface != null && climbingSurface.AllowJump && !_attaching && !_leaving)
		{
			_leaving = true;
			_leavingWithJump = true;
			Vector3 forward = _playerTransform.forward;
			_leavePosition = _playerTransform.position;
			_leavePosition -= forward * 0.42f;
			_leavePosition.y += 0.24f;
			MilMo_PlayerControllerBase.PreviousVelocityY = 0f;
			Vector3 currentSpeed = forward * (0f - MilMo_PlayerControllerBase.WalkJumpSpeed);
			currentSpeed.y += MilMo_PlayerControllerBase.WalkJumpSpeed;
			MilMo_PlayerControllerBase.CurrentSpeed = currentSpeed;
		}
	}

	public void ExhaustedKnockoff()
	{
		Jump(null);
	}
}
