using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerPlayerChangeRoomHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("player_change_room", message);
	}
}
