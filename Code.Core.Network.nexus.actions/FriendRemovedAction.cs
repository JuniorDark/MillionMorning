namespace Code.Core.Network.nexus.actions;

public class FriendRemovedAction : IAction
{
	private readonly Friend _friend;

	public FriendRemovedAction(Friend friend)
	{
		_friend = friend;
	}

	public void Accept(INexusListener listener)
	{
		listener.FriendRemoved(_friend);
	}
}
