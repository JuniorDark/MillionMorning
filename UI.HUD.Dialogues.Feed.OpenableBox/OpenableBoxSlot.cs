using Core.GameEvent;
using Core.Utilities;
using TMPro;
using UI.Elements.Slot;
using UI.Tooltip.Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.Dialogues.Feed.OpenableBox;

public class OpenableBoxSlot : MonoBehaviour
{
	[SerializeField]
	private Image icon;

	[SerializeField]
	private TMP_Text amountText;

	private IEntryItem _item;

	public async void Init(IEntryItem item, int amount)
	{
		_item = item;
		Image image = icon;
		Core.Utilities.UI.SetIcon(image, await item.AsyncGetIcon());
		amountText.text = amount.ToString();
	}

	public async void ShowTooltip()
	{
		if (_item != null)
		{
			TooltipData args = await new CreateTooltipDataHandler(_item, icon.sprite.texture).GetTooltipData();
			GameEvent.ShowTooltipEvent?.RaiseEvent(args);
		}
	}

	public void HideTooltip()
	{
		GameEvent.HideTooltipEvent?.RaiseEvent();
	}
}
