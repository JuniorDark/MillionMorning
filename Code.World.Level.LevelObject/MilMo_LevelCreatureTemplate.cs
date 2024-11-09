using System.Collections.Generic;
using System.Linq;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.Core.Utility;
using Code.World.Attack;
using Code.World.CreatureMover;
using Core;
using UnityEngine;

namespace Code.World.Level.LevelObject;

public class MilMo_LevelCreatureTemplate : MilMo_MovableObjectTemplate
{
	protected MilMo_DamageSusceptibilityTemplate DamageSusceptibility;

	public string Mover { get; protected set; }

	public float Velocity { get; protected set; }

	public float AggroVelocity { get; protected set; }

	public string ImpactMoverType { get; protected set; }

	public float Pull { get; protected set; }

	public float Drag { get; protected set; }

	public float TurnSpeed { get; protected set; }

	public AudioClip DamageSound { get; protected set; }

	public AudioClip NoDamageSound { get; protected set; }

	public AudioClip AggroSound { get; protected set; }

	public List<MilMo_CreatureAttackTemplate> Attacks { get; protected set; }

	public bool IsDangerous { get; protected set; }

	private MilMo_LevelCreatureTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "Creature")
	{
		Attacks = new List<MilMo_CreatureAttackTemplate>();
	}

	public override bool LoadFromNetwork(Template t)
	{
		if (!base.LoadFromNetwork(t))
		{
			return false;
		}
		if (!(t is TemplateCreature templateCreature))
		{
			return false;
		}
		Mover = templateCreature.GetMover();
		Velocity = templateCreature.GetVelocity();
		AggroVelocity = templateCreature.GetAggroVelocity();
		ImpactMoverType = templateCreature.GetImpactMover();
		Pull = templateCreature.GetPull();
		Drag = templateCreature.GetDrag();
		TurnSpeed = templateCreature.GetTurnSpeed();
		LoadSoundsAsync(templateCreature);
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(templateCreature.GetDamageSusceptibility(), SusceptibilityTemplateLoaded);
		foreach (TemplateReference attack in templateCreature.GetAttacks())
		{
			MilMo_CreatureAttackTemplate.GetCreatureAttackTemplate(attack, AttackTemplateLoaded);
		}
		return true;
	}

	private async void LoadSoundsAsync(TemplateCreature creature)
	{
		string damageSound = creature.GetDamageSound();
		string noDamageSoundName = creature.GetNoDamageSound();
		string aggroSoundName = creature.GetAggroSound();
		if (!string.IsNullOrEmpty(damageSound))
		{
			DamageSound = await MilMo_ResourceManager.Instance.LoadAudioAsync(damageSound);
		}
		if (!string.IsNullOrEmpty(noDamageSoundName))
		{
			NoDamageSound = await MilMo_ResourceManager.Instance.LoadAudioAsync(noDamageSoundName);
		}
		if (!string.IsNullOrEmpty(aggroSoundName))
		{
			AggroSound = await MilMo_ResourceManager.Instance.LoadAudioAsync(aggroSoundName);
		}
	}

	public MilMo_CreatureMover GetMoverInstance()
	{
		return Mover switch
		{
			"Land" => new MilMo_LandMover(this), 
			"Bird" => new MilMo_BirdMover(this), 
			"Fish" => new MilMo_FishMover(this), 
			"Rabbit" => new MilMo_LandMover(this), 
			"Bee" => new MilMo_BeeMover(this), 
			_ => new MilMo_StandStillMover(this), 
		};
	}

	public bool HasKnockBack()
	{
		return Mover switch
		{
			"Land" => true, 
			"Bird" => false, 
			"Fish" => false, 
			"Rabbit" => false, 
			"Bee" => true, 
			_ => false, 
		};
	}

	public float GetSusceptibility(string damageType)
	{
		if (DamageSusceptibility == null || string.IsNullOrEmpty(damageType))
		{
			return 0f;
		}
		return DamageSusceptibility.GetSusceptibility(damageType);
	}

	public static MilMo_LevelCreatureTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_LevelCreatureTemplate(category, path, filePath);
	}

	private void SusceptibilityTemplateLoaded(MilMo_Template template, bool timedOut)
	{
		if (!timedOut)
		{
			DamageSusceptibility = template as MilMo_DamageSusceptibilityTemplate;
			if (DamageSusceptibility == null)
			{
				Debug.LogWarning("Failed to get damage susceptibility template for creature " + Path + " from the server.");
			}
		}
	}

	private void AttackTemplateLoaded(MilMo_Template template, bool timeout)
	{
		if (timeout)
		{
			return;
		}
		if (!(template is MilMo_CreatureAttackTemplate milMo_CreatureAttackTemplate))
		{
			Debug.LogWarning("Failed to get creature attack template for creature " + Path + " from the server.");
			return;
		}
		if (!IsDangerous)
		{
			using IEnumerator<MilMo_Damage> enumerator = milMo_CreatureAttackTemplate.Damage.Where((MilMo_Damage damage) => damage.Value > 0f).GetEnumerator();
			if (enumerator.MoveNext())
			{
				_ = enumerator.Current;
				IsDangerous = true;
			}
		}
		Attacks.Add(milMo_CreatureAttackTemplate);
	}
}
