using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerStaticGemFoundHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("static_gem_found", message);
	}
}
