using System;

namespace Code.World.Tutorial;

public interface IMilMo_Tutorial
{
	Action OnCloseTriggered { get; set; }

	ITutorialData GetTemplate();

	string GetIdentifier();

	void RemoveCloseTriggers();
}
