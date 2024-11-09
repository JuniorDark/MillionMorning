using System;
using Core.GameEvent;
using UnityEngine;

namespace UI.Elements.Window;

public abstract class UIWindow : MonoBehaviour
{
	[SerializeField]
	private bool remainActiveAfterInit;

	private UIManager _uiManager;

	protected virtual void Awake()
	{
		_uiManager = UIManager.Get();
	}

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

	protected virtual void OnEnable()
	{
		GameEvent.OnGoBackEvent = (Action)Delegate.Combine(GameEvent.OnGoBackEvent, new Action(Close));
	}

	protected virtual void OnDisable()
	{
		GameEvent.OnGoBackEvent = (Action)Delegate.Remove(GameEvent.OnGoBackEvent, new Action(Close));
	}

	public virtual void Close()
	{
		_uiManager.GoBack();
	}

	public virtual void Open()
	{
		_uiManager.OpenAndAddToHistory(this);
	}
}
