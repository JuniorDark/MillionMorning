using System.Collections.Generic;
using Code.Core.Network.types;
using Code.World.Inventory;
using Code.World.Level.LevelInfo;
using Code.World.Player;
using Core.State;
using Localization;

namespace UI.HUD.Dialogues.NPC.Travel;

public class NPCTravelMessageData : NPCMessageData
{
	private readonly IList<NpcLevelOffer> _levelOffers;

	private List<TravelInfo> _travelLocations = new List<TravelInfo>();

	public NPCTravelMessageData(NpcMessageTypes messageType, int npcId, string actorName, string portraitKey, string voicePath, List<LocalizedStringWithArgument> messages, IList<NpcLevelOffer> levelOffers)
		: base(messageType, npcId, actorName, portraitKey, voicePath, messages)
	{
		_levelOffers = levelOffers;
		SetupTravelLocations();
	}

	private void SetupTravelLocations()
	{
		List<TravelInfo> list = new List<TravelInfo>();
		for (int i = 0; i < _levelOffers.Count; i++)
		{
			NpcLevelOffer npcLevelOffer = _levelOffers[i];
			int levelIndex = i;
			string fullLevelName = npcLevelOffer.GetFullLevelName();
			int priceInGems = npcLevelOffer.GetPriceInGems();
			sbyte priceInHelicopterTickets = npcLevelOffer.GetPriceInHelicopterTickets();
			string travelSound = npcLevelOffer.GetTravelSound();
			string arriveSound = npcLevelOffer.GetArriveSound();
			MilMo_LevelInfoData levelInfoData = MilMo_LevelInfo.GetLevelInfoData(fullLevelName);
			string identifier = ((_levelOffers.Count > 1) ? levelInfoData.DisplayName.Identifier : "World_417");
			TravelInfo travelInfo = new TravelInfo
			{
				LevelIndex = levelIndex,
				LocationIsMembersOnly = levelInfoData.IsMembersOnlyArea,
				RequiredAvatarLevel = levelInfoData.RequiredAvatarLevel,
				PriceInGems = priceInGems,
				PriceInHelicopterTickets = priceInHelicopterTickets,
				TravelSound = travelSound,
				ArriveSound = arriveSound,
				World = levelInfoData.World,
				Level = levelInfoData.Level,
				DisplayName = levelInfoData.DisplayName.Identifier
			};
			travelInfo.SetLabel(new LocalizedStringWithArgument(identifier));
			bool flag = MilMo_Player.Instance?.IsMember ?? false;
			travelInfo.NeedMembership = travelInfo.LocationIsMembersOnly && !flag;
			travelInfo.EnoughGems = GlobalStates.Instance.playerState.gems.Get() >= priceInGems;
			travelInfo.ToLowLevel = MilMo_Player.Instance.AvatarLevel < travelInfo.RequiredAvatarLevel;
			if (travelInfo.PriceInHelicopterTickets > 0)
			{
				MilMo_InventoryEntry entry = MilMo_Player.Instance.Inventory.GetEntry("Shop:Batch01.Items.HeliTicket");
				travelInfo.EnoughTickets = entry != null && entry.Amount >= priceInHelicopterTickets;
			}
			list.Add(travelInfo);
		}
		_travelLocations = list;
	}

	public List<TravelInfo> GetTravelLocations()
	{
		return _travelLocations;
	}
}
