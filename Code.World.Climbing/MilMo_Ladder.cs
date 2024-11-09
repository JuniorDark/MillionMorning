using Code.Core.Collision;
using Code.Core.Input;
using Code.World.Player;
using UnityEngine;

namespace Code.World.Climbing;

public class MilMo_Ladder : MilMo_ClimbingSurface
{
	private const float ladderAttachSqrDistance = 0.24010001f;

	private const float ladderClimbAnimationSqrDistance = 0.64000005f;

	private const float ladderAttachMoveAngleLimit = 25f;

	private const float ladderCameraAutoResetSqrDistance = 1f;

	private Vector3 m_Start;

	private Vector3 m_End;

	private Vector3 m_Normal;

	private Vector3 m_FaceDirection;

	private Vector3 m_Direction;

	private bool m_AllowJump = true;

	public override bool SupportHorizontalMovement => false;

	public override bool AllowJump => m_AllowJump;

	public MilMo_Ladder(int identifier, Vector3 start, Vector3 end, Vector3 rotationAsEulerAngles, Vector3 normal, bool allowJump)
		: base(identifier)
	{
		m_Start = start;
		m_End = end;
		m_Normal = normal;
		m_AllowJump = allowJump;
		m_FaceDirection = m_Normal * -1f;
		m_Direction = (m_End - m_Start).normalized;
	}

	public override bool ShouldAttach(MilMo_Player player, out MilMo_AttachInfo attachInfo, out float distanceToSurfaceSqr)
	{
		Transform transform = player.Avatar.GameObject.transform;
		Vector3 position = transform.position;
		float num = (distanceToSurfaceSqr = MilMo_Physics.PointLineSegmentSqrDistance(m_Start, m_End, position));
		MPlayerIsClose = num < 1f;
		if (num < 0.24010001f)
		{
			if (MilMo_Input.VerticalAxis < -0.5f)
			{
				if ((position - m_End).magnitude > 0.24010001f)
				{
					attachInfo = new MilMo_AttachInfo(null, Vector3.zero, Vector3.zero);
					return false;
				}
				Vector3 position2 = MilMo_Physics.ClosestPointOnRay(m_Start, m_Direction, position);
				position2.y -= player.Avatar.Height * 0.5f + 0.05f;
				attachInfo = new MilMo_AttachInfo(this, position2, m_FaceDirection);
				return true;
			}
			if (MilMo_Input.VerticalAxis > 0.5f)
			{
				if (position.y + player.Avatar.Height * 0.5f > m_End.y)
				{
					attachInfo = new MilMo_AttachInfo(null, Vector3.zero, Vector3.zero);
					return false;
				}
				Vector3 to = MilMo_Input.VerticalAxis * transform.forward;
				if (Vector3.Angle(m_FaceDirection, to) > 25f)
				{
					attachInfo = new MilMo_AttachInfo(null, Vector3.zero, Vector3.zero);
					return false;
				}
				Vector3 position3 = MilMo_Physics.ClosestPointOnRay(m_Start, m_Direction, position);
				attachInfo = new MilMo_AttachInfo(this, position3, m_FaceDirection);
				return true;
			}
			attachInfo = new MilMo_AttachInfo(null, Vector3.zero, Vector3.zero);
			return false;
		}
		attachInfo = new MilMo_AttachInfo(null, Vector3.zero, Vector3.zero);
		return false;
	}

	public override bool Move(MilMo_Player player, Vector3 wantedPosition, out Vector3 leavePosition)
	{
		Vector3 point = wantedPosition;
		point = MilMo_Physics.ClosestPointOnRay(m_Start, m_Direction, point);
		if (Physics.Linecast(player.Avatar.Position, point, out var hitInfo, -805306369))
		{
			leavePosition = hitInfo.point;
			leavePosition -= player.Avatar.GameObject.transform.forward * 0.5f;
			leavePosition.y += 0.05f;
			return false;
		}
		player.Avatar.GameObject.transform.position = point;
		if (point.y + player.Avatar.Height * 0.5f > m_End.y)
		{
			leavePosition = m_End;
			leavePosition += player.Avatar.GameObject.transform.forward * 0.1f;
			leavePosition.y += player.Avatar.Height * 0.5f;
			return false;
		}
		if (point.y < m_Start.y && MilMo_Input.VerticalAxis < -0.5f)
		{
			leavePosition = player.Avatar.GameObject.transform.position;
			leavePosition -= player.Avatar.GameObject.transform.forward * 0.5f;
			leavePosition.y += 0.05f;
			return false;
		}
		leavePosition = Vector3.zero;
		return true;
	}

	public override bool HandleRemotePlayer(MilMo_RemotePlayer remotePlayer)
	{
		Vector3 position = remotePlayer.Avatar.GameObject.transform.position;
		Vector3 vector = MilMo_Physics.ClosestPointOnRay(m_Start, m_Direction, position);
		remotePlayer.Avatar.GameObject.transform.position = vector;
		remotePlayer.Avatar.GameObject.transform.rotation = Quaternion.LookRotation(m_FaceDirection);
		return (vector - position).sqrMagnitude < 0.64000005f;
	}
}
