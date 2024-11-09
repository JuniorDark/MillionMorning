using System.Collections.Generic;
using System.Linq;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UI.HUD.States;
using UnityEngine;

namespace Code.World.Level.LevelInfo;

public sealed class MilMo_LevelInfoData
{
	public string World { get; private set; }

	public string Level { get; private set; }

	public bool SkipNavigatorFeed { get; private set; }

	public MilMo_LocString DisplayName { get; private set; }

	public MilMo_LocString ShopDisplayName { get; private set; }

	public MilMo_LocString DisplayNameUpperCase { get; private set; }

	public MilMo_LocString WorldMapDescription { get; private set; }

	public Vector2 WorldMapPosition { get; set; }

	public MilMo_LocString FeedEventInGame { get; private set; }

	public string FeedEventExternal { get; private set; }

	public MilMo_LocString FeedDescriptionInGame { get; private set; }

	public string FeedDescriptionExternal { get; private set; }

	public bool InvisibleIfUnlocked { get; private set; }

	public int RequiredAvatarLevel { get; private set; }

	public bool IsMembersOnlyArea { get; private set; }

	public bool IsChatRoom { get; private set; }

	public bool IsPvp { get; private set; }

	public HudState.States HudState { get; private set; }

	public string DataPath { get; private set; }

	public MilMo_Mover.UpdateFunc PosMoveFunc { get; private set; }

	public Vector2 SinRate { get; private set; }

	public Vector2 SinAmp { get; private set; }

	public Vector2 PosMoveVel { get; private set; }

	public Vector2 PosMoveLoopReset { get; private set; }

	public Vector2 PosMoveLoopVal { get; private set; }

	public bool PosMoveLooping { get; private set; }

	public string FullLevelName => World + ":" + Level;

	public bool IsUnlocked
	{
		get
		{
			if (!IsChatRoom)
			{
				return MilMo_LevelInfo.IsUnlocked(FullLevelName);
			}
			return true;
		}
	}

	public bool IsGUIUnlocked => MilMo_LevelInfo.IsGUIUnlocked(FullLevelName);

	public string IconPath
	{
		get
		{
			string text = World.Replace("orld", "");
			string text2 = Level.Replace("evel", "");
			return "Content/Worlds/" + text + "/LevelIcons/LevelIcon" + text + text2;
		}
	}

	public List<bool> ExplorationTokens { get; }

	public List<bool> CoinTokens { get; }

	public PremiumToken PremiumToken { get; set; }

	public int TotalNumberOfCoinTokens => CoinTokens.Count;

	public int NumberOfCoinTokensFound => CoinTokens.Sum((bool tokenFound) => tokenFound ? 1 : 0);

	public int TotalNumberOfExplorationTokens => ExplorationTokens.Count;

	public int NumberOfExplorationTokensFound => ExplorationTokens.Count((bool tokenIsFound) => tokenIsFound);

	public MilMo_LevelInfoData()
	{
		SkipNavigatorFeed = false;
		WorldMapPosition = Vector2.zero;
		CoinTokens = new List<bool>();
		ExplorationTokens = new List<bool>();
		FeedDescriptionExternal = "";
		FeedDescriptionInGame = MilMo_LocString.Empty;
		FeedEventExternal = "";
		FeedEventInGame = MilMo_LocString.Empty;
		WorldMapDescription = MilMo_LocString.Empty;
		DisplayNameUpperCase = MilMo_LocString.Empty;
		DisplayName = MilMo_LocString.Empty;
		DataPath = "";
		PosMoveFunc = MilMo_Mover.UpdateFunc.Nothing;
		PosMoveVel = Vector2.zero;
		PosMoveLoopReset = Vector2.zero;
		PosMoveLoopVal = Vector2.zero;
		SinAmp = Vector2.zero;
		SinRate = Vector2.zero;
		HudState = UI.HUD.States.HudState.States.Normal;
	}

	public Vector2 WorldMapPositionRes(Vector2 res)
	{
		return new Vector2(WorldMapPosition.x * res.x, WorldMapPosition.y * res.y);
	}

	public void UnlockGUI()
	{
		MilMo_LevelInfo.UnlockGUI(FullLevelName);
	}

