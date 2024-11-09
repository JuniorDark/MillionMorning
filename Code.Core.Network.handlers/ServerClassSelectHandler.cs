using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerClassSelectHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("class_selected", message);
	}
}
