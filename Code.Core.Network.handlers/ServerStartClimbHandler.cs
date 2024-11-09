using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerStartClimbHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("start_climb", message);
	}
}
