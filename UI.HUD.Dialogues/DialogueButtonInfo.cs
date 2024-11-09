using Localization;
using UnityEngine.Events;

namespace UI.HUD.Dialogues;

public class DialogueButtonInfo
{
	private UnityAction _action;

	private LocalizedStringWithArgument _label;

	private readonly bool _isDefault;

	public DialogueButtonInfo(UnityAction action, LocalizedStringWithArgument label, bool isDefault = false)
	{
		_action = action;
		_label = label;
		_isDefault = isDefault;
	}

	public UnityAction GetAction()
	{
		return _action;
	}

	public void SetAction(UnityAction action)
	{
		_action = action;
	}

	public LocalizedStringWithArgument GetLabel()
	{
		return _label;
	}

	public string GetLabelText()
	{
		return _label.GetMessage();
	}

	public void SetLabel(LocalizedStringWithArgument label)
	{
		_label = label;
	}

	public bool IsDefault()
	{
		return _isDefault;
	}
}
