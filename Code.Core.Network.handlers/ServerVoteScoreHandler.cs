using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerVoteScoreHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("vote_score_received", message);
	}
}
