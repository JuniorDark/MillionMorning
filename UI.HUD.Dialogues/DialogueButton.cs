using System.Collections.Generic;
using TMPro;
using UI.FX;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.HUD.Dialogues;

public class DialogueButton : MonoBehaviour
{
	[SerializeField]
	private Button button;

	[SerializeField]
	private TMP_Text label;

	[SerializeField]
	private bool isDefault;

	protected UIAlphaFX Fader;

	private readonly List<UnityAction> _actions = new List<UnityAction>();

	public virtual void Init(DialogueButtonInfo dialogueButtonInfo)
	{
		SetLabel(dialogueButtonInfo.GetLabelText());
		AddOnClick(dialogueButtonInfo.GetAction());
		if (dialogueButtonInfo.IsDefault())
		{
			SetIsDefault();
		}
		if ((bool)Fader)
		{
			Fader.FadeOutFast();
		}
	}

	protected virtual void Awake()
	{
		if (button == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing button");
			return;
		}
		if (label == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing label");
			return;
		}
		Fader = GetComponent<UIAlphaFX>();
		if (!Fader)
		{
			Debug.LogWarning(base.name + ": Unable to find UIAlphaFX");
		}
	}

	protected void SetLabel(string value)
	{
		if (!(label == null))
		{
			label.text = value;
		}
	}

	protected void AddOnClick(UnityAction action)
	{
		if (!(button == null))
		{
			button.onClick.AddListener(action);
			_actions.Add(action);
		}
	}

	public bool GetIsDefault()
	{
		return isDefault;
	}

	private void SetIsDefault()
	{
		isDefault = true;
	}

	public void Enable(bool shouldEnable)
	{
		if (button == null)
		{
			return;
		}
		if ((bool)Fader)
		{
			if (shouldEnable)
			{
				Fader.FadeIn();
			}
			else
			{
				Fader.FadeOut();
			}
		}
		SetInteractable(shouldEnable);
		if (isDefault && shouldEnable)
		{
			EventSystem.current.SetSelectedGameObject(button.gameObject);
		}
	}

	public void SetInteractable(bool value)
	{
		if (!(button == null))
		{
			button.interactable = value;
		}
	}

	public bool HasAction(UnityAction anotherAction)
	{
		return _actions.Contains(anotherAction);
	}
}
