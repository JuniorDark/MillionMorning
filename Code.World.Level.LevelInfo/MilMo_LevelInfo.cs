using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Command;
using Code.Core.EventSystem;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.World.GUI.LoadingScreen;
using Code.World.Level.LevelObject;
using Code.World.Player;
using Core;
using Core.State;
using Player;
using UI.HUD.States;
using UnityEngine;

namespace Code.World.Level.LevelInfo;

public static class MilMo_LevelInfo
{
	private sealed class WorldState
	{
		public string Level { get; }

		public bool Locked { get; }

		public WorldState(string level, bool locked)
		{
			Level = level;
			Locked = locked;
		}
	}

	private static readonly string[] AlwaysGUIUnlocked = new string[2] { "World00:Level08", "World01:Level12" };

	private static List<string> _theLevelListPaths;

	private const string THE_CHATROOM_LIST_PATH = "LevelInfo/Chatrooms";

	private const string THE_WORLD_LIST_PATH = "LevelInfo/Worlds";

	private static Dictionary<string, MilMo_LevelInfoData> _theLevelInfoData;

	private static Dictionary<string, MilMo_LevelState> _theLevelStates;

	private static Dictionary<string, WorldState> _theWorldLevels;

	private static readonly List<string> HiddenWorlds = new List<string>();

	private static readonly List<MilMo_WorldMapSplineData> WorldMapSplineData = new List<MilMo_WorldMapSplineData>();

	private static List<MilMo_WorldInfoData> _theWorldInfoData;

	private static List<MilMo_WorldImageInfoData> _theWorldMapImages;

	private static MilMo_GenericReaction _worldLevelsDataListener;

	private static MilMo_GenericReaction _worldLevelChangedDataListener;

	private static MilMo_GenericReaction _levelStateUnlockedListener;

	private static bool _levelsLoaded;

	private static bool IsReady { get; set; }

	public static void StartLevelInfoSystem()
	{
		if (IsReady)
		{
			Debug.LogWarning("StartLevelInfoSystem should only be called once");
			return;
		}
		MilMo_Command.Instance.RegisterCommand("Player.PrintLevelStates", Debug_PrintPlayerLevelStates);
		MilMo_Command.Instance.RegisterCommand("Player.PrintWorldLevels", Debug_PrintWorldLevels);
		MilMo_Command.Instance.RegisterCommand("Player.UnlockGUI", Debug_UnlockGUI);
		MilMo_Command.Instance.RegisterCommand("Player.Travel", Debug_Travel);
		_theWorldLevels = new Dictionary<string, WorldState>();
		_worldLevelsDataListener = MilMo_EventSystem.Listen("world_levels", delegate(object msg)
		{
			if (!(msg is ServerWorldStates serverWorldStates))
			{
				return;
			}
			_theWorldLevels.Clear();
			foreach (WorldLevel worldLevel in serverWorldStates.getWorldLevels())
			{
				_theWorldLevels.Add(worldLevel.GetWorld(), new WorldState(worldLevel.GetLevel(), worldLevel.GetLocked() != 0));
			}
		});
		_worldLevelsDataListener.Repeating = true;
		_worldLevelChangedDataListener = MilMo_EventSystem.Listen("world_level_updated", delegate(object msg)
		{
			if (msg is ServerWorldLevelUpdate serverWorldLevelUpdate)
			{
				if (!_theWorldLevels.ContainsKey(serverWorldLevelUpdate.getWorld()))
				{
					Debug.LogWarning("Got world level update for world that isn't recognized on the client. " + serverWorldLevelUpdate.getWorld());
					_theWorldLevels.Add(serverWorldLevelUpdate.getWorld(), new WorldState(serverWorldLevelUpdate.getLevel(), serverWorldLevelUpdate.getLocked() != 0));
					MilMo_EventSystem.Instance.PostEvent("world_level_changed", serverWorldLevelUpdate.getWorld());
				}
				else
				{
					_theWorldLevels[serverWorldLevelUpdate.getWorld()] = new WorldState(serverWorldLevelUpdate.getLevel(), serverWorldLevelUpdate.getLocked() != 0);
					MilMo_EventSystem.Instance.PostEvent("world_level_changed", serverWorldLevelUpdate.getWorld());
				}
			}
		});
		_worldLevelChangedDataListener.Repeating = true;
		_theLevelStates = new Dictionary<string, MilMo_LevelState>();
		_levelStateUnlockedListener = MilMo_EventSystem.Listen("level_state_unlocked", delegate(object msg)
		{
			if (msg is ServerLevelStateUnlocked serverLevelStateUnlocked)
			{
				MilMo_LevelState milMo_LevelState = new MilMo_LevelState(serverLevelStateUnlocked.getFullLevelName(), guiUnlocked: false);
				if (_theLevelStates.ContainsKey(milMo_LevelState.FullLevelName))
				{
					_theLevelStates[milMo_LevelState.FullLevelName] = milMo_LevelState;
				}
				else
				{
					_theLevelStates.Add(milMo_LevelState.FullLevelName, milMo_LevelState);
				}
			}
		});
		_levelStateUnlockedListener.Repeating = true;
		CreateLevelListPaths();
		IsReady = true;
	}

