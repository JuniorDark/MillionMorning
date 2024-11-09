using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server.PVP;

public class NetworkTeam
{
	private int id;

	private IList<string> players;

	private color teamColor;

	private string teamName;

	public NetworkTeam(MessageReader reader)
	{
		id = reader.ReadInt32();
		players = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			players.Add(reader.ReadString());
		}
		teamColor = new color(reader);
		teamName = reader.ReadString();
	}

	public NetworkTeam(IList<string> players, color teamColor, string teamName)
	{
		this.players = players;
		this.teamColor = teamColor;
		this.teamName = teamName;
	}

	public int GetId()
	{
		return id;
	}

	public IList<string> GetPlayers()
	{
		return players;
	}

	public color GetColor()
	{
		return teamColor;
	}

	public string GetName()
	{
		return teamName;
	}

	public int size()
	{
		int num = 8;
		num += (short)(2 * players.Count);
		foreach (string player in players)
		{
			num += MessageWriter.GetSize(player);
		}
		num += teamColor.size();
		return num + MessageWriter.GetSize(teamName);
	}

	public void write(MessageWriter writer)
	{
		writer.WriteInt32(id);
		writer.WriteInt16((short)players.Count);
		foreach (string player in players)
		{
			writer.WriteString(player);
		}
		teamColor.write(writer);
		writer.WriteString(teamName);
	}
}
