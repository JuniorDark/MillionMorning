using System.Collections.Generic;
using Localization;
using UnityEngine;

namespace UI.HUD.Dialogues.NPC.Message;

public class NPCMessageDialogue : NPCDialogueWindow
{
	[Header("Dialogue SO")]
	[SerializeField]
	protected NPCMessageDialogueSO npcMessageDialogueSO;

	public override void Init(DialogueSO so)
	{
		npcMessageDialogueSO = (NPCMessageDialogueSO)so;
		if (npcMessageDialogueSO == null)
		{
			Debug.LogError(base.name + ": DialogueSO is of wrong type " + so.GetType().Name);
		}
		else
		{
			base.Init(so);
		}
	}

	protected override void RefreshActions()
	{
		if (!(npcMessageDialogueSO == null))
		{
			SetButtons(npcMessageDialogueSO.HasMoreMessages() ? GetNextButton() : GetActions());
		}
	}

	private List<DialogueButtonInfo> GetActions()
	{
		return npcMessageDialogueSO.GetActions();
	}

	private List<DialogueButtonInfo> GetNextButton()
	{
		return new List<DialogueButtonInfo>
		{
			new DialogueButtonInfo(base.ShowNextMessage, new LocalizedStringWithArgument("Generic_Next"), isDefault: true)
		};
	}
}
