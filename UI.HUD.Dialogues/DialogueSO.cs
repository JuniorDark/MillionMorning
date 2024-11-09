using System.Collections.Generic;
using Core.Utilities;
using UnityEngine;

namespace UI.HUD.Dialogues;

public abstract class DialogueSO : ScriptableObject, IHasPriorityKey
{
	protected DialogueWindow DialogueWindow;

	protected IDialogueUser DialogueUser;

	public abstract string GetAddressableKey();

	public abstract int GetPriority();

	public virtual bool Equals(IHasPriorityKey otherSO)
	{
		return this == otherSO;
	}

	public void SetWindow(DialogueWindow dialogueWindow)
	{
		DialogueWindow = dialogueWindow;
	}

	public void SetUser(IDialogueUser dialogueUser)
	{
		DialogueUser = dialogueUser;
	}

	public virtual List<DialogueButtonInfo> GetActions()
	{
		return new List<DialogueButtonInfo>();
	}

	public virtual bool CanBeShown()
	{
		if (DialogueUser != null)
		{
			if (!DialogueUser.IsTooHappy)
			{
				return !DialogueUser.InCombat;
			}
			return false;
		}
		return true;
	}
}
