using System;
using Code.Core.ResourceSystem;
using Code.World.Player;
using Core.GameEvent;

namespace UI.HUD.Chat;

public class ChatCensoringProcessor
{
	public class WordFilter
	{
		public bool Censored;

		public string Message;
	}

	private readonly ChatChannelSO _channel;

	public ChatCensoringProcessor(ChatChannelSO channel)
	{
		_channel = channel;
	}

	public WordFilter CheckWords(string message, string id)
	{
		MilMo_BadWordFilter.StringIntegrity stringIntegrity = MilMo_BadWordFilter.GetStringIntegrity(message);
		bool censored = false;
		switch (stringIntegrity)
		{
		case MilMo_BadWordFilter.StringIntegrity.Empty:
			message = "";
			break;
		case MilMo_BadWordFilter.StringIntegrity.Bad:
			if (MilMo_Player.Instance.Id == id)
			{
				GameEvent.OnBadWordEvent.RaiseEvent();
			}
			message = MilMo_BadWordFilter.CensorMessage(message);
			censored = true;
			break;
		case MilMo_BadWordFilter.StringIntegrity.IRL:
			if (MilMo_Player.Instance.Id == id)
			{
				PostInfoMessage("Tip: Stay safe online! Never share real life contact info in the chat.");
			}
			message = MilMo_BadWordFilter.CensorMessage(message);
			censored = true;
			break;
		case MilMo_BadWordFilter.StringIntegrity.Cheating:
			if (MilMo_Player.Instance.Id == id)
			{
				PostInfoMessage("Tip: Cheating, hacking or using bots will get you BANNED from MilMo!");
			}
			message = MilMo_BadWordFilter.CensorMessage(message);
			censored = true;
			break;
		case MilMo_BadWordFilter.StringIntegrity.IRLContactAttempt:
			if (MilMo_Player.Instance.Id == id)
			{
				GameEvent.OnBadWordEvent.RaiseEvent();
				PostSystemMessage("Stay safe online! Don't share real life contact info in the chat.");
			}
			message = MilMo_BadWordFilter.CensorMessage(MilMo_BadWordFilter.CensorDigits(message));
			censored = true;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case MilMo_BadWordFilter.StringIntegrity.OK:
			break;
		}
		return new WordFilter
		{
			Message = message,
			Censored = censored
		};
	}

	private void PostInfoMessage(string message)
	{
		GameEvent.OnChatMessageReceivedEvent.RaiseEvent(new ChatMessageObject("INFO", message, ChatMessageObject.MessageType.Info, ""));
	}

	private void PostSystemMessage(string message)
	{
		GameEvent.OnChatMessageReceivedEvent.RaiseEvent(new ChatMessageObject("SYSTEM", message, ChatMessageObject.MessageType.System, ""));
	}
}
