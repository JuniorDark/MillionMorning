using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace UI.HUD.Chat;

public class ChatLog : MonoBehaviour
{
	[Serializable]
	public struct ChatMessageType
	{
		public ChatMessageObject.MessageType type;

		public AssetReference prefab;
	}

	[Header("Setup")]
	[SerializeField]
	private GameObject targetContainer;

	[SerializeField]
	private UnityEvent onStart;

	[SerializeField]
	private ChatMessageType[] prefabs;

	[SerializeField]
	[Min(1f)]
	private int maxMessages = 5;

	[Header("Data")]
	[SerializeField]
	private ChatLogSO chatLogSO;

	[SerializeField]
	private List<GameObject> messages;

	private void Awake()
	{
		if (!targetContainer)
		{
			Debug.LogWarning("ChatLog " + base.gameObject.name + ": Missing target container");
			return;
		}
		string[] names = Enum.GetNames(typeof(ChatMessageObject.MessageType));
		List<string> list = prefabs.Select((ChatMessageType p) => Enum.GetName(typeof(ChatMessageObject.MessageType), p.type)).ToList();
		string[] array = names;
		foreach (string text in array)
		{
			if (!list.Contains(text))
			{
				Debug.LogWarning("ChatLog " + base.gameObject.name + ": Missing prefab for message type " + text);
			}
		}
		if (prefabs.Select((ChatMessageType p) => p.prefab).ToList().Any((AssetReference prefab) => prefab == null || !prefab.RuntimeKeyIsValid()))
		{
			Debug.LogWarning("ChatLog " + base.gameObject.name + ": Invalid prefab found");
		}
		else if (!chatLogSO)
		{
			Debug.LogWarning("ChatLog " + base.gameObject.name + ": Missing chat log");
		}
		else
		{
			messages = new List<GameObject>();
		}
	}

	private void Start()
	{
		onStart?.Invoke();
	}

	private void OnEnable()
	{
		if (!(chatLogSO == null))
		{
			chatLogSO.OnMessageAdded += AddMessage;
			SyncWithState();
		}
	}

	private void OnDisable()
	{
		if (!(chatLogSO == null))
		{
			Clear();
			chatLogSO.OnMessageAdded -= AddMessage;
		}
	}

	private void SyncWithState()
	{
		if (chatLogSO == null)
		{
			return;
		}
		Clear();
		foreach (ChatMessageObject item in chatLogSO.Get())
		{
			AddMessage(item);
		}
	}

	private void Clear()
	{
		foreach (GameObject message in messages)
		{
			UnityEngine.Object.Destroy(message.gameObject);
		}
		messages.Clear();
		for (int num = targetContainer.transform.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(targetContainer.transform.GetChild(num).gameObject);
		}
	}

	private void AddMessage(ChatMessageObject messageObject)
	{
		if (messages.Count >= maxMessages && messages.Count > 0)
		{
			GameObject obj = messages.First();
			messages.RemoveAt(0);
			UnityEngine.Object.Destroy(obj);
		}
		if (prefabs == null || prefabs.Length < 1)
		{
			Debug.LogWarning("ChatLog " + base.gameObject.name + ": Missing message prefabs");
			return;
		}
		AssetReference prefab = prefabs.FirstOrDefault((ChatMessageType p) => p.type == messageObject.messageType).prefab;
		if (prefab == null || !prefab.RuntimeKeyIsValid())
		{
			string text = Enum.GetName(typeof(ChatMessageObject.MessageType), messageObject.messageType);
			Debug.LogWarning("ChatLog " + base.gameObject.name + ": Missing prefab for message of type " + text);
		}
		else
		{
			ChatMessage chatMessage = InstantiateAddressable<ChatMessage>(prefab, targetContainer, "ChatMessage");
			chatMessage.SetMessage(messageObject);
			messages.Add(chatMessage.gameObject);
		}
	}

	private static T InstantiateAddressable<T>(AssetReference assetReference, GameObject parent, string name)
	{
		GameObject obj = assetReference.InstantiateAsync(parent.transform).WaitForCompletion();
		obj.name = name;
		obj.SetActive(value: true);
		return obj.GetComponent<T>();
	}
}
