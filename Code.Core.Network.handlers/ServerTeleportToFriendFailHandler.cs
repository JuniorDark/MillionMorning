using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerTeleportToFriendFailHandler : IHandler
{
	public const sbyte NO_TELEPOD = 0;

	public const sbyte NOT_UNLOCKED = 1;

	public const sbyte SAME_INSTANCE = 2;

	public const sbyte UNKNOWN_ERROR = 3;

	public const sbyte SAME_HOME = 4;

	public const sbyte HOME_NO_ACCESS = 5;

	public const sbyte NOT_MEMBER = 6;

	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("teleport_to_friend_failed", message);
	}
}
