using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerPlayerDeactivateBadgeHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("hide_badge", message);
	}
}
