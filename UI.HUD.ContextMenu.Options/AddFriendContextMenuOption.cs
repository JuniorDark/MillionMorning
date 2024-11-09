using Code.Core.BuddyBackend;
using Core;

namespace UI.HUD.ContextMenu.Options;

public class AddFriendContextMenuOption : BaseContextMenuOption
{
	private readonly MilMo_BuddyBackend _backend;

	public AddFriendContextMenuOption(ContextMenu context)
		: base(context)
	{
		_backend = Singleton<MilMo_BuddyBackend>.Instance;
	}

	public override string GetIconKey()
	{
		return "IconAddFriendSmall";
	}

	public override string GetButtonText()
	{
		return "Messenger_FriendMenu_356";
	}

	public override void Action()
	{
		string remotePlayerId = Context.RemotePlayerId;
		if (_backend.IsRequestingFriend(remotePlayerId))
		{
			_backend.SendApproveFriendRequest(remotePlayerId);
		}
		else
		{
			_backend.SendFriendRequest(remotePlayerId);
		}
	}

	public override bool ShowCondition()
	{
		if (!_backend.IsBuddy(Context.RemotePlayerId))
		{
			return !Context.IsLocalPlayer;
		}
		return false;
	}
}
