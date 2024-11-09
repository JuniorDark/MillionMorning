using Code.Core.Collision;
using Code.Core.EventSystem;
using Code.World.Gameplay;
using UnityEngine;

namespace Code.World.Player;

public class MilMo_PlayerControllerSplineRide : MilMo_PlayerControllerBase
{
	private const float ROTATION_INTERPOLATION_SPEED = 10f;

	private static MilMo_PlayerSpline _spline = new MilMo_PlayerSpline();

	private float _time;

	private float _lastDeltaTime;

	private MilMo_GameObjectSpline.SplinePoint _currentPoint;

	public override ControllerType Type => ControllerType.SplineRide;

	public static MilMo_PlayerSpline Spline
	{
		get
		{
			return _spline;
		}
		set
		{
			_spline = value;
		}
	}

	public MilMo_PlayerControllerSplineRide()
	{
		if (_spline != null)
		{
			_currentPoint = _spline.GetPointAtTime(0f);
			MilMo_PlayerControllerBase.Player.Avatar.PlayAnimation(_currentPoint.Animation);
			MilMo_PlayerControllerBase.Player.Avatar.SuperAlivenessManager.Disable();
			MilMo_PlayerControllerBase.Player.Avatar.PlaySoundEffectLooping(_spline.Template.BackgroundSound);
		}
	}

	public override void UpdatePlayer()
	{
		if (_spline == null)
		{
			MilMo_World.Instance.ChangePlayerController(ControllerType.Game);
			return;
		}
		Vector3 position = MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.position;
		MilMo_GameObjectSpline.SplinePoint pointAtTime = _spline.GetPointAtTime(_time);
		MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.position = pointAtTime.Position;
		if (pointAtTime.Animation != _currentPoint.Animation)
		{
			MilMo_PlayerControllerBase.Player.Avatar.PlayAnimation(pointAtTime.Animation);
		}
		if (pointAtTime.Sound != _currentPoint.Sound)
		{
			MilMo_PlayerControllerBase.Player.Avatar.PlaySoundEffect(pointAtTime.Sound);
		}
		_currentPoint = pointAtTime;
		HandleRotation();
		if (_spline.IsAtEnd(_time))
		{
			MilMo_PlayerControllerBase.WasGrounded = false;
			MilMo_World.Instance.ChangePlayerController(ControllerType.Game);
		}
		else if (_lastDeltaTime != 0f)
		{
			Vector3 currentSpeed = MilMo_PlayerControllerBase.CurrentSpeed;
			currentSpeed.y = Mathf.Max((pointAtTime.Position.y - position.y) / _lastDeltaTime, MilMo_PlayerControllerBase.MinVelocityY);
			MilMo_PlayerControllerBase.CurrentSpeed = currentSpeed;
		}
		_time += Time.deltaTime;
		_lastDeltaTime = Time.deltaTime;
		base.UpdatePlayer();
	}

	public override void Exit()
	{
		base.Exit();
		MilMo_PlayerControllerBase.Player.Avatar.SuperAlivenessManager.Enable();
		MilMo_PlayerControllerBase.Player.Avatar.StopLoopingSoundEffect(_spline.Template.BackgroundSound);
		if (MilMo_Physics.GetDistanceToGround(MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.position) <= MilMo_PlayerControllerBase.IsOnGroundTolerance || MilMo_PlayerControllerBase.IsOnTheGround)
		{
			if (MilMo_PlayerControllerBase.CurrentSpeed.y > MilMo_PlayerControllerBase.HardImpactVelocityLimit)
			{
				MilMo_PlayerControllerBase.Player.Avatar.EmitPuff("SoftImpact");
			}
			else
			{
				MilMo_PlayerControllerBase.Player.Avatar.EmitPuff("HardImpact");
				if (MilMo_PlayerControllerBase.Player.Avatar.GameObject.GetComponent<Animation>()["SuperLand01"] != null)
				{
					Lock(MilMo_PlayerControllerBase.Player.Avatar.GameObject.GetComponent<Animation>()["SuperLand01"].length, playMoveAnimationOnUnlock: true);
					MilMo_PlayerControllerBase.Player.Avatar.PlayAnimation("SuperLand01");
				}
			}
			MilMo_PlayerControllerBase.WasGrounded = true;
		}
		MilMo_EventSystem.Instance.PostEvent("spline_ride_end", _spline);
	}

	private void HandleRotation()
	{
		Quaternion b = _currentPoint.AlignAxis switch
		{
			2 => Quaternion.LookRotation(_currentPoint.Binormal, _currentPoint.Tangent), 
			3 => Quaternion.LookRotation(_currentPoint.Tangent, -_currentPoint.Binormal), 
			1 => Quaternion.LookRotation(_currentPoint.Normal, -_currentPoint.Binormal), 
			_ => Quaternion.Euler(0f, Quaternion.LookRotation(_currentPoint.Tangent, Vector3.up).eulerAngles.y, 0f), 
		};
		MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation = Quaternion.Slerp(MilMo_PlayerControllerBase.Player.Avatar.GameObject.transform.rotation, b, 10f * Time.deltaTime);
	}
}
