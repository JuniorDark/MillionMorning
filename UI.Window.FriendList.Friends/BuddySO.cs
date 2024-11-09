using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Code.Core.Network.nexus;
using UnityEngine;

namespace UI.Window.FriendList.Friends;

[CreateAssetMenu(menuName = "Buddy/New Buddy", fileName = "testing")]
public class BuddySO : ScriptableObject, IIdentity
{
	[SerializeField]
	private int buddyId;

	[SerializeField]
	private string buddyName;

	[SerializeField]
	private string buddyTweet;

	[SerializeField]
	private bool buddyIsOnline;

	[SerializeField]
	private List<InstantMessage> buddyMessages;

	private Friend _friend;

	public int UserIdentifier => buddyId;

	public string Name => buddyName;

	public bool IsOnline => buddyIsOnline;

	public string Tweet => buddyTweet;

	public event Action OnTweetChange;

	public event Action OnMessagesChange;

	public void Init(Friend friendObject)
	{
		_friend = friendObject;
		if (_friend != null)
		{
			buddyId = _friend.UserIdentifier;
			buddyName = _friend.Name;
			buddyTweet = _friend.Tweet;
			buddyIsOnline = _friend.Online;
			buddyMessages = _friend.GetAllMessages();
			RegisterListeners();
		}
	}

	public void OnDestroy()
	{
		if (_friend != null)
		{
			RemoveListeners();
		}
	}

	private void RegisterListeners()
	{
		_friend.OnTweetChange += UpdateTweet;
		_friend.OnMessagesChange += UpdateMessages;
	}

	private void RemoveListeners()
	{
		_friend.OnTweetChange -= UpdateTweet;
		_friend.OnMessagesChange -= UpdateMessages;
	}

	private void UpdateTweet()
	{
		buddyTweet = _friend.Tweet;
		this.OnTweetChange?.Invoke();
	}

	private void UpdateMessages()
	{
		buddyMessages = _friend.GetAllMessages();
		this.OnMessagesChange?.Invoke();
	}

	public bool HasUnreadMessage()
	{
		return buddyMessages.Count > 0;
	}

	public void OpenAllMessages()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (InstantMessage item in buddyMessages.ToList())
		{
			stringBuilder.AppendLine(item.Message);
			item.MarkAsRead();
		}
		new InstantMessage(buddyId.ToString(), buddyName, stringBuilder.ToString()).ReadMessage();
	}

	public void OpenFirstMessage()
	{
		buddyMessages.FirstOrDefault()?.ReadMessage();
	}
}
