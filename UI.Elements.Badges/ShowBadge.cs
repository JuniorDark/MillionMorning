using Code.Core.Avatar;
using Core.GameEvent;
using TMPro;
using UI.Marker.Combat;
using UI.Tooltip.Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Elements.Badges;

public class ShowBadge : MonoBehaviour
{
	[Header("Assets")]
	[SerializeField]
	private Image icon;

	[SerializeField]
	private BadgeSO badgeSO;

	private string _tooltipText;

	private TMP_Text _text;

	private MilMo_Avatar _avatar;

	public void Start()
	{
		if (badgeSO == null)
		{
			Debug.LogError(base.gameObject.name + ": BadgeSO is null");
			return;
		}
		_avatar = GetAvatar();
		if (_avatar == null)
		{
			Debug.LogError(base.gameObject.name + ": Avatar is null");
			return;
		}
		badgeSO.Init(_avatar);
		LoadSprite();
	}

	private MilMo_Avatar GetAvatar()
	{
		return GetComponentInParent<PlayerMarker>().GetAvatar();
	}

	private void OnEnable()
	{
		Show(shouldShow: true);
	}

	private void LoadSprite()
	{
		Sprite sprite = badgeSO.GetSprite();
		if (sprite == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to load badge sprite");
		}
		else
		{
			icon.sprite = sprite;
		}
	}

	public void Show(bool shouldShow)
	{
		bool active = shouldShow && badgeSO.IsEarned(_avatar);
		base.gameObject.SetActive(active);
	}

	public void SetText(string newText)
	{
		if (!(_text == null))
		{
			_text.text = newText;
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
