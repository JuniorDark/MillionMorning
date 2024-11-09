using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Core.Items;
using Core.GameEvent;
using Core.Utilities;
using TMPro;
using UI.Tooltip.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.HUD.Dialogues.NPC.Shop;

public class NPCShopItem : MonoBehaviour
{
	[Header("Elements")]
	[SerializeField]
	private Toggle toggle;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private TMP_Text itemText;

	[SerializeField]
	private Image itemIcon;

	[SerializeField]
	private TMP_Text priceText;

	[SerializeField]
	private Image priceIcon;

	[SerializeField]
	private TMP_Text inventoryAmountText;

	[SerializeField]
	private UnityEvent onSelect;

	[SerializeField]
	private UnityEvent onNotAfforded;

	private NPCShopEntry _shopEntry;

	private Texture2D _itemTexture;

	private MilMo_Item _item;

	private void Awake()
	{
		if (toggle == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing toggle");
		}
		else if (canvasGroup == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing canvasGroup");
		}
		else if (itemText == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing text");
		}
		else if (itemIcon == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing icon");
		}
		else if (priceText == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing priceText");
		}
		else if (priceIcon == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing priceIcon");
		}
		else if (inventoryAmountText == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing inventoryAmountText");
		}
	}

	public void Show(bool shouldShow)
	{
		if (base.gameObject.activeSelf != shouldShow)
		{
			base.gameObject.SetActive(shouldShow);
		}
	}

	public void SetEnabled(bool shouldEnable)
	{
		if (!(canvasGroup == null))
		{
			canvasGroup.alpha = (shouldEnable ? 1f : 0.5f);
		}
	}

	public async Task Setup(NPCShopEntry entry, Currency currency)
	{
		_shopEntry = entry;
		if (_shopEntry == null)
		{
			return;
		}
		SetEnabled(_shopEntry.IsEnabled);
		if (itemText != null)
		{
			itemText.text = _shopEntry.Template?.DisplayName?.String;
		}
		if (priceText != null)
		{
			priceText.text = _shopEntry.Price.ToString();
		}
		RefreshAmount();
		if (itemIcon != null)
		{
			itemIcon.enabled = false;
		}
		if (priceIcon != null && currency != null)
		{
			priceIcon.enabled = false;
			Texture2D texture2D = currency.GetTexture2D();
			if (texture2D != null)
			{
				Core.Utilities.UI.SetIcon(priceIcon, texture2D);
			}
		}
		_item = _shopEntry.Template?.Instantiate(new Dictionary<string, string>());
		if (_item != null)
		{
			_itemTexture = await _item.AsyncGetIcon();
			if (_itemTexture != null)
			{
				Core.Utilities.UI.SetIcon(itemIcon, _itemTexture);
			}
		}
	}

	public void RefreshAmount()
	{
		if (!(inventoryAmountText == null))
		{
			bool flag = !_shopEntry.ForSale;
			inventoryAmountText.enabled = flag;
			if (flag)
			{
				SetAmount((_shopEntry?.InventoryEntry?.GetAmount()).GetValueOrDefault());
			}
		}
	}

	private void SetAmount(int value)
	{
		inventoryAmountText.text = ((value > 1) ? value.ToString() : "");
	}

	public void OnValueChanged(bool wentOn)
	{
		if (!wentOn || !TryToSelect())
		{
			toggle.SetIsOnWithoutNotify(value: false);
		}
		if (!wentOn && _shopEntry != null)
		{
			_shopEntry.OnDeselected?.Invoke(_shopEntry);
		}
	}

	public void Deselect()
	{
		toggle.isOn = false;
	}

	public bool TryToSelect()
	{
		if (_shopEntry == null)
		{
			return false;
		}
		if (!_shopEntry.IsEnabled)
		{
			NotAffordedEffect();
			return false;
		}
		onSelect?.Invoke();
		_shopEntry.OnSelected?.Invoke(_shopEntry);
		return true;
	}

	private void NotAffordedEffect()
	{
		if (_shopEntry != null)
		{
			onNotAfforded?.Invoke();
			if (_shopEntry.CurrencyIdentifier.Equals("GEMS", StringComparison.CurrentCultureIgnoreCase))
			{
				GameEvent.GemsNotEnoughEvent?.RaiseEvent();
			}
		}
	}

	public async void ShowTooltip()
	{
		if (_item != null)
		{
			TooltipData args = await new CreateTooltipDataHandler(_item, _itemTexture).GetTooltipData();
			GameEvent.ShowTooltipEvent?.RaiseEvent(args);
		}
	}

	public void HideTooltip()
	{
		GameEvent.HideTooltipEvent?.RaiseEvent();
	}
}
