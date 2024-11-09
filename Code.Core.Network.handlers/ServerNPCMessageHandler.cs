using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerNPCMessageHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("npc_message", message);
	}
}
