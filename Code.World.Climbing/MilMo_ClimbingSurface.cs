using Code.World.Player;
using UnityEngine;

namespace Code.World.Climbing;

public abstract class MilMo_ClimbingSurface
{
	public class MilMo_AttachInfo
	{
		public MilMo_ClimbingSurface ClimbingSurface;

		public Vector3 Position;

		public Vector3 Direction;

		public MilMo_AttachInfo(MilMo_ClimbingSurface climbingSurface, Vector3 position, Vector3 direction)
		{
			ClimbingSurface = climbingSurface;
			Position = position;
			Direction = direction;
		}
	}

	private readonly int _mIdentifier;

	protected bool MPlayerIsClose;

	public int Identifier => _mIdentifier;

	public abstract bool SupportHorizontalMovement { get; }

	public abstract bool AllowJump { get; }

	public bool PlayerIsClose => MPlayerIsClose;

	protected MilMo_ClimbingSurface(int identifier)
	{
		_mIdentifier = identifier;
	}

	public abstract bool ShouldAttach(MilMo_Player player, out MilMo_AttachInfo attachInfo, out float distanceToSurfaceSqr);

	public abstract bool Move(MilMo_Player player, Vector3 wantedPosition, out Vector3 leavePosition);

	public abstract bool HandleRemotePlayer(MilMo_RemotePlayer remotePlayer);
}
