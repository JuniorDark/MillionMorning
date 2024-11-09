using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.Core.Avatar;
using Code.Core.BuddyBackend;
using Code.Core.EventSystem;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Code.Core.Network.nexus;
using Code.Core.Network.types;
using Code.Core.Template;
using Code.World.Achievements;
using Code.World.Level.LevelInfo;
using Core;
using Core.Utilities;
using UnityEngine;

namespace Code.World.Player;

public static class MilMo_ProfileManager
{
	private enum ProfileResponse
	{
		OK,
		NotAllowed,
		Error
	}

	private delegate void ProfileDone(ProfileResponse response, MilMo_Profile profile);

	public delegate void RequestNameResponseCallback(string name, string id);

	private static readonly Dictionary<string, List<ProfileDone>> RequestedProfiles = new Dictionary<string, List<ProfileDone>>();

	private static readonly Dictionary<string, string> StoredPlayerNames = new Dictionary<string, string>();

	private static readonly Dictionary<string, RequestNameResponseCallback> RequestedPlayerNames = new Dictionary<string, RequestNameResponseCallback>();

	private static readonly Dictionary<string, Texture2D> StoredPortraits = new Dictionary<string, Texture2D>();

	private static readonly Dictionary<string, List<MilMo_ThumbnailGenerator.ThumbnailDone>> CurrentlyLoadingPortraits = new Dictionary<string, List<MilMo_ThumbnailGenerator.ThumbnailDone>>();

	public static void Initialize()
	{
		MilMo_EventSystem.Listen("player_profile", ProfileArrived).Repeating = true;
		MilMo_EventSystem.Listen("requested_player_name_response", RequestedPlayerNameResponse).Repeating = true;
	}

	public static async Task<MilMo_Profile> GetProfileAsync(string playerId)
	{
		if (MilMo_Player.Instance == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(playerId) || playerId == MilMo_Player.Instance.Id)
		{
			MilMo_Profile currentPlayerProfile = GetCurrentPlayerProfile();
			if (currentPlayerProfile == null)
			{
				return null;
			}
			currentPlayerProfile.isFriend = false;
			return currentPlayerProfile;
		}
		TaskCompletionSource<MilMo_Profile> tcs = new TaskCompletionSource<MilMo_Profile>();
		RequestProfile(playerId, delegate(ProfileResponse response, MilMo_Profile profile)
		{
			if (response != 0)
			{
				tcs.TrySetException(new Exception("Could not load profile: " + playerId));
			}
			else
			{
				tcs.TrySetResult(profile);
			}
		});
		return await tcs.Task;
	}

	private static void RequestProfile(string playerId, ProfileDone callback)
	{
		if (RequestedProfiles.TryGetValue(playerId, out var value))
		{
			value.Add(callback);
			return;
		}
		value = new List<ProfileDone> { callback };
		bool isFriend = Singleton<MilMo_BuddyBackend>.Instance.IsBuddy(playerId);
		if (!Singleton<GameNetwork>.Instance.RequestPlayerProfile(playerId, isFriend))
		{
			callback(ProfileResponse.Error, null);
		}
		else
		{
			RequestedProfiles.Add(playerId, value);
		}
	}

	private static MilMo_Profile GetCurrentPlayerProfile()
	{
		MilMo_Player instance = MilMo_Player.Instance;
		if (instance != null)
		{
			MilMo_Avatar avatar = instance.Avatar;
			if (avatar != null && !avatar.IsDestroyed)
			{
				MilMo_Player instance2 = MilMo_Player.Instance;
				List<MilMo_Medal> list = new List<MilMo_Medal>();
				foreach (MilMo_MedalCategory medalCategory in Singleton<MilMo_AchievementHandler>.Instance.GetMedalCategories())
				{
					list.AddRange(medalCategory.Medals);
				}
				Dictionary<string, List<bool>> explorationTokens = MilMo_LevelInfo.GetLevelInfoDataArray().ToDictionary((MilMo_LevelInfoData level) => level.FullLevelName, (MilMo_LevelInfoData level) => level.ExplorationTokens);
				string tweet = "";
				Friend buddy = Singleton<MilMo_BuddyBackend>.Instance.GetBuddy(instance2.Id);
				if (buddy != null)
				{
					tweet = buddy.Tweet;
				}
				return new MilMo_Profile(instance2, tweet, list, explorationTokens, new List<InventoryEntry>());
			}
		}
		return null;
	}

