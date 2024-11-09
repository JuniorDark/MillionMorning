using System.Collections.Generic;
using Code.Core.Network;
using Code.World.Level;
using Core;
using Localization;

namespace UI.HUD.Dialogues.NPC.Message;

public class OfferQuestNPCMessageDialogueSO : NPCMessageDialogueSO
{
	private void Confirm()
	{
		Singleton<GameNetwork>.Instance.SendAcceptQuestMessage();
		DialogueWindow.Close();
	}

	private void Cancel()
	{
		Singleton<GameNetwork>.Instance.SendRejectQuestMessage();
		DialogueWindow.Close();
	}

	public override List<DialogueButtonInfo> GetActions()
	{
		List<DialogueButtonInfo> list = new List<DialogueButtonInfo>();
		MilMo_Level currentLevel = MilMo_Level.CurrentLevel;
		if (currentLevel == null || !currentLevel.IsStarterLevel())
		{
			list.Add(new DialogueButtonInfo(Cancel, new LocalizedStringWithArgument("Generic_Cancel")));
		}
		list.Add(new DialogueButtonInfo(Confirm, new LocalizedStringWithArgument("Generic_OK"), isDefault: true));
		return list;
	}
}
