using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerBuyItemFailHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("shop_buy_fail", message);
	}
}
