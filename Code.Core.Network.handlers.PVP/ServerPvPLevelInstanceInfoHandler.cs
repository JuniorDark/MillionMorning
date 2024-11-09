using Code.Core.EventSystem;

namespace Code.Core.Network.handlers.PVP;

public class ServerPvPLevelInstanceInfoHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("got_load_pvp_level_info", message);
	}
}
