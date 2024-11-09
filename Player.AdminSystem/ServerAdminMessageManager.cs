using Code.Core.EventSystem;
using Code.Core.Network.messages.server;
using Code.Core.ResourceSystem;
using Core.GameEvent;
using Localization;
using UI.HUD.Chat;
using UI.HUD.Dialogues;
using UI.Sprites;
using UnityEngine;

namespace Player.AdminSystem;

public class ServerAdminMessageManager : MonoBehaviour
{
	private static MilMo_GenericReaction _adminMessageListener;

	protected void Awake()
	{
		_adminMessageListener = MilMo_EventSystem.Listen("server_admin_message", OnServerAdminMessage);
		_adminMessageListener.Repeating = true;
	}

	public void OnDestroy()
	{
		MilMo_EventSystem.RemoveReaction(_adminMessageListener);
		_adminMessageListener = null;
	}

	private void OnServerAdminMessage(object messageAsObject)
	{
		if (!(messageAsObject is ServerAdminMessage serverAdminMessage) || serverAdminMessage.getMessage().Length == 0)
		{
			Debug.LogWarning("Got admin message from server with empty string as message");
			return;
		}
		string text;
		if (MilMo_Localization.TryGetLocString(serverAdminMessage.getMessage(), out var locString))
		{
			if (serverAdminMessage.getFormatArguments().Count > 0)
			{
				object[] array = new object[serverAdminMessage.getFormatArguments().Count];
				int num = 0;
				foreach (string formatArgument in serverAdminMessage.getFormatArguments())
				{
					array[num++] = formatArgument;
				}
				locString.SetFormatArgs(array);
			}
			text = locString.String;
		}
		else
		{
			text = serverAdminMessage.getMessage();
		}
		if (serverAdminMessage.getType() == 0)
		{
			SpawnSystemDialog(text);
			PostSystemMessage(text);
		}
		else if (serverAdminMessage.getType() == 1)
		{
			SpawnInfoDialog(text);
			PostInfoMessage(text);
		}
		else if (serverAdminMessage.getType() == 2)
		{
			SpawnGmDialog(text);
			PostInfoMessage(text);
		}
	}

	private void PostSystemMessage(string message)
	{
		GameEvent.OnChatMessageReceivedEvent.RaiseEvent(new ChatMessageObject("  SYSTEM: ", "      " + message, ChatMessageObject.MessageType.System, ""));
	}

	private void PostInfoMessage(string message)
	{
		GameEvent.OnChatMessageReceivedEvent.RaiseEvent(new ChatMessageObject("  INFO: ", "      " + message, ChatMessageObject.MessageType.Info, ""));
	}

	private void SpawnInfoDialog(string compiledMessage)
	{
		DialogueSpawner.SpawnOkModal(new LocalizedStringWithArgument("Generic_Info"), new LocalizedStringWithArgument(compiledMessage), new AddressableSpriteLoader("InfoIcon"), null);
	}

	private void SpawnSystemDialog(string compiledMessage)
	{
		DialogueSpawner.SpawnOkModal(new LocalizedStringWithArgument("Generic_System"), new LocalizedStringWithArgument(compiledMessage), new AddressableSpriteLoader("InfoIcon"), null);
	}

	private void SpawnGmDialog(string compiledMessage)
	{
		DialogueSpawner.SpawnOkModal(new LocalizedStringWithArgument("Generic_System"), new LocalizedStringWithArgument(compiledMessage), new AddressableSpriteLoader("IconGMBadge"), null);
	}
}