	public static void LoadLevels()
	{
		if (_levelsLoaded)
		{
			return;
		}
		_theLevelInfoData = new Dictionary<string, MilMo_LevelInfoData>();
		_theWorldInfoData = new List<MilMo_WorldInfoData>();
		_theWorldMapImages = new List<MilMo_WorldImageInfoData>();
		MilMo_SFFile milMo_SFFile = MilMo_SimpleFormat.LoadLocal("LevelInfo/Chatrooms");
		if (milMo_SFFile == null)
		{
			Debug.LogWarning("Failed to load chatroom information. This is fatal.");
			return;
		}
		while (milMo_SFFile.NextRow())
		{
			if (milMo_SFFile.IsNext("<LEVEL>"))
			{
				MilMo_LevelInfoData milMo_LevelInfoData = new MilMo_LevelInfoData();
				if (milMo_LevelInfoData.Read(milMo_SFFile))
				{
					_theLevelInfoData.Add(milMo_LevelInfoData.FullLevelName, milMo_LevelInfoData);
				}
				else
				{
					Debug.LogWarning("Failed to load level info data in " + milMo_SFFile.Path + " at line " + milMo_SFFile.GetLineNumber());
				}
			}
		}
		foreach (string theLevelListPath in _theLevelListPaths)
		{
			string world = theLevelListPath.Split('/')[1].Replace("Levels", "").Replace("W", "World");
			MilMo_SFFile milMo_SFFile2 = MilMo_SimpleFormat.LoadLocal(theLevelListPath);
			if (milMo_SFFile2 == null)
			{
				Debug.LogWarning("Failed to load world and level information. This is fatal.");
				return;
			}
			while (milMo_SFFile2.NextRow())
			{
				if (milMo_SFFile2.IsNext("<WORLDMAP>"))
				{
					MilMo_WorldInfoData milMo_WorldInfoData = new MilMo_WorldInfoData(!HiddenWorlds.Contains(theLevelListPath));
					if (milMo_WorldInfoData.Read(milMo_SFFile2))
					{
						_theWorldInfoData.Add(milMo_WorldInfoData);
					}
					else
					{
						Debug.LogWarning($"Failed to load world info data in {milMo_SFFile2.Path} at line {milMo_SFFile2.GetLineNumber()}");
					}
				}
				else if (milMo_SFFile2.IsNext("<IMAGE>"))
				{
					MilMo_WorldImageInfoData milMo_WorldImageInfoData = new MilMo_WorldImageInfoData(world);
					if (milMo_WorldImageInfoData.Read(milMo_SFFile2))
					{
						_theWorldMapImages.Add(milMo_WorldImageInfoData);
					}
					else
					{
						Debug.LogWarning($"Failed to load world map image data in {milMo_SFFile2.Path} at line {milMo_SFFile2.GetLineNumber()}");
					}
				}
				else if (milMo_SFFile2.IsNext("<LEVEL>"))
				{
					MilMo_LevelInfoData milMo_LevelInfoData2 = new MilMo_LevelInfoData();
					if (milMo_LevelInfoData2.Read(milMo_SFFile2))
					{
						_theLevelInfoData.Add(milMo_LevelInfoData2.FullLevelName, milMo_LevelInfoData2);
					}
					else
					{
						Debug.LogWarning($"Failed to load level info data in {milMo_SFFile2.Path} at line {milMo_SFFile2.GetLineNumber()}");
					}
				}
				else if (milMo_SFFile2.IsNext("<SPLINE>"))
				{
					MilMo_WorldMapSplineData milMo_WorldMapSplineData = new MilMo_WorldMapSplineData();
					if (milMo_WorldMapSplineData.Read(milMo_SFFile2))
					{
						WorldMapSplineData.Add(milMo_WorldMapSplineData);
					}
					else
					{
						Debug.LogWarning($"Failed to load world map spline data in {milMo_SFFile2.Path} at line {milMo_SFFile2.GetLineNumber()}");
					}
				}
			}
		}
		_levelsLoaded = true;
	}

