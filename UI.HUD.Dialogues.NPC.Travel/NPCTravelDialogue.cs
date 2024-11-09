using Code.World.Player;
using UnityEngine;

namespace UI.HUD.Dialogues.NPC.Travel;

public class NPCTravelDialogue : NPCDialogueWindow
{
	[Header("Dialogue SO")]
	[SerializeField]
	protected NPCTravelDialogueSO npcTravelDialogueSO;

	public override void Init(DialogueSO so)
	{
		npcTravelDialogueSO = (NPCTravelDialogueSO)so;
		if (npcTravelDialogueSO == null)
		{
			Debug.LogError(base.name + ": DialogueSO is of wrong type");
		}
		else
		{
			base.Init(so);
		}
	}

	protected override void RefreshActions()
	{
		if (npcTravelDialogueSO == null)
		{
			return;
		}
		ClearButtons();
		foreach (TravelInfo item in npcTravelDialogueSO.GetTravelInfo())
		{
			AddButton(item);
		}
	}
}
