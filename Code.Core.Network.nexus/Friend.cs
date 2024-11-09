using System;
using System.Collections.Generic;
using System.Linq;
using UI.Window.FriendList;
using UnityEngine;

namespace Code.Core.Network.nexus;

public class Friend : IIdentity
{
	private readonly Connection _connection;

	private readonly int _buddyId;

	private readonly string _name;

	private bool _online;

	private string _tweet;

	private readonly List<InstantMessage> _messages;

	public int UserIdentifier => _buddyId;

	public string Name => _name;

	public bool Online
	{
		get
		{
			return _online;
		}
		set
		{
			_online = value;
			this.OnStatusChange?.Invoke();
		}
	}

	public string Tweet
	{
		get
		{
			return _tweet;
		}
		set
		{
			_tweet = value;
			this.OnTweetChange?.Invoke();
		}
	}

	public event Action OnStatusChange;

	public event Action OnTweetChange;

	public event Action OnMessagesChange;

	public Friend(int userIdentifier, string name, bool online, string tweet, Connection connection)
	{
		_buddyId = userIdentifier;
		_name = name;
		_online = online;
		_tweet = tweet;
		_messages = new List<InstantMessage>();
		_connection = connection;
	}

	public List<InstantMessage> GetAllMessages()
	{
		return _messages.ToList();
	}

	public void AddMessage(InstantMessage message)
	{
		_messages.Add(message);
		message.OnMarkAsRead += RemoveMessage;
		this.OnMessagesChange?.Invoke();
	}

	private void RemoveMessage(InstantMessage message)
	{
		_messages.Remove(message);
		this.OnMessagesChange?.Invoke();
	}

	private void MarkAllMessagesAsRead()
	{
		_messages.ForEach(delegate(InstantMessage m)
		{
			m.MarkAsRead();
		});
	}

	public void Remove()
	{
		Debug.Log("Remove player " + _buddyId + " from friend list.");
		MarkAllMessagesAsRead();
		_connection.RemoveFriend(_buddyId);
	}

	public void SendMessage(string message)
	{
		Debug.Log("Send IM to player " + _buddyId);
		_connection.SendMessage(_buddyId, message);
	}
}
