using System;
using Code.Core.Network;
using Core;
using Core.GameEvent;
using Core.GameEvent.Types.ChatChannel;
using Core.Input;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.HUD.Chat;

public class Chat : HudElement
{
	[Header("Elements")]
	[SerializeField]
	private TMP_InputField inputField;

	[SerializeField]
	private Button sendButton;

	[SerializeField]
	private GameObject blocker;

	[Header("Messages")]
	[SerializeField]
	private ChatLogSO chatLogSO;

	[SerializeField]
	private UnityEvent onOverMessageMaxSize;

	[Header("Channel")]
	[SerializeField]
	private ChangeChatChannelEvent changeChatChannelEvent;

	[SerializeField]
	private ChatChannelSO defaultChannel;

	private ChatChannelSO _currentMessageChannel;

	private ChatEmoteProcessor _chatEmoteProcessor;

	[Header("Focus Events")]
	[SerializeField]
	private UnityEvent onFocus;

	[SerializeField]
	private UnityEvent onFocusExit;

	[SerializeField]
	private int maxMessageSize = 255;

	private bool _inEditMode;

	private bool _pointerInside;

	private void Awake()
	{
		if (!inputField || !sendButton)
		{
			Debug.LogWarning("UIChat " + base.gameObject.name + ": Missing input elements");
			return;
		}
		if (!chatLogSO)
		{
			Debug.LogWarning("UIChat " + base.gameObject.name + ": Missing chat log");
			return;
		}
		if (!changeChatChannelEvent)
		{
			Debug.LogWarning("UIChat " + base.gameObject.name + ": Missing channel event");
			return;
		}
		if (!defaultChannel)
		{
			Debug.LogWarning("UIChat " + base.gameObject.name + ": Missing default channel");
			return;
		}
		if (_chatEmoteProcessor == null)
		{
			_chatEmoteProcessor = new ChatEmoteProcessor();
		}
		_currentMessageChannel = defaultChannel;
	}

	private void Start()
	{
		blocker.SetActive(value: false);
		if ((bool)changeChatChannelEvent && (bool)_currentMessageChannel)
		{
			changeChatChannelEvent.Raise(_currentMessageChannel);
		}
	}

	private void OnEnable()
	{
		GameEvent.OnGroupChangeEvent.RegisterAction(OnGroupChange);
	}

	private void OnDisable()
	{
		GameEvent.OnGroupChangeEvent.UnregisterAction(OnGroupChange);
	}

	public void OnSelect()
	{
		_inEditMode = true;
		ShowChatLog();
		if (Singleton<InputController>.Instance != null)
		{
			GameEvent.OnSubmit = (Action)Delegate.Combine(GameEvent.OnSubmit, new Action(SendChatMessage));
			Singleton<InputController>.Instance.SetInputController();
		}
	}

	public void OnDeselect()
	{
		_inEditMode = false;
		HideChatLog();
		if (Singleton<InputController>.Instance != null)
		{
			GameEvent.OnSubmit = (Action)Delegate.Remove(GameEvent.OnSubmit, new Action(SendChatMessage));
			Singleton<InputController>.Instance.RestorePreviousController();
		}
	}

	public void OnEndEdit()
	{
		EventSystem.current.SetSelectedGameObject(null);
	}

	public void OnPointerEnter()
	{
		_pointerInside = true;
		if (!_inEditMode)
		{
			ShowChatLog();
		}
	}

	public void OnPointerExit()
	{
		_pointerInside = false;
		if (!_inEditMode)
		{
			HideChatLog();
		}
	}

	public void OnBlockerPress()
	{
		HideChatLog();
	}

	private void ShowChatLog()
	{
		onFocus?.Invoke();
		blocker.SetActive(value: true);
	}

	private void HideChatLog()
	{
		if (!_pointerInside)
		{
			onFocusExit?.Invoke();
			blocker.SetActive(value: false);
		}
	}

	private void OnGroupChange(bool joinGroup)
	{
		if (!joinGroup)
		{
			changeChatChannelEvent.Raise(defaultChannel);
		}
	}

	public void OnChangeChatChannel(ChatChannelSO channel)
	{
		if (!channel)
		{
			Debug.LogWarning("UIChat " + base.gameObject.name + ".OnChangeChatChannel: argument is null");
			return;
		}
		_currentMessageChannel = channel;
		if ((bool)inputField && (bool)inputField.image)
		{
			inputField.image.color = channel.inputColor;
		}
		if ((bool)sendButton && (bool)sendButton.image)
		{
			sendButton.image.color = channel.inputColor;
		}
	}

	public void OnMessageReceived(ChatMessageObject messageObject)
	{
		if (messageObject == null)
		{
			Debug.LogWarning("UIChat " + base.gameObject.name + ".OnMessageReceived: argument is null");
		}
		else if (chatLogSO != null)
		{
			chatLogSO.Add(messageObject);
		}
	}

	private void SendChatMessage(string message, ChatManager.EChatChanel chanel)
	{
		if (message.Length >= 1)
		{
			Singleton<GameNetwork>.Instance.SendChatMessage(message, chanel);
		}
	}

	public void SendChatMessage()
	{
		if (!_currentMessageChannel || !inputField)
		{
			return;
		}
		string text = inputField.text;
		text = text.Trim();
		if (text == "")
		{
			return;
		}
		if (Singleton<GameNetwork>.Instance.IsConnectedToGameServer)
		{
			SendChatMessage(text, (ChatManager.EChatChanel)_currentMessageChannel.messageChannel);
		}
		else
		{
			ChatMessageObject.MessageType messageType = ChatMessageObject.MessageType.Normal;
			if (_currentMessageChannel.messageChannel == ChatChannelSO.MessageChannel.Group)
			{
				messageType = ChatMessageObject.MessageType.GroupMessage;
			}
			GameEvent.OnChatMessageReceivedEvent.RaiseEvent(new ChatMessageObject("[Offline]", text, messageType, ""));
		}
		_chatEmoteProcessor?.CheckEmotes(text);
		inputField.text = "";
	}

	public void OnInputChanged(string input)
	{
		if (input.Length >= maxMessageSize)
		{
			inputField.text = input.Substring(0, maxMessageSize);
			onOverMessageMaxSize?.Invoke();
		}
	}

	public override void SetHudVisibility(bool shouldShow)
	{
		base.gameObject.SetActive(shouldShow);
	}
}
