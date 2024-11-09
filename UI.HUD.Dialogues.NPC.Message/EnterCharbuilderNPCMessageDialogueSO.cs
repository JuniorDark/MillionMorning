using System.Collections.Generic;
using Code.World.Player;
using Localization;

namespace UI.HUD.Dialogues.NPC.Message;

public class EnterCharbuilderNPCMessageDialogueSO : NPCMessageDialogueSO
{
	private int _npcId;

	public override void Init(NPCMessageData npcMessageData)
	{
		base.Init(npcMessageData);
		_npcId = npcMessageData.GetNpcId();
	}

	private void Confirm()
	{
		MilMo_Player.Instance.RequestEnterCharBuilder(_npcId);
		DialogueWindow.Close();
	}

	private void Cancel()
	{
		DialogueWindow.Close();
	}

	public override List<DialogueButtonInfo> GetActions()
	{
		return new List<DialogueButtonInfo>
		{
			new DialogueButtonInfo(Cancel, new LocalizedStringWithArgument("Generic_Later")),
			new DialogueButtonInfo(Confirm, new LocalizedStringWithArgument("Generic_Yes"), isDefault: true)
		};
	}
}
