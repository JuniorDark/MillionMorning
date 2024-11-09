using System.Collections.Generic;
using Code.Core.Network.types;
using Code.Core.PlayerState;
using Code.Core.Template;
using Core;

namespace Code.Core.Items;

public class MilMo_ConsumableTemplate : MilMo_ItemTemplate
{
	private bool _isAutoPickup;

	public float ActivationTime { get; private set; }

	public float Duration { get; private set; }

	public List<MilMo_PlayerStateTemplate> OnUse { get; private set; }

	public override bool IsAutoPickup => _isAutoPickup;

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		ConsumableTemplate consumableTemplate = t as ConsumableTemplate;
		base.LoadFromNetwork((Code.Core.Network.types.Template)consumableTemplate);
		if (consumableTemplate == null)
		{
			return false;
		}
		ActivationTime = consumableTemplate.GetActivationTime();
		Duration = consumableTemplate.GetDuration();
		_isAutoPickup = consumableTemplate.GetAutoPickup() != 0;
		foreach (TemplateReference item in consumableTemplate.GetOnUse())
		{
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(item, OnUseTemplateArrived);
		}
		return true;
	}

	public static MilMo_ConsumableTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_ConsumableTemplate(category, path, filePath, "Consumable");
	}

	protected MilMo_ConsumableTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
		OnUse = new List<MilMo_PlayerStateTemplate>();
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_Consumable(this, modifiers);
	}

	public void OnUseTemplateArrived(MilMo_Template template, bool timeout)
	{
		if (!(!(template is MilMo_PlayerStateTemplate) || timeout))
		{
			OnUse.Add((MilMo_PlayerStateTemplate)template);
		}
	}
}
