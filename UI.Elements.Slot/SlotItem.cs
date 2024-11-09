using Code.Core.Items;
using Core.GameEvent;
using TMPro;
using UI.Tooltip.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Elements.Slot;

public class SlotItem : MonoBehaviour
{
	[Header("Assets")]
	[SerializeField]
	protected Image iconImage;

	[SerializeField]
	protected TMP_Text amountText;

	public UnityEvent onClearEntry;

	protected ISlotItemEntry Entry;

	private int _loadingTween;

	public event UnityAction OnIconReady;

	private void Awake()
	{
		if (iconImage == null)
		{
			Debug.LogWarning("Missing iconImage component");
			base.enabled = false;
		}
		if (amountText == null)
		{
			Debug.LogWarning("Missing amountText component");
			base.enabled = false;
		}
		if (base.enabled)
		{
			if ((object)iconImage != null)
			{
				iconImage.enabled = false;
			}
			if ((object)amountText != null)
			{
				amountText.text = "";
				amountText.enabled = false;
			}
		}
	}

	private void OnDestroy()
	{
		if (Entry != null)
		{
			Entry.UnregisterOnAmountUpdated(SetAmount);
			Entry = null;
		}
	}

	private void SetAmount(int amount)
	{
		if (!(amountText == null))
		{
			if (amount > 1)
			{
				amountText.text = amount.ToString();
				amountText.enabled = true;
			}
			else
			{
				ResetAmount();
			}
		}
	}

	private void ResetAmount()
	{
		amountText.text = "";
		amountText.enabled = false;
	}

	public virtual void ClearEntry()
	{
		if (Entry != null)
		{
			onClearEntry?.Invoke();
			Entry.UnregisterOnAmountUpdated(SetAmount);
			ResetAmount();
			SetIcon(null);
			Entry = null;
		}
	}

	public virtual void SetEntry(ISlotItemEntry entry)
	{
		ClearEntry();
		if (entry != null)
		{
			Entry = entry;
			Entry.RegisterOnAmountUpdated(SetAmount);
			SetAmount(Entry.GetAmount());
			if (Entry.GetItem() != null)
			{
				LoadIcon();
			}
		}
	}

	public ISlotItemEntry GetEntry()
	{
		return Entry;
	}

	private IEntryItem GetItem()
	{
		return Entry?.GetItem();
	}

	public bool HasEntry()
	{
		return Entry != null;
	}

	private async void LoadIcon()
	{
		if ((bool)iconImage)
		{
			if (_loadingTween == 0 && (float)LeanTween.tweensRunning < (float)LeanTween.maxSimulataneousTweens * 0.5f)
			{
				_loadingTween = LeanTween.rotateAroundLocal(iconImage.gameObject, Vector3.forward, -360f, 2f).setRepeat(999).id;
			}
			SetIconColor(new Color(1f, 1f, 1f, 0.4f));
			Texture2D texture2D = Entry.GetItem().GetItemIcon();
			if (!texture2D)
			{
				texture2D = await Entry.GetItem().AsyncGetIcon();
			}
			SetIcon(texture2D);
		}
	}

	private void SetIcon(Texture2D newTexture)
	{
		if (iconImage == null)
		{
			return;
		}
		iconImage.enabled = false;
		if (_loadingTween != 0)
		{
			LeanTween.cancel(_loadingTween);
			_loadingTween = 0;
		}
		iconImage.transform.localRotation = Quaternion.identity;
		if ((bool)newTexture)
		{
			if (!iconImage.sprite || iconImage.sprite.texture != newTexture)
			{
				Vector2 pivot = iconImage.rectTransform.pivot;
				Rect rect = new Rect(0f, 0f, newTexture.width, newTexture.height);
				Sprite sprite = Sprite.Create(newTexture, rect, pivot);
				iconImage.sprite = sprite;
			}
			SetIconColor(Color.white);
			iconImage.enabled = true;
			this.OnIconReady?.Invoke();
		}
	}

	protected void SetIconColor(Color color)
	{
		if (!(iconImage == null) && !(iconImage.color == color))
		{
			iconImage.color = color;
		}
	}

	private Texture2D GetIcon()
	{
		if (!iconImage || !iconImage.sprite || !iconImage.sprite.texture)
		{
			return null;
		}
		return iconImage.sprite.texture;
	}

	public async void ShowTooltip()
	{
		IEntryItem item = GetItem();
		Texture2D icon = GetIcon();
		if (item != null)
		{
			TooltipData tooltipData = await new CreateTooltipDataHandler(item, icon).GetTooltipData();
			if (tooltipData == null)
			{
				Debug.LogWarning(base.gameObject.name + ": Unable to get tooltip data");
			}
			else
			{
				GameEvent.ShowTooltipEvent?.RaiseEvent(tooltipData);
			}
		}
	}

	public void HideTooltip()
	{
		GameEvent.HideTooltipEvent?.RaiseEvent();
	}

	public void UseItem()
	{
		IEntryItem entryItem = Entry?.GetItem();
		if (entryItem == null)
		{
			Debug.LogWarning("SI: Entry does not have an item.");
		}
		else if (!(entryItem is IUsable usable))
		{
			Debug.LogWarning($"SI: {entryItem.GetDisplayName()} is not usable.");
		}
		else if (!usable.Use(Entry.GetId()))
		{
			Debug.LogWarning($"SI: Failed to use item {Entry.GetItem().GetDisplayName()}");
		}
	}

	public bool IsAssignable()
	{
		return Entry?.GetItem() is IAssignable;
	}
}
