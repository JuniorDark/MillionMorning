using Code.Core.EventSystem;

namespace Code.Core.Network.handlers.PVP;

public class ServerPvPQueueSizeUpdateHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("pvp_queue_size_update", message);
	}
}
