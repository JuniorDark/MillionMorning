using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerShopCategoriesHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("shop_categories", message);
	}
}
