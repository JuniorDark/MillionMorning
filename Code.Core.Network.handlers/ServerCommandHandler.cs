using System.Text.RegularExpressions;
using Code.Core.EventSystem;
using Code.Core.Network.messages.server;

namespace Code.Core.Network.handlers;

public class ServerCommandHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		string[] array = Regex.Split(((ServerCommand)message).getCommandResult(), "<br>", RegexOptions.IgnoreCase);
		foreach (string arg in array)
		{
			MilMo_EventSystem.Instance.AsyncPostEvent("command_response", arg);
		}
	}
}
