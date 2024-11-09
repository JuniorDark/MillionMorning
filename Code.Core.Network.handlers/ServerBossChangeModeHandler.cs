using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerBossChangeModeHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("boss_change_mode", message);
	}
}
