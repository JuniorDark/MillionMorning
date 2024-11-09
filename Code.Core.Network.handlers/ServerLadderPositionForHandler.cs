using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerLadderPositionForHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("pvp_rank_received", message);
	}
}
