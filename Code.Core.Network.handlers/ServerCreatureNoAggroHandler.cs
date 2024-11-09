using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerCreatureNoAggroHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("creature_no_aggro", message);
	}
}
