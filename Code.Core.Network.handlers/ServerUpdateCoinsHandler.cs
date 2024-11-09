using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerUpdateCoinsHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("voucher_points_updated", message);
	}
}
