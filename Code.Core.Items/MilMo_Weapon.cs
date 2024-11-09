using System.Collections.Generic;
using Code.Core.Visual.Effect;
using Code.World.Player;
using Core.GameEvent;
using Player;
using UnityEngine;

namespace Code.Core.Items;

public abstract class MilMo_Weapon : MilMo_Wieldable, IMilMo_WeaponStats
{
	public float GetAttackStat => Template.NormalDamage;

	public float GetMagicStat => Template.MagicDamage;

	public float GetSpeedStat => Template.Cooldown;

	public float GetRangeStat => Template.Range;

	public new MilMo_WeaponTemplate Template => ((MilMo_Item)this).Template as MilMo_WeaponTemplate;

	public override float Cooldown => Template.Cooldown + Wielder.GetVariableValue("WeaponCooldownModifier");

	public abstract float AttackMovementSpeedModifier { get; }

	protected MilMo_Weapon(MilMo_WeaponTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public virtual MilMo_Effect Attack()
	{
		if (!base.GameObject)
		{
			return null;
		}
		Animation component = base.GameObject.GetComponent<Animation>();
		if (!component || component["Attack"] == null)
		{
			return null;
		}
		component["Attack"].speed = 1f;
		component["Attack"].wrapMode = WrapMode.Once;
		component.Play("Attack");
		return null;
	}

	public void Ready()
	{
		if ((bool)base.GameObject)
		{
			Animation component = base.GameObject.GetComponent<Animation>();
			if ((bool)component && !(component["Attack"] == null))
			{
				component["Attack"].time = 0.5f;
				component["Attack"].speed = 0f;
				component.Play("Attack");
			}
		}
	}

	public override bool CanUse()
	{
		if (!base.CanUse())
		{
			return false;
		}
		if (!Template.NeedAmmo)
		{
			return true;
		}
		AmmoManager playerAmmoManager = MilMo_Player.Instance.PlayerAmmoManager;
		int num;
		if ((bool)playerAmmoManager)
		{
			num = ((playerAmmoManager.GetAmount(Template.AmmoType) >= Template.AmmoAmount) ? 1 : 0);
			if (num != 0)
			{
				goto IL_005d;
			}
		}
		else
		{
			num = 0;
		}
		GameEvent.AmmoNotEnoughEvent.RaiseEvent();
		goto IL_005d;
		IL_005d:
		return (byte)num != 0;
	}
}
