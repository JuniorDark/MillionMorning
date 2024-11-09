using System.Collections.Generic;
using Code.Core.Avatar;
using Code.Core.Avatar.Ragdoll;
using Code.Core.EventSystem;
using Code.Core.Utility;
using Code.World.Level;
using Code.World.Level.LevelInfo;
using Code.World.Level.LevelObject;
using Code.World.Player;
using UnityEngine;

namespace Code.World.Attack;

public class MilMo_MeleeCreatureAttack : MilMo_CreatureAttack
{
	private readonly List<MilMo_DamageToPlayer> _mDamagedPlayers;

	private MilMo_MeleeCreatureAttackTemplate Template => MAttackTemplate as MilMo_MeleeCreatureAttackTemplate;

	public MilMo_MeleeCreatureAttack(MilMo_MovableObject attacker, MilMo_Avatar target, MilMo_MeleeCreatureAttackTemplate attackTemplate, bool isHit, float healthDamage, float armorDamage, float healthLeft)
		: base(attacker, target, attackTemplate, isHit, healthDamage, armorDamage, healthLeft)
	{
		MilMo_EventSystem.At(attackTemplate.ImpactDelay, Resolve);
	}

	public MilMo_MeleeCreatureAttack(MilMo_MovableObject attacker, List<MilMo_DamageToPlayer> damagedPlayers, MilMo_MeleeCreatureAttackTemplate attackTemplate)
		: base(attacker, null, attackTemplate, isHit: false, 0f, 0f, 0f)
	{
		_mDamagedPlayers = damagedPlayers;
		attacker.Attack(Template, null);
		MilMo_EventSystem.At(attackTemplate.ImpactDelay, Resolve);
	}

	public override void Resolve()
	{
		if (MAttacker != null && Template != null)
		{
			MAttacker.PlaySound(Template.ImpactSound);
		}
		if (MTarget == null && _mDamagedPlayers == null)
		{
			return;
		}
		MilMo_Avatar mTarget = MTarget;
		if (mTarget != null && mTarget.Health <= 0f)
		{
			return;
		}
		if (MTarget != null)
		{
			if (MAttacker != null && MAttacker.IsDangerous())
			{
				MTarget.ResetCombatTimer();
			}
			if (_isHit)
			{
				PlayerHit(MTarget, MHealthDamage, MArmorDamage, MHealthLeft);
			}
			return;
		}
		foreach (MilMo_DamageToPlayer mDamagedPlayer in _mDamagedPlayers)
		{
			if (mDamagedPlayer != null && mDamagedPlayer.MAvatar != null)
			{
				if (MAttacker != null && MAttacker.IsDangerous())
				{
					mDamagedPlayer.MAvatar.ResetCombatTimer();
				}
				PlayerHit(mDamagedPlayer.MAvatar, mDamagedPlayer.MHealthDamage, mDamagedPlayer.MArmorDamage, mDamagedPlayer.MHealthLeft);
			}
		}
	}

	private void PlayerHit(MilMo_Avatar avatar, float healthDamage, float armorDamage, float healthLeft)
	{
		if (avatar == null)
		{
			return;
		}
		avatar.UpdateHealth(healthLeft);
		if (avatar.Player.IsLocalPlayer)
		{
			MilMo_Player.Instance.Avatar.Armor.DamageArmor(armorDamage);
			if (avatar.GameObject != null)
			{
				Vector3 impulse = ((MAttacker == null || MilMo_Utility.Equals(avatar.Position, MAttacker.Position)) ? avatar.GameObject.transform.forward : (avatar.Position - MAttacker.Position).normalized);
				impulse *= Template.PlayerImpulse.x;
				impulse.y = Template.PlayerImpulse.y;
				MilMo_PlayerControllerBase.AddKnockBack(impulse);
			}
		}
		if (MAttacker != null)
		{
			avatar.Damaged(healthDamage, MAttacker.Position);
		}
		if (avatar.Health <= 0f)
		{
			PVPDefeat(avatar);
		}
	}

	private void PVPDefeat(MilMo_Avatar avatar)
	{
		if (MilMo_Level.CurrentLevel != null && MilMo_LevelInfo.IsPvp(MilMo_Level.CurrentLevel.VerboseName) && avatar.GameObject != null)
		{
			float magnitude = Template.PlayerImpulse.magnitude;
			Vector3 force = ((MAttacker == null || MilMo_Utility.Equals(avatar.Position, MAttacker.Position)) ? (avatar.GameObject.transform.forward * magnitude) : ((avatar.Position - MAttacker.Position).normalized * magnitude));
			avatar.EnableRagdoll(force, ForcePosition.Torso);
		}
	}
}
