using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.EventSystem;
using Code.Core.Items;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.World.Level.LevelInfo;
using Code.World.Player;
using Core;
using UI.LockState;
using UnityEngine;

namespace Code.World.Achievements;

public class MilMo_AchievementHandler : Singleton<MilMo_AchievementHandler>
{
	private List<MilMo_AchievementTemplate> _uncompletedAchievements;

	private List<MilMo_AchievementTemplate> _completedAchievements;

	private IList<string> _completedAchievementIdentifiers;

	private Dictionary<string, List<MilMo_AchievementCounter>> _achievementCounters;

	private List<MilMo_MedalCategory> _medalsForGUI;

	private Dictionary<string, List<MilMo_AchievementObjectiveListener>> _achievementCompletedListeners;

	private MilMo_GenericReaction _counterUpdatedReaction;

	private MilMo_GenericReaction _achievementCompletedReaction;

	public int NumberCompleted => _completedAchievements.Count;

	public MilMo_LocString MedalCategoryDisplayName(string categoryIdentifier)
	{
		if (!Enum.TryParse<MilMo_MedalCategory.MedalCategory>(categoryIdentifier, out var result))
		{
			return MilMo_LocString.Empty;
		}
		MilMo_MedalCategory.MedalCategoryLocales.TryGetValue(result, out var value);
		return MilMo_Localization.GetLocString(value);
	}

	private void Awake()
	{
		_uncompletedAchievements = new List<MilMo_AchievementTemplate>();
		_completedAchievements = new List<MilMo_AchievementTemplate>();
		_completedAchievementIdentifiers = new List<string>();
		_achievementCounters = new Dictionary<string, List<MilMo_AchievementCounter>>();
		_medalsForGUI = new List<MilMo_MedalCategory>();
		_achievementCompletedListeners = new Dictionary<string, List<MilMo_AchievementObjectiveListener>>();
		_counterUpdatedReaction = MilMo_EventSystem.Listen("achievement_counter_update", UpdateCounter);
		_counterUpdatedReaction.Repeating = true;
		_achievementCompletedReaction = MilMo_EventSystem.Listen("achievement_completed", AchievementCompleted);
		_achievementCompletedReaction.Repeating = true;
		InitializeMedalCategories();
	}

	public void OnDestroy()
	{
		MilMo_EventSystem.RemoveReaction(_counterUpdatedReaction);
		MilMo_EventSystem.RemoveReaction(_achievementCompletedReaction);
	}

	private void InitializeMedalCategories()
	{
		foreach (KeyValuePair<MilMo_MedalCategory.MedalCategory, string> medalCategoryLocale in MilMo_MedalCategory.MedalCategoryLocales)
		{
			MilMo_MedalCategory.MedalCategory key = medalCategoryLocale.Key;
			_medalsForGUI.Add(new MilMo_MedalCategory(key.ToString()));
		}
	}

	public void ReadAll(IEnumerable<AchievementTemplate> templateList, IList<string> completedAchievements, IEnumerable<AchievementCounter> counters)
	{
		_achievementCounters.Clear();
		foreach (AchievementCounter counter in counters)
		{
			if (!_achievementCounters.ContainsKey(counter.GetTemplateType()))
			{
				_achievementCounters.Add(counter.GetTemplateType(), new List<MilMo_AchievementCounter>());
			}
			_achievementCounters[counter.GetTemplateType()].Add(new MilMo_AchievementCounter(counter.GetTemplateType(), counter.GetObj(), counter.GetCount()));
		}
		_completedAchievementIdentifiers = completedAchievements;
		_completedAchievements.Clear();
		_uncompletedAchievements.Clear();
		foreach (AchievementTemplate template in templateList)
		{
			if (!(Singleton<MilMo_TemplateContainer>.Instance.LoadTemplateFromNetworkMessage(template) is MilMo_AchievementTemplate milMo_AchievementTemplate))
			{
				continue;
			}
			bool flag = false;
			foreach (string completedAchievementIdentifier in _completedAchievementIdentifiers)
			{
				if (!(completedAchievementIdentifier != milMo_AchievementTemplate.Identifier))
				{
					flag = true;
					_completedAchievements.Add(milMo_AchievementTemplate);
					break;
				}
			}
			foreach (MilMo_MedalCategory item in _medalsForGUI)
			{
				if (!(item.Identifier != milMo_AchievementTemplate.AchievementCategoryIdentifier))
				{
					if (milMo_AchievementTemplate.Instantiate(new Dictionary<string, string>()) is MilMo_Medal milMo_Medal)
					{
						milMo_Medal.Acquired = flag;
						item.AddMedal(milMo_Medal);
					}
					break;
				}
			}
			if (flag)
			{
				continue;
			}
			_uncompletedAchievements.Add(milMo_AchievementTemplate);
			foreach (MilMo_AchievementObjective objective in milMo_AchievementTemplate.Objectives)
			{
				objective.UnregisterListener();
				objective.RegisterListener(new MilMo_AchievementObjectiveListener(milMo_AchievementTemplate, objective.Object));
			}
			TestCompletion(milMo_AchievementTemplate);
		}
	}