	public static void ReadAll(IEnumerable<LevelStateData> levelStates, IEnumerable<WorldLevel> worldLevels, IEnumerable<FoundTokensInfo> explorationTokens, IEnumerable<FoundCoinTokensInfo> coinTokens, IEnumerable<PremiumToken> premiumTokens)
	{
		_theLevelStates.Clear();
		foreach (LevelStateData levelState in levelStates)
		{
			MilMo_LevelState milMo_LevelState = new MilMo_LevelState(levelState.GetFullLevelName(), levelState.GetGUIUnlocked() == 1);
			_theLevelStates.Add(milMo_LevelState.FullLevelName, milMo_LevelState);
		}
		_theWorldLevels.Clear();
		foreach (WorldLevel worldLevel in worldLevels)
		{
			_theWorldLevels.Add(worldLevel.GetWorld(), new WorldState(worldLevel.GetLevel(), worldLevel.GetLocked() != 0));
		}
		foreach (FoundTokensInfo explorationToken in explorationTokens)
		{
			if (_theLevelInfoData.ContainsKey(explorationToken.GetLevel()))
			{
				MilMo_LevelInfoData milMo_LevelInfoData = _theLevelInfoData[explorationToken.GetLevel()];
				int tokensFound = explorationToken.GetTokensFound();
				sbyte tokensAmount = explorationToken.GetTokensAmount();
				milMo_LevelInfoData.ExplorationTokens.Clear();
				for (int i = 0; i < tokensAmount; i++)
				{
					int num = 1 << i;
					bool item = (tokensFound & num) != 0;
					milMo_LevelInfoData.ExplorationTokens.Add(item);
				}
			}
			else
			{
				Debug.Log("Got exploration token for unknown level " + explorationToken.GetLevel());
			}
		}
		foreach (FoundCoinTokensInfo coinToken in coinTokens)
		{
			if (_theLevelInfoData.ContainsKey(coinToken.GetLevel()))
			{
				MilMo_LevelInfoData milMo_LevelInfoData2 = _theLevelInfoData[coinToken.GetLevel()];
				int tokensFound2 = coinToken.GetTokensFound();
				sbyte tokensAmount2 = coinToken.GetTokensAmount();
				milMo_LevelInfoData2.CoinTokens.Clear();
				for (int j = 0; j < tokensAmount2; j++)
				{
					int num2 = 1 << j;
					bool item2 = (tokensFound2 & num2) != 0;
					milMo_LevelInfoData2.CoinTokens.Add(item2);
				}
			}
			else
			{
				Debug.Log("Got coin token for unknown level " + coinToken.GetLevel());
			}
		}
		foreach (PremiumToken premiumToken in premiumTokens)
		{
			if (_theLevelInfoData.ContainsKey(premiumToken.GetLevel()))
			{
				_theLevelInfoData[premiumToken.GetLevel()].PremiumToken = premiumToken;
				if (premiumToken.GetIsFound() == 0 && MilMo_Player.Instance.IsMember && MilMo_Level.CurrentLevel != null && MilMo_Level.CurrentLevel.VerboseName == premiumToken.GetLevel())
				{
					MilMo_EventSystem.Instance.PostEvent("level_premiumtoken_create", premiumToken);
				}
			}
			else
			{
				Debug.Log("Got premium token for unknown level " + premiumToken.GetLevel());
			}
		}
		MilMo_EventSystem.Listen("premium_tokens_data", UpdatePremiumTokens).Repeating = true;
	}

