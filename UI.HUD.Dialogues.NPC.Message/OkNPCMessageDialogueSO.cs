using System.Collections.Generic;
using Localization;
using UnityEngine;

namespace UI.HUD.Dialogues.NPC.Message;

[CreateAssetMenu(menuName = "Dialogues/OkNPCDialogueSO", fileName = "OkNPCDialogueSO")]
public class OkNPCMessageDialogueSO : NPCMessageDialogueSO
{
	private void Confirm()
	{
		DialogueWindow.Close();
	}

	public override List<DialogueButtonInfo> GetActions()
	{
		return new List<DialogueButtonInfo>
		{
			new DialogueButtonInfo(Confirm, new LocalizedStringWithArgument("Generic_OK"), isDefault: true)
		};
	}
}
