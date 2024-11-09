using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.Utility;
using Code.World.Level;
using Code.World.Level.LevelObject;
using Code.World.Player;
using UnityEngine;

namespace Code.World.CreatureMover;

public class MilMo_LandMover : MilMo_CreatureMover
{
	private float _ySpeed;

	private bool _onTheGround = true;

	private float _realRotationY;

	private float _targetRotationY;

	public MilMo_LandMover(MilMo_LevelCreatureTemplate template)
		: base(template)
	{
	}

	public override void Update()
	{
		Update(Time.deltaTime);
	}

	protected override void Update(float time)
	{
		if (!base.IsReady)
		{
			return;
		}
		if (Dead && (bool)base.DeathCollectNode)
		{
			HandleDeathCollectNodeInterpolation();
		}
		else
		{
			if (MilMo_Utility.Equals(base.Position, Target) && MilMo_Utility.IsClose(_realRotationY, _targetRotationY, 0.1f))
			{
				return;
			}
			float num = Velocity * time;
			if (HasImpulse)
			{
				num -= 15f * time * time * 0.5f;
			}
			else if (IsStunned)
			{
				num = 0f;
			}
			bool flag = false;
			if (num != 0f)
			{
				Vector3 vector = new Vector3(Target.x, 0f, Target.z) - new Vector3(base.Position.x, 0f, base.Position.z);
				if (vector.sqrMagnitude <= num * num)
				{
					if (!MilMo_Utility.Equals(base.Position, Target))
					{
						base.Position = Target;
						if (!IsStunned)
						{
							PlayAnimation("Idle", 0.3f, WrapMode.Loop);
						}
					}
					flag = true;
				}
				else
				{
					vector.Normalize();
					Vector3 position = base.Position + vector * num;
					position.y += _ySpeed * time;
					base.Position = position;
				}
			}
			Vector3 vector2 = base.Position + ImpactMover.Pos;
			Vector3 normal = Vector3.zero;
			if (!Template.IsImmobile)
			{
				float walkableHeight = MilMo_Level.GetWalkableHeight(vector2, out normal);
				if (!_onTheGround && vector2.y <= walkableHeight)
				{
					_onTheGround = true;
					_ySpeed = 0f;
				}
				if (_onTheGround && !base.HasDeathImpulse)
				{
					vector2.y = walkableHeight;
				}
			}
			base.Position = vector2;
			if (!Dead)
			{
				if (MilMo_Utility.IsClose(_realRotationY, _targetRotationY, 0.1f))
				{
					_realRotationY = _targetRotationY;
				}
				else
				{
					_realRotationY = Mathf.LerpAngle(_realRotationY, _targetRotationY, time * RotationSpeed);
				}
				base.Rotation = Quaternion.Euler(0f, _realRotationY, 0f);
				if (!MilMo_Utility.Equals(normal, Vector3.zero))
				{
					base.Rotation = Quaternion.FromToRotation(Vector3.up, normal) * base.Rotation;
				}
				if (ImpactMover.Angle != Vector3.zero)
				{
					base.Rotation = Quaternion.Euler(base.Rotation.eulerAngles + ImpactMover.Angle);
				}
				if (ImpactType == "Scale")
				{
					Vector3 localScale = OriginalScale + ImpactMover.Scale;
					localScale.x = Mathf.Clamp(localScale.x, 0f, localScale.x);
					localScale.y = Mathf.Clamp(localScale.y, 0f, localScale.y);
					localScale.z = Mathf.Clamp(localScale.z, 0f, localScale.z);
					base.LocalScale = localScale;
				}
			}
			if (HasImpulse && Velocity > 0f)
			{
				float num2 = Velocity - 15f * time;
				if (num2 <= 0f && flag)
				{
					Velocity = 0f;
					base.Position = Target;
				}
				else if (num2 > 0f)
				{
					Velocity = num2;
				}
			}
			if (!_onTheGround && !base.HasDeathImpulse && !Template.IsImmobile)
			{
				_ySpeed += -7.64f * time;
			}
			if (base.HasDeathImpulse || Template.IsImmobile)
			{
				_ySpeed = 0f;
			}
		}
	}

	protected override void SetTargetRotation()
	{
		Vector3 vector = new Vector3(Target.x, 0f, Target.z);
		Vector3 vector2 = new Vector3(base.Position.x, 0f, base.Position.z);
		_targetRotationY = ((!MilMo_Utility.Equals(vector, vector2, 0.01f)) ? Quaternion.LookRotation(vector - vector2).eulerAngles.y : base.Rotation.eulerAngles.y);
	}

	public override void TurnTo(Vector3 position)
	{
		Vector3 vector = new Vector3(position.x, 0f, position.z);
		Vector3 vector2 = new Vector3(base.Position.x, 0f, base.Position.z);
		_targetRotationY = ((!MilMo_Utility.Equals(vector, vector2, 0.01f)) ? Quaternion.LookRotation(vector - vector2).eulerAngles.y : base.Rotation.eulerAngles.y);
	}

	public override void AddLocalImpulse(float impact)
	{
		if (!base.IsReady)
		{
			return;
		}
		if (ImpactType == "Scale")
		{
			ImpactMover.ScaleImpulse(Vector3.down * (impact * 0.5f));
			return;
		}
		Vector3 vector = base.Position - MilMo_Player.Instance.Avatar.GameObject.transform.position;
		if (ImpactType == "Rotational")
		{
			vector.y = 0f;
			vector.Normalize();
			ImpactMover.AngleImpulse(vector * (impact * 149f));
		}
		else if (ImpactType == "Positional")
		{
			vector.y = 0f;
			vector.Normalize();
			ImpactMover.Impulse(vector * (impact * 1f));
		}
	}

	public override void HandleRealImpulse(ServerMoveableImpulse msg)
	{
		base.HandleRealImpulse(msg);
		float num = msg.getImpulse() / 15f;
		_ySpeed = (Target.y - base.Position.y) / num - -3.82f * num;
		_onTheGround = false;
		if (!IsStunned)
		{
			PlayAnimation("Walk", 0.3f, WrapMode.Loop);
		}
	}

	public override void ReadUpdate(CreatureUpdate creatureUpdate)
	{
		base.ReadUpdate(creatureUpdate);
		PlayAnimation("Walk", 0.3f, WrapMode.Loop);
	}

	public override void StartMoving(Vector3 target, float speed, float timeSinceCreationMsg)
	{
		base.StartMoving(target, speed, timeSinceCreationMsg);
		PlayAnimation("Walk", 0.3f, WrapMode.Loop, startAtRandomTime: true);
	}
}
