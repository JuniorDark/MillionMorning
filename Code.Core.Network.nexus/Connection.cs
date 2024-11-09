namespace Code.Core.Network.nexus;

public abstract class Connection
{
	public abstract void Update();

	public abstract void SendFriendRequest(int userIdentifier);

	public abstract void ChangeTweet(string tweet);

	public abstract void Disconnect();

	public abstract void SendMessage(int userIdentifier, string message);

	public abstract void DeclineFriendRequest(int userIdentifier);

	public abstract void RemoveFriend(int userIdentifier);
}
