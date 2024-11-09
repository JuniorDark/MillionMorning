using Code.Core.EventSystem;

namespace Code.World.Chat;

public static class MilMo_ChatCommandHandler
{
	public static bool HandleCommand(string command)
	{
		if (command == "/afk")
		{
			MilMo_EventSystem.At(2f, delegate
			{
				MilMo_EventSystem.Instance.PostEvent("set_afk", null);
			});
			return true;
		}
		return false;
	}
}
