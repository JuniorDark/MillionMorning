using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerJuneCashUpdateHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("junecash_updated", message);
	}
}
