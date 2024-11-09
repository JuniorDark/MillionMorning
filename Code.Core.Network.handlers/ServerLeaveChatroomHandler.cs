using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerLeaveChatroomHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("leave_chatroom", message);
	}
}
