namespace Code.Core.Network.nexus.actions;

public class FriendConnectedAction : IAction
{
	private readonly int _userIdentifier;

	public FriendConnectedAction(int userIdentifier)
	{
		_userIdentifier = userIdentifier;
	}

	public void Accept(INexusListener listener)
	{
		listener.FriendConnected(_userIdentifier);
	}
}
