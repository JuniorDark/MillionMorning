using System.Collections.Generic;

namespace Code.Core.Network.types;

public class FullAvatar
{
	private readonly string _name;

	private readonly sbyte _gender;

	private readonly string _skinColor;

	private readonly int _hairColor;

	private readonly string _eyeColor;

	private readonly string _mouth;

	private readonly string _eyes;

	private readonly string _eyeBrows;

	private readonly string _hair;

	private readonly float _height;

	private readonly float _health;

	private readonly float _maxHealth;

	private readonly string _mood;

	private readonly string _title;

	private readonly int _gems;

	private readonly int _teleportStones;

	private readonly int _exp;

	private readonly int _avatarLevel;

	private readonly sbyte _unusedAvailableClassLevels;

	private readonly int _avatarLevelExpRequirement;

	private readonly int _nextAvatarLevelExpRequirement;

	private readonly IList<InventoryEntry> _items;

	private readonly IList<Item> _equippedItems;

	private readonly IList<Quest> _completedQuests;

	private readonly IList<Quest> _activeQuests;

	private readonly IList<AchievementTemplate> _achievementTemplates;

	private readonly IList<string> _achievements;

	private readonly IList<AchievementCounter> _counters;

	private readonly IList<AmmoType> _ammoTypes;

	private readonly IList<LevelStateData> _levelStates;

	private readonly IList<WorldLevel> _worldLevels;

	private readonly IList<FoundTokensInfo> _currentExplorationTokens;

	private readonly IList<FoundCoinTokensInfo> _currentCoinTokens;

	private readonly IList<PremiumToken> _premiumTokens;

	private readonly IList<string> _completedTutorials;

	private readonly IList<ActionTime> _lastActionTimes;

	private readonly IList<int> _seenHotItemsHashCodes;

	private readonly IList<int> _readNewsHashCodes;

	private readonly long _millisecondsToNextHomeDelivery;

	private readonly TemplateReference _nextHomeDeliveryBox;

	private readonly IList<SelectedClass> _selectedClasses;

	private readonly IList<SkillTemplate> _skillTemplates;

	private readonly IList<string> _playersBannedFromHome;

