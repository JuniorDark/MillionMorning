using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerCurrencyInfoHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("got_currency_exchange_rate", message);
	}
}
