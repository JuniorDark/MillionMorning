using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerTeleportToFriendOkHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("teleport_to_friend_ok", message);
	}
}