	public MilMo_AchievementCounter GetCounter(string type, string obj)
	{
		if (!_achievementCounters.ContainsKey(type))
		{
			return null;
		}
		foreach (MilMo_AchievementCounter item in _achievementCounters[type])
		{
			if (item.Object == obj)
			{
				return item;
			}
		}
		return null;
	}

	public bool IsCompleted(string achievementTemplateIdentifier)
	{
		foreach (string completedAchievementIdentifier in _completedAchievementIdentifiers)
		{
			if (completedAchievementIdentifier == achievementTemplateIdentifier)
			{
				return true;
			}
		}
		return false;
	}

	private void AchievementCompleted(object msgAsObj)
	{
		if (!(msgAsObj is ServerAchievementCompleted serverAchievementCompleted) || serverAchievementCompleted.getPlayerId() != MilMo_Player.Instance.Avatar.Id)
		{
			return;
		}
		MilMo_AchievementTemplate milMo_AchievementTemplate = null;
		for (int num = _uncompletedAchievements.Count - 1; num >= 0; num--)
		{
			if (!(serverAchievementCompleted.getAchievement() != _uncompletedAchievements[num].Identifier))
			{
				milMo_AchievementTemplate = _uncompletedAchievements[num];
				_completedAchievements.Add(milMo_AchievementTemplate);
				_completedAchievementIdentifiers.Add(milMo_AchievementTemplate.Identifier);
				_uncompletedAchievements.RemoveAt(num);
				foreach (MilMo_MedalCategory item in _medalsForGUI)
				{
					if (!(item.Identifier != milMo_AchievementTemplate.AchievementCategoryIdentifier))
					{
						item.SetAsCompleted(milMo_AchievementTemplate);
						break;
					}
				}
				foreach (MilMo_AchievementObjective objective in milMo_AchievementTemplate.Objectives)
				{
					objective.UnregisterListener();
				}
				MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_ReceiveItem", milMo_AchievementTemplate.Name);
				MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_ReceiveMedal");
				Singleton<LockStateManager>.Instance.HasUnlockedProfile = true;
				break;
			}
		}
		if (milMo_AchievementTemplate == null)
		{
			return;
		}
		if (_achievementCompletedListeners.ContainsKey("Any"))
		{
			foreach (MilMo_AchievementObjectiveListener item2 in _achievementCompletedListeners["Any"])
			{
				item2.Notify();
			}
		}
		if (!_achievementCompletedListeners.ContainsKey(milMo_AchievementTemplate.Identifier))
		{
			return;
		}
		foreach (MilMo_AchievementObjectiveListener item3 in _achievementCompletedListeners[milMo_AchievementTemplate.Identifier])
		{
			item3.Notify();
		}
		_achievementCompletedListeners.Remove(milMo_AchievementTemplate.Identifier);
	}

	private void UpdateCounter(object msgAsObj)
	{
		if (!(msgAsObj is ServerUpdateAchievementCounter serverUpdateAchievementCounter) || !_achievementCounters.ContainsKey(serverUpdateAchievementCounter.getCounter().GetTemplateType()))
		{
			return;
		}
		foreach (MilMo_AchievementCounter item in _achievementCounters[serverUpdateAchievementCounter.getCounter().GetTemplateType()])
		{
			if (!(item.Object != serverUpdateAchievementCounter.getCounter().GetObj()))
			{
				item.Count = serverUpdateAchievementCounter.getCounter().GetCount();
				break;
			}
		}
	}

	public void TestCompletion(MilMo_AchievementTemplate template)
	{
		if (VerifyObjectives(template))
		{
			Singleton<GameNetwork>.Instance.RequestCompleteAchievement(template.Identifier);
		}
	}

