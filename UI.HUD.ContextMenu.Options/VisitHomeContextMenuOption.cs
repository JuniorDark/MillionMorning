using Code.World;
using Code.World.Home;

namespace UI.HUD.ContextMenu.Options;

public class VisitHomeContextMenuOption : BaseContextMenuOption
{
	public VisitHomeContextMenuOption(ContextMenu context)
		: base(context)
	{
	}

	public override string GetButtonText()
	{
		return "Messenger_FriendCard_7865";
	}

	public override string GetIconKey()
	{
		return "IconHomesKey60x60";
	}

	public override void Action()
	{
		if (MilMo_World.Instance.GoToHome(Context.RemotePlayerId, Context.RemotePlayerName))
		{
			Context.confirmSound.PlayAudioCue();
		}
		else
		{
			Context.wrongSound.PlayAudioCue();
		}
	}

	public override bool ShowCondition()
	{
		if (!Context.IsLocalPlayer)
		{
			if (MilMo_Home.CurrentHome != null)
			{
				return MilMo_Home.CurrentHome.OwnerID != Context.RemotePlayerId;
			}
			return true;
		}
		return false;
	}
}
