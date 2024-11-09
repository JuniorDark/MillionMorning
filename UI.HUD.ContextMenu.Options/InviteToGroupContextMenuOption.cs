using System.Linq;
using Code.Core.Avatar;
using Code.Core.BuddyBackend;
using Code.World.Level;
using Code.World.Player;
using Core;

namespace UI.HUD.ContextMenu.Options;

public class InviteToGroupContextMenuOption : BaseContextMenuOption
{
	private readonly MilMo_BuddyBackend _backend;

	public InviteToGroupContextMenuOption(ContextMenu context)
		: base(context)
	{
		_backend = Singleton<MilMo_BuddyBackend>.Instance;
	}

	public override string GetButtonText()
	{
		return "Messenger_FriendList_10117";
	}

	public override string GetIconKey()
	{
		return "GroupIcon";
	}

	public override void Action()
	{
		GroupManager.InviteToGroup(Context.RemotePlayerId, Context.RemotePlayerName);
	}

	public override bool ShowCondition()
	{
		if (!MilMo_Player.InHome && MilMo_Instance.CurrentInstance.Avatars.Any((MilMo_Avatar avatar) => avatar.Id == Context.RemotePlayerId))
		{
			if (GroupManager.PlayerIsInGroup)
			{
				return !GroupManager.InGroup(Context.RemotePlayerId);
			}
			return true;
		}
		return false;
	}
}
