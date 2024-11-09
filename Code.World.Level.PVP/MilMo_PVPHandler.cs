using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Avatar;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.Network.messages.server;
using Code.Core.Network.messages.server.PVP;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.World.Gameplay;
using Code.World.GUI.PVP;
using Code.World.Player;
using UnityEngine;

namespace Code.World.Level.PVP;

public class MilMo_PVPHandler
{
	public enum MilMo_MatchState
	{
		Waiting,
		On,
		Scoreboard
	}

	private bool _matchStateLockedPlayer;

	private long? _nextMatchState;

	private readonly Dictionary<string, MilMo_PVPTeam> _playerTeamMap;

	private int _nrOfAlivePlayers;

	public MilMo_MatchState MatchState { get; set; }

	public bool IsTeamMode { get; private set; }

	public MilMo_MatchMode MatchMode { get; private set; }

	public int NrOfRounds { get; private set; }

	public int MaxRoundScore { get; private set; }

	public Dictionary<int, MilMo_PVPTeam> Teams { get; set; }

	public bool MatchHasEnded { get; set; }

	public int SecondsToNextMatchState
	{
		get
		{
			if (_nextMatchState.HasValue)
			{
				return (int)new TimeSpan(_nextMatchState.Value - DateTime.Now.Ticks).TotalSeconds;
			}
			return -1;
		}
		set
		{
			if (value > 0)
			{
				_nextMatchState = new TimeSpan(DateTime.Now.Ticks).Add(new TimeSpan(0, 0, value)).Ticks;
			}
			else
			{
				_nextMatchState = null;
			}
		}
	}

	public MilMo_PVPHandler()
	{
		MatchState = MilMo_MatchState.Waiting;
		_matchStateLockedPlayer = false;
		IsTeamMode = false;
		MatchMode = MilMo_MatchMode.DEATH_MATCH;
		NrOfRounds = 1;
		MaxRoundScore = 3;
		Teams = new Dictionary<int, MilMo_PVPTeam>();
		_playerTeamMap = new Dictionary<string, MilMo_PVPTeam>();
	}

	public void CreateAndStartShit()
	{
		MilMo_PVPQueueHandler.Create();
		MilMo_EventSystem.Listen("player_joins_team", PlayerJoinsTeam).Repeating = true;
		MilMo_EventSystem.Listen("player_leaves_team", PlayerLeavesTeam).Repeating = true;
		MilMo_EventSystem.Listen("team_composition", TeamsComposition).Repeating = true;
		MilMo_EventSystem.Listen("player_died", PlayerDied).Repeating = true;
		MilMo_EventSystem.Listen("start_capture_zone_countdown", StartCaptureZoneCountdown).Repeating = true;
		MilMo_EventSystem.Listen("stop_capture_zone_countdown", StopCaptureZoneCountdown).Repeating = true;
		MilMo_EventSystem.Listen("team_captured_zone", TeamCapturedZone).Repeating = true;
		MilMo_EventSystem.Listen("player_delivered_flag", PlayerDeliveredFlag).Repeating = true;
		MilMo_EventSystem.Listen("remote_player_added", HandleRemotePlayerAdded).Repeating = true;
		MilMo_EventSystem.Listen("players_alive_count", PlayerAliveCountReceived).Repeating = true;
	}

	public void GotLoadPvpLevelInfo(object msgAsObj)
	{
		if (msgAsObj is ServerPvPLevelInstanceInfo serverPvPLevelInstanceInfo)
		{
			IsTeamMode = serverPvPLevelInstanceInfo.isTeamMode();
			MatchMode = (MilMo_MatchMode)serverPvPLevelInstanceInfo.getMatchMode();
			NrOfRounds = serverPvPLevelInstanceInfo.getNrOfRounds();
			MaxRoundScore = serverPvPLevelInstanceInfo.getMaxRoundScore();
			MilMo_World.PvpModeInfoWindow.SetDescription(MatchMode, MaxRoundScore);
			MilMo_World.PvpModeInfoWindow.Open();
			MilMo_World.HudHandler.pvpTeamScore.SetEnabled(IsTeamMode);
			Debug.Log("GotLoadPvpLevelInfo, IsTeamMode: " + IsTeamMode + ", MatchMode: " + MatchMode.ToString() + ", NrOfRounds: " + NrOfRounds + ", MaxRoundScore: " + MaxRoundScore);
		}
	}

	private void HandleRemotePlayerAdded(object remoteAsObj)
	{
		_ = remoteAsObj is MilMo_RemotePlayer;
	}

