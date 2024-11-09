using Localization;
using UI.HUD.Dialogues;

namespace UI.HUD.QuickInfo;

public class QuickInfoMessageData
{
	private readonly LocalizedStringWithArgument _captionLocString;

	private readonly LocalizedStringWithArgument _messageLocString;

	private readonly DialogueButtonInfo _callToAction;

	private readonly string _iconKey;

	private readonly string _soundKey;

	private readonly int _lifetime;

	public QuickInfoMessageData(LocalizedStringWithArgument caption, LocalizedStringWithArgument message, DialogueButtonInfo callToAction = null, string iconKey = null, int lifetime = 5, string soundKey = null)
	{
		_captionLocString = caption;
		_messageLocString = message;
		_callToAction = callToAction;
		_iconKey = iconKey;
		_lifetime = lifetime;
		_soundKey = soundKey;
	}

	public LocalizedStringWithArgument GetCaption()
	{
		return _captionLocString;
	}

	public LocalizedStringWithArgument GetMessage()
	{
		return _messageLocString;
	}

	public DialogueButtonInfo GetCallToAction()
	{
		return _callToAction;
	}

	public string GetIconKey()
	{
		return _iconKey;
	}

	public string GetSoundKey()
	{
		return _soundKey;
	}

	public int GetLifetime()
	{
		return _lifetime;
	}
}
