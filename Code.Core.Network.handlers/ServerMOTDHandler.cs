using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerMOTDHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("message_of_the_day", message);
	}
}
