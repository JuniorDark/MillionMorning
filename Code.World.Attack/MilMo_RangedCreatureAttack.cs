using Code.Core.Avatar;
using Code.Core.EventSystem;
using Code.World.Level;
using Code.World.Level.LevelObject;
using Code.World.Player;
using UnityEngine;

namespace Code.World.Attack;

public class MilMo_RangedCreatureAttack : MilMo_CreatureAttack
{
	private MilMo_RangedCreatureAttackTemplate Template => MAttackTemplate as MilMo_RangedCreatureAttackTemplate;

	public MilMo_RangedCreatureAttack(MilMo_MovableObject attacker, MilMo_Avatar target, MilMo_RangedCreatureAttackTemplate attackTemplate, bool isHit, float healthDamage, float armorDamage, float healthLeft)
		: base(attacker, target, attackTemplate, isHit, healthDamage, armorDamage, healthLeft)
	{
		MilMo_EventSystem.At(0f, SpawnProjectile);
	}

	private void SpawnProjectile()
	{
		MilMo_RangedCreatureAttackTemplate template = Template;
		if (template?.Projectile == null || (template.Projectile.InstantHit && (MAttacker == null || MAttacker.GameObject == null)) || MilMo_Level.CurrentLevel == null)
		{
			Resolve(null);
			return;
		}
		if (MTarget != null && MAttacker != null && MAttacker.IsDangerous())
		{
			MTarget.ResetCombatTimer();
		}
		if (!template.Projectile.InstantHit || MAttacker == null)
		{
			return;
		}
		Vector3 vector = MAttacker.GameObject.transform.TransformPoint(Template.Offset);
		Vector3 zero = Vector3.zero;
		Vector3 direction;
		if (MTarget == null || MTarget.GameObject == null)
		{
			direction = MAttacker.GameObject.transform.forward;
		}
		else
		{
			zero.y += 0.5f;
			Vector3 vector2 = MTarget.GameObject.transform.TransformPoint(zero);
			if (!_isHit)
			{
				Vector3 vector3 = vector2 - vector;
				vector3.y = 0f;
				Vector3 vector4 = new Vector3(vector3.z, 0f, 0f - vector3.x);
				vector4.Normalize();
				vector2 += vector4 * TargetRadius;
			}
			direction = (vector2 - vector).normalized;
		}
		new MilMo_LevelProjectile(MAttacker.FullLevelName, vector, direction, template.Projectile, (_isHit && MTarget != null && MTarget.GameObject != null) ? MTarget.GameObject.transform : null, zero, this, (MAttacker != null) ? MAttacker.Id.ToString() : "-1", MilMo_Level.CurrentLevel.AddLevelProjectile);
	}

	public override void Resolve()
	{
		Resolve(null);
	}

	public override void Resolve(MilMo_LevelProjectile projectile)
	{
		MilMo_Player instance = MilMo_Player.Instance;
		MilMo_Avatar avatar = instance.Avatar;
		if (MTarget != null && MAttacker != null && MAttacker.IsDangerous())
		{
			MTarget.ResetCombatTimer();
		}
		if (!_isHit || MTarget == null || MTarget.Health <= 0f)
		{
			return;
		}
		if (MTarget.Id == avatar.Id)
		{
			avatar.UpdateHealth(MHealthLeft);
			instance.Avatar.Armor.DamageArmor(MArmorDamage);
		}
		else
		{
			MTarget.UpdateHealth(MHealthLeft);
		}
		if (!(MTarget.GameObject == null))
		{
			Vector3 pointOfOrigin = Vector3.zero;
			if (projectile != null)
			{
				pointOfOrigin = projectile.Position;
			}
			else if (MAttacker != null)
			{
				pointOfOrigin = MAttacker.Position;
			}
			else if (MTarget.HeadStart != null)
			{
				pointOfOrigin = MTarget.HeadStart.position;
			}
			MTarget.Damaged(MHealthDamage, pointOfOrigin);
		}
	}
}
