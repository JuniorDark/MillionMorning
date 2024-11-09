namespace Code.Core.Network.nexus.actions;

public class MessageAction : IAction
{
	private readonly int _userIdentifier;

	private readonly string _message;

	public MessageAction(int userIdentifier, string message)
	{
		_userIdentifier = userIdentifier;
		_message = message;
	}

	public void Accept(INexusListener listener)
	{
		listener.MessageReceived(_userIdentifier, _message);
	}
}
