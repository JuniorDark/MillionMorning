using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerEnterCharBuilderResponseHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("charbuilder_response", message);
	}
}
