using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class DebugServerCapsuleVolumeSyncHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("debug_capsule_volume_sync", message);
	}
}
