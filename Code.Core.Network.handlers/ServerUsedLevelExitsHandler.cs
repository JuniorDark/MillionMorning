using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerUsedLevelExitsHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("used_level_exits_info", message);
	}
}
