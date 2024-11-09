using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerApplyRoomSkinHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("apply_room_skin", message);
	}
}
