using System.Collections.Generic;
using Code.Core.Network.types;
using Code.Core.Template;
using Code.World.Attack;
using Core;
using UnityEngine;

namespace Code.World.Level.LevelObject;

public class MilMo_BossModeTemplate : MilMo_Template
{
	protected List<MilMo_CreatureAttackTemplate> _attacks = new List<MilMo_CreatureAttackTemplate>();

	protected MilMo_DamageSusceptibilityTemplate _damageSusceptibility;

	public string LoopingAnimation { get; protected set; }

	public string HealingEffect { get; protected set; }

	public float Speed { get; protected set; }

	public float AggroSpeed { get; protected set; }

	public float TurnSpeed { get; protected set; }

	public List<string> DamageEffects { get; protected set; }

	public List<string> NoDamageEffects { get; protected set; }

	public List<string> AggroEffects { get; protected set; }

	public List<string> EnterEffects { get; protected set; }

	private MilMo_BossModeTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "BossMode")
	{
	}

	public override bool LoadFromNetwork(Template t)
	{
		BossModeTemplate bossModeTemplate = (BossModeTemplate)t;
		LoopingAnimation = bossModeTemplate.GetLoopingAnimation();
		HealingEffect = bossModeTemplate.GetHealingEffect();
		Speed = bossModeTemplate.GetSpeed();
		AggroSpeed = bossModeTemplate.GetAggroSpeed();
		TurnSpeed = bossModeTemplate.GetTurnSpeed();
		foreach (TemplateReference actionTemplate in bossModeTemplate.GetActionTemplates())
		{
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(actionTemplate, delegate
			{
			});
		}
		DamageEffects = (List<string>)bossModeTemplate.GetDamageEffects();
		NoDamageEffects = (List<string>)bossModeTemplate.GetNoDamageEffects();
		AggroEffects = (List<string>)bossModeTemplate.GetAggroEffects();
		EnterEffects = (List<string>)bossModeTemplate.GetEnterEffects();
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(bossModeTemplate.GetDamageSusceptibility(), SusceptibilityTemplateLoaded);
		return true;
	}

	public float GetSusceptibility(string damageType)
	{
		if (_damageSusceptibility == null || string.IsNullOrEmpty(damageType))
		{
			return 0f;
		}
		return _damageSusceptibility.GetSusceptibility(damageType);
	}

	public static MilMo_BossModeTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_BossModeTemplate(category, path, filePath);
	}

	private void SusceptibilityTemplateLoaded(MilMo_Template template, bool timedOut)
	{
		if (timedOut)
		{
			Debug.LogWarning("Failed to get damage susceptibility template for boss mode " + Path + " from the server.");
			return;
		}
		_damageSusceptibility = template as MilMo_DamageSusceptibilityTemplate;
		if (_damageSusceptibility == null)
		{
			Debug.LogWarning("Failed to get damage susceptibility template for boss mode " + Path + " from the server.");
		}
	}
}
