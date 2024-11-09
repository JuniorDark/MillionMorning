using System.Collections.Generic;
using Code.Core.Network.types;

namespace UI.HUD.Dialogues.NPC.Shop;

public class NPCShopDialogueSO : NPCDialogueSO
{
	private int _npcId = -1;

	private List<InGameShopItem> _inGameShopItems;

	public override string GetAddressableKey()
	{
		return "NPCShopDialogue";
	}

	public override void Init(NPCMessageData npcMessageData)
	{
		if (npcMessageData is NPCShopMessageData nPCShopMessageData)
		{
			base.Init(npcMessageData);
			_npcId = npcMessageData.GetNpcId();
			_inGameShopItems = nPCShopMessageData.GetInGameShopItems();
		}
	}

	public int GetNpcId()
	{
		return _npcId;
	}

	public List<InGameShopItem> GetInGameShopItems()
	{
		return _inGameShopItems;
	}
}
