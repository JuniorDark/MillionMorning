using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.HUD.HomeMenu;

public class HomeMenuToggle : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
{
	[SerializeField]
	protected Toggle toggle;

	protected bool WasOn;

	protected virtual void Awake()
	{
		if (toggle == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing toggle");
		}
	}

	protected virtual void OnEnable()
	{
		OnValueChanged(value: false);
	}

	public virtual void OnValueChanged(bool value)
	{
		if (!(toggle == null) && !(toggle.targetGraphic == null))
		{
			WasOn = value;
			toggle.targetGraphic.enabled = !value;
			if (toggle.isOn != value)
			{
				toggle.SetIsOnWithoutNotify(value);
			}
			EventSystem.current.SetSelectedGameObject(null);
		}
	}

	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		Impulse();
	}

	protected void Impulse()
	{
		LeanTween.scale(base.gameObject, Vector3.one * 1.1f, 0f);
		LeanTween.scale(base.gameObject, Vector3.one, 0.4f).setEase(LeanTweenType.easeOutQuart);
	}
}
