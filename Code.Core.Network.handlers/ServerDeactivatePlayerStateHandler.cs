using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerDeactivatePlayerStateHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("playerstate_deactivated", message);
	}
}
