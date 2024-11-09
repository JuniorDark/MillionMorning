using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerUpdateKillsDeathsHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("update_killsdeaths", message);
	}
}
