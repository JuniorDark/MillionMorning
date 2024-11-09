using System;
using System.Collections.Generic;
using Code.Core.Avatar;
using Code.Core.Items;
using Code.Core.Network.types;
using Code.Core.Utility;
using UnityEngine;

namespace Code.World.Level.LevelObject;

public interface IMilMo_AttackTarget
{
	float CollisionRadius { get; }

	Bounds BoundingBox { get; }

	GameObject GameObject { get; }

	Vector3 Position { get; }

	float ImpactRadius { get; }

	float ImpactRadiusSqr { get; }

	bool ShouldBeKilled { get; set; }

	float ImpactHeight { get; }

	float Health { get; }

	float MaxHealth { get; }

	string Name { get; }

	int AvatarLevel { get; }

	float MarkerYOffset { get; }

	event Action OnHealthChanged;

	float GetDamage(List<MilMo_Damage> damage);

	bool IsDeadOrDying();

	bool HasKnockBack();

	void Damage(float damage);

	bool IsDangerous();

	bool IsCritter();

	void DamageEffectLocal(MilMo_Avatar attacker, MilMo_MeleeWeapon hitWeapon, float damage);

	void DamageEffectLocal(MilMo_LevelProjectile projectile, float damage);

	void DamageEffectLocal(float damage);

	void Kill();

	AttackTarget AsNetworkAttackTarget();

	bool IsBoss();

	void Target();

	void UnTarget();
}
