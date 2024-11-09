using Code.Core.Avatar;
using Code.Core.Items;
using Code.World.Level.LevelObject;
using Code.World.Player;

namespace Code.World.Attack;

public class MilMo_PlayerAttack : MilMo_Attack
{
	protected IMilMo_AttackTarget MTarget;

	protected float MDamage;

	protected MilMo_Weapon MWeapon;

	protected MilMo_Avatar MAttacker;

	protected bool MIsKillingBlow;

	public IMilMo_AttackTarget Target => MTarget;

	public override float TargetRadius
	{
		get
		{
			if (MTarget == null)
			{
				return 0f;
			}
			return MTarget.ImpactRadius;
		}
	}

	public override float TargetSqrRadius
	{
		get
		{
			if (MTarget == null)
			{
				return 0f;
			}
			return MTarget.ImpactRadiusSqr;
		}
	}

	protected MilMo_PlayerAttack(MilMo_Avatar attacker, IMilMo_AttackTarget target, MilMo_Weapon weapon, bool isHit, float damage, bool isKillingBlow)
		: base(isHit)
	{
		MAttacker = attacker;
		MWeapon = weapon;
		MTarget = target;
		MDamage = damage;
		MIsKillingBlow = isKillingBlow;
		if (attacker != null)
		{
			if (target != null && target.IsDangerous())
			{
				attacker.ResetCombatTimer();
			}
			if (attacker.Id != MilMo_Player.Instance.Avatar.Id && weapon != null)
			{
				attacker.PlayAttackEffects();
				attacker.AddParticleEffect(weapon.Attack());
			}
		}
	}

	public static MilMo_PlayerAttack CreateAttack(string fullLevelName, MilMo_Avatar attacker, IMilMo_AttackTarget target, bool isHit, float damage, bool wasKillingBlow, MilMo_Weapon weapon)
	{
		if (weapon is MilMo_MeleeWeapon)
		{
			return new MilMo_MeleePlayerAttack(attacker, target, weapon, isHit, damage, wasKillingBlow);
		}
		if (weapon is MilMo_RangedWeapon)
		{
			return new MilMo_RangedPlayerAttack(fullLevelName, attacker, target, weapon, isHit, damage, wasKillingBlow);
		}
		return new MilMo_NoWeaponPlayerAttack(attacker, target, null, isHit, damage, wasKillingBlow);
	}

	public override void Resolve()
	{
		if (MTarget != null)
		{
			MTarget.Damage(MDamage);
		}
	}
}
