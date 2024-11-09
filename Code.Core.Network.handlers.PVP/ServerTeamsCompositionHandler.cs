using Code.Core.EventSystem;

namespace Code.Core.Network.handlers.PVP;

public class ServerTeamsCompositionHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("team_composition", message);
	}
}
