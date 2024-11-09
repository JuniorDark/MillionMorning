using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerObjectMoveableCreateHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("object_movable_create", message);
	}
}
