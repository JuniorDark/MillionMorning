using System.Collections.Generic;
using System.Linq;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Core;
using UI.HUD.Dialogues;
using UI.LockState;
using UnityEngine;

namespace Code.World.Tutorial;

public class MilMo_TutorialManager : Singleton<MilMo_TutorialManager>
{
	private readonly List<MilMo_Tutorial> _tutorials = new List<MilMo_Tutorial>();

	private readonly List<MilMo_TutorialArea> _areasCurrentLevel = new List<MilMo_TutorialArea>();

	private readonly Dictionary<string, int> _areaListenerCounters = new Dictionary<string, int>();

	public void Initialize()
	{
		LoadTutorials();
	}

	public void RefreshLockStates(LockStateManager lockStateManager)
	{
		lockStateManager.HasFoundGems = HasCompletedTutorial("ReceiveGem");
		lockStateManager.HasFoundCoins = HasCompletedTutorial("ReceiveCoin");
		lockStateManager.HasFoundTokens = HasCompletedTutorial("ReceiveExplorationToken");
		lockStateManager.HasFoundActionBar = HasCompletedTutorial("ReceiveConsumable") || HasCompletedTutorial("LevelUp");
		lockStateManager.HasUnlockedProfile = HasCompletedTutorial("ReceiveMedal");
		lockStateManager.HasUnlockedQuestLog = HasCompletedTutorial("TalkToAny");
		lockStateManager.HasUnlockedFriendList = false;
		lockStateManager.HasUnlockedShop = HasCompletedTutorial("UseCoins");
		lockStateManager.HasUnlockedTown = HasCompletedTutorial("Town");
		lockStateManager.HasUnlockedNavigator = HasCompletedTutorial("Town");
		lockStateManager.HasUnlockedBag = HasCompletedTutorial("ReceiveItemAny");
	}

	private bool HasCompletedTutorial(string tutorialName)
	{
		return _tutorials.Any((MilMo_Tutorial tutorial) => tutorial.IsComplete && tutorial.Name == tutorialName);
	}

	public void TestAllTutorials()
	{
		foreach (MilMo_Tutorial tutorial in _tutorials)
		{
			DialogueSpawner.SpawnTutorialDialogue(tutorial);
		}
	}

	public void ReadAll(IList<string> completedTutorials)
	{
		foreach (MilMo_Tutorial item in _tutorials.Where((MilMo_Tutorial tutorial) => completedTutorials.Contains(tutorial.Name)))
		{
			item.IsComplete = true;
		}
		foreach (MilMo_Tutorial item2 in _tutorials.Where((MilMo_Tutorial tutorial) => !completedTutorials.Contains(tutorial.Name)))
		{
			item2.SetupTriggers();
		}
	}

	public void RegisterAreaListener(string fullAreaName)
	{
		if (_areaListenerCounters.ContainsKey(fullAreaName))
		{
			_areaListenerCounters[fullAreaName]++;
		}
		else
		{
			_areaListenerCounters.Add(fullAreaName, 1);
		}
		foreach (MilMo_TutorialArea item in _areasCurrentLevel.Where((MilMo_TutorialArea area) => area.FullName.Equals(fullAreaName)))
		{
			item.Active = true;
		}
	}

	public void UnregisterAreaListener(string fullAreaName)
	{
		if (_areaListenerCounters.ContainsKey(fullAreaName))
		{
			_areaListenerCounters[fullAreaName]--;
		}
		if (_areaListenerCounters.ContainsKey(fullAreaName) && _areaListenerCounters[fullAreaName] != 0)
		{
			return;
		}
		foreach (MilMo_TutorialArea item in _areasCurrentLevel.Where((MilMo_TutorialArea area) => area.FullName.Equals(fullAreaName)))
		{
			item.Active = false;
		}
	}

	public void Update()
	{
		foreach (MilMo_TutorialArea item in _areasCurrentLevel)
		{
			item.Update();
		}
	}

	public void LoadAreas(MilMo_SFFile file, string world, string level)
	{
		_areasCurrentLevel.Clear();
		if (file == null)
		{
			return;
		}
		while (file.NextRow())
		{
			MilMo_TutorialArea milMo_TutorialArea = new MilMo_TutorialArea(world, level);
			if (milMo_TutorialArea.Load(file))
			{
				_areasCurrentLevel.Add(milMo_TutorialArea);
				if (_areaListenerCounters.ContainsKey(milMo_TutorialArea.FullName) && _areaListenerCounters[milMo_TutorialArea.FullName] > 0)
				{
					milMo_TutorialArea.Active = true;
				}
			}
		}
	}

	public void ClearAreas()
	{
		_areasCurrentLevel.Clear();
	}

	private void LoadTutorials()
	{
		foreach (MilMo_SFFile item in MilMo_SimpleFormat.LoadAllLocal("Tutorial"))
		{
			if (!(Singleton<MilMo_TemplateContainer>.Instance.GetTemplate("Tutorial", item.Path) is MilMo_TutorialTemplate template))
			{
				Debug.LogWarning("Failed to load tutorial " + item.Path);
			}
			else
			{
				_tutorials.Add(new MilMo_Tutorial(template));
			}
		}
	}
}
