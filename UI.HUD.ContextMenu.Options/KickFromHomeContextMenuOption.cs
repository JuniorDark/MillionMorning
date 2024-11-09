using System.Linq;
using Code.Core.Avatar;
using Code.Core.Network;
using Code.Core.Network.messages.client;
using Code.World.Home;
using Core;

namespace UI.HUD.ContextMenu.Options;

public class KickFromHomeContextMenuOption : BaseContextMenuOption
{
	public KickFromHomeContextMenuOption(ContextMenu context)
		: base(context)
	{
	}

	public override string GetButtonText()
	{
		return "Homes_6804";
	}

	public override string GetIconKey()
	{
		return "Kick60";
	}

	public override void Action()
	{
		Singleton<GameNetwork>.Instance.SendToGameServer(new ClientKickPlayerFromHome(Context.RemotePlayerId));
		Context.confirmSound.PlayAudioCue();
	}

	public override bool ShowCondition()
	{
		if (!Context.IsLocalPlayer && MilMo_Home.CurrentHome != null && MilMo_Home.CurrentHome.OwnerID == LocalPlayer.Id)
		{
			return MilMo_Home.CurrentHome.Avatars.Any((MilMo_Avatar avatar) => avatar.Id == Context.RemotePlayerId);
		}
		return false;
	}
}
