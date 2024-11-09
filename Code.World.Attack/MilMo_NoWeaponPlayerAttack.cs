using Code.Core.Avatar;
using Code.Core.EventSystem;
using Code.Core.Items;
using Code.World.Level.LevelObject;

namespace Code.World.Attack;

public class MilMo_NoWeaponPlayerAttack : MilMo_PlayerAttack
{
	public MilMo_NoWeaponPlayerAttack(MilMo_Avatar attacker, IMilMo_AttackTarget target, MilMo_Weapon weapon, bool isHit, float damage, bool isKillingBlow)
		: base(attacker, target, weapon, isHit, damage, isKillingBlow)
	{
		MilMo_EventSystem.At(0f, Resolve);
	}

	public override void Resolve()
	{
		if (MTarget == null || !_isHit)
		{
			return;
		}
		MTarget.DamageEffectLocal(MDamage);
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
