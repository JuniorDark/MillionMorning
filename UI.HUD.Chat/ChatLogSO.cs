using System;
using System.Collections.Generic;
using Core.State.Basic;
using UnityEngine;

namespace UI.HUD.Chat;

[CreateAssetMenu(fileName = "ChatLog", menuName = "Chat/Add Chat Log")]
public class ChatLogSO : ScriptableObject
{
	[SerializeField]
	private List<ChatMessageObject> messageList;

	[SerializeField]
	private IntState maxMessages;

	public event Action<ChatMessageObject> OnMessageAdded;

	private void OnEnable()
	{
		messageList.Clear();
	}

	private void OnDisable()
	{
		messageList.Clear();
	}

	public void Add(ChatMessageObject messageObject)
	{
		if (messageList.Count >= maxMessages.Get())
		{
			messageList.RemoveAt(0);
		}
		messageList.Add(messageObject);
		this.OnMessageAdded?.Invoke(messageObject);
	}

	public List<ChatMessageObject> Get()
	{
		return messageList;
	}
}
