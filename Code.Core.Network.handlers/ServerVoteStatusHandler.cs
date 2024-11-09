using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerVoteStatusHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("vote_info_received", message);
	}
}
