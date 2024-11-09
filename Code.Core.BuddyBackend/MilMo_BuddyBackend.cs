using System;
using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Code.Core.Network.nexus;
using Core;
using Localization;
using UI.HUD.Dialogues;
using UI.Notification;
using UI.Sprites;
using UI.Window.FriendList;
using UnityEngine;

namespace Code.Core.BuddyBackend;

public class MilMo_BuddyBackend : Singleton<MilMo_BuddyBackend>, INexusListener
{
	public enum FriendStatus
	{
		Online,
		Offline,
		Away,
		Etc
	}

	private readonly Dictionary<int, FriendRequest> _requests = new Dictionary<int, FriendRequest>();

	private readonly Dictionary<int, Friend> _buddies = new Dictionary<int, Friend>();

	private bool _gotBuddyData;

	private ConnectionImplementation _nexusConnection;

	public bool GotBuddyData => _gotBuddyData;

	public bool IsConnected => _nexusConnection != null;

	public event Action OnConnected;

	public event Action OnDisconnected;

	public event Action<InstantMessage> OnMessageReceived;

	public event Action<Friend> OnFriendAdded;

	public event Action<Friend> OnFriendRemoved;

	public event Action<FriendRequest> OnFriendRequestAdded;

	public event Action<FriendRequest> OnFriendRequestRemoved;

	public event Action<Friend, FriendStatus> OnStatusChange;

	public event Action<Friend> OnTweetChanged;

	private void Awake()
	{
		MilMo_EventSystem.Listen("login_info_nexus", OnLoginInfoNexus);
	}

	public void Update()
	{
		_nexusConnection?.Update();
	}

	private void OnLoginInfoNexus(object msgAsObj)
	{
		if (msgAsObj is ServerNexusToken serverNexusToken)
		{
			Connect(serverNexusToken.getHost(), serverNexusToken.getPort(), serverNexusToken.getToken());
		}
	}

	private void Connect(string host, int port, string token)
	{
		if (_nexusConnection != null)
		{
			throw new InvalidOperationException("Trying to connect to Nexus more than once!");
		}
		_nexusConnection = CreateConnection();
		if (_nexusConnection == null)
		{
			throw new NullReferenceException("Could not create Nexus connection!");
		}
		Debug.Log("Connecting to Nexus on " + host + ":" + port);
		_nexusConnection?.Connect(host, port, token);
	}

	public void Disconnect()
	{
		if (_nexusConnection != null)
		{
			Debug.Log("Disconnecting from Nexus");
			_nexusConnection.Disconnect();
			_nexusConnection = null;
		}
	}

	private ConnectionImplementation CreateConnection()
	{
		return ConnectionFactory.CreateTcpConnection(this) as ConnectionImplementation;
	}

	void INexusListener.Connected(IList<Friend> friends, IList<FriendRequest> requests)
	{
		Debug.Log("Successfully connected to Nexus.");
		_buddies.Clear();
		foreach (Friend friend in friends)
		{
			_buddies[friend.UserIdentifier] = friend;
		}
		_requests.Clear();
		foreach (FriendRequest request in requests)
		{
			if (!_buddies.ContainsKey(request.UserIdentifier))
			{
				_requests[request.UserIdentifier] = request;
			}
		}
		Debug.Log($"Got {_buddies.Count} friends and {_requests.Count} friend requests.");
		_gotBuddyData = true;
		this.OnConnected?.Invoke();
	}

	void INexusListener.Disconnected()
	{
		Debug.Log("Disconnected from Nexus");
		_nexusConnection = null;
		this.OnDisconnected?.Invoke();
	}

