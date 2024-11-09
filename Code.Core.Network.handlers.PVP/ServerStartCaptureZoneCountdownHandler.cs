using Code.Core.EventSystem;

namespace Code.Core.Network.handlers.PVP;

public class ServerStartCaptureZoneCountdownHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("start_capture_zone_countdown", message);
	}
}
