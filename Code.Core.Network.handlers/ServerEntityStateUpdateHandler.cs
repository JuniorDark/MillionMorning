using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerEntityStateUpdateHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("entity_state_update", message);
	}
}
