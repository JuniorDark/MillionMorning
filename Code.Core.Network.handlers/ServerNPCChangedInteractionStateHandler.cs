using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerNPCChangedInteractionStateHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("npc_change_interaction_state", message);
	}
}
