using UnityEngine;

namespace Code.Core.Emote;

public class MilMo_RandomEmote
{
	public MilMo_Emote Emote { get; private set; }

	public Vector2 RouletteWheelSection { get; set; }

	public float Weight { get; private set; }

	public MilMo_RandomEmote(MilMo_Emote emote, float weight)
	{
		Emote = emote;
		Weight = weight;
		RouletteWheelSection = Vector2.zero;
	}

	public bool IsMyNumber(float number)
	{
		if (number >= RouletteWheelSection.x)
		{
			return number < RouletteWheelSection.y;
		}
		return false;
	}
}