	private static void CreateLevelListPaths()
	{
		_theLevelListPaths = new List<string>();
		MilMo_SFFile milMo_SFFile = MilMo_SimpleFormat.LoadLocal("LevelInfo/Worlds");
		if (milMo_SFFile == null)
		{
			Debug.LogError("Failed to load World information. This is fatal.");
			return;
		}
		while (milMo_SFFile.NextRow())
		{
			if (milMo_SFFile.IsNext("World"))
			{
				string @string = milMo_SFFile.GetString();
				_theLevelListPaths.Add(@string);
				if (milMo_SFFile.HasMoreTokens() && milMo_SFFile.IsNext("NotVisibleInGUI"))
				{
					HiddenWorlds.Add(@string);
				}
			}
		}
	}

	public static MilMo_LevelInfoData GetLastLevelInfoForWorld(string world)
	{
		if (_theWorldLevels.ContainsKey(world))
		{
			return GetLevelInfoData(world + ":" + _theWorldLevels[world].Level);
		}
		Debug.LogWarning("Failed to get last level info for world " + world);
		return null;
	}

	public static IList<MilMo_LevelInfoData> GetPvpLevels()
	{
		return _theLevelInfoData.Values.Where((MilMo_LevelInfoData levelInfo) => levelInfo.IsPvp).ToList();
	}

	public static IList<MilMo_LevelInfoData> GetChatRooms()
	{
		return _theLevelInfoData.Values.Where((MilMo_LevelInfoData chatroomInfo) => chatroomInfo.IsChatRoom && !chatroomInfo.IsPvp).ToList();
	}

	public static MilMo_WorldInfoData GetWorldInfoData(string world)
	{
		using (IEnumerator<MilMo_WorldInfoData> enumerator = _theWorldInfoData.Where((MilMo_WorldInfoData wid) => wid.World == world).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
		}
		throw new InvalidOperationException("World " + world + " does not exist in world info.");
	}

	public static bool IsWorldLocked(string world)
	{
		return _theWorldLevels[world].Locked;
	}

	public static IEnumerable<MilMo_WorldInfoData> GetWorldInfoDataArray()
	{
		return _theWorldInfoData;
	}

	public static IEnumerable<MilMo_WorldImageInfoData> GetImagesInWorld(string world)
	{
		return _theWorldMapImages.Where((MilMo_WorldImageInfoData image) => image.World == world).ToList();
	}

	public static IEnumerable<MilMo_LevelInfoData> GetLevelsInWorld(string world)
	{
		return _theLevelInfoData?.Values.Where((MilMo_LevelInfoData levelInfo) => levelInfo.World == world && !levelInfo.IsChatRoom).ToList();
	}

	public static IEnumerable<MilMo_LevelInfoData> GetLevelInfoDataArray()
	{
		return _theLevelInfoData?.Keys.Select((string level) => _theLevelInfoData[level]).ToList();
	}

	public static IEnumerable<MilMo_WorldMapSplineData> GetSplinesInWorld(string world)
	{
		return WorldMapSplineData.Where((MilMo_WorldMapSplineData splineData) => splineData.World == world).ToList();
	}

	public static bool IsChatroom(string fullLevelName)
	{
		if (_theLevelInfoData.ContainsKey(fullLevelName))
		{
			return _theLevelInfoData[fullLevelName].IsChatRoom;
		}
		Debug.LogWarning("Checking if a level that does not exist is a chatroom (" + fullLevelName + ").");
		return false;
	}

	public static bool IsPvp(string fullLevelName)
	{
		if (_theLevelInfoData.ContainsKey(fullLevelName))
		{
			return _theLevelInfoData[fullLevelName].IsPvp;
		}
		Debug.LogWarning("Checking if a level that does not exist is a PVP level (" + fullLevelName + ").");
		return false;
	}

	public static bool IsStarterLevel(string fullLevelName)
	{
		if (_theLevelInfoData.ContainsKey(fullLevelName))
		{
			return _theLevelInfoData[fullLevelName].HudState == HudState.States.StarterLevel;
		}
		Debug.LogWarning("Checking if a level that does not exist is a Starter level (" + fullLevelName + ").");
		return false;
	}

