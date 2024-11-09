using System;
using System.Collections.Generic;
using Code.Core.Network;
using Code.Core.Network.types;
using Code.World.Level;
using Code.World.Level.LevelObject;
using Code.World.Player;
using Core;
using UnityEngine;

namespace Code.Core.Items;

public class MilMo_SkillItem : MilMo_Item, IUsable, IHaveCooldown, IAssignable
{
	private float _activationTime = float.MinValue;

	private Action _onUsed;

	private Action _onFail;

	private new MilMo_SkillTemplate Template => base.Template as MilMo_SkillTemplate;

	public MilMo_SkillItem(MilMo_SkillTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public float GetTimeRemaining()
	{
		return Math.Max(_activationTime + Template.Cooldown - Time.time, 0f);
	}

	public bool TestCooldownExpired()
	{
		return Time.time - _activationTime > Template.Cooldown;
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
		_onFail = (Action)Delegate.Combine(_onFail, onFail);
	}

	public void UnregisterOnFailedToUse(Action onFail)
	{
		_onFail = (Action)Delegate.Remove(_onFail, onFail);
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
		Debug.LogWarning($"MilMo_SkillItem:Using {entryId}");
		Vector3 position = MilMo_Player.Instance.Avatar.GameObject.transform.position;
		Vector3 eulerAngles = MilMo_Player.Instance.Avatar.GameObject.transform.rotation.eulerAngles;
		IMilMo_AttackTarget milMo_AttackTarget = ((MilMo_Player.Instance.Avatar.WieldedItem is MilMo_Weapon milMo_Weapon) ? MilMo_Level.CurrentLevel.GetClosestTarget(position, MilMo_Player.Instance.Avatar.GameObject.transform.forward, milMo_Weapon.Template, useHitRange: true) : null);
		Singleton<GameNetwork>.Instance.RequestActiveSkillItem(entryId, milMo_AttackTarget?.AsNetworkAttackTarget(), new vector3(position.x, position.y, position.z), new vector3(eulerAngles.x, eulerAngles.y, eulerAngles.z));
		return true;
	}

	public string GetSaveString(int entryId)
	{
		return $"Item:{entryId}";
	}

	public void Activate()
	{
		_activationTime = Time.time;
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
