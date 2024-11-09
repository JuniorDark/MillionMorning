using Code.Core.Avatar;
using Code.World.Level.LevelObject;

namespace Code.World.Attack;

public abstract class MilMo_CreatureAttack : MilMo_Attack
{
	public class MilMo_DamageToPlayer
	{
		public MilMo_Avatar MAvatar;

		public float MHealthDamage;

		public float MArmorDamage;

		public float MHealthLeft;

		public MilMo_DamageToPlayer(MilMo_Avatar avatar, float healthDamage, float armorDamage, float healthLeft)
		{
			MAvatar = avatar;
			MHealthDamage = healthDamage;
			MArmorDamage = armorDamage;
		}
	}

	protected MilMo_MovableObject MAttacker;

	protected float MHealthDamage;

	protected float MArmorDamage;

	protected float MHealthLeft;

	protected MilMo_CreatureAttackTemplate MAttackTemplate;

	protected MilMo_Avatar MTarget;

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

	protected MilMo_CreatureAttack(MilMo_MovableObject attacker, MilMo_Avatar target, MilMo_CreatureAttackTemplate attackTemplate, bool isHit, float healthDamage, float armorDamage, float healthLeft)
		: base(isHit)
	{
		MAttacker = attacker;
		MTarget = target;
		MAttackTemplate = attackTemplate;
		MHealthDamage = healthDamage;
		MArmorDamage = armorDamage;
		MHealthLeft = healthLeft;
	}
}
