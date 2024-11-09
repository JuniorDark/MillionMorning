using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerExpUpdateHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("server_exp_update", message);
	}
}
