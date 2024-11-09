using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerCollectablesHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("collectables", message);
	}
}
