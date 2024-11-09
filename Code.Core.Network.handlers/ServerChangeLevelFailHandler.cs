using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerChangeLevelFailHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("change_level_fail");
	}
}
