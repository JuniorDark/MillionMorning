using System.Collections.Generic;

namespace Code.Core.Network.nexus.actions;

public class ConnectAction : IAction
{
	private readonly IList<Friend> _friends;

	private readonly IList<FriendRequest> _requests;

	public ConnectAction(IList<Friend> friends, IList<FriendRequest> requests)
	{
		_friends = friends;
		_requests = requests;
	}

	public void Accept(INexusListener listener)
	{
		listener.Connected(_friends, _requests);
	}
}
