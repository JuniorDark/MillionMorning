using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.BuddyBackend;
using Code.Core.Network.types;
using Code.World.Achievements;
using Core;

namespace Code.World.Player;

[Serializable]
public class MilMo_Profile
{
	public bool isMe;

	public bool isFriend;

	public string avatarName;

	public string title;

	public int memberDaysLeft;

	public string mood;

	public sbyte role;

	public bool member;

	public int avatarLevel;

	public string playerId;

	private IPlayer _player;

	public string tweet;

	public Action OnTitleChange;

	private readonly MilMo_BuddyBackend _backend;

	public IList<MilMo_Medal> Medals { get; private set; }

	public Dictionary<string, List<bool>> ExplorationTokens { get; private set; }

	public IList<InventoryEntry> FavoriteItems { get; private set; }

	public IPlayer GetPlayer()
	{
		return _player;
	}

	public MilMo_Profile(IPlayer player, string tweet, IList<MilMo_Medal> medals, Dictionary<string, List<bool>> explorationTokens, IList<InventoryEntry> favoriteItems)
		: this(player.Id, player.Avatar.Name, tweet, player.Avatar.Title, medals, player.Avatar.Mood, player.Avatar.Role, player.Avatar.MembershipDaysLeft, explorationTokens, favoriteItems, player.Avatar.AvatarLevel)
	{
		_player = player;
	}

	public MilMo_Profile(string playerId, string avatarName, string tweet, string title, IList<MilMo_Medal> medals, string mood, sbyte role, int memberDaysLeft, Dictionary<string, List<bool>> explorationTokens, IList<InventoryEntry> favoriteItems, int avatarLevel)
	{
		_player = null;
		_backend = Singleton<MilMo_BuddyBackend>.Instance;
		this.playerId = playerId;
		this.avatarName = avatarName;
		this.tweet = tweet;
		this.title = title;
		if (playerId == MilMo_Player.Instance.Id)
		{
			isMe = true;
		}
		if (!isMe)
		{
			isFriend = _backend.IsBuddy(playerId);
		}
		MilMo_Medal milMo_Medal = medals.FirstOrDefault((MilMo_Medal m) => m.Template.Identifier == title);
		if (milMo_Medal != null)
		{
			string identifier = milMo_Medal.Template.DisplayName.Identifier;
			this.title = identifier;
		}
		Medals = medals;
		this.mood = mood;
		this.memberDaysLeft = memberDaysLeft;
		member = memberDaysLeft > -1;
		this.role = role;
		ExplorationTokens = explorationTokens;
		FavoriteItems = favoriteItems;
		this.avatarLevel = avatarLevel;
	}

	private void OnTitleChanged()
	{
		title = _player.Avatar?.Title;
		OnTitleChange?.Invoke();
	}

	public void AddTitleChangeListener()
	{
		if (_player?.Avatar != null)
		{
			_player.Avatar.OnAvatarTitleUpdated += OnTitleChanged;
		}
	}

	public void RemoveTitleChangeListener()
	{
		if (_player?.Avatar != null)
		{
			_player.Avatar.OnAvatarTitleUpdated -= OnTitleChanged;
		}
	}
}
