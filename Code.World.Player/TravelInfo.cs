using System;
using UI.HUD.Dialogues;

namespace Code.World.Player;

[Serializable]
public class TravelInfo : DialogueButtonInfo
{
	public int PriceInGems;

	public sbyte PriceInHelicopterTickets;

	public bool LocationIsMembersOnly;

	public int RequiredAvatarLevel;

	public int LevelIndex;

	public string World;

	public string Level;

	public string DisplayName;

	public string TravelSound;

	public string ArriveSound;

	public bool EnoughTickets = true;

	public bool ToLowLevel;

	public bool EnoughGems = true;

	public bool NeedMembership;

	public bool HasLimits
	{
		get
		{
			if (PriceInGems <= 0 && !LocationIsMembersOnly)
			{
				return PriceInHelicopterTickets > 0;
			}
			return true;
		}
	}

	public TravelInfo()
		: base(null, null)
	{
	}
}
