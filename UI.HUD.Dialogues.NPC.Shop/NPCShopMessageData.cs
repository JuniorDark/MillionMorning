using System.Collections.Generic;
using Code.Core.Network.types;
using Localization;

namespace UI.HUD.Dialogues.NPC.Shop;

public class NPCShopMessageData : NPCMessageData
{
	private readonly List<InGameShopItem> _shopItems;

	public NPCShopMessageData(int npcId, string actorName, string actorPortrait, List<InGameShopItem> shopItems)
		: base(null, npcId, actorName, actorPortrait, "", new List<LocalizedStringWithArgument>())
	{
		_shopItems = shopItems;
	}

	public List<InGameShopItem> GetInGameShopItems()
	{
		return _shopItems;
	}
}
