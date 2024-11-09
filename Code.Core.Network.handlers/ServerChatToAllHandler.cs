using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerChatToAllHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("chat_to_all", message);
	}
}
