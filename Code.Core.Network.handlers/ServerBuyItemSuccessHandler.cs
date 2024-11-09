using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerBuyItemSuccessHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("shop_buy_success", message);
	}
}
