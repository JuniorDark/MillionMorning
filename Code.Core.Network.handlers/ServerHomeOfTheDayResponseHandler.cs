using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerHomeOfTheDayResponseHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("home_of_the_day_response", message);
	}
}