	public void UpdateMatchState(object msgAsObj)
	{
		if (!(msgAsObj is ServerUpdateMatchState serverUpdateMatchState))
		{
			return;
		}
		SecondsToNextMatchState = serverUpdateMatchState.getSecondsToNextMatchState();
		MatchState = (MilMo_MatchState)serverUpdateMatchState.getMatchState();
		switch (MatchState)
		{
		case MilMo_MatchState.Scoreboard:
			MilMo_Level.CurrentLevel.ResetZones();
			MilMo_World.Instance.ChangePlayerController(MilMo_PlayerControllerBase.ControllerType.InGUIApp);
			MilMo_World.Instance.PlayerController.Lock(0f, playMoveAnimationOnUnlock: false);
			_matchStateLockedPlayer = true;
			PlayEndEffect(playerDied: false);
			break;
		case MilMo_MatchState.On:
			if (_matchStateLockedPlayer)
			{
				MilMo_World.Instance.ChangePlayerController(MilMo_PlayerControllerInGUIApp.PreviousControllerType);
				MilMo_World.Instance.PlayerController.Unlock();
				_matchStateLockedPlayer = false;
			}
			MilMo_World.PvpScoreBoard.Close(null);
			MilMo_World.HudHandler.theMenuBar.UpdatePvpKillCounter(0);
			MilMo_World.HudHandler.theMenuBar.UpdatePvpDeathCounter(0);
			break;
		case MilMo_MatchState.Waiting:
			MilMo_Level.CurrentLevel.ResetZones();
			MilMo_World.Instance.ChangePlayerController(MilMo_PlayerControllerBase.ControllerType.InGUIApp);
			MilMo_World.Instance.PlayerController.Lock(0f, playMoveAnimationOnUnlock: false);
			_matchStateLockedPlayer = true;
			break;
		}
	}

	public void ScoreBoardArrived(object msgAsObj)
	{
		if (!(msgAsObj is ServerScoreBoard serverScoreBoard))
		{
			return;
		}
		foreach (TeamScoreEntry teamEntry in serverScoreBoard.getTeamEntries())
		{
			if (Teams.TryGetValue(teamEntry.GetTeam().GetId(), out var value))
			{
				value.RoundsWon = teamEntry.GetRoundsWon();
				value.RoundScore = teamEntry.GetRoundScore();
			}
		}
		MatchHasEnded = serverScoreBoard.MatchHasEnded();
		MilMo_World.PvpScoreBoard.Scores = serverScoreBoard.getEntries();
		MilMo_World.PvpScoreBoard.TeamScores = serverScoreBoard.getTeamEntries();
		MilMo_World.HudHandler.pvpTeamScore.setScore(Teams.Values.ToList());
		if (MatchHasEnded)
		{
			MilMo_World.PvpScoreBoard.Open();
		}
		else
		{
			MilMo_World.PvpScoreBoard.Refresh();
		}
	}

	public void KillsDeathsArrived(object msgAsObj)
	{
		if (msgAsObj is ServerUpdateKillsDeaths serverUpdateKillsDeaths)
		{
			MilMo_World.HudHandler.theMenuBar.UpdatePvpKillCounter(serverUpdateKillsDeaths.getKills());
			if (!MatchMode.Equals(MilMo_MatchMode.BATTLE_ROYALE))
			{
				MilMo_World.HudHandler.theMenuBar.UpdatePvpDeathCounter(serverUpdateKillsDeaths.getDeaths());
			}
		}
	}

	private void PlayerJoinsTeam(object msgAsObject)
	{
		if (!(msgAsObject is ServerPlayerJoinsTeam serverPlayerJoinsTeam))
		{
			return;
		}
		if (!Teams.ContainsKey(serverPlayerJoinsTeam.GetTeamId()))
		{
			Debug.LogWarning($"Could not add player {serverPlayerJoinsTeam.GetPlayerId()} to team {serverPlayerJoinsTeam.GetTeamId()}");
			return;
		}
		Teams[serverPlayerJoinsTeam.GetTeamId()].Players.Add(serverPlayerJoinsTeam.GetPlayerId());
		_playerTeamMap.Add(serverPlayerJoinsTeam.GetPlayerId(), Teams[serverPlayerJoinsTeam.GetTeamId()]);
		Debug.LogWarning("Team " + serverPlayerJoinsTeam.GetTeamId() + " now contains members " + Teams[serverPlayerJoinsTeam.GetTeamId()].Players.Aggregate((string a, string b) => a + " " + b));
	}

