using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerGroupInitiateTravelHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("group_travel_initiated", message);
	}
}
