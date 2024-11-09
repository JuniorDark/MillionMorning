using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerPlayerRoomChangeErrorHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("home_room_change_failed", message);
	}
}
