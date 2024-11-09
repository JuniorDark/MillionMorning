using System.Collections.Generic;
using Code.Core.Network.types;
using Code.Core.PlayerState;
using Code.Core.Template;
using Core;

namespace Code.Core.Items;

public class MilMo_OrbTemplate : MilMo_PowerUpTemplate
{
	public List<MilMo_PlayerStateTemplate> OnUse { get; private set; }

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		OrbTemplate orbTemplate = t as OrbTemplate;
		base.LoadFromNetwork((Code.Core.Network.types.Template)orbTemplate);
		if (orbTemplate == null)
		{
			return false;
		}
		foreach (TemplateReference item in orbTemplate.GetOnUse())
		{
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(item, OnUseTemplateArrived);
		}
		return true;
	}

	public static MilMo_OrbTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_OrbTemplate(category, path, filePath, "Orb");
	}

	protected MilMo_OrbTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
		OnUse = new List<MilMo_PlayerStateTemplate>();
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_Orb(this, modifiers);
	}

	public void OnUseTemplateArrived(MilMo_Template template, bool timeout)
	{
		if (!(template == null || !(template is MilMo_PlayerStateTemplate) || timeout))
		{
			OnUse.Add(template as MilMo_PlayerStateTemplate);
		}
	}
}
