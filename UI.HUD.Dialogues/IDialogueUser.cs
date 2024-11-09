namespace UI.HUD.Dialogues;

public interface IDialogueUser
{
	bool InCombat { get; }

	bool InDialogue { get; }

	bool IsTalking { get; }

	bool IsTooHappy { get; }

	void UpdateInDialogue(bool inDialogue);

	void UpdateIsTalking(bool isTalking);
}
