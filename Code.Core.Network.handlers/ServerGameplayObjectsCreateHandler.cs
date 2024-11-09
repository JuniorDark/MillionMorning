using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerGameplayObjectsCreateHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("level_gameplayobject_create", message);
	}
}
