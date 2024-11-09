using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerCreatureAttackInstantHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("movable_object_attack_instant", message);
	}
}
