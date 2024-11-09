using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerLadderHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("pvp_ladder_received", message);
	}
}
