using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerCreatureBeginAttackHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("movable_object_begin_attack", message);
	}
}
