using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerPlayerLeaveShopHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("player_leave_shop", message);
	}
}
