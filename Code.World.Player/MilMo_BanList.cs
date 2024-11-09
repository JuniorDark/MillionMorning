using System.Collections.Generic;

namespace Code.World.Player;

public class MilMo_BanList
{
	private readonly List<string> _bannedPlayers;

	public MilMo_BanList(IEnumerable<string> bannedIds)
	{
		_bannedPlayers = new List<string>(bannedIds);
	}

	public void SetBanned(string id)
	{
		if (!IsBanned(id))
		{
			_bannedPlayers.Add(id);
		}
	}

	public bool IsBanned(string id)
	{
		return _bannedPlayers.Contains(id);
	}

	public void SetUnbanned(string id)
	{
		if (IsBanned(id))
		{
			_bannedPlayers.Remove(id);
		}
	}
}