	private void PlayerLeavesTeam(object msgAsObject)
	{
		if (msgAsObject is ServerPlayerLeavesTeam serverPlayerLeavesTeam)
		{
			if (!Teams.ContainsKey(serverPlayerLeavesTeam.GetTeamId()))
			{
				Debug.LogWarning($"Could not remove player {serverPlayerLeavesTeam.GetPlayerId()} from team {serverPlayerLeavesTeam.GetTeamId()}");
				return;
			}
			Teams[serverPlayerLeavesTeam.GetTeamId()].Players.Remove(serverPlayerLeavesTeam.GetPlayerId());
			_playerTeamMap.Remove(serverPlayerLeavesTeam.GetPlayerId());
		}
	}

	private void TeamsComposition(object msgAsObject)
	{
		if (!(msgAsObject is ServerTeamsComposition serverTeamsComposition))
		{
			Debug.LogWarning("Team composition message was null");
			return;
		}
		Teams.Clear();
		_playerTeamMap.Clear();
		foreach (NetworkTeam team in serverTeamsComposition.GetTeams())
		{
			int id = team.GetId();
			string name = team.GetName();
			Color asColor = team.GetColor().getAsColor();
			IList<string> players = team.GetPlayers();
			MilMo_PVPTeam value = new MilMo_PVPTeam(id, players, asColor, name);
			Teams.Add(id, value);
			foreach (string item in players)
			{
				_playerTeamMap.Add(item, value);
			}
		}
		MilMo_World.HudHandler.pvpTeamScore.setScore(Teams.Values.ToList());
	}

	public bool IsTeamMate(MilMo_RemotePlayer player)
	{
		if (_playerTeamMap.TryGetValue(MilMo_Player.Instance.Id, out var value))
		{
			return value.Players.Contains(player.Id);
		}
		return false;
	}

	public bool IsTeamMate(string playerId)
	{
		if (_playerTeamMap.TryGetValue(MilMo_Player.Instance.Id, out var value))
		{
			return value.Players.Contains(playerId);
		}
		return false;
	}

	public Color GetTeamColor(string playerId)
	{
		if (!_playerTeamMap.TryGetValue(playerId, out var value))
		{
			return Color.black;
		}
		return value.Color;
	}

	public Color GetTeamColor(int teamId)
	{
		if (!Teams.TryGetValue(teamId, out var value))
		{
			return Color.black;
		}
		return value.Color;
	}

	public MilMo_PVPTeam GetTeam(int teamId)
	{
		if (!Teams.TryGetValue(teamId, out var value))
		{
			return null;
		}
		return value;
	}

	public string GetTeamName(string playerId)
	{
		if (!_playerTeamMap.TryGetValue(MilMo_Player.Instance.Id, out var value))
		{
			return "";
		}
		return value.Name;
	}

	private void StartCaptureZoneCountdown(object msgAsObject)
	{
		if (!(msgAsObject is ServerStartCaptureZoneCountdown serverStartCaptureZoneCountdown))
		{
			return;
		}
		MilMo_GameplayObject gameplayObject = MilMo_Level.CurrentLevel.GetGameplayObject(serverStartCaptureZoneCountdown.getGampleyObjectId());
		if (gameplayObject != null)
		{
			gameplayObject.TeamStartedCapturingZone(serverStartCaptureZoneCountdown.getTeamId(), serverStartCaptureZoneCountdown.getCaptureTimeStamp());
			Vector3 position = MilMo_Player.Instance.Avatar.Position;
			float num = 100f;
			if (gameplayObject.VisualRep != null && gameplayObject.VisualRep.GameObject != null && gameplayObject.VisualRep.GameObject.GetComponent<Collider>() != null)
			{
				num = gameplayObject.VisualRep.GameObject.GetComponent<Collider>().bounds.SqrDistance(position);
			}
			if (num < 10f)
			{
				MilMo_World.HudHandler.pvpZoneCountdown.setCountdown(serverStartCaptureZoneCountdown.getCaptureTimeStamp(), gameplayObject.Id, Teams[serverStartCaptureZoneCountdown.getTeamId()].Color);
			}
		}
	}

	private static void StopCaptureZoneCountdown(object msgAsObject)
	{
		if (msgAsObject is ServerStopCaptureZoneCountdown serverStopCaptureZoneCountdown)
		{
			MilMo_Level.CurrentLevel.GetGameplayObject(serverStopCaptureZoneCountdown.getGampleyObjectId()).TeamStoppedCapturingZone();
			if (MilMo_World.HudHandler.pvpZoneCountdown.Enabled && MilMo_World.HudHandler.pvpZoneCountdown.getZoneId() == serverStopCaptureZoneCountdown.getGampleyObjectId())
			{
				MilMo_World.HudHandler.pvpZoneCountdown.SetEnabled(e: false);
			}
		}
	}

