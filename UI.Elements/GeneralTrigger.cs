using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI.Elements;

public class GeneralTrigger : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[Header("Events")]
	[SerializeField]
	private UnityEvent onPointerEnter;

	[SerializeField]
	private UnityEvent onPointerExit;

	public void OnPointerEnter(PointerEventData eventData)
	{
		onPointerEnter?.Invoke();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		onPointerExit?.Invoke();
	}
}
