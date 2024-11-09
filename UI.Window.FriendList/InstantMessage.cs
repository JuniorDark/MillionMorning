using System;
using Code.Core.BuddyBackend;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Core;
using Localization;
using UI.HUD.Dialogues;
using UI.HUD.Dialogues.Modal;
using UI.Sprites;
using UnityEngine;

namespace UI.Window.FriendList;

[Serializable]
public class InstantMessage
{
	private readonly MilMo_BuddyBackend _backend;

	public string SenderID { get; }

	public string SenderName { get; }

	public string Message { get; private set; }

	public string ReceiverID { get; }

	public string ReceiverName { get; }

	public event Action<InstantMessage> OnMarkAsRead;

	public InstantMessage(string senderId, string senderName, string message)
	{
		_backend = Singleton<MilMo_BuddyBackend>.Instance;
		SenderID = senderId;
		SenderName = senderName;
		Message = message;
	}

	public InstantMessage(string receiverID, string receiverName)
	{
		_backend = Singleton<MilMo_BuddyBackend>.Instance;
		ReceiverID = receiverID;
		ReceiverName = receiverName;
	}

	public void MarkAsRead()
	{
		this.OnMarkAsRead?.Invoke(this);
	}

	public void ReadMessage()
	{
		DialogueSpawner.SpawnReplyCloseModal(new LocalizedStringWithArgument("Messenger_FriendCard_329", SenderName).GetMessage(), Message, new PortraitSpriteLoader(SenderID), Reply, null);
		MarkAsRead();
	}

	private void Reply()
	{
		if (_backend.IsBuddy(SenderID))
		{
			OpenSendIMDialog(SenderID, SenderName);
		}
		else
		{
			DialogueSpawner.SpawnQuickInfoDialogue(null, new LocalizedStringWithArgument("Messenger_FriendCard_13399"), "IconComposeMessage", 7);
		}
	}

	public static void OpenSendIMDialog(string receiverID, string receiverName)
	{
		InstantMessage @object = new InstantMessage(receiverID, receiverName);
		DialogueButtonInfo confirm = new DialogueButtonInfo(@object.Send, new LocalizedStringWithArgument("Messenger_FriendCard_328"), isDefault: true);
		DialogueButtonInfo cancel = new DialogueButtonInfo(null, new LocalizedStringWithArgument("Generic_Cancel"));
		DialogueSpawner.SpawnInputDialogue(new InputModalMessageData(new LocalizedStringWithArgument("Messenger_FriendCard_327", receiverName), new LocalizedStringWithArgument("World_359"), @object.OnInputChange, confirm, cancel));
	}

	private void OnInputChange(string newValue)
	{
		Message = newValue;
	}

	private void Send()
	{
		if (_backend.GetBuddy(ReceiverID) == null)
		{
			Debug.LogError("Unable to find friend: " + ReceiverID);
		}
		else
		{
			SendMessage(Message, ReceiverID);
		}
	}

	private void SendMessage(string message, string playerID)
	{
		if (!(message == "") && !string.IsNullOrEmpty(message))
		{
			MilMo_BadWordFilter.StringIntegrity stringIntegrity = MilMo_BadWordFilter.GetStringIntegrity(message);
			if (stringIntegrity == MilMo_BadWordFilter.StringIntegrity.Bad || stringIntegrity == MilMo_BadWordFilter.StringIntegrity.IRLContactAttempt)
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
				message = MilMo_BadWordFilter.CensorMessage(message);
			}
			if (message.EndsWith("\r\n"))
			{
				message = message[..^2];
			}
			else if (message.EndsWith("\n") || message.EndsWith("\r"))
			{
				message = message[..^1];
			}
			_backend.SendIm(playerID, message);
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Confirm);
		}
	}
}
