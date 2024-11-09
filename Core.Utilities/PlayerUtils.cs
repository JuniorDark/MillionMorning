using Code.World.Level;
using Code.World.Player;
using UnityEngine;

namespace Core.Utilities;

public static class PlayerUtils
{
	public static bool FindPlayer(string id, out IPlayer player)
	{
		if (id == MilMo_Player.Instance.Id)
		{
			player = MilMo_Player.Instance;
		}
		else
		{
			player = MilMo_Instance.CurrentInstance?.GetRemotePlayer(id);
		}
		return player != null;
	}

	public static bool IsLocalPlayer(GameObject other)
	{
		return MilMo_Player.Instance.Avatar.GameObject.Equals(other);
	}
}
