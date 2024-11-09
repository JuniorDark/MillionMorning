using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerHubResponseHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("hub_response", message);
	}
}
