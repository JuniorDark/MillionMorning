using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerUseConverterFailedHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("use_converter_failed", message);
	}
}
