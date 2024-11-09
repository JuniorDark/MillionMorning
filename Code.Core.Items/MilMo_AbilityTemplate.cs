using System.Collections.Generic;
using Code.Core.Network.types;
using Code.Core.Template;
using Core;

namespace Code.Core.Items;

public class MilMo_AbilityTemplate : MilMo_ItemTemplate
{
	public float Cooldown { get; private set; }

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		AbilityTemplate abilityTemplate = t as AbilityTemplate;
		base.LoadFromNetwork((Code.Core.Network.types.Template)abilityTemplate);
		if (abilityTemplate == null)
		{
			return false;
		}
		abilityTemplate.GetActivationEvent();
		abilityTemplate.GetActivationKey();
		abilityTemplate.GetActivationTime();
		abilityTemplate.GetDuration();
		abilityTemplate.GetReactivationTime();
		Cooldown = abilityTemplate.GetCooldown();
		foreach (TemplateReference item in abilityTemplate.GetOnActivation())
		{
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(item, OnActivationTemplateArrived);
		}
		foreach (TemplateReference item2 in abilityTemplate.GetOnEquip())
		{
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(item2, OnEquipTemplateArrived);
		}
		return true;
	}

	public static MilMo_AbilityTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_AbilityTemplate(category, path, filePath, "Ability");
	}

	private MilMo_AbilityTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_Ability(this, modifiers);
	}

	private static void OnEquipTemplateArrived(MilMo_Template template, bool timeout)
	{
	}

	private static void OnActivationTemplateArrived(MilMo_Template template, bool timeout)
	{
	}
}
