using System.Threading.Tasks;
using Code.World.Feeds;
using Code.World.Player;
using Core;
using Core.GameEvent;
using Core.Utilities;
using Player;
using UI.HUD.Dialogues.NPC;
using UnityEngine;

namespace UI.HUD.Dialogues;

public class DialogueManager : Singleton<DialogueManager>
{
	public enum FeedTypes
	{
		Generic,
		Medal,
		Box,
		Converter,
		Gift,
		Invite,
		InviteReward,
		Item,
		Level,
		Tutorial
	}

	[Header("Current dialogue")]
	[SerializeField]
	private DialogueWindow currentWindow;

	private IDialogueUser _dialogueUser;

	private readonly MilMoPrioQueue<DialogueWindow> _queue = new MilMoPrioQueue<DialogueWindow>();

	private DialogueWindow _hasHigher;

	public void Initialize()
	{
		DestroyChildren();
		base.gameObject.SetActive(value: true);
	}

	protected void Awake()
	{
		GameEvent.SpawnDialogueEvent.RegisterAction(AddDialogue);
		GameEvent.ShowHUDEvent.RegisterAction(Pause);
	}

	private void Start()
	{
		_dialogueUser = MilMo_Player.Instance;
	}

	private void Pause(bool shouldShow)
	{
		base.gameObject.SetActive(shouldShow);
	}

	protected void OnDestroy()
	{
		GameEvent.SpawnDialogueEvent.UnregisterAction(AddDialogue);
		GameEvent.ShowHUDEvent.UnregisterAction(Pause);
	}

	private void Update()
	{
		CheckQueue();
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

	private void AddDialogue(DialogueWindow dialogueWindow)
	{
		if (!(dialogueWindow == null))
		{
			if ((object)dialogueWindow == currentWindow)
			{
				Debug.LogError("AddDialogue: CURRENT IS SAME");
				DestroyDialogue(dialogueWindow);
			}
			else if (_queue.Contains(dialogueWindow))
			{
				Debug.LogError("AddDialogue: EXIST IN QUEUE");
				DestroyDialogue(dialogueWindow);
			}
			else if (currentWindow != null && currentWindow.Equals(dialogueWindow))
			{
				Debug.LogError("AddDialogue: CURRENT EQUALS");
				DestroyDialogue(dialogueWindow);
			}
			else
			{
				dialogueWindow.SetUser(_dialogueUser);
				_queue.Enqueue(dialogueWindow);
			}
		}
	}

	private void DestroyDialogue(DialogueWindow dialogueWindow)
	{
		Object.Destroy(dialogueWindow.gameObject);
	}

	private void CheckQueue()
	{
		if (_queue.IsEmpty() || !_queue.Peek().CanBeShown())
		{
			return;
		}
		if (MilMo_GameDialogCreator.IsShowingStuff())
		{
			MilMo_GameDialogCreator.CloseAll();
			return;
		}
		Singleton<ThinkManager>.Instance.ClearHead();
		if (currentWindow != null && _queue.HasHigherPriority(currentWindow))
		{
			Debug.LogWarning("Tossing current window back into queue...");
			DialogueWindow dialogueWindow = currentWindow;
			dialogueWindow.gameObject.SetActive(value: false);
			currentWindow = null;
			AddDialogue(dialogueWindow);
		}
		else if (!(currentWindow != null))
		{
			ShowNextDialogue();
		}
	}

	private void ShowNextDialogue()
	{
		if (!_queue.IsEmpty())
		{
			currentWindow = _queue.Pop();
			if (currentWindow == null)
			{
				Debug.LogWarning("CurrentWindow is null");
			}
			else
			{
				SpawnDialogueAsync(currentWindow);
			}
		}
	}

	private async void SpawnDialogueAsync(DialogueWindow dialogueWindow)
	{
		UpdatePlayerDialogueState(inDialogue: true);
		await Task.Delay(200);
		dialogueWindow.Show();
	}

	public void Close(DialogueWindow dialogueWindow)
	{
		if (dialogueWindow == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Wanted to close dialogue that is null");
		}
		if ((object)dialogueWindow != currentWindow)
		{
			Debug.LogWarning(base.gameObject.name + ": Wanted to close dialogue that isn't current");
		}
		if (_queue.IsEmpty())
		{
			UpdatePlayerDialogueState(inDialogue: false);
		}
		currentWindow = null;
	}

	private void UpdatePlayerDialogueState(bool inDialogue)
	{
		if (_dialogueUser != null)
		{
			NPCDialogueWindow nPCDialogueWindow = currentWindow as NPCDialogueWindow;
			bool isTalking = inDialogue && nPCDialogueWindow != null;
			_dialogueUser.UpdateInDialogue(inDialogue);
			_dialogueUser.UpdateIsTalking(isTalking);
		}
	}
}
