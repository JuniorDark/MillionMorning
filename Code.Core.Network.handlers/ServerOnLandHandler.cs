using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerOnLandHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("received_on_land", message);
	}
}
