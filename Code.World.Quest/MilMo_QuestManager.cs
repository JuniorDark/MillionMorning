using System.Collections.Generic;
using System.Linq;
using Code.Core.EventSystem;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.Template;
using Code.World.Achievements;
using Code.World.Level.LevelInfo;
using Code.World.Level.LevelObject;
using Core;
using Core.Analytics;
using Localization;
using UI.HUD.Dialogues;
using UI.HUD.Dialogues.NPC;

namespace Code.World.Quest;

public sealed class MilMo_QuestManager
{
	private readonly List<MilMo_Quest> _activeQuests = new List<MilMo_Quest>();

	private readonly List<MilMo_Quest> _completedQuests = new List<MilMo_Quest>();

	private readonly Dictionary<string, List<MilMo_AchievementObjectiveListener>> _questCompletionListeners = new Dictionary<string, List<MilMo_AchievementObjectiveListener>>();

	public List<MilMo_Quest> ActiveQuests => _activeQuests;

	public List<MilMo_Quest> CompletedQuests => _completedQuests;

	public MilMo_QuestManager()
	{
		MilMo_QuestAreaManager.Initialize();
		MilMo_EventSystem.Listen("quest_added", QuestAdded).Repeating = true;
		MilMo_EventSystem.Listen("internal_quest_condition_update", QuestConditionUpdate).Repeating = true;
	}

	public void Destroy()
	{
		_activeQuests.Clear();
		_completedQuests.Clear();
		_questCompletionListeners.Clear();
	}

