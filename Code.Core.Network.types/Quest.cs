using System.Collections.Generic;

namespace Code.Core.Network.types;

public class Quest
{
	private readonly int _id;

	private readonly string _templatePath;

	private readonly string _templateName;

	private readonly string _templateDescription;

	private readonly string _levelTemplateFullName;

	private readonly sbyte _isGlobal;

	private readonly sbyte _state;

	private readonly short _rewardGems;

	private readonly short _rewardTelepods;

	private readonly short _rewardCoins;

	private readonly IList<QuestRewardInfo> _rewardItems;

	private readonly int _rewardExp;

	private readonly string _rewardSkill;

	private readonly IList<Condition> _conditions;

	private readonly bool _isTracked;

	public Quest(MessageReader reader)
	{
		_id = reader.ReadInt32();
		_templatePath = reader.ReadString();
		_templateName = reader.ReadString();
		_templateDescription = reader.ReadString();
		_levelTemplateFullName = reader.ReadString();
		_isGlobal = reader.ReadInt8();
		_state = reader.ReadInt8();
		_rewardGems = reader.ReadInt16();
		_rewardTelepods = reader.ReadInt16();
		_rewardCoins = reader.ReadInt16();
		_rewardItems = new List<QuestRewardInfo>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_rewardItems.Add(new QuestRewardInfo(reader));
		}
		_rewardExp = reader.ReadInt32();
		_rewardSkill = reader.ReadString();
		_conditions = new List<Condition>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			_conditions.Add(Condition.Create(reader.ReadTypeCode(), reader));
		}
		_isTracked = reader.ReadInt8() == 1;
	}

	public Quest(int id, string templatePath, string templateName, string templateDescription, string levelTemplateFullName, sbyte isGlobal, sbyte state, short rewardGems, short rewardTelepods, short rewardCoins, IList<QuestRewardInfo> rewardItems, int rewardExp, string rewardSkill, IList<Condition> conditions, bool isTracked)
	{
		_id = id;
		_templatePath = templatePath;
		_templateName = templateName;
		_templateDescription = templateDescription;
		_levelTemplateFullName = levelTemplateFullName;
		_isGlobal = isGlobal;
		_state = state;
		_rewardGems = rewardGems;
		_rewardTelepods = rewardTelepods;
		_rewardCoins = rewardCoins;
		_rewardItems = rewardItems;
		_rewardExp = rewardExp;
		_rewardSkill = rewardSkill;
		_conditions = conditions;
		_isTracked = isTracked;
	}

	public int GetId()
	{
		return _id;
	}

	public string GetTemplatePath()
	{
		return _templatePath;
	}

	public string GetTemplateName()
	{
		return _templateName;
	}

	public string GetTemplateDescription()
	{
		return _templateDescription;
	}

	public string GetLevelTemplateFullName()
	{
		return _levelTemplateFullName;
	}

	public sbyte GetIsGlobal()
	{
		return _isGlobal;
	}

	public sbyte GetState()
	{
		return _state;
	}

	public short GetRewardGems()
	{
		return _rewardGems;
	}

	public short GetRewardTelepods()
	{
		return _rewardTelepods;
	}

	public short GetRewardCoins()
	{
		return _rewardCoins;
	}

	public IList<QuestRewardInfo> GetRewardItems()
	{
		return _rewardItems;
	}

	public int GetRewardExp()
	{
		return _rewardExp;
	}

	public string GetRewardSkill()
	{
		return _rewardSkill;
	}

	public IList<Condition> GetConditions()
	{
		return _conditions;
	}

	public bool IsTracked()
	{
		return _isTracked;
	}

	public int Size()
	{
		int num = 31;
		num += MessageWriter.GetSize(_templatePath);
		num += MessageWriter.GetSize(_templateName);
		num += MessageWriter.GetSize(_templateDescription);
		num += MessageWriter.GetSize(_levelTemplateFullName);
		foreach (QuestRewardInfo rewardItem in _rewardItems)
		{
			num += rewardItem.Size();
		}
		num += MessageWriter.GetSize(_rewardSkill);
		num += (short)(_conditions.Count * 2);
		foreach (Condition condition in _conditions)
		{
			num += condition.Size();
		}
		return num;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteInt32(_id);
		writer.WriteString(_templatePath);
		writer.WriteString(_templateName);
		writer.WriteString(_templateDescription);
		writer.WriteString(_levelTemplateFullName);
		writer.WriteInt8(_isGlobal);
		writer.WriteInt8(_state);
		writer.WriteInt16(_rewardGems);
		writer.WriteInt16(_rewardTelepods);
		writer.WriteInt16(_rewardCoins);
		writer.WriteInt16((short)_rewardItems.Count);
		foreach (QuestRewardInfo rewardItem in _rewardItems)
		{
			rewardItem.Write(writer);
		}
		writer.WriteInt32(_rewardExp);
		writer.WriteString(_rewardSkill);
		writer.WriteInt16((short)_conditions.Count);
		foreach (Condition condition in _conditions)
		{
			writer.WriteTypeCode(condition.GetTypeId());
			condition.Write(writer);
		}
		writer.WriteInt8((sbyte)(_isTracked ? 1 : 0));
	}
}
