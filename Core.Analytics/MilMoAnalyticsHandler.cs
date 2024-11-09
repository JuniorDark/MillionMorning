using System;
using System.Collections.Generic;
using Code.World.Player;
using Unity.Services.Analytics;
using UnityEngine;

namespace Core.Analytics;

public static class MilMoAnalyticsHandler
{
	public static bool IsEnabled = true;

	public static void FirstTimePlayer()
	{
		SendAnalytics("FirstTimePlayer", new Dictionary<string, object>());
	}

	public static void NpcInteraction(string npcId)
	{
		Dictionary<string, object> parameters = new Dictionary<string, object> { { "npcId", npcId } };
		SendAnalytics("npcInteraction", parameters);
	}

	private static void SendAnalytics(string eventName, Dictionary<string, object> parameters)
	{
		if (!IsEnabled)
		{
			return;
		}
		try
		{
			int num = int.Parse(MilMo_Player.Instance.Id ?? "0");
			parameters.Add("PlayerId", num);
			Debug.LogWarning("Analytics: " + eventName);
			AnalyticsService.Instance.CustomData(eventName, parameters);
			AnalyticsService.Instance.Flush();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public static void NpcInteractionState(string npcId, int state)
	{
		Dictionary<string, object> parameters = new Dictionary<string, object>
		{
			{ "npcId", npcId },
			{ "InteractionType", state }
		};
		SendAnalytics("NpcInteractionState", parameters);
	}

	public static void LevelStart(string levelId, float loadTime)
	{
		Dictionary<string, object> parameters = new Dictionary<string, object>
		{
			{ "LevelId", levelId },
			{ "LoadTime", loadTime }
		};
		SendAnalytics("LevelStart", parameters);
	}

	public static void LevelQuit(string levelId)
	{
		Dictionary<string, object> parameters = new Dictionary<string, object> { { "LevelId", levelId } };
		SendAnalytics("LevelQuit", parameters);
	}

	public static void SceneVisit(string screenId)
	{
		Dictionary<string, object> parameters = new Dictionary<string, object> { { "SceneId", screenId } };
		SendAnalytics("SceneVisit", parameters);
	}

	public static void SceneLeave(string screenId)
	{
		Dictionary<string, object> parameters = new Dictionary<string, object> { { "SceneId", screenId } };
		SendAnalytics("SceneLeave", parameters);
	}

	public static void StartCutscene(string cutsceneId)
	{
		Dictionary<string, object> parameters = new Dictionary<string, object> { { "CutsceneId", cutsceneId } };
		SendAnalytics("StartCutscene", parameters);
	}

	public static void SkipCutscene(string cutsceneId)
	{
		Dictionary<string, object> parameters = new Dictionary<string, object> { { "CutsceneId", cutsceneId } };
		SendAnalytics("SkipCutscene", parameters);
	}

	public static void EndCutscene(string cutsceneId)
	{
		Dictionary<string, object> parameters = new Dictionary<string, object> { { "CutsceneId", cutsceneId } };
		SendAnalytics("EndCutscene", parameters);
	}

	public static void ChangeWorld(string worldId)
	{
		Dictionary<string, object> parameters = new Dictionary<string, object> { { "WorldId", worldId } };
		SendAnalytics("ChangeWorld", parameters);
	}

	public static void OpenCompass(string levelId)
	{
		Dictionary<string, object> parameters = new Dictionary<string, object> { { "LevelId", levelId } };
		SendAnalytics("OpenCompass", parameters);
	}

	public static void CharbuilderStart()
	{
		SendAnalytics("CharbuilderStart", new Dictionary<string, object>());
	}

	public static void CharbuilderDone()
	{
		SendAnalytics("CharbuilderDone", new Dictionary<string, object>());
	}

	public static void CharacterCreated(string characterName)
	{
		Dictionary<string, object> parameters = new Dictionary<string, object> { { "CharacterName", characterName } };
		SendAnalytics("CharacterCreated", parameters);
	}

	public static void CharacterCreatedFailed(string characterName, string reason)
	{
		Dictionary<string, object> parameters = new Dictionary<string, object>
		{
			{ "CharacterName", characterName },
			{ "Reason", reason }
		};
		SendAnalytics("CharacterCreatedFailed", parameters);
	}

	public static void QuestEvent(string questId, int questStatus)
	{
		Dictionary<string, object> parameters = new Dictionary<string, object>
		{
			{ "QuestId", questId },
			{ "QuestStatus", questStatus }
		};
		SendAnalytics("QuestEvent", parameters);
	}

	public static void CoinCollected(string levelId, string progress)
	{
		Dictionary<string, object> parameters = new Dictionary<string, object>
		{
			{ "LevelId", levelId },
			{ "Progress", progress }
		};
		SendAnalytics("CoinCollected", parameters);
	}

	public static void ExplorationTokenCollected(string levelId, string progress)
	{
		Dictionary<string, object> parameters = new Dictionary<string, object>
		{
			{ "LevelId", levelId },
			{ "Progress", progress }
		};
		SendAnalytics("CoinCollected", parameters);
	}

	public static void GameOver(float fps, float ping)
	{
		Dictionary<string, object> parameters = new Dictionary<string, object>
		{
			{ "MedianFPS", fps },
			{ "MedianPing", ping }
		};
		SendAnalytics("GameOver", parameters);
	}

	public static void OpenTutorial(string tutorialId)
	{
		Dictionary<string, object> parameters = new Dictionary<string, object> { { "TutorialId", tutorialId } };
		SendAnalytics("OpenTutorial", parameters);
	}

	public static void CloseTutorial(string tutorialId)
	{
		Dictionary<string, object> parameters = new Dictionary<string, object> { { "TutorialId", tutorialId } };
		SendAnalytics("CloseTutorial", parameters);
	}

	public static void PlayerExhausted(string levelId, string position)
	{
		Dictionary<string, object> parameters = new Dictionary<string, object>
		{
			{ "LevelId", levelId },
			{ "LevelPosition", position }
		};
		SendAnalytics("PlayerExhausted", parameters);
	}

	public static void ItemPickup(string itemIdentifier)
	{
		Dictionary<string, object> parameters = new Dictionary<string, object> { { "ItemIdentifier", itemIdentifier } };
		SendAnalytics("ItemPickup", parameters);
	}
}
