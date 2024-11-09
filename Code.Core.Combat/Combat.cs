using Code.Core.Avatar;
using Code.Core.EventSystem;
using Core.GameEvent;
using UnityEngine;

namespace Code.Core.Combat;

public class Combat
{
	private const float COMBAT_TIMER_TIMEOUT = 8f;

	private MilMo_Avatar _avatar;

	private float _combatTimer;

	private bool _inCombat;

	public bool InCombat
	{
		get
		{
			return _inCombat;
		}
		private set
		{
			_inCombat = value;
		}
	}

	public Combat(MilMo_Avatar avatar)
	{
		_avatar = avatar;
		_avatar.OnHealthDecreased += ResetCombatTimer;
	}

	public void ResetCombatTimer()
	{
		if (!(_avatar.Health <= 0f))
		{
			_combatTimer = Time.time;
			if (!InCombat)
			{
				EnterCombat();
			}
		}
	}

	public void EndCombat()
	{
		_combatTimer = Time.time - 16f;
		ExitCombat();
	}

	public void EnterCombat()
	{
		InCombat = true;
		MilMo_EventSystem.At(8f, TryExitCombat);
		_avatar.DrawWeapon();
		if (_avatar.IsTheLocalPlayer)
		{
			GameEvent.InCombatEvent?.RaiseEvent(args: true);
		}
	}

	public void ExitCombat()
	{
		if (InCombat)
		{
			InCombat = false;
			_avatar.SheathWeapon();
			if (_avatar.IsTheLocalPlayer)
			{
				GameEvent.InCombatEvent?.RaiseEvent(args: false);
			}
		}
	}

	public void TryExitCombat()
	{
		float num = Time.time - _combatTimer;
		if (num < 8f)
		{
			MilMo_EventSystem.At(8f - num, TryExitCombat);
		}
		else
		{
			ExitCombat();
		}
	}
}
