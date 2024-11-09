using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Core.Utilities;
using UnityEngine;

namespace UI.HUD.QuickInfo;

public class QuickInfoManager : Singleton<QuickInfoManager>
{
	[Header("Current dialogue")]
	[SerializeField]
	private QuickInfoSO currentSO;

	private readonly Queue<QuickInfoSO> _queue = new Queue<QuickInfoSO>();

	private readonly Dictionary<QuickInfoSO, QuickInfoWindow> _availableWindows = new Dictionary<QuickInfoSO, QuickInfoWindow>();

	public void Initialize()
	{
		DestroyChildren();
		base.gameObject.SetActive(value: true);
	}

	private void Update()
	{
		CheckQueue();
	}

	private void CheckQueue()
	{
		if (_queue.Count >= 1 && !(currentSO != null))
		{
			ShowNextDialogue();
		}
	}

	private void ShowNextDialogue()
	{
		Debug.Log(base.gameObject.name + ": ShowNextDialogue");
		if (_queue.Count >= 1)
		{
			currentSO = _queue.Dequeue();
			if (!(currentSO == null))
			{
				ShowQuickInfo(currentSO);
			}
		}
	}

	private async void ShowQuickInfo(QuickInfoSO dialogueSO)
	{
		Debug.Log(base.gameObject.name + ": ShowQuickInfo");
		QuickInfoWindow quickInfoWindow = Instantiator.Instantiate<QuickInfoWindow>(dialogueSO.GetAddressableKey(), Singleton<QuickInfoManager>.Instance.transform);
		quickInfoWindow.gameObject.SetActive(value: false);
		quickInfoWindow.Init(dialogueSO);
		await Task.Delay(200);
		_availableWindows.Add(dialogueSO, quickInfoWindow);
		quickInfoWindow.Show();
	}

	public void AddQuickInfo(QuickInfoSO so)
	{
		Debug.Log(base.gameObject.name + ": AddQuickInfo");
		if (!(so == null))
		{
			if (_availableWindows.ContainsKey(so))
			{
				Debug.LogWarning(base.gameObject.name + ": Already exists");
			}
			else if (so.Equals(currentSO))
			{
				Debug.LogWarning(base.gameObject.name + ": Already exists && is currentSO");
			}
			else
			{
				_queue.Enqueue(so);
			}
		}
	}

	public void Close(QuickInfoSO quickInfo)
	{
		Debug.Log(base.gameObject.name + ": Close");
		if (!_availableWindows.ContainsKey(quickInfo))
		{
			Debug.LogWarning(base.gameObject.name + ": Trying to close unknown QuickInfo");
			return;
		}
		_availableWindows.TryGetValue(quickInfo, out var value);
		_availableWindows.Remove(quickInfo);
		DestroyWindow(value);
		currentSO = null;
	}

	private void DestroyChildren()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if ((bool)child)
			{
				Object.Destroy(child.gameObject);
			}
		}
	}

	private void DestroyWindow(QuickInfoWindow dialogueWindow)
	{
		Debug.Log(base.gameObject.name + ": DestroyWindow");
		Object.Destroy(dialogueWindow.gameObject);
	}
}
