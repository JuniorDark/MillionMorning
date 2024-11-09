using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerThumbnailAvatarInfoHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("thumbnail_avatar_info", message);
	}
}
