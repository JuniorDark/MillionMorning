using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace UI.Console;

public class ConsoleLog : MonoBehaviour
{
	[Header("Setup")]
	[SerializeField]
	private GameObject targetContainer;

	[SerializeField]
	private UnityEvent onStart;

	[SerializeField]
	[Min(1f)]
	private int maxMessages = 5;

	public AssetReference messagePrefab;

	[Header("Data")]
	[SerializeField]
	private ConsoleLogSO consoleLogSO;

	[SerializeField]
	private List<GameObject> messages;

	private void Awake()
	{
		if (!targetContainer)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing target container");
		}
		else if (!messagePrefab.RuntimeKeyIsValid())
		{
			Debug.LogWarning(base.gameObject.name + ": Invalid messagePrefab");
		}
		else if (!consoleLogSO)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing console log");
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
		if (!(consoleLogSO == null))
		{
			consoleLogSO.OnMessageAdded += AddMessage;
			SyncWithState();
		}
	}

	private void OnDisable()
	{
		if (!(consoleLogSO == null))
		{
			Clear();
			consoleLogSO.OnMessageAdded -= AddMessage;
		}
	}

	private void SyncWithState()
	{
		if (consoleLogSO == null)
		{
			return;
		}
		Clear();
		foreach (string item in consoleLogSO.Get())
		{
			AddMessage(item);
		}
	}

	private void Clear()
	{
		foreach (GameObject message in messages)
		{
			Object.Destroy(message.gameObject);
		}
		messages.Clear();
		for (int num = targetContainer.transform.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(targetContainer.transform.GetChild(num).gameObject);
		}
	}

	private void AddMessage(string message)
	{
		if (messages.Count >= maxMessages && messages.Count > 0)
		{
			GameObject obj = messages.First();
			messages.RemoveAt(0);
			Object.Destroy(obj);
		}
		if (messagePrefab == null || !messagePrefab.RuntimeKeyIsValid())
		{
			Debug.LogWarning(base.gameObject.name + ": Missing prefab for console message");
			return;
		}
		ConsoleMessage consoleMessage = InstantiateAddressable<ConsoleMessage>(messagePrefab, targetContainer, "ConsoleMessage");
		consoleMessage.SetMessage(message);
		messages.Add(consoleMessage.gameObject);
	}

	private static T InstantiateAddressable<T>(AssetReference assetReference, GameObject parent, string name)
	{
		GameObject obj = assetReference.InstantiateAsync(parent.transform).WaitForCompletion();
		obj.name = name;
		obj.SetActive(value: true);
		return obj.GetComponent<T>();
	}
}
