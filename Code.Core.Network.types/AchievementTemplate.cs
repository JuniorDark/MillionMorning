using System.Collections.Generic;

namespace Code.Core.Network.types;

public class AchievementTemplate : ItemTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new AchievementTemplate(reader);
		}
	}

	private readonly string _category;

	private readonly int _categoryIndex;

	private readonly IList<string> _worlds;

	private readonly IList<AchievementObjective> _objectives;

	private readonly string _notCompleteDescription;

	private readonly string _boyReward;

	private readonly int _boyRewardAmount;

	private readonly string _girlReward;

	private readonly int _girlRewardAmount;

	private const int TYPE_ID = 25;

	public override int GetTypeId()
	{
		return 25;
	}

	public AchievementTemplate(MessageReader reader)
		: base(reader)
	{
		_category = reader.ReadString();
		_categoryIndex = reader.ReadInt32();
		_worlds = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_worlds.Add(reader.ReadString());
		}
		_objectives = new List<AchievementObjective>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			_objectives.Add(new AchievementObjective(reader));
		}
		_notCompleteDescription = reader.ReadString();
		_boyReward = reader.ReadString();
		_boyRewardAmount = reader.ReadInt32();
		_girlReward = reader.ReadString();
		_girlRewardAmount = reader.ReadInt32();
	}

	public AchievementTemplate(string category, int categoryIndex, IList<string> worlds, IList<AchievementObjective> objectives, string notCompleteDescription, string boyReward, int boyRewardAmount, string girlReward, int girlRewardAmount, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_category = category;
		_categoryIndex = categoryIndex;
		_worlds = worlds;
		_objectives = objectives;
		_notCompleteDescription = notCompleteDescription;
		_boyReward = boyReward;
		_boyRewardAmount = boyRewardAmount;
		_girlReward = girlReward;
		_girlRewardAmount = girlRewardAmount;
	}

	public string GetCategory()
	{
		return _category;
	}

	public int GetCategoryIndex()
	{
		return _categoryIndex;
	}

	public IList<string> GetWorlds()
	{
		return _worlds;
	}

	public IList<AchievementObjective> GetObjectives()
	{
		return _objectives;
	}

	public string GetNotCompleteDescription()
	{
		return _notCompleteDescription;
	}

	public string GetBoyReward()
	{
		return _boyReward;
	}

	public int GetBoyRewardAmount()
	{
		return _boyRewardAmount;
	}

	public string GetGirlReward()
	{
		return _girlReward;
	}

	public int GetGirlRewardAmount()
	{
		return _girlRewardAmount;
	}

	public override int Size()
	{
		int num = 24 + base.Size();
		num += MessageWriter.GetSize(_category);
		num += (short)(2 * _worlds.Count);
		foreach (string world in _worlds)
		{
			num += MessageWriter.GetSize(world);
		}
		foreach (AchievementObjective objective in _objectives)
		{
			num += objective.Size();
		}
		num += MessageWriter.GetSize(_notCompleteDescription);
		num += MessageWriter.GetSize(_boyReward);
		return num + MessageWriter.GetSize(_girlReward);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_category);
		writer.WriteInt32(_categoryIndex);
		writer.WriteInt16((short)_worlds.Count);
		foreach (string world in _worlds)
		{
			writer.WriteString(world);
		}
		writer.WriteInt16((short)_objectives.Count);
		foreach (AchievementObjective objective in _objectives)
		{
			objective.Write(writer);
		}
		writer.WriteString(_notCompleteDescription);
		writer.WriteString(_boyReward);
		writer.WriteInt32(_boyRewardAmount);
		writer.WriteString(_girlReward);
		writer.WriteInt32(_girlRewardAmount);
	}
}
