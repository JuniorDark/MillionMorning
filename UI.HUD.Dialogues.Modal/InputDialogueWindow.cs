using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UI.HUD.Dialogues.Modal;

[SelectionBase]
public class InputDialogueWindow : ModalDialogueWindow
{
	[Header("Elements")]
	[SerializeField]
	private TMP_InputField input;

	[SerializeField]
	private TMP_Text info;

	[SerializeField]
	private GameObject infoMessageContainer;

	[Header("Dialogue SO")]
	[SerializeField]
	protected InputDialogueSO inputDialogueSO;

	private TMP_Text _placeholder;

	private bool _hasError;

	private UnityAction<string> _onValueChange;

	private UnityAction _onSubmit;

	public override void Init(DialogueSO so)
	{
		inputDialogueSO = (InputDialogueSO)so;
		if (inputDialogueSO == null)
		{
			Debug.LogError(base.name + ": DialogueSO is of wrong type");
			return;
		}
		base.Init(so);
		SetPlaceholderText(inputDialogueSO.GetPlaceholder());
		SetOnValueChangeCallback(inputDialogueSO.GetOnInputChangeAction());
		SetOnSubmitCallback(inputDialogueSO.GetConfirmAction());
		base.OnShow += FocusInput;
	}

	protected override void Awake()
	{
		base.Awake();
		if (info == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing info");
			return;
		}
		info.text = "";
		if (input == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing input");
			return;
		}
		input.text = "";
		_placeholder = input.placeholder.GetComponent<TMP_Text>();
		if (_placeholder == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find placeholder");
		}
		else
		{
			_placeholder.text = "";
		}
	}

	private void Start()
	{
		base.OnShow += DisableSubmit;
	}

	private void DisableSubmit()
	{
		EnableSubmit(shouldEnable: false);
	}

	private void OnDestroy()
	{
		base.OnShow -= DisableSubmit;
	}

	public void SetError(bool hasError)
	{
		_hasError = hasError;
		EnableSubmit(!hasError);
		if (hasError)
		{
			infoMessageContainer.SetActive(value: true);
			return;
		}
		info.text = "";
		infoMessageContainer.SetActive(value: false);
	}

	private void SetPlaceholderText(string newText)
	{
		if (_placeholder != null)
		{
			_placeholder.SetText(newText);
		}
	}

	private void FocusInput()
	{
		if ((bool)input)
		{
			input.ActivateInputField();
		}
	}

	private void SetOnValueChangeCallback(UnityAction<string> callback)
	{
		_onValueChange = callback;
	}

	private void SetOnSubmitCallback(UnityAction callback)
	{
		_onSubmit = callback;
		if (!input)
		{
			Debug.LogWarning(base.gameObject.name + ": Unable to find input");
		}
		else
		{
			input.onSubmit.AddListener(OnSubmit);
		}
	}

	public void OnValueChanged(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			_onValueChange?.Invoke(value);
		}
		else
		{
			EnableSubmit(shouldEnable: false);
		}
	}

	public void OnSelect()
	{
	}

	public void OnDeselect()
	{
	}

	private void EnableSubmit(bool shouldEnable)
	{
		DialogueButton dialogueButton = DialogueButtons.FirstOrDefault((DialogueButton b) => b.HasAction(_onSubmit));
		if (dialogueButton == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Unable to find Submit button...");
		}
		else
		{
			dialogueButton.SetInteractable(shouldEnable);
		}
	}

	private void OnSubmit(string value = "")
	{
		if (!_hasError)
		{
			_onSubmit?.Invoke();
		}
	}
}
