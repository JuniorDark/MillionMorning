using Code.Core.EventSystem;
using Code.Core.Network.messages.server;

namespace Code.Core.Network.handlers;

public class ServerEnterChatroomHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		ServerEnterChatroom serverEnterChatroom = (ServerEnterChatroom)message;
		MilMo_EventSystem.Instance.AsyncPostEvent("enter_chatroom", serverEnterChatroom.getPlayerChatroomInfo());
	}
}
