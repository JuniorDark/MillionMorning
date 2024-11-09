using System.Collections.Generic;
using Code.Core.Avatar;
using Code.Core.Network.types;
using Code.Core.Template;
using Code.Core.Utility;
using Code.World.Level.LevelObject;
using UnityEngine;

namespace Code.World.Attack;

public abstract class MilMo_CreatureAttackTemplate : MilMo_Template
{
	protected Vector3 MOffset = new Vector3(0f, 0f, 0f);

	protected List<string> MAttackEffects = new List<string>();

	protected List<string> MPreparationEffects = new List<string>();

	protected MilMo_TemplateContainer.TemplateArrivedCallback MFullyLoadedCallback;

	public Vector3 Offset => MOffset;

	public List<string> AttackEffects => MAttackEffects;

	public List<string> PreparationEffects => MPreparationEffects;

	public abstract List<MilMo_Damage> Damage { get; }

	protected MilMo_CreatureAttackTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public override bool LoadFromNetwork(Template t)
	{
		TemplateCreatureAttack templateCreatureAttack = (TemplateCreatureAttack)t;
		MAttackEffects = (List<string>)templateCreatureAttack.GetAttackEffects();
		MPreparationEffects = (List<string>)templateCreatureAttack.GetPreparationEffects();
		MOffset = new Vector3(templateCreatureAttack.GetOffset().GetX(), templateCreatureAttack.GetOffset().GetY(), templateCreatureAttack.GetOffset().GetZ());
		return true;
	}

	public static void GetCreatureAttackTemplate(TemplateReference reference, MilMo_TemplateContainer.TemplateArrivedCallback callback)
	{
		MilMo_TemplateContainer.Get().GetTemplate(reference, delegate(MilMo_Template template, bool timeout)
		{
			if (template == null || timeout)
			{
				callback(template, timeout);
			}
			else if (!(template is MilMo_CreatureAttackTemplate milMo_CreatureAttackTemplate))
			{
				callback(null, timeOut: false);
			}
			else
			{
				milMo_CreatureAttackTemplate.RegisterFullyLoadedCallback(callback);
			}
		});
	}

	public abstract MilMo_CreatureAttack Instantiate(MilMo_MovableObject attacker, MilMo_Avatar target, bool isHit, float healthDamage, float armorDamage, float healthLeft);

	public abstract MilMo_CreatureAttack Instantiate(MilMo_MovableObject attacker, List<MilMo_CreatureAttack.MilMo_DamageToPlayer> playersHit);

	public virtual void RegisterFullyLoadedCallback(MilMo_TemplateContainer.TemplateArrivedCallback callback)
	{
		MFullyLoadedCallback = callback;
		MFullyLoadedCallback(this, timeOut: false);
	}
}
