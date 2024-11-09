using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerSpawnVisualRepAtHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("visual_rep_spawned", message);
	}
}
