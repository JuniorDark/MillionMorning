using Localization;
using UI.Sprites;
using UnityEngine.Events;

namespace UI.HUD.Dialogues.Modal;

public class InputModalMessageData : ModalMessageData
{
	private readonly LocalizedStringWithArgument _placeholder;

	private readonly UnityAction<string> _onInputChange;

	public InputModalMessageData(LocalizedStringWithArgument caption, LocalizedStringWithArgument placeholder, UnityAction<string> onInputChange, DialogueButtonInfo confirm, DialogueButtonInfo cancel, IHaveSprite spriteReference = null)
		: base(caption, null, confirm, cancel, null, spriteReference)
	{
		_placeholder = placeholder;
		_onInputChange = onInputChange;
	}

	public LocalizedStringWithArgument GetPlaceholder()
	{
		return _placeholder;
	}

	public UnityAction<string> GetOnInputChangeAction()
	{
		return _onInputChange;
	}
}
