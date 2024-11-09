using System.Linq;
using Code.Core.Avatar;
using Code.Core.BuddyBackend;
using Code.World.Level;
using Core;

namespace UI.HUD.ContextMenu.Options;

public class TeleportToFriendContextMenuOption : BaseContextMenuOption
{
	private readonly MilMo_BuddyBackend _backend;

	public TeleportToFriendContextMenuOption(ContextMenu context)
		: base(context)
	{
		_backend = Singleton<MilMo_BuddyBackend>.Instance;
	}

	public override string GetButtonText()
	{
		return "Messenger_FriendCard_6249";
	}

	public override string GetIconKey()
	{
		return "IconTeleportStoneSmall";
	}

	public override void Action()
	{
		if (ShowCondition())
		{
			LocalPlayer.RequestTeleportToFriend(Context.RemotePlayerId);
		}
	}

	public override bool ShowCondition()
	{
		if (!GroupManager.PlayerIsInGroup && LocalPlayer.OkToTeleport() && _backend.IsFriendConnected(Context.RemotePlayerId))
		{
			return MilMo_Instance.CurrentInstance.Avatars.All((MilMo_Avatar avatar) => avatar.Id != Context.RemotePlayerId);
		}
		return false;
	}
}
