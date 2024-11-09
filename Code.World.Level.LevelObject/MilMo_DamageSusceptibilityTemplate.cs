using System.Collections.Generic;
using Code.Core.Network.types;
using Code.Core.Template;

namespace Code.World.Level.LevelObject;

public class MilMo_DamageSusceptibilityTemplate : MilMo_Template
{
	private readonly Dictionary<string, float> _susceptibilities = new Dictionary<string, float>();

	private MilMo_DamageSusceptibilityTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "DamageSusceptibility")
	{
	}

	public override bool LoadFromNetwork(Template t)
	{
		foreach (Damage damage2 in ((TemplateDamageSusceptibility)t).GetDamages())
		{
			string templateType = damage2.GetTemplateType();
			float damage = damage2.GetDamage();
			_susceptibilities.Add(templateType, damage);
		}
		return true;
	}

	public float GetSusceptibility(string damageType)
	{
		if (string.IsNullOrEmpty(damageType) || !_susceptibilities.TryGetValue(damageType, out var value))
		{
			return 0f;
		}
		return value;
	}

	public static MilMo_DamageSusceptibilityTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_DamageSusceptibilityTemplate(category, path, filePath);
	}
}
