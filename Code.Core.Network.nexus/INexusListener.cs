using System.Collections.Generic;

namespace Code.Core.Network.nexus;

public interface INexusListener
{
	void Connected(IList<Friend> friends, IList<FriendRequest> requests);

	void Disconnected();

	void MessageReceived(int userIdentifier, string message);

	void FriendAdded(Friend friend);

	void FriendRemoved(Friend friend);

	void FriendRequestReceived(FriendRequest request);

	void FriendRequestDeclined(FriendRequest request);

	void FriendConnected(int userIdentifier);

	void FriendDisconnected(int userIdentifier);

	void TweetChanged(int userIdentifier, string tweet);

	void UserTweet(string tweet);
}
