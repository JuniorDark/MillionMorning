using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerNPCCreateHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("level_npc_create", message);
	}
}