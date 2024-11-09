using System;
using System.Collections.Generic;
using Code.Core.Network.types;
using Code.Core.Template;
using Code.Core.Utility;
using Code.World.Level.LevelObject;
using Core;
using UnityEngine;

namespace Code.Core.Items;

public class MilMo_RangedWeaponTemplate : MilMo_WeaponTemplate
{
	private bool _isCollect;

	public Vector3 ProjectileSpawnOffset { get; protected set; }

	public float FireTime { get; protected set; }

	public MilMo_ProjectileTemplate ProjectileTemplate { get; protected set; }

	public string WeaponAnimation { get; protected set; }

	public override bool IsCollect => _isCollect;

	public override List<MilMo_Damage> Damage
	{
		get
		{
			if (ProjectileTemplate != null)
			{
				return ProjectileTemplate.Damage;
			}
			return new List<MilMo_Damage>();
		}
	}

	public override float NormalDamage
	{
		get
		{
			if (ProjectileTemplate != null)
			{
				return ProjectileTemplate.NormalDamage;
			}
			return 0f;
		}
	}

	public override float MagicDamage
	{
		get
		{
			if (ProjectileTemplate != null)
			{
				return ProjectileTemplate.MagicDamage;
			}
			return 0f;
		}
	}

	public override float Range
	{
		get
		{
			if (ProjectileTemplate == null)
			{
				return 0f;
			}
			return ProjectileTemplate.Range;
		}
	}

	public override float PrioritizedTargetingAngle => 100f;

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		RangedWeaponTemplate rangedWeaponTemplate = t as RangedWeaponTemplate;
		base.LoadFromNetwork((Code.Core.Network.types.Template)rangedWeaponTemplate);
		if (rangedWeaponTemplate == null)
		{
			return false;
		}
		ProjectileSpawnOffset = new Vector3(rangedWeaponTemplate.GetProjectileSpawnOffset().GetX(), rangedWeaponTemplate.GetProjectileSpawnOffset().GetY(), rangedWeaponTemplate.GetProjectileSpawnOffset().GetZ());
		FireTime = rangedWeaponTemplate.GetFireTime();
		WeaponAnimation = rangedWeaponTemplate.GetWeaponAnimation();
		string animation = rangedWeaponTemplate.GetAnimation();
		base.AttackAnimations.Add(animation + "01");
		base.JumpAttackAnimations.Add(animation + "Jump01");
		base.AttackIdleAnimation = animation + "Idle01";
		base.ReadyAnimation = animation + "Ready01";
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(rangedWeaponTemplate.GetProjectile(), ProjectileTemplateLoaded);
		return true;
	}

	public new static MilMo_RangedWeaponTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_RangedWeaponTemplate(category, path, filePath);
	}

	protected MilMo_RangedWeaponTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "RangedWeapon")
	{
		ProjectileSpawnOffset = Vector3.zero;
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_RangedWeapon(this, modifiers);
	}

	public void ProjectileTemplateLoaded(MilMo_Template template, bool timedOut)
	{
		if (timedOut)
		{
			return;
		}
		ProjectileTemplate = template as MilMo_ProjectileTemplate;
		if (ProjectileTemplate == null)
		{
			Debug.LogWarning("Failed to get projectile template for ranged weapon " + Path + " from the server.");
			return;
		}
		AutoAimRange = ProjectileTemplate.Range;
		bool flag = false;
		bool flag2 = false;
		foreach (MilMo_Damage item in ProjectileTemplate.Damage)
		{
			if (item.DamageType.Equals("Collect", StringComparison.InvariantCultureIgnoreCase))
			{
				flag = true;
			}
			else
			{
				flag2 = true;
			}
		}
		_isCollect = flag && !flag2;
	}

	public override float GetAutoAimRange(float targetRadius)
	{
		float num = ((targetRadius >= 2.5f) ? targetRadius : 1f);
		return AutoAimRange * num;
	}

	public override float GetHitRange(float targetRadius)
	{
		float num = ((ProjectileTemplate != null) ? ProjectileTemplate.Range : 0f);
		float num2 = ((targetRadius >= 2.5f) ? targetRadius : 1f);
		return num * num2;
	}
}
