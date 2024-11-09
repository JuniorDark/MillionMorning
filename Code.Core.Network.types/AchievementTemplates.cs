using System.Collections.Generic;

namespace Code.Core.Network.types;

public class AchievementTemplates
{
	private readonly IList<AchievementTemplate> _templates;

	public AchievementTemplates(MessageReader reader)
	{
		_templates = new List<AchievementTemplate>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_templates.Add(new AchievementTemplate(reader));
		}
	}

	public AchievementTemplates(IList<AchievementTemplate> templates)
	{
		_templates = templates;
	}

	public IList<AchievementTemplate> GetTemplates()
	{
		return _templates;
	}

	public int Size()
	{
		int num = 2;
		foreach (AchievementTemplate template in _templates)
		{
			num += template.Size();
		}
		return num;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteInt16((short)_templates.Count);
		foreach (AchievementTemplate template in _templates)
		{
			template.Write(writer);
		}
	}
}
