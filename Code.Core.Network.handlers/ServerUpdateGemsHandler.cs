using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerUpdateGemsHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("update_gems", message);
	}
}
