using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerLocalPlayerInfoHandler : IHandler
{
	public const sbyte MEMBER = 1;

	public const sbyte NOT_MEMBER = 0;

	public const sbyte NOT_MEMBER_JUST_EXPIRED = -1;

	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("player_info", message);
	}
}
