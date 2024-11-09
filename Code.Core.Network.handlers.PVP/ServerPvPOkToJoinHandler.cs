using Code.Core.EventSystem;

namespace Code.Core.Network.handlers.PVP;

public class ServerPvPOkToJoinHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("pvp_ok_to_join", message);
	}
}
