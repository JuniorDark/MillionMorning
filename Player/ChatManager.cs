using Code.Core.Avatar;
using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.Core.Network.messages.server;
using Code.Core.ResourceSystem;
using Code.World.Chat;
using Code.World.Chat.ChatRoom;
using Code.World.Level;
using Code.World.Player;
using Core.GameEvent;
using Core.Utilities;
using UI;
using UI.Bubbles;
using UI.HUD.Chat;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Player;

public class ChatManager : MonoBehaviour
{
	public enum EChatChanel
	{
		Standard,
		Group,
		NrOfTypes
	}

	[SerializeField]
	private AssetReference chatBubblePublic;

	[SerializeField]
	private AssetReference chatBubbleGroup;

	private Transform _containerTransform;

	private ChatCensoringProcessor _chatCensoringProcessor;

	private MilMo_GenericReaction _chatToAllReaction;

	private MilMo_Player PlayerInstance => MilMo_Player.Instance;

	protected void Awake()
	{
		_chatToAllReaction = MilMo_EventSystem.Listen("chat_to_all", OnChatToAll);
		_chatToAllReaction.Repeating = true;
	}

	public void OnDestroy()
	{
		MilMo_EventSystem.RemoveReaction(_chatToAllReaction);
		_chatToAllReaction = null;
	}

	protected void Start()
	{
		_chatCensoringProcessor = new ChatCensoringProcessor(null);
		GameObject container = WorldSpaceManager.GetContainer();
		if (container == null)
		{
			Debug.LogError(base.gameObject.name + ": missing container");
		}
		else
		{
			_containerTransform = container.transform;
		}
	}

	private void OnChatToAll(object msgAsObj)
	{
		if (msgAsObj is ServerChatToAll serverChatToAll)
		{
			string playerID = serverChatToAll.getPlayerID();
			string message = serverChatToAll.getMessage();
			HandleChat(playerID, message, (EChatChanel)serverChatToAll.getChanel());
		}
	}

	private void HandleChat(string playerId, string chatMessage, EChatChanel chanel)
	{
		if (PlayerInstance == null)
		{
			return;
		}
		MilMo_Instance currentInstance = MilMo_Instance.CurrentInstance;
		if (currentInstance == null)
		{
			Debug.Log("Unable to get Level/Home");
			return;
		}
		bool flag = PlayerInstance.Id == playerId;
		MilMo_Avatar milMo_Avatar = (flag ? PlayerInstance.Avatar : currentInstance.GetRemotePlayer(playerId)?.Avatar);
		if (milMo_Avatar == null)
		{
			Debug.Log("Talking avatar is null");
			return;
		}
		if (flag)
		{
			MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_Chat");
		}
		bool flag2 = !flag && !PlayerInstance.InSinglePlayerArea && (currentInstance is MilMo_Level || PlayerInstance.Avatar.Room == milMo_Avatar.Room);
		if (!flag && !flag2)
		{
			Debug.Log("Player is not in the same room... " + PlayerInstance.Avatar.Room + " vs " + milMo_Avatar.Room);
			return;
		}
		foreach (MilMo_Avatar avatar in currentInstance.Avatars)
		{
			avatar.SuperAlivenessManager.PlayerSocialized(milMo_Avatar);
		}
		MilMo_ChatRoomManager.Instance.ChatMessageFromPlayer(milMo_Avatar);
		ChatCensoringProcessor.WordFilter wordFilter = _chatCensoringProcessor.CheckWords(chatMessage, playerId);
		if (string.IsNullOrEmpty(wordFilter.Message))
		{
			return;
		}
		if (flag && MilMo_ChatCommandHandler.HandleCommand(chatMessage))
		{
			Debug.Log("Message is command");
			return;
		}
		bool inGroup = chanel == EChatChanel.Group;
		if (milMo_Avatar.Room == MilMo_UserInterface.CurrentRoom)
		{
			PlayerTalks(milMo_Avatar, MilMo_Localization.GetNotLocalizedLocString(wordFilter.Message), inGroup);
		}
		PostChatMessage(milMo_Avatar.Name, wordFilter.Message, inGroup);
	}

	private void PlayerTalks(MilMo_Avatar avatar, MilMo_LocString loco, bool inGroup)
	{
		ChatBubble chatBubble = Instantiator.Instantiate<ChatBubble>(inGroup ? chatBubbleGroup : chatBubblePublic, _containerTransform);
		if (!(chatBubble == null))
		{
			chatBubble.SetText(loco.String);
			Transform transform = ((avatar != null && avatar.Head != null) ? avatar.Head : null);
			if (transform != null)
			{
				chatBubble.SetTarget(transform);
			}
			chatBubble.Show();
		}
	}

	private void PostChatMessage(string senderName, string message, bool inGroup)
	{
		ChatMessageObject.MessageType messageType = (inGroup ? ChatMessageObject.MessageType.GroupMessage : ChatMessageObject.MessageType.Normal);
		GameEvent.OnChatMessageReceivedEvent.RaiseEvent(new ChatMessageObject("[" + senderName + "] ", message, messageType, ""));
	}
}
