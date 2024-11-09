using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerCharbuilderDataHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("charbuilder_data", message);
	}
}