	public bool Read(MilMo_SFFile file)
	{
		while (file.NextRow())
		{
			if (file.IsNext("</LEVEL>"))
			{
				if (World != null)
				{
					return Level != null;
				}
				return false;
			}
			if (file.IsNext("HudState"))
			{
				HudState = file.GetString().ToUpper() switch
				{
					"NORMAL" => UI.HUD.States.HudState.States.Normal, 
					"STARTLEVEL" => UI.HUD.States.HudState.States.StarterLevel, 
					"PVP" => UI.HUD.States.HudState.States.Pvp, 
					"PVP_ABILITIES" => UI.HUD.States.HudState.States.PvpAbilities, 
					_ => UI.HUD.States.HudState.States.Normal, 
				};
			}
			else if (file.IsNext("World"))
			{
				World = file.GetString();
			}
			else if (file.IsNext("Level"))
			{
				Level = file.GetString();
			}
			else if (file.IsNext("SkipNavigatorFeed"))
			{
				SkipNavigatorFeed = true;
			}
			else if (file.IsNext("DisplayName"))
			{
				string @string = file.GetString();
				DisplayName = MilMo_Localization.GetLocString(@string);
			}
			else if (file.IsNext("DisplayNameUpperCase"))
			{
				DisplayNameUpperCase = MilMo_Localization.GetLocString(file.GetString());
			}
			else if (file.IsNext("ShopDisplayName"))
			{
				ShopDisplayName = MilMo_Localization.GetLocString(file.GetString());
			}
			else if (file.IsNext("WorldMapDescription"))
			{
				WorldMapDescription = MilMo_Localization.GetLocString(file.GetString());
			}
			else if (file.IsNext("WorldMapPosition"))
			{
				WorldMapPosition = file.GetVector2();
			}
			else if (file.IsNext("FeedDescriptionIngame"))
			{
				FeedDescriptionInGame = MilMo_Localization.GetLocString(file.GetString());
			}
			else if (file.IsNext("FeedDescriptionExternal"))
			{
				FeedDescriptionExternal = MilMo_Localization.GetLocString(file.GetString()).String;
			}
			else if (file.IsNext("FeedEventIngame"))
			{
				FeedEventInGame = MilMo_Localization.GetLocString(file.GetString());
			}
			else if (file.IsNext("FeedEventExternal"))
			{
				FeedEventExternal = MilMo_Localization.GetLocString(file.GetString()).String;
			}
			else if (file.IsNext("MembersOnly"))
			{
				IsMembersOnlyArea = true;
			}
			else if (file.IsNext("RequiredAvatarLevel"))
			{
				RequiredAvatarLevel = file.GetInt();
			}
			else if (file.IsNext("IsChatroom"))
			{
				IsChatRoom = true;
			}
			else if (file.IsNext("InvisibleIfUnlocked"))
			{
				InvisibleIfUnlocked = true;
			}
			else if (file.IsNext("PosMover.Looping"))
			{
				PosMoveLooping = true;
			}
			else if (file.IsNext("PosMover.Vel"))
			{
				PosMoveVel = file.GetVector2();
			}
			else if (file.IsNext("PosMover.LoopReset"))
			{
				PosMoveLoopReset = file.GetVector2();
			}
			else if (file.IsNext("PosMover.LoopVal"))
			{
				PosMoveLoopVal = file.GetVector2();
			}
			else if (file.IsNext("PosMover.SinAmp"))
			{
				SinAmp = file.GetVector2();
			}
			else if (file.IsNext("PosMover.SinRate"))
			{
				SinRate = file.GetVector2();
			}
			else if (file.IsNext("PosMover.UpdateFunc"))
			{
				PosMoveFunc = file.GetString().ToUpper() switch
				{
					"LINEAR" => MilMo_Mover.UpdateFunc.Linear, 
					"NOTHING" => MilMo_Mover.UpdateFunc.Nothing, 
					"SINUS" => MilMo_Mover.UpdateFunc.Sinus, 
					"SPRING" => MilMo_Mover.UpdateFunc.Spring, 
					_ => PosMoveFunc, 
				};
			}
			else if (file.IsNext("PVP"))
			{
				IsPvp = true;
			}
			else
			{
				if (!file.IsNext("DataPath"))
				{
					Debug.LogWarning($"Got unknown command in level travel info at line {file.GetLineNumber()} in file {file.Path}");
					return false;
				}
				DataPath = file.GetString();
			}
		}
		Debug.LogWarning($"Failed to load level travel information in {file.Path} on line {file.GetLineNumber()}");
		return false;
	}
}
