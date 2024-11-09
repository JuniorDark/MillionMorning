using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerLevelPlayerCountsHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("level_player_counts_info", message);
	}
}
