using System;

namespace UI.HUD.Chat;

[Serializable]
public class ChatMessageObject
{
	public enum MessageType
	{
		Normal,
		System,
		Info,
		PvpKill,
		GroupMessage
	}

	public string sender;

	public string message;

	public MessageType messageType;

	public string time;

	public string iconKey;

	public ChatMessageObject(string sender, string message, MessageType messageType, string time, string iconKey = null)
	{
		this.sender = sender.Trim(' ', '[', ']');
		this.message = message;
		this.messageType = messageType;
		this.time = ((time != "") ? time : GetTime());
		this.iconKey = iconKey;
	}

	private string GetTime()
	{
		DateTime dateTime = DateTime.Now.ToLocalTime();
		return ((dateTime.Hour < 10) ? "0" : "") + dateTime.Hour + ":" + ((dateTime.Minute < 10) ? "0" : "") + dateTime.Minute + ":" + ((dateTime.Second < 10) ? "0" : "") + dateTime.Second + " ";
	}
}
