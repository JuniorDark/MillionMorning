using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerClassLevelUpHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("player_class_level_update", message);
	}
}
