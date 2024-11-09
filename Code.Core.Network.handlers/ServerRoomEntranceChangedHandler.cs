using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerRoomEntranceChangedHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("room_entrance_changed", message);
	}
}
