using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.Utility;
using Code.World.Level;
using Code.World.Level.LevelObject;
using Code.World.Player;
using UnityEngine;

namespace Code.World.CreatureMover;

public class MilMo_BeeMover : MilMo_CreatureMover
{
	private float _rotationY;

	private float _targetRotationY;

	private float _heightAboveGround = 1f;

	public MilMo_BeeMover(MilMo_LevelCreatureTemplate template)
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
			if (MilMo_Utility.Equals(base.Position, Target) && MilMo_Utility.IsClose(_rotationY, _targetRotationY, 0.1f))
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
			Vector3 vector = new Vector3(Target.x, 0f, Target.z) - new Vector3(base.Position.x, 0f, base.Position.z);
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
			float walkableHeight = MilMo_Level.GetWalkableHeight(vector2);
			vector2.y = walkableHeight + _heightAboveGround;
			base.Position = vector2;
			if (!Dead)
			{
				_rotationY = (MilMo_Utility.IsClose(_rotationY, _targetRotationY, 0.1f) ? _targetRotationY : Mathf.LerpAngle(_rotationY, _targetRotationY, time * RotationSpeed));
				base.Rotation = Quaternion.Euler(0f, _rotationY, 0f);
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
		}
	}

	protected override void SetTargetRotation()
	{
		TurnTo(Target);
	}

	public override void TurnTo(Vector3 position)
	{
		Vector3 vector = new Vector3(position.x, 0f, position.z);
		Vector3 position2 = base.Position;
		Vector3 vector2 = new Vector3(position2.x, 0f, position2.z);
		float targetRotationY = ((!MilMo_Utility.Equals(vector, vector2)) ? Quaternion.LookRotation(vector - vector2).eulerAngles.y : base.Rotation.eulerAngles.y);
		_targetRotationY = targetRotationY;
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
		if (!IsStunned)
		{
			PlayAnimation("Fly", 0.3f, WrapMode.Loop);
		}
	}

	public override void ReadUpdate(CreatureUpdate creatureUpdate)
	{
		base.ReadUpdate(creatureUpdate);
		PlayAnimation("Fly", 0.3f, WrapMode.Loop);
	}

	public override void StartMoving(Vector3 target, float speed, float timeSinceCreationMsg)
	{
		base.StartMoving(target, speed, timeSinceCreationMsg);
		if (Terrain.activeTerrain != null)
		{
			float walkableHeight = MilMo_Level.GetWalkableHeight(Target);
			_heightAboveGround = Target.y - walkableHeight;
		}
		PlayAnimation("Fly", 0.3f, WrapMode.Loop, startAtRandomTime: true);
	}
}
