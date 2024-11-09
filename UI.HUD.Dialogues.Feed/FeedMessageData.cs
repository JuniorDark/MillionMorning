using System.Collections.Generic;

namespace UI.HUD.Dialogues.Feed;

public class FeedMessageData
{
	private readonly List<DialogueButtonInfo> _actions;

	public FeedMessageData(List<DialogueButtonInfo> actions)
	{
		_actions = actions;
	}

	public List<DialogueButtonInfo> GetActions()
	{
		return _actions;
	}
}
