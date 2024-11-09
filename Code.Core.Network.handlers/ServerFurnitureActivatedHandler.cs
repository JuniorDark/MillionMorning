using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerFurnitureActivatedHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("home_furniture_activated", message);
	}
}
