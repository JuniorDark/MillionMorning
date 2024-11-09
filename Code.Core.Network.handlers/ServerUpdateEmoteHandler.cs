using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerUpdateEmoteHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("update_emote", message);
	}
}