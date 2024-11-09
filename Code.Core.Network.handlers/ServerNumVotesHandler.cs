using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerNumVotesHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("vote_numvotes_received", message);
	}
}
