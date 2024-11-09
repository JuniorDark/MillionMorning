using Code.Core.BuddyBackend;
using Core;
using UI.Window.FriendList;

namespace UI.HUD.ContextMenu.Options;

public class SendMessageContextMenuOption : BaseContextMenuOption
{
	private readonly MilMo_BuddyBackend _backend;

	public SendMessageContextMenuOption(ContextMenu context)
		: base(context)
	{
		_backend = Singleton<MilMo_BuddyBackend>.Instance;
	}

	public override string GetButtonText()
	{
		return "Messenger_FriendMenu_357";
	}

	public override string GetIconKey()
	{
		return "IconMessageSmall";
	}

	public override void Action()
	{
		InstantMessage.OpenSendIMDialog(Context.RemotePlayerId, Context.RemotePlayerName);
	}

	public override bool ShowCondition()
	{
		return _backend.IsBuddy(Context.RemotePlayerId);
	}
}
