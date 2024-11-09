using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Items;

public class MilMo_SkillTemplate : MilMo_ItemTemplate
{
	public new string Name { get; private set; }

	public string Desc { get; private set; }

	public string Icon { get; private set; }

	public float Cooldown { get; private set; }

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		SkillItemTemplate skillItemTemplate = t as SkillItemTemplate;
		base.LoadFromNetwork((Code.Core.Network.types.Template)skillItemTemplate);
		if (skillItemTemplate == null)
		{
			return false;
		}
		Name = skillItemTemplate.GetSkillMode().GetName();
		Desc = skillItemTemplate.GetSkillMode().GetDesc();
		Icon = skillItemTemplate.GetSkillMode().GetIcon();
		Cooldown = skillItemTemplate.GetSkillMode().GetCooldown();
		return true;
	}

	public static MilMo_SkillTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_SkillTemplate(category, path, filePath, "SkillItem");
	}

	protected MilMo_SkillTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public override MilMo_Item Instantiate(Dictionary<string, string> modifiers)
	{
		return new MilMo_SkillItem(this, modifiers);
	}
}
