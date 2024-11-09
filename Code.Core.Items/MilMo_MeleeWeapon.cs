using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.Visual.Effect;
using UnityEngine;

namespace Code.Core.Items;

public class MilMo_MeleeWeapon : MilMo_Weapon
{
	public new MilMo_MeleeWeaponTemplate Template => ((MilMo_Item)this).Template as MilMo_MeleeWeaponTemplate;

	public override float AttackMovementSpeedModifier => 0.5f;

	public MilMo_MeleeWeapon(MilMo_MeleeWeaponTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public override MilMo_Effect Attack()
	{
		MilMo_Effect result = null;
		if (base.GameObject != null && !string.IsNullOrEmpty(Template.Trail))
		{
			Transform[] componentsInChildren = base.GameObject.GetComponentsInChildren<Transform>();
			foreach (Transform transform in componentsInChildren)
			{
				if (!(transform.name != "TrailNode"))
				{
					result = MilMo_EffectContainer.GetEffect(Template.Trail, transform.gameObject);
					break;
				}
			}
		}
		base.Attack();
		MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_MeleeAttack", "");
		return result;
	}
}
