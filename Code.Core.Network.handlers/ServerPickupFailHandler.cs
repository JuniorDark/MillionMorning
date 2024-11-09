using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerPickupFailHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("pickup_fail");
	}
}
