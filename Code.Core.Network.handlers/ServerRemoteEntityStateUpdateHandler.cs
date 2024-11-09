using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerRemoteEntityStateUpdateHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("remote_entity_state_update", message);
	}
}
