using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerGroupTravelResponseHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("group_travel_response", message);
	}
}
