using Core.GameEvent;
using TMPro;
using UI.Tooltip.Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Elements.Badges;

public class Badge : MonoBehaviour
{
	public enum BadgeType
	{
		Level,
		Member,
		Role,
		Gm,
		Admin,
		GroupLeader
	}

	[Header("Assets")]
	[SerializeField]
	private Image icon;

	[SerializeField]
	private TMP_Text text;

	[Header("Type")]
	public BadgeType type;

	private string _tooltipText;

	public void Show(bool shouldShow)
	{
		base.gameObject.SetActive(shouldShow);
	}

	public void SetIcon(Texture2D newTexture)
	{
		if (!(icon == null) && (bool)newTexture && (!icon.sprite || icon.sprite.texture != newTexture))
		{
			Vector2 pivot = icon.rectTransform.pivot;
			Rect rect = new Rect(0f, 0f, newTexture.width, newTexture.height);
			Sprite sprite = Sprite.Create(newTexture, rect, pivot);
			icon.sprite = sprite;
		}
	}

	public void SetText(string newText)
	{
		if (!(text == null))
		{
			text.text = newText;
		}
	}

	public void SetTooltipText(string newText)
	{
		_tooltipText = newText;
	}

	public void ShowTooltip()
	{
		TooltipData args = new TooltipData(_tooltipText);
		GameEvent.ShowTooltipEvent?.RaiseEvent(args);
	}

	public void HideTooltip()
	{
		GameEvent.HideTooltipEvent?.RaiseEvent();
	}
}
