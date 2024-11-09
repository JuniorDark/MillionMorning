namespace Code.Core.Network.nexus.actions;

public class FriendAddedAction : IAction
{
	private readonly Friend _friend;

	public FriendAddedAction(Friend friend)
	{
		_friend = friend;
	}

	public void Accept(INexusListener listener)
	{
		listener.FriendAdded(_friend);
	}
}
