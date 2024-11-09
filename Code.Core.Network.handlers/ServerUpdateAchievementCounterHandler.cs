using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerUpdateAchievementCounterHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("achievement_counter_update", message);
	}
}
