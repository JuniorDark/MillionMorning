using System;
using Localization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Window.FriendList;

public class FriendListButton : MonoBehaviour
{
	[SerializeField]
	private TMP_Text buttonText;

	[SerializeField]
	private GameObject buttonObject;

	private Button _button;

	private UnityAction _action;

	private void OnDestroy()
	{
		if (_button != null)
		{
			_button.onClick.RemoveAllListeners();
		}
	}

	public void Init(string localeKey, Action action)
	{
		LocalizedStringWithArgument localizedStringWithArgument = new LocalizedStringWithArgument(localeKey);
		buttonText.text = localizedStringWithArgument.GetMessage();
		_action = action.Invoke;
		_button = buttonObject.GetComponent<Button>();
		_button.onClick.AddListener(_action);
	}

	public void Show()
	{
		buttonObject.SetActive(value: true);
	}

	public void Hide()
	{
		buttonObject.SetActive(value: false);
	}
}
