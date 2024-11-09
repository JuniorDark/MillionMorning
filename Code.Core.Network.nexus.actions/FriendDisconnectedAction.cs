namespace Code.Core.Network.nexus.actions;

public class FriendDisconnectedAction : IAction
{
	private readonly int _userIdentifier;

	public FriendDisconnectedAction(int userIdentifier)
	{
		_userIdentifier = userIdentifier;
	}

	public void Accept(INexusListener listener)
	{
		listener.FriendDisconnected(_userIdentifier);
	}
}
