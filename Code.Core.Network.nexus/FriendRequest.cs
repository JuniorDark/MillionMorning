using UnityEngine;

namespace Code.Core.Network.nexus;

public class FriendRequest : IIdentity
{
	private readonly Connection _connection;

	public int UserIdentifier { get; }

	public string Name { get; }

	public FriendRequest(int userIdentifier, string name, Connection connection)
	{
		UserIdentifier = userIdentifier;
		Name = name;
		_connection = connection;
	}

	public void Accept()
	{
		Debug.Log("Approve friend request from player " + UserIdentifier);
		_connection?.SendFriendRequest(UserIdentifier);
	}

	public void Decline()
	{
		Debug.Log("Decline friend request from player " + UserIdentifier);
		_connection?.DeclineFriendRequest(UserIdentifier);
	}
}