	void INexusListener.MessageReceived(int userIdentifier, string message)
	{
		Debug.Log("Received message from player " + userIdentifier + ": " + message);
		if (_buddies.TryGetValue(userIdentifier, out var value))
		{
			InstantMessage instantMessage = new InstantMessage(value.UserIdentifier.ToString(), value.Name, message);
			instantMessage.OnMarkAsRead += NotificationMessageOpened;
			value.AddMessage(instantMessage);
			NotificationMessageAdded();
			if (value.GetAllMessages().Count == 0)
			{
				QuickInfoMessageReceived(instantMessage);
			}
			this.OnMessageReceived?.Invoke(instantMessage);
		}
	}

	void INexusListener.FriendAdded(Friend friend)
	{
		Debug.Log("Player " + friend.UserIdentifier + " (" + friend.Name + ") added to friend list");
		_buddies[friend.UserIdentifier] = friend;
		this.OnFriendAdded?.Invoke(friend);
		if (_requests.TryGetValue(friend.UserIdentifier, out var value))
		{
			_requests.Remove(friend.UserIdentifier);
			this.OnFriendRequestRemoved?.Invoke(value);
		}
	}

	void INexusListener.FriendRemoved(Friend friend)
	{
		Debug.Log("Player " + friend.UserIdentifier + " (" + friend.Name + ") removed from friend list");
		if (_buddies.ContainsKey(friend.UserIdentifier))
		{
			_buddies.Remove(friend.UserIdentifier);
			if (Singleton<GameNetwork>.Instance.IsConnectedToGameServer)
			{
				Singleton<GameNetwork>.Instance.SendFriendRemoved(friend.UserIdentifier.ToString());
			}
			this.OnFriendRemoved?.Invoke(friend);
		}
	}

	void INexusListener.FriendRequestReceived(FriendRequest request)
	{
		Debug.Log("Received friend request from player " + request.UserIdentifier + " (" + request.Name + ")");
		_requests[request.UserIdentifier] = request;
		SpawnFriendRequestDialogue(request);
		this.OnFriendRequestAdded?.Invoke(request);
	}

	void INexusListener.FriendRequestDeclined(FriendRequest request)
	{
		Debug.Log("Player " + request.UserIdentifier + " (" + request.Name + ") declined friend request");
		_requests.Remove(request.UserIdentifier);
		this.OnFriendRequestRemoved?.Invoke(request);
	}

	public void FriendConnected(int userIdentifier)
	{
		Debug.Log("Friend with ID " + userIdentifier + " connected");
		if (_buddies.TryGetValue(userIdentifier, out var value))
		{
			value.Online = true;
			this.OnStatusChange?.Invoke(value, FriendStatus.Online);
		}
	}

	public void FriendDisconnected(int userIdentifier)
	{
		Debug.Log("Friend with ID " + userIdentifier + " disconnected");
		if (_buddies.TryGetValue(userIdentifier, out var value))
		{
			value.Online = false;
			this.OnStatusChange?.Invoke(value, FriendStatus.Offline);
		}
	}

	public void TweetChanged(int userIdentifier, string tweet)
	{
		Debug.Log("Friend with ID " + userIdentifier + " changed tweet to: " + tweet);
		if (_buddies.TryGetValue(userIdentifier, out var value))
		{
			value.Tweet = tweet;
			this.OnTweetChanged?.Invoke(value);
		}
	}

	public void UserTweet(string tweet)
	{
		Debug.Log("Your tweet changed to: " + tweet);
		MilMo_EventSystem.Instance.PostEvent("yourtweet", tweet);
	}

	public bool IsFriendConnected(string playerId)
	{
		if (_buddies.ContainsKey(int.Parse(playerId)))
		{
			return _buddies[int.Parse(playerId)]?.Online ?? false;
		}
		return false;
	}

	public bool IsRequestingFriend(string playerId)
	{
		return _requests.ContainsKey(int.Parse(playerId));
	}

	public ICollection<Friend> GetBuddies()
	{
		return _buddies.Values;
	}

	public ICollection<FriendRequest> GetBuddyRequests()
	{
		return _requests.Values;
	}

	public bool IsBuddy(string playerId)
	{
		return _buddies.ContainsKey(int.Parse(playerId));
	}

