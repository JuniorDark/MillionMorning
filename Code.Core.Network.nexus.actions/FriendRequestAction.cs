namespace Code.Core.Network.nexus.actions;

public class FriendRequestAction : IAction
{
	private readonly FriendRequest _request;

	public FriendRequestAction(FriendRequest request)
	{
		_request = request;
	}

	public void Accept(INexusListener listener)
	{
		listener.FriendRequestReceived(_request);
	}
}
