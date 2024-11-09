using Code.Core.Avatar;
using Code.Core.EventSystem;
using Code.Core.Items;
using Code.World.Level;
using Code.World.Level.LevelObject;
using UnityEngine;

namespace Code.World.Attack;

public class MilMo_RangedPlayerAttack : MilMo_PlayerAttack
{
	private readonly string _mFullLevelName;

	private MilMo_RangedWeapon Weapon => MWeapon as MilMo_RangedWeapon;

	public MilMo_RangedPlayerAttack(string fullLevelName, MilMo_Avatar attacker, IMilMo_AttackTarget target, MilMo_Weapon weapon, bool isHit, float damage, bool isKillingBlow)
		: base(attacker, target, weapon, isHit, damage, isKillingBlow)
	{
		_mFullLevelName = fullLevelName;
		MilMo_RangedWeapon weapon2 = Weapon;
		if (weapon2 != null)
		{
			MilMo_EventSystem.At(weapon2.Template.FireTime, SpawnProjectile);
		}
		else
		{
			Resolve();
		}
	}

	private void SpawnProjectile()
	{
		MilMo_RangedWeapon weapon = Weapon;
		if (weapon == null || weapon.Template == null || weapon.Template.ProjectileTemplate == null)
		{
			Resolve();
		}
		else
		{
			if (!weapon.Template.ProjectileTemplate.InstantHit)
			{
				return;
			}
			Vector3 projectilePosition = weapon.GetProjectilePosition();
			Vector3 zero = Vector3.zero;
			Vector3 direction;
			if (MTarget == null)
			{
				direction = ((MAttacker != null && !(MAttacker.GameObject == null)) ? MAttacker.GameObject.transform.forward : Vector3.forward);
			}
			else
			{
				zero.y += MTarget.ImpactHeight;
				Vector3 vector = ((!(MTarget.GameObject != null)) ? (MTarget.Position + zero) : MTarget.GameObject.transform.TransformPoint(zero));
				if (!_isHit)
				{
					Vector3 vector2 = vector - projectilePosition;
					vector2.y = 0f;
					Vector3 vector3 = new Vector3(vector2.z, 0f, 0f - vector2.x);
					vector3.Normalize();
					vector += vector3 * TargetRadius;
				}
				direction = (vector - projectilePosition).normalized;
			}
			if (MilMo_Level.CurrentLevel == null)
			{
				return;
			}
			new MilMo_LevelProjectile(_mFullLevelName, projectilePosition, direction, weapon.Template.ProjectileTemplate, (_isHit && MTarget != null && MTarget.GameObject != null) ? MTarget.GameObject.transform : null, zero, this, (MAttacker != null) ? MAttacker.Id : "-1", delegate(bool success, MilMo_LevelObject projectileLvlObj)
			{
				if (MilMo_Level.CurrentLevel != null)
				{
					MilMo_Level.CurrentLevel.AddPlayerProjectile(success, projectileLvlObj);
				}
			});
		}
	}

	public override void Resolve(MilMo_LevelProjectile projectile)
	{
		if (MTarget == null || !_isHit)
		{
			return;
		}
		if (projectile != null)
		{
			MTarget.DamageEffectLocal(projectile, MDamage);
		}
		MTarget.Damage(MDamage);
		if (MIsKillingBlow)
		{
			if (MTarget.ShouldBeKilled)
			{
				MTarget.Kill();
			}
			else
			{
				MTarget.ShouldBeKilled = true;
			}
		}
	}
}