	public static MilMo_LevelInfoData GetLevelInfoData(string fullLevelName)
	{
		if (string.IsNullOrEmpty(fullLevelName))
		{
			Debug.LogWarning("Requesting level info data for null!");
			return null;
		}
		if (_theLevelInfoData != null && _theLevelInfoData.ContainsKey(fullLevelName))
		{
			return _theLevelInfoData[fullLevelName];
		}
		Debug.Log("Requesting level info data for non existing level (" + fullLevelName + ").");
		return null;
	}

	public static MilMo_LocString GetLevelDisplayName(string fullLevelName)
	{
		MilMo_LevelInfoData levelInfoData = GetLevelInfoData(fullLevelName);
		if (levelInfoData != null)
		{
			return levelInfoData.DisplayName;
		}
		Debug.LogWarning("Requesting display name for non existing level " + fullLevelName);
		return MilMo_LocString.Empty;
	}

	public static bool IsUnlocked(string fullLevelName)
	{
		if (!_theLevelStates.ContainsKey(fullLevelName))
		{
			return AlwaysGUIUnlocked.Contains(fullLevelName);
		}
		return true;
	}

	public static bool HasBeenInLevel(string fullLevelName)
	{
		if (_theLevelStates.TryGetValue(fullLevelName, out var value))
		{
			return value.GUIUnlocked;
		}
		return false;
	}

	public static bool IsGUIUnlocked(string fullLevelName)
	{
		if (AlwaysGUIUnlocked.Contains(fullLevelName))
		{
			return true;
		}
		if (_theLevelStates.TryGetValue(fullLevelName, out var value))
		{
			return value.GUIUnlocked;
		}
		return false;
	}

	public static void UnlockGUI(string fullLevelName)
	{
		if (_theLevelStates.TryGetValue(fullLevelName, out var value))
		{
			value.SetGUIUnlocked();
		}
	}

	public static bool Travel(string fullLevelName, bool isChatroom = false, bool checkUnlocked = true)
	{
		if (MilMo_Player.Instance == null || MilMo_Player.Instance.Avatar == null)
		{
			return false;
		}
		bool flag = false;
		MilMo_LevelInfoData lastLevelInfoForWorld = GetLastLevelInfoForWorld(MilMo_World.CurrentWorld);
		if (lastLevelInfoForWorld != null)
		{
			flag = MilMo_World.CurrentWorld + ":" + lastLevelInfoForWorld.Level == fullLevelName;
		}
		if (!isChatroom && !flag && ((checkUnlocked && !IsUnlocked(fullLevelName)) || (MilMo_Player.Instance.Avatar.TeleportStones <= 0 && MilMo_Player.Instance.Avatar.Role == 0)))
		{
			return false;
		}
		MilMo_LevelInfoData levelInfo = GetLevelInfoData(fullLevelName);
		MilMo_Player.Instance.Teleporting = true;
		if (!Singleton<GroupManager>.Instance.PlayerIsInGroup)
		{
			MilMo_LoadingScreen.Instance.LevelLoadFade(12f);
			MilMo_EventSystem.At(0.25f, delegate
			{
				MilMo_LocString locString = MilMo_Localization.GetLocString("Generic_56");
				locString.SetFormatArgs(levelInfo.DisplayName);
				MilMo_LoadingScreen.Instance.SetLoadingText(locString);
				MilMo_LevelData.LoadAndSetLevelIcon(levelInfo.World, levelInfo.Level);
			});
		}
		if (flag)
		{
			MilMo_EventSystem.At(0.8f, delegate
			{
				Singleton<GameNetwork>.Instance.SendWorldTravelRequest(MilMo_World.CurrentWorld);
			});
		}
		else
		{
			MilMo_EventSystem.At(0.8f, delegate
			{
				Singleton<GameNetwork>.Instance.SendLevelTravelRequest(levelInfo.World, levelInfo.Level);
			});
		}
		return true;
	}