	public Friend GetBuddy(string playerId)
	{
		if (string.IsNullOrEmpty(playerId))
		{
			return null;
		}
		if (_buddies.TryGetValue(int.Parse(playerId), out var value))
		{
			return value;
		}
		return null;
	}

	public bool SendFriendRequest(string playerId)
	{
		int num = int.Parse(playerId);
		if (_requests.ContainsKey(num))
		{
			Debug.LogWarning("Failed to send friend request to player " + playerId + ": Already sent.");
			return false;
		}
		Debug.Log("Send friend request to player " + playerId);
		_nexusConnection?.SendFriendRequest(num);
		return true;
	}

	public void SendApproveFriendRequest(string playerId)
	{
		int key = int.Parse(playerId);
		if (!_requests.ContainsKey(key))
		{
			Debug.LogWarning("Failed to approve friend request from player " + playerId + ": No such friend request.");
		}
		else
		{
			_requests[key].Accept();
		}
	}

	public void SendDeclineFriendRequest(string playerId)
	{
		int key = int.Parse(playerId);
		if (!_requests.ContainsKey(key))
		{
			Debug.LogWarning("Failed to decline friend request from player " + playerId + ": No such friend request.");
		}
		else
		{
			_requests[key].Decline();
		}
	}

	public void SendRemoveFriend(string playerId)
	{
		int key = int.Parse(playerId);
		if (!_buddies.ContainsKey(key))
		{
			Debug.LogWarning("Failed to remove player " + playerId + " from friend list: No such friend.");
		}
		else
		{
			_buddies[key].Remove();
		}
	}

	public void SendIm(string playerId, string message)
	{
		int key = int.Parse(playerId);
		if (!_buddies.TryGetValue(key, out var value))
		{
			Debug.LogWarning("Failed to send IM to player " + playerId + " : No such friend.");
		}
		else
		{
			value.SendMessage(message);
		}
	}

	public void SendChangeTweet(string tweet)
	{
		Debug.Log("Send tweet change " + tweet);
		_nexusConnection?.ChangeTweet(tweet);
	}

	private void SpawnFriendRequestDialogue(FriendRequest request)
	{
		LocalizedStringWithArgument localizedStringWithArgument = new LocalizedStringWithArgument("Messenger_FriendCard_332", request.Name);
		DialogueSpawner.SpawnAcceptIgnoreLaterModalDialogue("Messenger_FriendCard_333", localizedStringWithArgument.GetMessage(), request.Accept, delegate
		{
		}, request.Decline, new PortraitSpriteLoader(request.UserIdentifier.ToString()));
	}

	private void NotificationMessageOpened(InstantMessage im)
	{
		NotificationType notificationTypeByName = Singleton<NotificationManager>.Instance.GetNotificationTypeByName("FriendList");
		Singleton<NotificationManager>.Instance.Receive(notificationTypeByName, -1);
	}

	private void NotificationMessageAdded()
	{
		Debug.LogWarning("NotificationMessageAdded");
		NotificationType notificationTypeByName = Singleton<NotificationManager>.Instance.GetNotificationTypeByName("FriendList");
		Singleton<NotificationManager>.Instance.Receive(notificationTypeByName, 1);
	}

	private static void QuickInfoMessageReceived(InstantMessage im)
	{
		string text = "";
		if (im.Message.Length > 26)
		{
			for (int i = 0; i < 24; i++)
			{
				text += im.Message[i];
			}
			text += "...";
		}
		else
		{
			text = im.Message;
		}
		LocalizedStringWithArgument caption = new LocalizedStringWithArgument("Messenger_5595", im.SenderName);
		DialogueButtonInfo callToAction = new DialogueButtonInfo(im.ReadMessage, new LocalizedStringWithArgument("Messenger_FriendList_2964", im.SenderName));
		DialogueSpawner.SpawnQuickInfoDialogue(caption, new LocalizedStringWithArgument(text), "NoPortrait", 4, callToAction);
	}
}
