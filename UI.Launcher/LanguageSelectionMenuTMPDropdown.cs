using System.Collections.Generic;
using Code.Core.ResourceSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UI.Launcher;

[RequireComponent(typeof(TMP_Dropdown))]
public class LanguageSelectionMenuTMPDropdown : MonoBehaviour
{
	private TMP_Dropdown _dropdown;

	private AsyncOperationHandle _initializeOperation;

	private void Start()
	{
		_dropdown = GetComponent<TMP_Dropdown>();
		_dropdown.onValueChanged.AddListener(OnSelectionChanged);
		_dropdown.ClearOptions();
		_dropdown.options.Add(new TMP_Dropdown.OptionData("Loading..."));
		_dropdown.interactable = false;
		_initializeOperation = LocalizationSettings.SelectedLocaleAsync;
		if (_initializeOperation.IsDone)
		{
			InitializeCompleted(_initializeOperation);
		}
		else
		{
			_initializeOperation.Completed += InitializeCompleted;
		}
	}

	private void InitializeCompleted(AsyncOperationHandle obj)
	{
		List<string> list = new List<string>();
		int valueWithoutNotify = 0;
		List<Locale> locales = LocalizationSettings.AvailableLocales.Locales;
		for (int i = 0; i < locales.Count; i++)
		{
			Locale locale = locales[i];
			if (LocalizationSettings.SelectedLocale == locale)
			{
				valueWithoutNotify = i;
			}
			string item = ((locales[i].Identifier.CultureInfo != null) ? locales[i].Identifier.CultureInfo.NativeName : locales[i].ToString());
			list.Add(item);
		}
		if (list.Count == 0)
		{
			list.Add("No Locales Available");
			_dropdown.interactable = false;
		}
		else
		{
			_dropdown.interactable = true;
		}
		_dropdown.ClearOptions();
		_dropdown.AddOptions(list);
		_dropdown.SetValueWithoutNotify(valueWithoutNotify);
		LocalizationSettings.SelectedLocaleChanged += LocalizationSettings_SelectedLocaleChanged;
	}

	private void OnDestroy()
	{
		LocalizationSettings.SelectedLocaleChanged -= LocalizationSettings_SelectedLocaleChanged;
	}

	private void OnSelectionChanged(int index)
	{
		LocalizationSettings.SelectedLocaleChanged -= LocalizationSettings_SelectedLocaleChanged;
		LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
		LocalizationSettings.SelectedLocaleChanged += LocalizationSettings_SelectedLocaleChanged;
	}

	private void LocalizationSettings_SelectedLocaleChanged(Locale locale)
	{
		int valueWithoutNotify = LocalizationSettings.AvailableLocales.Locales.IndexOf(locale);
		_dropdown.SetValueWithoutNotify(valueWithoutNotify);
		MilMo_Localization.ChangeLanguage(locale.Identifier.Code);
	}
}
