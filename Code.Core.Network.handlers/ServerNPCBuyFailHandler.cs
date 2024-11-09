using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerNPCBuyFailHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("npc_shop_buy_fail", message);
	}
}
