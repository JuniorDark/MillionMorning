using Code.Core.Avatar.Badges;
using Core.GameEvent;
using TMPro;
using UI.Tooltip.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace UI.Marker.Combat;

public class BadgeTwo : MonoBehaviour
{
	[Header("Scene")]
	[SerializeField]
	private Image icon;

	[SerializeField]
	private TMP_Text text;

	private BaseBadge _badge;

	private Sprite _badgeSprite;

	private string _tooltipText;

	public void Init(BaseBadge badge)
	{
		_badge = badge;
	}

	private void Start()
	{
		if (_badge != null)
		{
			LoadIcon();
			_badge.OnChange += Refresh;
			Refresh();
		}
	}

	private void OnDestroy()
	{
		if (_badge != null)
		{
			_badge.OnChange -= Refresh;
		}
	}

	private void LoadIcon()
	{
		string iconPath = _badge.GetIconPath();
		if (string.IsNullOrEmpty(iconPath))
		{
			Debug.LogError(base.gameObject.name + ": iconPath is empty");
			return;
		}
		_badgeSprite = Addressables.LoadAssetAsync<Sprite>(iconPath).WaitForCompletion();
		if (!_badgeSprite)
		{
			Debug.LogError(base.gameObject.name + ": Unable to load Sprite");
		}
		icon.sprite = _badgeSprite;
	}

	private void Refresh()
	{
		SetText(_badge.GetText());
		SetTooltipText(_badge.GetTooltipText());
		base.gameObject.SetActive(_badge.IsEarned());
	}

	private void SetText(string newText)
	{
		if (!(text == null))
		{
			text.text = newText;
		}
	}

	private void SetTooltipText(string newText)
	{
		_tooltipText = newText;
	}

	public void ShowTooltip()
	{
		if (!string.IsNullOrEmpty(_tooltipText))
		{
			TooltipData args = new TooltipData(_tooltipText);
			GameEvent.ShowTooltipEvent?.RaiseEvent(args);
		}
	}

	public void HideTooltip()
	{
		GameEvent.HideTooltipEvent?.RaiseEvent();
	}
}
