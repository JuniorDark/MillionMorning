using System.Collections.Generic;
using System.Linq;
using Code.World.Achievements;
using Code.World.Level.LevelInfo;
using Core;
using UI.Elements.Slot;

namespace Code.World.Level.LevelObject;

public class MilMo_ExplorationTokenEntry
{
	public MilMo_LevelInfoData Level;

	public List<bool> FoundTokens;

	public MilMo_Medal GetMedal()
	{
		return Singleton<MilMo_AchievementHandler>.Instance.GetExplorationTokenMedal(Level.FullLevelName);
	}

	public bool IsFinished()
	{
		return FoundTokens.All((bool token) => token);
	}

	public IEntryItem GetReward()
	{
		return Singleton<MilMo_AchievementHandler>.Instance.GetExplorationTokenMedalRewardItemAsync(Level.FullLevelName);
	}

	public List<bool> GetFoundTokens()
	{
		return FoundTokens;
	}

	public string GetLevelDisplayNameIdentifier()
	{
		return Level.DisplayName?.Identifier;
	}
}
