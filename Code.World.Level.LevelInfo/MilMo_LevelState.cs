using Code.Core.Network;
using Core;

namespace Code.World.Level.LevelInfo;

public class MilMo_LevelState
{
	public string FullLevelName { get; }

	public bool GUIUnlocked { get; private set; }

	public MilMo_LevelState(string fullLevelName, bool guiUnlocked)
	{
		FullLevelName = fullLevelName;
		GUIUnlocked = guiUnlocked;
	}

	public void SetGUIUnlocked()
	{
		GUIUnlocked = true;
		Singleton<GameNetwork>.Instance.SendLevelTravelGUIUnlocked(FullLevelName);
	}
}
