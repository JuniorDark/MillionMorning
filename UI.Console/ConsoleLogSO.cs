using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Console;

[CreateAssetMenu(fileName = "ConsoleLog", menuName = "Console/Add Console Log")]
public class ConsoleLogSO : ScriptableObject
{
	[SerializeField]
	private List<string> messageList;

	[SerializeField]
	private int maxMessages;

	public event Action<string> OnMessageAdded;

	private void OnEnable()
	{
		messageList.Clear();
	}

	private void OnDisable()
	{
		messageList.Clear();
	}

	public void Add(string message)
	{
		if (messageList.Count >= maxMessages)
		{
			messageList.RemoveAt(0);
		}
		messageList.Add(message);
		this.OnMessageAdded?.Invoke(message);
	}

	public List<string> Get()
	{
		return messageList;
	}
}