	private static void UpdatePremiumTokens(object updatedPremiumTokensAsObj)
	{
		if (!(updatedPremiumTokensAsObj is ServerPremiumTokens serverPremiumTokens))
		{
			return;
		}
		foreach (PremiumToken premiumToken in serverPremiumTokens.getPremiumTokens())
		{
			if (_theLevelInfoData.ContainsKey(premiumToken.GetLevel()))
			{
				_theLevelInfoData[premiumToken.GetLevel()].PremiumToken = premiumToken;
				if (premiumToken.GetIsFound() == 0 && MilMo_Player.Instance.IsMember && MilMo_Level.CurrentLevel != null && MilMo_Level.CurrentLevel.VerboseName == premiumToken.GetLevel())
				{
					MilMo_EventSystem.Instance.PostEvent("level_premiumtoken_create", premiumToken);
				}
			}
			else
			{
				Debug.Log("Got premium token for unknown level " + premiumToken.GetLevel());
			}
		}
	}

	public static void UpdateExplorationTokens(List<MilMo_ExplorationToken> updatedTokens, string levelVerboseName)
	{
		if (updatedTokens == null || !_theLevelInfoData.ContainsKey(levelVerboseName))
		{
			return;
		}
		MilMo_LevelInfoData milMo_LevelInfoData = _theLevelInfoData[levelVerboseName];
		milMo_LevelInfoData.ExplorationTokens.Clear();
		foreach (MilMo_ExplorationToken updatedToken in updatedTokens)
		{
			milMo_LevelInfoData.ExplorationTokens.Add(updatedToken.IsFound);
		}
		GlobalStates.Instance.levelState.explorationTokenState.tokensFound.Set(milMo_LevelInfoData.NumberOfExplorationTokensFound);
		GlobalStates.Instance.levelState.explorationTokenState.tokensMax.Set(milMo_LevelInfoData.TotalNumberOfExplorationTokens);
	}

	public static void UpdateCoinTokens(IEnumerable<MilMo_CoinToken> updatedTokens, string levelVerboseName)
	{
		if (updatedTokens == null || !_theLevelInfoData.ContainsKey(levelVerboseName))
		{
			return;
		}
		MilMo_LevelInfoData milMo_LevelInfoData = _theLevelInfoData[levelVerboseName];
		milMo_LevelInfoData.CoinTokens.Clear();
		foreach (MilMo_CoinToken updatedToken in updatedTokens)
		{
			milMo_LevelInfoData.CoinTokens.Add(updatedToken.IsFound);
		}
		GlobalStates.Instance.levelState.coinState.tokensFound.Set(milMo_LevelInfoData.NumberOfCoinTokensFound);
		GlobalStates.Instance.levelState.coinState.tokensMax.Set(milMo_LevelInfoData.TotalNumberOfCoinTokens);
	}

	public static void PremiumTokenFound(string level)
	{
		if (_theLevelInfoData.ContainsKey(level) && _theLevelInfoData[level].PremiumToken != null)
		{
			_theLevelInfoData[level].PremiumToken = new PremiumToken(level, 0, 0f, _theLevelInfoData[level].PremiumToken.GetPosition(), 1);
		}
	}

	private static string Debug_PrintPlayerLevelStates(string[] args)
	{
		return _theLevelStates.Values.Aggregate("====================\n", (string current, MilMo_LevelState levelState) => current + levelState.FullLevelName + " (" + GetLevelDisplayName(levelState.FullLevelName)?.ToString() + ") guiUnlocked=" + levelState.GUIUnlocked + "\n") + "====================";
	}

	private static string Debug_PrintWorldLevels(string[] args)
	{
		string text = "====================\n";
		foreach (string key in _theWorldLevels.Keys)
		{
			string level = _theWorldLevels[key].Level;
			bool locked = _theWorldLevels[key].Locked;
			text = text + key + ":" + level + " (" + (locked ? "locked" : "not locked") + ")\n";
		}
		return text + "====================";
	}

	private static string Debug_UnlockGUI(string[] args)
	{
		if (MilMo_Level.CurrentLevel == null)
		{
			return "Not in a level";
		}
		UnlockGUI(MilMo_Level.CurrentLevel.VerboseName);
		return "GUI unlocked for '" + MilMo_Level.CurrentLevel.DisplayName?.ToString() + "'";
	}

	private static string Debug_Travel(string[] args)
	{
		if (args.Length < 2)
		{
			return "usage: Player.Travel <fullLevelName>";
		}
		if (!Travel(args[1]))
		{
			return "Failed to issue travel request";
		}
		return "Travel request sent to server";
	}
}
