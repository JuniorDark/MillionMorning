using Localization;
using UI.Sprites;
using UnityEngine.Events;

namespace UI.HUD.Dialogues.Modal;

public class ModalMessageData
{
	private readonly LocalizedStringWithArgument _captionLocString;

	private readonly LocalizedStringWithArgument _messageLocString;

	private readonly int _lifetime;

	private readonly IHaveSprite _spriteReference;

	private readonly DialogueButtonInfo _confirm;

	private readonly DialogueButtonInfo _cancel;

	private readonly DialogueButtonInfo _alternate;

	public ModalMessageData(LocalizedStringWithArgument caption, LocalizedStringWithArgument message, DialogueButtonInfo confirm, DialogueButtonInfo cancel, DialogueButtonInfo alternate, IHaveSprite spriteReference = null, int lifetime = 0)
	{
		_captionLocString = caption;
		_messageLocString = message;
		_confirm = confirm;
		_cancel = cancel;
		_alternate = alternate;
		_spriteReference = spriteReference;
		_lifetime = lifetime;
	}

	public LocalizedStringWithArgument GetCaption()
	{
		return _captionLocString;
	}

	public LocalizedStringWithArgument GetMessage()
	{
		return _messageLocString;
	}

	public UnityAction GetOnConfirm()
	{
		return _confirm?.GetAction();
	}

	public LocalizedStringWithArgument GetConfirmLabel()
	{
		return _confirm?.GetLabel();
	}

	public bool IsConfirmDefault()
	{
		return _confirm?.IsDefault() ?? false;
	}

	public UnityAction GetOnCancel()
	{
		return _cancel?.GetAction();
	}

	public LocalizedStringWithArgument GetCancelLabel()
	{
		return _cancel?.GetLabel();
	}

	public bool IsCancelDefault()
	{
		return _cancel?.IsDefault() ?? false;
	}

	public UnityAction GetOnAlternate()
	{
		return _alternate?.GetAction();
	}

	public LocalizedStringWithArgument GetAlternateLabel()
	{
		return _alternate?.GetLabel();
	}

	public bool IsAlternateDefault()
	{
		return _alternate?.IsDefault() ?? false;
	}

	public IHaveSprite GetSpriteReference()
	{
		return _spriteReference;
	}

	public int GetLifetime()
	{
		return _lifetime;
	}
}
