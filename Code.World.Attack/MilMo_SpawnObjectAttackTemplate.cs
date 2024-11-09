using System.Collections.Generic;
using Code.Core.Avatar;
using Code.Core.Network.types;
using Code.Core.Utility;
using Code.World.Level.LevelObject;

namespace Code.World.Attack;

public class MilMo_SpawnObjectAttackTemplate : MilMo_CreatureAttackTemplate
{
	public override List<MilMo_Damage> Damage => new List<MilMo_Damage>();

	protected MilMo_SpawnObjectAttackTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "SpawnObjectAttack")
	{
	}

	public override bool LoadFromNetwork(Template t)
	{
		base.LoadFromNetwork(t);
		return true;
	}

	public static MilMo_SpawnObjectAttackTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_SpawnObjectAttackTemplate(category, path, filePath);
	}

	public override MilMo_CreatureAttack Instantiate(MilMo_MovableObject attacker, MilMo_Avatar target, bool isHit, float healthDamage, float armorDamage, float healthLeft)
	{
		return new MilMo_SpawnObjectAttack(attacker, target, this, healthLeft);
	}

	public override MilMo_CreatureAttack Instantiate(MilMo_MovableObject attacker, List<MilMo_CreatureAttack.MilMo_DamageToPlayer> playersHit)
	{
		return new MilMo_SpawnObjectAttack(attacker, null, this, 0f);
	}
}
