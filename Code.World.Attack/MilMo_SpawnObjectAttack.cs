using Code.Core.Avatar;
using Code.World.Level.LevelObject;
using UnityEngine;

namespace Code.World.Attack;

public class MilMo_SpawnObjectAttack : MilMo_CreatureAttack
{
	private MilMo_SpawnObjectAttackTemplate Template => MAttackTemplate as MilMo_SpawnObjectAttackTemplate;

	public MilMo_SpawnObjectAttack(MilMo_MovableObject attacker, MilMo_Avatar target, MilMo_SpawnObjectAttackTemplate attackTemplate, float healthLeft)
		: base(attacker, target, attackTemplate, isHit: false, 0f, 0f, healthLeft)
	{
		Transform target2 = null;
		if (target != null && target.GameObject != null)
		{
			target2 = target.GameObject.transform;
		}
		attacker.Attack(Template, target2);
		if (MTarget != null && MAttacker != null && MAttacker.IsDangerous())
		{
			MTarget.ResetCombatTimer();
		}
	}
}