	private static void TeamCapturedZone(object msgAsObject)
	{
		if (msgAsObject is ServerTeamCapturedZone serverTeamCapturedZone)
		{
			MilMo_GameplayObject gameplayObject = MilMo_Level.CurrentLevel.GetGameplayObject(serverTeamCapturedZone.getGampleyObjectId());
			gameplayObject.TeamCapturedZone(serverTeamCapturedZone.getTeamId());
			gameplayObject.SetZoneCaptured(playSound: true);
		}
	}

	private void PlayerDeliveredFlag(object msgAsObject)
	{
		if (msgAsObject is ServerPlayerDeliveredFlag serverPlayerDeliveredFlag)
		{
			string clipPath = "";
			MilMo_Avatar milMo_Avatar = null;
			if (serverPlayerDeliveredFlag.PlayerId == MilMo_Player.Instance.Id)
			{
				clipPath = "Content/Sounds/Batch01/PVP/ally_delivered_flag";
				milMo_Avatar = MilMo_Player.Instance.Avatar;
			}
			else if (MilMo_Level.CurrentLevel.Players.ContainsKey(serverPlayerDeliveredFlag.PlayerId))
			{
				MilMo_RemotePlayer milMo_RemotePlayer = MilMo_Level.CurrentLevel.Players[serverPlayerDeliveredFlag.PlayerId];
				clipPath = (IsTeamMate(milMo_RemotePlayer) ? "Content/Sounds/Batch01/PVP/ally_delivered_flag" : "Content/Sounds/Batch01/PVP/enemy_delivered_flag");
				milMo_Avatar = milMo_RemotePlayer.Avatar;
			}
			PlaySound(clipPath);
			milMo_Avatar?.PlayParticleEffect("PvPEffect1");
		}
	}

	private static async void PlaySound(string clipPath)
	{
		AudioSourceWrapper audioSource = MilMo_Global.AudioListener.GetComponent<AudioSourceWrapper>();
		if (audioSource == null)
		{
			audioSource = MilMo_Global.AudioListener.AddComponent<AudioSourceWrapper>();
		}
		AudioClip audioClip = await MilMo_ResourceManager.Instance.LoadAudioAsync(clipPath);
		if (!(audioClip == null))
		{
			audioSource.Clip = audioClip;
			audioSource.Loop = false;
			audioSource.Play();
		}
	}

	private void PlayerDied(object msgAsObject)
	{
		if (msgAsObject is ServerPlayerDied serverPlayerDied)
		{
			PlaySound("Content/Sounds/Batch01/PVP/warhorn");
			if (MatchMode.Equals(MilMo_MatchMode.BATTLE_ROYALE) && serverPlayerDied.PlayerId.Equals(MilMo_Player.Instance.Id) && _nrOfAlivePlayers > 2)
			{
				PlayEndEffect(playerDied: true);
			}
		}
	}

	private void PlayerAliveCountReceived(object msgAsObject)
	{
		if (!(msgAsObject is ServerPlayersAliveCount serverPlayersAliveCount))
		{
			Debug.LogWarning("Could not load ServerPlayersAliveCount message.");
		}
		else
		{
			SetAlivePlayers(serverPlayersAliveCount.PlayerCount);
		}
	}

	private void SetAlivePlayers(int nrOfPlayers)
	{
		if (MatchMode.Equals(MilMo_MatchMode.BATTLE_ROYALE))
		{
			_nrOfAlivePlayers = nrOfPlayers;
			MilMo_World.HudHandler.theMenuBar.UpdatePvpAliveCounter(nrOfPlayers);
		}
	}

	public bool IsWinner()
	{
		if (IsTeamMode)
		{
			if (!_playerTeamMap.TryGetValue(MilMo_Player.Instance.Id, out var value))
			{
				return false;
			}
			if (MilMo_World.PvpScoreBoard.TeamScores == null || MilMo_World.PvpScoreBoard.TeamScores.Count == 0)
			{
				return false;
			}
			return MilMo_World.PvpScoreBoard.TeamScores.First().GetTeam().GetId()
				.Equals(value.Id);
		}
		if (MilMo_World.PvpScoreBoard.Scores == null || MilMo_World.PvpScoreBoard.Scores.Count == 0)
		{
			return false;
		}
		return MilMo_World.PvpScoreBoard.Scores.First().GetPlayerID().Equals(MilMo_Player.Instance.Id);
	}

	private void PlayEndEffect(bool playerDied)
	{
		bool flag = IsWinner();
		flag = !playerDied && flag;
		for (int i = 0; i < 20; i++)
		{
			Vector3 offset = new Vector3(UnityEngine.Random.Range(-20f, 20f), 0f, UnityEngine.Random.Range(-20f, 20f));
			MilMo_Player.Instance.Avatar.PlayParticleEffect(flag ? "PvPEffect1" : "PvPEffect3", offset);
		}
	}
}
