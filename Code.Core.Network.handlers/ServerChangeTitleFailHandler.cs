using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerChangeTitleFailHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("player_title_change_failed", message);
	}
}
