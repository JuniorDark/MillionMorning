using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerInitialSettingsHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("initial_settings_received", message);
	}
}
