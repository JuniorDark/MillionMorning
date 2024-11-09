using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerShopItemsHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("shop_items", message);
	}
}
