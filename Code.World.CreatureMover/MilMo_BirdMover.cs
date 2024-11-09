using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.Utility;
using Code.World.Level;
using Code.World.Level.LevelObject;
using Code.World.Player;
using UnityEngine;

namespace Code.World.CreatureMover;

public class MilMo_BirdMover : MilMo_CreatureMover
{
	private const float MIN_HEIGHT_ABOVE_GROUND = 0.4f;

	public MilMo_BirdMover(MilMo_LevelCreatureTemplate template)
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
			if (MilMo_Utility.Equals(base.Position, Target) && object.Equals(base.Rotation, TargetRotation))
			{
				return;
			}
			float num = Velocity * time;
			if (IsStunned)
			{
				num = 0f;
			}
			if (num != 0f)
			{
				Vector3 vector = Target - base.Position;
				if (vector.sqrMagnitude <= num * num)
				{
					base.Position = Target;
				}
				else
				{
					vector.Normalize();
					base.Position += vector * num;
				}
			}
			Vector3 vector2 = base.Position + ImpactMover.Pos;
			Vector3 pos = vector2;
			pos.y = base.Position.y;
			if ((double)Mathf.Abs(MilMo_Level.GetWalkableHeight(pos) - pos.y) > 0.01)
			{
				vector2.y = Mathf.Max(vector2.y, MilMo_Level.GetWalkableHeight(pos) + 0.4f);
			}
			else
			{
				vector2.y = Mathf.Max(vector2.y, MilMo_Level.GetWalkableHeight(pos));
			}
			base.Position = vector2;
			if (Dead)
			{
				return;
			}
			if (RotationSpeed != 0f)
			{
				if (MilMo_Utility.IsClose(base.Rotation.eulerAngles, TargetRotation.eulerAngles, 0.01f))
				{
					base.Rotation = TargetRotation;
				}
				else
				{
					base.Rotation = Quaternion.Lerp(base.Rotation, TargetRotation, time * RotationSpeed);
				}
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
		}
	}

	protected override void SetTargetRotation()
	{
		TargetRotation = ((!MilMo_Utility.Equals(Target, base.Position)) ? Quaternion.LookRotation(Target - base.Position) : base.Rotation);
	}

	public override void TurnTo(Vector3 position)
	{
		TargetRotation = ((!MilMo_Utility.Equals(position, base.Position)) ? Quaternion.LookRotation(position - base.Position) : base.Rotation);
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
		else
		{
			ImpactMover.Impulse(vector * impact);
		}
	}

	public override void HandleRealImpulse(ServerMoveableImpulse msg)
	{
	}

	public override void ReadUpdate(CreatureUpdate creatureUpdate)
	{
		if (creatureUpdate != null)
		{
			float speed = creatureUpdate.GetSpeed();
			vector3 position = creatureUpdate.GetPosition();
			vector3 target = creatureUpdate.GetTarget();
			base.Position = new Vector3(position.GetX(), position.GetY(), position.GetZ());
			Target = new Vector3(target.GetX(), target.GetY(), target.GetZ());
			float num = 1000f;
			if (!MilMo_Utility.IsClose(speed, 0f, 0.0001f) && !MilMo_Utility.Equals(Target, base.Position))
			{
				num = (Target - base.Position).magnitude / speed;
			}
			float magnitude = (Target - base.Position).magnitude;
			Velocity = magnitude / num;
			if (!MilMo_Utility.Equals(base.Position, Target))
			{
				SetTargetRotation();
			}
			HasImpulse = false;
			PlayAnimation("Fly", 0.3f, WrapMode.Loop);
		}
	}

	public override void StartMoving(Vector3 target, float speed, float timeSinceCreationMsg)
	{
		Target = target;
		float magnitude = (Target - base.Position).magnitude;
		if (speed == 0f)
		{
			Target = base.Position;
			Velocity = 0f;
			return;
		}
		float num = magnitude / speed - timeSinceCreationMsg;
		if (num <= 0f)
		{
			base.Position = Target;
		}
		else
		{
			Velocity = speed;
			Update(timeSinceCreationMsg);
			float magnitude2 = (Target - base.Position).magnitude;
			Velocity = magnitude2 / num;
		}
		SetTargetRotation();
		HasImpulse = false;
		PlayAnimation("Fly", 0.3f, WrapMode.Loop, startAtRandomTime: true);
	}
}
