using System;
using Code.Core.Network;
using Code.World.Player;
using Core;
using UI.HUD.Dialogues.ClassChoice.Abilities;
using UnityEngine;

namespace UI.HUD.Dialogues.ClassChoice;

[CreateAssetMenu(menuName = "Dialogues/ClassChoice", fileName = "ClassChoiceDialogueSO", order = 0)]
public class ClassChoiceDialogueSO : DialogueSO
{
	[SerializeField]
	private int level;

	public void Init(int value)
	{
		level = value;
	}

	public int GetLevel()
	{
		return level;
	}

	public void TellServer(int choice)
	{
		if (DialogueUser is MilMo_Player milMo_Player)
		{
			Singleton<GameNetwork>.Instance.SendClassSelectionRequest(Enum.GetName(typeof(ClassType), choice), (sbyte)level);
			milMo_Player.PlayerClassManager.DisableSelection(level);
			milMo_Player.PlayerClassManager.CheckForAvailableSelections();
		}
	}

	public override string GetAddressableKey()
	{
		return "ClassChoiceDialogueWindow";
	}

	public override int GetPriority()
	{
		return 3;
	}
}
