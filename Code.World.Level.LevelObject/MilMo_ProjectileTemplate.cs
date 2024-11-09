using System.Collections.Generic;
using System.Linq;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.Core.Utility;
using UnityEngine;

namespace Code.World.Level.LevelObject;

public class MilMo_ProjectileTemplate : MilMo_Template
{
	public AudioClip ImpactSound { get; private set; }

	public string ImpactParticle { get; private set; }

	public string Trail { get; private set; }

	public bool HasGravity { get; private set; }

	public List<MilMo_Damage> Damage { get; }

	public float Speed { get; private set; }

	public float Range { get; private set; }

	public bool InstantHit { get; private set; }

	public List<string> SpawnEffects { get; private set; }

	public string VisualRep { get; private set; }

	public float NormalDamage => (from damage in Damage
		where damage.DamageType.Equals("Normal") || damage.DamageType.Equals("Projectile")
		select damage.Value).FirstOrDefault();

	public float MagicDamage => (from damage in Damage
		where damage.DamageType.Equals("Magic")
		select damage.Value).FirstOrDefault();

	public override bool LoadFromNetwork(Template t)
	{
		if (!(t is ProjectileTemplate projectileTemplate))
		{
			return false;
		}
		base.LoadFromNetwork(projectileTemplate);
		string impactSound = projectileTemplate.GetImpactSound();
		ImpactParticle = projectileTemplate.GetImpactParticle();
		Trail = projectileTemplate.GetTrail();
		HasGravity = projectileTemplate.GetHasGravity() != 0;
		Speed = projectileTemplate.GetSpeed();
		Range = projectileTemplate.GetRange();
		InstantHit = projectileTemplate.GetHitScan() != 0;
		SpawnEffects = (List<string>)projectileTemplate.GetSpawnEffects();
		VisualRep = projectileTemplate.GetVisualRep();
		foreach (Damage item in projectileTemplate.GetDamage())
		{
			Damage.Add(new MilMo_Damage(item.GetTemplateType(), item.GetDamage()));
		}
		if (!string.IsNullOrEmpty(impactSound))
		{
			LoadImpactSoundAsync(impactSound);
		}
		return true;
	}

	private async void LoadImpactSoundAsync(string impactSoundName)
	{
		ImpactSound = await MilMo_ResourceManager.Instance.LoadAudioAsync(impactSoundName);
	}

	public static MilMo_ProjectileTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_ProjectileTemplate(category, path, filePath, "Projectile");
	}

	private MilMo_ProjectileTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
		SpawnEffects = new List<string>();
		Damage = new List<MilMo_Damage>();
		Trail = "";
		ImpactParticle = "";
	}
}
