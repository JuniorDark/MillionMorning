using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerChangeNameResponseHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("name_change_result", message);
	}
}
