namespace Code.Core.Network.nexus.actions;

public class UserTweetAction : IAction
{
	private readonly string _tweet;

	public UserTweetAction(string tweet)
	{
		_tweet = tweet;
	}

	public void Accept(INexusListener listener)
	{
		listener.UserTweet(_tweet);
	}
}
