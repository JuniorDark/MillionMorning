using Code.Core.Network.nexus;
using UI.Window.FriendList.Notifications;
using UnityEngine;

namespace UI.Window.FriendList.Friends;

public class BuddyUIFriend : UIFriend
{
	[SerializeField]
	private TweetFriendListNotification tweetFriendList;

	[SerializeField]
	private MessageFriendListNotification messageFriendList;

	private BuddySO _so;

	public override void Init(IIdentity identity)
	{
		if (!(identity is BuddySO buddySO))
		{
			Debug.LogError(base.name + ": so is of wrong type.");
			return;
		}
		_so = buddySO;
		base.Init(buddySO);
		UpdateHaveTweet();
		UpdateHaveMessage();
	}

	protected override void Awake()
	{
		if (tweetFriendList == null)
		{
			Debug.LogError(base.name + ": Unable to find tweetFriendList");
		}
		else if (tweetFriendList == null)
		{
			Debug.LogError(base.name + ": Unable to find messageFriendList");
		}
	}

	protected override void SetupListeners()
	{
		if (!(_so == null))
		{
			_so.OnTweetChange += UpdateHaveTweet;
			_so.OnMessagesChange += UpdateHaveMessage;
		}
	}

	protected override void RemoveListeners()
	{
		if (!(_so == null))
		{
			_so.OnTweetChange -= UpdateHaveTweet;
			_so.OnMessagesChange -= UpdateHaveMessage;
		}
	}

	public BuddySO GetBuddy()
	{
		return _so;
	}

	private void UpdateHaveTweet()
	{
		if (!string.IsNullOrEmpty(_so.Tweet))
		{
			tweetFriendList.Show();
		}
		else
		{
			tweetFriendList.Hide();
		}
	}

	private void UpdateHaveMessage()
	{
		if (_so.HasUnreadMessage())
		{
			messageFriendList.Show();
		}
		else
		{
			messageFriendList.Hide();
		}
	}
}
