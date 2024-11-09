using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerNPCBuySuccessHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("npc_shop_buy_success", message);
	}
}
