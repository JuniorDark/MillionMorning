using Code.Core.Network.messages.server;
using Code.World.Level.LevelObject;
using Code.World.Player;
using UnityEngine;

namespace Code.World.CreatureMover;

public class MilMo_StandStillMover : MilMo_CreatureMover
{
	public MilMo_StandStillMover(MilMo_LevelCreatureTemplate template)
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
			return;
		}
		base.Position += ImpactMover.Pos;
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

	protected override void SetTargetRotation()
	{
	}

	public override void TurnTo(Vector3 position)
	{
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
	}
}
