using Core.GameEvent;
using UI.Tooltip.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Tooltip.Triggers;

public abstract class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public bool active = true;

	protected virtual TooltipData GetData()
	{
		return new TooltipData("");
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		TriggerEvent();
	}

	private void TriggerEvent()
	{
		if (active && base.gameObject.activeSelf)
		{
			GameEvent.ShowTooltipEvent?.RaiseEvent(GetData());
		}
	}

	public virtual void OnPointerExit(PointerEventData eventData)
	{
		GameEvent.HideTooltipEvent?.RaiseEvent();
	}

	private void OnDisable()
	{
		GameEvent.HideTooltipEvent?.RaiseEvent();
	}
}
