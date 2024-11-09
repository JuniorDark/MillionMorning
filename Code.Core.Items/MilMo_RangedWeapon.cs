using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.Visual.Effect;
using Code.World.Level;
using UnityEngine;

namespace Code.Core.Items;

public class MilMo_RangedWeapon : MilMo_Weapon
{
	public new MilMo_RangedWeaponTemplate Template => ((MilMo_Item)this).Template as MilMo_RangedWeaponTemplate;

	public override float AttackMovementSpeedModifier
	{
		get
		{
			if (!MilMo_Level.CurrentLevel.IsPvp())
			{
				return 0.9f;
			}
			return 0.5f;
		}
	}

	public MilMo_RangedWeapon(MilMo_RangedWeaponTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public Vector3 GetProjectilePosition()
	{
		Transform transform = null;
		if (base.GameObject != null)
		{
			transform = base.GameObject.transform;
		}
		else
		{
			if (Wielder == null)
			{
				Debug.Log("Couldn't calculate projectile position: weapon wielder is null");
				return Vector3.zero;
			}
			if (base.BodyPack != null)
			{
				transform = base.BodyPack.GetFirstAddonTransform(Wielder.Renderer);
			}
		}
		if (transform == null)
		{
			Debug.Log("Failed to get bodypack transform when calculating projectile position");
			if (Wielder == null || !(Wielder.GameObject != null))
			{
				return Vector3.zero;
			}
			transform = Wielder.GameObject.transform;
		}
		return transform.position + transform.forward * Template.ProjectileSpawnOffset.z + transform.right * Template.ProjectileSpawnOffset.x + transform.up * Template.ProjectileSpawnOffset.y;
	}

	public override MilMo_Effect Attack()
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_RangedAttack", "");
		return base.Attack();
	}
}
