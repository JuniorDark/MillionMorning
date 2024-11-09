using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerNPCSellFailHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("npc_shop_sell_fail", message);
	}
}
