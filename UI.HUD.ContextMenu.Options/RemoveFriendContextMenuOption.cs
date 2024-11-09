using Code.Core.BuddyBackend;
using Core;
using UI.HUD.Dialogues;

namespace UI.HUD.ContextMenu.Options;

public class RemoveFriendContextMenuOption : BaseContextMenuOption
{
	private readonly MilMo_BuddyBackend _backend;

	public RemoveFriendContextMenuOption(ContextMenu context)
		: base(context)
	{
		_backend = Singleton<MilMo_BuddyBackend>.Instance;
	}

	public override string GetButtonText()
	{
		return "Messenger_FriendMenu_358";
	}

	public override string GetIconKey()
	{
		return "IconIgnoreSmall";
	}

	public override void Action()
	{
		DialogueSpawner.OpenRemoveFriendDialogue(Context.RemotePlayerName, Context.RemotePlayerId);
	}

	public override bool ShowCondition()
	{
		return _backend.IsBuddy(Context.RemotePlayerId);
	}
}
