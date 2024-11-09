using System.Collections.Generic;
using Code.Core.Network.types;
using Code.Core.Utility;

namespace Code.Core.Items;

public abstract class MilMo_WeaponTemplate : MilMo_WieldableTemplate
{
	protected float AutoAimRange;

	public List<string> AttackAnimations { get; protected set; }

	public List<string> JumpAttackAnimations { get; protected set; }

	public string ReadyAnimation { get; protected set; }

	public string AttackIdleAnimation { get; protected set; }

	public float AnimationSequenceTimeout { get; protected set; }

	public string WeaponType { get; protected set; }

	public string AmmoType { get; protected set; }

	public bool NeedAmmo => AmmoType.Length > 0;

	public short AmmoAmount { get; protected set; }

	public virtual bool IsCollect => false;

	public virtual float Impact => 0f;

	public abstract List<MilMo_Damage> Damage { get; }

	public abstract float NormalDamage { get; }

	public abstract float MagicDamage { get; }

	public abstract float Range { get; }

	public abstract float PrioritizedTargetingAngle { get; }

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		WeaponTemplate weaponTemplate = t as WeaponTemplate;
		base.LoadFromNetwork((Code.Core.Network.types.Template)weaponTemplate);
		if (weaponTemplate == null)
		{
			return false;
		}
		AnimationSequenceTimeout = weaponTemplate.GetAnimationSequenceTimeout();
		WeaponType = weaponTemplate.GetWeaponType();
		AmmoType = weaponTemplate.GetAmmoType();
		AmmoAmount = weaponTemplate.GetAmmoAmount();
		return true;
	}

	protected MilMo_WeaponTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
		AmmoAmount = 1;
		AmmoType = "";
		WeaponType = "";
		JumpAttackAnimations = new List<string>();
		AttackAnimations = new List<string>();
	}

	public abstract float GetAutoAimRange(float targetRadius);

	public abstract float GetHitRange(float targetRadius);
}
