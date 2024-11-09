namespace Code.Core.Network.nexus.actions;

public class TweetChangedAction : IAction
{
	private readonly int _userIdentifier;

	private readonly string _tweet;

	public TweetChangedAction(int userIdentifier, string tweet)
	{
		_userIdentifier = userIdentifier;
		_tweet = tweet;
	}

	public void Accept(INexusListener listener)
	{
		listener.TweetChanged(_userIdentifier, _tweet);
	}
}
