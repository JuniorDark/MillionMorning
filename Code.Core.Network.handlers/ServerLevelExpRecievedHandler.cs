using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerLevelExpRecievedHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("level_exp_received", message);
	}
}
