using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerPlayAnimationHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("play_animation", message);
	}
}
