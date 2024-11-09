using System;
using System.Collections.Generic;
using System.Linq;
using UI.FX;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI.HUD.Chat;

internal class ChatNotification : MonoBehaviour
{
	[Header("Setup")]
	[SerializeField]
	private GameObject targetContainer;

	[SerializeField]
	private Material fontMaterialOverride;

	[Header("Data")]
	[SerializeField]
	private ChatLogSO chatLogSO;

	[SerializeField]
	private ChatLog.ChatMessageType[] prefabs;

	[SerializeField]
	private List<GameObject> messages;

	private void Awake()
	{
		for (int num = targetContainer.transform.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(targetContainer.transform.GetChild(num).gameObject);
		}
	}

	private void OnEnable()
	{
		if (!(chatLogSO == null))
		{
			chatLogSO.OnMessageAdded += AddMessage;
		}
	}

	private void OnDisable()
	{
		if (!(chatLogSO == null))
		{
			chatLogSO.OnMessageAdded -= AddMessage;
		}
	}

	public void AddMessage(ChatMessageObject messageObject)
	{
		if (prefabs == null || prefabs.Length < 1)
		{
			Debug.LogWarning("ChatNotification " + base.gameObject.name + ": Missing message prefabs");
			return;
		}
		AssetReference prefab = prefabs.FirstOrDefault((ChatLog.ChatMessageType p) => p.type == messageObject.messageType).prefab;
		if (prefab == null || !prefab.RuntimeKeyIsValid())
		{
			string text = Enum.GetName(typeof(ChatMessageObject.MessageType), messageObject.messageType);
			Debug.LogWarning("ChatLog " + base.gameObject.name + ": Missing prefab for message of type " + text);
			return;
		}
		ChatMessage chatMessage = InstantiateAddressable<ChatMessage>(prefab, targetContainer, "ChatMessage");
		if (fontMaterialOverride != null)
		{
			chatMessage.OverrideMaterial(fontMaterialOverride);
		}
		chatMessage.SetMessage(messageObject);
		messages.Add(chatMessage.gameObject);
		UIAlphaFX component = chatMessage.GetComponent<UIAlphaFX>();
		if (!component)
		{
			Debug.LogWarning("Unable to find UIAlphaFX for " + base.gameObject.name);
			return;
		}
		component.FadeOut();
		UnityEngine.Object.Destroy(chatMessage.gameObject, component.GetFadeOutDuration(1f));
	}

	private static T InstantiateAddressable<T>(AssetReference assetReference, GameObject parent, string name)
	{
		GameObject obj = assetReference.InstantiateAsync(parent.transform).WaitForCompletion();
		obj.name = name;
		obj.SetActive(value: true);
		return obj.GetComponent<T>();
	}
}
