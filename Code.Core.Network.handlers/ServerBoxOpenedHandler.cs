using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerBoxOpenedHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("box_opened", message);
	}
}
