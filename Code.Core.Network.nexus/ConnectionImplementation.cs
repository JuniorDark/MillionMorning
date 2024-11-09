using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Code.Core.Network.nexus.actions;

namespace Code.Core.Network.nexus;

public class ConnectionImplementation : Connection
{
	private static readonly char[] Separator = new char[1] { ' ' };

	private readonly TcpClient _client;

	private readonly Thread _receiveThread;

	private readonly Queue<IAction> _actions;

	private readonly INexusListener _listener;

	public ConnectionImplementation(INexusListener listener)
	{
		_client = new TcpClient();
		_receiveThread = new Thread(WaitForData);
		_actions = new Queue<IAction>();
		_listener = listener;
	}

	public void Connect(string hostname, int port, string loginToken)
	{
		new Thread(new AsyncConnectCommand(hostname, port, loginToken, _client, _receiveThread).Connect).Start();
	}

	public override void Disconnect()
	{
		if (_client.Connected)
		{
			StreamWriter streamWriter = new StreamWriter(_client.GetStream());
			streamWriter.WriteLine("quit");
			streamWriter.Flush();
		}
	}

	public override void Update()
	{
		IAction action;
		lock (_actions)
		{
			action = ((_actions.Count <= 0) ? null : _actions.Dequeue());
		}
		while (action != null)
		{
			action.Accept(_listener);
			lock (_actions)
			{
				action = ((_actions.Count <= 0) ? null : _actions.Dequeue());
			}
		}
	}

	private void WaitForData()
	{
		IList<Friend> friends = null;
		IList<FriendRequest> requests = null;
		try
		{
			StreamReader streamReader = new StreamReader(_client.GetStream());
			string str;
			while (_client.Connected && (str = streamReader.ReadLine()) != null)
			{
				IAction action = null;
				if (GetAction(str, ref action, ref requests, ref friends))
				{
					break;
				}
				if (action != null)
				{
					lock (_actions)
					{
						_actions.Enqueue(action);
					}
				}
			}
		}
		catch (Exception)
		{
		}
		finally
		{
			_actions.Enqueue(new DisconnectAction());
		}
	}

	private bool GetAction(string str, ref IAction action, ref IList<FriendRequest> requests, ref IList<Friend> friends)
	{
		if (str.StartsWith("message"))
		{
			if (GetMessage(str, ref action))
			{
				return true;
			}
		}
		else if (str.StartsWith("add"))
		{
			if (AddFriend(str, ref action))
			{
				return true;
			}
		}
		else if (str.StartsWith("remove"))
		{
			if (RemoveFriend(str, ref action))
			{
				return true;
			}
		}
		else if (str.StartsWith("request"))
		{
			if (RequestFriend(str, requests, ref action))
			{
				return true;
			}
		}
		else if (str.StartsWith("decline"))
		{
			if (DeclineFriend(str, requests, ref action))
			{
				return true;
			}
		}
		else if (str.StartsWith("connected"))
		{
			if (FriendConnected(str, ref action))
			{
				return true;
			}
		}
		else if (str.StartsWith("disconnected"))
		{
			if (FriendDisconnected(str, ref action))
			{
				return true;
			}
		}
		else if (str.StartsWith("friend "))
		{
			if (AddToListOfFriends(str, friends))
			{
				return true;
			}
		}
		else if (str.StartsWith("tweet"))
		{
			if (FriendChangedTweet(str, out action))
			{
				return true;
			}
		}
		else if (str.StartsWith("usertweet"))
		{
			FriendTweet(str, out action);
		}
		else if (str.Equals("friendlist start"))
		{
			friends = new List<Friend>();
			requests = new List<FriendRequest>();
		}
		else if (str.Equals("friendlist stop"))
		{
			action = new ConnectAction(friends, requests);
			friends = null;
			requests = null;
		}
		return false;
	}

	private static bool FriendDisconnected(string str, ref IAction action)
	{
		string[] array = str.Split(Separator, 2);
		if (array.Length < 2)
		{
			return true;
		}
		if (int.TryParse(array[1], out var result))
		{
			action = new FriendDisconnectedAction(result);
		}
		return false;
	}

	private static bool FriendConnected(string str, ref IAction action)
	{
		string[] array = str.Split(Separator, 2);
		if (array.Length < 2)
		{
			return true;
		}
		if (int.TryParse(array[1], out var result))
		{
			action = new FriendConnectedAction(result);
		}
		return false;
	}

	private bool DeclineFriend(string str, IList<FriendRequest> requests, ref IAction action)
	{
		string[] array = str.Split(Separator, 3);
		if (array.Length < 3)
		{
			return true;
		}
		if (int.TryParse(array[1], out var result))
		{
			if (requests != null)
			{
				requests.Add(new FriendRequest(result, array[2], this));
			}
			else
			{
				action = new FriendRequestDeclinedAction(new FriendRequest(result, array[2], this));
			}
		}
		return false;
	}

