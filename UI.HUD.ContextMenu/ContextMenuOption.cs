using System;
using Code.Core.ResourceSystem;
using TMPro;
using UI.HUD.ContextMenu.Options;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.HUD.ContextMenu;

public class ContextMenuOption : MonoBehaviour
{
	[SerializeField]
	private Button button;

	[SerializeField]
	private TMP_Text buttonText;

	[SerializeField]
	private Image icon;

	private BaseContextMenuOption _option;

	private string _textIdentifier;

	private UnityAction _onClickAction;

	private Action<bool> _condition;

	private ContextMenu _menu;

	private void OnDestroy()
	{
		UnregisterListener();
	}

	public void Initialize(BaseContextMenuOption option, ContextMenu menu)
	{
		_option = option;
		_menu = menu;
	}

	public void SetStuff()
	{
		LoadImage();
		UpdateText();
		RegisterListener();
	}

	private void LoadImage()
	{
		Sprite sprite = Addressables.LoadAssetAsync<Sprite>(_option.GetIconKey()).WaitForCompletion();
		icon.sprite = sprite;
	}

	private void UpdateText()
	{
		buttonText.text = MilMo_Localization.GetLocString(_option.GetButtonText())?.String;
	}

	private void RegisterListener()
	{
		if (_option != null)
		{
			button.onClick.AddListener(_option.Action);
		}
		if (_menu != null)
		{
			button.onClick.AddListener(_menu.Close);
		}
	}

	private void UnregisterListener()
	{
		if (_option != null)
		{
			button.onClick.RemoveListener(_option.Action);
		}
		if (_menu != null)
		{
			button.onClick.RemoveListener(_menu.Close);
		}
	}

	public void Toggle()
	{
		bool active = _option.ShowCondition();
		base.gameObject.SetActive(active);
	}
}