	private static async void ProfileArrived(object messageAsObject)
	{
		if (!(messageAsObject is ServerPlayerProfile profile))
		{
			Debug.Log("Profile is null in MilMo_ProfileManager:ProfileArrived");
			return;
		}
		if (profile.GetAllowed() == 1)
		{
			AddAPlayerToNameList(profile.GetAvatarName(), profile.GetPlayerId());
		}
		if (!RequestedProfiles.TryGetValue(profile.GetPlayerId(), out var listeners) || listeners.Count == 0)
		{
			return;
		}
		if (profile.GetAllowed() != 1)
		{
			RequestedProfiles.Remove(profile.GetPlayerId());
			{
				foreach (ProfileDone item2 in listeners)
				{
					item2(ProfileResponse.NotAllowed, null);
				}
				return;
			}
		}
		Dictionary<string, List<bool>> tokens = new Dictionary<string, List<bool>>();
		foreach (FoundTokensInfo explorationToken in profile.GetExplorationTokens())
		{
			List<bool> list = new List<bool>();
			int tokensFound = explorationToken.GetTokensFound();
			sbyte tokensAmount = explorationToken.GetTokensAmount();
			for (int i = 0; i < tokensAmount; i++)
			{
				int num = 1 << i;
				bool item = (tokensFound & num) != 0;
				list.Add(item);
			}
			tokens.Add(explorationToken.GetLevel(), list);
		}
		List<MilMo_Medal> medals = new List<MilMo_Medal>();
		if (profile.GetMedals().Count == 0)
		{
			ProfileReady(profile, medals, tokens, listeners);
			return;
		}
		foreach (string medal in profile.GetMedals())
		{
			if (!(await Singleton<MilMo_TemplateContainer>.Instance.GetTemplateAsync(medal) is MilMo_AchievementTemplate milMo_AchievementTemplate))
			{
				foreach (ProfileDone item3 in listeners)
				{
					item3(ProfileResponse.Error, null);
				}
				Debug.Log("medal template is null in MilMo_ProfileManager:ProfileArrived");
			}
			else if (!(milMo_AchievementTemplate.Instantiate(new Dictionary<string, string>()) is MilMo_Medal milMo_Medal))
			{
				foreach (ProfileDone item4 in listeners)
				{
					item4(ProfileResponse.Error, null);
				}
				Debug.Log("medal is null in MilMo_ProfileManager:ProfileArrived");
			}
			else
			{
				milMo_Medal.Acquired = true;
				milMo_Medal.IsMine = false;
				medals.Add(milMo_Medal);
			}
		}
		ProfileReady(profile, medals, tokens, listeners);
	}

	private static void ProfileReady(ServerPlayerProfile profile, List<MilMo_Medal> medals, Dictionary<string, List<bool>> tokens, List<ProfileDone> listeners)
	{
		if (!RequestedProfiles.ContainsKey(profile.GetPlayerId()))
		{
			return;
		}
		PlayerUtils.FindPlayer(profile.GetPlayerId(), out var player);
		string tweet = "";
		Friend buddy = Singleton<MilMo_BuddyBackend>.Instance.GetBuddy(profile.GetPlayerId());
		if (buddy != null)
		{
			tweet = buddy.Tweet;
		}
		MilMo_Profile profile2 = ((player == null) ? new MilMo_Profile(profile.GetPlayerId(), profile.GetAvatarName(), tweet, profile.GetTitle(), medals, profile.GetMood(), profile.GetRole(), profile.GetMemberDaysLeft(), tokens, profile.GetFavoriteItems(), profile.GetAvatarLevel()) : new MilMo_Profile(player, tweet, medals, tokens, new List<InventoryEntry>()));
		RequestedProfiles.Remove(profile.GetPlayerId());
		foreach (ProfileDone listener in listeners)
		{
			listener(ProfileResponse.OK, profile2);
		}
	}

