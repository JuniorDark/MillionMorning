using Code.Core.Network;
using Code.Core.Network.messages.client;
using Core;

namespace UI.HUD.ContextMenu.Options;

public class UnbanPlayerContextMenuOption : BaseContextMenuOption
{
	public UnbanPlayerContextMenuOption(ContextMenu context)
		: base(context)
	{
	}

	public override string GetButtonText()
	{
		return "Homes_13303";
	}

	public override string GetIconKey()
	{
		return "Kick60";
	}

	public override void Action()
	{
		Singleton<GameNetwork>.Instance.SendToGameServer(new ClientUnBanPlayerFromHome(Context.RemotePlayerId));
		Context.confirmSound.PlayAudioCue();
	}

	public override bool ShowCondition()
	{
		return LocalPlayer.HomeBanList.IsBanned(Context.RemotePlayerId);
	}
}
