using System.Collections.Generic;
using System.Linq;
using Code.Core.Command;
using Code.Core.EventSystem;
using Code.Core.Network.messages.server;
using Code.World.Level;
using Code.World.Player;
using UnityEngine;

namespace Code.World.Quest;

public static class MilMo_QuestAreaManager
{
	private static readonly Dictionary<string, IList<MilMo_QuestArea>> QuestAreas = new Dictionary<string, IList<MilMo_QuestArea>>();

	public static void Initialize()
	{
		MilMo_EventSystem.Listen("world_quest_area_subscribe", Subscribe).Repeating = true;
		MilMo_EventSystem.Listen("world_quest_area_unsubscribe", Unsubscribe).Repeating = true;
		MilMo_Command.Instance.RegisterCommand("QuestArea.Count", Debug_NumberOfQuestAreas);
		MilMo_Command.Instance.RegisterCommand("QuestArea.Print", Debug_PrintQuestAreas);
	}

	public static void Update()
	{
		MilMo_Player instance = MilMo_Player.Instance;
		if (instance == null || !instance.InInstance || MilMo_Level.CurrentLevel == null)
		{
			return;
		}
		string verboseName = MilMo_Level.CurrentLevel.VerboseName;
		if (!QuestAreas.TryGetValue(verboseName, out var value))
		{
			return;
		}
		foreach (MilMo_QuestArea item in value)
		{
			item.Update();
		}
	}

	private static void Subscribe(object msgAsObject)
	{
		if (msgAsObject is ServerQuestAreaSubscribe serverQuestAreaSubscribe)
		{
			string fullAreaName = serverQuestAreaSubscribe.getFullAreaName();
			Vector3 center = new Vector3(serverQuestAreaSubscribe.getCenter().GetX(), serverQuestAreaSubscribe.getCenter().GetY(), serverQuestAreaSubscribe.getCenter().GetZ());
			float radiusSquared = serverQuestAreaSubscribe.getRadiusSquared();
			float height = serverQuestAreaSubscribe.getHeight();
			MilMo_QuestArea milMo_QuestArea = new MilMo_QuestArea(fullAreaName, center, radiusSquared, height);
			Debug.Log("Subscribing to " + fullAreaName + " in " + milMo_QuestArea.FullLevelName);
			if (QuestAreas.TryGetValue(milMo_QuestArea.FullLevelName, out var value))
			{
				value.Add(milMo_QuestArea);
				return;
			}
			value = new List<MilMo_QuestArea> { milMo_QuestArea };
			QuestAreas.Add(milMo_QuestArea.FullLevelName, value);
		}
	}

	private static void Unsubscribe(object msgAsObject)
	{
		if (!(msgAsObject is ServerQuestAreaUnsubscribe serverQuestAreaUnsubscribe))
		{
			return;
		}
		string fullAreaName = serverQuestAreaUnsubscribe.getFullAreaName();
		string[] array = fullAreaName.Split(':');
		string text = array[0] + ":" + array[1];
		string text2 = array[2];
		Debug.Log("Unsubscribing from " + fullAreaName);
		if (QuestAreas.TryGetValue(text, out var value))
		{
			for (int i = 0; i < value.Count; i++)
			{
				if (!(value[i].Name != text2))
				{
					value[i].Remove();
					value.RemoveAt(i);
					return;
				}
			}
			Debug.LogWarning("Unsubscribing from an area that player isn't subscribed to. " + text + ", " + text2 + "(1)");
		}
		else
		{
			Debug.LogWarning("Unsubscribing from an area that player isn't subscribed to. " + text + ", " + text2 + "(2)");
		}
	}

	private static string Debug_NumberOfQuestAreas(string[] args)
	{
		return "Subscribed to " + QuestAreas.Values.Cast<List<MilMo_QuestArea>>().Sum((List<MilMo_QuestArea> questAreas) => questAreas.Count) + " quest areas";
	}

	private static string Debug_PrintQuestAreas(string[] args)
	{
		string text = "==========================\n";
		foreach (MilMo_QuestArea item in QuestAreas.Values.Cast<List<MilMo_QuestArea>>().SelectMany((List<MilMo_QuestArea> questAreas) => questAreas))
		{
			text = text + " " + item.FullLevelName + ":" + item.Name + "\n";
		}
		return text + "==========================";
	}
}
