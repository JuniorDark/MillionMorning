using System;
using System.Collections.Generic;
using Code.Core.Network;
using Code.Core.Network.types;
using Code.World.Player;
using Core;
using UnityEngine;

namespace Code.Core.Items;

public class MilMo_Ability : MilMo_Item, IUsable, IHaveCooldown, IAssignable
{
	private bool _isCurrentlyActive;

	private float _activationTime = float.MinValue;

	private Action _onUsed;

	private Action _onFailedUse;

	private new MilMo_AbilityTemplate Template => base.Template as MilMo_AbilityTemplate;

	public MilMo_Ability(MilMo_ItemTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public float GetTimeRemaining()
	{
		return Math.Max(_activationTime + Template.Cooldown - Time.time, 0f);
	}

	public bool TestCooldownExpired()
	{
		if (!_isCurrentlyActive)
		{
			return Time.time - _activationTime > Template.Cooldown;
		}
		return false;
	}

	public float GetCooldownProgress()
	{
		if (Template.Cooldown != 0f)
		{
			return Mathf.Clamp01((Time.time - _activationTime) / Template.Cooldown);
		}
		return 1f;
	}

	public float GetCooldown()
	{
		return Template.Cooldown;
	}

	public void RegisterOnUsed(Action onUsed)
	{
		_onUsed = (Action)Delegate.Combine(_onUsed, onUsed);
	}

	public void UnregisterOnUsed(Action onUsed)
	{
		_onUsed = (Action)Delegate.Remove(_onUsed, onUsed);
	}

	public void RegisterOnFailedToUse(Action onFail)
	{
		_onFailedUse = (Action)Delegate.Combine(_onFailedUse, onFail);
	}

	public void UnregisterOnFailedToUse(Action onFail)
	{
		_onFailedUse = (Action)Delegate.Combine(_onFailedUse, onFail);
	}

	public bool Use(int entryId)
	{
		if (MilMo_Player.Instance == null || MilMo_Player.Instance.IsExhausted)
		{
			return false;
		}
		if (!TestCooldownExpired())
		{
			return false;
		}
		Vector3 position = MilMo_Player.Instance.Avatar.GameObject.transform.position;
		Vector3 eulerAngles = MilMo_Player.Instance.Avatar.GameObject.transform.rotation.eulerAngles;
		Singleton<GameNetwork>.Instance.RequestActivateAbility(entryId, new vector3(position.x, position.y, position.z), new vector3(eulerAngles.x, eulerAngles.y, eulerAngles.z));
		return true;
	}

	public string GetSaveString(int entryId)
	{
		return $"Item:{entryId}";
	}

	public void WasActivated()
	{
		if (!_isCurrentlyActive)
		{
			_isCurrentlyActive = true;
			_activationTime = Time.time;
			_onUsed?.Invoke();
		}
	}

	public void Deactivate()
	{
		_isCurrentlyActive = false;
		_onFailedUse?.Invoke();
	}

	public override bool IsWieldable()
	{
		return false;
	}

	public override bool IsWearable()
	{
		return false;
	}
}
