using System;
using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.World.Quest.Conditions;

namespace Code.World.Quest;

public sealed class MilMo_Quest
{
	private readonly IList<MilMo_QuestRewardItem> _mRewards = new List<MilMo_QuestRewardItem>();

	private readonly IList<MilMo_QuestCondition> _mConditions = new List<MilMo_QuestCondition>();

	public int Id { get; private set; }

	public MilMo_LocString Name { get; private set; }

	public MilMo_LocString Description { get; private set; }

	public string TemplatePath { get; private set; }

	public bool IsGlobal { get; private set; }

	public QuestState State { get; private set; }

	public short RewardGems { get; private set; }

	public short RewardTelepods { get; private set; }

	public short RewardCoins { get; private set; }

	public int RewardExp { get; private set; }

	public bool IsTracked { get; private set; }

	public string FullLevelName { get; private set; }

	public string World => FullLevelName.Split(':')[0];

	public string Level => FullLevelName.Split(':')[1];

	public IList<MilMo_QuestRewardItem> RewardsItems => _mRewards;

	public IList<MilMo_QuestCondition> Conditions => _mConditions;

	public void Read(Code.Core.Network.types.Quest quest)
	{
		Id = quest.GetId();
		Name = MilMo_Localization.GetLocString(quest.GetTemplateName());
		Description = MilMo_Localization.GetLocString(quest.GetTemplateDescription());
		TemplatePath = quest.GetTemplatePath();
		FullLevelName = quest.GetLevelTemplateFullName();
		IsGlobal = quest.GetIsGlobal() == 1;
		byte state = (byte)quest.GetState();
		State = (QuestState)state;
		RewardGems = quest.GetRewardGems();
		RewardTelepods = quest.GetRewardTelepods();
		RewardCoins = quest.GetRewardCoins();
		RewardExp = quest.GetRewardExp();
		foreach (QuestRewardInfo rewardItem in quest.GetRewardItems())
		{
			_mRewards.Add(new MilMo_QuestRewardItem(rewardItem.GetRewardItemIdentifier(), rewardItem.GetAmount(), rewardItem.GetGender()));
		}
		foreach (Condition condition in quest.GetConditions())
		{
			_mConditions.Add(CreateCondition(condition));
		}
		IsTracked = quest.IsTracked();
	}

	public void UpdateCondition(short index, Condition condition)
	{
		if (index < 0 || index >= _mConditions.Count)
		{
			throw new ArgumentOutOfRangeException("Condition index is out of range: " + index + ", " + _mConditions.Count);
		}
		_mConditions[index] = CreateCondition(condition);
		MilMo_EventSystem.Instance.PostEvent("quest_updated", this);
	}

	private static MilMo_QuestCondition CreateCondition(Condition condition)
	{
		if (!(condition is ConditionArrivesAt condition2))
		{
			if (!(condition is ConditionArrivesAtAny condition3))
			{
				if (!(condition is ConditionCollectedAny condition4))
				{
					if (!(condition is ConditionCollectedGem condition5))
					{
						if (!(condition is ConditionKilledAny condition6))
						{
							if (!(condition is ConditionCollected condition7))
							{
								if (!(condition is ConditionWear condition8))
								{
									if (!(condition is ConditionKilled condition9))
									{
										if (!(condition is ConditionTalkTo condition10))
										{
											if (condition is ConditionTalkToAny condition11)
											{
												return new MilMo_TalkToAny(condition11);
											}
											if (condition != null)
											{
												return new MilMo_QuestCondition(condition);
											}
											throw new ArgumentException("Got invalid quest condition from server");
										}
										return new MilMo_TalkTo(condition10);
									}
									return new MilMo_Killed(condition9);
								}
								return new MilMo_Wear(condition8);
							}
							return new MilMo_Collected(condition7);
						}
						return new MilMo_KilledAny(condition6);
					}
					return new MilMo_CollectedGem(condition5);
				}
				return new MilMo_CollectedAny(condition4);
			}
			return new MilMo_ArrivesAtAny(condition3);
		}
		return new MilMo_ArrivesAt(condition2);
	}
}
