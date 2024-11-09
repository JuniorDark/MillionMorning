using System;
using Code.Core.Collision;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.Utility;
using Code.World.Level.LevelObject;
using Code.World.Player;
using UnityEngine;

namespace Code.World.CreatureMover;

public class MilMo_FishMover : MilMo_CreatureMover
{
	private const float MIN_HEIGHT_ABOVE_GROUND = 0.2f;

	private float _acceleration;

	private const float JERK = 2f;

	public MilMo_FishMover(MilMo_LevelCreatureTemplate template)
		: base(template)
	{
		Velocity = 0f;
		RotationSpeed = 5f;
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
			if (MilMo_Utility.Equals(base.Position, Target) && object.Equals(base.Rotation, TargetRotation))
			{
				return;
			}
			float num = Velocity * time + _acceleration * time * time / 2f + 2f * time * time * time / 6f;
			Vector3 vector = Target - base.Position;
			bool flag = false;
			if (vector.sqrMagnitude <= num * num)
			{
				base.Position = Target;
				flag = true;
			}
			else
			{
				vector.Normalize();
				base.Position += vector * num;
			}
			Vector3 vector2 = base.Position + ImpactMover.Pos;
			vector2.y = Mathf.Max(vector2.y, MilMo_Physics.GetTerrainHeight(vector2) + 0.2f);
			base.Position = vector2;
			if (MilMo_Utility.IsClose(base.Rotation.eulerAngles, TargetRotation.eulerAngles, 0.01f))
			{
				base.Rotation = TargetRotation;
			}
			else
			{
				base.Rotation = Quaternion.Lerp(base.Rotation, TargetRotation, time * RotationSpeed);
			}
			base.Rotation = Quaternion.Euler(base.Rotation.eulerAngles + ImpactMover.Angle);
			if (ImpactType == "Scale")
			{
				Vector3 localScale = OriginalScale + ImpactMover.Scale;
				localScale.x = Mathf.Clamp(localScale.x, 0f, localScale.x);
				localScale.y = Mathf.Clamp(localScale.y, 0f, localScale.y);
				localScale.z = Mathf.Clamp(localScale.z, 0f, localScale.z);
				base.LocalScale = localScale;
			}
			if (Velocity > 0f)
			{
				float num2 = Velocity + _acceleration * time;
				if (num2 <= 0f && flag)
				{
					Velocity = 0f;
					base.Position = Target;
					PlayAnimation("Idle", 0.3f, WrapMode.Loop);
				}
				else if (num2 > 0f)
				{
					Velocity = num2;
				}
			}
			if (_acceleration < 0f)
			{
				_acceleration += 2f * time;
				if (_acceleration > 0f)
				{
					_acceleration = 0f;
				}
			}
		}
	}

	protected override void SetTargetRotation()
	{
		TurnTo(Target);
	}

	public override void TurnTo(Vector3 position)
	{
		Quaternion targetRotation = (MilMo_Utility.Equals(position, base.Position) ? base.Rotation : Quaternion.LookRotation(position - base.Position));
		TargetRotation = targetRotation;
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
		vector.Normalize();
		if (ImpactType == "Rotational")
		{
			Vector3 vector2 = vector * (impact * 100f);
			ImpactMover.AngleImpulse(vector2.x, vector2.x, vector2.y, vector2.y, vector2.z, vector2.z);
		}
		else if (ImpactType == "Positional")
		{
			ImpactMover.Impulse(vector * impact);
		}
	}

	public override void ReadUpdate(CreatureUpdate creatureUpdate)
	{
		if (creatureUpdate != null)
		{
			vector3 target = creatureUpdate.GetTarget();
			Target = new Vector3(target.GetX(), target.GetY(), target.GetZ());
			float magnitude = (Target - base.Position).magnitude;
			float num = (float)Math.Pow(6f * magnitude / 2f, 1.0 / 3.0);
			_acceleration = -2f * num;
			Velocity = (0f - _acceleration) * num - 2f * num * num / 2f;
			SetTargetRotation();
			PlayAnimation("Swim", 0.3f, WrapMode.Loop);
		}
	}

	public override void StartMoving(Vector3 target, float speed, float timeSinceCreationMsg)
	{
		Target = target;
		float magnitude = (Target - base.Position).magnitude;
		float num = (float)Math.Pow(6f * magnitude / 2f, 1.0 / 3.0);
		_acceleration = -2f * num;
		Velocity = (0f - _acceleration) * num - 2f * num * num / 2f;
		SetTargetRotation();
		PlayAnimation("Swim", 0.3f, WrapMode.Loop, startAtRandomTime: true);
	}

	public override void HandleRealImpulse(ServerMoveableImpulse msg)
	{
	}
}
