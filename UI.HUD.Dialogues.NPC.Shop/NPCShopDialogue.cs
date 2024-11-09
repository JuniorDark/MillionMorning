using UnityEngine;

namespace UI.HUD.Dialogues.NPC.Shop;

public class NPCShopDialogue : NPCDialogueWindow
{
	[Header("Dialogue SO")]
	[SerializeField]
	protected NPCShopDialogueSO shopDialogueSO;

	[Header("Shop")]
	[SerializeField]
	private NPCShop npcShop;

	public override void Init(DialogueSO so)
	{
		shopDialogueSO = (NPCShopDialogueSO)so;
		if (shopDialogueSO == null)
		{
			Debug.LogError(base.name + ": DialogueSO is of wrong type " + so.GetType().Name);
			return;
		}
		base.Init(so);
		if (npcShop != null)
		{
			npcShop.Setup(shopDialogueSO.GetNpcId(), shopDialogueSO.GetInGameShopItems());
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (npcShop == null)
		{
			Debug.LogError(base.name + ": Missing npcShop");
		}
	}

	protected override void RefreshActions()
	{
	}
}
