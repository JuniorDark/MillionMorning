using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI.Elements;

public class ActiveZone : MonoBehaviour, IPointerExitHandler, IEventSystemHandler
{
	public UnityEvent onExit;

	public void OnPointerExit(PointerEventData eventData)
	{
		onExit?.Invoke();
	}
}