	private bool VerifyObjectives(MilMo_AchievementTemplate achievement)
	{
		if (achievement.Type == "PVPMedal")
		{
			return false;
		}
		foreach (MilMo_AchievementObjective objective in achievement.Objectives)
		{
			if (objective == null)
			{
				Debug.LogWarning("Got null objective when iterating achievements");
				continue;
			}
			bool flag = false;
			if (objective.Type == "CompleteMedal")
			{
				if (objective.Object == "Any")
				{
					if (_completedAchievements.Count >= objective.Count)
					{
						flag = true;
					}
				}
				else
				{
					foreach (string completedAchievementIdentifier in _completedAchievementIdentifiers)
					{
						if (completedAchievementIdentifier == objective.Object)
						{
							flag = true;
							break;
						}
					}
				}
			}
			else if (objective.Type == "CompleteQuest")
			{
				if (MilMo_Player.Instance.Quests == null)
				{
					Debug.LogWarning("Got null quests when verifying CompleteQuest achievement objective");
					continue;
				}
				if (objective.Object == "Any")
				{
					if (MilMo_Player.Instance.Quests.CompletedQuests.Count >= objective.Count)
					{
						flag = true;
					}
				}
				else if (MilMo_Player.Instance.Quests.IsCompleted(objective.Object))
				{
					flag = true;
				}
			}
			else if (objective.Type == "FindAllSecrets")
			{
				MilMo_LevelInfoData levelInfoData = MilMo_LevelInfo.GetLevelInfoData(objective.Object);
				if (levelInfoData == null)
				{
					continue;
				}
				if (levelInfoData.NumberOfExplorationTokensFound >= levelInfoData.ExplorationTokens.Count)
				{
					flag = true;
				}
			}
			else if (objective.Type == "Collect" && objective.Count == 1)
			{
				if (MilMo_Player.Instance.Avatar == null || MilMo_Player.Instance.Inventory == null)
				{
					Debug.LogWarning("Got null avatar or inventory when verifying Collect achievement objective");
					continue;
				}
				if (MilMo_Player.Instance.Inventory.HaveItem(objective.Object))
				{
					flag = true;
				}
			}
			else if (_achievementCounters.ContainsKey(objective.Type))
			{
				foreach (MilMo_AchievementCounter item in _achievementCounters[objective.Type])
				{
					if (item.Meets(objective))
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				continue;
			}
			return false;
		}
		return true;
	}

	public void AddAchievementCompletedListener(MilMo_AchievementObjectiveListener listener)
	{
		if (!_achievementCompletedListeners.ContainsKey(listener.Object))
		{
			_achievementCompletedListeners.Add(listener.Object, new List<MilMo_AchievementObjectiveListener>());
		}
		_achievementCompletedListeners[listener.Object].Add(listener);
	}

	public void RemoveAchievementCompletedListener(MilMo_AchievementObjectiveListener listener)
	{
		if (_achievementCompletedListeners.ContainsKey(listener.Object))
		{
			if (_achievementCompletedListeners[listener.Object].Count == 1)
			{
				_achievementCompletedListeners.Remove(listener.Object);
			}
			else
			{
				_achievementCompletedListeners[listener.Object].Remove(listener);
			}
		}
	}

	public MilMo_AchievementTemplate GetNextInSameCategory(MilMo_AchievementTemplate medal)
	{
		foreach (MilMo_MedalCategory item in _medalsForGUI)
		{
			if (item.Identifier == medal.AchievementCategoryIdentifier)
			{
				return item.GetNext(medal);
			}
		}
		return null;
	}

	public IList<MilMo_Medal> GetAcquiredMedals()
	{
		List<MilMo_Medal> list = new List<MilMo_Medal>();
		foreach (MilMo_MedalCategory item in _medalsForGUI)
		{
			list.AddRange(item.Medals.Where((MilMo_Medal medal) => medal.Acquired));
		}
		return list;
	}

	public IEnumerable<MilMo_MedalCategory> GetMedalCategories()
	{
		_medalsForGUI.Sort(delegate(MilMo_MedalCategory category1, MilMo_MedalCategory category2)
		{
			Enum.TryParse<MilMo_MedalCategory.MedalCategory>(category1.Identifier, out var result);
			Enum.TryParse<MilMo_MedalCategory.MedalCategory>(category2.Identifier, out var result2);
			return result - result2;
		});
		return _medalsForGUI;
	}

	private List<MilMo_Medal> GetExplorationTokenMedals()
	{
		List<MilMo_Medal> list = new List<MilMo_Medal>();
		foreach (MilMo_Medal item in _medalsForGUI.SelectMany((MilMo_MedalCategory category) => category.Medals))
		{
			if (item.Template.Objectives.Count == 1 && !(item.Template.Objectives[0].Type != "FindAllSecrets"))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public MilMo_Medal GetExplorationTokenMedal(string fullLevelName)
	{
		return GetExplorationTokenMedals().FirstOrDefault((MilMo_Medal medal) => medal.Template.Objectives[0].Object == fullLevelName);
	}

	public MilMo_Item GetExplorationTokenMedalRewardItemAsync(string fullLevelName)
	{
		return GetExplorationTokenMedalRewardTemplate(fullLevelName)?.Instantiate(new Dictionary<string, string>());
	}

	private MilMo_ItemTemplate GetExplorationTokenMedalRewardTemplate(string fullLevelName)
	{
		return GetExplorationTokenMedal(fullLevelName)?.Template.GetReward(MilMo_Player.Instance.Avatar.IsBoy);
	}

	public Dictionary<string, MilMo_ItemTemplate> GetExplorationTokenMedalRewardTemplates()
	{
		Dictionary<string, MilMo_ItemTemplate> dictionary = new Dictionary<string, MilMo_ItemTemplate>();
		foreach (MilMo_Medal explorationTokenMedal in GetExplorationTokenMedals())
		{
			MilMo_ItemTemplate reward = explorationTokenMedal.Template.GetReward(MilMo_Player.Instance.Avatar.IsBoy);
			if (reward != null)
			{
				dictionary.Add(explorationTokenMedal.Template.Objectives[0].Object, reward);
			}
		}
		return dictionary;
	}

	public static MilMo_AchievementHandler Get()
	{
		return Singleton<MilMo_AchievementHandler>.Instance;
	}
}
