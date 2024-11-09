using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerEnterHomeFailHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("server_enter_home_fail", message);
	}
}
