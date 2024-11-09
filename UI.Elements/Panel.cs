using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI.Elements;

public class Panel : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	[SerializeField]
	private bool remainActiveAfterInit;

	[SerializeField]
	private UnityEvent onOpen;

	[SerializeField]
	private UnityEvent onClose;

	protected virtual void Start()
	{
		if (!remainActiveAfterInit)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void Toggle()
	{
		if (base.isActiveAndEnabled)
		{
			Close();
		}
		else
		{
			Open();
		}
	}

	public virtual void Open()
	{
		base.gameObject.SetActive(value: true);
		onOpen?.Invoke();
		base.transform.SetAsLastSibling();
	}

	public virtual void Close()
	{
		base.gameObject.SetActive(value: false);
		onClose?.Invoke();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		base.transform.SetAsLastSibling();
	}
}
