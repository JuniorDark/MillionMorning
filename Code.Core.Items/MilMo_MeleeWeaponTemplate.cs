using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Network.types;
using Code.Core.Utility;

namespace Code.Core.Items;

public class MilMo_MeleeWeaponTemplate : MilMo_WeaponTemplate
{
	private float _range;

	private float _impact;

	private readonly List<MilMo_Damage> _damage = new List<MilMo_Damage>();

	private bool _isCollect;

	public override float Range => _range;

	public short Spread { get; protected set; }

	public float HitTime { get; protected set; }

	public override float Impact => _impact;

	public override List<MilMo_Damage> Damage => _damage;

	public override float NormalDamage => (from damage in _damage
		where damage.DamageType.Equals("Normal") || damage.DamageType.Equals("Projectile")
		select damage.Value).FirstOrDefault();

	public override float MagicDamage => (from damage in _damage
		where damage.DamageType.Equals("Magic")
		select damage.Value).FirstOrDefault();

	public override bool IsCollect => _isCollect;

	public string Trail { get; private set; }

	public override float PrioritizedTargetingAngle => 360f;

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		MeleeWeaponTemplate meleeWeaponTemplate = t as MeleeWeaponTemplate;
		base.LoadFromNetwork((Code.Core.Network.types.Template)meleeWeaponTemplate);
		if (meleeWeaponTemplate == null)
		{
			return false;
		}
		_range = meleeWeaponTemplate.GetRange();
		Spread = meleeWeaponTemplate.GetSpread();
		HitTime = meleeWeaponTemplate.GetHitTime();
		_impact = meleeWeaponTemplate.GetImpact();
		Trail = meleeWeaponTemplate.GetTrail();
		IList<Damage> damages = meleeWeaponTemplate.GetDamages();
		bool flag = false;
		bool flag2 = false;
		foreach (Damage item in damages)
		{
			_damage.Add(new MilMo_Damage(item.GetTemplateType(), item.GetDamage()));
			if (item.GetTemplateType().Equals("Collect", StringComparison.InvariantCultureIgnoreCase))
			{
				flag = true;
			}
			else
			{
				flag2 = true;
			}
		}
		_isCollect = flag && !flag2;
		AutoAimRange = _range * 3f;
		string animation = meleeWeaponTemplate.GetAnimation();
		for (int i = 1; i < 4; i++)
		{
			base.AttackAnimations.Add(animation + "0" + i);
			base.JumpAttackAnimations.Add(animation + "Jump0" + i);
		}
		base.AttackIdleAnimation = animation + "Idle01";
		base.ReadyAnimation = animation + "Ready01";
		return true;
	}

	public new static MilMo_MeleeWeaponTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_MeleeWeaponTemplate(category, path, filePath);
	}

	protected MilMo_MeleeWeaponTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "MeleeWeapon")
	{
		Trail = "";
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_MeleeWeapon(this, modifiers);
	}

	public override float GetAutoAimRange(float targetRadius)
	{
		return AutoAimRange;
	}

	public override float GetHitRange(float targetRadius)
	{
		return _range;
	}
}