	public static void AddAPlayerToNameList(string name, string id)
	{
		if (!StoredPlayerNames.ContainsKey(id))
		{
			StoredPlayerNames.Add(id, name);
		}
	}

	public static void RequestPlayerName(string playerId, RequestNameResponseCallback callback)
	{
		if (string.IsNullOrEmpty(playerId))
		{
			Debug.LogWarning("Trying to request a player id that is null.");
		}
		else if (StoredPlayerNames.ContainsKey(playerId))
		{
			callback(StoredPlayerNames[playerId], playerId);
		}
		else if (!RequestedPlayerNames.ContainsKey(playerId))
		{
			RequestedPlayerNames.Add(playerId, callback);
			Singleton<GameNetwork>.Instance.RequestPlayerName(playerId);
		}
	}

	public static string GetStoredName(string playerId)
	{
		if (StoredPlayerNames.ContainsKey(playerId))
		{
			return StoredPlayerNames[playerId];
		}
		return "";
	}

	private static void RequestedPlayerNameResponse(object o)
	{
		if (o is ServerRequestPlayerNameResponse serverRequestPlayerNameResponse)
		{
			if (!StoredPlayerNames.ContainsKey(serverRequestPlayerNameResponse.getPlayerId()))
			{
				StoredPlayerNames.Add(serverRequestPlayerNameResponse.getPlayerId(), serverRequestPlayerNameResponse.getName());
			}
			if (RequestedPlayerNames.ContainsKey(serverRequestPlayerNameResponse.getPlayerId()))
			{
				RequestedPlayerNames[serverRequestPlayerNameResponse.getPlayerId()](serverRequestPlayerNameResponse.getName(), serverRequestPlayerNameResponse.getPlayerId());
			}
		}
	}

	public static async Task<Texture2D> GetPlayerPortraitAsync(string playerId)
	{
		TaskCompletionSource<Texture2D> tcs = new TaskCompletionSource<Texture2D>();
		GetPlayerPortrait(playerId, delegate(bool success, string pId, Texture2D texture)
		{
			if (!success)
			{
				Debug.LogError("Could not get player portrait for player: " + pId);
				tcs.TrySetResult(null);
			}
			else
			{
				tcs.TrySetResult(texture);
			}
		});
		return await tcs.Task;
	}

	public static void GetPlayerPortrait(string playerId, MilMo_ThumbnailGenerator.ThumbnailDone callback)
	{
		if (CurrentlyLoadingPortraits.ContainsKey(playerId))
		{
			CurrentlyLoadingPortraits[playerId].Add(callback);
			return;
		}
		if (StoredPortraits.ContainsKey(playerId))
		{
			callback(success: true, playerId, StoredPortraits[playerId]);
			return;
		}
		List<MilMo_ThumbnailGenerator.ThumbnailDone> value = new List<MilMo_ThumbnailGenerator.ThumbnailDone> { callback };
		CurrentlyLoadingPortraits.Add(playerId, value);
		MilMo_ThumbnailGenerator.Instance.Generate(playerId, delegate(bool success, string id, Texture2D tex)
		{
			if (CurrentlyLoadingPortraits.ContainsKey(playerId))
			{
				foreach (MilMo_ThumbnailGenerator.ThumbnailDone item in CurrentlyLoadingPortraits[playerId])
				{
					item(success, playerId, tex);
				}
				CurrentlyLoadingPortraits.Remove(playerId);
			}
			if (!success)
			{
				Debug.LogWarning("Error creating portrait for player with id: " + playerId + ".");
			}
			else if (!StoredPortraits.ContainsKey(playerId))
			{
				StoredPortraits.Add(playerId, tex);
			}
		});
	}
}
