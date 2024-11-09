using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerPlayerActivateBadgeHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("show_badge", message);
	}
}
