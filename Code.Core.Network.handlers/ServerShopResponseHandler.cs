using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerShopResponseHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("shop_response", message);
	}
}
