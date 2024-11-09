using Code.Core.EventSystem;

namespace Code.Core.Network.handlers.PVP;

public class ServerPvPQueuesHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("pvp_queues", message);
	}
}