	private bool RequestFriend(string str, IList<FriendRequest> requests, ref IAction action)
	{
		string[] array = str.Split(Separator, 3);
		if (array.Length < 3)
		{
			return true;
		}
		if (int.TryParse(array[1], out var result))
		{
			if (requests != null)
			{
				requests.Add(new FriendRequest(result, Encoding.UTF8.GetString(Convert.FromBase64String(array[2])), this));
			}
			else
			{
				action = new FriendRequestAction(new FriendRequest(result, Encoding.UTF8.GetString(Convert.FromBase64String(array[2])), this));
			}
		}
		return false;
	}

	private bool RemoveFriend(string str, ref IAction action)
	{
		string[] array = str.Split(Separator, 3);
		if (array.Length < 3)
		{
			return true;
		}
		if (int.TryParse(array[1], out var result))
		{
			action = new FriendRemovedAction(new Friend(result, Encoding.UTF8.GetString(Convert.FromBase64String(array[2])), online: false, "", this));
		}
		return false;
	}

	private bool AddFriend(string str, ref IAction action)
	{
		string[] array = str.Split(Separator, 5);
		if (array.Length < 4)
		{
			return true;
		}
		if (int.TryParse(array[1], out var result))
		{
			action = new FriendAddedAction(new Friend(result, Encoding.UTF8.GetString(Convert.FromBase64String(array[2])), array[3].Equals("online"), (array.Length != 5) ? "" : Encoding.UTF8.GetString(Convert.FromBase64String(array[4])), this));
		}
		return false;
	}

	private static bool GetMessage(string str, ref IAction action)
	{
		string[] array = str.Split(Separator, 3);
		if (array.Length < 3)
		{
			return true;
		}
		if (int.TryParse(array[1], out var result))
		{
			action = new MessageAction(result, Encoding.UTF8.GetString(Convert.FromBase64String(array[2])));
		}
		return false;
	}

	public override void SendMessage(int userIdentifier, string message)
	{
		if (_client.Connected)
		{
			StreamWriter streamWriter = new StreamWriter(_client.GetStream());
			streamWriter.WriteLine("send " + userIdentifier + " " + Convert.ToBase64String(Encoding.UTF8.GetBytes(message)));
			streamWriter.Flush();
		}
	}

	public override void SendFriendRequest(int userIdentifier)
	{
		if (_client.Connected)
		{
			StreamWriter streamWriter = new StreamWriter(_client.GetStream());
			streamWriter.WriteLine("add " + userIdentifier);
			streamWriter.Flush();
		}
	}

	public override void DeclineFriendRequest(int userIdentifier)
	{
		if (_client.Connected)
		{
			StreamWriter streamWriter = new StreamWriter(_client.GetStream());
			streamWriter.WriteLine("decline " + userIdentifier);
			streamWriter.Flush();
		}
	}

	public override void RemoveFriend(int userIdentifier)
	{
		if (_client.Connected)
		{
			StreamWriter streamWriter = new StreamWriter(_client.GetStream());
			streamWriter.WriteLine("remove " + userIdentifier);
			streamWriter.Flush();
		}
	}

	public override void ChangeTweet(string tweet)
	{
		if (_client.Connected)
		{
			StreamWriter streamWriter = new StreamWriter(_client.GetStream());
			streamWriter.WriteLine("tweet " + Convert.ToBase64String(Encoding.UTF8.GetBytes(tweet)));
			streamWriter.Flush();
		}
	}

	private static void FriendTweet(string str, out IAction action)
	{
		string[] array = str.Split(Separator, 2);
		action = new UserTweetAction((array.Length != 2) ? "" : Encoding.UTF8.GetString(Convert.FromBase64String(array[1])));
	}

	private static bool FriendChangedTweet(string str, out IAction action)
	{
		action = null;
		string[] array = str.Split(Separator, 3);
		if (array.Length < 2)
		{
			return true;
		}
		if (int.TryParse(array[1], out var result))
		{
			action = new TweetChangedAction(result, (array.Length != 3) ? "" : Encoding.UTF8.GetString(Convert.FromBase64String(array[2])));
		}
		return false;
	}

	private bool AddToListOfFriends(string str, IList<Friend> friends)
	{
		string[] array = str.Split(Separator, 5);
		if (array.Length < 4)
		{
			return true;
		}
		if (friends != null && int.TryParse(array[1], out var result))
		{
			friends.Add(new Friend(result, Encoding.UTF8.GetString(Convert.FromBase64String(array[2])), array[3].Equals("online"), (array.Length != 5) ? "" : Encoding.UTF8.GetString(Convert.FromBase64String(array[4])), this));
		}
		return false;
	}
}
