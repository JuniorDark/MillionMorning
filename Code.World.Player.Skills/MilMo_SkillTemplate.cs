using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.World.Player.Skills;

public class MilMo_SkillTemplate
{
	public List<MilMo_Skill> SkillModes { get; private set; }

	public string Name { get; private set; }

	public sbyte Level { get; private set; }

	public MilMo_SkillTemplate(SkillTemplate template)
	{
		Load(template);
	}

	private void Load(SkillTemplate t)
	{
		if (t == null)
		{
			return;
		}
		SkillModes = new List<MilMo_Skill>();
		Name = t.GetClassName();
		Level = t.GetLevel();
		IList<SkillMode> skillModes = t.GetSkillModes();
		if (skillModes != null)
		{
			for (int i = 0; i < skillModes.Count; i++)
			{
				MilMo_Skill milMo_Skill = new MilMo_Skill(skillModes[i].GetName(), skillModes[i].GetDesc(), skillModes[i].GetCooldown(), skillModes[i].GetIcon(), Name, Level, (sbyte)i);
				milMo_Skill.Template = this;
				SkillModes.Add(milMo_Skill);
			}
		}
	}
}
