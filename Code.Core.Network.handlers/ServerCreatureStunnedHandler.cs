using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerCreatureStunnedHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("movable_stun", message);
	}
}
