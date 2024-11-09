using Code.Core.Network.types;
using Code.World.GUI.LoadingScreen;
using Code.World.Player;

namespace Code.World.Gameplay;

public class MilMo_IntraTeleportTemplate : MilMo_TeleportTemplate
{
	public string Room { get; private set; }

	public bool NoPlop { get; private set; }

	private MilMo_IntraTeleportTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "IntraTeleport")
	{
		Room = "";
	}

	public override bool LoadFromNetwork(Template t)
	{
		if (!(t is IntraTeleport intraTeleport))
		{
			return false;
		}
		Room = intraTeleport.GetRoom();
		NoPlop = intraTeleport.GetNoPlop() != 0;
		return base.LoadFromNetwork(t);
	}

	public new static MilMo_IntraTeleportTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_IntraTeleportTemplate(category, path, filePath);
	}

	public override void Activate(MilMo_Player player)
	{
		if (!MilMo_LoadingScreen.Instance.IsLoading)
		{
			if (string.IsNullOrEmpty(Room) || Room == player.Avatar.Room)
			{
				bool fadeToWhite = string.IsNullOrEmpty(Room) && !string.IsNullOrEmpty(player.Avatar.Room);
				MilMo_LoadingScreen.Instance.IntraTeleportFade(2f, base.ArriveSound, fadeToWhite);
			}
			else
			{
				MilMo_LoadingScreen.Instance.LoadRoomFade(12f, base.ArriveSound);
			}
		}
	}
}
