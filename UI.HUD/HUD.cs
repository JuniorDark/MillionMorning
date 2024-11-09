using System;
using System.Collections.Generic;
using Core.GameEvent;
using UI.HUD.States;
using UnityEngine;

namespace UI.HUD;

public class HUD : MonoBehaviour
{
	[Header("Elements")]
	public HudElement counterContainer;

	public HudElement healthBar;

	public HudElement bagButton;

	public HudElement slidingPaneContainer;

	public HudElement topMenu;

	public HudElement homeMenu;

	public HudElement chat;

	public HudElement actionbar;

	public HudElement pvpActionbar;

	public HudElement weaponSwap;

	public HudElement useWidget;

	public HudElement combatWidget;

	public HudElement classSelectionButton;

	public HudElement contextMenu;

	public HudElement controllerChoice;

	[Header("State")]
	private HudState _currentState;

	private HudStateFactory _stateFactory;

	private readonly Queue<HudState.States> _pendingStates = new Queue<HudState.States>();

	private bool _initialized;

	public HudState CurrentState
	{
		get
		{
			return _currentState;
		}
		set
		{
			_currentState = value;
		}
	}

	private void Awake()
	{
		GameEvent.ShowHUDEvent.RegisterAction(ShowHUD);
		GameEvent.UpdateHudStateEvent.RegisterAction(UpdateState);
	}

	private void Start()
	{
		InitializeState();
	}

	private void OnDestroy()
	{
		GameEvent.ShowHUDEvent.UnregisterAction(ShowHUD);
		GameEvent.UpdateHudStateEvent.UnregisterAction(UpdateState);
	}

	private void ShowHUD(bool shouldEnable)
	{
		if (base.gameObject.activeSelf != shouldEnable)
		{
			base.gameObject.SetActive(shouldEnable);
		}
	}

	private void InitializeState()
	{
		_stateFactory = new HudStateFactory(this);
		_currentState = _stateFactory.Initial();
		_currentState.EnterState();
		_initialized = true;
		int count = _pendingStates.Count;
		for (int i = 0; i < count; i++)
		{
			UpdateState(_pendingStates.Dequeue());
		}
	}

	private void UpdateState(HudState.States newState)
	{
		if (!_initialized)
		{
			_pendingStates.Enqueue(newState);
			return;
		}
		switch (newState)
		{
		case HudState.States.Home:
			_currentState.SwitchState(_stateFactory.InHome());
			break;
		case HudState.States.Normal:
			_currentState.SwitchState(_stateFactory.InLevel());
			break;
		case HudState.States.StarterLevel:
			_currentState.SwitchState(_stateFactory.InStartLevel());
			break;
		case HudState.States.Pvp:
			_currentState.SwitchState(_stateFactory.InPVP());
			break;
		case HudState.States.PvpAbilities:
			_currentState.SwitchState(_stateFactory.InPVPAbilities());
			break;
		default:
			throw new ArgumentOutOfRangeException("newState", newState, null);
		}
	}

	public void ToggleBag()
	{
		if (base.gameObject.activeSelf)
		{
			GameEvent.ToggleInventoryEvent?.RaiseEvent();
		}
	}
}
