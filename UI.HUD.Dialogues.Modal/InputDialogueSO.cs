using Localization;
using UnityEngine;
using UnityEngine.Events;

namespace UI.HUD.Dialogues.Modal;

[CreateAssetMenu(menuName = "Dialogues/Input", fileName = "InputDialogueSO", order = 0)]
public class InputDialogueSO : ModalDialogueSO
{
	[SerializeField]
	protected LocalizedStringWithArgument placeholder;

	[SerializeField]
	protected UnityEvent<string> onInputChange = new UnityEvent<string>();

	public override string GetAddressableKey()
	{
		return "InputDialogueWindow";
	}

	public override int GetPriority()
	{
		return 9;
	}

	public override void Init(ModalMessageData modalMessageData)
	{
		if (modalMessageData is InputModalMessageData inputModalMessageData)
		{
			base.Init(modalMessageData);
			placeholder = inputModalMessageData.GetPlaceholder();
			UnityAction<string> onInputChangeAction = inputModalMessageData.GetOnInputChangeAction();
			if (onInputChangeAction != null)
			{
				onInputChange.AddListener(onInputChangeAction);
			}
		}
	}

	public string GetPlaceholder()
	{
		return placeholder.GetMessage();
	}

	public UnityAction<string> GetOnInputChangeAction()
	{
		return onInputChange.Invoke;
	}

	public UnityAction GetConfirmAction()
	{
		return base.Confirm;
	}
}
