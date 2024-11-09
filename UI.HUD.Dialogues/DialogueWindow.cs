using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Input;
using Core.Utilities;
using JetBrains.Annotations;
using UI.Contracts;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.HUD.Dialogues;

public abstract class DialogueWindow : MonoBehaviour, IShowableAndCloseable, IShowable, ICloseable, IHasPriorityKey
{
	private DialogueSO _dialogueSO;

	[Header("Actions")]
	[SerializeField]
	[CanBeNull]
	protected AssetReference buttonPrefab;

	[SerializeField]
	[CanBeNull]
	protected Transform buttonContainer;

	[SerializeField]
	[CanBeNull]
	protected Button closeButton;

	protected readonly List<DialogueButton> DialogueButtons = new List<DialogueButton>();

	private bool _isClosing;

	public event UnityAction OnShow = delegate
	{
	};

	public event UnityAction OnClose = delegate
	{
	};

	public virtual void Init(DialogueSO so)
	{
		_dialogueSO = so;
		_dialogueSO.SetWindow(this);
	}

	public virtual void Show()
	{
		base.gameObject.SetActive(value: true);
		ShowCloseButton(shouldShow: true);
		this.OnShow?.Invoke();
	}

	public virtual void Close()
	{
		if (!_isClosing)
		{
			_isClosing = true;
			this.OnClose?.Invoke();
			Singleton<DialogueManager>.Instance.Close(this);
			if ((bool)base.gameObject)
			{
				Object.Destroy(base.gameObject, 1f);
			}
		}
	}

	public bool CanBeShown()
	{
		return _dialogueSO.CanBeShown();
	}

	protected void LockController()
	{
		Singleton<InputController>.Instance.SetDialogueController();
	}

	protected void ReleaseController()
	{
		Singleton<InputController>.Instance.RestorePreviousController();
	}

	protected virtual void RefreshActions()
	{
		if (!(_dialogueSO == null))
		{
			SetButtons(_dialogueSO.GetActions());
		}
	}

	protected void DefaultChoice()
	{
		_dialogueSO.GetActions().FirstOrDefault((DialogueButtonInfo a) => a.IsDefault())?.GetAction()();
	}

	protected void SetButtons(List<DialogueButtonInfo> dialogueButtonInfos)
	{
		ClearButtons();
		foreach (DialogueButtonInfo dialogueButtonInfo in dialogueButtonInfos)
		{
			AddButton(dialogueButtonInfo);
		}
	}

	protected void AddButton(DialogueButtonInfo dialogueButtonInfo)
	{
		if (buttonPrefab != null && !buttonPrefab.RuntimeKeyIsValid())
		{
			Debug.LogError(base.name + ": Missing buttonPrefab");
			return;
		}
		if (buttonContainer == null)
		{
			Debug.LogError(base.name + ": Missing buttonContainer");
			return;
		}
		DialogueButton dialogueButton = Instantiator.Instantiate<DialogueButton>(buttonPrefab, buttonContainer);
		if (!(dialogueButton == null))
		{
			dialogueButton.Init(dialogueButtonInfo);
			DialogueButtons.Add(dialogueButton);
		}
	}

	protected void EnableButtons(bool shouldEnable)
	{
		foreach (DialogueButton dialogueButton in DialogueButtons)
		{
			dialogueButton.Enable(shouldEnable);
		}
	}

	protected void ShowCloseButton(bool shouldShow)
	{
		if (!(closeButton == null))
		{
			closeButton.gameObject.SetActive(shouldShow);
		}
	}

	protected void ClearButtons()
	{
		if (buttonContainer == null)
		{
			return;
		}
		for (int i = 0; i < buttonContainer.childCount; i++)
		{
			Transform child = buttonContainer.GetChild(i);
			if ((bool)child)
			{
				Object.Destroy(child.gameObject);
			}
		}
		DialogueButtons.Clear();
	}

	public int GetPriority()
	{
		return _dialogueSO.GetPriority();
	}

	public bool Equals(IHasPriorityKey other)
	{
		if (other is DialogueWindow dialogueWindow && _dialogueSO != null && dialogueWindow._dialogueSO != null)
		{
			return dialogueWindow._dialogueSO.Equals(_dialogueSO);
		}
		return false;
	}

	public void SetUser(IDialogueUser dialogueUser)
	{
		_dialogueSO.SetUser(dialogueUser);
	}
}
