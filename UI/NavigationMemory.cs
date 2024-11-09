using System.Collections.Generic;
using UI.Elements.TabSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI;

public class NavigationMemory : MonoBehaviour
{
	[SerializeField]
	private Selectable[] selectables;

	[SerializeField]
	private Selectable firstSelected;

	[SerializeField]
	private bool shouldReset = true;

	private UIManager _uiManager;

	private Selectable _lastSelected;

	private List<EventTrigger> _eventTriggers;

	private List<EventTrigger> _temporaryEventTriggers;

	private EventTrigger.Entry _selectTrigger;

	private void Awake()
	{
		_uiManager = UIManager.Get();
		if (_uiManager == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing _uiManager");
			base.enabled = false;
			return;
		}
		if (shouldReset && _uiManager != null)
		{
			_uiManager.NavigationResetEvent += ResetMemory;
		}
		EventTrigger.TriggerEvent triggerEvent = new EventTrigger.TriggerEvent();
		triggerEvent.AddListener(Select);
		_selectTrigger = new EventTrigger.Entry
		{
			eventID = EventTriggerType.Select,
			callback = triggerEvent
		};
		_eventTriggers = new List<EventTrigger>();
		_temporaryEventTriggers = new List<EventTrigger>();
		for (int i = 0; i < selectables.Length; i++)
		{
			Selectable selectable = selectables[i];
			if (selectable == null)
			{
				Debug.LogWarning("Missing selectable in Navigation Memory! " + base.gameObject.name);
				continue;
			}
			EventTrigger eventTrigger = selectable.gameObject.GetComponent<EventTrigger>();
			if (!eventTrigger)
			{
				eventTrigger = selectable.gameObject.AddComponent<EventTrigger>();
				_temporaryEventTriggers.Add(eventTrigger);
			}
			_eventTriggers.Add(eventTrigger);
		}
	}

	private void OnDestroy()
	{
		for (int num = _temporaryEventTriggers.Count - 1; num >= 0; num--)
		{
			Object.Destroy(_temporaryEventTriggers[num]);
		}
		if (shouldReset && _uiManager != null)
		{
			_uiManager.NavigationResetEvent -= ResetMemory;
		}
	}

	private void Select(BaseEventData baseEventData)
	{
		GameObject selectedObject = baseEventData.selectedObject;
		_lastSelected = (selectedObject ? selectedObject.GetComponent<Selectable>() : null);
	}

	private void OnEnable()
	{
		for (int i = 0; i < _eventTriggers.Count; i++)
		{
			_eventTriggers[i].triggers.Add(_selectTrigger);
		}
		if (_uiManager != null)
		{
			_uiManager.NavigationStartEvent += SelectLastSelected;
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < _eventTriggers.Count; i++)
		{
			_eventTriggers[i].triggers.Remove(_selectTrigger);
		}
		if (_uiManager != null)
		{
			_uiManager.NavigationStartEvent -= SelectLastSelected;
		}
	}

	public void SelectLastSelected()
	{
		if (EventSystem.current == null)
		{
			return;
		}
		Selectable selectable = ((_lastSelected != null) ? _lastSelected : ((firstSelected != null) ? firstSelected : null));
		if (selectable == null)
		{
			return;
		}
		EventSystem.current.SetSelectedGameObject(selectable.gameObject);
		selectable.OnSelect(null);
		Transform transform = selectable.transform.root.transform;
		Transform parent = selectable.transform;
		while (parent != transform && parent != null)
		{
			TabStrip component = parent.GetComponent<TabStrip>();
			if (component != null)
			{
				component.ActivateTabContaining(selectable);
				break;
			}
			parent = parent.parent;
		}
	}

	private void ResetMemory()
	{
		_lastSelected = null;
	}
}
