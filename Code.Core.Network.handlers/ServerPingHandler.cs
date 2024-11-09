using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerPingHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("ping_response");
	}
}
