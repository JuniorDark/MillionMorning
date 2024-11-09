using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerLadderPageHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("ladder_page_received", message);
	}
}
