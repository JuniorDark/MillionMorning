using Code.Core.EventSystem;

namespace Code.Core.Network.handlers.PVP;

public class ServerStopCaptureZoneCountdownHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("stop_capture_zone_countdown", message);
	}
}
