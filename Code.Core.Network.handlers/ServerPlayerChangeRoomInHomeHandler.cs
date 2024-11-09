using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerPlayerChangeRoomInHomeHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("player_change_room_in_home", message);
	}
}
