using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerLastActionTimeUpdatedHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("update_action_time", message);
	}
}
