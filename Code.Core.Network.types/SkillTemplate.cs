using System.Collections.Generic;

namespace Code.Core.Network.types;

public class SkillTemplate
{
	private readonly string _className;

	private readonly sbyte _level;

	private readonly IList<SkillMode> _skillModes;

	public SkillTemplate(MessageReader reader)
	{
		_className = reader.ReadString();
		_level = reader.ReadInt8();
		_skillModes = new List<SkillMode>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_skillModes.Add(new SkillMode(reader));
		}
	}

	public SkillTemplate(string className, sbyte level, IList<SkillMode> skillModes)
	{
		_className = className;
		_level = level;
		_skillModes = skillModes;
	}

	public string GetClassName()
	{
		return _className;
	}

	public sbyte GetLevel()
	{
		return _level;
	}

	public IList<SkillMode> GetSkillModes()
	{
		return _skillModes;
	}

	public int Size()
	{
		int num = 5;
		num += MessageWriter.GetSize(_className);
		foreach (SkillMode skillMode in _skillModes)
		{
			num += skillMode.Size();
		}
		return num;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_className);
		writer.WriteInt8(_level);
		writer.WriteInt16((short)_skillModes.Count);
		foreach (SkillMode skillMode in _skillModes)
		{
			skillMode.Write(writer);
		}
	}
}
