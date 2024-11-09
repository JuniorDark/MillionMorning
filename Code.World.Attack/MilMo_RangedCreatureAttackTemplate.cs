using System.Collections.Generic;
using Code.Core.Avatar;
using Code.Core.Network.types;
using Code.Core.Template;
using Code.Core.Utility;
using Code.World.Level.LevelObject;

namespace Code.World.Attack;

public class MilMo_RangedCreatureAttackTemplate : MilMo_CreatureAttackTemplate
{
	protected MilMo_ProjectileTemplate MProjectile;

	public MilMo_ProjectileTemplate Projectile => MProjectile;

	public override List<MilMo_Damage> Damage
	{
		get
		{
			if (MProjectile == null)
			{
				return new List<MilMo_Damage>();
			}
			return MProjectile.Damage;
		}
	}

	protected MilMo_RangedCreatureAttackTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "RangedCreatureAttack")
	{
	}

	public override bool LoadFromNetwork(Template t)
	{
		base.LoadFromNetwork(t);
		RangedCreatureAttackTemplate rangedCreatureAttackTemplate = (RangedCreatureAttackTemplate)t;
		MilMo_TemplateContainer.Get().GetTemplate(rangedCreatureAttackTemplate.GetProjectile(), delegate(MilMo_Template template, bool timeOut)
		{
			if (timeOut || template == null)
			{
				if (MFullyLoadedCallback != null)
				{
					MFullyLoadedCallback(this, timeOut);
				}
			}
			else
			{
				MProjectile = template as MilMo_ProjectileTemplate;
				if (MFullyLoadedCallback != null)
				{
					MFullyLoadedCallback(this, timeOut: false);
				}
			}
		});
		return true;
	}

	public static MilMo_RangedCreatureAttackTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_RangedCreatureAttackTemplate(category, path, filePath);
	}

	public override MilMo_CreatureAttack Instantiate(MilMo_MovableObject attacker, MilMo_Avatar target, bool isHit, float healthDamage, float armorDamage, float healthLeft)
	{
		return new MilMo_RangedCreatureAttack(attacker, target, this, isHit, healthDamage, armorDamage, healthLeft);
	}

	public override MilMo_CreatureAttack Instantiate(MilMo_MovableObject attacker, List<MilMo_CreatureAttack.MilMo_DamageToPlayer> playersHit)
	{
		return null;
	}

	public override void RegisterFullyLoadedCallback(MilMo_TemplateContainer.TemplateArrivedCallback callback)
	{
		MFullyLoadedCallback = callback;
		if (MProjectile != null)
		{
			MFullyLoadedCallback(this, timeOut: false);
		}
	}
}
