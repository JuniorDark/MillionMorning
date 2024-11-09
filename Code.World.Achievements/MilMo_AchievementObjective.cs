using Code.Core.Network.types;
using Code.World.Level;
using Code.World.Player;
using UnityEngine;

namespace Code.World.Achievements;

public class MilMo_AchievementObjective
{
	private MilMo_AchievementObjectiveListener _listener;

	public int Count { get; private set; }

	public string Type { get; private set; }

	public string Object { get; private set; }

	public MilMo_AchievementObjective(AchievementObjective objective)
	{
		Type = objective.GetTemplateType();
		Object = objective.GetObj();
		Count = objective.GetCount();
	}

	public void RegisterListener(MilMo_AchievementObjectiveListener listener)
	{
		if (_listener != null)
		{
			Debug.LogWarning("Failed to register achievement listener: " + ((listener == null) ? "null" : listener.Object));
			return;
		}
		_listener = listener;
		MilMo_AchievementHandler milMo_AchievementHandler = MilMo_AchievementHandler.Get();
		switch (Type)
		{
		case "CompleteMedal":
			milMo_AchievementHandler.AddAchievementCompletedListener(listener);
			return;
		case "CompleteQuest":
			MilMo_Player.Instance.Quests.AddQuestCompletedListener(listener);
			return;
		case "FindAllSecrets":
			MilMo_Level.AddAllExplorationTokensFoundListener(listener);
			return;
		}
		if (Type == "Collect" && Count == 1)
		{
			if (MilMo_Player.Instance.Inventory == null)
			{
				Debug.LogWarning("Inventory is null when receiving achievement objective " + Type);
			}
			else
			{
				MilMo_Player.Instance.Inventory.AddItemAddedListener(listener);
			}
			return;
		}
		MilMo_AchievementCounter counter = milMo_AchievementHandler.GetCounter(Type, Object);
		if (counter != null)
		{
			counter.AddListener(listener);
			return;
		}
		Debug.LogWarning("Got bad objective: " + Type + " " + Object + " " + Count + " in medal.");
		_listener = null;
	}

	public void UnregisterListener()
	{
		if (_listener == null)
		{
			return;
		}
		MilMo_AchievementHandler milMo_AchievementHandler = MilMo_AchievementHandler.Get();
		switch (Type)
		{
		case "CompleteMedal":
			milMo_AchievementHandler.RemoveAchievementCompletedListener(_listener);
			break;
		case "CompleteQuest":
			MilMo_Player.Instance.Quests.RemoveQuestCompletedListener(_listener);
			break;
		case "FindAllSecrets":
			MilMo_Level.RemoveAllExplorationTokensFoundListener(_listener);
			break;
		default:
		{
			if (Type == "Collect" && Count == 1)
			{
				MilMo_Player.Instance.Inventory.RemoveItemAddedListener(_listener);
				break;
			}
			MilMo_AchievementCounter counter = milMo_AchievementHandler.GetCounter(Type, Object);
			if (counter != null)
			{
				counter.RemoveListener(_listener);
				break;
			}
			Debug.LogWarning("Failed to remove listener. Counter " + Type + " " + Object + " not found.");
			return;
		}
		}
		_listener = null;
	}
}
