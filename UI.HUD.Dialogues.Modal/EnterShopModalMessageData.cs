using Localization;
using UI.Sprites;

namespace UI.HUD.Dialogues.Modal;

public class EnterShopModalMessageData : ModalMessageData
{
	private readonly string _itemToSelect;

	public EnterShopModalMessageData(LocalizedStringWithArgument caption, LocalizedStringWithArgument message, string itemToSelect, IHaveSprite spriteReference)
		: base(caption, message, new DialogueButtonInfo(null, new LocalizedStringWithArgument("Generic_Yes"), isDefault: true), new DialogueButtonInfo(null, new LocalizedStringWithArgument("Generic_Later")), null, spriteReference)
	{
		_itemToSelect = itemToSelect;
	}

	public string GetItemToSelect()
	{
		return _itemToSelect;
	}
}
