using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace UI.Elements.PrevNext;

public class PrevNext : MonoBehaviour
{
	[SerializeField]
	private Button previous;

	[SerializeField]
	private Button next;

	[SerializeField]
	private TMP_Text text;

	[SerializeField]
	private PrevNextBaseHandler handler;

	[SerializeField]
	private List<PrevNextOption> options;

	private int _index;

	public void Init(List<PrevNextOption> initial)
	{
		options = new List<PrevNextOption>();
	}

	private void Start()
	{
		if (handler == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find handler");
			return;
		}
		_index = 0;
		StartListeners();
		UpdateText();
	}

	private void OnDestroy()
	{
		StopListeners();
	}

	private void StartListeners()
	{
		previous.onClick.AddListener(Previous);
		next.onClick.AddListener(Next);
		LocalizationSettings.SelectedLocaleChanged += LocaleChanged;
	}

	private void StopListeners()
	{
		previous.onClick.RemoveAllListeners();
		next.onClick.RemoveAllListeners();
		LocalizationSettings.SelectedLocaleChanged -= LocaleChanged;
	}

	private void Previous()
	{
		if (_index <= 0)
		{
			_index = options.Count - 1;
		}
		else
		{
			_index--;
		}
		UpdateText();
		HandleChange();
	}

	private void Next()
	{
		if (_index >= options.Count - 1)
		{
			_index = 0;
		}
		else
		{
			_index++;
		}
		UpdateText();
		HandleChange();
	}

	private void LocaleChanged(Locale locale)
	{
		UpdateText();
	}

	private void UpdateText()
	{
		PrevNextOption prevNextOption = options[_index];
		string displayText = prevNextOption.displayText;
		string text = prevNextOption.localizedString?.GetLocalizedString();
		this.text.text = text ?? displayText;
	}

	private void HandleChange()
	{
		handler.Setup(options[_index].identifier);
		handler.Handle();
	}
}
