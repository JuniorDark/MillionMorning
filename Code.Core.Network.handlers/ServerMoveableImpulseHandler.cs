using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerMoveableImpulseHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("movable_impulse", message);
	}
}
