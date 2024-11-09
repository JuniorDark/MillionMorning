using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerNPCSellSuccessHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("npc_shop_sell_success", message);
	}
}
