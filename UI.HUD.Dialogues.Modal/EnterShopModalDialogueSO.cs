using Code.World.CharacterShop;
using Code.World.Player;
using UnityEngine;

namespace UI.HUD.Dialogues.Modal;

public class EnterShopModalDialogueSO : ModalDialogueSO
{
	[SerializeField]
	private string itemToSelect;

	public override void Init(ModalMessageData modalMessageData)
	{
		if (modalMessageData is EnterShopModalMessageData enterShopModalMessageData)
		{
			base.Init(modalMessageData);
			itemToSelect = enterShopModalMessageData.GetItemToSelect();
			onConfirm.AddListener(EnterShop);
		}
	}

	private void EnterShop()
	{
		if (!string.IsNullOrEmpty(itemToSelect))
		{
			MilMo_CharacterShop.SelectItem(itemToSelect);
		}
		MilMo_Player.Instance.RequestEnterShop();
	}
}
