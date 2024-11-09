using System;
using Core.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.HUD.Dialogues.NPC.Shop;

public class AmountDialogue : MonoBehaviour
{
	[Header("Currency")]
	[SerializeField]
	private Image currencyIcon;

	[SerializeField]
	private TMP_Text currencyText;

	[Header("Inputs")]
	[SerializeField]
	private TMP_InputField inputField;

	[SerializeField]
	private Button increaseButton;

	[SerializeField]
	private Button decreaseButton;

	[SerializeField]
	private Button okButton;

	[SerializeField]
	private Button cancelButton;

	[Header("Events")]
	[SerializeField]
	private UnityEvent onOpen;

	[SerializeField]
	private UnityEvent onCancel;

	[SerializeField]
	private UnityEvent onTick;

	public UnityAction<NPCShopEntry, int> OnConfirm;

	public UnityAction OnClose;

	private Currency _currency;

	private NPCShopEntry _wantedItem;

	private int _wantedAmount;

	private int _animation;

	protected void Awake()
	{
		if (currencyIcon == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing currencyIcon");
		}
		else if (currencyText == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing currencyText");
		}
		else if (inputField == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing inputField");
		}
		else if (increaseButton == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing increaseButton");
		}
		else if (decreaseButton == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing decreaseButton");
		}
		else if (okButton == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing okButton");
		}
		else if (cancelButton == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing cancelButton");
		}
	}

	public void Recalculate()
	{
		if (_wantedItem == null)
		{
			increaseButton.interactable = false;
			decreaseButton.interactable = false;
			okButton.interactable = false;
			return;
		}
		if (_wantedItem.ForSale)
		{
			int num = _currency?.GetAmount() ?? 0;
			if (num < _wantedItem.Price)
			{
				_wantedAmount = 0;
			}
			if (_wantedItem.InventoryEntry != null && _wantedItem.InventoryEntry.GetAmount() + _wantedAmount > 32767)
			{
				_wantedAmount = 32767 - _wantedItem.InventoryEntry.GetAmount();
			}
			if (num < _wantedAmount * _wantedItem.Price)
			{
				_wantedAmount = ((num != 0) ? (num / _wantedItem.Price) : 0);
			}
			increaseButton.interactable = (_wantedAmount + 1) * _wantedItem.Price < num;
		}
		else
		{
			int num2 = _wantedItem.InventoryEntry?.GetAmount() ?? 0;
			if (_wantedAmount > num2)
			{
				_wantedAmount = num2;
			}
			increaseButton.interactable = _wantedAmount + 1 <= num2;
		}
		currencyText.text = (_wantedAmount * _wantedItem.Price).ToString();
		inputField.SetTextWithoutNotify(_wantedAmount.ToString());
		decreaseButton.interactable = _wantedAmount > 0;
		okButton.interactable = _wantedAmount > 0;
	}

	public void Increase()
	{
		_wantedAmount++;
		onTick?.Invoke();
		Recalculate();
	}

	public void Decrease()
	{
		_wantedAmount--;
		onTick?.Invoke();
		Recalculate();
	}

	public void OnValueChanged(string newValue)
	{
		int.TryParse(newValue, out _wantedAmount);
		if (_wantedAmount < 0)
		{
			_wantedAmount = 0;
		}
		Recalculate();
	}

	public void OnCancel()
	{
		onCancel?.Invoke();
		Close();
	}

	public void OnOK()
	{
		OnConfirm?.Invoke(_wantedItem, _wantedAmount);
	}

	public void Open(NPCShopEntry shopEntry, Currency currency)
	{
		_currency = currency;
		_wantedItem = shopEntry;
		_wantedAmount = 1;
		if ((bool)currencyIcon)
		{
			currencyIcon.enabled = false;
			Texture2D texture2D = currency.GetTexture2D();
			if (texture2D != null)
			{
				Core.Utilities.UI.SetIcon(currencyIcon, texture2D);
			}
		}
		if (!base.gameObject.activeInHierarchy)
		{
			SlideIn();
			onOpen?.Invoke();
		}
		Activate(shouldActivate: true);
	}

	public void Close()
	{
		if (base.gameObject.activeInHierarchy)
		{
			SlideOut();
		}
	}

	private void SlideIn()
	{
		if (_animation != 0)
		{
			LeanTween.cancel(_animation);
			_animation = 0;
		}
		LeanTween.moveLocalX(base.gameObject, -200f, 0f);
		base.gameObject.SetActive(value: true);
		_animation = LeanTween.moveLocalX(base.gameObject, 0f, 0.4f).setEase(LeanTweenType.easeOutSine).setOnComplete((Action)delegate
		{
			_animation = 0;
		})
			.id;
	}

	private void SlideOut()
	{
		if (_animation != 0)
		{
			LeanTween.cancel(_animation);
			_animation = 0;
		}
		_animation = LeanTween.moveLocalX(base.gameObject, -200f, 0.4f).setEase(LeanTweenType.easeInBack).setOnComplete((Action)delegate
		{
			_animation = 0;
			base.gameObject.SetActive(value: false);
			OnClose?.Invoke();
		})
			.id;
	}

	public void Activate(bool shouldActivate)
	{
		okButton.interactable = shouldActivate;
		cancelButton.interactable = shouldActivate;
		Recalculate();
	}
}
