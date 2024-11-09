using Code.Core.Network;
using Code.Core.Network.messages.client;
using Core;

namespace UI.HUD.ContextMenu.Options;

public class GiveLeaderContextMenuOption : BaseContextMenuOption
{
	public GiveLeaderContextMenuOption(ContextMenu context)
		: base(context)
	{
	}

	public override string GetButtonText()
	{
		return "Messenger_FriendList_10246";
	}

	public override string GetIconKey()
	{
		return "GroupIcon";
	}

	public override void Action()
	{
		Singleton<GameNetwork>.Instance.SendToGameServer(new ClientGroupPromote(Context.RemotePlayerId));
	}

	public override bool ShowCondition()
	{
		if (GroupManager.LocalPlayerIsLeader() && !Context.IsLocalPlayer)
		{
			return GroupManager.InGroup(Context.RemotePlayerId);
		}
		return false;
	}
}
