using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Window.FriendList.Notifications;

[RequireComponent(typeof(Image))]
public abstract class FriendListNotification : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	private Image _icon;

	protected virtual void Awake()
	{
		_icon = GetComponent<Image>();
	}

	public void Show()
	{
		base.gameObject.SetActive(value: true);
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}

	protected abstract void Action();

	public void OnPointerEnter(PointerEventData eventData)
	{
		OnHoverShow(eventData);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		OnHoverHide();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.clickCount == 2)
		{
			Debug.LogWarning("Double-Click!: " + base.gameObject.name);
			Action();
		}
	}

	protected abstract void OnHoverShow(PointerEventData data);

	protected abstract void OnHoverHide();
}
