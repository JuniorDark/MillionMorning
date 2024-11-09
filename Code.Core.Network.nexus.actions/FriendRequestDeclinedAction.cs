namespace Code.Core.Network.nexus.actions;

public class FriendRequestDeclinedAction : IAction
{
	private readonly FriendRequest _request;

	public FriendRequestDeclinedAction(FriendRequest request)
	{
		_request = request;
	}

	public void Accept(INexusListener listener)
	{
		listener.FriendRequestDeclined(_request);
	}
}
