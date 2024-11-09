using Code.World.Player;
using Core;
using Player;

namespace UI.HUD.ContextMenu.Options;

public abstract class BaseContextMenuOption
{
	protected readonly ContextMenu Context;

	protected readonly GroupManager GroupManager;

	protected readonly MilMo_Player LocalPlayer;

	protected BaseContextMenuOption(ContextMenu context)
	{
		Context = context;
		GroupManager = Singleton<GroupManager>.Instance;
		LocalPlayer = MilMo_Player.Instance;
	}

	public abstract string GetButtonText();

	public abstract string GetIconKey();

	public abstract void Action();

	public abstract bool ShowCondition();
}
