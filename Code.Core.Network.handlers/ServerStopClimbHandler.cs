using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerStopClimbHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("stop_climb", message);
	}
}
