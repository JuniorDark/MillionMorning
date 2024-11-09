using Code.Core.Network;
using Code.Core.Network.messages.client;
using Core;

namespace UI.HUD.ContextMenu.Options;

public class LeaveGroupContextMenuOption : BaseContextMenuOption
{
	public LeaveGroupContextMenuOption(ContextMenu context)
		: base(context)
	{
	}

	public override string GetButtonText()
	{
		return "Messenger_FriendList_10120";
	}

	public override string GetIconKey()
	{
		return "GroupIcon";
	}

	public override void Action()
	{
		Singleton<GameNetwork>.Instance.SendToGameServer(new ClientGroupLeave());
	}

	public override bool ShowCondition()
	{
		if (GroupManager.PlayerIsInGroup)
		{
			return Context.IsLocalPlayer;
		}
		return false;
	}
}
