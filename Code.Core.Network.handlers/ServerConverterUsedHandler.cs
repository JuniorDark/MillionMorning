using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerConverterUsedHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("converter_used", message);
	}
}