	public FullAvatar(MessageReader reader)
	{
		_name = reader.ReadString();
		_gender = reader.ReadInt8();
		_skinColor = reader.ReadString();
		_hairColor = reader.ReadInt32();
		_eyeColor = reader.ReadString();
		_mouth = reader.ReadString();
		_eyes = reader.ReadString();
		_eyeBrows = reader.ReadString();
		_hair = reader.ReadString();
		_height = reader.ReadFloat();
		_health = reader.ReadFloat();
		_maxHealth = reader.ReadFloat();
		_mood = reader.ReadString();
		_title = reader.ReadString();
		_gems = reader.ReadInt32();
		_teleportStones = reader.ReadInt32();
		_exp = reader.ReadInt32();
		_avatarLevel = reader.ReadInt32();
		_unusedAvailableClassLevels = reader.ReadInt8();
		_avatarLevelExpRequirement = reader.ReadInt32();
		_nextAvatarLevelExpRequirement = reader.ReadInt32();
		_items = new List<InventoryEntry>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			InventoryEntry item = InventoryEntry.Create(reader.ReadTypeCode(), reader);
			_items.Add(item);
		}
		_equippedItems = new List<Item>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			_equippedItems.Add(Item.Create(reader.ReadTypeCode(), reader));
		}
		_completedQuests = new List<Quest>();
		short num5 = reader.ReadInt16();
		for (short num6 = 0; num6 < num5; num6++)
		{
			_completedQuests.Add(new Quest(reader));
		}
		_activeQuests = new List<Quest>();
		short num7 = reader.ReadInt16();
		for (short num8 = 0; num8 < num7; num8++)
		{
			_activeQuests.Add(new Quest(reader));
		}
		_achievementTemplates = new List<AchievementTemplate>();
		short num9 = reader.ReadInt16();
		for (short num10 = 0; num10 < num9; num10++)
		{
			_achievementTemplates.Add(new AchievementTemplate(reader));
		}
		_achievements = new List<string>();
		short num11 = reader.ReadInt16();
		for (short num12 = 0; num12 < num11; num12++)
		{
			_achievements.Add(reader.ReadString());
		}
		_counters = new List<AchievementCounter>();
		short num13 = reader.ReadInt16();
		for (short num14 = 0; num14 < num13; num14++)
		{
			_counters.Add(new AchievementCounter(reader));
		}
		_ammoTypes = new List<AmmoType>();
		short num15 = reader.ReadInt16();
		for (short num16 = 0; num16 < num15; num16++)
		{
			_ammoTypes.Add(new AmmoType(reader));
		}
		_levelStates = new List<LevelStateData>();
		short num17 = reader.ReadInt16();
		for (short num18 = 0; num18 < num17; num18++)
		{
			_levelStates.Add(new LevelStateData(reader));
		}
		_worldLevels = new List<WorldLevel>();
		short num19 = reader.ReadInt16();
		for (short num20 = 0; num20 < num19; num20++)
		{
			_worldLevels.Add(new WorldLevel(reader));
		}
		_currentExplorationTokens = new List<FoundTokensInfo>();
		short num21 = reader.ReadInt16();
		for (short num22 = 0; num22 < num21; num22++)
		{
			_currentExplorationTokens.Add(new FoundTokensInfo(reader));
		}
		_currentCoinTokens = new List<FoundCoinTokensInfo>();
		short num23 = reader.ReadInt16();
		for (short num24 = 0; num24 < num23; num24++)
		{
			_currentCoinTokens.Add(new FoundCoinTokensInfo(reader));
		}
		_premiumTokens = new List<PremiumToken>();
		short num25 = reader.ReadInt16();
		for (short num26 = 0; num26 < num25; num26++)
		{
			_premiumTokens.Add(new PremiumToken(reader));
		}
		_completedTutorials = new List<string>();
		short num27 = reader.ReadInt16();
		for (short num28 = 0; num28 < num27; num28++)
		{
			_completedTutorials.Add(reader.ReadString());
		}
		_lastActionTimes = new List<ActionTime>();
		short num29 = reader.ReadInt16();
		for (short num30 = 0; num30 < num29; num30++)
		{
			_lastActionTimes.Add(new ActionTime(reader));
		}
		_seenHotItemsHashCodes = new List<int>();
		short num31 = reader.ReadInt16();
		for (short num32 = 0; num32 < num31; num32++)
		{
			_seenHotItemsHashCodes.Add(reader.ReadInt32());
		}
		_readNewsHashCodes = new List<int>();
		short num33 = reader.ReadInt16();
		for (short num34 = 0; num34 < num33; num34++)
		{
			_readNewsHashCodes.Add(reader.ReadInt32());
		}
		_millisecondsToNextHomeDelivery = reader.ReadInt64();
		if (reader.ReadInt8() == 1)
		{
			_nextHomeDeliveryBox = new TemplateReference(reader);
		}
		_selectedClasses = new List<SelectedClass>();
		short num35 = reader.ReadInt16();
		for (short num36 = 0; num36 < num35; num36++)
		{
			_selectedClasses.Add(new SelectedClass(reader));
		}
		_skillTemplates = new List<SkillTemplate>();
		short num37 = reader.ReadInt16();
		for (short num38 = 0; num38 < num37; num38++)
		{
			_skillTemplates.Add(new SkillTemplate(reader));
		}
		_playersBannedFromHome = new List<string>();
		short num39 = reader.ReadInt16();
		for (short num40 = 0; num40 < num39; num40++)
		{
			_playersBannedFromHome.Add(reader.ReadString());
		}
	}

	public FullAvatar(string name, sbyte gender, string skinColor, int hairColor, string eyeColor, string mouth, string eyes, string eyeBrows, string hair, float height, float health, float maxHealth, string mood, string title, int gems, int teleportStones, int exp, int avatarLevel, sbyte unusedAvailableClassLevels, int avatarLevelExpRequirement, int nextAvatarLevelExpRequirement, IList<InventoryEntry> items, IList<Item> equippedItems, IList<Quest> completedQuests, IList<Quest> activeQuests, IList<AchievementTemplate> achievementTemplates, IList<string> achievements, IList<AchievementCounter> counters, IList<AmmoType> ammoTypes, IList<LevelStateData> levelStates, IList<WorldLevel> worldLevels, IList<FoundTokensInfo> currentExplorationTokens, IList<FoundCoinTokensInfo> currentCoinTokens, IList<PremiumToken> premiumTokens, IList<string> completedTutorials, IList<ActionTime> lastActionTimes, IList<int> seenHotItemsHashCodes, IList<int> readNewsHashCodes, long millisecondsToNextHomeDelivery, TemplateReference nextHomeDeliveryBox, IList<SelectedClass> selectedClasses, IList<SkillTemplate> skillTemplates, IList<string> playersBannedFromHome)
	{
		_name = name;
		_gender = gender;
		_skinColor = skinColor;
		_hairColor = hairColor;
		_eyeColor = eyeColor;
		_mouth = mouth;
		_eyes = eyes;
		_eyeBrows = eyeBrows;
		_hair = hair;
		_height = height;
		_health = health;
		_maxHealth = maxHealth;
		_mood = mood;
		_title = title;
		_gems = gems;
		_teleportStones = teleportStones;
		_exp = exp;
		_avatarLevel = avatarLevel;
		_unusedAvailableClassLevels = unusedAvailableClassLevels;
		_avatarLevelExpRequirement = avatarLevelExpRequirement;
		_nextAvatarLevelExpRequirement = nextAvatarLevelExpRequirement;
		_items = items;
		_equippedItems = equippedItems;
		_completedQuests = completedQuests;
		_activeQuests = activeQuests;
		_achievementTemplates = achievementTemplates;
		_achievements = achievements;
		_counters = counters;
		_ammoTypes = ammoTypes;
		_levelStates = levelStates;
		_worldLevels = worldLevels;
		_currentExplorationTokens = currentExplorationTokens;
		_currentCoinTokens = currentCoinTokens;
		_premiumTokens = premiumTokens;
		_completedTutorials = completedTutorials;
		_lastActionTimes = lastActionTimes;
		_seenHotItemsHashCodes = seenHotItemsHashCodes;
		_readNewsHashCodes = readNewsHashCodes;
		_millisecondsToNextHomeDelivery = millisecondsToNextHomeDelivery;
		_nextHomeDeliveryBox = nextHomeDeliveryBox;
		_selectedClasses = selectedClasses;
		_skillTemplates = skillTemplates;
		_playersBannedFromHome = playersBannedFromHome;
	}

	public string GetName()
	{
		return _name;
	}

	public sbyte GetGender()
	{
		return _gender;
	}

	public string GetSkinColor()
	{
		return _skinColor;
	}

	public int GetHairColor()
	{
		return _hairColor;
	}

	public string GetEyeColor()
	{
		return _eyeColor;
	}

	public string GetMouth()
	{
		return _mouth;
	}

	public string GetEyes()
	{
		return _eyes;
	}

	public string GetEyeBrows()
	{
		return _eyeBrows;
	}

	public string GetHair()
	{
		return _hair;
	}

	public float GetHeight()
	{
		return _height;
	}

	public float GetHealth()
	{
		return _health;
	}

	public float GetMaxHealth()
	{
		return _maxHealth;
	}

	public string GetMood()
	{
		return _mood;
	}

	public string GetTitle()
	{
		return _title;
	}

	public int GetGems()
	{
		return _gems;
	}

	public int GetTeleportStones()
	{
		return _teleportStones;
	}

	public int GetExp()
	{
		return _exp;
	}

	public int GetAvatarLevel()
	{
		return _avatarLevel;
	}

	public sbyte GetUnusedAvailableClassLevels()
	{
		return _unusedAvailableClassLevels;
	}

	public int GetAvatarLevelExpRequirement()
	{
		return _avatarLevelExpRequirement;
	}

	public int GetNextAvatarLevelExpRequirement()
	{
		return _nextAvatarLevelExpRequirement;
	}

	public IList<InventoryEntry> GetItems()
	{
		return _items;
	}

	public IList<Item> GetEquippedItems()
	{
		return _equippedItems;
	}

	public IList<Quest> GetCompletedQuests()
	{
		return _completedQuests;
	}

	public IList<Quest> GetActiveQuests()
	{
		return _activeQuests;
	}

	public IList<AchievementTemplate> GetAchievementTemplates()
	{
		return _achievementTemplates;
	}

	public IList<string> GetAchievements()
	{
		return _achievements;
	}

	public IList<AchievementCounter> GetCounters()
	{
		return _counters;
	}

	public IList<AmmoType> GetAmmoTypes()
	{
		return _ammoTypes;
	}

	public IList<LevelStateData> GetLevelStates()
	{
		return _levelStates;
	}

	public IList<WorldLevel> GetWorldLevels()
	{
		return _worldLevels;
	}

	public IList<FoundTokensInfo> GetCurrentExplorationTokens()
	{
		return _currentExplorationTokens;
	}

	public IList<FoundCoinTokensInfo> GetCurrentCoinTokens()
	{
		return _currentCoinTokens;
	}

	public IList<PremiumToken> GetPremiumTokens()
	{
		return _premiumTokens;
	}

	public IList<string> GetCompletedTutorials()
	{
		return _completedTutorials;
	}

	public IList<ActionTime> GetLastActionTimes()
	{
		return _lastActionTimes;
	}

	public IList<int> GetSeenHotItemsHashCodes()
	{
		return _seenHotItemsHashCodes;
	}

	public IList<int> GetReadNewsHashCodes()
	{
		return _readNewsHashCodes;
	}

	public long GetMillisecondsToNextHomeDelivery()
	{
		return _millisecondsToNextHomeDelivery;
	}

	public TemplateReference GetNextHomeDeliveryBox()
	{
		return _nextHomeDeliveryBox;
	}

	public IList<SelectedClass> GetSelectedClasses()
	{
		return _selectedClasses;
	}

	public IList<SkillTemplate> GetSkillTemplates()
	{
		return _skillTemplates;
	}

	public IList<string> GetPlayersBannedFromHome()
	{
		return _playersBannedFromHome;
	}

	public int Size()
	{
		int num = 109;
		num += MessageWriter.GetSize(_name);
		num += MessageWriter.GetSize(_skinColor);
		num += MessageWriter.GetSize(_eyeColor);
		num += MessageWriter.GetSize(_mouth);
		num += MessageWriter.GetSize(_eyes);
		num += MessageWriter.GetSize(_eyeBrows);
		num += MessageWriter.GetSize(_hair);
		num += MessageWriter.GetSize(_mood);
		num += MessageWriter.GetSize(_title);
		num += (short)(_items.Count * 2);
		foreach (InventoryEntry item in _items)
		{
			num += item.Size();
		}
		num += (short)(_equippedItems.Count * 2);
		foreach (Item equippedItem in _equippedItems)
		{
			num += equippedItem.Size();
		}
		foreach (Quest completedQuest in _completedQuests)
		{
			num += completedQuest.Size();
		}
		foreach (Quest activeQuest in _activeQuests)
		{
			num += activeQuest.Size();
		}
		foreach (AchievementTemplate achievementTemplate in _achievementTemplates)
		{
			num += achievementTemplate.Size();
		}
		num += (short)(2 * _achievements.Count);
		foreach (string achievement in _achievements)
		{
			num += MessageWriter.GetSize(achievement);
		}
		foreach (AchievementCounter counter in _counters)
		{
			num += counter.Size();
		}
		foreach (AmmoType ammoType in _ammoTypes)
		{
			num += ammoType.Size();
		}
		foreach (LevelStateData levelState in _levelStates)
		{
			num += levelState.Size();
		}
		foreach (WorldLevel worldLevel in _worldLevels)
		{
			num += worldLevel.Size();
		}
		foreach (FoundTokensInfo currentExplorationToken in _currentExplorationTokens)
		{
			num += currentExplorationToken.Size();
		}
		foreach (FoundCoinTokensInfo currentCoinToken in _currentCoinTokens)
		{
			num += currentCoinToken.Size();
		}
		foreach (PremiumToken premiumToken in _premiumTokens)
		{
			num += premiumToken.Size();
		}
		num += (short)(2 * _completedTutorials.Count);
		foreach (string completedTutorial in _completedTutorials)
		{
			num += MessageWriter.GetSize(completedTutorial);
		}
		foreach (ActionTime lastActionTime in _lastActionTimes)
		{
			num += lastActionTime.Size();
		}
		num += (short)(_seenHotItemsHashCodes.Count * 4);
		num += (short)(_readNewsHashCodes.Count * 4);
		if (_nextHomeDeliveryBox != null)
		{
			num += _nextHomeDeliveryBox.Size();
		}
		foreach (SelectedClass selectedClass in _selectedClasses)
		{
			num += selectedClass.Size();
		}
		foreach (SkillTemplate skillTemplate in _skillTemplates)
		{
			num += skillTemplate.Size();
		}
		num += (short)(2 * _playersBannedFromHome.Count);
		foreach (string item2 in _playersBannedFromHome)
		{
			num += MessageWriter.GetSize(item2);
		}
		return num;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_name);
		writer.WriteInt8(_gender);
		writer.WriteString(_skinColor);
		writer.WriteInt32(_hairColor);
		writer.WriteString(_eyeColor);
		writer.WriteString(_mouth);
		writer.WriteString(_eyes);
		writer.WriteString(_eyeBrows);
		writer.WriteString(_hair);
		writer.WriteFloat(_height);
		writer.WriteFloat(_health);
		writer.WriteFloat(_maxHealth);
		writer.WriteString(_mood);
		writer.WriteString(_title);
		writer.WriteInt32(_gems);
		writer.WriteInt32(_teleportStones);
		writer.WriteInt32(_exp);
		writer.WriteInt32(_avatarLevel);
		writer.WriteInt8(_unusedAvailableClassLevels);
		writer.WriteInt32(_avatarLevelExpRequirement);
		writer.WriteInt32(_nextAvatarLevelExpRequirement);
		writer.WriteInt16((short)_items.Count);
		foreach (InventoryEntry item in _items)
		{
			writer.WriteTypeCode(item.GetTypeId());
			item.Write(writer);
		}
		writer.WriteInt16((short)_equippedItems.Count);
		foreach (Item equippedItem in _equippedItems)
		{
			writer.WriteTypeCode(equippedItem.GetTypeId());
			equippedItem.Write(writer);
		}
		writer.WriteInt16((short)_completedQuests.Count);
		foreach (Quest completedQuest in _completedQuests)
		{
			completedQuest.Write(writer);
		}
		writer.WriteInt16((short)_activeQuests.Count);
		foreach (Quest activeQuest in _activeQuests)
		{
			activeQuest.Write(writer);
		}
		writer.WriteInt16((short)_achievementTemplates.Count);
		foreach (AchievementTemplate achievementTemplate in _achievementTemplates)
		{
			achievementTemplate.Write(writer);
		}
		writer.WriteInt16((short)_achievements.Count);
		foreach (string achievement in _achievements)
		{
			writer.WriteString(achievement);
		}
		writer.WriteInt16((short)_counters.Count);
		foreach (AchievementCounter counter in _counters)
		{
			counter.Write(writer);
		}
		writer.WriteInt16((short)_ammoTypes.Count);
		foreach (AmmoType ammoType in _ammoTypes)
		{
			ammoType.Write(writer);
		}
		writer.WriteInt16((short)_levelStates.Count);
		foreach (LevelStateData levelState in _levelStates)
		{
			levelState.Write(writer);
		}
		writer.WriteInt16((short)_worldLevels.Count);
		foreach (WorldLevel worldLevel in _worldLevels)
		{
			worldLevel.Write(writer);
		}
		writer.WriteInt16((short)_currentExplorationTokens.Count);
		foreach (FoundTokensInfo currentExplorationToken in _currentExplorationTokens)
		{
			currentExplorationToken.Write(writer);
		}
		writer.WriteInt16((short)_currentCoinTokens.Count);
		foreach (FoundCoinTokensInfo currentCoinToken in _currentCoinTokens)
		{
			currentCoinToken.Write(writer);
		}
		writer.WriteInt16((short)_premiumTokens.Count);
		foreach (PremiumToken premiumToken in _premiumTokens)
		{
			premiumToken.Write(writer);
		}
		writer.WriteInt16((short)_completedTutorials.Count);
		foreach (string completedTutorial in _completedTutorials)
		{
			writer.WriteString(completedTutorial);
		}
		writer.WriteInt16((short)_lastActionTimes.Count);
		foreach (ActionTime lastActionTime in _lastActionTimes)
		{
			lastActionTime.Write(writer);
		}
		writer.WriteInt16((short)_seenHotItemsHashCodes.Count);
		foreach (int seenHotItemsHashCode in _seenHotItemsHashCodes)
		{
			writer.WriteInt32(seenHotItemsHashCode);
		}
		writer.WriteInt16((short)_readNewsHashCodes.Count);
		foreach (int readNewsHashCode in _readNewsHashCodes)
		{
			writer.WriteInt32(readNewsHashCode);
		}
		writer.WriteInt64(_millisecondsToNextHomeDelivery);
		if (_nextHomeDeliveryBox == null)
		{
			writer.WriteInt8(0);
		}
		else
		{
			writer.WriteInt8(1);
			_nextHomeDeliveryBox.Write(writer);
		}
		writer.WriteInt16((short)_selectedClasses.Count);
		foreach (SelectedClass selectedClass in _selectedClasses)
		{
			selectedClass.Write(writer);
		}
		writer.WriteInt16((short)_skillTemplates.Count);
		foreach (SkillTemplate skillTemplate in _skillTemplates)
		{
			skillTemplate.Write(writer);
		}
		writer.WriteInt16((short)_playersBannedFromHome.Count);
		foreach (string item2 in _playersBannedFromHome)
		{
			writer.WriteString(item2);
		}
	}
}
