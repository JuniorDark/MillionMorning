using UnityEngine;

namespace Code.Core.Utility;

public static class MilMo_Time
{
	public static float GetCurrentTime()
	{
		return Time.realtimeSinceStartup;
	}
}
