using System.Collections.Generic;
using UnityEngine;

namespace Code.World.Level.PVP;

public class MilMo_PVPTeam
{
	public HashSet<string> Players { get; }

	public int Id { get; }

	public Color Color { get; }

	public string Name { get; }

	public int RoundsWon { get; set; }

	public int RoundScore { get; set; }

	public MilMo_PVPTeam(int id, IEnumerable<string> players, Color color, string name)
	{
		Id = id;
		Players = new HashSet<string>(players);
		Color = color;
		Name = name;
		RoundsWon = 0;
		RoundScore = 0;
	}
}
