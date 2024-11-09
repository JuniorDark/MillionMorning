using System.Collections.Generic;
using Code.Core.Avatar;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using Code.World.Level.LevelObject;
using UnityEngine;

namespace Code.World.Attack;

public class MilMo_MeleeCreatureAttackTemplate : MilMo_CreatureAttackTemplate
{
	protected AudioClip MImpactSound;

	protected float MImpactDelay;

	protected Vector2 MPlayerImpulse;

	protected List<MilMo_Damage> MDamage = new List<MilMo_Damage>();

	public AudioClip ImpactSound => MImpactSound;

	public float ImpactDelay => MImpactDelay;

	public Vector2 PlayerImpulse => MPlayerImpulse;

	public override List<MilMo_Damage> Damage => MDamage;

	protected MilMo_MeleeCreatureAttackTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "MeleeCreatureAttack")
	{
	}

	public override bool LoadFromNetwork(Template t)
	{
		base.LoadFromNetwork(t);
		MeleeCreatureAttackTemplate meleeCreatureAttackTemplate = (MeleeCreatureAttackTemplate)t;
		MImpactDelay = meleeCreatureAttackTemplate.GetImpactDelay();
		MPlayerImpulse.x = meleeCreatureAttackTemplate.GetPlayerImpulseXz();
		MPlayerImpulse.y = meleeCreatureAttackTemplate.GetPlayerImpulseY();
		foreach (Damage item in meleeCreatureAttackTemplate.GetDamage())
		{
			MDamage.Add(new MilMo_Damage(item.GetTemplateType(), item.GetDamage()));
		}
		string impactSound = meleeCreatureAttackTemplate.GetImpactSound();
		if (!string.IsNullOrEmpty(impactSound))
		{
			LoadAudioAsync(impactSound);
		}
		return true;
	}

	private async void LoadAudioAsync(string path)
	{
		MImpactSound = await MilMo_ResourceManager.Instance.LoadAudioAsync(path);
	}

	public static MilMo_MeleeCreatureAttackTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_MeleeCreatureAttackTemplate(category, path, filePath);
	}

	public override MilMo_CreatureAttack Instantiate(MilMo_MovableObject attacker, MilMo_Avatar target, bool isHit, float healthDamage, float armorDamage, float healthLeft)
	{
		return new MilMo_MeleeCreatureAttack(attacker, target, this, isHit, healthDamage, armorDamage, healthLeft);
	}

	public override MilMo_CreatureAttack Instantiate(MilMo_MovableObject attacker, List<MilMo_CreatureAttack.MilMo_DamageToPlayer> playersHit)
	{
		return new MilMo_MeleeCreatureAttack(attacker, playersHit, this);
	}
}
