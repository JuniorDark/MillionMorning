using Code.Core.Network;
using Code.Core.Network.messages.client;
using Core;

namespace UI.HUD.ContextMenu.Options;

public class BanPlayerContextMenuOption : BaseContextMenuOption
{
	public BanPlayerContextMenuOption(ContextMenu context)
		: base(context)
	{
	}

	public override string GetButtonText()
	{
		return "Homes_13302";
	}

	public override string GetIconKey()
	{
		return "Kick60";
	}

	public override void Action()
	{
		Singleton<GameNetwork>.Instance.SendToGameServer(new ClientBanPlayerFromHome(Context.RemotePlayerId));
		Context.confirmSound.PlayAudioCue();
	}

	public override bool ShowCondition()
	{
		if (!LocalPlayer.HomeBanList.IsBanned(Context.RemotePlayerId))
		{
			return !Context.IsLocalPlayer;
		}
		return false;
	}
}
