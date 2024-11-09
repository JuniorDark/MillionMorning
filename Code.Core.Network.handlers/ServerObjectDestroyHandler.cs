using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerObjectDestroyHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("object_destroy", message);
	}
}
