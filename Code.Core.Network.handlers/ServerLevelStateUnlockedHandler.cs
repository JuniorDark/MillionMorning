using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerLevelStateUnlockedHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("level_state_unlocked", message);
	}
}
