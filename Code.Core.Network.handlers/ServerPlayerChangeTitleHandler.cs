using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerPlayerChangeTitleHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("title_change_success", message);
	}
}
