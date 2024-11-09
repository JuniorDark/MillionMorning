using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerAddPlayerToHomeOfTheDayRaffleResponseHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("home_of_the_day_raffle_response", message);
	}
}
