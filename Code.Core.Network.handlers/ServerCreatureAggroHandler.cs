using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerCreatureAggroHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("creature_aggro", message);
	}
}
