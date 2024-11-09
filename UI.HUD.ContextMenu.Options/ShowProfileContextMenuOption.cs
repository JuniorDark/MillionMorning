using Core.GameEvent;

namespace UI.HUD.ContextMenu.Options;

public class ShowProfileContextMenuOption : BaseContextMenuOption
{
	public ShowProfileContextMenuOption(ContextMenu context)
		: base(context)
	{
	}

	public override string GetButtonText()
	{
		return "ProfileWindow_6089";
	}

	public override string GetIconKey()
	{
		return "IconProfilesSmall";
	}

	public override void Action()
	{
		int num = int.Parse(Context.RemotePlayerId);
		if (num > 0)
		{
			GameEvent.OpenProfileEvent?.RaiseEvent(num);
		}
	}

	public override bool ShowCondition()
	{
		return !Context.IsLocalPlayer;
	}
}
