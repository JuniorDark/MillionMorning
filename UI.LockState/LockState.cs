using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.LockState;

public class LockState : MonoBehaviour
{
	[SerializeField]
	private bool locked;

	[SerializeField]
	private UnityEvent onUnlock;

	private Selectable _target;

	private bool _locked;

	private void Awake()
	{
		_target = GetComponent<Selectable>();
		if (!_target)
		{
			Debug.LogError("LockState " + base.gameObject.name + ": Missing selectable element");
		}
		else
		{
			_locked = locked;
		}
	}

	private void OnEnable()
	{
		UpdateTarget();
	}

	private void OnValidate()
	{
		if ((bool)_target)
		{
			Refresh();
		}
	}

	public bool IsLocked()
	{
		return locked;
	}

	public void Lock()
	{
		locked = true;
		Refresh();
	}

	public void Unlock()
	{
		locked = false;
		Refresh();
	}

	private void Refresh()
	{
		bool num = _locked;
		_locked = locked;
		UpdateTarget();
		if (num && !locked)
		{
			onUnlock?.Invoke();
		}
	}

	private void UpdateTarget()
	{
		if (!_target)
		{
			Debug.LogError(base.gameObject.name + ": LockState has no target to update");
			return;
		}
		_target.interactable = !locked;
		_target.targetGraphic.raycastTarget = !locked;
		Color color = _target.targetGraphic.color;
		_target.targetGraphic.color = (locked ? new Color(color.r, color.g, color.b, 0.4f) : new Color(color.r, color.g, color.b, 1f));
	}
}