	private void QuestAdded(object msgAsObject)
	{
		ServerQuestAdded serverQuestAdded = (ServerQuestAdded)msgAsObject;
		if (serverQuestAdded == null)
		{
			return;
		}
		MilMo_Quest quest = new MilMo_Quest();
		quest.Read(serverQuestAdded.getAddedQuest());
		if (quest.State == QuestState.Active)
		{
			_activeQuests.Add(quest);
			MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_ReceiveQuest", quest.TemplatePath);
			IList<string> messagesOnAdd = serverQuestAdded.getNpcTextsOnAdd();
			if (messagesOnAdd.Count > 0)
			{
				Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(serverQuestAdded.getQuestGiver().GetTemplateIdentifier(), delegate(MilMo_Template template, bool timeOut)
				{
					if (!timeOut && template is MilMo_NpcTemplate milMo_NpcTemplate)
					{
						List<LocalizedStringWithArgument> list = new List<LocalizedStringWithArgument>();
						foreach (string item in messagesOnAdd)
						{
							list.Add(new LocalizedStringWithArgument(item));
						}
						DialogueSpawner.SpawnNPCMessageDialogue(new NPCMessageData(null, -1, new LocalizedStringWithArgument(milMo_NpcTemplate.NPCName).GetMessage(), milMo_NpcTemplate.GetPortraitKey(), "", list));
					}
				});
			}
			MilMoAnalyticsHandler.QuestEvent(quest.TemplatePath, 0);
		}
		else
		{
			using (IEnumerator<MilMo_Quest> enumerator = _activeQuests.Where((MilMo_Quest q) => q.Id == quest.Id).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					MilMo_Quest current = enumerator.Current;
					_activeQuests.Remove(current);
				}
			}
			if (_completedQuests.Any((MilMo_Quest q) => q.TemplatePath == quest.TemplatePath))
			{
				MilMo_EventSystem.Instance.PostEvent("quest_received", quest);
				return;
			}
			_completedQuests.Add(quest);
			if (_questCompletionListeners.ContainsKey("Any"))
			{
				foreach (MilMo_AchievementObjectiveListener item2 in _questCompletionListeners["Any"])
				{
					item2.Notify();
				}
			}
			if (_questCompletionListeners.ContainsKey(quest.TemplatePath))
			{
				foreach (MilMo_AchievementObjectiveListener item3 in _questCompletionListeners[quest.TemplatePath])
				{
					item3.Notify();
				}
				_questCompletionListeners.Remove(quest.TemplatePath);
			}
			MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_CompleteQuest", quest.TemplatePath);
			MilMoAnalyticsHandler.QuestEvent(quest.TemplatePath, 1);
		}
		MilMo_EventSystem.Instance.PostEvent("quest_received", quest);
	}

    private void QuestConditionUpdate(object msgAsObject)
    {
        ServerQuestConditionUpdate msg = msgAsObject as ServerQuestConditionUpdate;
        if (msg == null)
        {
            return;
        }

        using (IEnumerator<MilMo_Quest> activeEnumerator = _activeQuests.Where((MilMo_Quest quest) => quest.Id == msg.getQuestId()).GetEnumerator())
        {
            if (activeEnumerator.MoveNext())
            {
                activeEnumerator.Current.UpdateCondition(msg.getIndex(), msg.getCondition());
                return;
            }
        }

        using (IEnumerator<MilMo_Quest> completedEnumerator = _completedQuests.Where((MilMo_Quest quest) => quest.Id == msg.getQuestId()).GetEnumerator())
        {
            if (completedEnumerator.MoveNext())
            {
                completedEnumerator.Current.UpdateCondition(msg.getIndex(), msg.getCondition());
            }
        }
    }

    public void ReadAll(IList<Code.Core.Network.types.Quest> activeQuests, IList<Code.Core.Network.types.Quest> completedQuests)
	{
		_activeQuests.Clear();
		foreach (Code.Core.Network.types.Quest activeQuest in activeQuests)
		{
			MilMo_Quest milMo_Quest = new MilMo_Quest();
			milMo_Quest.Read(activeQuest);
			if (QuestLevelExist(milMo_Quest))
			{
				_activeQuests.Add(milMo_Quest);
				if (milMo_Quest.IsTracked)
				{
					SetQuestActive(milMo_Quest);
				}
			}
		}
		_completedQuests.Clear();
		foreach (Code.Core.Network.types.Quest completedQuest in completedQuests)
		{
			MilMo_Quest milMo_Quest2 = new MilMo_Quest();
			milMo_Quest2.Read(completedQuest);
			_completedQuests.Add(milMo_Quest2);
		}
	}

	private static bool QuestLevelExist(MilMo_Quest quest)
	{
		return MilMo_LevelInfo.GetLevelInfoData(quest.FullLevelName) != null;
	}

	private static void SetQuestActive(MilMo_Quest quest)
	{
		if (quest != null)
		{
			if (MilMo_World.HudHandler.theQuestTracker.ActiveQuest != null && quest.Id == MilMo_World.HudHandler.theQuestTracker.ActiveQuest.Id)
			{
				MilMo_World.HudHandler.theQuestTracker.SetInactive();
			}
			else
			{
				MilMo_World.HudHandler.theQuestTracker.SetActiveQuest(quest);
			}
		}
	}

	public void AddQuestCompletedListener(MilMo_AchievementObjectiveListener listener)
	{
		if (!_questCompletionListeners.ContainsKey(listener.Object))
		{
			_questCompletionListeners.Add(listener.Object, new List<MilMo_AchievementObjectiveListener>());
		}
		_questCompletionListeners[listener.Object].Add(listener);
	}

	public void RemoveQuestCompletedListener(MilMo_AchievementObjectiveListener listener)
	{
		if (_questCompletionListeners.ContainsKey(listener.Object))
		{
			if (_questCompletionListeners[listener.Object].Count == 1)
			{
				_questCompletionListeners.Remove(listener.Object);
			}
			else
			{
				_questCompletionListeners[listener.Object].Remove(listener);
			}
		}
	}

	public bool IsCompleted(string templatePath)
	{
		return _completedQuests.Any((MilMo_Quest quest) => quest.TemplatePath == templatePath);
	}

	public bool IsInActiveQuests(string templatePath)
	{
		return _activeQuests.Any((MilMo_Quest quest) => quest.TemplatePath == templatePath);
	}
}
